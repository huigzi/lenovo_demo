using Core.Interface;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;

namespace Core
{
    public class UsbServer: IService
    {
        private UsbDevice myUsbDevice;
        private readonly UsbDeviceFinder myUsbFinder;
        private UsbEndpointReader reader;
        private readonly ILogger logger;
        
        private readonly DataFlowBlock transformBlock;

        public UsbServer(DataFlowBlock transformBlock, ILogger logger)
        {
            myUsbFinder = new UsbDeviceFinder(0x0483, 0x572B);
            this.transformBlock = transformBlock;
            this.logger = logger;
        }

        public class UsbNotFoundException : ApplicationException
        {
            public UsbNotFoundException(string message) : base(message)
            {

            }
        }

        public void OpenServer()
        {
            try
            {
                if (myUsbDevice != null) throw new NotSupportedException("Device Exits");
                if (UsbDevice.AllDevices.FindAll(myUsbFinder).Count == 0) throw new UsbNotFoundException("Device Not Found. Please Restart the application");

                myUsbDevice = UsbDevice.OpenUsbDevice(myUsbFinder);
                var wholeUsbDevice = myUsbDevice as IUsbDevice;

                wholeUsbDevice?.SetConfiguration(1);
                wholeUsbDevice?.ClaimInterface(0);

                reader = myUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
                reader.DataReceived += OnRxEndPointData;
                reader.ReadBufferSize = 5600;
                reader.Reset();
                reader.DataReceivedEnabled = true;
            }
            catch(UsbNotFoundException e)
            {
                logger.WriteToLog(e.ToString());
                throw e;
            }
        }

        private void OnRxEndPointData(object sender, EndpointDataEventArgs e)
        {
            transformBlock.Post(e.Buffer);
        }

        public void CloseServer()
        {
            try
            {
                if (myUsbDevice == null) throw new NullReferenceException("No device closed");
                if (myUsbDevice.IsOpen)
                {
                    IUsbDevice wholeUsbDevice = myUsbDevice as IUsbDevice;
                    wholeUsbDevice?.ReleaseInterface(0);
                }
                myUsbDevice = null;
                UsbDevice.Exit();
            }
            catch (Exception ex)
            {
                logger.WriteToLog(ex.ToString());
            }
        }
    }
}
