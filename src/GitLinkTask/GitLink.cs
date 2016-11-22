// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkProject.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLinkTask
{
    using System;
    using System.IO;
    using System.Linq;
    using global::GitLink;
    using global::GitLink.Pdb;
    using global::GitLink.Providers;
    using Catel.Logging;
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
            LogManager.GetCurrentClassLogger().LogMessage += this.LinkProject_LogMessage;

            var options = new LinkOptions
            {
                DownloadWithPowerShell = this.DownloadWithPowershell,
                SkipVerify = this.SkipVerify,
                GitRemoteUrl = this.GitRemoteUrl != null ? new Uri(this.GitRemoteUrl, UriKind.Absolute) : null,
            };
            Linker.Link(this.PdbFile.GetMetadata("FullPath"), options);

            return !this.Log.HasLoggedErrors;
        }

        private void LinkProject_LogMessage(object sender, LogMessageEventArgs e)
        {
            switch (e.LogEvent)
            {
                case LogEvent.Error:
                    this.Log.LogError(e.Message);
                    break;
                case LogEvent.Warning:
                    this.Log.LogWarning(e.Message);
                    break;
                case LogEvent.Info:
                    this.Log.LogMessage(MessageImportance.Normal, e.Message);
                    break;
                case LogEvent.Debug:
                    this.Log.LogMessage(MessageImportance.Low, e.Message);
                    break;
                default:
                    break;
            }
        }
    }
}