// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectHelperFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitLink.Tests
{
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class ProjectHelperFacts
    {
        private const string SolutionFile = @"TestSolution\TestSolution.sln"; 

        [Test]
        public void GettingProjectsFromSolution()
        {
            Assert.AreEqual(3, ProjectHelper.GetProjects(SolutionFile, "Debug", "Any CPU").Count());
        }
    }
}