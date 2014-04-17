// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitDirFinder.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink.Git
{
    using System.IO;
    using LibGit2Sharp;

    public class GitDirFinder
    {
        #region Methods

        public static string TreeWalkForGitDir(string currentDirectory)
        {
            var gitDirectory = Repository.Discover(currentDirectory);

            if (gitDirectory != null)
            {
                return gitDirectory.TrimEnd(new[] {Path.DirectorySeparatorChar});
            }

            return null;
        }

        #endregion
    }
}