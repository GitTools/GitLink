// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitPreparer.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Git
{
    using System;
    using System.IO;
    using Catel;
    using Catel.Logging;
    using LibGit2Sharp;

    public class GitPreparer : IRepositoryPreparer
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public bool IsPreparationRequired(Context context)
        {
            Argument.IsNotNull(() => context);

            var gitPath = GitDirFinder.TreeWalkForGitDir(context.SolutionDirectory);
            return string.IsNullOrEmpty(gitPath);
        }

        public string Prepare(Context context)
        {
            Argument.IsNotNull(() => context);

            var gitDirectory = Path.Combine(Path.GetTempPath(), "GitLink", Guid.NewGuid().ToString());
            Directory.CreateDirectory(gitDirectory);

            if (!string.IsNullOrWhiteSpace(context.TargetUrl))
            {
                gitDirectory = GetGitInfoFromUrl(context, gitDirectory);
            }

            return GitDirFinder.TreeWalkForGitDir(gitDirectory);
        }

        private string GetGitInfoFromUrl(Context context, string gitDirectory)
        {
            gitDirectory = Path.Combine(gitDirectory, ".git");
            if (Directory.Exists(gitDirectory))
            {
                Log.Info("Deleting existing .git folder from '{0}' to force new checkout from url", gitDirectory);

                DeleteHelper.DeleteGitRepository(gitDirectory);
            }

            Log.Info("Retrieving git info from url '{0}'", context.TargetUrl);

            var cloneOptions = new CloneOptions
            {
                Checkout = false,
                IsBare = true
            };

            Repository.Clone(context.TargetUrl, gitDirectory, cloneOptions);

            if (!string.IsNullOrWhiteSpace(context.TargetBranch))
            {
                // Normalize (download branches) before using the branch
                GitHelper.NormalizeGitDirectory(gitDirectory);

                using (var repository = new Repository(gitDirectory))
                {
                    var targetBranchName = string.Format("refs/heads/{0}", context.TargetBranch);
                    if (!string.Equals(repository.Head.CanonicalName, targetBranchName))
                    {
                        Log.Info("Switching to branch '{0}'", context.TargetBranch);

                        repository.Refs.UpdateTarget("HEAD", targetBranchName);
                    }
                }
            }

            return gitDirectory;
        }
    }
}