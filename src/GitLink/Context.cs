// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using Catel;
    using Catel.IO;
    using Catel.Logging;
    using GitTools;
    using Providers;

    public class Context : RepositoryContext
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IProviderManager _providerManager;
        private IProvider _provider;

        public Context(IProviderManager providerManager)
        {
            Argument.IsNotNull(() => providerManager);

            _providerManager = providerManager;

            Authentication = new Authentication();
            ConfigurationName = "Release";
            PlatformName = "AnyCPU";
            IgnoredProjects = new List<string>();
        }

        public bool IsHelp { get; set; }

        public bool IsDebug { get; set; }

        public string LogFile { get; set; }

        //[Obsolete("Use 'Directory' instead")]
        public string SolutionDirectory
        {
            get { return Directory; }
            set { Directory = value; }
        }

        public string ConfigurationName { get; set; }

        public string PlatformName { get; set; }

        public Authentication Authentication { get; private set; }

        public IProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = _providerManager.GetProvider(TargetUrl);
                }

                return _provider;
            }
            set
            {
                _provider = value;
            }
        }

        //[Obsolete("Use 'Url' instead")]
        public string TargetUrl
        {
            get { return Url; }
            set { Url = value; }
        }

        //[Obsolete("Use 'Branch' instead")]
        public string TargetBranch
        {
            get { return Branch; }
            set { Branch = value; }
        }

        public string ShaHash { get; set; }

        public string SolutionFile { get; set; }

        public List<string> IgnoredProjects { get; private set; }

        public string PdbFilesDirectory { get; set; }

        public void ValidateContext()
        {
            if (!string.IsNullOrWhiteSpace(SolutionDirectory))
            {
                SolutionDirectory = Path.GetFullPath(SolutionDirectory, Environment.CurrentDirectory);
            }

            if (string.IsNullOrEmpty(SolutionDirectory))
            {
                Log.ErrorAndThrowException<GitLinkException>("Solution directory is missing");
            }

            if (string.IsNullOrEmpty(ConfigurationName))
            {
                Log.ErrorAndThrowException<GitLinkException>("Configuration name is missing");
            }

            if (string.IsNullOrEmpty(PlatformName))
            {
                Log.ErrorAndThrowException<GitLinkException>("Platform name is missing");
            }

            if (string.IsNullOrEmpty(TargetUrl))
            {
                Log.ErrorAndThrowException<GitLinkException>("Target url is missing");
            }

            if (Provider == null)
            {
                Log.ErrorAndThrowException<GitLinkException>("Cannot determine git provider");
            }
        }
    }
}