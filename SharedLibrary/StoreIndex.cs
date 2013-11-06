/*Author: George Karaszi
 * Date:11-5-2013
 * 
 * Desicrition: Handles the binary tree functions for the incoming data that nmap has produced.
 * 
 * NOTE: No balancing is implemented. So tree can easily become O(N) especially during a range network scan.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SharedLibrary
{
    public class StoreIndex
    {
        private BSTNode _IPTree;                 //Binary tree holding IP, port, and OS information
        private short _ipCounter;                //Total amount of IP's searched

        public enum InsertType {IP_PortInsert = 0, IP_OSInsert = 1};  //Inserting into BST options

        //---------------------------PUBLIC METHODS----------------------------------

        //----------------------------------------------------------------------------
        /// <summary>
        /// Returns the whole binary tree of the given object
        /// </summary>
        public BSTNode GetTreeRoot
        {
            get
            {
                return _IPTree;
            }
        }

        //------------------------------------------------------------------------------
        public StoreIndex()
        {
            _ipCounter = 0;
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Insert new IP query into tree
        /// </summary>
        /// <param name="ip">IP of scanned computer</param>
        /// <param name="RD">Data containing Port and OS information</param>
        /// <param name="optionType">Inserting Action</param>
        public void Add(RawData RD, InsertType optionType)
        {
            BSTNode ipNode = null;

            if (_IPTree == null)
            {
                _IPTree = new BSTNode(RD.IP_ADDRESS);
                ipNode  = _IPTree;
            }
            else
            {
                ipNode = SearchForIP(RD.IP_ADDRESS);

                if (ipNode == null)
                {
                    InsertIPAddress(RD.IP_ADDRESS);
                    ipNode = SearchForIP(RD.IP_ADDRESS);
                }

            }

            switch (optionType)
            {
                case InsertType.IP_OSInsert:
                    ipNode.InsertOSType(RD.OS_TYPE, RD.OS_VERSION);
                    break;
                case InsertType.IP_PortInsert:          
                    ipNode.InsertOnePort(RD);
                    break;
                default:
                    break;
            }
        }


        //--------------------------------------------------------------------
        /// <summary>
        /// Save all information and close files.
        /// </summary>
        public void FinishUp()
        {
        }

        //----------------------PRIVATE METHODS--------------------------------

        //---------------------------------------------------------------------
        /// <summary>
        /// Search for a IP in the tree
        /// </summary>
        /// <param name="ip">Search term</param>
        /// <returns>Node of the found searched item</returns>
        private BSTNode SearchForIP(string ip)
        {
            BSTNode currentNode = _IPTree;

            while (currentNode != null)
            {
                if (currentNode.CompareTo(ip) > 0)
                {
                    currentNode = currentNode.RChildPtr;
                }
                else if (currentNode.CompareTo(ip) < 0)
                {
                    currentNode = currentNode.LChildPtr;
                }
                else
                {
                    return currentNode;
                }
            }

            return null;

        }

        //--------------------------------------------------------------------------
        /// <summary>
        /// Insert a new IP address into binary tree
        /// </summary>
        /// <param name="ip">IP for storing</param>
        /// <returns>State of inserting</returns>
        private bool InsertIPAddress(string ip)
        {
            BSTNode prevNode = null, currentNode = _IPTree;


            //Search tree to find an empty spot
            while (currentNode != null)
            {
                prevNode = currentNode;

                if (currentNode.CompareTo(ip) > 0)
                {
                    currentNode = currentNode.RChildPtr;
                }
                else if (currentNode.CompareTo(ip) < 0)
                {
                    currentNode = currentNode.LChildPtr;
                }
                else //Similar IP was found in tree
                {
                    return false;
                }
            }


            if (prevNode.CompareTo(ip) > 0)
            {
                prevNode.RChildPtr = new BSTNode(ip);
            }
            else
            {
                prevNode.LChildPtr = new BSTNode(ip);
            }

            _ipCounter++;

            return true;
        }




        //--------------------------------------------------------------------------
        /// <summary>
        /// Save all tree information to the file in order form
        /// </summary>
        /// <param name="currentNode">Root of the BSTNode tree</param>
        private void IOTSave(BSTNode currentNode)
        {

            for (int i = 0; i < _ipCounter; i++ )

                if (currentNode == null)
                    return;

            IOTSave(currentNode.LChildPtr);
          //  currentNode.FinishUp(bBackupFileW);
            IOTSave(currentNode.RChildPtr);

        }
    }

}

