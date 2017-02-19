using DisasterAlertNode.Core.Serial.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial.AtCommands
{
   internal class FilterCommand : Command
    {
        internal FilterCommand(string commandString) : base(commandString)
        {
            AddResponse("SET1");
        }
    }
}
