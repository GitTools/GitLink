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
    using System.Reflection;
    using Catel;
    using Catel.Logging;
    using GitTools;
    using GitTools.Git;
    using LibGit2Sharp;
    using Microsoft.Build.Evaluation;
    using Pdb;

    /// <summary>
    /// Class Linker.
    /// </summary>
    public static class Linker
    {
        private static readonly string PdbStrExePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "pdbstr.exe");
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static bool Link(string pdbPath, LinkOptions options = default(LinkOptions))
        {
            Argument.IsNotNullOrEmpty(() => pdbPath);

            var projectSrcSrvFile = pdbPath + ".srcsrv";
            string repositoryDirectory;
            using (var pdb = new PdbFile(pdbPath))
            {
                var sourceFiles = pdb.GetFiles().Select(f => f.Item1).ToList();

                repositoryDirectory = GitDirFinder.TreeWalkForGitDir(Path.GetDirectoryName(sourceFiles.First()));
                if (repositoryDirectory == null)
                {
                    Log.Error("No source files found that are tracked in a git repo.");
                    return false;
                }

                using (var repository = new Repository(repositoryDirectory))
                {
                    var repoSourceFiles = sourceFiles.ToDictionary(e => e, repository.GetRepoNormalizedPath);

                    var providerManager = new Providers.ProviderManager();
                    Providers.IProvider provider;
                    if (options.GitRemoteUrl == null)
                    {
                        var candidateProviders = from remote in repository.Network.Remotes
                                                 let p = providerManager.GetProvider(remote.Url)
                                                 where p != null
                                                 select p;
                        provider = candidateProviders.First();
                    }
                    else
                    {
                        provider = providerManager.GetProvider(options.GitRemoteUrl.AbsoluteUri);
                    }

                    if (!options.SkipVerify)
                    {
                        Log.Info("Verifying pdb file");

                        var missingFiles = pdb.FindMissingOrChangedSourceFiles();
                        foreach (var missingFile in missingFiles)
                        {
                            Log.Warning($"File \"{missingFile}\" missing or changed since the PDB was compiled.");
                        }
                    }

                    string commitId;
                    commitId = repository.Head.Commits.First().Sha;

                    string rawUrl = provider.RawGitUrl;
                    if (!rawUrl.Contains("%var2%") && !rawUrl.Contains("{0}"))
                    {
                        rawUrl = string.Format("{0}/{{0}}/%var2%", rawUrl);
                    }

                    var srcSrvContext = new SrcSrvContext
                    {
                        RawUrl = rawUrl,
                        DownloadWithPowershell = options.DownloadWithPowerShell,
                        Revision = commitId,
                    };
                    foreach (var sourceFile in repoSourceFiles)
                    {
                        // Skip files that aren't tracked by source control.
                        if (sourceFile.Value != null)
                        {
                            srcSrvContext.Paths.Add(Tuple.Create(sourceFile.Key, sourceFile.Value.Replace('\\', '/')));
                        }
                    }

                    if (provider is Providers.VisualStudioTeamServicesProvider)
                    {
                        srcSrvContext.VstsData["TFS_COLLECTION"] = provider.CompanyUrl;
                        srcSrvContext.VstsData["TFS_TEAM_PROJECT"] = provider.ProjectName;
                        srcSrvContext.VstsData["TFS_REPO"] = provider.ProjectName;
                    }

                    ProjectExtensions.CreateSrcSrv(projectSrcSrvFile, srcSrvContext);
                }
            }

            Log.Debug("Created source server link file, updating pdb file '{0}'", Catel.IO.Path.GetRelativePath(pdbPath, repositoryDirectory));
            PdbStrHelper.Execute(PdbStrExePath, pdbPath, projectSrcSrvFile);
            return true;
        }
    }
}