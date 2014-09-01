// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Test
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

                ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => context.ValidateContext());
            }

            [TestMethod]
            public void ThrowsExceptionForMissingConfigurationName()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\GitLink",
                    ConfigurationName = string.Empty
                };

                ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => context.ValidateContext());
            }

            [TestMethod]
            public void ThrowsExceptionForMissingTargetUrl()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\GitLink",
                };

                ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => context.ValidateContext());
            }

            [TestMethod]
            public void SucceedsForValidContext()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\GitLink",
                    TargetUrl = "https://github.com/CatenaLogic/GitLink"
                };

                // should not throw
                context.ValidateContext();
            }
        }
    }
}