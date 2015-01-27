// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LibGitExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using LibGit2Sharp;

    public static class LibGitExtensions
    {
        public static bool IsDetachedHead(this Branch branch)
        {
            Argument.IsNotNull(() => branch);

            return branch.CanonicalName.Equals("(no branch)", StringComparison.OrdinalIgnoreCase);
        }

        public static IEnumerable<Branch> GetBranchesContainingCommit(this IRepository repository, string commitSha)
        {
            var directBranchHasBeenFound = false;
            foreach (var branch in repository.Branches)
            {
                if (branch.Tip.Sha != commitSha)
                {
                    continue;
                }

                directBranchHasBeenFound = true;
                yield return branch;
            }

            if (directBranchHasBeenFound)
            {
                yield break;
            }

            foreach (var branch in repository.Branches)
            {
                var commits = repository.Commits.QueryBy(new CommitFilter { Since = branch }).Where(c => c.Sha == commitSha);

                if (!commits.Any())
                {
                    continue;
                }

                yield return branch;
            }
        }
    }
}