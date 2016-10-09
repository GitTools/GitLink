namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public struct LinkOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the source should be downloaded with
        /// powershell instead of simple HTTP(S) URLs.
        /// </summary>
        public bool DownloadWithPowerShell { get; set; }

        public bool SkipVerify { get; set; }

        public Uri GitRemoteUrl { get; set; }

        public string CommitId { get; set; }

        public string GitWorkingDirectory { get; set; }
    }
}
