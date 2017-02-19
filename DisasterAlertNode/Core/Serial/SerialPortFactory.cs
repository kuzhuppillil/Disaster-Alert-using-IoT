using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial
{
    class SerialPortFactory
    {
        private static SerialPortFactory _serialFactory;
        private static object _lockObject = new object(); // For synchronization

        Dictionary<SerialDeviceType, ISerialPort> _serialDevices;

        private SerialPortFactory()
        {
            _serialDevices = new Dictionary<SerialDeviceType, ISerialPort>();
            // Default constructor, used for making singleton instance.
        }

        public static SerialPortFactory GetInstance()
        {
            lock (_lockObject)
            {
                if(null == _serialFactory)
                {
                    _serialFactory = new SerialPortFactory();
                }
            }

            return _serialFactory;
        }

        public ISerialPort Create(SerialDeviceType device)
        {
           
            if(_serialDevices.ContainsKey(device) && null != _serialDevices[device])
            {
                return _serialDevices[device];
            }

            ISerialPort _serialPort = null;
            switch (device)
            {
                case SerialDeviceType.LoRa:
                    _serialPort = new LoRaSerial();
                    break;
                case SerialDeviceType.BTLe:
                    _serialPort = new BTLESerial();
                    break;
            }
            
            if(null != _serialPort)
            {
                _serialDevices[device] = _serialPort;
            }

            return _serialPort;
        }
    }
}
