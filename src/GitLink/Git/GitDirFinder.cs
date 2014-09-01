// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitDirFinder.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Git
{
    using System.IO;
    using LibGit2Sharp;

    public class GitDirFinder
    {
        public static string TreeWalkForGitDir(string currentDirectory)
        {
            string gitDirectory = Repository.Discover(currentDirectory);

            if (gitDirectory != null)
            {
                return gitDirectory.TrimEnd(new[] { Path.DirectorySeparatorChar });
            }

            return null;
        }
    }
}