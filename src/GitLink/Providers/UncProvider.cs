// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UncProvider.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Providers
{
    using GitTools.Git;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A git provider for UNC network paths.
    /// </summary>
    public class UncProvider : ProviderBase, IBackSlashSupport
    {
        /// <summary>
        /// Gets a placeholder for file name.
        /// </summary>
        public const string FileNamePlaceHolder = "{filename}";

        /// <summary>
        /// Gets a placeholder for commit hash.
        /// </summary>
        public const string RevisionPlaceHolder = "{revision}";

        private static readonly Regex _uncRegex = new Regex(@"^(//|\\).+");
        private string _rawUrl;

        public UncProvider()
            : base(new GitPreparer())
        {
        }

        public override string RawGitUrl
        {
            get { return _rawUrl; }
        }

        public bool IsBackSlashSupported
        {
            get
            {
                if (RawGitUrl.StartsWith("//"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public override bool Initialize(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            bool isMatch = false;
            if (_uncRegex.IsMatch(url))
            {
                if (url.Contains(FileNamePlaceHolder))
                {
                    _rawUrl = url.Replace(FileNamePlaceHolder, "%var2%");
                    isMatch = true;
                }

                if (url.Contains(RevisionPlaceHolder))
                {
                    _rawUrl = _rawUrl.Replace(RevisionPlaceHolder, "{0}");
                    isMatch = true;
                }
            }

            return isMatch;
        }
    }
}
