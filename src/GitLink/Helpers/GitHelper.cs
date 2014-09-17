// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitHelper.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Linq;
    using Catel.Logging;
    using LibGit2Sharp;

    public static class GitHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static void NormalizeGitDirectory(string gitDirectory)
        {
            using (var repo = new Repository(gitDirectory))
            {
                var remote = EnsureOnlyOneRemoteIsDefined(repo);

                Log.Info("Fetching from remote '{0}' using the following refspecs: {1}.",
                    remote.Name, string.Join(", ", remote.FetchRefSpecs.Select(r => r.Specification)));

                var fetchOptions = new FetchOptions();
                repo.Network.Fetch(remote, fetchOptions);

                CreateMissingLocalBranchesFromRemoteTrackingOnes(repo, remote.Name);

                if (!repo.Info.IsHeadDetached)
                {
                    Log.Info("HEAD points at branch '{0}'.", repo.Refs.Head.TargetIdentifier);
                    return;
                }

                Log.Info("HEAD is detached and points at commit '{0}'.", repo.Refs.Head.TargetIdentifier);

                CreateFakeBranchPointingAtThePullRequestTip(repo);
            }
        }

        private static void CreateFakeBranchPointingAtThePullRequestTip(Repository repo)
        {
            var remote = repo.Network.Remotes.Single();
            var remoteTips = repo.Network.ListReferences(remote);

            var headTipSha = repo.Head.Tip.Sha;

            var refs = remoteTips.Where(r => r.TargetIdentifier == headTipSha).ToList();

            if (refs.Count == 0)
            {
                Log.ErrorAndThrowException<GitLinkException>("Couldn't find any remote tips from remote '{0}' pointing at the commit '{1}'.", remote.Url, headTipSha);
            }

            if (refs.Count > 1)
            {
                var names = string.Join(", ", refs.Select(r => r.CanonicalName));
                Log.ErrorAndThrowException<GitLinkException>("Found more than one remote tip from remote '{0}' pointing at the commit '{1}'. Unable to determine which one to use ({2}).", remote.Url, headTipSha, names);
            }

            var canonicalName = refs[0].CanonicalName;
            Log.Info("Found remote tip '{0}' pointing at the commit '{1}'.", canonicalName, headTipSha);

            if (!canonicalName.StartsWith("refs/pull/"))
            {
                Log.ErrorAndThrowException<Exception>("Remote tip '{0}' from remote '{1}' doesn't look like a valid pull request.", canonicalName, remote.Url);
            }

            var fakeBranchName = canonicalName.Replace("refs/pull/", "refs/heads/pull/");

            Log.Info("Creating fake local branch '{0}'.", fakeBranchName);
            repo.Refs.Add(fakeBranchName, new ObjectId(headTipSha));

            Log.Info("Checking local branch '{0}' out.", fakeBranchName);
            repo.Checkout(fakeBranchName);
        }

        private static void CreateMissingLocalBranchesFromRemoteTrackingOnes(Repository repo, string remoteName)
        {
            var prefix = string.Format("refs/remotes/{0}/", remoteName);

            foreach (var remoteTrackingReference in repo.Refs.FromGlob(prefix + "*"))
            {
                var localCanonicalName = "refs/heads/" + remoteTrackingReference.CanonicalName.Substring(prefix.Length);
                if (repo.Refs.Any(x => x.CanonicalName == localCanonicalName))
                {
                    Log.Info("Skipping local branch creation since it already exists '{0}'.", remoteTrackingReference.CanonicalName);
                    continue;
                }

                Log.Info("Creating local branch from remote tracking '{0}'.", remoteTrackingReference.CanonicalName);

                var symbolicReference = remoteTrackingReference as SymbolicReference;
                if (symbolicReference == null)
                {
                    repo.Refs.Add(localCanonicalName, new ObjectId(remoteTrackingReference.TargetIdentifier), true);
                }
                else
                {
                    repo.Refs.Add(localCanonicalName, new ObjectId(symbolicReference.ResolveToDirectReference().TargetIdentifier), true);
                }
            }
        }

        private static Remote EnsureOnlyOneRemoteIsDefined(IRepository repo)
        {
            var remotes = repo.Network.Remotes;
            var howMany = remotes.Count();

            if (howMany == 1)
            {
                var remote = remotes.Single();
                Log.Info("One remote found ({0} -> '{1}').", remote.Name, remote.Url);
                return remote;
            }

            Log.ErrorAndThrowException<GitLinkException>("{0} remote(s) have been detected. When being run on a TeamCity agent, the Git repository is expected to bear one (and no more than one) remote.", howMany);

            return null;
        }
    }
}