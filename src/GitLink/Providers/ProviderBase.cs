// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProviderBase.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Providers
{
    using System.Linq;
    using Catel;
    using LibGit2Sharp;

    public abstract class ProviderBase
    {
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

        public string GetLatestCommitShaOfCurrentBranch(string repositoryDirectory)
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