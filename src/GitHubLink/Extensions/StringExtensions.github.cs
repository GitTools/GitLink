// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.github.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitHubLink
{
    using System;
    using Catel;

    public static partial class StringExtensions
    {
        public static string GetGitHubCompanyName(this string url)
        {
            Argument.IsNotNullOrWhitespace(() => url);

            url = url.Replace(GitHubLinkEnvironment.GitHubUrl, string.Empty);
            var splittedUrl = url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedUrl.Length != 2)
            {
                return null;
            }

            return splittedUrl[0];
        }

        public static string GetGitHubProjectName(this string url)
        {
            Argument.IsNotNullOrWhitespace(() => url);

            url = url.Replace(GitHubLinkEnvironment.GitHubUrl, string.Empty);
            var splittedUrl = url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedUrl.Length != 2)
            {
                return null;
            }

            var projectName = splittedUrl[1];
            if (projectName.EndsWith(".git"))
            {
                projectName = projectName.Substring(0, projectName.Length - ".git".Length);
            }

            return projectName;
        }

        public static string GetGitHubProjectUrl(this string url)
        {
            Argument.IsNotNullOrWhitespace(() => url);

            var companyUrl = GetGitHubCompanyUrl(url);
            var project = GetGitHubProjectName(url);

            return string.Format("{0}/{1}", companyUrl, project);
        }

        public static string GetGitHubCompanyUrl(this string url)
        {
            Argument.IsNotNullOrWhitespace(() => url);

            var company = GetGitHubCompanyName(url);

            return string.Format("{0}/{1}", GitHubLinkEnvironment.GitHubUrl, company);
        }

        public static string GetGitHubRawUrl(this string url)
        {
            Argument.IsNotNullOrWhitespace(() => url);

            var company = GetGitHubCompanyName(url);
            var project = GetGitHubProjectName(url);

            return string.Format("{0}/{1}/{2}", GitHubLinkEnvironment.GitHubRawUrl, company, project);
        }
    }
}