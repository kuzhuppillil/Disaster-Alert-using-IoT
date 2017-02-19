using DisasterAlertNode.Core.Serial.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial.AtCommands
{
    internal class AddressCommand : Command
    {
        internal AddressCommand(string commandString) : base(commandString)
        {
            AddResponse("ADDR:");
        }
    }
}
