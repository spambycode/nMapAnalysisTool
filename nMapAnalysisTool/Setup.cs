using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SharedLibrary;

namespace nMapAnalysisTool
{
    class Setup
    {
        static void Main(string[] args)
        {
            string IP = args[args.Length];
            string[] Arguements = new string[args.Length - 1];
            Array.Copy(args, Arguements, args.Length - 1);

            SharedLibrary.RawData RawNMap = new RawData(IP, Arguements);
            SharedLibrary.MainData CaptureData = new MainData();

            while (RawNMap.ReadRawLine() != -1)
            { 
                
            }
        }
    }
}
