// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink.Test.Extensions
{
    using System;
    using GitHubLink;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class StringExtensionsFacts
    {
        [TestClass]
        public class TheGetGitHubCompanyNameMethod
        {
            [TestMethod]
            public void ReturnsValidCompany()
            {
                var company = GitHubLink.StringExtensions.GetGitHubCompanyName("https://github.com/GeertvanHorrik/GitHubLink");

                Assert.AreEqual("GeertvanHorrik", company);
            }
        }

        [TestClass]
        public class TheGetGitHubProjectNameMethod
        {
            [TestMethod]
            public void ReturnsValidProject()
            {
                var project = GitHubLink.StringExtensions.GetGitHubProjectName("https://github.com/GeertvanHorrik/GitHubLink");

                Assert.AreEqual("GitHubLink", project);
            }
        }

        [TestClass]
        public class TheGetGitHubProjectUrlMethod
        {
            [TestMethod]
            public void ReturnsValidUrl()
            {
                var company = GitHubLink.StringExtensions.GetGitHubProjectUrl("https://github.com/GeertvanHorrik/GitHubLink");

                Assert.AreEqual("https://github.com/GeertvanHorrik/GitHubLink", company);
            }
        }

        [TestClass]
        public class TheGetGitHubCompanyUrlMethod
        {
            [TestMethod]
            public void ReturnsValidUrl()
            {
                var company = GitHubLink.StringExtensions.GetGitHubCompanyUrl("https://github.com/GeertvanHorrik/GitHubLink");

                Assert.AreEqual("https://github.com/GeertvanHorrik", company);
            }
        }

        [TestClass]
        public class TheGetGitHubRawUrlMethod
        {
            [TestMethod]
            public void ReturnsValidUrl()
            {
                var company = GitHubLink.StringExtensions.GetGitHubRawUrl("https://github.com/GeertvanHorrik/GitHubLink");

                Assert.AreEqual("https://raw.github.com/GeertvanHorrik/GitHubLink", company);
            }
        }
    }
}