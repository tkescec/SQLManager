using SQLManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLManager.Dal
{
    class SqlRepository : IRepository
    {
        private string cs;

        private const string ConnectionString = "Server={0};Uid={1};Pwd={2}";
        private const string SelectDatabases = "SELECT name As Name FROM sys.databases";
        private const string Query = "USE {0}; {1}";

        public void LogIn(string server, string username, string password)
        {
            using (SqlConnection con = new SqlConnection(string.Format(ConnectionString, server, username, password)))
            {
                cs = con.ConnectionString;
                con.Open();
            }
        }

        public IEnumerable<Database> GetDatabases()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = SelectDatabases;
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return new Database
                            {
                                Name = dr[nameof(Database.Name)].ToString()
                            };
                        }
                    }
                }
            }
        }

        public void GetDataSet(Database db, string query)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = (db.Name != null) ? string.Format(Query, db.Name, query) : query;
                    cmd.CommandType = CommandType.Text;

                    SqlDataAdapter da = new SqlDataAdapter
                    {
                        SelectCommand = cmd
                    };

                    DataSet dataSet = new DataSet(db.Name ?? "Main");
                    da.Fill(dataSet);

                    //using (SqlDataReader dr = cmd.ExecuteReader())
                    //{
                    //    while (dr.Read())
                    //    {
                    //        var test = dr;
                    //        //yield return new DataSet
                    //        //{
                    //        //    Name = dr[nameof(Parameter.Name)].ToString(),
                    //        //    Mode = dr[nameof(Parameter.Mode)].ToString(),
                    //        //    DataType = dr[nameof(Parameter.DataType)].ToString()
                    //        //};
                    //    }
                    //}
                }
            }
        }
    }
}
