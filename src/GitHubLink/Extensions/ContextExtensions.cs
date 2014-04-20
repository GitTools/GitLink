// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitHubLink
{
    using Catel;
    using Catel.IO;

    public static class ContextExtensions
    {
        public static string GetRelativePath(this Context context, string fullPath)
        {
            Argument.IsNotNull(() => context);
            Argument.IsNotNull(() => fullPath);

            return Path.GetRelativePath(fullPath, context.SolutionDirectory);
        }
    }
}