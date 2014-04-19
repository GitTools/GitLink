// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    using System;
    using Catel.Logging;
    using GitHubLink.Logging;

    internal class Program
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Methods

        private static int Main(string[] args)
        {
#if DEBUG
            LogManager.AddDebugListener(true);
#endif

            var consoleLogListener = new OutputLogListener();

            LogManager.AddListener(consoleLogListener);

            try
            {
                HelpWriter.WriteAppHeader(s => Log.Write(LogEvent.Info, s));

                Log.Info("Arguments: {0}", string.Join(" ", args));
                Log.Info(string.Empty);

                var context = ArgumentParser.ParseArguments(args);
                if (context.IsHelp)
                {
                    HelpWriter.WriteHelp(s => Log.Write(LogEvent.Info, s));
                    
                    WaitForKeyPress();

                    return 0;
                }

                var result = Linker.Link(context);

#if DEBUG
                WaitForKeyPress();
#endif

                return result;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion

        private static void WaitForKeyPress()
        {
            Log.Info(string.Empty);
            Log.Info("Press any key to continue");
            Console.ReadKey();
        }
    }
}