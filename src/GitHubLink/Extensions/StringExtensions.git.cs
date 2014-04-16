// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    using System.Linq;
    using Catel;
    using LibGit2Sharp;

    public static partial class StringExtensions
    {
        public static string GetLatestCommitShaOfCurrentBranch(this string repositoryDirectory)
        {
            Argument.IsNotNull(() => repositoryDirectory);

            using (var repository = new Repository(repositoryDirectory))
            {
                var lastCommit = repository.Commits.First();
                return lastCommit.Sha;
            }
        }
    }
}