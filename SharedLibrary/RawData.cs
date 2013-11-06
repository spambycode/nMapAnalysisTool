/*
 * Author:George Karaszi
 * Date:11-5-2013
 * Description: Reads incoming data from the nmap application in its raw form. 
 *              Proceeds to parse and process the information into identifiable 
 *              variables. That will allow for storage and easy access later.
 *              
 * Programs Accessed: nmap.exe (Assumed that system environment's have been set)
 */

using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SharedLibrary
{

    public class RawData
    {

        public string PORT_NUM { get; set; }
        public string PORT_TYPE { get; set; }
        public string STATE { get; set; }
        public string SERVICE { get; set; }
        public string OS_TYPE { get; set; }
        public string OS_VERSION { get; set; }
        public string IP_ADDRESS { get; set; }

        private Process nMapProcess;

        public RawData(string Arguments)
        {
            nMapProcess = new Process();
            nMapProcess.StartInfo.FileName  = "nmap";
            nMapProcess.StartInfo.Arguments = Arguments;

            nMapProcess.StartInfo.UseShellExecute = false;
            nMapProcess.StartInfo.RedirectStandardOutput = true;
            nMapProcess.Start();
        }

        //-----------------------------------------------------------------------------------

        public RawData()
        {

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
            IPAddress IpAddress;

            while(!nMapProcess.StandardOutput.EndOfStream)
            {
                string Line = nMapProcess.StandardOutput.ReadLine();
                var LineSplit = Line.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);

                if (LineSplit.Length == 0)
                    continue;

                //Incoming string containing the IP can often be clouded with '(' IP ')'.
                string IPCheckString = LineSplit[LineSplit.Length - 1].Replace('(', ' ').Replace(')', ' ');

                if (IPAddress.TryParse(IPCheckString.Trim(), out IpAddress))
                {
                    IP_ADDRESS = IpAddress.ToString();
                }
                else
                {
                    //nMap always places port numbers first before information and status info
                    //EX Results: 80/TCP OPEN HTTP
                    var PortTypeSplit = LineSplit[0].Split('/');

                    if (PortTypeSplit.Length == 2 && LineSplit.Length >= 3 &&
                        Int32.TryParse(PortTypeSplit[0], out portTest))
                    {      

                        PORT_NUM          = PortTypeSplit[0];
                        PORT_TYPE         = PortTypeSplit[1];
                        STATE             = LineSplit[1];
                        SERVICE           = LineSplit[2];

                        return 0;
                    }
                    else
                    {

                        if (LineSplit[0].ToUpper().CompareTo("OS") == 0 && LineSplit.Length > 2)
                        {
                            //EX Results: OS details: Linux 2.6.32 - 2.6.39 (Might not have version Number)
                            OS_TYPE = LineSplit[2];

                            for (int i = 3; i < LineSplit.Length; i++)
                            {
                                OS_VERSION += LineSplit[i];
                            }

                            return 1;
                        }
                    }
                }

            }

            return -1;

        }
    }
}
