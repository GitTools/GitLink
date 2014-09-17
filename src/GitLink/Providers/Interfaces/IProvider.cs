// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProvider.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Providers
{
    public interface IProvider
    {
        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the company URL.
        /// </summary>
        /// <value>The company URL.</value>
        string CompanyUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the project URL.
        /// </summary>
        /// <value>The project URL.</value>
        string ProjectUrl { get; set; }

        /// <summary>
        /// Gets the raw git URL.
        /// </summary>
        /// <value>The raw git URL.</value>
        string RawGitUrl { get; }

        bool Initialize(string url);

        string GetShaHashOfCurrentBranch(Context context);
    }
}