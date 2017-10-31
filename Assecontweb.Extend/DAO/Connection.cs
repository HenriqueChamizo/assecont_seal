using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class Connection
    {
        public string connectionstring;
        public SqlConnection conn;
        public SqlCommand cmd;
        public SqlTransaction tran;
        public DbDataReader dr;

        public Connection(string ConnectionString)
        {
            connectionstring = ConnectionString;
        }

        public Connection(string server = "200.170.88.138", string database = "assecont2", string user = "assecont5272", string password = "*h6prMvr")
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource = server;
            builder.InitialCatalog = database;
            builder.UserID = user;
            builder.Password = password;

            connectionstring = builder.ConnectionString;
        }

        public Connection(SqlConnectionStringBuilder SqlStringBuilder)
        {
            connectionstring = SqlStringBuilder.ConnectionString;
        }
    }
}
