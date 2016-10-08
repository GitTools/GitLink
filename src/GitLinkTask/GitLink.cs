// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitLink.cs" company="Andrew Arnott">
//   Copyright (c) 2016 Andrew Arnott. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLinkTask
{
    using System;
    using Catel.Logging;
    using global::GitLink;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class GitLink : Task
    {
        [Required]
        public ITaskItem PdbFile { get; set; }

        public bool DownloadWithPowershell { get; set; }

        public bool SkipVerify { get; set; }

        public string GitRemoteUrl { get; set; }

        public override bool Execute()
        {
            LogManager.AddListener(new MSBuildListener(this.Log));

            var options = new LinkOptions
            {
                DownloadWithPowerShell = this.DownloadWithPowershell,
                SkipVerify = this.SkipVerify,
                GitRemoteUrl = this.GitRemoteUrl != null ? new Uri(this.GitRemoteUrl, UriKind.Absolute) : null,
            };
            bool success = Linker.Link(this.PdbFile.GetMetadata("FullPath"), options);

            return success && !this.Log.HasLoggedErrors;
        }

        private class MSBuildListener : LogListenerBase
        {
            private readonly TaskLoggingHelper log;

            internal MSBuildListener(TaskLoggingHelper log)
            {
                this.log = log;
            }

            protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
            {
                switch (logEvent)
                {
                    case LogEvent.Error:
                        this.log.LogError(message);
                        break;
                    case LogEvent.Warning:
                        this.log.LogWarning(message);
                        break;
                    case LogEvent.Info:
                        this.log.LogMessage(MessageImportance.Normal, message);
                        break;
                    case LogEvent.Debug:
                        this.log.LogMessage(MessageImportance.Low, message);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
