// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.CommandLine;
    using System.Diagnostics;
    using System.IO;
    using Catel.Logging;
    using Logging;

    internal class Program
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static int Main(string[] args)
        {
#if DEBUG
            LogManager.AddDebugListener(true);
#endif

            var consoleLogListener = new OutputLogListener();
            LogManager.AddListener(consoleLogListener);

            Uri remoteGitUrl = null;
            string commitId = null;
            string baseDir = null;
            string pdbPath = null;
            bool skipVerify = false;
            bool downloadWithPowershell = false;
            var arguments = ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("u|url", ref remoteGitUrl, s => new Uri(s, UriKind.Absolute), "Url to remote git repository.");
                syntax.DefineOption("commit", ref commitId, "The git ref to assume all the source code belongs to.");
                syntax.DefineOption("baseDir", ref baseDir, "The path to the root of the git repo.");
                syntax.DefineOption("s|skipVerify", ref skipVerify, "Verify all source files are available in source control.");
                syntax.DefineOption("p|powershell", ref downloadWithPowershell, "Use an indexing strategy that won't rely on SRCSRV http support, but use a powershell command for URL download instead.");
                syntax.DefineParameter("pdb", ref pdbPath, "The PDB to add source indexing to.");

                if (!string.IsNullOrEmpty(pdbPath) && !File.Exists(pdbPath))
                {
                    syntax.ReportError($"File not found: \"{pdbPath}\"");
                }

                if (!string.IsNullOrEmpty(baseDir) && !Directory.Exists(baseDir))
                {
                    syntax.ReportError($"Directory not found: \"{baseDir}\"");
                }
            });

            if (string.IsNullOrEmpty(pdbPath))
            {
                Log.Info(arguments.GetHelpText());
                return 1;
            }

            var options = new LinkOptions
            {
                GitRemoteUrl = remoteGitUrl,
                GitWorkingDirectory = baseDir != null ? Catel.IO.Path.GetFullPath(baseDir, Environment.CurrentDirectory) : null,
                CommitId = commitId,
                SkipVerify = skipVerify,
                DownloadWithPowerShell = downloadWithPowershell,
            };

            Linker.Link(pdbPath, options);
            WaitForKeyPressWhenDebugging();
            return 0;
        }

        [Conditional("DEBUG")]
        private static void WaitForKeyPressWhenDebugging()
        {
            if (Debugger.IsAttached) // VS only closes the window immediately when debugging
            {
                Log.Info(string.Empty);
                Log.Info("Press any key to continue");

                Console.ReadKey();
            }
        }
    }
}