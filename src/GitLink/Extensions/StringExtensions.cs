// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using Catel;

    public static class StringExtensions
    {
        public static string GetCanonicalBranchName(this string branchName)
        {
            if (branchName.IsPullRequest())
            {
                branchName = branchName.Replace("pull-requests", "pull");
                branchName = branchName.Replace("pr", "pull");

                return string.Format("refs/{0}/head", branchName);
            }

            return string.Format("refs/heads/{0}", branchName);
        }

        public static bool IsPullRequest(this string branchName)
        {
            return branchName.Contains("pull/") || branchName.Contains("pull-requests/") || branchName.Contains("pr/");
        }

        public static string OptimizeUrl(this string url)
        {
            Argument.IsNotNullOrWhitespace(() => url);

            url = url.EndsWith(".git") ? url.Substring(0, url.Length - 4) : url;
            return url;
        }
    }
}