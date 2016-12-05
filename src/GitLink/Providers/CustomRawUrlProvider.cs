// <copyright file="CustomRawUrlProvider.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>

namespace GitLink.Providers
{
    using System;
    using System.Text.RegularExpressions;
    using GitTools.Git;

    public sealed class CustomRawUrlProvider : ProviderBase
    {
        private static readonly Regex HostingUrlPattern = new Regex(@"https?://.+");

        private string _rawUrl;

        public CustomRawUrlProvider()
            : base(new GitPreparer())
        {
        }

        public override string RawGitUrl => _rawUrl;

        public override bool Initialize(string url)
        {
            if (string.IsNullOrEmpty(url) || !HostingUrlPattern.IsMatch(url))
            {
                return false;
            }

            _rawUrl = url;
            if (_rawUrl.EndsWith("/", StringComparison.Ordinal))
            {
                _rawUrl = _rawUrl.TrimEnd('/');
            }

            return true;
        }
    }
}
