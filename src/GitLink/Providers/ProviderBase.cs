// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProviderBase.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Providers
{
    using System;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using GitTools;
    using GitTools.Git;
    using LibGit2Sharp;

    public abstract class ProviderBase : IProvider
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IRepositoryPreparer _repositoryPreparer;

        protected ProviderBase(IRepositoryPreparer repositoryPreparer)
        {
            Argument.IsNotNull(() => repositoryPreparer);

            _repositoryPreparer = repositoryPreparer;
        }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the company URL.
        /// </summary>
        /// <value>The company URL.</value>
        public string CompanyUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the project URL.
        /// </summary>
        /// <value>The project URL.</value>
        public string ProjectUrl { get; set; }

        /// <summary>
        /// Gets the raw git URL.
        /// </summary>
        /// <value>The raw git URL.</value>
        public abstract string RawGitUrl { get; }

        public abstract bool Initialize(string url);

        public string GetShaHashOfCurrentBranch(Context context, TemporaryFilesContext temporaryFilesContext)
        {
            Argument.IsNotNull(() => context);

            string commitSha = null;
            var deleteTempRepository = false;
            var repositoryDirectory = context.SolutionDirectory;

            if (_repositoryPreparer.IsPreparationRequired(context))
            {
                Log.Info("No local repository is found in '{0}', creating a temporary one", repositoryDirectory);

                repositoryDirectory = _repositoryPreparer.Prepare(context, temporaryFilesContext);
                deleteTempRepository = true;
            }

            using (var repository = new Repository(repositoryDirectory))
            {
                if (string.IsNullOrEmpty(context.ShaHash))
                {
                    Log.Info("No sha hash is available on the context, retrieving latest commit of current branch");

                    var lastCommit = repository.Commits.First();
                    commitSha = lastCommit.Sha;
                }
                else
                {
                    Log.Info("Checking if commit with sha hash '{0}' exists on the repository", context.ShaHash);

                    var commit = repository.Commits.FirstOrDefault(c => string.Equals(c.Sha, context.ShaHash, StringComparison.OrdinalIgnoreCase));
                    if (commit != null)
                    {
                        commitSha = commit.Sha;
                    }
                }
            }

            if (deleteTempRepository)
            {
                Log.Debug("Deleting temporary directory '{0}'", repositoryDirectory);

                try
                {
                    // Always sleep 1 second to give IO a chance to release
                    ThreadHelper.Sleep(1000);

                    Directory.Delete(repositoryDirectory, true);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete temporary directory");
                }
            }

            if (commitSha == null)
            {
                Log.ErrorAndThrowException<GitLinkException>("Cannot find commit '{0}' in repo.", context.ShaHash);
            }

            return commitSha;
        }
    }
}