// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectExtensionsFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitLink.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class ProjectExtensionsFacts
    {
        [Test]
        public void NoIncludesExcludes_ProjectNotIgnored()
        {
            Assert.IsFalse(ProjectHelper.ShouldBeIgnored("project", new string[0], new string[0]));
        }

        [TestCase("ignoredProject", "ignoredProject", true)]
        [TestCase("ignoredProject", "ignoredproject", true)]
        [TestCase("ignoredProject", "/ignoredProject/", true)]
        [TestCase("ignoredProject", "/ignoredproject/", true)]
        [TestCase("ignoredProject", "/^i\\w+t$/", true)]
        [TestCase("nonIgnoredProject", "ignoredProject", false)]
        public void ExcludedProject_IgnoredOnlySpecifiedOne(string projectName, string ignorePattern, bool expectedIgnore)
        {
            Assert.AreEqual(expectedIgnore, ProjectHelper.ShouldBeIgnored(projectName, new string[0], new[] { ignorePattern }));
        }

        [TestCase("anotherProject", "includedProject", true)]
        [TestCase("anotherProject", "includedproject", true)]
        [TestCase("anotherProject", "/includedProject/", true)]
        [TestCase("anotherProject", "/includedproject/", true)]
        [TestCase("anotherProject", "/[a-z]+/", false)]
        [TestCase("includedProject", "includedProject", false)]
        public void ExplicitlyIncludedProject_OthersAreIgnored(string projectName, string includePattern, bool expectedIgnore)
        {
            Assert.AreEqual(expectedIgnore, ProjectHelper.ShouldBeIgnored(projectName, new[] { includePattern }, new string[0]));
        }

        [TestCase("excludedProject", true)]
        [TestCase("includedProject", false)]
        [TestCase("includedAndExcludedProject", true)]
        [TestCase("notIncludedNorExcludedProject", true)]
        public void BothIncludedAndExcludedProjects(string projectName, bool expectedIgnore)
        {
            Assert.AreEqual(expectedIgnore, ProjectHelper.ShouldBeIgnored(projectName,
                                                                          new[] { "includedProject", "includedAndExcludedProject" },
                                                                          new[] { "excludedProject", "includedAndExcludedProject" }));
        }
    }
}