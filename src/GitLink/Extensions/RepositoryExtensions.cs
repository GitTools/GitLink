// <copyright file="RepositoryExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>

namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IO;
    using LibGit2Sharp;

    internal static class RepositoryExtensions
    {
        internal static string GetRepoNormalizedPath(this Repository repository, string path)
        {
            Argument.IsNotNull(nameof(repository), repository);
            Argument.IsNotNullOrEmpty(nameof(path), path);

            string relativePath = Path.GetRelativePath(path, repository.Info.WorkingDirectory);
            var repoFile = repository.Index.FirstOrDefault(e => string.Equals(e.Path, relativePath, StringComparison.OrdinalIgnoreCase));
            return repoFile?.Path;
        }
    }
}
