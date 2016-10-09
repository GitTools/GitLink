// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbRoot.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Pdb
{
    using System.Collections.Generic;
    using Catel;

    public class PdbRoot
    {
        public PdbRoot(PdbStream stream)
        {
            Argument.IsNotNull(() => stream);

            Stream = stream;
            Streams = new List<PdbStream>();
        }

        public PdbStream Stream { get; set; }

        public List<PdbStream> Streams { get; private set; }

        public int AddStream(PdbStream stream)
        {
            Streams.Add(stream);

            return Streams.Count - 1;
        }
    }
}