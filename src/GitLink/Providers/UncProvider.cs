using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitTools.Git;
using System.Text.RegularExpressions;

namespace GitLink.Providers
{
    /// <summary>
    /// A git provider for UNC network paths.
    /// </summary>
    public class UncProvider : ProviderBase, IBackSlashSupport
    {
        /// <summary>
        /// A class with placeholder constants.
        /// </summary>
        public static class PlaceHolder
        {
            /// <summary>
            /// Gets a placeholder for file name.
            /// </summary>
            public const string FileName = "{filename}";

            /// <summary>
            /// Gets a placeholder for commit hash.
            /// </summary>
            public const string Revision = "{revision}";
        }

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
                    return false;
                else
                    return true;
            }
        }

        public override bool Initialize(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            bool isMatch = false;
            if(_uncRegex.IsMatch(url))
            {
                if (url.Contains(PlaceHolder.FileName))
                {
                    _rawUrl = url.Replace(PlaceHolder.FileName, "%var2%");
                    isMatch = true;
                }

                if (url.Contains(PlaceHolder.Revision))
                {
                    _rawUrl = _rawUrl.Replace(PlaceHolder.Revision, "{0}");
                    isMatch = true;
                }
            }

            return isMatch;
        }
    }
}
