// <copyright file="CustomUrlProvider.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>

namespace GitLink.Providers
{
    using System.Text.RegularExpressions;
    using GitTools.Git;

    public sealed class CustomUrlProvider : ProviderBase
    {
        private const string FileNamePlaceHolder = "{filename}";
        private const string RevisionPlaceHolder = "{revision}";
        private readonly Regex _regexUrl = new Regex(@"https?://.+");

        private string _rawUrl;

        public CustomUrlProvider()
            : base(new GitPreparer())
        {
        }

        public override string RawGitUrl
        {
            get
            {
                return _rawUrl;
            }
        }

        public override bool Initialize(string url)
        {
            if (string.IsNullOrEmpty(url) || !_regexUrl.IsMatch(url) ||
               (!url.Contains(FileNamePlaceHolder) && !url.Contains(RevisionPlaceHolder)))
            {
                return false;
            }

            if (url.Contains(FileNamePlaceHolder))
            {
                _rawUrl = url.Replace(FileNamePlaceHolder, "%var2%");
            }

            if (url.Contains(RevisionPlaceHolder))
            {
                _rawUrl = _rawUrl.Replace(RevisionPlaceHolder, "{0}");
            }

            return true;
        }
    }
}
