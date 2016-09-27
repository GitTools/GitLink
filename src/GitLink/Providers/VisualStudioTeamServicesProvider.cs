// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualStudioTeamServicesProvider.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Providers
{
    using System;
    using System.Text.RegularExpressions;
    using Catel;
    using GitTools.Git;

    public class VisualStudioTeamServicesProvider : ProviderBase
    {
        private readonly Regex _visualStudioTeamServicesRegex = new Regex(@"(?<url>(?<companyurl>(?:https://)?(?<accountname>([a-zA-Z0-9\-\.]*)?)\.visualstudio\.com/)(?<project>[a-zA-Z0-9\-\.]*)/?_git/(?<repo>[^/]+))");

        public VisualStudioTeamServicesProvider()
            : base(new GitPreparer())
        {
        }

        public override string RawGitUrl
        {
            get { return string.Empty; }
        }

        public override bool Initialize(string url)
        {
            var match = _visualStudioTeamServicesRegex.Match(url);

            if (!match.Success)
            {
                return false;
            }

            CompanyName = match.Groups["accountname"].Value;
            CompanyUrl = match.Groups["companyurl"].Value;

            ProjectName = match.Groups["project"].Value;
            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                ProjectName = match.Groups["repo"].Value;
            }

            ProjectUrl = match.Groups["companyurl"].Value + ProjectName + "/";

            if (!CompanyUrl.StartsWithIgnoreCase("https://"))
            {
                CompanyUrl = String.Concat("https://", CompanyUrl);
            }

            if (!ProjectUrl.StartsWithIgnoreCase("https://"))
            {
                ProjectUrl = String.Concat("https://", ProjectUrl);
            }

            return true;
        }
    }
}