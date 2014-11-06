// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitBucketProviderFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Tests.Providers
{
    using GitLink.Providers;
    using NUnit.Framework;

    public class BitBucketProviderFacts
    {
        [TestFixture]
        public class TheBitBucketProviderInitialization
        {
            [TestCase]
            public void ReturnsValidInitialization()
            {
                var provider = new BitBucketProvider();
                var valid = provider.Initialize("https://bitbucket.org/CatenaLogic/GitLink");

                Assert.IsTrue(valid);
            }

            [TestCase]
            public void ReturnsInValidInitialization()
            {
                var provider = new BitBucketProvider();
                var valid = provider.Initialize("https://github.com/CatenaLogic/GitLink");

                Assert.IsFalse(valid);
            }

            [TestFixture]
            public class TheBitBucketProviderProperties
            {
                [TestCase]
                public void ReturnsValidCompany()
                {
                    var provider = new BitBucketProvider();
                    provider.Initialize("https://bitbucket.org/CatenaLogic/GitLink");

                    Assert.AreEqual("CatenaLogic", provider.CompanyName);
                }

                [TestCase]
                public void ReturnsValidCompanyUrl()
                {
                    var provider = new BitBucketProvider();
                    provider.Initialize("https://bitbucket.org/CatenaLogic/GitLink");

                    Assert.AreEqual("https://bitbucket.org/CatenaLogic", provider.CompanyUrl);
                }

                [TestCase]
                public void ReturnsValidProject()
                {
                    var provider = new BitBucketProvider();
                    provider.Initialize("https://bitbucket.org/CatenaLogic/GitLink");

                    Assert.AreEqual("GitLink", provider.ProjectName);
                }

                [TestCase]
                public void ReturnsValidProjectUrl()
                {
                    var provider = new BitBucketProvider();
                    provider.Initialize("https://bitbucket.org/CatenaLogic/GitLink");

                    Assert.AreEqual("https://bitbucket.org/CatenaLogic/GitLink", provider.ProjectUrl);
                }

                [TestCase]
                public void ReturnsValidRawGitUrl()
                {
                    var provider = new BitBucketProvider();
                    provider.Initialize("https://bitbucket.org/CatenaLogic/GitLink");

                    Assert.AreEqual("https://bitbucket.org/CatenaLogic/GitLink/raw", provider.RawGitUrl);
                }
            }
        }
    }
}