// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitHubLinkException.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    using System;

    public class GitHubLinkException : Exception
    {
        #region Constructors

        public GitHubLinkException(string message)
            : base(message)
        {
        }

        #endregion
    }
}