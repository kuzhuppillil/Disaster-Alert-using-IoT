using DisasterAlertNode.Core.Serial.AtCommands;
using Windows.Foundation;
using Windows.System.Threading;

namespace DisasterAlertNode.Core.Serial.Configurator
{
    public delegate void OnConfigurationStartedHandler();
    public delegate void OnDeviceConfiguredHandler(string deviceId);
    public delegate void OnDeviceConfigurationFailedHandler();

    internal sealed class DeviceConfigurator
    {
        public event OnConfigurationStartedHandler OnConfigurationStarted;
        public event OnDeviceConfiguredHandler OnDeviceConfigured;
        public event OnDeviceConfigurationFailedHandler OnDeviceConfigurationFailed;
        ICommandReader _bleCommandReader = new BleCommandReader();

        bool _bleConfigured = false;
        object _syncObject = new object();
        int _retryCount = 2;
        int _retries = 0;

        ThreadPoolTimer _timer = null;
        bool pendingOperation = false;
        int connectAddrCount = 0;

        public void ConfigureDevice()
        {
            _bleCommandReader.Load();
        }

        private void Configure()
        {
            if (_bleCommandReader.IsInitialized == false)
            {
                _bleCommandReader.MoveNext();
            }

            if (_bleCommandReader.Current != null)
            {
                if (_bleCommandReader.Current.NeedFormatting)
                {
                    if (_bleCommandReader.Current is ConnectCommand)
                    {
                        if (_bleCommandReader.Previous != null && _bleCommandReader.Previous is DiscoveryCommand)
                        {
                            connectAddrCount++;

                            if (connectAddrCount <= (_bleCommandReader.Previous as DiscoveryCommand).Addresses.Count)
                            {
                                pendingOperation = connectAddrCount != (_bleCommandReader.Previous as DiscoveryCommand).Addresses.Count;
                                _bleCommandReader.Current.FormatString = _bleCommandReader.Previous.ParsedResponses.Count > 0 ? (_bleCommandReader.Previous as DiscoveryCommand).Addresses[connectAddrCount - 1] : "";
                            }else
                            {
                                _bleCommandReader.Reset();
                                connectAddrCount = 0;
                                pendingOperation = false;
                                _retries = 0;
                                _bleCommandReader.MoveNext();
                                Configure();
                                return;
                            }
                        }else
                        {
                            _bleCommandReader.Reset();
                            pendingOperation = false;
                            connectAddrCount = 0;
                            _retries = 0;
                            _bleCommandReader.MoveNext();
                            Configure();
                            return;
                        }
                    }

                }

                _retries++;
                _bleCommandReader.Current.ClearResponses(); // We are interested in fresh responses.
                SerialPortFactory.GetInstance().Create(SerialDeviceType.BTLe).Send(_bleCommandReader.Current.CommandString);    // Send the command to system.
                _timer = ThreadPoolTimer.CreateTimer(new TimerElapsedHandler(OnTimerElapsed), System.TimeSpan.FromMilliseconds(_bleCommandReader.Current.TimeOut));
            }
        }

        internal void StartConfiguration()
        {
            OnConfigurationStarted?.Invoke();
            Configure();
        }

        internal void OnTimerElapsed(ThreadPoolTimer timer)
        {
            if (_retries > _retryCount)
            {
                // check if the current one needs no further processing.
                if (_bleCommandReader.Current != null && _bleCommandReader.Current.CanSkipResponse)
                {
                    _bleCommandReader.MoveNext();
                    _retries = 0;
                }
            }
            Configure();
        }

        internal bool IsDeviceConfigured { get { return _bleConfigured; } }

        internal void HandleConfigurationResponse(string message)
        {
            if (IsDeviceConfigured == false)
            {
                if (_bleCommandReader.Current != null)
                {
                    _bleCommandReader.Current.ParseResponse(message);
                    // Check if command needs to be waited.

                    // We should check the parsed message, current command and its response to see whether it is done with device connectivity.
                    _retries = 0;
                    if (_timer != null)
                    {
                        _timer.Cancel();
                        _timer = null;
                    }

                    if (_bleCommandReader.Current.HasReceivedCompleteResponse)
                    {
                        if(_bleCommandReader.Current is ConnectCommand)
                        {
                            // Connection is successfull.
                            _bleConfigured = true;

                            // We just need to say which device is that we have connected to.
                            // Just read the address of the device which we have connected to.

                            OnDeviceConfigured?.Invoke(_bleCommandReader.Current.FormatString);
                        }
                        // Just get the next message.
                        if (!pendingOperation && _bleCommandReader.MoveNext() == null)
                        {
                            _bleCommandReader.Reset();
                            _bleCommandReader.MoveNext();
                        }

                        Configure();
                    }
                }
            }
        }

        internal void RestartConfiguration()
        {
            _bleConfigured = false;
            _retries = 0;
            StartConfiguration();
        }
    }
}
