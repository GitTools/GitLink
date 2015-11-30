// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentParser.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Collections;
    using Catel.Logging;
    using GitLink.Providers;
    using GitTools;
    using GitTools.Git;
    using LibGit2Sharp;

    public static class ArgumentParser
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static Context ParseArguments(string commandLineArguments)
        {
            return ParseArguments(commandLineArguments.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                new ProviderManager());
        }

        public static Context ParseArguments(params string[] commandLineArguments)
        {
            return ParseArguments(commandLineArguments.ToList(), new ProviderManager());
        }

        public static Context ParseArguments(List<string> commandLineArguments, IProviderManager providerManager)
        {
            var context = new Context(providerManager);

            if (commandLineArguments.Count == 0)
            {
                context.IsHelp = true;
                return context;
            }

            var firstArgument = commandLineArguments.First();
            if (IsHelp(firstArgument))
            {
                context.IsHelp = true;
                return context;
            }

            if (commandLineArguments.Count < 3 && commandLineArguments.Count != 1)
            {
                throw Log.ErrorAndCreateException<GitLinkException>("Invalid number of arguments");
            }

            context.SolutionDirectory = firstArgument;

            var namedArguments = commandLineArguments.Skip(1).ToList();
            for (var index = 0; index < namedArguments.Count; index++)
            {
                var name = namedArguments[index];

                // First check everything without values
                if (IsSwitch("debug", name))
                {
                    context.IsDebug = true;
                    continue;
                }

                if (IsSwitch("errorsaswarnings", name))
                {
                    context.ErrorsAsWarnings = true;
                    continue;
                }

                if (IsSwitch("skipverify", name))
                {
                    context.SkipVerify = true;
                    continue;
                }

                if (IsSwitch("powershell", name))
                {
                    context.DownloadWithPowershell = true;
                    continue;
                }

                // After this point, all arguments should have a value
                index++;
                var valueInfo = GetValue(namedArguments, index);
                var value = valueInfo.Key;
                index = index + (valueInfo.Value - 1);

                if (IsSwitch("l", name))
                {
                    context.LogFile = value;
                    continue;
                }

                if (IsSwitch("c", name))
                {
                    context.ConfigurationName = value;
                    continue;
                }

                if (IsSwitch("p", name))
                {
                    context.PlatformName = value;
                    continue;
                }

                if (IsSwitch("u", name))
                {
                    context.TargetUrl = value;
                    continue;
                }

                if (IsSwitch("b", name))
                {
                    context.TargetBranch = value;
                    continue;
                }

                if (IsSwitch("s", name))
                {
                    context.ShaHash = value;
                    continue;
                }

                if (IsSwitch("f", name))
                {
                    context.SolutionFile = value;
                    continue;
                }

                if (IsSwitch("d", name))
                {
                    context.PdbFilesDirectory = value;
                    continue;
                }

                if (IsSwitch("ignore", name))
                {
                    context.IgnoredProjects.AddRange(value.Split(new []{ ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                    continue;
                }

                throw Log.ErrorAndCreateException<GitLinkException>("Could not parse command line parameter '{0}'.", name);
            }

            if (string.IsNullOrEmpty(context.TargetUrl))
            {
                Log.Info("No target url was specified, trying to determine the target url automatically");

                var gitDir = GitDirFinder.TreeWalkForGitDir(context.SolutionDirectory);
                if (gitDir != null)
                {
                    using (var repo = RepositoryLoader.GetRepo(gitDir))
                    {
                        var currentBranch = repo.Head;

                        if (string.IsNullOrEmpty(context.ShaHash))
                        {
                            context.ShaHash = currentBranch.Tip.Sha;
                        }

                        if (currentBranch.Remote == null || currentBranch.IsDetachedHead())
                        {
                            currentBranch = repo.GetBranchesContainingCommit(context.ShaHash).FirstOrDefault(b => b.Remote != null);
                        }

                        if (currentBranch != null && currentBranch.Remote != null)
                        {
                            var url = currentBranch.Remote.Url;
                            if (url.StartsWith("https://"))
                            {
                                context.TargetUrl = url.OptimizeUrl();

                                Log.Info("Automatically determine target url '{0}'", context.TargetUrl);
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(context.TargetUrl))
            {
                context.Provider = providerManager.GetProvider(context.TargetUrl);
            }

            return context;
        }

        private static KeyValuePair<string, int> GetValue(List<string> arguments, int index)
        {
            var totalCounter = 1;

            var value = arguments[index];

            while (value.StartsWith("\""))
            {
                if (value.EndsWith("\""))
                {
                    break;
                }

                index++;
                value += " " + arguments[index];

                totalCounter++;
            }

            value = value.Trim('\"');

            return new KeyValuePair<string, int>(value, totalCounter);
        }

        private static bool IsSwitch(string switchName, string value)
        {
            if (value.StartsWith("-"))
            {
                value = value.Remove(0, 1);
            }

            if (value.StartsWith("/"))
            {
                value = value.Remove(0, 1);
            }

            return (string.Equals(switchName, value));
        }

        private static bool IsHelp(string singleArgument)
        {
            return (singleArgument == "?") ||
                   IsSwitch("h", singleArgument) ||
                   IsSwitch("help", singleArgument) ||
                   IsSwitch("?", singleArgument);
        }
    }
}