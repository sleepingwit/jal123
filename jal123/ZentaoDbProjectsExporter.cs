using System;
using System.Collections.Generic;
using Jal123.GUI.Properties;
using MySql.Data.MySqlClient;

namespace Jal123.GUI
{
    internal class ZentaoProjectData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    internal class ZentaoDbProjectsExporter
    {
        private const string SqlCmd =
            "SELECT zt_project.id, zt_project.name, zt_projectproduct.product, zt_project.status "
            + "FROM zt_project INNER JOIN zt_projectproduct ON zt_project.id = zt_projectproduct.project "
            + "WHERE zt_projectproduct.product= {0} AND zt_project.deleted = \"0\" AND zt_project.status != \"done\" ORDER BY zt_project.id DESC";

        public List<ZentaoProjectData> Export(ZentaoProductData productData)
        {
            _connection = new MySqlConnection();

            List<ZentaoProjectData> results = new List<ZentaoProjectData>();
            using (_connection = new MySqlConnection())
            {
                try
                {
                    _connection.ConnectionString = Resources.ZentaoMysqlString;
                    _connection.Open();

                    string sqlCmd = String.Format(SqlCmd, productData.Id);
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, _connection);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ZentaoProjectData data = new ZentaoProjectData
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            ProductId = reader.GetInt32("product")
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