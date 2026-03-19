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
    private string m_groupID;
    private ConnectionProfileTaskScheduler m_threadScheduler;
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

    [NonRecordField]
    public string GroupID
    {
        get => m_groupID;
        set
        {
            m_groupID = value;
            m_taskThread = null;
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

    private ConnectionProfileTaskScheduler ThreadScheduler => m_threadScheduler ??= GetThreadScheduler();
    private LogicalThread TaskThread => m_taskThread ??= ThreadScheduler.CreateThread(GroupID);

    #endregion

    #region [ Methods ]

    public LogicalThreadExtensions.LogicalThreadAwaitable Yield(int priority)
    {
        return TaskThread.Yield(priority);
    }

    private ConnectionProfileTaskScheduler GetThreadScheduler()
    {
        ConnectionProfileTaskScheduler threadScheduler = s_threadSchedulers.GetOrAdd(Name, _ => new ConnectionProfileTaskScheduler(s_priorityLevels));
        threadScheduler.MaxThreadCount = MaxThreadCount <= 0 ? Environment.ProcessorCount : MaxThreadCount;
        threadScheduler.UseBackgroundThreads = UseBackgroundThreads;
        return threadScheduler;
    }

    #endregion

    #region [ Static ]

    // Static Fields
    private static readonly ConcurrentDictionary<string, ConnectionProfileTaskScheduler> s_threadSchedulers = new();
    private static readonly int s_priorityLevels = Enum.GetValues(typeof(QueuePriority)).Cast<int>().Max();

    #endregion
}

public class ConnectionProfileTaskScheduler(int priorityLevels)
{
    private LogicalThreadScheduler Scheduler { get; } = new(priorityLevels);

    public int MaxThreadCount
    {
        get => Scheduler.MaxThreadCount;
        set => Scheduler.MaxThreadCount = value;
    }

    public bool UseBackgroundThreads
    {
        get => Scheduler.UseBackgroundThreads;
        set => Scheduler.UseBackgroundThreads = value;
    }

    public LogicalThread CreateThread(string groupID)
    {
        return s_threads.GetOrAdd(groupID, _ => Scheduler.CreateThread());
    }

    private static readonly ConcurrentDictionary<string, LogicalThread> s_threads = new();
}