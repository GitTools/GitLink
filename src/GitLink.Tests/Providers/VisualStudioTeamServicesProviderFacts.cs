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
                [TestCase("https://CatenaLogic.visualstudio.com/_git/main-repo", "main-repo")]
                [TestCase("https://CatenaLogic.visualstudio.com/BigProject/_git/main-repo", "BigProject")]
                [TestCase("https://CatenaLogic.visualstudio.com/DefaultCollection/BigProject/_git/main-repo", "BigProject")]
                public void ReturnsValidProject(string url, string expectedProjectName)
                {
                    var provider = new VisualStudioTeamServicesProvider();
                    provider.Initialize(url);

                    Assert.AreEqual(expectedProjectName, provider.ProjectName);
                }

                [TestCase("https://CatenaLogic.visualstudio.com/_git/main-repo", "CatenaLogic")]
                public void ReturnsValidCompany(string url, string expectedCompanyName)
                {
                    var provider = new VisualStudioTeamServicesProvider();
                    provider.Initialize(url);

                    Assert.AreEqual(expectedCompanyName, provider.CompanyName);
                }

                [TestCase("https://CatenaLogic.visualstudio.com/Project/_git/main-repo", "main-repo")]
                [TestCase("https://CatenaLogic.visualstudio.com/Project/_git/main.repo", "main.repo")]
                [TestCase("https://CatenaLogic.visualstudio.com/DefaultCollection/Project/_git/main.repo", "main.repo")]
                public void ReturnsValidRepositoryName(string url, string expectedProjectUrl)
                {
                    var provider = new VisualStudioTeamServicesProvider();
                    provider.Initialize(url);

                    Assert.AreEqual(expectedProjectUrl, provider.ProjectUrl);
                }

                [TestCase("https://CatenaLogic.visualstudio.com/_git/main-repo", "https://CatenaLogic.visualstudio.com/")]
                [TestCase("https://CatenaLogic.visualstudio.com/DefaultCollection/BigProject/_git/main-repo", "https://CatenaLogic.visualstudio.com/DefaultCollection/")]
                [TestCase("https://CatenaLogic.visualstudio.com/Other.Collection/BigProject/_git/main-repo", "https://CatenaLogic.visualstudio.com/Other.Collection/")]
                public void ReturnsValidCompanyUrl(string url, string expectedCompanyUrl)
                {
                    var provider = new VisualStudioTeamServicesProvider();
                    provider.Initialize(url);

                    Assert.AreEqual(expectedCompanyUrl, provider.CompanyUrl);
                }
            }
        }
    }
}
