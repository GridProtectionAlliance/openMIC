//******************************************************************************************************
//  ConnectionProfileTaskQueue.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/05/2017 - Stephen C Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using GSF.ComponentModel;
using GSF.Data.Model;
using GSF.Threading;

namespace openMIC.Model
{
    public class ConnectionProfileTaskQueue
    {
        #region [ Members ]

        // Nested Types
        private class LogicalThreadQueue
        {
            public LogicalThreadScheduler Scheduler { get; }
            public LogicalThread QueuingThread { get; }

            public LogicalThreadQueue()
            {
                Scheduler = new LogicalThreadScheduler();
                QueuingThread = Scheduler.CreateThread(2);
            }
        }

        // Constants
        private const int NormalPriority = 1;
        private const int HighPriority = 2;

        // Fields
        private string m_name;
        private LogicalThreadQueue m_threadQueue;
        private LogicalThread m_taskThread;
        private ICancellationToken m_cancellationToken;

        #endregion

        #region [ Properties ]

        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
                m_threadQueue = null;
            }
        }

        [Required]
        public int MaxThreadCount
        {
            get;
            set;
        }

        public bool UseBackgroundThreads
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        [DefaultValueExpression("DateTime.UtcNow")]
        public DateTime CreatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [DefaultValueExpression("UserInfo.CurrentUserID")]
        public string CreatedBy
        {
            get;
            set;
        }

        [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
        [UpdateValueExpression("DateTime.UtcNow")]
        public DateTime UpdatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
        [UpdateValueExpression("UserInfo.CurrentUserID")]
        public string UpdatedBy
        {
            get;
            set;
        }

        private LogicalThreadQueue ThreadQueue
        {
            get
            {
                return m_threadQueue ?? (m_threadQueue = GetThreadQueue());
            }
        }

        private LogicalThread TaskThread
        {
            get
            {
                return m_taskThread ?? (m_taskThread = ThreadQueue.Scheduler.CreateThread());
            }
        }

        #endregion

        #region [ Methods ]

        public bool QueueAction(Action action)
        {
            ICancellationToken cancellationToken = new GSF.Threading.CancellationToken();

            if (Interlocked.CompareExchange(ref m_cancellationToken, cancellationToken, null) != null)
                return false;

            ThreadQueue.QueuingThread.Push(NormalPriority, () =>
            {
                if (!cancellationToken.Cancel())
                    return;

                TaskThread.Push(() =>
                {
                    action();
                    Interlocked.CompareExchange(ref m_cancellationToken, null, cancellationToken);
                });
            });

            return true;
        }

        public bool PrioritizeAction(Action action)
        {
            ICancellationToken priorityToken = new GSF.Threading.CancellationToken();
            ICancellationToken normalToken = null;
            bool cancelled = false;

            priorityToken.Cancel();

            while (Interlocked.CompareExchange(ref m_cancellationToken, priorityToken, normalToken) != normalToken)
            {
                normalToken = Interlocked.CompareExchange(ref m_cancellationToken, null, null);
                cancelled = normalToken?.Cancel() ?? true;

                if (!cancelled)
                    return false;
            }

            ThreadQueue.QueuingThread.Push(HighPriority, () =>
            {
                TaskThread.Push(() =>
                {
                    action();
                    Interlocked.Exchange(ref m_cancellationToken, null);
                });
            });

            return true;
        }

        public void RegisterExceptionHandler(Action<Exception> exceptionHandler)
        {
            TaskThread.UnhandledException += (sender, args) => exceptionHandler(args.Argument);
        }

        private LogicalThreadQueue GetThreadQueue()
        {
            LogicalThreadQueue threadQueue = s_threadSchedulers.GetOrAdd(Name, name => new LogicalThreadQueue());
            threadQueue.Scheduler.MaxThreadCount = (MaxThreadCount > 0) ? MaxThreadCount : Environment.ProcessorCount;
            threadQueue.Scheduler.UseBackgroundThreads = UseBackgroundThreads;
            return threadQueue;
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly ConcurrentDictionary<string, LogicalThreadQueue> s_threadSchedulers = new ConcurrentDictionary<string, LogicalThreadQueue>();

        #endregion
    }
}