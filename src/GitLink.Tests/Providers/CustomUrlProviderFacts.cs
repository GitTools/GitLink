// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomUrlProviderFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Tests.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GitLink.Providers;
    using NUnit.Framework;

    public class CustomUrlProviderFacts
    {
        private const string CorrectUrl = "https://bitbucket.intra.company.com/projects/aaa/repos/a/browse/{filename}?at={revision}&raw";

        [TestFixture]
        public class TheInitialization
        {
            [TestCase(CorrectUrl, true)]
            [TestCase("https://example.com/repo", false)]
            [TestCase("https://bitbucket.intra.company.com/projects/aaa/repos/a/browse/{filename}?raw", true)]
            [TestCase("gopher://example.com/repo", false)]
            public void CorrectlyValidatesForUrls(string url, bool expectedValue)
            {
                var provider = new CustomUrlProvider();
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
                var provider = new CustomUrlProvider();
                provider.Initialize(CorrectUrl);

                Assert.IsNull(provider.CompanyName);
            }

            [TestCase]
            public void ReturnsNullCompanyUrl()
            {
                var provider = new CustomUrlProvider();
                provider.Initialize(CorrectUrl);

                Assert.IsNull(provider.CompanyUrl);
            }

            [TestCase]
            public void ReturnsNullProject()
            {
                var provider = new CustomUrlProvider();
                provider.Initialize(CorrectUrl);

                Assert.IsNull(provider.ProjectName);
            }

            [TestCase]
            public void ReturnsNullProjectUrl()
            {
                var provider = new CustomUrlProvider();
                provider.Initialize(CorrectUrl);

                Assert.IsNull(provider.ProjectUrl);
            }

            [TestCase]
            public void ReturnsValidRawGitUrl()
            {
                var provider = new CustomUrlProvider();
                provider.Initialize(CorrectUrl);

                string correctReturnedUrl = CorrectUrl.Replace("{filename}", "%var2%");
                correctReturnedUrl = correctReturnedUrl.Replace("{revision}", "{0}");

                Assert.AreEqual(correctReturnedUrl, provider.RawGitUrl);
            }
        }
    }
}
