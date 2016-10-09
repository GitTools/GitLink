// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitLinkException.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink
{
    using System;

    public class GitLinkException : Exception
    {
        public GitLinkException(string message)
            : base(message)
        {
        }
    }
}