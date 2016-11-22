// Copyright (c) Andrew Arnott. All rights reserved.

namespace GitLinkTask
{
    using System;
    using Catel.Logging;
    using global::GitLink;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class LinkPdbToGitRemote : Task
    {
        [Required]
        public ITaskItem PdbFile { get; set; }

        public string Method
        {
            get { return this.MethodEnum.ToString(); }
            set { this.MethodEnum = string.IsNullOrEmpty(value) ? LinkMethod.Http : (LinkMethod)Enum.Parse(typeof(LinkMethod), value); }
        }

        public bool SkipVerify { get; set; }

        public string GitRemoteUrl { get; set; }

        public string GitCommitId { get; set; }

        public string GitWorkingDirectory { get; set; }

        private LinkMethod MethodEnum { get; set; }

        public override bool Execute()
        {
            LogManager.AddListener(new MSBuildListener(this.Log));

            var options = new LinkOptions
            {
                Method = this.MethodEnum,
                SkipVerify = this.SkipVerify,
                GitRemoteUrl = this.GitRemoteUrl != null ? new Uri(this.GitRemoteUrl, UriKind.Absolute) : null,
                CommitId = this.GitCommitId,
                GitWorkingDirectory = this.GitWorkingDirectory,
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
