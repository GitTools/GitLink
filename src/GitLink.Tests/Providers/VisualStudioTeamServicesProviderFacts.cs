// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitHubProviderFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Tests.Providers
{
    using GitLink.Providers;
    using NUnit.Framework;

    public class VisualStudioTeamServicesProviderFacts
    {
        [TestFixture]
        public class TheVisualStudioTeamServicesProviderInitialization
        {
            [TestCase]
            public void ReturnsValidInitialization()
            {
                var provider = new VisualStudioTeamServicesProvider();
                var valid = provider.Initialize("https://my-account.visualstudio.com/_git/main-repo");

                Assert.IsTrue(valid);
            }

            [TestCase]
            public void ReturnsInValidInitialization()
            {
                var provider = new VisualStudioTeamServicesProvider();
                var valid = provider.Initialize("https://github.com/CatenaLogic/GitLink");

                Assert.IsFalse(valid);
            }

            [TestFixture]
            public class TheVisualStudioTeamServicesProviderProperties
            {
                [TestCase]
                public void ReturnsValidCompany()
                {
                    var provider = new VisualStudioTeamServicesProvider();
                    provider.Initialize("https://CatenaLogic.visualstudio.com/_git/main-repo");

                    Assert.AreEqual("CatenaLogic", provider.CompanyName);
                }

                [TestCase]
                public void ReturnsValidCompanyUrl()
                {
                    var provider = new VisualStudioTeamServicesProvider();
                    provider.Initialize("https://CatenaLogic.visualstudio.com/_git/main-repo");

                    Assert.AreEqual("https://CatenaLogic.visualstudio.com/", provider.CompanyUrl);
                }

                [TestCase]
                public void ReturnsValidProject()
                {
                    var provider = new VisualStudioTeamServicesProvider();
                    provider.Initialize("https://CatenaLogic.visualstudio.com/_git/main-repo");

                    Assert.AreEqual("main-repo", provider.ProjectName);
                }

                [TestCase]
                public void ReturnsValidProject2()
                {
                    var provider = new VisualStudioTeamServicesProvider();
                    provider.Initialize("https://CatenaLogic.visualstudio.com/BigProject/_git/main-repo");

                    Assert.AreEqual("BigProject", provider.ProjectName);
                }

                [TestCase]
                public void ReturnsValidRepositoryName()
                {
                    var provider = new VisualStudioTeamServicesProvider();
                    provider.Initialize("https://CatenaLogic.visualstudio.com/Project/_git/main-repo");

                    Assert.AreEqual("main-repo", provider.ProjectUrl);
                }

                [TestCase]
                public void ReturnsValidRepositoryNameWhenContainsPeriod()
                {
                    var provider = new VisualStudioTeamServicesProvider();
                    provider.Initialize("https://CatenaLogic.visualstudio.com/Big.Project/_git/main.repo");

                    Assert.AreEqual("main.repo", provider.ProjectUrl);
                }

            }
        }
    }
}
