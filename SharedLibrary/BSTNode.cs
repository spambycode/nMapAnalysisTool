using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SharedLibrary
{
    class BSTNode
    {

        private struct PORTInformation
        {
            public string PORT;
            public string PORT_TYPE;
            public string STATE;
            public string SERVICE;
        }

        public BSTNode LChildPtr { get; set; }
        public string IP { get; set; }
        public string OS_TYPE { get; set; }
        public string OS_VERSION { get; set; }
        public BSTNode RChildPtr { get; set; }
        public PORTInformation[] PortInfo;
        private short _portCount;

        //-----------------------------------------------------------------------
        /// <summary>
        /// Initialize a tree node containing the comparative IP parameter
        /// </summary>
        /// <param name="IP">IP of the network</param>
        public BSTNode(string IP)
        {
            this.IP = IP;
            OS_TYPE = "Unknown";
            OS_VERSION = "0";
            _portCount = 0;
            PortInfo   = new PORTInformation[0];
            LChildPtr  = null;
            RChildPtr  = null;
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
        /// Insert a port that relates to the current IP
        /// </summary>
        /// <param name="RD">Raw information about the port</param>

        public void InsertOnePort(RawData RD)
        {
            Array.Resize<PORTInformation>(ref PortInfo, PortInfo.Length + 1);

            PortInfo[_portCount].PORT      = RD.PORT_NUM;
            PortInfo[_portCount].PORT_TYPE = RD.OS_TYPE;
            PortInfo[_portCount].SERVICE   = RD.SERVICE;
            PortInfo[_portCount].STATE     = RD.STATE;

            ++_portCount;
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

        //--------------------------------------------------------------------------
        public void FinishUp(BinaryWriter bIndexFileW)
        {

            bIndexFileW.Write(_portCount);
            bIndexFileW.Write(IP);
            bIndexFileW.Write(OS_TYPE);
            bIndexFileW.Write(OS_VERSION);

            foreach(PORTInformation pi in PortInfo)
            {
                bIndexFileW.Write(pi.PORT);
                bIndexFileW.Write(pi.PORT_TYPE);
                bIndexFileW.Write(pi.SERVICE);
                bIndexFileW.Write(pi.STATE);
            }

        }

    }
}
