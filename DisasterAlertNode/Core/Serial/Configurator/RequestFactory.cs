using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisasterAlertNode.Core.Serial.Configurator
{
    internal class RequestFactory
    {
        private static RequestFactory _instance;
        private static object _lockObject = new object();

        private RequestFactory()
        {
        }

        public static RequestFactory Instance()
        {
            lock (_lockObject)
            {
                if (null == _instance)
                {
                    _instance = new RequestFactory();
                }
            }

            return _instance;
        }

    }
}
