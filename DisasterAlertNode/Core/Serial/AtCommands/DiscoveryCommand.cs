using DisasterAlertNode.Core.Serial.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial.AtCommands
{
    internal class DiscoveryCommand : Command
    {
        internal DiscoveryCommand(string commandStr) : base(commandStr)
        {
            _timeOut = 30000;
            AddResponse("DISCS");
            AddResponse("DIS0");
            AddResponse("DIS1");
            AddResponse("DIS2");
            AddResponse("DIS3");
            AddResponse("DIS4");
            AddResponse("DIS5");
            AddResponse("DISCE");
            CanSkipResponse = false;
        }

        public override bool HasReceivedCompleteResponse
        {
            get
            {
                if(this.ParsedResponses.Contains("DISCS") && this.ParsedResponses.Contains("DISCE"))
                {
                    return true;
                }
                return false;

            }
        }

        internal List<string> Addresses
        {
            get
            {
                List<string> address = new List<string>();
                for(int i = 0; i < 6; i++)
                {
                    if (this.ParsedResponses.Contains(String.Format("DIS{0}", i)))
                    {
                        var strAddr = _actualResponses.Where(x => x.StartsWith(String.Format("DIS{0}:", i))).FirstOrDefault();
                        if(String.IsNullOrEmpty(strAddr) == false)
                        {
                            address.Add(strAddr.Substring(String.Format("DIS{0}:", i).Length));
                        }
                    }
                    break;
                }
                return address;
            }
        }
    }
}
