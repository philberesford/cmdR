﻿using cmdR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Abstract
{
    public interface IRouteCommands
    {
        void RegisterRoute(string commandName, IDictionary<string, ParameterType> parameters, Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> action, string description = null, bool overwriteRoutes = false);
        void RegisterRoute(Route route, bool overwriteRoutes = false);

        List<IRoute> GetRoutes();

        IRoute FindRoute(string commandName, IDictionary<string, string> parameters);
    }
}
