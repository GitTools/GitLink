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
    using Catel.Logging;
    using GitLink.Providers;

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
                Log.ErrorAndThrowException<GitLinkException>("Invalid number of arguments");
            }

            var firstArgument = commandLineArguments.First();
            if (IsHelp(firstArgument))
            {
                context.IsHelp = true;
                return context;
            }

            if (commandLineArguments.Count < 3)
            {
                Log.ErrorAndThrowException<GitLinkException>("Invalid number of arguments");
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

                // After this point, all arguments should have a value
                index++;
                var value = namedArguments[index];

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

                Log.ErrorAndThrowException<GitLinkException>("Could not parse command line parameter '{0}'.", name);
            }

            if (!string.IsNullOrEmpty(context.TargetUrl))
            {
                context.Provider = providerManager.GetProvider(context.TargetUrl);
            }

            return context;
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