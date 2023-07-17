using System.Collections.Generic;
using System.Linq;
using Dapper;
using ReadIdCardApp.utils;

namespace ReadIdCardApp.db;

public class Schedule : BaseDao {
    public string Name { get; set; }

    public string IdName {
        get => $"[{Id}]-{Name}";
        set { }
    }

    public int? PeopleCount { get; set; }

    public long? TimeBegin { get; set; }
    public long? TimeEnd { get; set; }

    public string TimeBeginStr {
        get => Util.FormatTimeBegin(TimeBegin);
        set { }
    }

    public string TimeEndStr {
        get => Util.FormatTimeBegin(TimeEnd);
        set { }
    }

    public override string CreateTableSql() {
        return $@"CREATE TABLE IF NOT EXISTS {TableName()} (
                id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                modify_time INTEGER,
                name TEXT,
                time_begin INTEGER,
                time_end INTEGER
            )";
    }

    public override string TableName() {
        return "schedule";
    }

    public override int StartId() {
        return 8000;
    }

    public void UpdateDb() {
        string sql =
            $"UPDATE {TableName()} SET name = @Name, modify_time = @ModifyTime, time_begin = @TimeBegin, time_end = @TimeEnd WHERE Id = @Id";
        FillModifyTime();
        DbUtil.Connection.Execute(sql, this);
    }

    public int InsertToDb() {
        FillModifyTime();
        string insertSql = $@"INSERT INTO {TableName()} (modify_time, name, time_begin, time_end)
                     VALUES (@ModifyTime, @Name, @TimeBegin, @TimeEnd);
                     SELECT LAST_INSERT_ROWID();";
        return DbUtil.Connection.QuerySingle<int>(insertSql, this);
    }

    public void DeleteFromDb() {
        if (Id == null) {
            return;
        }

        string sql = $@"DELETE FROM {TableName()} WHERE id = @Id";
        DbUtil.Connection.Execute(sql, this);
    }

    public int DeleteAllFromDb() {
        string sql = $@"DELETE FROM {TableName()}";
        return DbUtil.Connection.Execute(sql);
    }

    public List<Schedule> SelectByIds(List<int?>? ids) {
        string cols = "id, modify_time as ModifyTime, name, time_begin as TimeBegin, time_end as TimeEnd";

        List<Schedule> schedules;
        if (ids == null || ids.Count == 0) {
            string sql = $"SELECT {cols} FROM {TableName()}";
            schedules = DbUtil.Connection.Query<Schedule>(sql).ToList();
        } else {
            ids.RemoveAll(id => id == null);
            string sql = $"SELECT {cols} FROM {TableName()} WHERE id IN @ids";
            schedules = DbUtil.Connection.Query<Schedule>(sql, new { ids }).ToList();
        }

        var scheduleSet = schedules.Select(schedule => schedule.Id).ToHashSet();
        var countScheduleMap = new People().CountEverySchedule(new List<int?>(scheduleSet));
        foreach (var schedule in schedules) {
            int id = (int)schedule.Id;
            schedule.PeopleCount = countScheduleMap.TryGetValue(id, out var value) ? value : 0;
        }

        return schedules;
    }
}