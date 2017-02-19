using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial.AtCommands
{
    internal class NotificationCommand : ShowCommand
    {
        internal NotificationCommand(string command) : base(command)
        {
            CanSkipResponse = false;
        }
    }
}
