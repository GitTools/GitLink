// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitPreparer.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitHubLink.Git
{
    using System.IO;
    using Catel;
    using Catel.Logging;
    using LibGit2Sharp;

    public class GitPreparer
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Context _context;

        public GitPreparer(Context context)
        {
            Argument.IsNotNull(() => context);

            _context = context;
        }

        public string Prepare()
        {
            var gitPath = _context.TempDirectory;

            if (!string.IsNullOrWhiteSpace(_context.TargetUrl))
            {
                gitPath = GetGitInfoFromUrl();
            }

            return GitDirFinder.TreeWalkForGitDir(gitPath);
        }

        private string GetGitInfoFromUrl()
        {
            var gitDirectory = Path.Combine(_context.TempDirectory, ".git");
            if (Directory.Exists(gitDirectory))
            {
                Log.Info("Deleting existing .git folder from '{0}' to force new checkout from url", gitDirectory);

                DeleteHelper.DeleteGitRepository(gitDirectory);
            }

            Log.Info("Retrieving git info from url '{0}'", _context.TargetUrl);

            var cloneOptions = new CloneOptions
            {
                Checkout = false,
                IsBare = true
            };

            Repository.Clone(_context.TargetUrl, gitDirectory, cloneOptions);

            if (!string.IsNullOrWhiteSpace(_context.TargetBranch))
            {
                // Normalize (download branches) before using the branch
                GitHelper.NormalizeGitDirectory(gitDirectory);

                using (var repository = new Repository(gitDirectory))
                {
                    var targetBranchName = string.Format("refs/heads/{0}", _context.TargetBranch);
                    if (!string.Equals(repository.Head.CanonicalName, targetBranchName))
                    {
                        Log.Info("Switching to branch '{0}'", _context.TargetBranch);

                        repository.Refs.UpdateTarget("HEAD", targetBranchName);
                    }
                }
            }

            return gitDirectory;
        }
    }
}