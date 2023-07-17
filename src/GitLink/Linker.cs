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
    using System.Web;
    using Catel;
    using Catel.Logging;
    using GitTools;
    using GitTools.Git;
    using LibGit2Sharp;
    using Pdb;
    using Providers;

    /// <summary>
    /// Class Linker.
    /// </summary>
    public static class Linker
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly string FilenamePlaceholder = Uri.EscapeUriString("{filename}");
        private static readonly string UrlEncodedFileNamePlaceHolder = Uri.EscapeUriString("{urlencoded_filename}");
        private static readonly string RevisionPlaceholder = Uri.EscapeUriString("{revision}");
        private static readonly string PdbStrExePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "pdbstr.exe");
        private static readonly string SrcToolExePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "srctool.exe");
        private static readonly string[] ExtensionsToIgnore = new string[] { ".g.cs" };
        private static readonly HashSet<string> SourceExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".cs", ".cpp", ".c", ".cc", ".cxx", ".c++", ".h", ".hh", ".inl", ".hpp" };
        private static IReadOnlyList<string> _sourceFilesList = null;

        public static bool LinkDirectory(string pdbFolderPath, LinkOptions options = default(LinkOptions))
        {
            return Directory.EnumerateFiles(pdbFolderPath, "*.pdb", SearchOption.AllDirectories).All(filePath => Link(filePath, options));
        }

        public static bool Link(string pdbPath, LinkOptions options = default(LinkOptions))
        {
            Argument.IsNotNullOrEmpty(() => pdbPath);

            var projectSrcSrvFile = pdbPath + ".srcsrv";
            string repositoryDirectory = null;
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
                    repositoryDirectory = GetRepositoryFromFiles(new[] { pdbPath });
                    if (repositoryDirectory == null)
                    {
                        Log.Error($"Couldn't auto detect git repo from PDB: {pdbPath}. Please use -baseDir to manually set it.");
                        return false;
                    }
                }

                if (_sourceFilesList == null)
                {
                    _sourceFilesList = GetSourceFilesFromDepot(repositoryDirectory);
                }
            }
            else
            {
                if (options.IndexWithSrcTool)
                {
                    _sourceFilesList = SrcToolHelper.GetSourceFiles(SrcToolExePath, pdbPath);
                }
                else
                {
                    _sourceFilesList = GetSourceFilesFromPdb(pdbPath, !options.SkipVerify);
                }

                if (!_sourceFilesList.Any())
                {
                    Log.Error($"No source files were found in the PDB: {pdbPath}. If your PDB is native you could use the -a or -t option.");
                    return false;
                }

                if (repositoryDirectory == null)
                {
                    repositoryDirectory = GetRepositoryFromFiles(new[] { pdbPath }.Union(_sourceFilesList));
                    if (repositoryDirectory == null)
                    {
                        Log.Error("Couldn't auto detect git repo. Please use -baseDir to manually set it.");
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
                                             select (remote.Name, Provider: p);
                    provider = candidateProviders.FirstOrDefault(c => c.Name == "origin").Provider
                        ?? candidateProviders.FirstOrDefault().Provider;
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
                    var repo = repository.Value;

                    var files = string.IsNullOrEmpty(options.IntermediateOutputPath) ?
                        _sourceFilesList : _sourceFilesList.Where(f => !f.StartsWithIgnoreCase(options.IntermediateOutputPath));

                    repoSourceFiles = files.ToDictionary(e => e, e => repo.GetNormalizedPath(e));
                }
                catch (RepositoryNotFoundException)
                {
                    // Normalize using file system since we can't find the git repo.
                    Log.Warning($"Unable to find git repo at \"{options.GitWorkingDirectory}\". Using file system to find canonical capitalization of file paths.");
                    repoSourceFiles = _sourceFilesList.ToDictionary(e => e, e => GetNormalizedPath(e, workingDirectory));
                }

                if (!options.SkipVerify)
                {
                    // Filter to only files which are tracked by git
                    var commit = repository.Value.Lookup<Commit>(commitId);
                    if (commit != null)
                    {
                        var trackedRepoSourceFiles = repoSourceFiles.Where(file => (commit[file.Value] != null)).ToDictionary(file => file.Key, file => file.Value);

                        var untrackedSourceFiles = repoSourceFiles.Keys.Except(trackedRepoSourceFiles.Keys);

                        foreach (var untrackedFile in untrackedSourceFiles)
                        {
                            Log.Warning($"Untracked file \"{untrackedFile}\" will not be indexed");
                        }

                        repoSourceFiles = trackedRepoSourceFiles;
                    }
                }

                string rawUrl = provider.RawGitUrl;
                if (rawUrl.Contains(RevisionPlaceholder) || rawUrl.Contains(FilenamePlaceholder) || rawUrl.Contains(UrlEncodedFileNamePlaceHolder))
                {
                    if (!rawUrl.Contains(RevisionPlaceholder) || !(rawUrl.Contains(FilenamePlaceholder) || rawUrl.Contains(UrlEncodedFileNamePlaceHolder)))
                    {
                        Log.Error("Supplied custom URL pattern must contain both a revision and a filename placeholder.");
                        return false;
                    }

                    rawUrl = rawUrl
                        .Replace(RevisionPlaceholder, "{0}")
                        .Replace(FilenamePlaceholder, "%var2%")
                        .Replace(UrlEncodedFileNamePlaceHolder, "%var2%");
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
                    var ignore = false;

                    foreach (var extensionToIgnore in ExtensionsToIgnore)
                    {
                        if (sourceFile.Key.EndsWithIgnoreCase(extensionToIgnore) || sourceFile.Value.EndsWithIgnoreCase(extensionToIgnore))
                        {
                            ignore = true;
                            break;
                        }
                    }

                    if (ignore)
                    {
                        continue;
                    }

                    // Skip files that aren't tracked by source control.
                    if (sourceFile.Value != null)
                    {
                        var relativePathForUrl = ReplaceSlashes(provider, sourceFile.Value);
                        if (provider.RawGitUrl.Contains(UrlEncodedFileNamePlaceHolder))
                        {
                            relativePathForUrl = HttpUtility.UrlEncode(relativePathForUrl);
                        }

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
                Log.Error($"Unable to find git repo at \"{repositoryDirectory}\".");
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
            Log.Info($"Remote git source information for {indexedFilesCount}/{_sourceFilesList.Count} files written to pdb: \"{pdbPath}\"");

            return true;
        }

        private static string GetRepositoryFromFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var repositoryDirectory = GitDirFinder.TreeWalkForGitDir(Path.GetDirectoryName(file));
                if (repositoryDirectory != null)
                {
                    Log.Debug($"git repo detected: {repositoryDirectory}");
                    return repositoryDirectory;
                }
            }

            return null;
        }

        private static List<string> GetSourceFilesFromPdb(string pdbPath, bool verifyFiles)
        {
            using (var pdb = new PdbFile(pdbPath))
            {
                var sources = pdb.GetFilesAndChecksums().Keys.ToList();

                if (verifyFiles)
                {
                    Log.Debug("Verifying pdb files");

                    foreach (var file in pdb.FindMissingOrChangedSourceFiles())
                    {
                        if (File.Exists(file))
                        {
                            Log.Warning($"File \"{file}\" changed since the PDB was compiled.");
                        }
                        else
                        {
                            Log.Warning($"File \"{file}\" missing since the PDB was compiled.");
                        }
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
                              where ValidExtension(file)
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

        public static Boolean ValidExtension(string sourceFile)
        {
            var ext = Path.GetExtension(sourceFile);

            return SourceExtensions.Contains(ext);
        }
    }
}
