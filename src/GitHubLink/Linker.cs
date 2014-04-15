// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Linker.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    using System;
    using System.IO;
    using System.Linq;
    using Catel.Logging;
    using GitHubLink.Git;
    using SourceLink;

    /// <summary>
    /// Class Linker.
    /// </summary>
    public static class Linker
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static int Link(Context context)
        {
            int? exitCode = null;

            context.ValidateContext();

            try
            {
                var gitPreparer = new GitPreparer(context);
                gitPreparer.Prepare();

                var projectFiles = Directory.GetFiles(context.SolutionDirectory, "*.csproj", SearchOption.AllDirectories);

                int successCount = 0;
                int failedCount = 0;
                Log.Info("Found '{0}' project(s)", projectFiles.Count());

                foreach (var projectFile in projectFiles)
                {
                    Log.Info("Handling project '{0}'", projectFile);

                    Log.Indent();

                    try
                    {
                        var project = VsBuild.Project.LoadRelease.Static(projectFile);
                        var files = project.Compiles; // -- "/**/*AssemblyInfo*.cs" 
                        //proj.CreateSrcSrv (sprintf "%s/%s/{0}/%%var2%%" gitRaw gitName) repo.Revision (repo.Paths files)
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "An error occurred while processing project '{0}'", projectFile);
                    }

                    Log.Unindent();

                    //VsProj.LoadRelease()

                    //            SourceLink.

                    //                    let proj = VsProj.LoadRelease projectFile
                    //logfn "source linking %s" proj.OutputFilePdb
                    //let files = proj.Compiles -- "/**/*AssemblyInfo*.cs" 
                    ////repo.VerifyChecksums files
                    //proj.VerifyPdbChecksums files
                    //proj.CreateSrcSrv (sprintf "%s/%s/{0}/%%var2%%" gitRaw gitName) repo.Revision (repo.Paths files)
                    //Pdbstr.exec proj.OutputFilePdb proj.OutputFilePdbSrcSrv
                }

                // TODO: CAll SourceLink
                //SourceLink.
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                Log.Debug("Clearing temporary directory '{0}'", context.TempDirectory);

                Directory.Delete(context.TempDirectory);
            }

            return exitCode ?? -1;
        }
    }
}