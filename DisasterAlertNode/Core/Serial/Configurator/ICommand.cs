using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisasterAlertNode.Core.Serial.Configurator
{
    internal  interface ICommand
    {
        bool NeedFormatting { get; set; }
        string CommandString { get; }
        string FormatString { get; set; }
        string ParseResponse(string response);
        List<string> ParsedResponses { get; }
        int TimeOut { get; }
        bool CanSkipResponse { get; }
        bool HasReceivedCompleteResponse { get; }
        void ClearResponses();
    }
}
