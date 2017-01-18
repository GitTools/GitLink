// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomRawUrlProviderFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Tests.Providers
{
    using GitLink.Providers;
    using NUnit.Framework;

    public class CustomRawUrlProviderFacts
    {
        [TestFixture]
        public class TheInitialization
        {
            [TestCase("http://example.com/repo", true)]
            [TestCase("https://example.com/repo", true)]
            [TestCase("https://example.com/repo/", true)]
            [TestCase("gopher://example.com/repo", false)]
            public void CorrectlyValidatesForUrls(string url, bool expectedValue)
            {
                var provider = new CustomRawUrlProvider();
                var valid = provider.Initialize(url);

                Assert.AreEqual(expectedValue, valid);
            }
        }

        [TestFixture]
        public class TheGitHubProviderProperties
        {
            [TestCase]
            public void ReturnsNullCompany()
            {
                var provider = new CustomRawUrlProvider();
                provider.Initialize("http://example.com/repo");

                Assert.IsNull(provider.CompanyName);
            }

            [TestCase]
            public void ReturnsNullCompanyUrl()
            {
                var provider = new CustomRawUrlProvider();
                provider.Initialize("http://example.com/repo");

                Assert.IsNull(provider.CompanyUrl);
            }

            [TestCase]
            public void ReturnsNullProject()
            {
                var provider = new CustomRawUrlProvider();
                provider.Initialize("http://example.com/repo");

                Assert.IsNull(provider.ProjectName);
            }

            [TestCase]
            public void ReturnsNullProjectUrl()
            {
                var provider = new CustomRawUrlProvider();
                provider.Initialize("http://example.com/repo");

                Assert.IsNull(provider.ProjectUrl);
            }

            [TestCase]
            public void ReturnsValidRawGitUrl()
            {
                var provider = new CustomRawUrlProvider();
                provider.Initialize("http://example.com/repo");

                Assert.AreEqual("http://example.com/repo", provider.RawGitUrl);
            }

            [TestCase]
            public void ReturnsValidRawGitUrlWithNoTrailingSlash()
            {
                var provider = new CustomRawUrlProvider();
                provider.Initialize("http://example.com/repo/");

                Assert.AreEqual("http://example.com/repo", provider.RawGitUrl);
            }
        }
    }
}
