using System;
using System.Collections.Generic;
using Jal123.GUI.Properties;
using MySql.Data.MySqlClient;

namespace Jal123.GUI
{
    internal class ZentaoProductData
    {
        public int Id { get; set; }

        public String Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    internal class ZentaoDbProductsExporter
    {
        private const string SqlCmd = "SELECT id,name FROM zt_product";

        public List<ZentaoProductData> Export()
        {
            List<ZentaoProductData> results = new List<ZentaoProductData>();
            using (_connection = new MySqlConnection())
            {
                try
                {
                    _connection.ConnectionString = Resources.ZentaoMysqlString;
                    _connection.Open();

                    MySqlCommand cmd = new MySqlCommand(SqlCmd, _connection);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ZentaoProductData data = new ZentaoProductData
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name")
                        };
                        results.Add(data);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }

            return results;
        }

        private MySqlConnection _connection;
    }
}