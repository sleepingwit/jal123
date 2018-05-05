using System;
using System.Collections.Generic;
using System.Text;
using Jal123.GUI.Properties;
using MySql.Data.MySqlClient;

namespace Jal123.GUI
{
    class ZentaoDbBugRevisionExporter
    {
        private const string SqlCmdGetBugRevisions =
            "SELECT id, extra FROM zt_action WHERE action =\"svncommited\" AND objectType=\"bug\" AND objectId IN ({0})";

        public List<uint> Export(ICollection<int> bugs)
        {
            List<uint> numbers = new List<uint>();
            if (bugs == null || bugs.Count == 0)
            {
                return numbers;
            }

            string rawBugs = String.Join(",", bugs);
            using (_connection = new MySqlConnection())
            {
                try
                {
                    _connection.ConnectionString = Resources.ZentaoMysqlString;
                    _connection.Open();

                    string sqlCmdGetBugRevisions = String.Format(SqlCmdGetBugRevisions, rawBugs);
                    MySqlCommand cmdGetBugRevisions = new MySqlCommand(sqlCmdGetBugRevisions, _connection);

                    var readerGetBugRevisions = cmdGetBugRevisions.ExecuteReader();
                    while (readerGetBugRevisions.Read())
                    {
                        string extra = readerGetBugRevisions.GetString("extra");
                        if (!string.IsNullOrWhiteSpace(extra))
                        {
                            numbers.Add(uint.Parse(extra));
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