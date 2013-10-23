using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    class BSTPortNode
    {

        public BSTPortNode RChildPtr { get; set; }
        public int PORT_NUM { get; set; }
        public string PORT_TYPE { get; set; }
        public string STATE { get; set; }
        public string SERVICE { get; set; }
        public BSTPortNode LChildPtr { get; set; }

        public BSTPortNode(int port, string portType, string state, string service)
        {
            PORT_NUM = port;
            PORT_TYPE = portType;
            STATE = state;
            SERVICE = service;
            RChildPtr = null;
            LChildPtr = null;
        }

        public BSTPortNode()
        {
            RChildPtr = null;
            LChildPtr = null;
        }

        public int CompareTo(string port)
        {
            return CompareTo(Int32.Parse(port));
        }

        public int CompareTo(int port)
        {
            return PORT_NUM.CompareTo(port);
        }
    }
}
