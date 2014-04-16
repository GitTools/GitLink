// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectItemExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    using Catel;
    using Catel.IO;
    using Microsoft.Build.Evaluation;

    public static class ProjectItemExtensions
    {
        public static string GetRelativeFileName(this ProjectItem projectItem)
        {
            Argument.IsNotNull(() => projectItem);

            return projectItem.EvaluatedInclude;
        }

        public static string GetFullFileName(this ProjectItem projectItem)
        {
            Argument.IsNotNull(() => projectItem);

            var filePath = Path.Combine(projectItem.Project.DirectoryPath, projectItem.GetRelativeFileName());
            var fullFile = Path.GetFullPath(filePath, System.Environment.CurrentDirectory);
            return fullFile;
        }
    }
}