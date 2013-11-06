/*Author: George Karaszi
 * Date: 11-5-2013
 * 
 * Discription:Handles all interactions with the given database.
 *         -Takes the root of the tree from StoreIndex.cs and queries 
 *          all information to the database, by checking(Select) and inserting. 
 *          To determin if data is currently present in the database.
 */


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

        //----------------------------------------------------------------------
        /// <summary>
        /// Sends all IP's and its port information stored, to the sql database
        /// </summary>
        /// <param name="root">root of tree object</param>
        /// <returns></returns>
        public void QueryIPLists(BSTNode root)
        {
            IOTQuery(root);
        }


        //------------------------Private Methods-------------------------------

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
        /// <summary>
        /// Handles all transaction to and from the SQL server, 
        /// from the result of the current node selected
        /// </summary>
        /// <param name="currentNode">Node that needs to be sent to SQL database</param>
        private void QueryChild(BSTNode currentNode)
        {
            int IP_ID;
            int PORT_ID;

            if (this.OpenConnection() == true)
            {


                IP_ID = InsertIP(currentNode.IP);

                foreach (BSTNode.PORTInformation p in currentNode.PortInfo)
                {
                    PORT_ID = InsertPort(p);
                    ConnectIPAndPort(IP_ID, PORT_ID);
                }
            }

            CloseConnection();
        }

        //----------------------------------------------------------------------------

        /// <summary>
        /// Inserts the port and its info into the database
        /// </summary>
        /// <param name="p">Port that is being looked at</param>
        /// <returns>returns ID of port that the table has assigned</returns>
        private int InsertPort(BSTNode.PORTInformation p)
        {

            string selectInfo = string.Format("Select * From nmapanalysistool.port_information" +
                                              " where port_information.portInfo = '{0}'", p.SERVICE);

            string insertInfo = string.Format("INSERT INTO nmapanalysistool.port_information" +
                                              "(portInfo) VALUES ('{0}');", p.SERVICE);

            string selectPort = "Select * From nmapanalysistool.port, nmapanalysistool.port_information" +
                                " where port.port_number = {0} and port.port_type = '{1}' and port.state = '{2}' " +
                                "and port.port_informationID = {3}";

            string insertPort = "INSERT INTO nmapanalysistool.port" +
                                "(port_number, port_type, state, port_informationID)" +
                                "VALUES({0}, '{1}', '{2}', {3})";
            int infoID = (int)GetAndSetRowCommand(insertInfo, selectInfo, "id");

            //Assign inforID to the following queries
            selectPort = string.Format(selectPort, p.PORT, p.PORT_TYPE, p.STATE, infoID);
            insertPort = string.Format(insertPort, p.PORT, p.PORT_TYPE, p.STATE, infoID);




            return (int)GetAndSetRowCommand(insertPort, selectPort, "id");
        }

        //-----------------------------------------------------------------------------
        /// <summary>
        /// Inserts the IP into the database
        /// </summary>
        /// <param name="ip">IP of the targeted computer network</param>
        /// <returns>ID of ip in the table</returns>
        private int InsertIP(string ip)
        {
            string queryCheck = string.Format("SELECT * FROM nmapanalysistool.ipaddress" +
                                              " where ipaddress.IpAddress = '{0}'", ip);

            string queryInsert = string.Format("INSERT INTO nmapanalysistool.ipaddress(ipaddress.IpAddress)" +  
                                               "VALUES('{0}')", ip);
           
            return (int)GetAndSetRowCommand(queryInsert, queryCheck, "id");
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Link the port information and IP together in a linking table
        /// </summary>
        /// <param name="ip_id">ID of IP within the table</param>
        /// <param name="port_id">ID of the port information within the table</param>
        private void ConnectIPAndPort(int ip_id, int port_id)
        {
            string selectIPandPort = string.Format("SELECT ip_port.IP_ID, ip_port.PortID " +
                                                   "FROM nmapanalysistool.ip_port " +
                                                   "where ip_port.IP_ID = {0} and ip_port.PortID = {1};",
                                                   ip_id, port_id);

            string insertIpAndPort = string.Format("INSERT INTO nmapanalysistool.ip_port(IP_ID,PortID) " +
                                                   "VALUES({0}, {1});",
                                                    ip_id, port_id);

            GetAndSetRowCommand(insertIpAndPort, selectIPandPort, "IP_ID");
        }


        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// An all incumbent function that returns the first row of a given table. If no table is 
        /// found, it will insert then return its desired row return.
        /// </summary>
        /// <param name="insert">Insert query string</param>
        /// <param name="select">Select query string</param>
        /// <param name="rowReturn">Row that wants to be returned</param>
        /// <returns>object of requested row</returns>
        private object GetAndSetRowCommand(string insert, string select, string rowReturn)
        {
            MySqlCommand cmd = new MySqlCommand(select, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            object ReturnRow;

            //No data was found if condition is true.
            if (dataReader.Read() == false)
            {
                dataReader.Close();

                cmd.CommandText = insert;
                cmd.ExecuteNonQuery();
                cmd.CommandText = select;
                dataReader = cmd.ExecuteReader();
                dataReader.Read();
            }
            
            ReturnRow = dataReader[rowReturn];

            dataReader.Close();

            return ReturnRow;

        }

    }
}