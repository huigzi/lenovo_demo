using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections;
using System.Threading.Tasks.Dataflow;

namespace Core
{
    public class UsbServer
    {
        private UsbDevice myUsbDevice;
        private readonly UsbDeviceFinder myUsbFinder;
        private UsbRegDeviceList regList;
        private UsbEndpointReader reader;
        
        private readonly TransformBlock<byte[], byte[]> transformBlock;

        public UsbServer(TransformBlock<byte[], byte[]> transformBlock)
        {
            myUsbFinder = new UsbDeviceFinder(0x0483, 0x572B);
            this.transformBlock = transformBlock;
        }

        public int UsbFind()
        {
            regList = UsbDevice.AllDevices.FindAll(myUsbFinder);
            if (regList.Count == 0)
            {
                return -1;
            }
            return 0;
        }

        public void Initialize()
        {

            if (myUsbDevice != null) return;

            if (regList.Count == 0) return;

            myUsbDevice = UsbDevice.OpenUsbDevice(myUsbFinder);
            var wholeUsbDevice = myUsbDevice as IUsbDevice;

            wholeUsbDevice?.SetConfiguration(1);
            wholeUsbDevice?.ClaimInterface(0);

            reader = myUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
            reader.DataReceived += OnRxEndPointData;
            reader.ReadBufferSize = 10800;
            reader.Reset();
            reader.DataReceivedEnabled = true;
        }

        private void OnRxEndPointData(object sender, EndpointDataEventArgs e)
        {
            transformBlock.Post(e.Buffer);
        }

        public void CloseUsb()
        {
            if (myUsbDevice == null) return;

            if (myUsbDevice.IsOpen)
            {
                try
                {
                    IUsbDevice wholeUsbDevice = myUsbDevice as IUsbDevice;
                    wholeUsbDevice?.ReleaseInterface(0);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }
            myUsbDevice = null;
            UsbDevice.Exit();
        }
    }
}
