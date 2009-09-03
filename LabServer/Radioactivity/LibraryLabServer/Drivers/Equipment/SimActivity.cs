using System;
using System.Diagnostics;
using System.Threading;
using Library.Lab;

namespace Library.LabServer.Drivers.Equipment
{
    public class SimActivity
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "SimActivity";

        // Tube offset distance at initialisation
        private const int INT_TubeOffsetDistance = 14;

        // Tube distance at home position
        private const int INT_TubeHomeDistance = 20;

        //
        // Source table select and return times (secs) for positions A thru F
        //
        private const string STRCSV_SourceSelectTimes = "14,4,6,8,10,12";
        private const string STRCSV_SourceReturnTimes = "10,19,17,15,14,12";
        private const char CHR_SourceIndexStart = 'A';
        private const char CHR_SourceHomeLocation = 'F';

        //
        // Absorber table select and return times (secs) for position A
        //
        private const string STRCSV_AbsorberSelectTimes = "0";
        private const string STRCSV_AbsorberReturnTimes = "0";
        private const char CHR_AbsorberIndexStart = 'A';
        private const char CHR_AbsorberHomeLocation = 'A';

        // Comma-seperated value string splitter character
        private const char CHR_CsvSplitterChar = ',';

        // Rate at which the Geiger tube moves (milleseconds per millimeter)
        private const int TUBE_MOVE_RATE_MS_PER_MM = 520;

        //
        // Local variables
        //
        private static bool initialised = false;
        private int distance;
        private int duration;
        private int mean;
        private int deviation;
        private int tubeHomeDistance;
        private char sourceHomeLocation;
        private char absorberHomeLocation;
        private int tubeDistance;   // Remembers the current tube distance

        private struct AxisInfo
        {
            public int[] selectTimes;
            public int[] returnTimes;
        }

        private AxisInfo sourceAxisInfo;
        private AxisInfo absorberAxisInfo;

        #endregion

        #region Properties

        private bool simulateDelays;

