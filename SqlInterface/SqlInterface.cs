using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SharedLibrary;

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

        public bool QueryIPLists(BSTNode root, int amount)
        {
            IOTQuery(root);


            return true;

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


        //--------------------------------------------------------------------------
        /// <summary>
        /// Easily query all IP's in the tree, using the In order Traversal method
        /// </summary>
        /// <param name="currentNode">Root of the BSTNode tree</param>
        private void IOTQuery(BSTNode currentNode)
        {
                if (currentNode == null)
                    return;

                IOTQuery(currentNode.LChildPtr);
                QueryChild(currentNode);
                IOTQuery(currentNode.RChildPtr);

        }


        //----------------------------------------------------------------------------
        private void QueryChild(BSTNode currentNode)
        {
            string queryStr;

            if (this.OpenConnection() == true)
            {
                foreach (BSTNode.PORTInformation p in currentNode.PortInfo)
                {
                    queryStr = string.Format("SELECT * FROM nmapanalysistool.port," + 
                                             " nmapanalysistool.port_information " + 
                                             "where port_information.id = port.port_informationID " + 
                                             "and port.port_number = {0};", p.PORT);



                }
            }
        }


        /// <summary>
        /// Check if port information has already been entered
        /// </summary>
        /// <param name="p">Port that is being looked at</param>
        /// <returns>true if port is known</returns>
        private bool IsPortKnown(BSTNode.PORTInformation p, string ip)
        {
            string query = string.Format("SELECT * FROM nmapanalysistool.port," +
                                         " nmapanalysistool.port_information " +
                                         "where port_information.id = port.port_informationID " +
                                         "and port.port_number = {0};", p.PORT);

            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader["id"] != null)
                return true;


            return false;
        }

    }
}