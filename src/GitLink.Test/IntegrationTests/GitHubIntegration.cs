// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitHubIntegration.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Test.IntegrationTests
{
    using NUnit.Framework;

    [TestFixture, Explicit]
    public class GitHubIntegration : IntegrationTestBase
    {
        public const string Url = "https://github.com/CatenaLogic/GitLinkTestRepro";
        public const string Directory = @"C:\Source\GitLinkTestRepro_GitHub";

        [TestCase]
        public void CorrectlyUpdatesPdbFiles()
        {
            var result = RunGitLink(Directory, Url, "master", "Release");

            Assert.AreEqual(0, result);

            // TODO: Validate pdb
        }
    }
}