// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentParserFacts.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink.Test
{
    using Catel.Test;
    using GitHubLink;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ArgumentParserFacts
    {
        #region Methods

        [TestMethod]
        public void ThrowsExceptionForEmptyParameters()
        {
            ExceptionTester.CallMethodAndExpectException<GitHubLinkException>(() => ArgumentParser.ParseArguments(string.Empty));
        }

        [TestMethod]
        public void CorrectlyParsesSolutionDirectory()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -u http://github.com/GeertvanHorrik/GitHubLink");

            Assert.AreEqual("solutionDirectory", context.SolutionDirectory);
        }

        [TestMethod]
        public void CorrectlyParsesLogFilePath()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -l logFilePath");

            Assert.AreEqual("solutionDirectory", context.SolutionDirectory);
            Assert.AreEqual("logFilePath", context.LogFile);
        }

        [TestMethod]
        public void CorrectlyParsesHelp()
        {
            var context = ArgumentParser.ParseArguments("-h");

            Assert.IsTrue(context.IsHelp);
        }

        [TestMethod]
        public void CorrectlyParsesUrlAndBranchName()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -u http://github.com/GeertvanHorrik/GitHubLink -b somebranch");

            Assert.AreEqual("solutionDirectory", context.SolutionDirectory);
            Assert.AreEqual("http://github.com/GeertvanHorrik/GitHubLink", context.TargetUrl);
            Assert.AreEqual("somebranch", context.TargetBranch);
        }

        [TestMethod]
        public void CorrectlyParsesUrlAndConfiguration()
        {
            var context = ArgumentParser.ParseArguments("solutionDirectory -u http://github.com/GeertvanHorrik/GitHubLink -c someConfiguration");

            Assert.AreEqual("solutionDirectory", context.SolutionDirectory);
            Assert.AreEqual("http://github.com/GeertvanHorrik/GitHubLink", context.TargetUrl);
            Assert.AreEqual("someConfiguration", context.ConfigurationName);
        }

        [TestMethod]
        public void ThrowsExceptionForInvalidNumberOfArguments()
        {
            ExceptionTester.CallMethodAndExpectException<GitHubLinkException>(() => ArgumentParser.ParseArguments("solutionDirectory -l logFilePath extraArg"));
        }

        [TestMethod]
        public void ThrowsExceptionForUnknownArgument()
        {
            ExceptionTester.CallMethodAndExpectException<GitHubLinkException>(() => ArgumentParser.ParseArguments("solutionDirectory -x logFilePath"));
        }

        #endregion
    }
}