// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbRoot.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Pdb
{
    using System.Collections.Generic;

    public class PdbRoot
    {
        public PdbRoot(PdbStream stream)
        {
            Stream = stream;
            Streams = new List<PdbStream>();
        }

        public PdbStream Stream { get; set; }
        public List<PdbStream> Streams { get; set; }

        public int AddStream(PdbStream stream)
        {
            Streams.Add(stream);
            return Streams.Count - 1;
        }
    }
}