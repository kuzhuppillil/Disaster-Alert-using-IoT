using DisasterAlertNode.Core.Serial.Configurator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace DisasterAlertNode.Core.Serial
{
    class BTLESerial : GenericSerialPort
    {

        DeviceConfigurator _configurator = new DeviceConfigurator();
        private object _lockInstance = new object();
        string connectedDevice = String.Empty;

        public override void Configure(uint baudRate, uint writeTimeout, uint readTimeout)
        {
            base.Configure(baudRate, writeTimeout, readTimeout);
            _configurator.OnConfigurationStarted += OnDeviceConfigurationStarted;
            _configurator.OnDeviceConfigured += OnBTLEDeviceConfigured;
            _configurator.OnDeviceConfigurationFailed += OnDeviceConfigurationFailed;
            _configurator.ConfigureDevice();
            _configurator.StartConfiguration();
        }

        private void OnDeviceConfigurationFailed()
        {
            base.OnDeviceDisconnected();
            this.connectedDevice = String.Empty;
        }

        private void OnBTLEDeviceConfigured(string deviceId)
        {
            base.OnDeviceConfigured(deviceId);
            this.connectedDevice = deviceId;
        }

        private void OnDeviceConfigurationStarted()
        {
            this.connectedDevice = String.Empty;
        }

        internal string ConnectedDevice { get { return connectedDevice; } }

        internal override void OnDataReceived(string dataReceived)
        {
            Debug.WriteLine(String.Format("Data Received - {0}", dataReceived));
            // Parse the data and chek whether it is for BTLE for configuration purpose.
            // Need to write a message parsing routine, which will parse the message based on inputs given.
            if (_configurator.IsDeviceConfigured == false)
            {
                _configurator.HandleConfigurationResponse(dataReceived);
                return; // No need to return this back to device.
            }

            // One need to see if link is established or disconnected, based on that we have to handle all responses.
            if (String.Equals(dataReceived, "OK+CONN"))
            {
                Debug.WriteLine("Established connection! We just need to ignore this message!");
                this.OnBTLEDeviceConfigured("");    // Just empty id, to show that it is getting called.
                // Data received. Device is connected, so let's try to send some data. Just return from this function.
                return;
            } else if (String.Equals(dataReceived, "OK+LOST"))
            {
                this.OnDeviceConfigurationFailed();
                // Connection lost, we need to rest the interfaces and redo connection again.
                _configurator.RestartConfiguration();   // One need to restart configuration.
                // OUTput to debug console.
                Debug.WriteLine("restarting configuration as the connection is down!");
                return;
            }

            // When it is configured, we need to check whether ble configuration is in progress.
            base.OnDataReceived(dataReceived);


        }

        public override void Send(String message)
        {
            if (_configurator.IsDeviceConfigured == false)
            {
                byte[] buff = Encoding.ASCII.GetBytes(message);
                SendInternal(buff);
            }
            else
            {
                // Change the power of the device to high, by changing pin.
                // Currently for development, it is set to full power mode, so no need to change it.

                int size = 64;
                byte[] details = new byte[size];
                byte[] dataToBuffer = Encoding.ASCII.GetBytes(message);

                //if (null != _deviceDataWriter)
                //{
                //    _deviceDataWriter.Dispose();
                //}

                // Try to split the message and dump there in the array

                //details[0] = 0xff;
                //details[1] = 0xff;
                int index = 0;
                int maxLengthSize = size - index - 1; ;
                int totalNumbers = dataToBuffer.Length / maxLengthSize;
                if (totalNumbers > 0)
                {
                    for (int i = 0; i < totalNumbers; i++)
                    {
                        // _loraDataWriter = new DataWriter(_loraDevice.OutputStream);
                        Array.ConstrainedCopy(dataToBuffer, i * maxLengthSize, details, index, maxLengthSize);

                        details[maxLengthSize + index] = (byte)'\n';
                        SendInternal(details);
                    }
                }

                // _loraDataWriter = new DataWriter(_loraDevice.OutputStream);

                if (dataToBuffer.Length % maxLengthSize > 0)
                {
                    details = new byte[size];
                    //details[0] = 0xff;
                    //details[1] = 0xff;

                    Array.ConstrainedCopy(dataToBuffer, totalNumbers * maxLengthSize, details, index, dataToBuffer.Length % maxLengthSize);
                    details[dataToBuffer.Length % maxLengthSize + index] = (byte)'\n';
                    SendInternal(details);
                }
            }
        }
    }
}
