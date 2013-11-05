using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SqlInterface
{

    public class SqlInterface
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public SqlInterface(string server, string database, string uid, string password)
        {
            this.server = server;
            this.database = database;
            this.uid = uid;
            this.password = password;

            string connectionString = "SERVER=" + server + ";" +
                                      "DATABASE=" + database + ";" +
                                      "UID=" + uid + ";" +
                                      "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }


        //----------------------------------------------------------------------
        /// <summary>
        /// Open Connection to the database
        /// </summary>
        /// <returns></returns>
        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //----------------------------------------------------------------------
        /// <summary>
        /// Close Connection to database
        /// </summary>
        /// <returns></returns>

        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

       
       


        //------------------------Private Methods-------------------------------

        private bool ExecuteCommand(string query)
        {
            MySqlCommand cmd;
            if(OpenConnection() == true)
            {
                cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                if(this.CloseConnection())
                    return true;
            }
            return false;
        }


    }
}