// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Linker.cs" company="Andrew Arnott">
//   Copyright (c) 2016 Andrew Arnott. All rights reserved.
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
            IReadOnlyCollection<string> sourceFiles;
            IReadOnlyDictionary<string, string> repoSourceFiles;
            using (var pdb = new PdbFile(pdbPath))
            {
                sourceFiles = pdb.GetFilesAndChecksums().Keys.ToList();

                if (options.GitWorkingDirectory != null)
                {
                    repositoryDirectory = Path.Combine(options.GitWorkingDirectory, ".git");
                }
                else
                {
                    repositoryDirectory = GitDirFinder.TreeWalkForGitDir(Path.GetDirectoryName(sourceFiles.First()));
                    if (repositoryDirectory == null)
                    {
                        Log.Error("No source files found that are tracked in a git repo.");
                        return false;
                    }
                }

                string workingDirectory = Path.GetDirectoryName(repositoryDirectory);

                var repository = new Lazy<Repository>(() => new Repository(repositoryDirectory));
                try
                {
                    string commitId = options.CommitId ?? repository.Value.Head.Commits.FirstOrDefault()?.Sha;
                    if (commitId == null)
                    {
                        Log.Error("No commit is checked out to HEAD. Have you committed yet?");
                        return false;
                    }

                    var providerManager = new Providers.ProviderManager();
                    Providers.IProvider provider;
                    if (options.GitRemoteUrl == null)
                    {
                        var candidateProviders = from remote in repository.Value.Network.Remotes
                                                 let p = providerManager.GetProvider(remote.Url)
                                                 where p != null
                                                 select p;
                        provider = candidateProviders.FirstOrDefault();
                    }
                    else
                    {
                        provider = providerManager.GetProvider(options.GitRemoteUrl.AbsoluteUri);
                    }

                    if (provider == null)
                    {
                        Log.Error("Unable to detect the remote git service.");
                        return false;
                    }

                    try
                    {
                        Repository repo = repository.Value;
                        repoSourceFiles = sourceFiles.ToDictionary(e => e, repo.GetRepoNormalizedPath);
                    }
                    catch (RepositoryNotFoundException)
                    {
                        // Normalize using file system since we can't find the git repo.
                        Log.Warning($"Unable to find git repo at \"{options.GitWorkingDirectory}\". Using file system to find canonical capitalization of file paths.");
                        repoSourceFiles = sourceFiles.ToDictionary(e => e, e => GetNormalizedPath(e, workingDirectory));
                    }

                    if (!options.SkipVerify)
                    {
                        Log.Debug("Verifying pdb file");

                        var missingFiles = pdb.FindMissingOrChangedSourceFiles();
                        foreach (var missingFile in missingFiles)
                        {
                            Log.Warning($"File \"{missingFile}\" missing or changed since the PDB was compiled.");
                        }
                    }

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
                catch (RepositoryNotFoundException)
                {
                    Log.Error($"Unable to find git repo at \"{options.GitWorkingDirectory}\".");
                    return false;
                }
                finally
                {
                    if (repository.IsValueCreated)
                    {
                        repository.Value.Dispose();
                    }
                }
            }

            Log.Debug("Created source server link file, updating pdb file '{0}'", Catel.IO.Path.GetRelativePath(pdbPath, repositoryDirectory));
            PdbStrHelper.Execute(PdbStrExePath, pdbPath, projectSrcSrvFile);
            var indexedFilesCount = repoSourceFiles.Values.Count(v => v != null);
            Log.Info($"Remote git source information for {indexedFilesCount}/{sourceFiles.Count} files written to pdb: \"{pdbPath}\"");

            return true;
        }

        private static string GetNormalizedPath(string path, string gitRepoRootDir)
        {
            Argument.IsNotNullOrEmpty(nameof(path), path);
            Argument.IsNotNullOrEmpty(nameof(gitRepoRootDir), gitRepoRootDir);

            string relativePath = Catel.IO.Path.GetRelativePath(path, gitRepoRootDir);
            string[] segments = relativePath.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
            DirectoryInfo currentDir = new DirectoryInfo(gitRepoRootDir);
            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];
                var next = currentDir.GetFileSystemInfos(segment).FirstOrDefault();
                segments[i] = next.Name; // get canonical capitalization
                currentDir = next as DirectoryInfo;
            }

            return Path.Combine(segments);
        }
    }
}