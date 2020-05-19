using GHIElectronics.TinyCLR.Data.GpsParser;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace TestGPSModule
{
    class Program
    {
        static void Main()
        {
            GpsDevice device = new GpsDevice(SC20260.UartPort.Uart5);
            device.StartGPS();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
