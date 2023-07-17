using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using Dapper;

namespace ReadIdCardApp.db;

public class DbUtil {
    private static readonly string DatabaseFile = "db.sqlite";
    public static SQLiteConnection Connection;

    public static void Init() {
        if (!File.Exists(DatabaseFile)) {
            File.Create(DatabaseFile).Close();
        }

        Connection = new SQLiteConnection($"Data Source={DatabaseFile};Version=3;");
        Connection.Open();

        foreach (var sql in GetAllCreateTableSql()) {
            Connection.Execute(sql);
        }
    }

    public static List<string> GetAllCreateTableSql() {
        Assembly assembly = Assembly.GetExecutingAssembly();
        List<Type> derivedDaoTypes = assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(BaseDao)))
            .ToList();

        List<string> list = new List<string>();
        foreach (Type type in derivedDaoTypes) {
            MethodInfo getSqlMethod = type.GetMethod("CreateTableSql");
            string sqlCode = (string)getSqlMethod.Invoke(Activator.CreateInstance(type), null);
            list.Add(sqlCode);

            getSqlMethod = type.GetMethod("InitPrimaryKeyId");
            sqlCode = (string)getSqlMethod.Invoke(Activator.CreateInstance(type), null);
            list.Add(sqlCode);
        }

        return list;
    }
}