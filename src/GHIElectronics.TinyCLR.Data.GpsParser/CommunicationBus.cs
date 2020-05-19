﻿using GHIElectronics.TinyCLR.Devices.Uart;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Data.GpsParser
{
    public class CommunicationsBus : IDisposable
    {
        private string TempData { set; get; } = string.Empty;
        private CommunicationsProtocal protocal;
        private StreamReader commandFile;
        private UartController serialPort;
        private bool disposed;
        public event IncomingDataEventHandler DataReceived;
        public delegate void IncomingDataEventHandler(string Data);
        public enum CommunicationsProtocal
        {
            Uart,
            Spi,
            I2C
        }
        public void SetBaudRate(int BaudRate)
        {
            serialPort.SetActiveSettings(BaudRate, 8, UartParity.None, UartStopBitCount.One, UartHandshake.None);
        }
        public CommunicationsBus(string UartPort, int BaudRate = 115200)
        {
            this.disposed = false;
            this.commandFile = null;
            this.protocal = CommunicationsProtocal.Uart;

            serialPort = UartController.FromName(UartPort);

            serialPort.SetActiveSettings(BaudRate, 8, UartParity.None, UartStopBitCount.One, UartHandshake.None);

            serialPort.Enable();

            serialPort.DataReceived += serialPort_DataReceived;

        }
        private void serialPort_DataReceived(UartController sender, DataReceivedEventArgs e)
        {
            var rxBuffer = new byte[e.Count];
            var bytesReceived = serialPort.Read(rxBuffer, 0, e.Count);
            var dataStr = Encoding.UTF8.GetString(rxBuffer, 0, bytesReceived);
            Debug.WriteLine(dataStr);
            TempData += dataStr;
            if (dataStr.IndexOf("\n") > -1)
            {
                DataReceived?.Invoke(TempData.Trim());
                TempData = string.Empty;
            }

        }
        public CommunicationsBus(string portName, StreamReader commandFile) : this(portName)
        {
            this.commandFile = commandFile;
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
            {
                if (this.commandFile != null)
                {
                    this.commandFile.Close();
                    this.commandFile.Dispose();
                    this.commandFile = null;
                }

                if (this.serialPort != null)
                {
                    this.serialPort.Disable();
                    this.serialPort.Dispose();
                    this.serialPort = null;
                }
            }

            this.disposed = true;
        }

        ~CommunicationsBus()
        {
            this.Dispose(false);
        }

        public void Write(string data)
        {
            data = data.Trim();

            switch (this.protocal)
            {
                case CommunicationsProtocal.I2C:
                    throw new NotImplementedException();

                case CommunicationsProtocal.Spi:
                    throw new NotImplementedException();

                case CommunicationsProtocal.Uart:
                    var buffer = Encoding.UTF8.GetBytes(data);
                    this.serialPort.Write(buffer, 0, buffer.Length);

                    break;
            }
        }

        public void Write(byte[] data)
        {
            switch (this.protocal)
            {
                case CommunicationsProtocal.I2C:
                    throw new NotImplementedException();

                case CommunicationsProtocal.Spi:
                    throw new NotImplementedException();

                case CommunicationsProtocal.Uart:
                    this.serialPort.Write(data, 0, data.Length);

                    break;
            }
        }

        public void WriteLine(string line)
        {
            line = line.Trim();

            switch (this.protocal)
            {
                case CommunicationsProtocal.I2C:
                    throw new NotImplementedException();

                case CommunicationsProtocal.Spi:
                    throw new NotImplementedException();

                case CommunicationsProtocal.Uart:

                    byte[] databytes = UTF8Encoding.UTF8.GetBytes(line + "\n");
                    this.serialPort.Write(databytes);

                    break;
            }
        }

        public bool ProcessCommandFromFile()
        {
            if (this.commandFile == null) throw new InvalidOperationException("This object was not initialized for a command file.");

            var line = this.commandFile.ReadLine(); //Possible make this non-blocking
            if (line == null)
                return false;

            this.WriteLine(line);

            return true;
        }

        public string ReadLine()
        {
            if (this.serialPort != null)
            {
                if (serialPort.BytesToRead > 0)
                {
                    var rxBuffer = new byte[serialPort.BytesToRead];
                    var bytesReceived = serialPort.Read(rxBuffer, 0, serialPort.BytesToRead);
                    var dataStr = Encoding.UTF8.GetString(rxBuffer, 0, bytesReceived);
                    return dataStr;
                }
                //this.serialPort.read();
            }
            else if (this.commandFile != null)
            {
                return this.commandFile.ReadLine(); //Handle end of file
            }
            else
            {
                throw new NotImplementedException();
            }
            return string.Empty;
        }

        public void Read(byte[] data)
        {
            if (this.serialPort != null)
            {
                this.serialPort.Read(data, 0, data.Length);
            }
            else if (this.commandFile != null)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
