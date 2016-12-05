// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SrcSrvContext.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Pdb
{
    using System;
    using System.Collections.Generic;

    internal class SrcSrvContext
    {
        internal SrcSrvContext()
        {
            Paths = new List<Tuple<string, string>>();
            VstsData = new Dictionary<string, string>();
        }

        internal string RawUrl { get; set; }

        internal bool DownloadWithPowershell { get; set; }

        internal string Revision { get; set; }

        internal List<Tuple<string, string>> Paths { get; private set; }

        internal Dictionary<string, string> VstsData { get; private set; }
    }
}
