using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial
{
    delegate void OnSerialDataReceivedEventHandler(object sender, String data);
    delegate void OnSerialDeviceConnectedEventHandler(object sender, string deviceId);
    delegate void OnSerialDeviceDisconnectedEventHandler(object sender);

    interface ISerialPort
    {
        event OnSerialDataReceivedEventHandler OnSerialDataEvent;
        event OnSerialDeviceConnectedEventHandler OnSerialDeviceConnected;
        event OnSerialDeviceDisconnectedEventHandler OnSerialDeviceDisconnected;

        void Configure(UInt32 baudRate, UInt32 writeTimeout, UInt32 readTimeout);
        void Close();
        String Id { get; }
        bool IsConfigured { get; }
        void Send(string message);
        Task Create(String deviceId);
    }
}
