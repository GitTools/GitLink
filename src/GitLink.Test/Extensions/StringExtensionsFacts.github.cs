// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsFacts.github.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Test.Extensions
{
    using NUnit.Framework;

    public class StringExtensionsFacts
    {
        [TestFixture]
        public class TheGetGitHubCompanyNameMethod
        {
            [TestCase]
            public void ReturnsValidCompany()
            {
                var company = StringExtensions.GetGitHubCompanyName("https://github.com/CatenaLogic/GitLink");

                Assert.AreEqual("CatenaLogic", company);
            }
        }

        [TestFixture]
        public class TheGetGitHubCompanyUrlMethod
        {
            [TestCase]
            public void ReturnsValidUrl()
            {
                var company = StringExtensions.GetGitHubCompanyUrl("https://github.com/CatenaLogic/GitLink");

                Assert.AreEqual("https://github.com/CatenaLogic", company);
            }
        }

        [TestFixture]
        public class TheGetGitHubProjectNameMethod
        {
            [TestCase]
            public void ReturnsValidProject()
            {
                var project = StringExtensions.GetGitHubProjectName("https://github.com/CatenaLogic/GitLink");

                Assert.AreEqual("GitLink", project);
            }
        }

        [TestFixture]
        public class TheGetGitHubProjectUrlMethod
        {
            [TestCase]
            public void ReturnsValidUrl()
            {
                var company = StringExtensions.GetGitHubProjectUrl("https://github.com/CatenaLogic/GitLink");

                Assert.AreEqual("https://github.com/CatenaLogic/GitLink", company);
            }

            [TestCase]
            public void ReturnsValidUrlWhenGitIsAppended()
            {
                var company = StringExtensions.GetGitHubProjectUrl("https://github.com/CatenaLogic/GitLink.git");

                Assert.AreEqual("https://github.com/CatenaLogic/GitLink", company);
            }
        }

        [TestFixture]
        public class TheGetGitHubRawUrlMethod
        {
            [TestCase]
            public void ReturnsValidUrl()
            {
                var company = StringExtensions.GetGitHubRawUrl("https://github.com/CatenaLogic/GitLink");

                Assert.AreEqual("https://raw.github.com/CatenaLogic/GitLink", company);
            }
        }
    }
}