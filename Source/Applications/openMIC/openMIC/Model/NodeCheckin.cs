using System;
using System.ComponentModel;
using GSF.Data.Model;

namespace openMIC.Model;

/// <summary>
/// Represents a checkin with a polling node in the openMIC cluster.
/// </summary>
public class NodeCheckin
{
    [PrimaryKey(true)]
    public int ID
    {
        get;
        set;
    }

    public string Url
    {
        get;
        set;
    }

    public string Task
    {
        get;
        set;
    }

    [FieldDataType(System.Data.DbType.DateTime2)]
    public DateTime LastCheckin
    {
        get;
        set;
    }

    public string FailureReason
    { 
        get;
        set;
    }

    [DefaultValue(0)]
    public int TasksQueued
    {
        get;
        set;
    }
}

public static partial class TableOperationsExtensions
{
    public static void Upsert(this TableOperations<NodeCheckin> table, NodeCheckin record)
    {
        if (record.ID != 0)
        {
            table.SafeUpdateByID(record);
            return;
        }

        table.SafeInsert(record);
        table.SafeUpdateByUniqueKey(record);
    }

    private static void SafeInsert(this TableOperations<NodeCheckin> table, NodeCheckin record)
    {
        string fromClause = table.Connection.IsOracle
            ? "FROM dual "
            : "";

        string insertQueryFormat =
            "INSERT INTO NodeCheckin(Url, Task, LastCheckin, FailureReason, TasksQueued) " +
            "SELECT {0} Url, {1} Task, {2} LastCheckin, {3} FailureReason, {4} TasksQueued " +
            fromClause +
            "WHERE NOT EXISTS(SELECT * FROM NodeCheckin WHERE Url = {0} AND Task = {1})";

        table.Connection.ExecuteNonQuery(insertQueryFormat,
            record.Url, record.Task, record.LastCheckin, record.FailureReason, record.TasksQueued);
    }

    private static void SafeUpdateByID(this TableOperations<NodeCheckin> table, NodeCheckin record)
    {
        const string UpdateQueryFormat =
            """
            UPDATE NodeCheckin
            SET
                Url = {1},
                Task = {2},
                LastCheckin = {3},
                FailureReason {4},
                TasksQueued = {5}
            WHERE ID = {0}
            """;

        table.Connection.ExecuteNonQuery(UpdateQueryFormat, record.ID,
            record.Url, record.Task, record.LastCheckin, record.FailureReason, record.TasksQueued);
    }

    private static void SafeUpdateByUniqueKey(this TableOperations<NodeCheckin> table, NodeCheckin record)
    {
        const string UpdateQueryFormat =
            """
            UPDATE NodeCheckin
            SET
                LastCheckin = {2},
                FailureReason = {3},
                TasksQueued = {4}
            WHERE Url = {0} AND Task = {1}
            """;

        table.Connection.ExecuteNonQuery(UpdateQueryFormat, record.Url, record.Task,
            record.LastCheckin, record.FailureReason, record.TasksQueued);
    }
}