// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    using Catel;

    public static class ContextExtensions
    {
        public static string GetRelativePath(this Context context, string fullPath)
        {
            Argument.IsNotNull(() => context);
            Argument.IsNotNull(() => fullPath);

            return Catel.IO.Path.GetRelativePath(fullPath, context.SolutionDirectory);
        }
    }
}