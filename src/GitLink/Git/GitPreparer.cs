// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitPreparer.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Git
{
    using System;
    using System.IO;
    using System.Linq;
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

        public string Prepare(Context context, TemporaryFilesContext temporaryFilesContext)
        {
            Argument.IsNotNull(() => context);

            var gitDirectory = temporaryFilesContext.GetDirectory("git");
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

            Credentials credentials = null;
            var authentication = context.Authentication;
            if (!string.IsNullOrWhiteSpace(authentication.Username) && !string.IsNullOrWhiteSpace(authentication.Password))
            {
                Log.Info("Setting up credentials using name '{0}'", authentication.Username);

                credentials = new UsernamePasswordCredentials
                {
                    Username = authentication.Username,
                    Password = authentication.Password
                };
            }

            var cloneOptions = new CloneOptions
            {
                Checkout = false,
                IsBare = true,
                CredentialsProvider = (url, username, supportedTypes) => credentials
            };

            Repository.Clone(context.TargetUrl, gitDirectory, cloneOptions);

            if (!string.IsNullOrWhiteSpace(context.TargetBranch))
            {
                using (var repository = new Repository(gitDirectory))
                {
                    Reference newHead = null;

                    var localReference = GetLocalReference(repository, context.TargetBranch);
                    if (localReference != null)
                    {
                        newHead = localReference;
                    }

                    if (newHead == null)
                    {
                        var remoteReference = GetRemoteReference(repository, context.TargetBranch, context.TargetUrl);
                        if (remoteReference != null)
                        {
                            repository.Network.Fetch(context.TargetUrl, new[]
                            {
                                string.Format("{0}:{1}", remoteReference.CanonicalName, context.TargetBranch)
                            });

                            newHead = repository.Refs[string.Format("refs/heads/{0}", context.TargetBranch)];
                        }
                    }

                    if (newHead != null)
                    {
                        Log.Info("Switching to branch '{0}'", context.TargetBranch);

                        repository.Refs.UpdateTarget(repository.Refs.Head, newHead);
                    }
                }
            }

            return gitDirectory;
        }

        private static Reference GetLocalReference(Repository repository, string branchName)
        {
            var targetBranchName = branchName.GetCanonicalBranchName();

            return repository.Refs.FirstOrDefault(localRef => string.Equals(localRef.CanonicalName, targetBranchName));
        }

        private static DirectReference GetRemoteReference(Repository repository, string branchName, string repositoryUrl)
        {
            var targetBranchName = branchName.GetCanonicalBranchName();
            var remoteReferences = repository.Network.ListReferences(repositoryUrl);

            return remoteReferences.FirstOrDefault(remoteRef => string.Equals(remoteRef.CanonicalName, targetBranchName));
        }
    }
}