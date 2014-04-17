// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceLinkEnvironment.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    using System;
    using System.IO;
    using Catel.Logging;

    public class Context
    {
        private static ILog Log = LogManager.GetCurrentClassLogger();

        public Context()
        {
            TempDirectory = Path.Combine(Path.GetTempPath(), "SourceLink", Guid.NewGuid().ToString());
            Directory.CreateDirectory(TempDirectory);

            ConfigurationName = "Release";
        }

        public bool IsHelp { get; set; }
        public string LogFile { get; set; }

        public string SolutionDirectory { get; set; }
        public string ConfigurationName { get; set; }
        public string TempDirectory { get; set; }

        public string TargetUrl { get; set; }
        public string TargetBranch { get; set; }

        public void ValidateContext()
        {
            if (string.IsNullOrEmpty(SolutionDirectory))
            {
                Log.ErrorAndThrowException<GitHubLinkException>("Solution directory is missing");
            }

            if (string.IsNullOrEmpty(ConfigurationName))
            {
                Log.ErrorAndThrowException<GitHubLinkException>("Configuration name is missing");
            }

            if (string.IsNullOrEmpty(TargetUrl))
            {
                Log.ErrorAndThrowException<GitHubLinkException>("Target url is missing");
            }
        }
    }
}