﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SharedLibrary;
using SqlInterface;

namespace nMapAnalysisTool
{
    class Setup
    {
        static void Main(string[] args)
        {
            string commandInput = string.Empty;
            int RawReturn = -1;
            SharedLibrary.RawData RawNMap         = new RawData();
            SharedLibrary.StoreIndex CaptureData  = new StoreIndex();
            SqlInterface.SqlInterface SqlDatabase = new SqlInterface.SqlInterface("127.0.0.1",
                                                                                  "nmapanalysistool", 
                                                                                  "root", "root");

            Console.Write("nMap Options:");
            commandInput = Console.ReadLine();

            while (commandInput.ToUpper().CompareTo("STOP") != 0)
            {
                RawNMap = new RawData(commandInput);

                while ((RawReturn = RawNMap.ReadRawLine()) != -1)
                {
                    StoreIndex.InsertType InsertOption = (StoreIndex.InsertType)RawReturn;

                    CaptureData.Add(RawNMap, InsertOption);
                }

                Console.Write("\nnMap Options:");
                commandInput = Console.ReadLine();
            }

            SqlDatabase.QueryIPLists(CaptureData.GetTreeRoot);
            
            Console.WriteLine("All entries have been entered to database");
            Console.ReadLine();


        }
    }
}
