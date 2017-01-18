// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputLogListener.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Logging
{
    using System;
    using Catel.Logging;

    internal class OutputLogListener : ConsoleLogListener
    {
        internal OutputLogListener()
        {
            IgnoreCatelLogging = true;
            IsDebugEnabled = true;
        }

        protected override string FormatLogEvent(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            return message;
        }
    }
}