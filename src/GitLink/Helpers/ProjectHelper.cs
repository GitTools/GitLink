// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectHelper.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Catel;
    using Catel.Logging;
    using Catel.Reflection;
    using Microsoft.Build.Evaluation;
    using System.IO;
    using System.Text.RegularExpressions;

    public static class ProjectHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly Type SolutionParserType;
        private static readonly PropertyInfo SolutionReaderPropertyInfo;
        private static readonly PropertyInfo ProjectsPropertyInfo;
        private static readonly MethodInfo ParseSolutionMethodInfo;
        private static readonly PropertyInfo RelativePathPropertyInfo;
        private static readonly PropertyInfo ProjectTypePropertyInfo;
        private static readonly object KnownToBeMsBuildFormat;

        static ProjectHelper()
        {
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            SolutionParserType = TypeCache.GetType("Microsoft.Build.Construction.SolutionParser");
            if (SolutionParserType != null)
            {
                SolutionReaderPropertyInfo = SolutionParserType.GetProperty("SolutionReader", bindingFlags);
                ProjectsPropertyInfo = SolutionParserType.GetProperty("Projects", bindingFlags);
                ParseSolutionMethodInfo = SolutionParserType.GetMethod("ParseSolution", bindingFlags);
            }

            var projectInSolutionType = TypeCache.GetType("Microsoft.Build.Construction.ProjectInSolution");
            if (projectInSolutionType != null)
            {
                RelativePathPropertyInfo = projectInSolutionType.GetProperty("RelativePath", bindingFlags);
                ProjectTypePropertyInfo = projectInSolutionType.GetProperty("ProjectType", bindingFlags);
            }

            var solutionProjectTypeType = TypeCache.GetType("Microsoft.Build.Construction.SolutionProjectType");
            if (solutionProjectTypeType != null)
            {
                KnownToBeMsBuildFormat = Enum.Parse(solutionProjectTypeType, "KnownToBeMSBuildFormat");
            }
        }

        public static IEnumerable<Project> GetProjects(string solutionFile, string configurationName, string platformName)
        {
            var projects = new List<Project>();
            var solutionParser = SolutionParserType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);

            using (var streamReader = new StreamReader(solutionFile))
            {
                SolutionReaderPropertyInfo.SetValue(solutionParser, streamReader, null);
                ParseSolutionMethodInfo.Invoke(solutionParser, null);
                var solutionDirectory = Path.GetDirectoryName(solutionFile);
                var array = (Array)ProjectsPropertyInfo.GetValue(solutionParser, null);
                for (int i = 0; i < array.Length; i++)
                {
                    var projectInSolution = array.GetValue(i);
                    if (!ObjectHelper.AreEqual(ProjectTypePropertyInfo.GetValue(projectInSolution), KnownToBeMsBuildFormat))
                    {
                        continue;
                    }

                    var relativePath = (string)RelativePathPropertyInfo.GetValue(projectInSolution);
                    var projectFile = Path.Combine(solutionDirectory, relativePath);

                    var project = LoadProject(projectFile, configurationName, platformName, solutionDirectory);
                    if (project != null)
                    {
                        projects.Add(project);
                    }
                }
            }

            return projects;
        }

        public static Project LoadProject(string projectFile, string configurationName, string platformName, string solutionDirectory)
        {
            Argument.IsNotNullOrWhitespace(() => projectFile);
            Argument.IsNotNullOrWhitespace(() => configurationName);
            Argument.IsNotNullOrWhitespace(() => platformName);
            Argument.IsNotNullOrWhitespace(() => solutionDirectory);

            if (!solutionDirectory.EndsWith(@"\"))
            {
                solutionDirectory += @"\";
            }

            try
            {
                var collections = new Dictionary<string, string>();
                collections["Configuration"] = configurationName;
                collections["Platform"] = platformName;
                collections["SolutionDir"] = solutionDirectory;

                var project = new Project(projectFile, collections, null);
                return project;
            }
            catch (Exception ex)
            {
                Log.Warning("Failed to load project '{0}': {1}", projectFile, ex.Message);
                return null;
            }
        }

        public static bool ShouldBeIgnored(string projectName, ICollection<string> projectsToInclude, ICollection<string> projectsToIgnore)
        {
            Argument.IsNotNull(() => projectName);

            if (projectsToIgnore.Any(projectToIgnore => ProjectNameMatchesPattern(projectName, projectToIgnore)))
            {
                return true;
            }

            if (projectsToInclude.Count == 0) return false;

            if (projectsToInclude.All(projectToInclude => !ProjectNameMatchesPattern(projectName, projectToInclude)))
            {
                return true;
            }

            return false;
        }

        // pattern may be either a literal string, and then we'll be comparing literally ignoring case
        // or it can be a regex eclosed in slashes like /this-is-my-regex/
        private static bool ProjectNameMatchesPattern(string projectName, string pattern)
        {
            if (pattern.Length > 2 && pattern.StartsWith("/") && pattern.EndsWith("/"))
            {
                var ignoreRegex = new Regex(pattern.Substring(1, pattern.Length - 2), RegexOptions.IgnoreCase);
                if (ignoreRegex.IsMatch(projectName))
                    return true;
            }
            return string.Equals(projectName, pattern, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}