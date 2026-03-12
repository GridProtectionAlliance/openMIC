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
        if (record.ID == 0)
            table.SafeInsert(record);

        table.SafeUpdate(record);
    }

    private static void SafeInsert(this TableOperations<NodeCheckin> table, NodeCheckin record)
    {
        string fromClause = table.Connection.IsOracle
            ? "FROM dual "
            : "";

        string insertQueryFormat =
            "INSERT INTO NodeCheckin(Url, LastCheckin, FailureReason, TasksQueued) " +
            "SELECT {0} Url, {1} LastCheckin, {2} FailureReason, 0 TasksQueued " +
            fromClause +
            "WHERE NOT EXISTS(SELECT * FROM NodeCheckin WHERE Url = {0})";

        table.Connection.ExecuteNonQuery(insertQueryFormat,
            record.Url, record.LastCheckin, record.FailureReason);
    }

    private static void SafeUpdate(this TableOperations<NodeCheckin> table, NodeCheckin record)
    {
        string whereClause = record.ID != 0
            ? "WHERE ID = {0}"
            : "WHERE Url = {0}";

        string updateQueryFormat =
            "UPDATE NodeCheckin " +
            "SET " +
            "    LastCheckin = CASE WHEN LastCheckin < {1} THEN {1} ELSE LastCheckin END, " +
            "    FailureReason CASE WHEN LastCheckin < {1} THEN {2} ELSE FailureReason END, " +
            "    TasksQueued = TasksQueued + {3} " +
            whereClause;

        object key = record.ID != 0
            ? record.ID
            : record.Url;

        table.Connection.ExecuteNonQuery(updateQueryFormat,
            key, record.LastCheckin, record.FailureReason, record.TasksQueued);
    }
}