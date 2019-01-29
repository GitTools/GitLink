// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkOptions.cs" company="Andrew Arnott">
//   Copyright (c) 2016 Andrew Arnott. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public struct LinkOptions
    {
        public LinkMethod Method { get; set; }

        public bool SkipVerify { get; set; }

        public Uri GitRemoteUrl { get; set; }

        public string CommitId { get; set; }

        public string GitWorkingDirectory { get; set; }

        public bool IndexAllDepotFiles { get; set; }

        public bool IndexWithSrcTool { get; set; }

        public string IntermediateOutputPath { get; set; }
    }
}
