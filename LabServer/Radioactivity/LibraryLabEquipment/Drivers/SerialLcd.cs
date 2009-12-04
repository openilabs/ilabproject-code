using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Xml;
using Library.Lab;

namespace Library.LabEquipment.Drivers
{
    public class SerialLcd : IDisposable
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "SerialLcd";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_NotInitialised = " Not Initialised!";
        private const string STRLOG_Initialising = " Initialising...";
        private const string STRLOG_Online = " Online: ";
        private const string STRLOG_disposing = " disposing: ";
        private const string STRLOG_disposed = " disposed: ";
        private const string STRLOG_SerialPort = " SerialPort: ";
        private const string STRLOG_BaudRate = " BaudRate: ";
        private const string STRLOG_Success = " Success: ";
        private const string STRLOG_OpeningSerialPort = " Opening serial port...";
        private const string STRLOG_SerialLcdThreadsAreStarting = " SerialLcd threads are starting...";
        private const string STRLOG_SerialLcdThreadsAreRunning = " SerialLcd threads are running.";

        //
        // String constants for error messages
        //
        private const string STRERR_SerialLcdThreadsFailedToStart = "SerialLcd threads failed to start!";

        //
        // Command and report packet types
        //
        private const byte PKTRPT_KEY_ACTIVITY = 0x80;
        private const byte PKTRPT_FAN_SPEED = 0x81;
        private const byte PKTRPT_TEMP_SENSOR = 0x82;
        private const byte PKTRPT_CAPTURE_DATA = 0x83;

        private const int MAX_DATA_LENGTH = 16;
        private const int MAX_PACKET_LENGTH = MAX_DATA_LENGTH + 4;
        private const int LINE_LENGTH = 16;
        private const int MAX_RESPONSE_TIME = 500;
        private const ushort CRC_SEED = 0xFFFF;

        //
        // Local variables
        //
        private bool disposed;
        private bool initialised;
        private string lastError;
        private SerialPort serialPort;
        private byte[] packetXMitBuffer;
        private byte[] packetRcvBuffer;
        private LCDPacket responsePacket;
        private Object responseSignal;
        private Queue<LCDPacket> reportQueue;
        private Object reportSignal;
        private Thread receiveThread;
        private Thread reportThread;
        private bool receiveRunning;
        private bool reportRunning;

        #endregion

        #region Properties

        public const int DELAY_INITIALISE = 2;

        private bool online;
        private string statusMessage;
        private bool timedOut;
        private int captureData;
        private LCDKey lcdKey;

        /// <summary>
        /// Returns the time (in seconds) that it takes for the equipment to initialise.
        /// </summary>
        public int InitialiseDelay
        {
            get { return DELAY_INITIALISE; }
        }

        /// <summary>
        /// Returns true if the hardware has been initialised successfully and is ready for use.
        /// </summary>
        public bool Online
        {
            get { return this.online; }
        }

        public string StatusMessage
        {
            get { return this.statusMessage; }
        }

        public bool IsOpen
        {
            get
            {
                if (serialPort == null)
                {
                    return false;
                }
                else
                {
                    return serialPort.IsOpen;
                }
            }
        }

        public bool TimedOut
        {
            get { return timedOut; }
            set { timedOut = value; }
        }

        public int CaptureData
        {
            get
            {
                int data;

                lock (this)
                {
                    data = this.captureData;
                    this.captureData = -1;
                }
                return data;
            }
            set
            {
                lock (this)
                {
                    this.captureData = value;
                }
            }
        }

