// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectHelper.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System.Collections.Generic;
    using Catel;
    using Microsoft.Build.Evaluation;

    public static class ProjectHelper
    {
        public static Project LoadProject(string projectName, string configurationName)
        {
            Argument.IsNotNullOrWhitespace(() => projectName);
            Argument.IsNotNullOrWhitespace(() => configurationName);

            var collections = new Dictionary<string, string>();
            collections["Configuration"] = configurationName;

            var project = new Project(projectName, collections, null);
            return project;
        }
    }
}