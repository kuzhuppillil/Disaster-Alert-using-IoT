using DisasterAlertNode.Core.Serial.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial.AtCommands
{
    internal class StartCommand : Command
    {
        internal StartCommand(string command) : base(command)
        {
            AddResponse("START");
        }
    }
}
