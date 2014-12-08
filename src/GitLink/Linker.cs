// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Linker.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using Git;
    using Microsoft.Build.Evaluation;

    /// <summary>
    /// Class Linker.
    /// </summary>
    public static class Linker
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static int Link(Context context)
        {
            int? exitCode = null;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            context.ValidateContext();

            if (!string.IsNullOrEmpty(context.LogFile))
            {
                var fileLogListener = new FileLogListener(context.LogFile, 25 * 1024);
                fileLogListener.IsDebugEnabled = context.IsDebug;

                fileLogListener.IgnoreCatelLogging = true;
                LogManager.AddListener(fileLogListener);
            }

            if (!PdbStrHelper.IsPdbStrAvailable())
            {
                Log.Error("PdbStr is not found on the computer, please install 'Debugging Tools for Windows'");
                return -1;
            }

            try
            {
                var projects = new List<Project>();
                string[] solutionFiles;
                if (string.IsNullOrEmpty(context.SolutionFile))
                {
                    solutionFiles = Directory.GetFiles(context.SolutionDirectory, "*.sln", SearchOption.AllDirectories);
                }
                else
                {
                    var pathToSolutionFile = Path.Combine(context.SolutionDirectory, context.SolutionFile);
                    if (!File.Exists(pathToSolutionFile))
                    {
                        Log.Error("Could not find solution file: " + pathToSolutionFile);
                        return -1;
                    }

                    solutionFiles = new[] { pathToSolutionFile };
                }

                foreach (var solutionFile in solutionFiles)
                {
                    var solutionProjects = ProjectHelper.GetProjects(solutionFile, context.ConfigurationName);
                    projects.AddRange(solutionProjects);
                }

                var provider = context.Provider;
                if (provider == null)
                {
                    Log.ErrorAndThrowException<GitLinkException>("Cannot find a matching provider for '{0}'", context.TargetUrl);
                }

                Log.Info("Using provider '{0}'", provider.GetType().Name);

                var shaHash = context.Provider.GetShaHashOfCurrentBranch(context);

                Log.Info("Using commit sha '{0}' as version stamp", shaHash);

                var projectCount = projects.Count();
                var failedProjects = new List<Project>();
                Log.Info("Found '{0}' project(s)", projectCount);

                foreach (var project in projects)
                {
                    try
                    {
                        if (context.IsDebug)
                        {
                            project.DumpProperties();
                        }

                        if (!LinkProject(context, project, shaHash))
                        {
                            failedProjects.Add(project);
                        }
                    }
                    catch (Exception)
                    {
                        failedProjects.Add(project);
                    }
                }

                Log.Info("All projects are done. {0} of {1} succeeded", projectCount - failedProjects.Count, projectCount);

                if (failedProjects.Count > 0)
                {
                    Log.Info(string.Empty);
                    Log.Info("The following projects have failed:");
                    Log.Indent();

                    foreach (var failedProject in failedProjects)
                    {
                        Log.Info("* {0}", context.GetRelativePath(failedProject.GetProjectName()));
                    }

                    Log.Unindent();
                }

                exitCode = (failedProjects.Count == 0) ? 0 : -1;
            }
            catch (GitLinkException ex)
            {
                Log.Error(ex, "An error occurred");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred");
            }

            stopWatch.Stop();

            Log.Info(string.Empty);
            Log.Info("Completed in '{0}'", stopWatch.Elapsed);

            return exitCode ?? -1;
        }

        private static bool LinkProject(Context context, Project project, string shaHash)
        {
            Argument.IsNotNull(() => context);
            Argument.IsNotNull(() => project);

            try
            {
                var projectName = project.GetProjectName();

                Log.Info("Handling project '{0}'", projectName);

                Log.Indent();

                var compilables = project.GetCompilableItems().Select(x => x.GetFullFileName());

                var projectPdbFile = Path.GetFullPath(project.GetOutputPdbFile());
                var projectStcSrvFile = Path.GetFullPath(project.GetOutputSrcSrvFile());
                if (!File.Exists(projectPdbFile))
                {
                    Log.Warning("No pdb file found for '{0}', is project built in '{1}' mode with pdb files enabled? Expected file is '{2}'", projectName, context.ConfigurationName, projectPdbFile);
                    return false;
                }

                Log.Info("Verifying pdb file");

                var missingFiles = project.VerifyPdbFiles(compilables);
                foreach (var missingFile in missingFiles)
                {
                    Log.Warning("Missing file '{0}' or checksum '{1}' did not match", missingFile.Key, missingFile.Value);
                }

                var rawUrl = string.Format("{0}/{{0}}/%var2%", context.Provider.RawGitUrl);
                var paths = new Dictionary<string, string>();
                foreach (var compilable in compilables)
                {
                    var relativePathForUrl = compilable.Replace(context.SolutionDirectory, string.Empty).Replace("\\", "/");
                    while (relativePathForUrl.StartsWith("/"))
                    {
                        relativePathForUrl = relativePathForUrl.Substring(1, relativePathForUrl.Length - 1);
                    }

                    paths.Add(compilable, relativePathForUrl);
                }

                project.CreateSrcSrv(rawUrl, shaHash, paths);

                Log.Debug("Created source server link file, updating pdb file '{0}'", context.GetRelativePath(projectPdbFile));

                PdbStrHelper.Execute(projectPdbFile, projectStcSrvFile);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred while processing project '{0}'", project.GetProjectName());
                throw;
            }
            finally
            {
                Log.Unindent();
                Log.Info(string.Empty);
            }

            return true;
        }
    }
}