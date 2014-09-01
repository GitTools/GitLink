// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTestBase.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Test.IntegrationTests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using ApprovalTests;
    using Catel.Logging;
    using GitLink.Providers;
    using Microsoft.Win32;

    public class IntegrationTestBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected void PrepareTestSolution(string directory, string configurationName)
        {
            if (!Directory.Exists(directory))
            {
                throw new Exception(string.Format("Please make sure to clone the repository to '{0}'", directory));
            }

            var outputDirectory = Path.Combine(directory, "output");
            if (Directory.Exists(outputDirectory))
            {
                Log.Info("Deleting directory '{0}'", outputDirectory);

                Directory.Delete(outputDirectory, true);
            }

            Log.Info("Building project at '{0}'", directory);

            var msBuildDirectory = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0", "MSBuildToolsPath", string.Empty);
            var msBuildFileName = Path.Combine(msBuildDirectory, "msbuild.exe");
            var solutionFileName = Path.Combine(directory, "src", "TestSolution.sln");
            var arguments = string.Format("{0} /p:Configuration={1}", solutionFileName, configurationName);

            var processStartInfo = new ProcessStartInfo(msBuildFileName, arguments);
            processStartInfo.Arguments = arguments;
            processStartInfo.UseShellExecute = true;
            processStartInfo.CreateNoWindow = true;

            var process = Process.Start(processStartInfo);
            process.WaitForExit();
        }

        protected int RunGitLink(string directory, string repositoryUrl, string branchName, string configurationName)
        {
            PrepareTestSolution(directory, configurationName);

            var context = new Context(new ProviderManager())
            {
                SolutionDirectory = directory,
                TargetUrl = repositoryUrl,
                TargetBranch = branchName
            };

            return Linker.Link(context);
        }

        protected void VerifyUpdatedPdbs(string directory, string configurationName)
        {
            var outputDirectoryBase = Path.Combine(directory, "output", configurationName);

            VerifyUpdatedPdb(outputDirectoryBase, "CPlusPlusTestLibrary", true);
            VerifyUpdatedPdb(outputDirectoryBase, "CSharpTestLibrary");
            VerifyUpdatedPdb(outputDirectoryBase, "FSharpTestLibrary");
            VerifyUpdatedPdb(outputDirectoryBase, "VisualBasicTestLibrary");
        }

        private void VerifyUpdatedPdb(string outputDirectoryBase, string name, bool verifySrcSrv = false)
        {
            var pdbFileName = Path.Combine(outputDirectoryBase, name, "{0}.pdb");

            if (verifySrcSrv)
            {
                // Required for Approvals
                var pdbSrcSrvFileName = string.Format("{0}.srcsrv", pdbFileName);
                var pdbSrcSrvTxtFileName = string.Format("{0}.txt", pdbSrcSrvFileName);
                File.Copy(pdbSrcSrvFileName, pdbSrcSrvTxtFileName, true);

                Approvals.VerifyFile(pdbSrcSrvTxtFileName);
            }

            var containsVariables = false;
            var containsControl = false;
            var containsServer = false;

            var pdbContents = File.ReadAllLines(pdbFileName);
            foreach (var pdbContent in pdbContents)
            {
                if (pdbContent.Contains("SRCSRV: variables"))
                {
                    containsVariables = true;
                }

                if (pdbContent.Contains("SRCSRVVERCTRL="))
                {
                    containsControl = true;
                }

                if (pdbContent.Contains("SRCSRVTRG="))
                {
                    containsServer = true;
                }
            }

            if (!containsVariables || !containsControl || !containsServer)
            {
                throw new Exception("Generated pdb file is invalid");
            }
        }
    }
}