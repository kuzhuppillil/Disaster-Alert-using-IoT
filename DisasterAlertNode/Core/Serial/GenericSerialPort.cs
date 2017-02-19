using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace DisasterAlertNode.Core.Serial
{
    abstract class GenericSerialPort : ISerialPort
    {
        SerialDevice _device;
        DataWriter _deviceDataWriter = null;

        /* Read data in from the serial port */
        const uint maxReadLength = 256;
        bool _isConfigured;
        private String _id;
        private object _lockObject = new object();

        public async Task Create(String deviceId)
        {
            _id = deviceId;
            // Retrieve the serial device from ID.
            await CreateInternal();
        }

        private async Task CreateInternal()
        {
            _device = await SerialDevice.FromIdAsync(_id);
        }
        public event OnSerialDataReceivedEventHandler OnSerialDataEvent;

        public event OnSerialDeviceConnectedEventHandler OnSerialDeviceConnected;
        public event OnSerialDeviceDisconnectedEventHandler OnSerialDeviceDisconnected;

        public virtual void Configure(uint baudRate, uint writeTimeout, uint readTimeout)
        {
            lock (_lockObject)
            {
                if (null == _device)
                {
                    return;
                    //throw new Exception("Serial port device was not initialized, application will fail to run.");
                }

                _device.WriteTimeout = TimeSpan.FromMilliseconds(writeTimeout);
                _device.ReadTimeout = TimeSpan.FromMilliseconds(readTimeout);
                _device.BaudRate = baudRate;
                _device.Parity = SerialParity.None;
                _device.StopBits = SerialStopBitCount.One;
                _device.DataBits = 8;

                ReadDataAsync();

                _deviceDataWriter = new DataWriter(_device.OutputStream);

                _isConfigured = true;
            }
        }

        public void Close()
        {

        }

        internal virtual void OnDataReceived(String dataReceived)
        {
            OnSerialDataEvent?.Invoke(this, dataReceived);
        }

        private async Task ReadDataAsync()
        {
            if (null != _device)
            {
                DataReader dataReader = null;
                if (null != _device)
                {
                    dataReader = new DataReader(_device.InputStream);
                }

                // Get the input stream, and wait for data till it is available, or needed to be available
                while (null != dataReader)
                {
                    uint bytesToRead = await dataReader.LoadAsync(maxReadLength);
                    if (null != dataReader)
                    {
                        try
                        {
                            string rxBuffer = dataReader.ReadString(bytesToRead);
                            OnDataReceived(rxBuffer);
                        }
                        catch (Exception ex)
                        {

                            Debug.WriteLine(ex.ToString());
                        }

                    }

                }
            } // Async read.
        } // ReadDataAsync

        public bool IsConfigured { get { return _isConfigured; } }

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public virtual void Send(String message)
        {
            // Change the power of the device to high, by changing pin.
            // Currently for development, it is set to full power mode, so no need to change it.

            int size = 128;
            byte[] details = new byte[size];
            byte[] dataToBuffer = Encoding.ASCII.GetBytes(message);

            int index = 0;
            int maxLengthSize = size - index; ;
            int totalNumbers = dataToBuffer.Length / maxLengthSize;
            if (totalNumbers > 0)
            {
                for (int i = 0; i < totalNumbers; i++)
                {
                    // _loraDataWriter = new DataWriter(_loraDevice.OutputStream);
                    Array.ConstrainedCopy(dataToBuffer, i * maxLengthSize, details, index, maxLengthSize);
                    SendInternal(details);
                    details = new byte[size];
                }
            }

            if (dataToBuffer.Length % maxLengthSize > 0)
            {
                details = new byte[size];

                Array.ConstrainedCopy(dataToBuffer, totalNumbers * maxLengthSize, details, index, dataToBuffer.Length % maxLengthSize);
                SendInternal(details);
            }

        }

        internal async void SendInternal(byte[] buffer)
        {
            _deviceDataWriter.WriteBytes(buffer);
            await _device.OutputStream.WriteAsync(_deviceDataWriter.DetachBuffer());
        }

        internal void OnDeviceConfigured(string id)
        {
            OnSerialDeviceConnected?.Invoke(this,id);
        }

        internal void OnDeviceDisconnected()
        {
            OnSerialDeviceDisconnected?.Invoke(this);
        }
    }
}
