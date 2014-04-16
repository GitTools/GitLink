// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogListener.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink.Logging
{
    using Catel.Logging;

    public class OutputLogListener : ConsoleLogListener
    {
        public OutputLogListener()
        {
            IgnoreCatelLogging = true;
            IsDebugEnabled = true;
        }

        protected override string FormatLogEvent(ILog log, string message, LogEvent logEvent, object extraData)
        {
            return message;
        }
    }
}