// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitHubLinkException.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitHubLink
{
    using System;

    public class GitHubLinkException : Exception
    {
        public GitHubLinkException(string message)
            : base(message)
        {
        }
    }
}