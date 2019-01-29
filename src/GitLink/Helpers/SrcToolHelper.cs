// <copyright file="SrcToolHelper.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>

namespace GitLink
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Catel;
    using Catel.Logging;
    using GitTools.Git;

    internal static class SrcToolHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        internal static List<string> GetSourceFiles(string srcToolFilePath, string projectPdbFile)
        {
            Argument.IsNotNullOrWhitespace(() => projectPdbFile);
            List<string> sources = new List<string>();

            var processStartInfo = new ProcessStartInfo(srcToolFilePath)
            {
                Arguments = string.Format("-r \"{0}\"", projectPdbFile),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            using (var process = new Process())
            {
                process.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        var sourceFile = e.Data.ToLower();

                        if (Linker.ValidExtension(sourceFile))
                        {
                            var repositoryDirectory = GitDirFinder.TreeWalkForGitDir(Path.GetDirectoryName(sourceFile));

                            if (repositoryDirectory != null)
                            {
                                sources.Add(sourceFile);
                            }
                        }
                    }
                };

                process.EnableRaisingEvents = true;
                process.StartInfo = processStartInfo;
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                var processExitCode = process.ExitCode;
                if (processExitCode != 0)
                {
                    throw Log.ErrorAndCreateException<GitLinkException>("SrcTool exited with unexpected error code '{0}'", processExitCode);
                }
            }

            return sources;
        }
    }
}
