namespace GitLink.Providers
{
    using GitTools.Git;
    using System.Text.RegularExpressions;

    public sealed class CustomUrlProvider : ProviderBase
    {
        private static readonly string fileNamePlaceHolder = "{filename}";
        private static readonly string revisionPlaceHolder = "{revision}";
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
            if (string.IsNullOrEmpty(url) || !_regexUrl.IsMatch(url) ||(
                !url.Contains(fileNamePlaceHolder) && !url.Contains(revisionPlaceHolder)))
            {
                return false;
            }

            if(url.Contains(fileNamePlaceHolder))
                _rawUrl = url.Replace(fileNamePlaceHolder, "%var2%");

            if(url.Contains(revisionPlaceHolder))
                _rawUrl = _rawUrl.Replace(revisionPlaceHolder, "{0}");

            return true;
        }
    }
}
