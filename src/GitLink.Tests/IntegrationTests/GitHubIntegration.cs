// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitHubIntegration.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Tests.IntegrationTests
{
    using NUnit.Framework;

    [TestFixture, Explicit]
    public class GitHubIntegration : IntegrationTestBase
    {
        public const string Url = "https://github.com/CatenaLogic/GitLinkTestRepro";
        public const string Directory = @"C:\Source\GitLinkTestRepro_GitHub";

        [Test]
        public void CorrectlyUpdatesPdbFiles()
        {
            const string directory = Directory;
            const string configurationName = "Release";

            var result = RunGitLink(directory, Url, "master", configurationName);

            Assert.AreEqual(0, result);

            VerifyUpdatedPdbs(directory, configurationName);
        }
    }
}