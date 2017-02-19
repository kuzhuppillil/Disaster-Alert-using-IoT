using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace DisasterAlertNode.Core.Serial
{
    internal class SerialWatcher
    {
        DeviceWatcher _deviceWatcher;
        List<string> _devices;

        internal delegate void OnSerialDiscoveryFinishedHandler();
        internal delegate void OnSerialDeviceAddedHandler(String id);
        internal delegate void OnSerialDeviceRemovedHandler(String id);

        internal event OnSerialDiscoveryFinishedHandler OnSerialDiscoveryFinished;
        internal event OnSerialDeviceAddedHandler OnSerialDeviceAdded;
        internal event OnSerialDeviceRemovedHandler OnSerialDeviceRemoved;

        internal SerialWatcher()
        {
            _devices = new List<string>();
            string _deviceSelector = SerialDevice.GetDeviceSelector();

            _deviceWatcher = DeviceInformation.CreateWatcher(_deviceSelector);
            _deviceWatcher.Added += OnNewDeviceAdded;
            _deviceWatcher.Removed += OnDeviceRemoved;
            _deviceWatcher.Updated += OnDeviceCollectionUpdated;
            _deviceWatcher.EnumerationCompleted += OnDeviceEnumerationCompleted;
        }

        internal void Start()
        {
            if (null != _deviceWatcher && (_deviceWatcher.Status != DeviceWatcherStatus.Started && _deviceWatcher.Status != DeviceWatcherStatus.EnumerationCompleted && _deviceWatcher.Status != DeviceWatcherStatus.Stopping))
            {
                _deviceWatcher.Start();
            }
        }

        List<String> Devices { get { return _devices; } }

        private void OnDeviceEnumerationCompleted(DeviceWatcher sender, object args)
        {
            OnSerialDiscoveryFinished?.Invoke();
            sender.Stop();
        }

        private void OnDeviceCollectionUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //throw new NotImplementedException();
        }

        private void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (_devices.Contains(args.Id))
            {
                _devices.Remove(args.Id);
            }
            OnSerialDeviceRemoved?.Invoke(args.Id);
        }

        private void OnNewDeviceAdded(DeviceWatcher sender, DeviceInformation args)
        {
            Debug.WriteLine("Device name is - {0} Device ID is- {1}", args.Name, args.Id);
            if (_devices.Contains(args.Id))
            {
                Debug.WriteLine("Device name is - {0} Device ID is- {1}. Device present in cache, not giving to application.", args.Name, args.Id);
                return;
            }
            OnSerialDeviceAdded?.Invoke(args.Id);
            _devices.Add(args.Id);

        }
    }
}
