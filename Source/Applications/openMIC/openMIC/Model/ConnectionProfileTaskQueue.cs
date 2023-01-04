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
using System.Linq;
using GSF.ComponentModel;
using GSF.Data.Model;
using GSF.Threading;

namespace openMIC.Model;

public class ConnectionProfileTaskQueue
{
#region [ Members ]

    // Fields
    private string m_name;
    private LogicalThreadScheduler m_threadScheduler;
    private LogicalThread m_taskThread;

#endregion

#region [ Properties ]

    [PrimaryKey(true)]
    public int ID { get; set; }

    [Required]
    [StringLength(200)]
    public string Name
    {
        get => m_name;
        set
        {
            m_name = value;
            m_threadScheduler = null;
        }
    }

    [Required]
    public int MaxThreadCount { get; set; }

    public bool UseBackgroundThreads { get; set; }

    public string Description { get; set; }

    [DefaultValueExpression("DateTime.UtcNow")]
    public DateTime CreatedOn { get; set; }

    [Required]
    [StringLength(200)]
    [DefaultValueExpression("UserInfo.CurrentUserID")]
    public string CreatedBy { get; set; }

    [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
    [UpdateValueExpression("DateTime.UtcNow")]
    public DateTime UpdatedOn { get; set; }

    [Required]
    [StringLength(200)]
    [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
    [UpdateValueExpression("UserInfo.CurrentUserID")]
    public string UpdatedBy { get; set; }

    private LogicalThreadScheduler ThreadScheduler => m_threadScheduler ??= GetThreadScheduler();

    private LogicalThread TaskThread => m_taskThread ??= ThreadScheduler.CreateThread();

#endregion

#region [ Methods ]

    public void QueueAction(Action action, QueuePriority priority) => TaskThread.Push((int)priority, action);

    public void RegisterExceptionHandler(Action<Exception> exceptionHandler) => TaskThread.UnhandledException += (sender, args) => exceptionHandler(args.Argument);

    private LogicalThreadScheduler GetThreadScheduler()
    {
        LogicalThreadScheduler threadScheduler = s_threadSchedulers.GetOrAdd(Name, _ => new LogicalThreadScheduler(s_priorityLevels));
        threadScheduler.MaxThreadCount = MaxThreadCount <= 0 ? Environment.ProcessorCount : MaxThreadCount;
        threadScheduler.UseBackgroundThreads = UseBackgroundThreads;
        return threadScheduler;
    }

#endregion

#region [ Static ]

    // Static Fields
    private static readonly ConcurrentDictionary<string, LogicalThreadScheduler> s_threadSchedulers = new();
    private static readonly int s_priorityLevels = Enum.GetValues(typeof(QueuePriority)).Cast<int>().Max();

#endregion
}