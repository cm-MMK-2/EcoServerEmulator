using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CommonLib
{
    public class DatabaseManager
    {
        private static string connectionString = string.Empty;
        private static string ConnectionString {
            get {
                if(string.IsNullOrEmpty(connectionString))
                {
                    connectionString = Utilities.ReadSettings("settings.json").ConnectionString;
                }
                return connectionString;
            }
        } 

        private static async Task<int> RunAsync(Func<MySqlConnection, Task<int>> taskFunc)
        {
            try
            {
                using (var db = new MySqlConnection(ConnectionString))
                {
                    await db.OpenAsync();
                    return await taskFunc(db);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"DB Error: {e.Message}\n{e.StackTrace}");
                return -1;
            }
        }

        private static async Task<T> RunAsync<T>(Func<MySqlConnection, Task<T>> taskFunc)
        {
            try
            {
                using (var db = new MySqlConnection(ConnectionString))
                {
                    await db.OpenAsync();
                    return await taskFunc(db);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"DB Error: {e.Message}\n{e.StackTrace}");
                return default(T);
            }
        }

        /// <summary>
        /// Customizable Execute NonQuery Command
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteNonQueryAsync(string tableName, string commandText, List<MySqlParameter> parameters = null)
        {
            return await RunAsync(async (db) =>
            {
                var cmd = db.CreateCommand() as MySqlCommand;
                cmd.CommandText = commandText;
                if (parameters != null)
                {
                    foreach (var para in parameters)
                    {
                        cmd.Parameters.Add(para);
                    }
                }
                return await cmd.ExecuteNonQueryAsync();
            });
        }

        public static async Task<int> InsertAsync<T>(T obj, string tableName = null) where T : class
        {
            return await RunAsync(async (db) =>
            {
                if(tableName == null)
                {
                    tableName = typeof(T).Name;
                }
                var cmd = db.CreateCommand() as MySqlCommand;
                var properties = typeof(T).GetProperties();
                
                cmd.CommandText = $@"INSERT INTO `{tableName}` ({string.Join(", ", properties.Select(p => $"`{p.Name}`"))}) VALUES ({string.Join(", ", properties.Select(p => $"@{p.Name}"))})";
                foreach (var prop in properties)
                {
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(obj));
                }

                return await cmd.ExecuteNonQueryAsync();
            });
        }

        public static async Task<int> InsertAsync(string tableName, Dictionary<string, object> parameters)
        {
            return await RunAsync(async (db) =>
            {
                var cmd = db.CreateCommand() as MySqlCommand;
                var keys = parameters.Keys;
                cmd.CommandText = $@"INSERT INTO `{tableName}` ({string.Join(", ", keys.Select(k=> $"`{k}`"))}) VALUES ({string.Join(", ", keys.Select(k => $"@{k}"))})";
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue($"@{param.Key}", param.Value);
                }

                return await cmd.ExecuteNonQueryAsync();
            });
        }

        public static async Task<int> UpdateAsync(string tableName, Dictionary<string, Tuple<DbType, object>> parameters, int id)
        {
            return await RunAsync(async (db) =>
            {
                var cmd = db.CreateCommand() as MySqlCommand;
                var keys = parameters.Keys;
                cmd.CommandText = $@"UPDATE `{tableName}` SET {string.Join(", ", keys.Select(k => $"`{k}` = @{k}"))} WHERE `ID` = @id";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@id",
                    DbType = DbType.Int32,
                    Value = id,
                });
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = $"@{param.Key}",
                        DbType = param.Value.Item1,
                        Value = param.Value.Item2,
                    });
                }

                return await cmd.ExecuteNonQueryAsync();
            });
        }

        public static async Task<int> DeleteAsync(string tableName, int id)
        {
            return await RunAsync(async (db) =>
            {
                var cmd = db.CreateCommand() as MySqlCommand;
                cmd.CommandText = $@"DELETE FROM `{tableName}` WHERE `Id` = @{id};";
                return await cmd.ExecuteNonQueryAsync();
            });
        }

        public static async Task<T> SelectAsync<T>(string columName,string columStr, string tableName = null) where T : class
        {
            return await RunAsync(async (db) =>
            {
                var type = typeof(T);
                var properties = type.GetProperties();
                var instance = (T)Activator.CreateInstance(type);
                try
                {
                    var cmd = db.CreateCommand() as MySqlCommand;
                    if (tableName == null)
                    {
                        tableName = type.Name;
                    }
                    cmd.CommandText = $@"SELECT {string.Join(",", properties.Select(p => $"`{p.Name}`"))} FROM `{tableName}` WHERE `{columName}` = {columStr}";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var prop = type.GetProperty(reader.GetName(i));
                                if (prop != null)
                                {
                                    object value = await reader.GetFieldValueAsync<object>(i);

                                    if (value.GetType() != typeof(DBNull))
                                    {
                                        prop.SetValue(instance, value);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
                return instance;
            });
        }

        public static async Task<T> SelectAsync<T>(string command) where T : class
        {
            return await RunAsync(async (db) =>
            {
                var type = typeof(T);
                //var properties = type.GetProperties();
                var instance = (T)Activator.CreateInstance(type);
                try
                {
                    var cmd = db.CreateCommand() as MySqlCommand;
                    cmd.CommandText = command;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            for(int i = 0; i < reader.FieldCount; i++)
                            {
                                var prop = type.GetProperty(reader.GetName(i));
                                if (prop != null)
                                {
                                    object value = await reader.GetFieldValueAsync<object>(i);

                                    if (value.GetType() != typeof(DBNull))
                                    {
                                        prop.SetValue(instance, value);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
                return instance;
            });
        }

        public static async Task<uint> SelectAsync(string command)
        {
            return await RunAsync<uint>(async (db) =>
            {
                try
                {
                    var cmd = db.CreateCommand() as MySqlCommand;
                    cmd.CommandText = command;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        return (uint)reader[0];
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
                return 0;
            });
        }

        public static async Task<ushort> SelectAsyncUshort(string command)
        {
            return await RunAsync<ushort>(async (db) =>
            {
                try
                {
                    var cmd = db.CreateCommand() as MySqlCommand;
                    cmd.CommandText = command;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        return (ushort)reader[0];
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
                return 0;
            });
        }

        public static async Task<T> SelectAsync<T>(uint id, string tableName = null) where T : class
        {
            return await RunAsync(async (db) =>
            {
                var type = typeof(T);
                var properties = type.GetProperties();
                var instance = (T)Activator.CreateInstance(type);
                var cmd = db.CreateCommand() as MySqlCommand;
                if (tableName == null)
                {
                    tableName = type.Name;
                }
                cmd.CommandText = $@"SELECT {string.Join(",", properties.Select(p => $"`{p.Name}`"))} FROM {tableName} WHERE `id` = {id}";

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    await reader.ReadAsync();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var prop = type.GetProperty(reader.GetName(i));
                        if (prop != null)
                        {
                            object value = await reader.GetFieldValueAsync<object>(i);

                            if (value.GetType() != typeof(DBNull))
                            {
                                prop.SetValue(instance, value);
                            }
                        }
                    }
                }
                return instance;
            });
        }


        public static async Task<uint> SelectCountAsync(string columName, string columStr, string tableName, string countColumName = null)
        {
            return await RunAsync<uint>(async (db) =>
            {
                try
                {
                    var cmd = db.CreateCommand() as MySqlCommand;
                    cmd.CommandText = $@"SELECT COUNT({countColumName??"*"}) AS COUNT FROM {tableName} WHERE `{columName}` = {columStr}";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();

                        return (uint)reader[0];
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
                return 0;
            });
        }

        public static async Task<uint> SelectCountAsync(string command)
        {
            return await RunAsync<uint>(async (db) =>
            {
                try
                {
                    var cmd = db.CreateCommand() as MySqlCommand;
                    cmd.CommandText = command;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();

                        return (uint)reader[0];
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
                return 0;
            });
        }

        public static async Task<List<T>> SelectMultiAsync<T>(string columName, string columStr, string tableName = null)
        {
            return await RunAsync<List<T>> (async (db) =>
            {
                List<T> list = new List<T>();
                var type = typeof(T);
                var properties = type.GetProperties();
                try
                {
                    var cmd = db.CreateCommand() as MySqlCommand;
                    if (tableName == null)
                    {
                        tableName = type.Name;
                    }
                    cmd.CommandText = $@"SELECT {string.Join(",", properties.Select(p => $"`{p.Name}`"))} FROM {tableName} WHERE `{columName}` = {columStr}";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var instance = (T)Activator.CreateInstance(type);
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var prop = type.GetProperty(reader.GetName(i));
                                if (prop != null)
                                {
                                    object value = await reader.GetFieldValueAsync<object>(i);

                                    if (value.GetType() != typeof(DBNull))
                                    {
                                        prop.SetValue(instance, value);
                                    }
                                }
                            }
                            list.Add(instance);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
                return list;
            });
        }


        public static async Task<List<T>> SelectMultiAsync<T>(string command)
        {
            return await RunAsync<List<T>>(async (db) =>
            {
                List<T> list = new List<T>();
                var type = typeof(T);
                var properties = type.GetProperties();
                try
                {
                    var cmd = db.CreateCommand() as MySqlCommand;
                    cmd.CommandText = command;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var instance = (T)Activator.CreateInstance(type);
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var prop = type.GetProperty(reader.GetName(i));
                                if (prop != null)
                                {
                                    object value = await reader.GetFieldValueAsync<object>(i);

                                    if (value.GetType() != typeof(DBNull))
                                    {
                                        prop.SetValue(instance, value);
                                    }
                                }
                            }
                            list.Add(instance);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
                return list;
            });
        }
    }

    public class ExTable
    {
        public long Id { get; set; }

        [JsonIgnore]
        protected string TableName { get; set; }

        public ExTable(string tableName = null)
        {
            TableName = tableName ?? this.GetType().Name;
        }

        private void Bind(ref MySqlCommand cmd, string colName, DbType dbType, object value)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = $"@{colName}",
                DbType = dbType,
                Value = value,
            });
        }
    }
}
