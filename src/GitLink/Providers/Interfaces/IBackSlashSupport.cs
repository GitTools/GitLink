// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBackSlashSupport.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Providers
{
    /// <summary>
    /// Implementing this interface path resolver gives provider a change to determine 
    /// if back slashes are supported or should be replaced.
    /// </summary>
    public interface IBackSlashSupport
    {
        /// <summary>
        /// Gets whether back slashes in paths are supported or should be replaced.
        /// </summary>
        /// <value>
        /// If value is <c>true</c>, back slashes are not replaced; otherwise back slahes are replaced by forward slashes.
        /// </value>
        bool IsBackSlashSupported { get; }
    }
}
