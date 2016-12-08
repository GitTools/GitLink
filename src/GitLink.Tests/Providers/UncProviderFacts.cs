using GitLink.Providers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitLink.Tests.Providers
{
    /// <summary>
    /// Test cases for <see cref="UncProvider"/>.
    /// </summary>
    public class UncProviderFacts
    {
        [TestFixture]
        public class TheInitialization
        {
            [TestCase(@"//home/repo/{filename}", true)]
            [TestCase(@"//home/repo/{revision}/{filename}", true)]
            [TestCase(@"\\home\repo\{filename}", true)]
            [TestCase(@"\\home\repo\{revision}\{filename}", true)]
            [TestCase(@"//home/repo/", false)]
            [TestCase(@"\\home\repo\", false)]
            public void CorrectlyValidatesUncPath(string path, bool expectedValue)
            {
                var provider = new UncProvider();
                var valid = provider.Initialize(path);

                Assert.AreEqual(expectedValue, valid);
            }

            [TestCase(@"//home/repo/{filename}")]
            [TestCase(@"//home/repo/{revision}/{filename}")]
            [TestCase(@"\\home\repo\{filename}")]
            [TestCase(@"\\home\repo\{revision}\{filename}")]
            public void CorrenctlyReplacesPlaceHolders(string path)
            {
                var provider = new UncProvider();
                Assert.IsTrue(provider.Initialize(path));

                string expectedPath = path.Replace("{filename}", "%var2%").Replace("{revision}", "{0}");
                Assert.AreEqual(expectedPath, provider.RawGitUrl);
            }
        }
    }
}
