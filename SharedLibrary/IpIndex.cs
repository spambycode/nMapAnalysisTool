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
        private BSTNode _IPTree;
        private short _ipCounter;

        enum InsertType {IP_PortInsert, IP_OSInsert };
        public IpIndex()
        {
            indexBackupFile = new FileStream("index.bin", FileMode.OpenOrCreate);
            bBackupFileR = new BinaryReader(indexBackupFile);
            bBackupFileW = new BinaryWriter(indexBackupFile);
            _ipCounter = 0;


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
            BSTNode ipNode = null;

            if (_IPTree == null)
            {

                _IPTree = new BSTNode(ip);

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

        //--------------------------------------------------------------------
        /// <summary>
        /// Save all information and close files.
        /// </summary>
        public void FinishUp()
        {
            bBackupFileW.Write(_ipCounter);
            IOTSave(_IPTree);
            bBackupFileR.Close();
            bBackupFileW.Close();
            indexBackupFile.Close();
        }

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
            if (currentNode == null)
                return;

            IOTSave(currentNode.LChildPtr);
            currentNode.FinishUp(bBackupFileW);
            IOTSave(currentNode.RChildPtr);

        }

        private void ReadBackUpFile()
        {
            _ipCounter = bBackupFileR.ReadInt16();
            short PortCount;
            string IP, OS, OS_Version, Port,PortType, PortService, PortState;

            for(int i = 0; i < _ipCounter; i++)
            {
                IP = bBackupFileR.ReadString();
                OS = bBackupFileR.ReadString():
                OS_Version = bBackupFileR.ReadString():
                PortCount = bBackupFileR.ReadInt16();

            }
        }
    }

}

