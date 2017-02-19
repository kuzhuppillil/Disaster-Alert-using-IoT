using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisasterAlertNode.Core.Serial.Configurator
{
   internal interface ICommandReader
    {
        void Load();
        ICommand MoveNext();
        ICommand Current { get; }
        ICommand Previous { get; }
        void Reset();
        int Count { get; }
        void RetrieveCommands();
        bool IsInitialized { get; }
    }
}
