// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbStrHelper.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System.Diagnostics;
    using Catel;
    using Catel.Logging;

    public static class PdbStrHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static void Execute(string pdbStrFileName, string projectPdbFile, string pdbStrFile)
        {
            Argument.IsNotNullOrWhitespace(() => projectPdbFile);
            Argument.IsNotNullOrWhitespace(() => pdbStrFile);

            var processStartInfo = new ProcessStartInfo(pdbStrFileName)
            {
                Arguments = string.Format("-w -s:srcsrv -p:\"{0}\" -i:\"{1}\"", projectPdbFile, pdbStrFile),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var process = new Process();
            process.OutputDataReceived += (s, e) => Log.Info(e.Data);
            process.ErrorDataReceived += (s, e) => Log.Error(e.Data);
            process.EnableRaisingEvents = true;
            process.StartInfo = processStartInfo;
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();

            var processExitCode = process.ExitCode;
            if (processExitCode != 0)
            {
                throw Log.ErrorAndCreateException<GitLinkException>("PdbStr exited with unexpected error code '{0}'", processExitCode);
            }
        }
    }
}