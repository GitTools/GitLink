// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SrcSrvContext.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Pdb
{
    using System;
    using System.Collections.Generic;

    public class SrcSrvContext
    {
        public SrcSrvContext()
        {
            Paths = new List<Tuple<string, string>>();
            VstsData = new Dictionary<string, string>();

        }

        public string RawUrl { get; set; }

        public bool DownloadWithPowershell { get; set; }

        public string Revision { get; set; }

        public List<Tuple<string, string>> Paths { get; private set; }

        public Dictionary<string, string> VstsData { get; private set; }
    }
}
