using DisasterAlertNode.Core.Serial.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial.AtCommands
{
   internal class ShowCommand : Command
    {
        internal ShowCommand(string commandStr) : base(commandStr)
        {
            AddResponse("Set:");
        }
    }
}
