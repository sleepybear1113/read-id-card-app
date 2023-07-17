using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Dapper;
using ReadIdCardApp.excel;
using ReadIdCardApp.utils;

namespace ReadIdCardApp.db;

public class People : BaseDao {
    private string? idCard;
    public int? ScheduleId { get; set; }
    public string? ScheduleName { get; set; }
    public string? No { get; set; }
    public string? Name { get; set; }

    public string? IdCard {
        get => idCard;
        set {
            idCard = value;
            int type = Util.ValidateIdCard(idCard);
            IdCardValid = "";
            switch (type) {
                case -1:
                    IdCardValid = "身份证为空;";
                    break;
                case -2:
                    IdCardValid = "非18位身份证格式;";
                    break;
                case -3:
                    IdCardValid = "第18位校验码不符";
                    break;
            }
        }
    }

    public string? Area { get; set; }
    public string? Work { get; set; }
    public string? Phone { get; set; }
    public string? Col1 { get; set; }
    public string? Col2 { get; set; }
    public string? Col3 { get; set; }

    public string? ErrMsg { get; set; }
    public string? WarnMsg { get; set; }

    public string? IdCardValid { get; set; }

    public static List<string?> ColNames() {
        return new List<string?>() { "场次id", "场次名称", "编号", "姓名", "身份证号", "地区", "单位", "手机", "Col1", "Col2", "Col3" };
    }

