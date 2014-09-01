// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentParserFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Test
{
    using Catel.Test;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentParserFacts
    {
        [TestCase]
        public void ThrowsExceptionForEmptyParameters()
        {
            ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => ArgumentParser.ParseArguments(string.Empty));
        }

        [TestCase]
        public void CorrectlyParsesSolutionDirectory()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -u http://github.com/CatenaLogic/GitLink");

            Assert.AreEqual("solutionDirectory", context.SolutionDirectory);
        }

        [TestCase]
        public void CorrectlyParsesLogFilePath()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -l logFilePath");

            Assert.AreEqual("solutionDirectory", context.SolutionDirectory);
            Assert.AreEqual("logFilePath", context.LogFile);
        }

        [TestCase]
        public void CorrectlyParsesHelp()
        {
            var context = ArgumentParser.ParseArguments("-h");

            Assert.IsTrue(context.IsHelp);
        }

        [TestCase]
        public void CorrectlyParsesUrlAndBranchName()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -u http://github.com/CatenaLogic/GitLink -b somebranch");

            Assert.AreEqual("solutionDirectory", context.SolutionDirectory);
            Assert.AreEqual("http://github.com/CatenaLogic/GitLink", context.TargetUrl);
            Assert.AreEqual("somebranch", context.TargetBranch);
        }

        [TestCase]
        public void CorrectlyParsesUrlAndConfiguration()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -u http://github.com/CatenaLogic/GitLink -c someConfiguration");

            Assert.AreEqual("solutionDirectory", context.SolutionDirectory);
            Assert.AreEqual("http://github.com/CatenaLogic/GitLink", context.TargetUrl);
            Assert.AreEqual("someConfiguration", context.ConfigurationName);
        }

        [TestCase]
        public void ThrowsExceptionForInvalidNumberOfArguments()
        {
            ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => ArgumentParser.ParseArguments("solutionDirectory -l logFilePath extraArg"));
        }

        [TestCase]
        public void ThrowsExceptionForUnknownArgument()
        {
            ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => ArgumentParser.ParseArguments("solutionDirectory -x logFilePath"));
        }
    }
}