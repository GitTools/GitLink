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

        [TestCase("ignoredProject", true)]
        [TestCase("nonIgnoredProject", false)]
        public void ExcludedProject_IgnoredOnlySpecifiedOne(string projectName, bool expectedIgnore)
        {
            Assert.AreEqual(expectedIgnore, ProjectHelper.ShouldBeIgnored(projectName, new string[0], new[] { "ignoredProject" }));
        }

        [TestCase("anotherProject", true)]
        [TestCase("includedProject", false)]
        public void ExplicitlyIncludedProject_OthersAreIgnored(string projectName, bool expectedIgnore)
        {
            Assert.AreEqual(expectedIgnore, ProjectHelper.ShouldBeIgnored(projectName, new[] { "includedProject" }, new string[0]));
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