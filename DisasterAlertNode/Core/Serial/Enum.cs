using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial
{
    public enum SerialDeviceType
    {
        LoRa,
        BTLe,
        Invalid
    };

    public enum BLEState
    {
        Invalid,
        Ready,
        Config,
        Discovery,
        DiscoveryFinished,
        Pairing,
        Connected,
        Disconnected,
        Error,
        Progress,
        Stopped
    }
}
