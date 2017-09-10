// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Linker.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
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
    using Providers;

    /// <summary>
    /// Class Linker.
    /// </summary>
    public static class Linker
    {
        private static readonly string FilenamePlaceholder = Uri.EscapeUriString("{filename}");
        private static readonly string RevisionPlaceholder = Uri.EscapeUriString("{revision}");
        private static readonly string PdbStrExePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "pdbstr.exe");
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static bool Link(string pdbPath, LinkOptions options = default(LinkOptions))
        {
            Argument.IsNotNullOrEmpty(() => pdbPath);

            var projectSrcSrvFile = pdbPath + ".srcsrv";
            string repositoryDirectory = null;
            IReadOnlyList<string> sourceFiles;
            IReadOnlyDictionary<string, string> repoSourceFiles;

            if (options.GitWorkingDirectory != null)
            {
                repositoryDirectory = Path.Combine(options.GitWorkingDirectory, ".git");
                if (!Directory.Exists(repositoryDirectory))
                {
                    Log.Error("Provided directory does not contain a git depot.");
                    return false;
                }
            }

            if (PortablePdbHelper.IsPortablePdb(pdbPath))
            {
                Log.Warning("Portable PDB format is not compatible with GitLink. Please use SourceLink (https://github.com/ctaggart/SourceLink).");
                return true;
            }

            if (options.IndexAllDepotFiles)
            {
                if (repositoryDirectory == null)
                {
                    repositoryDirectory = GitDirFinder.TreeWalkForGitDir(Path.GetDirectoryName(pdbPath));
                    if (repositoryDirectory == null)
                    {
                        Log.Error("Couldn't auto detect git repo. Please use -baseDir to manually set it.");
                        return false;
                    }
                }

                sourceFiles = GetSourceFilesFromDepot(repositoryDirectory);
            }
            else
            {
                sourceFiles = GetSourceFilesFromPdb(pdbPath, !options.SkipVerify);

                string someSourceFile = sourceFiles.FirstOrDefault();
                if (someSourceFile == null)
                {
                    Log.Error("No source files were found in the PDB. If you're PDB is a native one you should use -a option.");
                    return false;
                }

                if (repositoryDirectory == null)
                {
                    repositoryDirectory = GitDirFinder.TreeWalkForGitDir(Path.GetDirectoryName(sourceFiles.FirstOrDefault()));
                    if (repositoryDirectory == null)
                    {
                        Log.Error("No source files found that are tracked in a git repo.");
                        return false;
                    }
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
                    repoSourceFiles = sourceFiles.ToDictionary(e => e, e => repo.GetNormalizedPath(e));
                }
                catch (RepositoryNotFoundException)
                {
                    // Normalize using file system since we can't find the git repo.
                    Log.Warning($"Unable to find git repo at \"{options.GitWorkingDirectory}\". Using file system to find canonical capitalization of file paths.");
                    repoSourceFiles = sourceFiles.ToDictionary(e => e, e => GetNormalizedPath(e, workingDirectory));
                }

                string rawUrl = provider.RawGitUrl;
                if (rawUrl.Contains(RevisionPlaceholder) || rawUrl.Contains(FilenamePlaceholder))
                {
                    if (!rawUrl.Contains(RevisionPlaceholder) || !rawUrl.Contains(FilenamePlaceholder))
                    {
                        Log.Error("Supplied custom URL pattern must contain both a revision and a filename placeholder.");
                        return false;
                    }

                    rawUrl = rawUrl
                        .Replace(RevisionPlaceholder, "{0}")
                        .Replace(FilenamePlaceholder, "%var2%");
                }
                else
                {
                    rawUrl = $"{rawUrl}/{{0}}/%var2%";
                }

                Log.Info($"Using {string.Format(rawUrl, commitId)} for source server URLs.");
                var srcSrvContext = new SrcSrvContext
                {
                    RawUrl = rawUrl,
                    DownloadWithPowershell = options.Method == LinkMethod.Powershell,
                    Revision = commitId,
                };
                foreach (var sourceFile in repoSourceFiles)
                {
                    // Skip files that aren't tracked by source control.
                    if (sourceFile.Value != null)
                    {
                        string relativePathForUrl = ReplaceSlashes(provider, sourceFile.Value);
                        srcSrvContext.Paths.Add(Tuple.Create(sourceFile.Key, relativePathForUrl));
                    }
                }

                // When using the VisualStudioTeamServicesProvider, add extra infomration to dictionary with VSTS-specific data
                if (provider is Providers.VisualStudioTeamServicesProvider)
                {
                    srcSrvContext.VstsData["TFS_COLLECTION"] = provider.CompanyUrl;
                    srcSrvContext.VstsData["TFS_TEAM_PROJECT"] = provider.ProjectName;
                    srcSrvContext.VstsData["TFS_REPO"] = provider.ProjectUrl;
                }

                CreateSrcSrv(projectSrcSrvFile, srcSrvContext);
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

            Log.Debug("Created source server link file, updating pdb file \"{0}\"", Catel.IO.Path.GetRelativePath(pdbPath, repositoryDirectory));
            PdbStrHelper.Execute(PdbStrExePath, pdbPath, projectSrcSrvFile);
            var indexedFilesCount = repoSourceFiles.Values.Count(v => v != null);
            Log.Info($"Remote git source information for {indexedFilesCount}/{sourceFiles.Count} files written to pdb: \"{pdbPath}\"");

            return true;
        }

        private static List<string> GetSourceFilesFromPdb(string pdbPath, bool verifyFiles)
        {
            using (var pdb = new PdbFile(pdbPath))
            {
                var sources = pdb.GetFilesAndChecksums().Keys.ToList();

                if (verifyFiles)
                {
                    Log.Debug("Verifying pdb files");

                    var missingFiles = pdb.FindMissingOrChangedSourceFiles();
                    foreach (var missingFile in missingFiles)
                    {
                        Log.Warning($"File \"{missingFile}\" missing or changed since the PDB was compiled.");
                    }
                }

                return sources;
            }
        }

        private static List<string> GetSourceFilesFromDepot(string repositoryDirectory)
        {
            IEnumerable<string> sourceFiles;
            var repo = new Repository(repositoryDirectory);
            {
                sourceFiles = from file in Directory.GetFiles(repo.Info.WorkingDirectory, "*.*", SearchOption.AllDirectories)
                              where !repo.Ignore.IsPathIgnored(file)
                              let ext = Path.GetExtension(file)
                              where string.Equals(ext, ".cs", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(ext, ".cpp", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(ext, ".c", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(ext, ".cc", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(ext, ".cxx", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(ext, ".c++", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(ext, ".h", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(ext, ".hh", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(ext, ".inl", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(ext, ".hpp", StringComparison.OrdinalIgnoreCase)
                              select file;
            }

            return sourceFiles.ToList();
        }

        private static void CreateSrcSrv(string srcsrvFile, SrcSrvContext srcSrvContext)
        {
            Argument.IsNotNull(nameof(srcSrvContext), srcSrvContext);
            Argument.IsNotNullOrWhitespace(nameof(srcSrvContext) + "." + nameof(srcSrvContext.RawUrl), srcSrvContext.RawUrl);
            Argument.IsNotNullOrWhitespace(nameof(srcSrvContext) + "." + nameof(srcSrvContext.Revision), srcSrvContext.Revision);
            Argument.IsNotNullOrWhitespace(nameof(srcsrvFile), srcsrvFile);

            if (srcSrvContext.VstsData.Count != 0)
            {
                Log.Debug("Writing VSTS specific bytes to srcsrv file because VstsData was not empty.");
                File.WriteAllBytes(srcsrvFile, SrcSrv.CreateVsts(srcSrvContext.Revision, srcSrvContext.Paths, srcSrvContext.VstsData));
            }
            else
            {
                File.WriteAllBytes(srcsrvFile, SrcSrv.Create(srcSrvContext.RawUrl, srcSrvContext.Revision, srcSrvContext.Paths, srcSrvContext.DownloadWithPowershell));
            }
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
                if (next == null)
                {
                    Log.Error($"Unable to find path \"{path}\" on disk.");
                    return path;
                }

                segments[i] = next.Name; // get canonical capitalization
                currentDir = next as DirectoryInfo;
            }

            return Path.Combine(segments);
        }

        private static string ReplaceSlashes(IProvider provider, string relativePathForUrl)
        {
            bool isBackSlashSupported = false;

            // Check if provider is capable of determining whether to use back slashes or forward slashes.
            var backSlashSupport = provider as IBackSlashSupport;
            if (backSlashSupport != null)
            {
                isBackSlashSupported = backSlashSupport.IsBackSlashSupported;
            }

            if (isBackSlashSupported)
            {
                relativePathForUrl = relativePathForUrl.Replace("/", "\\");
            }
            else
            {
                relativePathForUrl = relativePathForUrl.Replace("\\", "/");
            }

            return relativePathForUrl;
        }
    }
}