using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.BizTalk.ExplorerOM;

namespace bizilante.Deployment.Apps
{
    class Program
    {
        private static string _appName = string.Empty;
        private static string _server = string.Empty;
        private static string _database = "BizTalkMgmtDb";

        static void Main(string[] args)
        {
            if (!GetNamedArgs(args)) Environment.Exit(1);

            string dependencyString = string.Empty;
            LinkedList<string> dependencies = GetDependencies(_appName);
            if (dependencies.Count > 0)
            {
                foreach (string app in dependencies)
                {
                    if (app.ToLower() != args[0].ToLower())
                    {
                        if (!string.IsNullOrEmpty(dependencyString))
                        {
                            dependencyString = dependencyString + "," + app;
                        }
                        else
                        {
                            dependencyString = dependencyString + app;
                        }
                    }
                }
            }
            Console.WriteLine(dependencyString);
        }

        private static bool GetNamedArgs(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify the application name!");
                return false;
            }
            _appName = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                string[] values = args[i].Split(new string[] { ":" }, StringSplitOptions.None);
                if (values.Length < 2) continue;
                switch (values[0].ToLower())
                {
                    case "-server":
                        _server = values[1];
                        break;
                    case "-database":
                        _database = values[1];
                        break;
                }
            }

            if (string.IsNullOrEmpty(_server) || string.IsNullOrEmpty(_database))
            {
                if (string.IsNullOrEmpty(_server))
                    Console.WriteLine("Please specify the server name");
                if (string.IsNullOrEmpty(_database))
                    Console.WriteLine("Please specify the database name");
                return false;
            }

            return true;
        }

        private static LinkedList<string> GetDependencies(string btsapp)
        {
            LinkedList<string> result = new LinkedList<string>();
            if (null == ConfigurationManager.ConnectionStrings["BizTalkMgmtDb"]) return result;

            string connectionString = string.Format(ConfigurationManager.ConnectionStrings["BizTalkMgmtDb"].ConnectionString, _server, _database);
            Get(ref result, btsapp, connectionString);
            return result;
        }
        private static void Get(ref LinkedList<string> list, string btsapp, string connectionString)
        {
            try
            {
                // Get the application dependencies based on the assembly dependencies
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand sqlCom = new SqlCommand();
                    sqlCom.Connection = conn;
                    sqlCom.CommandText = "dbo.USP_GetApplicationDependencies";
                    sqlCom.CommandType = CommandType.StoredProcedure;

                    SqlParameter p = new SqlParameter();
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.String;
                    p.ParameterName = "@p_BTSApplication";
                    p.Value = btsapp;
                    sqlCom.Parameters.Add(p);

                    // also add the application to the linked list
                    if (!list.Contains(btsapp))
                        list.AddLast(new LinkedListNode<string>(btsapp));

                    SqlDataReader reader = sqlCom.ExecuteReader(CommandBehavior.CloseConnection);
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SqlString s0 = reader.GetSqlString(0);
                            SqlString s1 = reader.GetSqlString(1);

                            if ((!s1.IsNull) && (s1.Value.ToLower() != btsapp.ToLower()))
                                AddNode(ref list, s0.Value, s1.Value);
                        }
                    }
                }
                // Get the application dependencies based on application references
                BtsCatalogExplorer explorer = new BtsCatalogExplorer();
                explorer.ConnectionString = connectionString;
                GetReferredBy(ref list, btsapp, explorer);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to retrieve the Application Dependencies for the application {0} - {1} : {2}", btsapp, connectionString, ex.Message), ex);
            }
        }

        private static void GetReferredBy(ref LinkedList<string> list, string app, BtsCatalogExplorer explorer)
        {
            Application application = explorer.Applications[app];
            if (null != application.BackReferences)
            {
                foreach (Application referredby in application.BackReferences)
                {
                    GetReferredBy(ref list, referredby.Name, explorer);
                    AddNode(ref list, app, referredby.Name);
                }
            }
        }
        private static void AddNode(ref LinkedList<string> list, string app, string referredby)
        {
            // Already available in linkedlist
            if (!list.Contains(app))
            {
                if (!list.Contains(referredby))
                {
                    LinkedListNode<string> node = new LinkedListNode<string>(app);
                    list.AddLast(node);
                    LinkedListNode<string> node1 = new LinkedListNode<string>(referredby);
                    list.AddBefore(node, node1);
                }
                else
                {
                    LinkedListNode<string> node1 = list.Find(referredby);
                    LinkedListNode<string> node = new LinkedListNode<string>(app);
                    list.AddAfter(node1, node);
                }
            }
            else
            {
                LinkedListNode<string> node = list.Find(app);
                if (!list.Contains(referredby))
                {
                    LinkedListNode<string> node1 = new LinkedListNode<string>(referredby);
                    list.AddBefore(node, node1);
                }
                else
                {
                    // Make sure the sequence is right
                    LinkedListNode<string> node1 = list.Find(referredby);
                    list.Remove(node1);
                    list.AddBefore(node, node1);
                }
            }
        }
    }
}
