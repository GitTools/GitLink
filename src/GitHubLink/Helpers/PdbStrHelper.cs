// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbStrHelper.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitHubLink
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Catel;
    using Catel.Logging;

    public static class PdbStrHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly List<String> PossibleLocations;

        static PdbStrHelper()
        {
            // TODO: Read from registry in the future?

            PossibleLocations = new List<string>(new[]
            {
                @"C:\Program Files (x86)\Windows Kits\8.1\Debuggers\x64\srcsrv\pdbstr.exe", // 6.3.9600.16384
                @"C:\Program Files\Microsoft Team Foundation Server 12.0\Tools\pdbstr.exe", // 6.3.9600.16384
                @"C:\Program Files (x86)\Windows Kits\8.0\Debuggers\x64\srcsrv\pdbstr.exe", // 6.2.9200.16384
                @"C:\Program Files\Microsoft Team Foundation Server 11.0\Tools\pdbstr.exe", // 6.2.9200.16384
                @"C:\Program Files\Debugging Tools for Windows (x64)\srcsrv\pdbstr.exe"
            });
        }

        public static bool IsPdbStrAvailable()
        {
            var pdbStrFileName = GetPdbStrFileName();
            return !string.IsNullOrEmpty(pdbStrFileName);
        }

        public static string GetPdbStrFileName()
        {
            foreach (var possibleLocation in PossibleLocations)
            {
                if (File.Exists(possibleLocation))
                {
                    return possibleLocation;
                }
            }

            return null;
        }

        public static void Execute(string projectPdbFile, string pdbStrFile)
        {
            Argument.IsNotNullOrWhitespace(() => projectPdbFile);
            Argument.IsNotNullOrWhitespace(() => pdbStrFile);

            var pdbStrFileName = GetPdbStrFileName();
            var processStartInfo = new ProcessStartInfo(pdbStrFileName)
            {
                Arguments = string.Format("-w -s:srcsrv -p:\"{0}\" -i:\"{1}\"", projectPdbFile, pdbStrFile),
                CreateNoWindow = true,
                UseShellExecute = false
            };

            var process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();

            var processExitCode = process.ExitCode;
            if (processExitCode != 0)
            {
                Log.ErrorAndThrowException<GitHubLinkException>("PdbStr exited with unexpected error code '{0}'", processExitCode);
            }
        }
    }
}