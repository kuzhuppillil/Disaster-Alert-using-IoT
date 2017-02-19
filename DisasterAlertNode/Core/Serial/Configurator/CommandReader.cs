using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisasterAlertNode.Core.Serial.Configurator
{
    internal class CommandReader : ICommandReader
    {
        internal IList<ICommand> _commands = new List<ICommand>();
        ICommand _currentCommand = null;
        ICommand _previousCommand = null;
        int currentIndex = -1;
        bool _isInitialized = false;

        public void Load()
        {
            RetrieveCommands();
            _isInitialized = false;
        }

        public virtual void RetrieveCommands()
        {

        }

        public ICommand MoveNext()
        {
            currentIndex++;
            if (currentIndex >= _commands.Count)
            {
                currentIndex = _commands.Count;
                _previousCommand = _currentCommand == null ? _previousCommand : _currentCommand;
                return null;
            }

            _isInitialized = true;
            _previousCommand = _currentCommand;
            _currentCommand = _commands[currentIndex];

            return _currentCommand;

        }

        public ICommand Current
        {
            get
            {
                return _currentCommand;
            }
        }

        public ICommand Previous
        {
            get
            {
                return _previousCommand;
            }
        }

        public void Reset()
        {
            currentIndex = -1;
            _currentCommand = null;
            _previousCommand = null;
            _isInitialized = true;
        }

        public int Count
        {
            get
            {
                return _commands.Count;
            }
        }

        public bool IsInitialized { get { return _isInitialized; } }
    }
}
