﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SharedLibrary
{

    class RawData
    {

        public string PORT_NUM { get; set; }
        public string PORT_TYPE { get; set; }
        public string STATE { get; set; }
        public string SERVICE { get; set; }
        public string OS_TYPE { get; set; }
        public string OS_VERSION { get; set; }

        private Process nMapProcess;

        public RawData(string ip, string[] Arguments)
        {
            nMapProcess = new Process();
            nMapProcess.StartInfo.FileName = "nmap.exe";

            foreach (string arg in Arguments)
                nMapProcess.StartInfo.Arguments += arg;

            nMapProcess.StartInfo.Arguments += " " + ip;

            nMapProcess.StartInfo.UseShellExecute = false;
            nMapProcess.StartInfo.RedirectStandardOutput = true;
            nMapProcess.Start();
        }

        //-------------------------------------------------------------------------------------
        /// <summary>
        /// Reads a line from the command prompt's output, and returns the raw data of 
        /// Port and OS information
        /// </summary>
        /// <returns>Less than zero    = EOF. 
        ///          Equal to Zero     = Port Information. 
        ///          Greater than Zero = OS information</returns>
        public int ReadRawLine()
        {
            int portTest = 0;

            while(!nMapProcess.StandardOutput.EndOfStream)
            {
                string Line = nMapProcess.StandardOutput.ReadLine();
                var LineSplit = Line.Split(' ');

                //nMap always places port numbers first before information and status info
               if(Int32.TryParse(LineSplit[0], out portTest))
               {
                   //EX Results: 80/TCP OPEN HTTP
                   var PortTypeSplit = LineSplit[0].Split('/');
                   PORT_NUM  = PortTypeSplit[0];
                   PORT_TYPE = PortTypeSplit[1];
                   STATE     = LineSplit[1];
                   SERVICE   = LineSplit[2];

                   return 0;
               }
               else
               {

                   if(LineSplit[0].ToUpper().CompareTo("OS") == 0)
                   {
                       //EX Results: OS details: Linux 2.6.32 - 2.6.39 (Might not have version Number)
                       OS_TYPE = LineSplit[2];

                       for(int i = 3; i < LineSplit.Length; i++)
                       {
                           OS_VERSION += LineSplit[i];
                       }

                       return 1;
                   }
               }

            }

            return -1;

        }
    }
}