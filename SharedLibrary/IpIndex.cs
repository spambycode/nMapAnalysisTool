using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SharedLibrary
{
    public class IpIndex
    {
        private FileStream indexBackupFile;
        private BinaryWriter bBackupFileW;
        private BinaryReader bBackupFileR;
        private BSTIpNode _IPTree;
        private short _ipCounter;
        private short _root;
        enum InsertType {IP_PortInsert, IP_OSInsert };
        public IpIndex()
        {
            indexBackupFile = new FileStream("index.bin", FileMode.OpenOrCreate);
            bBackupFileR = new BinaryReader(indexBackupFile);
            bBackupFileW = new BinaryWriter(indexBackupFile);
            _ipCounter = 0;
            _root = -1;


        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Insert new IP query into tree
        /// </summary>
        /// <param name="ip">IP of scanned computer</param>
        /// <param name="RD">Data containing Port and OS information</param>
        /// <param name="optionType">Inserting Action</param>
        public void InsertIPAddress(string ip, RawData RD, InsertType optionType)
        {
            BSTIpNode ipNode = null;

            if (_root == -1)
            {

                _IPTree = new BSTIpNode(ip);
                _root = 0;

            }

            ipNode = SearchForIP(ip);

            if(ipNode == null)
            {
                InsertIPAddress(ip);
                ipNode = SearchForIP(ip);
            }



            switch (optionType)
            {
                case InsertType.IP_OSInsert:
                    ipNode.InsertOSType(RD.OS_TYPE, RD.OS_VERSION);
                    break;
                case InsertType.IP_PortInsert:          
                    ipNode.InsertOnePort(RD);
                    break;
            }
        }

        public void FinishUp()
        {

        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Search for a IP in the tree
        /// </summary>
        /// <param name="ip">Search term</param>
        /// <returns>Node of the found searched item</returns>
        private BSTIpNode SearchForIP(string ip)
        {
            BSTIpNode currentNode = _IPTree;

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
            BSTIpNode prevNode = null, currentNode = _IPTree;

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
                else
                {
                    return false;
                }
            }


            if (prevNode.CompareTo(ip) > 0)
            {
                prevNode.RChildPtr = new BSTIpNode(ip);
            }
            else
            {
                prevNode.LChildPtr = new BSTIpNode(ip);
            }

            _ipCounter++;

            return true;
        }
    }

}

