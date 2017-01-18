// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryExtensionFacts.cs" company="Andrew Arnott">
//   Copyright (c) 2016 Andrew Arnott. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Tests.Extensions
{
    using System.IO;
    using GitTools.Git;
    using LibGit2Sharp;
    using NUnit.Framework;

    [TestFixture]
    public class RepositoryExtensionFacts
    {
        private static readonly char[] PathSeparators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        private Repository repo;

        [SetUp]
        public void SetUp()
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

            string repositoryDirectory = GitDirFinder.TreeWalkForGitDir(assemblyDirectory);
            repo = new Repository(repositoryDirectory);
        }

        [Theory, Pairwise]
        public void NormalizeFileAtRoot(bool scrambleCase, bool absolutePath, bool emptySegments, bool forwardSlashes)
        {
            string expected = "LICENSE";
            string input = GetPathToTest(expected, scrambleCase, absolutePath, emptySegments, forwardSlashes);
            string actual = repo.GetNormalizedPath(input);
            Assert.AreEqual(expected, actual);
        }

        [Theory, Pairwise]
        public void NormalizeFileOneDirDeep(bool scrambleCase, bool absolutePath, bool emptySegments, bool forwardSlashes)
        {
            string expected = "src/EnlistmentInfo.targets";
            string input = GetPathToTest(expected, scrambleCase, absolutePath, emptySegments, forwardSlashes);
            string actual = repo.GetNormalizedPath(input);
            Assert.AreEqual(expected, actual);
        }

        [Theory, Pairwise]
        public void NormalizeFileTwoDirsDeep(bool scrambleCase, bool absolutePath, bool emptySegments, bool forwardSlashes)
        {
            string expected = "src/GitLink/Linker.cs";
            string input = GetPathToTest(expected, scrambleCase, absolutePath, emptySegments, forwardSlashes);
            string actual = repo.GetNormalizedPath(input);
            Assert.AreEqual(expected, actual);
        }

        [Theory]
        public void NormalizeMissingFile([Values("T/N", "T\\N", "T", "T/n/C")]string path)
        {
            Assert.AreEqual(path, repo.GetNormalizedPath(path));
        }

        private string GetPathToTest(string expected, bool scrambleCase, bool absolutePath, bool emptySegments, bool forwardSlashes)
        {
            string actual = expected;
            if (scrambleCase)
            {
                actual = actual.ToUpperInvariant();
                if (actual == expected)
                {
                    actual = actual.ToLowerInvariant();
                }
            }

            if (absolutePath)
            {
                actual = Path.Combine(repo.Info.WorkingDirectory, actual);
            }

            if (emptySegments)
            {
                if (actual.IndexOfAny(PathSeparators) >= 0)
                {
                    actual = actual.Replace(Path.DirectorySeparatorChar.ToString(), Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString())
                                   .Replace(Path.AltDirectorySeparatorChar.ToString(), Path.AltDirectorySeparatorChar.ToString() + Path.AltDirectorySeparatorChar.ToString());
                }
            }

            actual = forwardSlashes
                ? actual.Replace('\\', '/')
                : actual.Replace('/', '\\');

            return actual;
        }
    }
}
