using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    class BSTNode
    {
        public BSTNode LChildPtr {get; set;}
        public string IP { get; set; }
        public int PORT_NUM { get; set; }
        public string PORT_TYPE { get; set; }
        public string STATE { get; set; }
        public string SERVICE { get; set; }
        public string OS_TYPE { get; set; }
        public string OS_VERSION { get; set; }
        public BSTNode RChildPtr { get; set; }

        public BSTNode(string IP, int port_num, string port_type, string state, string service, string OS_TYPE, string OS_VERSION)
        {
            this.IP         = IP;
            this.PORT_NUM   = port_num;
            this.PORT_TYPE  = port_type;
            this.STATE      = state;
            this.SERVICE    = service;
            this.OS_TYPE    = OS_TYPE;
            this.OS_VERSION = OS_VERSION;

            LChildPtr = null;
            RChildPtr = null;
        }
        public BSTNode()
        {
            LChildPtr = null;
            RChildPtr = null;
        }

    }
}
