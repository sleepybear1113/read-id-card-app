namespace ReadIdCardApp.db;

public class SignIn : BaseDao {
    public int ScheduleId { get; set; }
    public int PeopleId { get; set; }
    public string? PeopleNo { get; set; }
    public string? PeopleName { get; set; }
    public string? IdCard { get; set; }
    public string? Phone { get; set; }
    public int Type { get; set; }
    public long SignInTime { get; set; }

    public override string CreateTableSql() {
        return $@"CREATE TABLE IF NOT EXISTS {TableName()} (
                    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    modify_time INTEGER,
                    schedule_id INTEGER,
                    people_id INTEGER,
                    type INTEGER,
                    sign_in_time INTEGER
                )";
    }
    
    public override string TableName() {
        return "sign_in";
    }
    
    public override int StartId() {
        return 40000;
    }
}