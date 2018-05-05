using System;
using System.Collections.Generic;
using Jal123.GUI.Properties;
using MySql.Data.MySqlClient;

namespace Jal123.GUI
{
    internal class ZentaoBuildData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        ///     产品
        /// </summary>
        public int Product { get; set; }

        /// <summary>
        ///     迭代
        /// </summary>
        public int Project { get; set; }

        /// <summary>
        ///     完成的需求
        /// </summary>
        public List<int> Stories { get; set; }

        /// <summary>
        ///     解决的BUG
        /// </summary>
        public List<int> Bugs { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    internal class ZentaoDbBuildsExporter
    {
        private const string SqlCmd =
            "SELECT id,name, stories,bugs FROM zt_build WHERE product = {0} AND project = {1} ORDER BY id DESC";

        public List<ZentaoBuildData> Export(ZentaoProjectData projectData)
        {
            List<ZentaoBuildData> results = new List<ZentaoBuildData>();

            using (_connection = new MySqlConnection())
            {
                try
                {
                    _connection.ConnectionString = Resources.ZentaoMysqlString;
                    _connection.Open();

                    string sqlCmd = String.Format(SqlCmd, projectData.ProductId, projectData.Id);
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, _connection);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ZentaoBuildData data = new ZentaoBuildData
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Product = projectData.ProductId,
                            Project = projectData.Id,
                            Stories = ParseZentaoStories(reader.GetString("stories")),
                            Bugs = ParseZentaoBugs(reader.GetString("bugs"))
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

        private List<int> ParseZentaoStories(string rawStories)
        {
            List<int> results = new List<int>();
            if (String.IsNullOrWhiteSpace(rawStories))
            {
                return results;
            }

            string[] rawNumbers = rawStories.Split(',');
            foreach (string story in rawNumbers)
            {
                if (String.IsNullOrWhiteSpace(story))
                    continue;
                if (!int.TryParse(story.Trim(), out int storyId))
                {
                    throw new Exception(String.Format("{0} 不是有效的需求Id", story));
                }

                results.Add(storyId);
            }

            return results;
        }

        private List<int> ParseZentaoBugs(string rawBugs)
        {
            List<int> results = new List<int>();
            if (String.IsNullOrWhiteSpace(rawBugs))
            {
                return results;
            }

            string[] rawNumbers = rawBugs.Split(',');
            foreach (string bug in rawNumbers)
            {
                if (String.IsNullOrWhiteSpace(bug))
                    continue;
                if (!int.TryParse(bug.Trim(), out int bugId))
                {
                    throw new Exception(String.Format("{0} 不是有效的Bug Id", bug));
                }

                results.Add(bugId);
            }

            return results;
        }

        private MySqlConnection _connection;
    }
}