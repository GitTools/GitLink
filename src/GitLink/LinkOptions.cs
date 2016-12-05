// <copyright file="LinkOptions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>

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
    }
}
