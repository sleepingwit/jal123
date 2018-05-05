using System.Collections.Generic;
using Jal123.GUI.Properties;
using MySql.Data.MySqlClient;

namespace Jal123.GUI
{
    public class ZentaoDbStoryRevisionExporter
    {
        private const string SqlCmdGetAllTask =
            "SELECT id,name FROM zt_task WHERE story IN ({0})";

        private const string SqlCmdGetTaskRevisions =
            "SELECT id,extra FROM zt_action WHERE action=\"svncommited\" AND objectType=\"task\" AND objectId IN ({0})";

        public List<uint> Export(ICollection<int> stories)
        {
            List<uint> numbers = new List<uint>();

            if (stories == null || stories.Count == 0)
            {
                return numbers;
            }

            string rawStory = string.Join(",", stories);

            using (_connection = new MySqlConnection())
            {
                try
                {
                    _connection.ConnectionString = Resources.ZentaoMysqlString;
                    _connection.Open();

                    List<int> tasks = new List<int>();
                    string sqlCmdGetAllTask = string.Format(SqlCmdGetAllTask, rawStory);
                    MySqlCommand cmdGetAllTask = new MySqlCommand(sqlCmdGetAllTask, _connection);

                    var readerGetAllTask = cmdGetAllTask.ExecuteReader();
                    while (readerGetAllTask.Read())
                    {
                        tasks.Add(readerGetAllTask.GetInt32("id"));
                    }

                    readerGetAllTask.Close();

                    if (tasks.Count > 0)
                    {
                        string rawTask = string.Join(",", tasks);
                        string sqlCmdGetTaskRevisions = string.Format(SqlCmdGetTaskRevisions, rawTask);
                        MySqlCommand cmdGetTaskRevisions = new MySqlCommand(sqlCmdGetTaskRevisions, _connection);

                        var readerGetTaskRevisions = cmdGetTaskRevisions.ExecuteReader();
                        while (readerGetTaskRevisions.Read())
                        {
                            string extra = readerGetTaskRevisions.GetString("extra");
                            if (!string.IsNullOrWhiteSpace(extra))
                            {
                                numbers.Add(uint.Parse(extra));
                            }
                        }
                    }
                }
                finally
                {
                    _connection.Close();
                }
            }

            return numbers;
        }

        private MySqlConnection _connection;
    }
}