using DisasterAlertNode.Core.Serial.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial.AtCommands
{
    internal class ConnectCommand : Command
    {
        internal ConnectCommand(string command) : base(command)
        {
            _timeOut = 30000;
            AddResponse("CONNA");
            AddResponse("CONNE");
            AddResponse("CONN");
            AddResponse("CONNF");
            NeedFormatting = true;
            CanSkipResponse = false;
            this._maximumResponses = 2;
        }

        public override bool HasReceivedCompleteResponse
        {
            get
            {
                return this.ParsedResponses.Contains("CONN") || this.ParsedResponses.Contains("CONNA")  || this.ParsedResponses.Count == _maximumResponses;
            }
        }
    }
}
