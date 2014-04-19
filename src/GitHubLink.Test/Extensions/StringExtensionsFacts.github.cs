// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink.Test.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class StringExtensionsFacts
    {
        [TestClass]
        public class TheGetGitHubCompanyNameMethod
        {
            [TestMethod]
            public void ReturnsValidCompany()
            {
                var company = StringExtensions.GetGitHubCompanyName("https://github.com/GeertvanHorrik/GitHubLink");

                Assert.AreEqual("GeertvanHorrik", company);
            }
        }

        [TestClass]
        public class TheGetGitHubProjectNameMethod
        {
            [TestMethod]
            public void ReturnsValidProject()
            {
                var project = StringExtensions.GetGitHubProjectName("https://github.com/GeertvanHorrik/GitHubLink");

                Assert.AreEqual("GitHubLink", project);
            }
        }

        [TestClass]
        public class TheGetGitHubProjectUrlMethod
        {
            [TestMethod]
            public void ReturnsValidUrl()
            {
                var company = StringExtensions.GetGitHubProjectUrl("https://github.com/GeertvanHorrik/GitHubLink");

                Assert.AreEqual("https://github.com/GeertvanHorrik/GitHubLink", company);
            }

            [TestMethod]
            public void ReturnsValidUrlWhenGitIsAppended()
            {
                var company = StringExtensions.GetGitHubProjectUrl("https://github.com/GeertvanHorrik/GitHubLink.git");

                Assert.AreEqual("https://github.com/GeertvanHorrik/GitHubLink", company);
            }
        }

        [TestClass]
        public class TheGetGitHubCompanyUrlMethod
        {
            [TestMethod]
            public void ReturnsValidUrl()
            {
                var company = StringExtensions.GetGitHubCompanyUrl("https://github.com/GeertvanHorrik/GitHubLink");

                Assert.AreEqual("https://github.com/GeertvanHorrik", company);
            }
        }

        [TestClass]
        public class TheGetGitHubRawUrlMethod
        {
            [TestMethod]
            public void ReturnsValidUrl()
            {
                var company = StringExtensions.GetGitHubRawUrl("https://github.com/GeertvanHorrik/GitHubLink");

                Assert.AreEqual("https://raw.github.com/GeertvanHorrik/GitHubLink", company);
            }
        }
    }
}