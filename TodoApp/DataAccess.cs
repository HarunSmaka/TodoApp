using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TodoApp
{
    public class DataAccess
    {
        private string _connectionString,
                       _masterConnectionString;

        public DataAccess(string connectionString,string masterDataBaseConnectionString, string dataBaseName)
        {
            _connectionString = connectionString;
            _masterConnectionString = masterDataBaseConnectionString;

            if (!DBCheck(dataBaseName))
            {
                CreateDataBase();
                CreateDataBaseElements();
            }
        }
        private bool DBCheck(string dbName)
        {

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                string sql = $@"SELECT name FROM master.dbo.sysdatabases WHERE name = N'{dbName}'";
                bool dbExists;

                try
                {
                    dbExists = connection.Query(sql).Any();
                }
                catch (Exception)
                {
                    dbExists = false;
                }

                return dbExists;
            }
        }
        private void CreateDataBase()
        {
            using (IDbConnection connection = new SqlConnection(_masterConnectionString))
            {
                string rootPath = @"..\..\..\CreateDatabase.sql";

                FileInfo file = new FileInfo(rootPath);

                string script = file.OpenText().ReadToEnd();
                connection.Query(script);
            }
        }

        private void CreateDataBaseElements()
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                string rootPath = @"..\..\..\TodoAppQuerys";
                string[] files = Directory.GetFiles(rootPath, "*.sql", SearchOption.TopDirectoryOnly);
                Array.Sort(files, (a, b) => int.Parse(Regex.Replace(a, "[^0-9]", "")) 
                            - int.Parse(Regex.Replace(b, "[^0-9]", "")));
                
                foreach (var file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    string script = fileInfo.OpenText().ReadToEnd();
                    Server server = new Server(new ServerConnection((SqlConnection)connection));
                    connection.Query(script);
                }
            }
        }

        public int EmailSearch(string email)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var emailResult = connection.Query("proc_email_search @email", 
                    new { Email = email }).Count();

                return emailResult;
            }
        }

        public User GetUser(string email, string password)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                password = CryptoEngine.Encrypt(password, "sblw-3hn8-sqoy19");

                var output = connection.Query<User>(
                    "dbo.proc_LoginUser @email, @password",
                    new { Email = email, Password = password }).ToList();

                if (output.Any() == false)
                {
                    return null;
                }

                return output[0];
            }
        }

        public List<Todos> GetTodos(string email)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var output = connection.Query<Todos>("dbo.proc_get_todos @email", new { Email = email }).ToList();
                return output;
            }
        }

        public List<DoneTodos> GetDoneTodos(int userID)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var output = connection.Query<DoneTodos>("dbo.proc_get_done_todos @userID", new { UserID = userID }).ToList();
                return output;
            }
        }

        public void InsertUser(string firstName, string lastName, string email, string password)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                List<User> user = new List<User>();

                user.Add(new User { FirstName = firstName, LastName = lastName, Email = email, Password = password });
                user[0].Password = CryptoEngine.Encrypt(user[0].Password, "sblw-3hn8-sqoy19");
                connection.Execute("dbo.proc_insert_user @firstName, @lastName, @email, @password", user);

            }
        }

        public void InsertTodo(string content, int userID)
        {

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                List<Todos> todo = new List<Todos>();

                todo.Add(new Todos { UserID = userID, Content = content });
                connection.Execute("dbo.proc_insert_todo @userID,@content", todo);

            }

        }

        public void InsertDoneTodo(string content, int userID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                List<DoneTodos> doneTodo = new List<DoneTodos>();

                doneTodo.Add(new DoneTodos { UserID = userID, Content = content });
                connection.Execute("dbo.proc_insert_done_todo @userID,@content", doneTodo);

                List<Todos> todo = new List<Todos>();


            }
        }

        public void UpdateTodo(string newContent, string content, int userID)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {

                List<Todos> todo = new List<Todos>();

                todo.Add(new Todos { UserID = userID, Content = content });
                connection.Query("dbo.proc_update_todo @newContent, @content, @userID", new { newContent = newContent, content = content, userID = userID });

            }

        }

        public void DeleteTodo(string content, int userID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                List<Todos> todo = new List<Todos>();

                todo.Add(new Todos { Content = content, UserID = userID });
                connection.Execute("dbo.proc_delete_todo @content, @userID", todo);
            }
        }

        public void DeleteDoneTodo(string content, int userID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                List<DoneTodos> doneTodo = new List<DoneTodos>();

                doneTodo.Add(new DoneTodos { Content = content, UserID = userID });
                connection.Execute("dbo.proc_delete_done_todo @content, @userID", doneTodo);

            }

        }

        public void DeleteAllDoneTodos()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Execute("dbo.proc_delete_done_all_todos");
            }

        }
    }
}
