// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectHelper.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Catel;
    using Catel.Logging;
    using Catel.Reflection;
    using Microsoft.Build.Evaluation;
    using System.IO;
    using System.Text.RegularExpressions;
    using ImpromptuInterface;

    public static class ProjectHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly Type SolutionParserType;
        private static readonly object KnownToBeMsBuildFormat;

        public interface ISolutionParser
        {
            StreamReader SolutionReader { get; set; }
            object[] Projects { get; }
            void ParseSolution();
        }

        public interface IProjectInSolution
        {
            object ProjectType { get; }
            string RelativePath { get; }
            object ProjectConfigurations { get; }
        }

        public interface IProjectConfigurationInSolution
        {
            bool IncludeInBuild { get; }
        }

        static ProjectHelper()
        {
            SolutionParserType = TypeCache.GetType("Microsoft.Build.Construction.SolutionParser");

            var solutionProjectTypeType = TypeCache.GetType("Microsoft.Build.Construction.SolutionProjectType");
            if (solutionProjectTypeType != null)
            {
                KnownToBeMsBuildFormat = Enum.Parse(solutionProjectTypeType, "KnownToBeMSBuildFormat");
            }
        }

        public static IEnumerable<Project> GetProjects(string solutionFile, string configurationName, string platformName)
        {
            var projects = new List<Project>();
            var invoke = SolutionParserType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);
            var solutionParser = invoke.ActLike<ISolutionParser>();

            using (var streamReader = new StreamReader(solutionFile))
            {
                solutionParser.SolutionReader = streamReader;
                solutionParser.ParseSolution();
                var solutionDirectory = Path.GetDirectoryName(solutionFile);
                var projectsInSolution = solutionParser.Projects.AllActLike<IProjectInSolution>();
                foreach (var projectInSolution in projectsInSolution)
                {
                    var isKnownToBeMsBuildFormat = ObjectHelper.AreEqual(projectInSolution.ProjectType, KnownToBeMsBuildFormat);
                    var isSelectedForBuild = ProjectIsSelectedForBuild(projectInSolution, configurationName, platformName);
                    if (!isKnownToBeMsBuildFormat || !isSelectedForBuild)
                    {
                        continue;
                    }

                    var relativePath = projectInSolution.RelativePath;
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

            if (projectsToInclude.Count == 0)
            {
                return false;
            }

            if (projectsToInclude.All(projectToInclude => !ProjectNameMatchesPattern(projectName, projectToInclude)))
            {
                return true;
            }

            return false;
        }

        // pattern may be either a literal string, and then we'll be comparing literally ignoring case
        // or it can be a regex enclosed in slashes like /this-is-my-regex/
        private static bool ProjectNameMatchesPattern(string projectName, string pattern)
        {
            Argument.IsNotNull(() => pattern);

            if (pattern.Length > 2 && pattern.StartsWith("/") && pattern.EndsWith("/"))
            {
                var ignoreRegex = new Regex(pattern.Substring(1, pattern.Length - 2), RegexOptions.IgnoreCase);
                if (ignoreRegex.IsMatch(projectName))
                {
                    return true;
                }
            }
            return string.Equals(projectName, pattern, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool ProjectIsSelectedForBuild(IProjectInSolution project, string configurationName, string platformName)
        {
            var configurationPlatformKey = configurationName + "|" + platformName;

            var configurationsDictionary = (IDictionary)project.ProjectConfigurations;
            if (configurationsDictionary.Contains(configurationPlatformKey))
            {
                var cis = configurationsDictionary[configurationPlatformKey].ActLike<IProjectConfigurationInSolution>();

                return cis.IncludeInBuild;
            }

            return true;
        }
    }
}