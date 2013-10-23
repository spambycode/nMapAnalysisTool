using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    class BSTIpNode
    {
        public BSTIpNode LChildPtr { get; set; }
        public string IP { get; set; }
        public BSTPortNode PORT_BST { get; set; }
        public string OS_TYPE { get; set; }
        public string OS_VERSION { get; set; }
        public BSTIpNode RChildPtr { get; set; }

        private short _portCount;
        private short _portRoot;

        //-----------------------------------------------------------------------
        /// <summary>
        /// Initalize a tree node containing the compartive IP parameter
        /// </summary>
        /// <param name="IP">IP of the network</param>
        public BSTIpNode(string IP)
        {
            this.IP = IP;
            OS_TYPE = "Unknown";
            OS_VERSION = "0";
            _portCount = 0;
            _portRoot = -1;
            LChildPtr = null;
            RChildPtr = null;
        }


        //-----------------------------------------------------------------------
        /// <summary>
        /// Insert the OS information of the computer on the network being scanned
        /// </summary>
        /// <param name="OS_TYPE">What kind of OS</param>
        /// <param name="OS_VERSION">Version of said OS</param>
        public void InsertOSType(string OS_TYPE, string OS_VERSION)
        {
            this.OS_TYPE = OS_TYPE;
            this.OS_VERSION = OS_VERSION;
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Insert a port into the BST for the current IP
        /// </summary>
        /// <param name="RD">Raw information about the port</param>

        public void InsertOnePort(RawData RD)
        {
            BSTPortNode childNode = null;
            BSTPortNode prevNode = null;
            BSTPortNode InsertNode = new BSTPortNode(Int32.Parse(RD.PORT_NUM),
                                        RD.PORT_TYPE, RD.STATE, RD.SERVICE);
            if (_portRoot == -1)
            {
                PORT_BST = InsertNode;
                _portRoot = 0;
            }
            else
            {
                //Search for a posisition in the tree to insert.
                childNode = PORT_BST;

                while (childNode != null)
                {
                    prevNode = childNode;

                    if (childNode.CompareTo(RD.PORT_NUM) > 0)
                    {
                        childNode = childNode.RChildPtr;
                    }
                    else
                    {
                        childNode = childNode.LChildPtr;
                    }
                }

                //Hit a null, we've reached the bottom, time to insert
                if (prevNode.CompareTo(RD.PORT_NUM) > 0)
                {
                    prevNode.RChildPtr = InsertNode;
                }
                else
                {
                    prevNode.LChildPtr = InsertNode;
                }

                _portCount++;

            }
        }


        //------------------------------------------------------------------------
        /// <summary>
        /// Compares IP address stored in the current node
        /// </summary>
        /// <param name="ip">IP address of computer</param>
        public int CompareTo(string ip)
        {
            return IP.ToUpper().CompareTo(ip.ToUpper());
        }

    }
}
