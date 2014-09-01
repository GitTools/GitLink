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
                var gitPreparer = new GitPreparer(context);
                gitPreparer.Prepare();

                var projects = new List<Project>();
                var solutionFiles = Directory.GetFiles(context.SolutionDirectory, "*.sln", SearchOption.AllDirectories);
                foreach (var solutionFile in solutionFiles)
                {
                    var solutionProjects = ProjectHelper.GetProjects(solutionFile, context.ConfigurationName);
                    projects.AddRange(solutionProjects);
                }

                var projectCount = projects.Count();
                var failedProjects = new List<Project>();
                Log.Info("Found '{0}' project(s)", projectCount);

                foreach (var project in projects)
                {
                    try
                    {
                        if (!LinkProject(context, project))
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
            finally
            {
                Log.Debug("Clearing temporary directory '{0}'", context.TempDirectory);

                DeleteHelper.DeleteGitRepository(context.TempDirectory);
            }

            stopWatch.Stop();

            Log.Info(string.Empty);
            Log.Info("Completed in '{0}'", stopWatch.Elapsed);

            return exitCode ?? -1;
        }

        private static bool LinkProject(Context context, Project project)
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
                    Log.Warning("No pdb file found for '{0}', is project built in '{1}' mode with pdb files enabled?", projectName, context.ConfigurationName);
                    return false;
                }

                // Note: verify doesn't work yet, maybe implement later
                Log.Warning("Pdb verification not yet implemented, cannot garantuee that pdb-files are up-to-date");
                //var missingFiles = project.VerifyPdbFiles(compilables);
                //foreach (var missingFile in missingFiles)
                //{
                //    Log.Warning("Missing file '{0}' or checksum '{1}' did not match", missingFile.Key, missingFile.Value);
                //}

                var rawUrl = string.Format("{0}/{{0}}/%var2%", context.Provider.RawGitUrl);
                var revision = context.Provider.GetLatestCommitShaOfCurrentBranch(context.TempDirectory);

                var paths = new Dictionary<string, string>();
                foreach (var compilable in compilables)
                {
                    var relativePathForUrl = compilable.Replace(context.SolutionDirectory, string.Empty)
                                                       .Replace("\\", "/");
                    while (relativePathForUrl.StartsWith("/"))
                    {
                        relativePathForUrl = relativePathForUrl.Substring(1, relativePathForUrl.Length - 1);
                    }

                    paths.Add(compilable, relativePathForUrl);
                }

                project.CreateSrcSrv(rawUrl, revision, paths);

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