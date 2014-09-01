// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextExtensionsFacts.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Test.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class ContextExtensionsFacts
    {
        [TestClass]
        public class TheGetRelativePathMethod
        {
            [TestMethod]
            public void ReturnsRelativePathWithDirectoryDownwards()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\GitLink"
                };

                var relativePath = context.GetRelativePath(@"c:\source\GitLink\src\subdir1\somefile.cs");

                Assert.AreEqual(@"src\subdir1\somefile.cs", relativePath);
            }

            [TestMethod]
            public void ReturnsRelativePathWithDirectoryUpwards()
            {
                var context = new Context
                {
                    SolutionDirectory = @"c:\source\GitLink"
                };

                var relativePath = context.GetRelativePath(@"c:\source\catel\src\subdir1\somefile.cs");

                Assert.AreEqual(@"..\catel\src\subdir1\somefile.cs", relativePath);
            }
        }
    }
}