// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputLogListener.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitHubLink.Logging
{
    using System;
    using Catel.Logging;

    public class OutputLogListener : ConsoleLogListener
    {
        public OutputLogListener()
        {
            IgnoreCatelLogging = true;
            IsDebugEnabled = true;
        }

        /// <summary>
        /// Formats the log event to a message which can be written to a log persistence storage.
        /// </summary>
        /// <param name="log">The log.</param><param name="message">The message.</param><param name="logEvent">The log event.</param><param name="extraData">The extra data.</param><param name="time">The time.</param>
        /// <returns>
        /// The formatted log event.
        /// </returns>
        protected override string FormatLogEvent(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            return message;
        }
    }
}