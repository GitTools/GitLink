// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitHubLink.Test
{
    using Catel.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class ContextFacts
    {
        [TestClass]
        public class TheDefaultValues
        {
            [TestMethod]
            public void SetsRightDefaultValues()
            {
                var context = new Context();

                Assert.AreEqual("Release", context.ConfigurationName);
                Assert.IsFalse(context.IsHelp);
            }
        }

        [TestClass]
        public class TheValidateContextMethod
        {
            [TestMethod]
            public void ThrowsExceptionForMissingSolutionDirectory()
            {
                var context = new Context();

                ExceptionTester.CallMethodAndExpectException<GitHubLinkException>(() => context.ValidateContext());
            }

            [TestMethod]
            public void ThrowsExceptionForMissingConfigurationName()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\githublink",
                    ConfigurationName = string.Empty
                };

                ExceptionTester.CallMethodAndExpectException<GitHubLinkException>(() => context.ValidateContext());
            }

            [TestMethod]
            public void ThrowsExceptionForMissingTargetUrl()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\githublink",
                };

                ExceptionTester.CallMethodAndExpectException<GitHubLinkException>(() => context.ValidateContext());
            }

            [TestMethod]
            public void SucceedsForValidContext()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\githublink",
                    TargetUrl = "https://github.com/geertvanhorrik/githublink"
                };

                // should not throw
                context.ValidateContext();
            }
        }
    }
}