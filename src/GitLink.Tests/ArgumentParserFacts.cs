// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentParserFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Tests
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
        public void CorrectlyParsesSolutionFile()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -u http://github.com/CatenaLogic/GitLink -f someSolution");

            Assert.AreEqual("someSolution", context.SolutionFile);
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
        public void CorrectlyParsesUrlAndConfigurationAndPlatform()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -u http://github.com/CatenaLogic/GitLink -c someConfiguration -p \"Any CPU\"");

            Assert.AreEqual("solutionDirectory", context.SolutionDirectory);
            Assert.AreEqual("http://github.com/CatenaLogic/GitLink", context.TargetUrl);
            Assert.AreEqual("someConfiguration", context.ConfigurationName);
            Assert.AreEqual("Any CPU", context.PlatformName);
        }

        [TestCase]
        public void CorrectlyParsesUrlAndConfigurationWithDebug()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -u http://github.com/CatenaLogic/GitLink -debug -c someConfiguration");

            Assert.AreEqual("solutionDirectory", context.SolutionDirectory);
            Assert.AreEqual("http://github.com/CatenaLogic/GitLink", context.TargetUrl);
            Assert.AreEqual("someConfiguration", context.ConfigurationName);
            Assert.IsTrue(context.IsDebug);
        }

        [TestCase]
        public void CorrectlyParsesIgnoredProjects()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -u http://github.com/CatenaLogic/GitLink -debug -c someConfiguration -ignore test1,test2");

            Assert.AreEqual("solutionDirectory", context.SolutionDirectory);
            Assert.AreEqual("http://github.com/CatenaLogic/GitLink", context.TargetUrl);
            Assert.AreEqual("someConfiguration", context.ConfigurationName);
            Assert.IsTrue(context.IsDebug);

            Assert.AreEqual(2, context.IgnoredProjects.Count);
            Assert.AreEqual("test1", context.IgnoredProjects[0]);
            Assert.AreEqual("test2", context.IgnoredProjects[1]);
        }

        [TestCase]
        public void ThrowsExceptionForUnknownArgument()
        {
            ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => ArgumentParser.ParseArguments("solutionDirectory -x logFilePath"));
        }
    }
}