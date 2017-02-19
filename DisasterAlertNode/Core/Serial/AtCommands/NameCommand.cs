using DisasterAlertNode.Core.Serial.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial.AtCommands
{
    internal class NameCommand : Command
    {
        internal NameCommand(string commandString) : base(commandString)
        {
            AddResponse("NAME");
            this.CanSkipResponse = false;
            this._timeOut = 5000;
        }
    }
}
