using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;
using System.Diagnostics;
using Windows.Devices.Gpio;
using DisasterAlertNode.Core.Serial;
using Windows.System.Threading;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Storage;


// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace DisasterAlertNode
{
    enum PulseStatus
    {
        CriticallyLow,
        VeryLow,
        Low,
        Normal,
        High,
        VeryHigh,
        CriticallyHigh,
        Invalid
    };

    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral = null;

        ISerialPort _loraSerialDevice = null;
        ISerialPort _bleSerialDevice = null;

        SerialWatcher _watcher = null;

        GpioPin _enPin = null;
        GpioPin _setPin = null;
        GpioController _controller = null;
        const int EN_PIN_NUMBER = 12;   // Gpio12
        const int SET_PIN_NUMBER = 6;   // Gipo6
        const int AUX_PIN_NUMBER = 5;   // Gpio5

        List<double> _pulseData = new List<double>();
        List<double> _criticalRate = new List<double>();

        const int MAX_PULSE_COUNT = 30;
        const int MAX_CRITICAL_RATE = 2;
        const string LOCATION = "12°56'02.1\"N 77°41'31.4\"E";

        ThreadPoolTimer _timer = null;

        public void Run(IBackgroundTaskInstance taskInstance)
        {

            _deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCancelled;

            _watcher = new SerialWatcher();
            _watcher.OnSerialDiscoveryFinished += OnSerialDeviceDiscoveryFinished;
            _watcher.OnSerialDeviceAdded += OnSerialDeviceDiscovered;
            _watcher.OnSerialDeviceRemoved += OnSerialDeviceRemoved;

            _timer = ThreadPoolTimer.CreatePeriodicTimer(OnTimerExpired, TimeSpan.FromMilliseconds(30000));
            _controller = GpioController.GetDefault();

            ConfigureLoRa();

        }

        private void OnMediaPlayerServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {

        }

        private PulseStatus ConvertToPulseStatus(double pulse, bool critical)
        {
            PulseStatus status = PulseStatus.Invalid;
            if (pulse >= 40 && pulse <= 100)
            {
                status = PulseStatus.Normal;
            }
            else if (pulse > 120 && pulse <= 140)
            {
                status = PulseStatus.High;
            }
            else if (pulse > 140)
            {
                status = critical ? PulseStatus.CriticallyHigh : PulseStatus.VeryHigh;
            }
            else if (pulse < 40 && pulse > 20)
            {
                status = PulseStatus.Low;
            }
            else
            {
                status = critical ? PulseStatus.CriticallyLow : PulseStatus.VeryLow;
            }

            return status;
        }

        private void OnTimerExpired(ThreadPoolTimer timer)
        {
            if (null != _watcher)
            {
                if (_loraSerialDevice == null || _bleSerialDevice == null)
                    _watcher.Start();
            }
        }

        private void OnSerialDeviceRemoved(string id)
        {
            if (id.Contains("UART0"))
            {
                if (_loraSerialDevice != null)
                {
                    // Dispose LoRa serial device interface. 
                    // TODO Add dispose code here.
                }
            }
        }

        private async void OnSerialDeviceDiscovered(string id)
        {
            if (id.Contains("UART0"))
            {
                // LoRa device.
                _loraSerialDevice = SerialPortFactory.GetInstance().Create(SerialDeviceType.LoRa);
                await _loraSerialDevice.Create(id);
                if (false == _loraSerialDevice.IsConfigured)
                {
                    _loraSerialDevice.Configure(9600, 1000, 500);
                    if (_loraSerialDevice.IsConfigured)
                    {
                        _loraSerialDevice.OnSerialDataEvent += OnLoraDataReceived;
                    }
                }


            }
            else
            {
                _bleSerialDevice = SerialPortFactory.GetInstance().Create(SerialDeviceType.BTLe);
                await _bleSerialDevice.Create(id);
                if (false == _bleSerialDevice.IsConfigured)
                {
                    _bleSerialDevice.Configure(9600, 3000, 500);
                    if (_bleSerialDevice.IsConfigured)
                    {
                        _bleSerialDevice.OnSerialDataEvent += OnBleDataReceived;
                        _bleSerialDevice.OnSerialDeviceConnected += OnBLEDeviceConnected;
                        _bleSerialDevice.OnSerialDeviceDisconnected += OnBLEDeviceDisconnected;

                    }
                }
            }
        }

        private void OnBLEDeviceDisconnected(object sender)
        {
            // Device is disconnected, Get the device details.
            if (sender != null && sender is BTLESerial)
            {
                _loraSerialDevice.Send(String.Format("Remote device disconnected from node. Remote device id - {0}", (sender as BTLESerial).ConnectedDevice));
            }

            _pulseData.Clear();
            _criticalRate.Clear();

        }
        // At a time, only one device connects, as for test purpose.
        private void OnBLEDeviceConnected(object sender, string id)
        {
            _loraSerialDevice.Send(String.Format("Remote device connected to node. Remote device id - {0}", (sender as BTLESerial).ConnectedDevice));
        }

        private void OnSerialDeviceDiscoveryFinished()
        {

        }

        private void OnBleDataReceived(object sender, string data)
        {

            Debug.WriteLine(String.Format("Data received from BLE - {0}\n", data));
            // If data from BLE is received, we have to send it to LoRa device.
            if (null != _loraSerialDevice)
            {
                if (_loraSerialDevice.IsConfigured == false)
                {
                    _loraSerialDevice.Configure(9600, 3000, 500);
                }

                // Try to parse the value received.

                double currentBPM = 0;
                if (double.TryParse(data, out currentBPM))
                {
                    _pulseData.Add(currentBPM);
                    if (_pulseData.Count == MAX_PULSE_COUNT)
                    {
                        double average = _pulseData.Average();
                        _criticalRate.Add(average);

                        if (_criticalRate.Count == MAX_CRITICAL_RATE)
                        {
                            double averageCriticalRate = _criticalRate.Average();
                            _loraSerialDevice.Send(String.Format("\r\nLoc - {0}\r\nCur.BPM - {1}\r\nCur.Pulse Status - {2}\r\nAvg.BPM - {3}\r\n", LOCATION, (int)average, ConvertToPulseStatus(average, true), ConvertToPulseStatus(averageCriticalRate, true)));
                            _criticalRate.Clear();
                        }
                        else
                        {
                            _loraSerialDevice.Send(String.Format("\r\nLoc- {0}\r\nCur.BPM - {1}\r\nCur.Pulse Status - {2}\r\n", LOCATION, (int)average, ConvertToPulseStatus(average, false)));
                        }

                        _pulseData.Clear();
                        _pulseData.Add(average);

                    }
                }
            }
        }

        private async void OnLoraDataReceived(object sender, string data)
        {

            try
            {
                String fileName = String.Format("{0}\\{1}", System.IO.Directory.GetCurrentDirectory(), "alert.wav");
                var istorage = await StorageFile.GetFileFromPathAsync(fileName);
                BackgroundMediaPlayer.Current.SetFileSource(istorage);
                BackgroundMediaPlayer.Current.Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception occured- Exception details - {0}", ex);
            }



            Debug.WriteLine(String.Format("Data received from LoRa - {0}\n", data));

        }

        private void OnTaskCancelled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Cancel all serial operations, close port and exit read operations
            _timer.Cancel();
            BackgroundMediaPlayer.Shutdown();
            _deferral.Complete();   // Notify that the operation is completed.
        }

        private void ConfigureLoRa()
        {
            if (OpenGpioPin(EN_PIN_NUMBER, out _enPin))
            {
                _enPin.SetDriveMode(GpioPinDriveMode.Output);
                _enPin.Write(GpioPinValue.Low);
            }

            if (OpenGpioPin(SET_PIN_NUMBER, out _setPin))
            {
                _setPin.SetDriveMode(GpioPinDriveMode.Input);
                _setPin.Write(GpioPinValue.Low);
            }
        }

        private bool OpenGpioPin(int pinNumber, out GpioPin pin)
        {
            GpioOpenStatus openStat = GpioOpenStatus.PinUnavailable;
            if (_controller.TryOpenPin(pinNumber, GpioSharingMode.Exclusive, out pin, out openStat))
            {
                switch (openStat)
                {
                    case GpioOpenStatus.PinOpened:
                        Debug.WriteLine("Pin Successfully opened - Pin Number - {0}!!", pinNumber);
                        break;
                    case GpioOpenStatus.PinUnavailable:
                        Debug.WriteLine("Pin Unavailable. Pin Number - {0}", pinNumber);
                        break;
                    case GpioOpenStatus.SharingViolation:
                        Debug.WriteLine("Sharing violation to open pin on pin number - {0}", pinNumber);
                        break;
                }
            }

            return openStat == GpioOpenStatus.PinOpened;
        }
    }
}
