using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterAlertNode.Core.Serial.Configurator
{
    internal class Command : ICommand
    {
        List<string> _responses;
        List<string> _receivedResponses = new List<string>();
        internal List<string> _actualResponses = new List<string>();

        string _commandString;
        bool _canSkipResponse = true;
        string _formatString = String.Empty;
        internal int _timeOut = 3000;
        internal int _maximumResponses = 1;

        public Command(string commandString)
        {
            _commandString = commandString;
            _responses = new List<string>();
        }

        public bool CanSkipResponse { get { return _canSkipResponse; } set { _canSkipResponse = value; } }

        public bool NeedFormatting { get; set; }

        public void AddResponse(string response)
        {
            _responses.Add(response);
        }

        public virtual string ParseResponse(string response)
        {
            if (response.Contains("OK+"))
            {
                string[] resp = response.Split(new string[] { "OK+" },  StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in resp)
                {
                    if (String.IsNullOrEmpty(str))
                    {
                        continue;
                    }

                    foreach (String st in _responses)
                    {
                        if (String.Equals(str, st) || str.StartsWith(st))
                        {
                            _receivedResponses.Add(st);
                            _actualResponses.Add(str);
                            break;
                        }
                    }
                }
            }else
            {
                foreach(String resp in _responses)
                {
                    if (String.Equals(resp, response))
                    {
                        _receivedResponses.Add(resp);
                        _actualResponses.Add(resp);
                    }
                }
            }
            return _receivedResponses.FirstOrDefault();
        }

        public string CommandString
        {
            get { return NeedFormatting ? String.Format(_commandString, FormatString) : _commandString; }
        }

        public string FormatString
        {
            get { return _formatString; }
            set { _formatString = value; }
        }

        public List<string> ParsedResponses { get { return _receivedResponses; } }

        public int TimeOut { get { return _timeOut; } }

        public virtual bool HasReceivedCompleteResponse {
            get
            {
                return _maximumResponses == this._receivedResponses.Count; // Just a default implementation, where in total count == 1
            }
        }

        public void ClearResponses()
        {
            this._receivedResponses.Clear();
            _actualResponses.Clear();
        }
    }
}
