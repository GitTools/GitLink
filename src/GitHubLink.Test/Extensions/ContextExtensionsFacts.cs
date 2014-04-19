// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextExtensionsFacts.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink.Test.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class ContextExtensionsFacts
    {
        #region Nested type: TheGetRelativePathMethod

        [TestClass]
        public class TheGetRelativePathMethod
        {
            [TestMethod]
            public void ReturnsRelativePathWithDirectoryDownwards()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\githublink"
                };

                var relativePath = context.GetRelativePath(@"c:\source\githublink\src\subdir1\somefile.cs");

                Assert.AreEqual(@"src\subdir1\somefile.cs", relativePath);
            }

            [TestMethod]
            public void ReturnsRelativePathWithDirectoryUpwards()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\githublink"
                };

                var relativePath = context.GetRelativePath(@"c:\source\catel\src\subdir1\somefile.cs");

                Assert.AreEqual(@"..\catel\src\subdir1\somefile.cs", relativePath);
            }
        }

        #endregion
    }
}