    public override string CreateTableSql() {
        return $@"CREATE TABLE IF NOT EXISTS {TableName()} (
                id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                modify_time INTEGER,
                schedule_id INTEGER,
                no TEXT, -- 自定义编号
                name TEXT,
                id_card TEXT,
                area TEXT, -- 地区
                work TEXT, -- 工作地点
                phone TEXT,
                col1 TEXT, -- 保留字段 1
                col2 TEXT, -- 保留字段 2
                col3 TEXT -- 保留字段 3
            )";
    }

    public override string TableName() {
        return "people";
    }

    public override int StartId() {
        return 1000;
    }

    public int CountAll(int? scheduleId) {
        return CountAll(new List<int?> { scheduleId });
    }

    public int CountAll(List<int?>? scheduleIdList) {
        string sql = $@"SELECT COUNT(*) FROM {TableName()}";
        if (scheduleIdList != null) {
            string inClause = string.Join(",", scheduleIdList);
            sql += $" WHERE schedule_id IN ({inClause})";
        }

        return DbUtil.Connection.ExecuteScalar<int>(sql);
    }

    public int? InsertToDb() {
        FillModifyTime();
        string insertSql =
            $@"INSERT INTO {TableName()} (modify_time, schedule_id, no, name, id_card, area, work, phone, col1, col2, col3)
                          VALUES (@ModifyTime, @ScheduleId, @No, @Name, @IdCard, @Area, @Work, @Phone, @Col1, @Col2, @Col3);
                          SELECT last_insert_rowid();";
        return DbUtil.Connection.QuerySingleOrDefault<int?>(insertSql, this);
    }

    public void UpdateDb() {
        FillModifyTime();
        string updateSql =
            $@"UPDATE {TableName()} SET schedule_id = @ScheduleId, no = @No, name = @Name, id_card = @IdCard, area = @Area, work = @Work, phone = @Phone, col1 = @Col1, col2 = @Col2, col3 = @Col3, modify_time = @ModifyTime
                          WHERE id = @Id";
        DbUtil.Connection.Execute(updateSql, this);
    }

    public void DeleteFromDb() {
        if (Id == null) {
            return;
        }

        string sql = $@"DELETE FROM {TableName()} WHERE id = @Id";
        DbUtil.Connection.Execute(sql, new { Id });
    }

    public int DeleteScheduleIdFromDb(List<int?>? scheduleIdList) {
        string sql = $@"DELETE FROM {TableName()}";
        if (scheduleIdList != null) {
            string inClause = string.Join(",", scheduleIdList);
            sql += $" where schedule_id IN ({inClause})";
        }

        int rows = DbUtil.Connection.Execute(sql);
        return rows;
    }

    public int DeleteScheduleIdFromDb(int? scheduleId) {
        return DeleteScheduleIdFromDb(new List<int?> { scheduleId });
    }

    public int DeleteAllFromDb() {
        string sql = $@"DELETE FROM {TableName()}";
        int rows = DbUtil.Connection.Execute(sql);
        return rows;
    }

    public List<People> SelectByIds(List<int>? ids) {
        string cols =
            "id, modify_time as ModifyTime, schedule_id as ScheduleId, no as No, name, id_card as IdCard, area, work, phone, col1 as Col1, col2 as Col2, col3 as Col3";

        List<People> peopleList;
        if (ids == null || ids.Count == 0) {
            string sql = $"SELECT {cols} FROM {TableName()}";
            peopleList = DbUtil.Connection.Query<People>(sql).ToList();
        } else {
            string inClause = string.Join(",", ids);
            string sql = $"SELECT {cols} FROM {TableName()} WHERE id IN ({inClause})";
            peopleList = DbUtil.Connection.Query<People>(sql).ToList();
        }

        FillPeopleInfo(peopleList);
        return peopleList;
    }

    public void FillPeopleInfo(List<People>? peopleList) {
        if (peopleList == null) {
            return;
        }

        List<int?> scheduleIds = new List<int?>();
        foreach (var people in peopleList) {
            scheduleIds.Add(people.ScheduleId);
        }

        var selectedScheduleIds =
            new Schedule().SelectByIds(scheduleIds).Where(schedule => schedule.Id != null).ToList();
        Dictionary<int, Schedule> dictionary = selectedScheduleIds.ToDictionary(schedule => (int)schedule.Id);
        foreach (var people in peopleList) {
            var peopleId = people.Id;
            if (peopleId == null || !dictionary.ContainsKey((int)people.ScheduleId)) {
                continue;
            }

            people.ScheduleName = dictionary[(int)people.ScheduleId].Name;
        }
    }

    public List<People> SelectByScheduleIds(List<int?>? scheduleIds) {
        string cols =
            "id, modify_time as ModifyTime, schedule_id as ScheduleId, no as No, name, id_card as IdCard, area, work, phone, col1 as Col1, col2 as Col2, col3 as Col3";

        List<People> peopleList;
        if (scheduleIds == null || scheduleIds.Count == 0) {
            string sql = $"SELECT {cols} FROM {TableName()}";
            peopleList = DbUtil.Connection.Query<People>(sql).ToList();
        } else {
            string inClause = string.Join(",", scheduleIds);
            string sql = $"SELECT {cols} FROM {TableName()} WHERE schedule_id IN ({inClause})";
            peopleList = DbUtil.Connection.Query<People>(sql).ToList();
        }

        FillPeopleInfo(peopleList);

        return peopleList;
    }

    public Dictionary<int, int> CountEverySchedule(List<int?>? scheduleIdList) {
        string whereSql = "";
        if (scheduleIdList != null && scheduleIdList.Count > 0) {
            whereSql = $"WHERE schedule_id in ({string.Join(",", scheduleIdList)})";
        }
        string sql = $"SELECT schedule_id, COUNT(*) AS count FROM {TableName()} {whereSql} GROUP BY schedule_id";
        IEnumerable<dynamic> results = DbUtil.Connection.Query(sql);

        Dictionary<int, int> countMap = new Dictionary<int, int>();
        foreach (var result in results) {
            dynamic id = result.schedule_id;
            dynamic count = result.count;
            countMap[Convert.ToInt32(id)] = Convert.ToInt32(count);
        }

        return countMap;
    }

    public static List<People>? BuildFromExcelDataList(ExcelData? excelData) {
        if (excelData == null) {
            return null;
        }

        var list = excelData.Get(ColNames(), null);
        List<People> res = new List<People>();
        foreach (var item in list) {
            People people = new People();
            res.Add(people);

            int index = 0;
            people.ScheduleId = null;
            people.ErrMsg = "";
            people.WarnMsg = "";
            if (!string.IsNullOrEmpty(item[index])) {
                if (Util.IsPositiveInteger(item[index])) {
                    people.ScheduleId = int.Parse(item[index]);
                } else if (item[index] == "0") {
                    people.ScheduleId = 0;
                } else {
                    people.ErrMsg += "场次id格式错误(不为正整数或者0);";
                }
            }

            index++;
            people.ScheduleName = string.IsNullOrEmpty(item[index]) ? null : item[index].Trim();
            if (people.ScheduleId == null || people.ScheduleId == 0) {
                if (string.IsNullOrEmpty(people.ScheduleName)) {
                    people.ErrMsg += "场次名称为空;";
                }
            }

            index++;
            people.No = string.IsNullOrEmpty(item[index]) ? null : item[index].Trim();
            index++;
            people.Name = string.IsNullOrEmpty(item[index]) ? null : item[index].Trim();
            index++;
            people.IdCard = string.IsNullOrEmpty(item[index]) ? null : item[index].Trim();
            if (people.IdCard == null) {
                people.ErrMsg += "身份证为空;";
            }

            index++;
            people.Area = string.IsNullOrEmpty(item[index]) ? null : item[index].Trim();
            index++;
            people.Work = string.IsNullOrEmpty(item[index]) ? null : item[index].Trim();
            index++;
            people.Phone = string.IsNullOrEmpty(item[index]) ? null : item[index].Trim();
            index++;
            people.Col1 = string.IsNullOrEmpty(item[index]) ? null : item[index].Trim();
            index++;
            people.Col2 = string.IsNullOrEmpty(item[index]) ? null : item[index].Trim();
            index++;
            people.Col3 = string.IsNullOrEmpty(item[index]) ? null : item[index].Trim();
        }

        return res;
    }
}