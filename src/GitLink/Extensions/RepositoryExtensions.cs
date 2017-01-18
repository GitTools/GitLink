// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryExtensions.cs" company="Andrew Arnott">
//   Copyright (c) 2016 Andrew Arnott. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink
{
    using System;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using LibGit2Sharp;

    internal static class RepositoryExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly char[] PathSeparators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        internal static string GetNormalizedPath(this Repository repository, string path)
        {
            Argument.IsNotNull(nameof(repository), repository);
            Argument.IsNotNullOrEmpty(nameof(path), path);

            string relativePath = GetRelativePath(path, repository.Info.WorkingDirectory);
            string[] relativePathSegments = relativePath.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            var tree = repository.Commits.FirstOrDefault()?.Tree;
            if (tree == null)
            {
                // Throw an exception that will cause our caller to fallback to poor man's normalization.
                throw new RepositoryNotFoundException();
            }

            for (int i = 0; i < relativePathSegments.Length; i++)
            {
                string segment = relativePathSegments[i];
                TreeEntry entry = tree[segment] ?? tree.FirstOrDefault(te => string.Equals(te.Name, segment, StringComparison.OrdinalIgnoreCase));

                if (entry == null)
                {
                    Log.Warning("Unable to find file in git.");
                    return path;
                }

                if (entry.TargetType == TreeEntryTargetType.Tree)
                {
                    tree = (Tree)entry.Target;
                }
                else
                {
                    if (i < relativePathSegments.Length - 1)
                    {
                        Log.Error("Found a file where we expected to find a directory.");
                        return path;
                    }
                }

                relativePathSegments[i] = entry.Name;
            }

            return string.Join("/", relativePathSegments);
        }

        private static string GetRelativePath(string target, string relativeTo)
        {
            if (!Path.IsPathRooted(target))
            {
                // It is already relative.
                return target;
            }

            target = string.Join(Path.DirectorySeparatorChar.ToString(), target.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries));

            Uri baseUri = new Uri(relativeTo, UriKind.Absolute);
            Uri targetUri = new Uri(target, UriKind.Absolute);
            return baseUri.MakeRelativeUri(targetUri).ToString();
        }
    }
}
