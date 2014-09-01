// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using Microsoft.Build.Evaluation;
    using SourceLink;

    public static class ProjectExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static string GetProjectName(this Project project)
        {
            Argument.IsNotNull(() => project);

            var projectName = GetProjectPropertyValue(project, "MSBuildProjectName");
            return projectName ?? Path.GetFileName(project.FullPath);
        }

        public static string GetProjectPropertyValue(this Project project, string propertyName)
        {
            Argument.IsNotNull(() => project);
            Argument.IsNotNullOrWhitespace(() => propertyName);

            var projectNameProperty = (from property in project.Properties
                                       where string.Equals(property.Name, "MSBuildProjectName")
                                       select property).FirstOrDefault();

            if (projectNameProperty == null)
            {
                Log.Debug("Failed to get property 'MSBuildProjectName', returning null");
                return null;
            }

            return projectNameProperty.EvaluatedValue;
        }

        public static void CreateSrcSrv(this Project project, string rawUrl, string revision, Dictionary<string, string> paths)
        {
            Argument.IsNotNull(() => project);
            Argument.IsNotNullOrWhitespace(() => rawUrl);
            Argument.IsNotNullOrWhitespace(() => revision);

            var srcsrvFile = GetOutputSrcSrvFile(project);

            File.WriteAllBytes(srcsrvFile, SrcSrv.create(rawUrl, revision, paths.Select(x => new Tuple<string, string>(x.Key, x.Value))));
        }

        public static IEnumerable<ProjectItem> GetCompilableItems(this Project project)
        {
            Argument.IsNotNull(() => project);

            return project.Items.Where(x => string.Equals(x.ItemType, "Compile") || string.Equals(x.ItemType, "ClCompile") || string.Equals(x.ItemType, "ClInclude"));
        }

        public static Dictionary<string, string> VerifyPdbFiles(this Project project, IEnumerable<string> files)
        {
            Argument.IsNotNull(() => project);

            using (var pdb = new PdbFile(Path.ChangeExtension(project.GetOutputFile(), ".pdb")))
            {
                return pdb.VerifyPdbFiles(files);
            }
        }

        public static string GetOutputSrcSrvFile(this Project project)
        {
            Argument.IsNotNull(() => project);

            var pdbFile = GetOutputPdbFile(project);
            return string.Format("{0}.srcsrv", pdbFile);
        }

        public static string GetOutputPdbFile(this Project project)
        {
            Argument.IsNotNull(() => project);

            var outputFile = project.GetOutputFile();
            var pdbFile = Path.ChangeExtension(outputFile, ".pdb");

            return pdbFile;
        }

        public static string GetOutputFile(this Project project)
        {
            Argument.IsNotNull(() => project);

            string extension = ".dll";

            string outputType = project.GetProperty("OutputType").EvaluatedValue;
            if (outputType.Contains("Exe") || outputType.Contains("WinExe"))
            {
                extension = ".exe";
            }

            var projectOutputPath = project.GetPropertyValue("OutputPath");
            var outputPath = Path.Combine(project.DirectoryPath, projectOutputPath);
            return Path.Combine(outputPath, string.Format("{0}{1}", project.GetProperty("AssemblyName").EvaluatedValue, extension));
        }
    }
}