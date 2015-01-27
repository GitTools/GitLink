namespace GitLink
{
    using System;
    using LibGit2Sharp;

    public static class LibGitExtensions
    {
        public static bool IsDetachedHead(this Branch branch)
        {
            return branch.CanonicalName.Equals("(no branch)", StringComparison.OrdinalIgnoreCase);
        }
    }
}