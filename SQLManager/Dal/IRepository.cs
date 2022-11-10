using SQLManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLManager.Dal
{
    interface IRepository
    {
        void LogIn(string server, string username, string password);
        IEnumerable<Database> GetDatabases();
        DataSet GetDataSet(Database db, string query);
    }
}
