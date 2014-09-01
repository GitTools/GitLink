// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsFacts.github.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Test.Extensions
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
                var company = StringExtensions.GetGitHubCompanyName("https://github.com/CatenaLogic/GitLink");

                Assert.AreEqual("CatenaLogic", company);
            }
        }

        [TestClass]
        public class TheGetGitHubCompanyUrlMethod
        {
            [TestMethod]
            public void ReturnsValidUrl()
            {
                var company = StringExtensions.GetGitHubCompanyUrl("https://github.com/CatenaLogic/GitLink");

                Assert.AreEqual("https://github.com/CatenaLogic", company);
            }
        }

        [TestClass]
        public class TheGetGitHubProjectNameMethod
        {
            [TestMethod]
            public void ReturnsValidProject()
            {
                var project = StringExtensions.GetGitHubProjectName("https://github.com/CatenaLogic/GitLink");

                Assert.AreEqual("GitLink", project);
            }
        }

        [TestClass]
        public class TheGetGitHubProjectUrlMethod
        {
            [TestMethod]
            public void ReturnsValidUrl()
            {
                var company = StringExtensions.GetGitHubProjectUrl("https://github.com/CatenaLogic/GitLink");

                Assert.AreEqual("https://github.com/CatenaLogic/GitLink", company);
            }

            [TestMethod]
            public void ReturnsValidUrlWhenGitIsAppended()
            {
                var company = StringExtensions.GetGitHubProjectUrl("https://github.com/CatenaLogic/GitLink.git");

                Assert.AreEqual("https://github.com/CatenaLogic/GitLink", company);
            }
        }

        [TestClass]
        public class TheGetGitHubRawUrlMethod
        {
            [TestMethod]
            public void ReturnsValidUrl()
            {
                var company = StringExtensions.GetGitHubRawUrl("https://github.com/CatenaLogic/GitLink");

                Assert.AreEqual("https://raw.github.com/CatenaLogic/GitLink", company);
            }
        }
    }
}