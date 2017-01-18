// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkMethod.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink
{
    /// <summary>
    /// The styles of operations a SRCSRV can perform to retrieve the source code.
    /// </summary>
    public enum LinkMethod
    {
        /// <summary>
        /// SRCSRV downloads from a web URL directly.
        /// </summary>
        Http,

        /// <summary>
        /// Use an indexing strategy that won't rely on SRCSRV http support, but use a powershell command for URL download instead.
        /// </summary>
        Powershell,
    }
}
