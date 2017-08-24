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
        // Matches the git origin URL, providing named capture groups
        // Example match: https://user.visualstudio.com/DefaultCollection/MyFirstProject/_git/MyFirstRepo
        private static readonly Regex HostingUrlPattern =
            new Regex(
                @"(?<companyurl>
                      (?:https://)?
                      (?<accountname>([a-zA-Z0-9\-\.]*)?)    # account name (e.g. user)
                      \.visualstudio\.com/
                      (
                          [a-zA-Z0-9\-\.]+/                  # collection (optional). e.g. DefaultCollection/
                          (?!/?_git/)                        # Negative lookahead to avoid capturing 'project' group
                      )?
                  )
                  (?<project>[a-zA-Z0-9\-\.]*)               # project name. e.g. MyFirstProject
                  (?<git>/?_git//?)
                  (?<repo>[^/]+)                             # the repository's name. e.g. MyFirstRepo
                ",
                RegexOptions.IgnorePatternWhitespace);

        public VisualStudioTeamServicesProvider()
            : base(new GitPreparer())
        {
        }

        public override string RawGitUrl => string.Empty;

        public override bool Initialize(string url)
        {
            var match = HostingUrlPattern.Match(url);

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

            // In the VSTS provider, the ProjectUrl will represent
            // the repository's name.
            ProjectUrl = match.Groups["repo"].Value;

            if (!CompanyUrl.StartsWithIgnoreCase("https://"))
            {
                CompanyUrl = String.Concat("https://", CompanyUrl);
            }

            return true;
        }
    }
}