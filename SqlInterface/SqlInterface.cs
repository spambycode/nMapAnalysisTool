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
            int IP_ID;
            int PORT_ID;

            if (this.OpenConnection() == true)
            {


                IP_ID = InsertIP(currentNode.IP);

                foreach (BSTNode.PORTInformation p in currentNode.PortInfo)
                {
                    if ((PORT_ID = IsPortKnown(p)) != -1)
                    {
                        if(IsPortConnectToIP(IP_ID, PORT_ID))
                        {
                            continue;
                        }
                        else
                        {

                        }
                    }



                }
            }
        }

        //----------------------------------------------------------------------------

        /// <summary>
        /// Check if port information has already been entered
        /// </summary>
        /// <param name="p">Port that is being looked at</param>
        /// <returns>returns ID of port that is known in the database</returns>
        private int IsPortKnown(BSTNode.PORTInformation p)
        {
            string query = string.Format("SELECT * FROM nmapanalysistool.port," +
                                         " nmapanalysistool.port_information " +
                                         "where port_information.id = port.port_informationID " +
                                         "and port.port_number = {0} "+
                                         "and port., p.PORT);

            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader["id"] != null)
                return Convert.ToInt32(dataReader["id"]);


            return -1;
        }

        //-----------------------------------------------------------------------------
        /// <summary>
        /// Inserts the IP into the database
        /// </summary>
        /// <param name="ip">IP of the targeted computer network</param>
        /// <returns>ID of ip in the table</returns>
        private int InsertIP(string ip)
        {
            MySqlCommand cmd;
            MySqlDataReader dataReader;
            string queryCheck = string.Format("SELECT * FROM nmapanalysistool.ipaddress" +
                                              "where ipaddress.IpAddress = \"{0}\";", ip);
            string queryInsert = string.Format("INSERT INTO nmapanalysistool.ipaddress(ipaddress.IpAddress)" +  
                                               "VALUES(\"{0}\");", ip);
            int id = -1;

            try
            {
                cmd = new MySqlCommand(queryInsert, connection);

            } catch(MySqlException ex)
            {
                if(ex.ErrorCode == 1062)
                {
                    //Dup was found
                }
            }
            finally
            {
                cmd = new MySqlCommand(queryCheck, connection);
                dataReader = cmd.ExecuteReader();

                if (dataReader.Read())
                    id = Convert.ToInt32(dataReader["id"]);

            }
            return id;
        }

    }
}