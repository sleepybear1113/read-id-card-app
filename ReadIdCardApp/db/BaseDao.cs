using System;

namespace ReadIdCardApp.db;

public abstract class BaseDao {
    public int? Id { get; set; }
    public long ModifyTime { get; set; }

    public void FillModifyTime() {
        // 以毫秒为单位的时间戳
        ModifyTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public abstract string CreateTableSql();
    public abstract string TableName();

    public abstract int StartId();

    public string InitPrimaryKeyId() {
        return $"INSERT INTO {TableName()}(id) values ({StartId()}); DELETE FROM {TableName()} WHERE id = {StartId()}";
    }
}