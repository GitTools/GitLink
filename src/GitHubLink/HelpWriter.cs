// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelpWriter.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    using System;
    using Catel.Reflection;

    public static class HelpWriter
    {
        #region Methods

        public static void WriteAppHeader(Action<string> writer)
        {
            var assembly = typeof(HelpWriter).Assembly;

            writer(string.Format("{0} v{1}", assembly.Title(), assembly.Version()));
            writer("===================");
            writer(string.Empty);
        }

        public static void WriteHelp(Action<string> writer)
        {
            var message =
                @"Update pdb files to link all sources. This will allow anyone to step through the source code while debugging without a symbol source server.

Note that the solution must be built because this application will update existing pdb files.

GitHubLink [solutionPath] -url [urlToRepository]

    solutionPath     The directory containing the solution with the pdb files.
    -url [url]       Url to remote git repository.
    -b [branch]      Name of the branch to use on the remote repository.
    -l [file]        
";
            writer(message);
        }

        #endregion
    }
}