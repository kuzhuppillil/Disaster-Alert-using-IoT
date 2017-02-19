using DisasterAlertNode.Core.Serial.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial.AtCommands
{
    internal class AtCommand : Command
    {
        internal AtCommand(String commandString) : base(commandString)
        {
            AddResponse("OK");
            AddResponse("LOST");
        }
    }
}