        public LCDKey Key
        {
            get { return lcdKey; }
            set { lcdKey = value; }
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        public enum LCDKey
        {
            None = 0, Up = 1, Down = 2, Left = 3, Right = 4, Enter = 5, Exit = 6
        }

        //---------------------------------------------------------------------------------------//

        public enum LCDPktType : byte
        {
            GetVersion = 1, WriteLine1 = 7, WriteLine2 = 8, SetCapture = 36
        }

        //---------------------------------------------------------------------------------------//

        public SerialLcd(XmlNode xmlNodeEquipmentConfig)
        {
            const string STRLOG_MethodName = "SerialLcd";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Initialise local variables
            //
            this.disposed = true;
            this.initialised = false;
            this.lastError = null;
            this.receiveRunning = false;
            this.reportRunning = false;
            this.timedOut = false;
            this.captureData = -1;
            this.lcdKey = LCDKey.None;

            //
            // Get the serial port to use and the baud rate
            //
            XmlNode xmlNode = XmlUtilities.GetXmlNode(xmlNodeEquipmentConfig, Consts.STRXML_serialLcd);
            string serialport = XmlUtilities.GetXmlValue(xmlNode, Consts.STRXML_port, false);
            int baudrate = XmlUtilities.GetIntValue(xmlNode, Consts.STRXML_baud);
            Logfile.Write(STRLOG_SerialPort + serialport);
            Logfile.Write(STRLOG_BaudRate + baudrate.ToString());

#if NO_HARDWARE
#else
            //
            // Create an instance of the serial port, set read and write timeouts
            //
            serialPort = new SerialPort(serialport, baudrate);
            serialPort.ReadTimeout = 3000;
            serialPort.WriteTimeout = 3000;

            //
            // Create the receive and transmit buffers
            //
            packetRcvBuffer = new byte[MAX_PACKET_LENGTH];
            packetXMitBuffer = new byte[MAX_PACKET_LENGTH];

            //
            // Create the report queue and report/response signal objects
            //
            reportQueue = new Queue<LCDPacket>();
            reportSignal = new Object();
            responseSignal = new Object();
#endif

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        public string GetLastError()
        {
            string lastError = this.lastError;
            this.lastError = null;
            return lastError;
        }

        //---------------------------------------------------------------------------------------//

        public bool Initialise()
        {
            const string STRLOG_MethodName = "Initialise";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = false;

            //
            // Check if this is first-time initialisation
            //
            if (this.initialised == false)
            {
                this.statusMessage = STRLOG_Initialising;

                //
                // Nothing to do here
                //

                //
                // First-time initialisation is complete
                //
                this.initialised = true;
            }

            //
            // Initialisation that must be done each time the equipment is powered up
            //
            try
            {
                this.disposed = false;

#if NO_HARDWARE
#else
                //
                // Open the serial
                //
                Logfile.Write(STRLOG_OpeningSerialPort);
                serialPort.Open();

                //
                // Create and start the report and receive threads
                //
                Logfile.Write(STRLOG_SerialLcdThreadsAreStarting);
                reportThread = new Thread(new ThreadStart(ReportHandler));
                reportThread.Start();
                receiveThread = new Thread(new ThreadStart(ReceiveHandler));
                receiveThread.Start();

                //
                // Give the thread a chance to start and then check that it has started
                //
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    if (this.receiveRunning == true && this.reportRunning == true)
                    {
                        Logfile.Write(STRLOG_SerialLcdThreadsAreRunning);
                        success = true;
                        break;
                    }
                    Trace.Write('?');
                }
                if (success == false)
                {
                    Logfile.WriteError(STRERR_SerialLcdThreadsFailedToStart);
                }

                //
                // Get the firmware version and display
                //
                string firmwareVersion = this.GetHardwareFirmwareVersion();
                this.WriteLine(1, STRLOG_ClassName);
                this.WriteLine(2, firmwareVersion);

                //
                // Ensure data capture is stopped
                //
                this.StopCapture();
#endif

                //
                // Initialisation is complete
                //
                this.online = true;
                this.statusMessage = StatusCodes.Ready.ToString();

                success = true;
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
                this.Close();
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //---------------------------------------------------------------------------------------//

        #region Close and Dispose

        /// <summary>
        /// Do not make this method virtual. A derived class should not be allowed to override this method.
        /// </summary>
        public void Close()
        {
            const string STRLOG_MethodName = "Close";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Calls the Dispose method without parameters
            Dispose();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Implement IDisposable. Do not make this method virtual. A derived class should not be able
        /// to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Take yourself off the Finalization queue to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Use C# destructor syntax for finalization code. This destructor will run only if the Dispose
        /// method does not get called. It gives your base class the opportunity to finalize. Do not provide
        /// destructors in types derived from this class.
        /// </summary>
        ~SerialLcd()
        {
            Trace.WriteLine("~SerialLcd():");

            //
            // Do not re-create Dispose clean-up code here. Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            //
            Dispose(false);
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios:
        /// 1. If disposing equals true, the method has been called directly or indirectly by a user's code.
        ///    Managed and unmanaged resources can be disposed.
        /// 2. If disposing equals false, the method has been called by the runtime from inside the finalizer
        ///    and you should not reference other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            const string STRLOG_MethodName = "Dispose";

            string logMessage = STRLOG_disposing + disposing.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_disposed + this.disposed.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            //
            // Check to see if Dispose has already been called
            //
            if (this.disposed == false)
            {
                //
                // If disposing equals true, dispose all managed and unmanaged resources.
                //
                if (disposing == true)
                {
                    // Dispose managed resources here. Anything that has a Dispose() method.
                }

                //
                // Release unmanaged resources here. If disposing is false, only the following
                // code is executed.
                //

#if NO_HARDWARE
#else
                //
                // Stop the receive and report threads and close the serial port
                //
                if (this.receiveRunning == true)
                {
                    this.receiveRunning = false;
                    this.receiveThread.Join();
                }
                if (this.reportRunning == true)
                {
                    this.reportRunning = false;
                    this.reportThread.Join();
                }
                if (this.serialPort != null && serialPort.IsOpen)
                {
                    this.serialPort.Close();
                }
#endif

                this.disposed = true;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        public string GetHardwareFirmwareVersion()
        {
            byte[] version = SendReturnData((byte)LCDPktType.GetVersion, 0, null);

            if (version != null)
            {
                return Encoding.ASCII.GetString(version);
            }

            return null;
        }

        //---------------------------------------------------------------------------------------//

        public bool WriteLine(int lineno, string message)
        {
            switch (lineno)
            {
                case 1:
                    return WriteLine((byte)LCDPktType.WriteLine1, message);

                case 2:
                    return WriteLine((byte)LCDPktType.WriteLine2, message);
            }
            return false;
        }

        //---------------------------------------------------------------------------------------//

        public bool StartCapture(int seconds)
        {
            byte[] data = new byte[1] { (byte)seconds };

            CaptureData = -1;
            return SendReturnBool((byte)LCDPktType.SetCapture, 1, data);
        }

        //---------------------------------------------------------------------------------------//

        public bool StopCapture()
        {
            byte[] data = new byte[1] { 0 };

            return SendReturnBool((byte)LCDPktType.SetCapture, 1, data);
        }

        //---------------------------------------------------------------------------------------//

        #region Private Methods

        private bool WriteLine(byte type, string line)
        {
            line = CreateStringOfLength(line, LINE_LENGTH);
            return SendReturnBool(type, LINE_LENGTH, Encoding.ASCII.GetBytes(line.ToCharArray()));
        }

        private string CreateStringOfLength(string s, int length)
        {
            if (length < s.Length)
            {
                s = s.Substring(0, length);
            }
            else if (length > s.Length)
            {
                s = s + (new String(' ', length - s.Length));
            }

            return s;
        }

        private bool SendReturnBool(byte type, byte dataLength, byte[] data)
        {
#if NO_HARDWARE
            return true;
#else
            LCDPacket packet = Send(type, dataLength, data);

            if (packet != null)
            {
                return (type == (packet.Type & ~0xC0) &&
                    responsePacket.PacketType == LCDPacket.LCDPacketType.NORMAL_RESPONSE);
            }
            else
            {
                return false;
            }
#endif
        }

        private byte[] SendReturnData(byte type, byte dataLength, byte[] data)
        {
            LCDPacket packet = Send(type, dataLength, data);

            if (packet != null)
            {
                return packet.Data;
            }
            else
            {
                return null;
            }
        }

        private LCDPacket Send(byte type, byte dataLength, byte[] data)
        {
            ushort crc;
            bool error = false;

            if (data == null)
            {
                if (dataLength != 0)
                {
                    error = true;
                }
            }
            else if (dataLength > data.Length)
            {
                error = true;
            }

            if (error == true)
            {
                throw new ArgumentException("bad data sent to Send");
            }

            // Enter header information
            packetXMitBuffer[0] = type;
            packetXMitBuffer[1] = dataLength;

            // Enter data, if any
            if (dataLength != 0)
            {
                Array.Copy(data, 0, packetXMitBuffer, 2, dataLength);
            }

            // Calculate and enter the checksum
            crc = CRCGenerator.GenerateCRC(packetXMitBuffer, dataLength + 2, CRC_SEED);
            packetXMitBuffer[2 + dataLength + 1] = (byte)(crc >> 8);
            packetXMitBuffer[2 + dataLength] = (byte)crc;

            //Trace.WriteLine("Send(): serialPort.Write");
            lock (responseSignal)
                {
                responsePacket = null;

                // Write the packet to the serial port
                serialPort.Write(packetXMitBuffer, 0, dataLength + 4);

                // Wait for the response packet
                if (Monitor.Wait(responseSignal, MAX_RESPONSE_TIME))
                {
                    //Trace.WriteLine("Send(): return responsePacket");
                    return responsePacket;
                }
            }

            Trace.WriteLine("Send(): return null");
            return null;
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        #region Receive and Report Handler Threads

        private void ReceiveHandler()
        {
            const string STRLOG_MethodName = "ReceiveHandler";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            byte[] receiveBuffer = new byte[128];
            int bytesRead = 0;
            int bufferIndex = 0;
            int startPacketIndex = 0;
            int expectedPacketLength = -1;
            bool expectedPacketLengthIsSet = false;
            int numBytesToRead = receiveBuffer.Length;

            try
            {
                this.receiveRunning = true;
                while (this.receiveRunning == true)
                {
                    if (expectedPacketLengthIsSet || bytesRead <= 1)
                    {
                        //If the expectedPacketLength has been or no bytes have been read
                        //This covers the case that more then 1 entire packet has been read in at a time
                        // comPort
                        try
                        {
                            bytesRead += this.serialPort.Read(receiveBuffer, bufferIndex, numBytesToRead);
                            Trace.WriteLine("ReceiveHandler: bytesRead=" + bytesRead.ToString());

                            bufferIndex = startPacketIndex + bytesRead;
                        }
                        catch (TimeoutException)
                        {
                            timedOut = true;
                            Trace.WriteLine("TimeoutException:");
                        }
                    }

                    if (bytesRead > 1)
                    {
                        //The buffer has the dataLength for the packet
                        if (!expectedPacketLengthIsSet)
                        {
                            //If the expectedPacketLength has not been set for this packet
                            expectedPacketLength = receiveBuffer[(1 + startPacketIndex) % receiveBuffer.Length] + 4;
                            expectedPacketLengthIsSet = true;
                        }

                        if (bytesRead >= expectedPacketLength)
                        {
                            //The buffer has at least as many bytes for this packet
                            AddPacket(receiveBuffer, startPacketIndex);
                            expectedPacketLengthIsSet = false;
                            if (bytesRead == expectedPacketLength)
                            {
                                //The buffer contains only the bytes for this packet
                                bytesRead = 0;
                                bufferIndex = startPacketIndex;
                            }
                            else
                            {
                                //The buffer also has bytes for the next packet
                                startPacketIndex += expectedPacketLength;
                                startPacketIndex %= receiveBuffer.Length;
                                bytesRead -= expectedPacketLength;
                                bufferIndex = startPacketIndex + bytesRead;
                            }
                        }
                    }

                    bufferIndex %= receiveBuffer.Length;
                    numBytesToRead = bufferIndex < startPacketIndex ? startPacketIndex - bufferIndex : receiveBuffer.Length - bufferIndex;
                }
            }
            catch (IOException)
            {
                this.receiveRunning = false;
                Thread.CurrentThread.Abort();
            }
            catch (ObjectDisposedException)
            {
                this.receiveRunning = false;
                if (receiveThread != null)
                {
                    receiveThread = null;
                }
            }
            Trace.WriteLine("ReceiveHandler(): Exiting");

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //---------------------------------------------------------------------------------------//

        private void ReportHandler()
        {
            const string STRLOG_MethodName = "ReportHandler";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            try
            {
                this.reportRunning = true;
                while (this.reportRunning == true)
                {
                    lock (reportSignal)
                    {
                        if (Monitor.Wait(reportSignal, 2000) == false)
                        {
                            // Timeout
                            continue;
                        }
                    }
                    if (reportQueue.Count != 0)
                    {
                        LCDPacket packet = reportQueue.Dequeue();

                        switch (packet.Type)
                        {
                            case PKTRPT_CAPTURE_DATA:
                                int value = packet.Data[1];
                                value = (value << 8) + packet.Data[0];
                                CaptureData = value;
                                break;

                            case PKTRPT_KEY_ACTIVITY:
                                if (packet.DataLength == 1)
                                {
                                    switch (packet.Data[0])
                                    {
                                        case 5:
                                            Key = LCDKey.Enter;
                                            break;

                                        case 6:
                                            Key = LCDKey.Exit;
                                            break;
                                    }
                                }
                                break;

                            case PKTRPT_TEMP_SENSOR:
                                break;
                        }
                    }
                }
            }
            catch (IOException)
            {
                this.reportRunning = false;
                Thread.CurrentThread.Abort();
            }
            catch (ObjectDisposedException)
            {
                this.reportRunning = false;
                if (reportThread != null)
                {
                    reportThread = null;
                }
            }
            Trace.WriteLine("ReportHandler(): Exiting");

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        #region Packet Handling Routines

        private LCDPacket CreatePacket(byte[] buffer, int startIndex)
        {
            byte type = buffer[startIndex];
            byte dataLength = buffer[(startIndex + 1) % buffer.Length];
            byte[] data = new byte[dataLength];
            ushort crc = 0;

            for (int i = 0; i < dataLength; i++)
            {
                data[i] = buffer[(startIndex + 2 + i) % buffer.Length];
            }

            crc |= (ushort)buffer[(startIndex + 2 + dataLength) % buffer.Length];
            crc |= (ushort)(buffer[(startIndex + 2 + dataLength + 1) % buffer.Length] << 8);
            return new LCDPacket(type, dataLength, data, crc);
        }

        private bool AddPacket(byte[] buffer, int startIndex)
        {
            //Trace.WriteLine("AddPacket():");

            LCDPacket packet = CreatePacket(buffer, startIndex);
            ushort calculatedCRC = CRCGenerator.GenerateCRC(buffer, startIndex, packet.DataLength + 2, CRC_SEED);

            switch (packet.PacketType)
            {
                case LCDPacket.LCDPacketType.NORMAL_RESPONSE:
                    AddResponsePacket(packet);
                    break;

                case LCDPacket.LCDPacketType.NORMAL_REPORT:
                    AddReportPacket(packet);
                    break;

                case LCDPacket.LCDPacketType.ERROR_RESPONSE:
                    AddResponsePacket(packet);
                    break;
            }
            if (calculatedCRC != packet.CRC)
            {
                Trace.WriteLine(
                    "CRC ERROR!!!: Calculated CRC=" + Convert.ToString(calculatedCRC, 16) +
                    "Actual CRC=" + packet.CRC.ToString()
                    );
                return false;
            }

            return true;
        }

        private void AddResponsePacket(LCDPacket packet)
        {
            lock (responseSignal)
                {
                responsePacket = packet;
                Monitor.Pulse(responseSignal);
            }
        }

        private void AddReportPacket(LCDPacket packet)
        {
            lock (reportSignal)
            {
                reportQueue.Enqueue(packet);
                Monitor.Pulse(reportSignal);
            }
        }

        #endregion
    }

    //---------------------------------------------------------------------------------------//

    #region LCDPacket Class

    public class LCDPacket
    {
        public enum LCDPacketType {
            NORMAL_RESPONSE,
            NORMAL_REPORT,
            ERROR_RESPONSE
        };

        private const byte NORMAL_RESPONSE = 0x40;
        private const byte NORMAL_REPORT = 0x80;
        private const byte ERROR_RESPONSE = 0xC0;

        private byte type;
        private byte dataLength;
        private byte[] data;
        private ushort crc;

        public LCDPacket(byte type, byte dataLength, byte[] data)
        {
            this.type = type;
            this.dataLength = dataLength;
            this.data = data;
        }

        public LCDPacket(byte type, byte dataLength, byte[] data, ushort crc)
        {
            this.type = type;
            this.dataLength = dataLength;
            this.data = data;
            this.crc = crc;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("Type: " + Convert.ToString(type, 16) + "\n");
            sb.Append("DataLength: " + Convert.ToString(dataLength, 16) + "\n");
            sb.Append("Data: ");
            for (int i = 0; i < dataLength; i++)
            {
                sb.Append(Convert.ToString(data[i], 16) + ", ");
            }

            sb.Append("\n");
            sb.Append("CRC: " + Convert.ToString(crc, 16) + "\n");
            return sb.ToString();
        }

        public byte Type
        {
            get { return type; }
        }

        public LCDPacketType PacketType
        {
            get
            {
                switch (type & 0xC0)
                {
                    case NORMAL_RESPONSE:
                        return LCDPacketType.NORMAL_RESPONSE;

                    case NORMAL_REPORT:
                        return LCDPacketType.NORMAL_REPORT;

                    case ERROR_RESPONSE:
                        return LCDPacketType.ERROR_RESPONSE;

                    default:
                        //throw new InvalidOperationException("Unexpected Packet Type: " +
                        //    System.Convert.ToString(type, 16));
                        return LCDPacketType.ERROR_RESPONSE;
                }
            }
        }

        public byte DataLength
        {
            get
            {
                return dataLength;
            }
        }
        public byte[] Data
        {
            get
            {
                return data;
            }
        }
        public ushort CRC
        {
            get
            {
                return crc;
            }
        }
    }

    #endregion

    //---------------------------------------------------------------------------------------//

    #region CRCGenerator Class

    public static class CRCGenerator
    {
        //CRC lookup table to avoid bit-shifting loops.
        static ushort[] crcLookupTable = {
			0x00000, 0x01189, 0x02312, 0x0329B, 0x04624, 0x057AD, 0x06536, 0x074BF,
			0x08C48, 0x09DC1, 0x0AF5A, 0x0BED3, 0x0CA6C, 0x0DBE5, 0x0E97E, 0x0F8F7,
			0x01081, 0x00108, 0x03393, 0x0221A, 0x056A5, 0x0472C, 0x075B7, 0x0643E,
			0x09CC9, 0x08D40, 0x0BFDB, 0x0AE52, 0x0DAED, 0x0CB64, 0x0F9FF, 0x0E876,
			0x02102, 0x0308B, 0x00210, 0x01399, 0x06726, 0x076AF, 0x04434, 0x055BD,
			0x0AD4A, 0x0BCC3, 0x08E58, 0x09FD1, 0x0EB6E, 0x0FAE7, 0x0C87C, 0x0D9F5,
			0x03183, 0x0200A, 0x01291, 0x00318, 0x077A7, 0x0662E, 0x054B5, 0x0453C,
			0x0BDCB, 0x0AC42, 0x09ED9, 0x08F50, 0x0FBEF, 0x0EA66, 0x0D8FD, 0x0C974,
			0x04204, 0x0538D, 0x06116, 0x0709F, 0x00420, 0x015A9, 0x02732, 0x036BB,
			0x0CE4C, 0x0DFC5, 0x0ED5E, 0x0FCD7, 0x08868, 0x099E1, 0x0AB7A, 0x0BAF3,
			0x05285, 0x0430C, 0x07197, 0x0601E, 0x014A1, 0x00528, 0x037B3, 0x0263A,
			0x0DECD, 0x0CF44, 0x0FDDF, 0x0EC56, 0x098E9, 0x08960, 0x0BBFB, 0x0AA72,
			0x06306, 0x0728F, 0x04014, 0x0519D, 0x02522, 0x034AB, 0x00630, 0x017B9,
			0x0EF4E, 0x0FEC7, 0x0CC5C, 0x0DDD5, 0x0A96A, 0x0B8E3, 0x08A78, 0x09BF1,
			0x07387, 0x0620E, 0x05095, 0x0411C, 0x035A3, 0x0242A, 0x016B1, 0x00738,
			0x0FFCF, 0x0EE46, 0x0DCDD, 0x0CD54, 0x0B9EB, 0x0A862, 0x09AF9, 0x08B70,
			0x08408, 0x09581, 0x0A71A, 0x0B693, 0x0C22C, 0x0D3A5, 0x0E13E, 0x0F0B7,
			0x00840, 0x019C9, 0x02B52, 0x03ADB, 0x04E64, 0x05FED, 0x06D76, 0x07CFF,
			0x09489, 0x08500, 0x0B79B, 0x0A612, 0x0D2AD, 0x0C324, 0x0F1BF, 0x0E036,
			0x018C1, 0x00948, 0x03BD3, 0x02A5A, 0x05EE5, 0x04F6C, 0x07DF7, 0x06C7E,
			0x0A50A, 0x0B483, 0x08618, 0x09791, 0x0E32E, 0x0F2A7, 0x0C03C, 0x0D1B5,
			0x02942, 0x038CB, 0x00A50, 0x01BD9, 0x06F66, 0x07EEF, 0x04C74, 0x05DFD,
			0x0B58B, 0x0A402, 0x09699, 0x08710, 0x0F3AF, 0x0E226, 0x0D0BD, 0x0C134,
			0x039C3, 0x0284A, 0x01AD1, 0x00B58, 0x07FE7, 0x06E6E, 0x05CF5, 0x04D7C,
			0x0C60C, 0x0D785, 0x0E51E, 0x0F497, 0x08028, 0x091A1, 0x0A33A, 0x0B2B3,
			0x04A44, 0x05BCD, 0x06956, 0x078DF, 0x00C60, 0x01DE9, 0x02F72, 0x03EFB,
			0x0D68D, 0x0C704, 0x0F59F, 0x0E416, 0x090A9, 0x08120, 0x0B3BB, 0x0A232,
			0x05AC5, 0x04B4C, 0x079D7, 0x0685E, 0x01CE1, 0x00D68, 0x03FF3, 0x02E7A,
			0x0E70E, 0x0F687, 0x0C41C, 0x0D595, 0x0A12A, 0x0B0A3, 0x08238, 0x093B1,
			0x06B46, 0x07ACF, 0x04854, 0x059DD, 0x02D62, 0x03CEB, 0x00E70, 0x01FF9,
			0x0F78F, 0x0E606, 0x0D49D, 0x0C514, 0x0B1AB, 0x0A022, 0x092B9, 0x08330,
			0x07BC7, 0x06A4E, 0x058D5, 0x0495C, 0x03DE3, 0x02C6A, 0x01EF1, 0x00F78
		};

        public static ushort GenerateCRC(byte[] data, int dataLength, ushort seed)
        {
            ushort newCrc;

            newCrc = seed;
            for (int i = 0; i < dataLength; i++)
            {
                newCrc = (ushort)((newCrc >> 8) ^ crcLookupTable[(newCrc ^ data[i]) & 0xff]);
            }

            return ((ushort)~newCrc);
        }

        public static ushort GenerateCRC(byte[] data, int startIndex, int length, ushort seed)
        {
            ushort newCrc;

            newCrc = seed;
            for (int i = 0; i < length; i++)
            {
                newCrc = (ushort)((newCrc >> 8) ^ crcLookupTable[(newCrc ^ data[(startIndex + i) % data.Length]) & 0xff]);
            }

            return ((ushort)~newCrc);
        }
    }

    #endregion

}
