using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataPusher
{

    class DataPusher
    {
        private string outPath = "C:\\Campbellsci\\LoggerNet\\CR1000_TEST_CHAMBER_Table105.dat";
        private string inPath = "new_format.dat";

        private void sleep(int milliseconds){
            System.Threading.Thread.Sleep(milliseconds);
        }

        public void run()
        {
            StreamReader r = File.OpenText(inPath);
            string line;
            while ((line = r.ReadLine()) != null)
            {
                StreamWriter w = File.AppendText(outPath);
                w.WriteLine(line);
                w.Close();
                System.Console.WriteLine(line);
                sleep(5000);
            }
            
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            DataPusher dummy = new DataPusher();
            dummy.run();
            
        }
    }
}
