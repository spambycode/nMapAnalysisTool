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

            SharedLibrary.RawData RawNMap = new RawData(args);
            SharedLibrary.StoreIndex CaptureData = new StoreIndex();
            int RawReturn = -1;

            while ((RawReturn = RawNMap.ReadRawLine()) != -1)
            {
                StoreIndex.InsertType InsertOption = (StoreIndex.InsertType)RawReturn;

                CaptureData.Add(RawNMap, InsertOption);
            }


        }
    }
}