        public bool SimulateDelays
        {
            get { return this.simulateDelays; }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public SimActivity()
        {
            //
            // Initialise local variables
            //
            this.tubeHomeDistance = INT_TubeHomeDistance;
            this.sourceHomeLocation = CHR_SourceHomeLocation;
            this.absorberHomeLocation = CHR_AbsorberHomeLocation;
            this.tubeDistance = INT_TubeOffsetDistance;

            //
            // Get simulation parameters from Application's configuration file
            //
            string strParams = Utilities.GetAppSetting(Consts.StrCfg_SimulationParameters);
            string[] strSplit = strParams.Split(new char[] { Consts.CHR_CsvSplitter });
            if (strSplit.Length != 4)
            {
                throw new ArgumentException();
            }
            this.distance = Convert.ToInt32(strSplit[0]);
            this.duration = Convert.ToInt32(strSplit[1]);
            this.mean = Convert.ToInt32(strSplit[2]);
            this.deviation = Convert.ToInt32(strSplit[3]);

            //
            // Determine whether delays should be simulated
            //
            this.simulateDelays = Utilities.GetBoolAppSetting(Consts.StrCfg_SimulateDelays);

            //
            // Initialise source select times array
            //
            strSplit = STRCSV_SourceSelectTimes.Split(new char[] { CHR_CsvSplitterChar });
            this.sourceAxisInfo.selectTimes = new int[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.sourceAxisInfo.selectTimes[i] = Int32.Parse(strSplit[i]);
            }

            //
            // Initialise source return times array
            //
            strSplit = STRCSV_SourceReturnTimes.Split(new char[] { CHR_CsvSplitterChar });
            this.sourceAxisInfo.returnTimes = new int[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.sourceAxisInfo.returnTimes[i] = Int32.Parse(strSplit[i]);
            }

            //
            // Initialise absorber axis info
            //
            this.absorberAxisInfo = new AxisInfo();

            //
            // Initialise absorber select times array
            //
            strSplit = STRCSV_AbsorberSelectTimes.Split(new char[] { CHR_CsvSplitterChar });
            this.absorberAxisInfo.selectTimes = new int[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.absorberAxisInfo.selectTimes[i] = Int32.Parse(strSplit[i]);
            }

            //
            // Initialise absorber return times array
            //
            strSplit = STRCSV_AbsorberReturnTimes.Split(new char[] { CHR_CsvSplitterChar });
            this.absorberAxisInfo.returnTimes = new int[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.absorberAxisInfo.returnTimes[i] = Int32.Parse(strSplit[i]);
            }

            if (initialised == false)
            {
                //
                // Set source, absorber and tube to their home positions
                //
                //SetTubeDistance(this.tubeHomeDistance);
                this.tubeDistance = this.tubeHomeDistance;
                //SetSourceLocation(this.sourceHomeLocation);
                //SetAbsorberLocation(this.absorberHomeLocation);

                initialised = true;
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public int GetTubeHomeDistance()
        {
            return this.tubeHomeDistance;
        }

        //-------------------------------------------------------------------------------------------------//

        public int GetTubeMoveTime(int startDistance, int endDistance)
        {
            // Get absolute distance
            int distance = endDistance - startDistance;
            if (distance < 0)
            {
                distance *= -1;
            }

            int seconds = (distance * TUBE_MOVE_RATE_MS_PER_MM) / 1000 + 1;

            return seconds;
        }

        //---------------------------------------------------------------------------------------//

        public char GetSourceHomeLocation()
        {
            return this.sourceHomeLocation;
        }

        //-------------------------------------------------------------------------------------------------//

        public int GetSourceSelectTime(char location)
        {
            int seconds = 0;

            int index = location - CHR_SourceIndexStart;
            if (index >= 0 && index < this.sourceAxisInfo.selectTimes.Length)
            {
                seconds = this.sourceAxisInfo.selectTimes[index] + 1;
            }

            return seconds;
        }

        //---------------------------------------------------------------------------------------//

        public int GetSourceReturnTime(char location)
        {
            int seconds = 0;

            int index = location - CHR_SourceIndexStart;
            if (index >= 0 && index < this.sourceAxisInfo.returnTimes.Length)
            {
                seconds = this.sourceAxisInfo.returnTimes[index] + 1;
            }

            return seconds;
        }

        //---------------------------------------------------------------------------------------//

        public char GetAbsorberHomeLocation()
        {
            return this.absorberHomeLocation;
        }

        //-------------------------------------------------------------------------------------------------//

        public int GetAbsorberSelectTime(char location)
        {
            int seconds = 0;

            int index = location - CHR_AbsorberHomeLocation;
            if (index >= 0 && index < this.absorberAxisInfo.selectTimes.Length)
            {
                seconds = this.absorberAxisInfo.selectTimes[index] + 1;
            }

            return seconds;
        }

        //---------------------------------------------------------------------------------------//

        public int GetAbsorberReturnTime(char location)
        {
            int seconds = 0;

            int index = location - CHR_AbsorberHomeLocation;
            if (index >= 0 && index < this.absorberAxisInfo.returnTimes.Length)
            {
                seconds = this.absorberAxisInfo.returnTimes[index] + 1;
            }

            return seconds;
        }

        //---------------------------------------------------------------------------------------//

        public bool SetTubeDistance(int targetDistance)
        {
            if (this.simulateDelays == true)
            {
                int seconds = this.GetTubeMoveTime(this.tubeDistance, targetDistance);
                for (int i = 0; i < seconds; i++)
                {
                    Thread.Sleep(1000);
                    Trace.Write("T");
                }
                Trace.WriteLine("");
            }

            this.tubeDistance = targetDistance;

            return true;
        }

        //---------------------------------------------------------------------------------------//

        public bool SetSourceLocation(char location)
        {
            if (this.simulateDelays == true)
            {
                int seconds = this.GetSourceSelectTime(location);
                for (int i = 0; i < seconds; i++)
                {
                    Thread.Sleep(1000);
                    Trace.Write("S");
                }
                Trace.WriteLine("");
            }

            return true;
        }

        //---------------------------------------------------------------------------------------//

        public bool SetAbsorberLocation(char location)
        {
            if (this.simulateDelays == true)
            {
                int seconds = this.GetAbsorberSelectTime(location);
                for (int i = 0; i < seconds; i++)
                {
                    Thread.Sleep(1000);
                    Trace.Write("A");
                }
                Trace.WriteLine("");
            }

            return true;
        }

        //---------------------------------------------------------------------------------------//

        public int[] GenerateData(int distance, int duration, int repeat)
        {
            //
            // Randomise the random number generator seed
            //
            int seed = DateTime.Now.Millisecond;
            Random random = new Random(seed);

            //
            // Generate Gaussian distribution of random numbers
            //
            double[] data = new double[repeat];
            for (int i = 0; i < repeat; i++)
            {
                data[i] = GetGaussian(random);
            }

            //
            // Adjust data for mean and standard deviation
            //
            data = AdjustForMeanStd(data, this.mean, this.deviation);

            //
            // Now adjust data for duration and distance
            //
            data = AdjustForDuration(data, Convert.ToDouble(duration));
            data = AdjustForDistance(data, Convert.ToDouble(distance));

            // Create array for simulated data
            int[] simData = new int[repeat];

            //
            // Copy to simulated data array
            //
            for (int i = 0; i < repeat; i++)
            {
                simData[i] = Convert.ToInt32(data[i]);
            }

            return simData;
        }

        //---------------------------------------------------------------------------------------//

        private double[] AdjustForDuration(double[] data, double duration)
        {
            double scale = duration / this.duration;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= scale;
            }
            return data;
        }

        private double[] AdjustForDistance(double[] data, double distance)
        {
            double dist = distance / this.distance;
            double sqrdist = dist * dist;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] /= sqrdist;
            }
            return data;
        }

        private double[] AdjustForMeanStd(double[] data, int mean, int std)
        {
            int[] idata = new int[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (data[i] * std) + mean;
            }
            return data;
        }

        private double GetGaussian(Random random)
        {
            double random1;
            for (; ; )
            {
                // random1 must be > 0.0 for Math.Log()
                random1 = random.NextDouble();
                if (random1 > 0.0)
                {
                    break;
                }
            }
            double random2 = random.NextDouble();

            double gaussian1 = Math.Sqrt(-2.0 * Math.Log(random1)) * Math.Cos(Math.PI * 2.0 * random2);

            // Don't need the second number
            //double gaussian2 = Math.Sqrt(-2.0 * Math.Log(random1)) * Math.Sin(Math.PI * 2.0 * random2);

            return gaussian1;
        }

    }

}
