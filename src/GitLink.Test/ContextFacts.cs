// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Test
{
    using Catel.Test;
    using NUnit.Framework;

    public class ContextFacts
    {
        [TestFixture]
        public class TheDefaultValues
        {
            [TestCase]
            public void SetsRightDefaultValues()
            {
                var context = new Context();

                Assert.AreEqual("Release", context.ConfigurationName);
                Assert.IsFalse(context.IsHelp);
            }
        }

        [TestFixture]
        public class TheValidateContextMethod
        {
            [TestCase]
            public void ThrowsExceptionForMissingSolutionDirectory()
            {
                var context = new Context();

                ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => context.ValidateContext());
            }

            [TestCase]
            public void ThrowsExceptionForMissingConfigurationName()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\GitLink",
                    ConfigurationName = string.Empty
                };

                ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => context.ValidateContext());
            }

            [TestCase]
            public void ThrowsExceptionForMissingTargetUrl()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\GitLink",
                };

                ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => context.ValidateContext());
            }

            [TestCase]
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