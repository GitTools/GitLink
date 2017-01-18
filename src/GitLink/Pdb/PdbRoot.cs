// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbRoot.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Pdb
{
    using System.Collections.Generic;
    using Catel;

    internal class PdbRoot
    {
        internal PdbRoot(PdbStream stream)
        {
            Argument.IsNotNull(() => stream);

            Stream = stream;
            Streams = new List<PdbStream>();
        }

        internal PdbStream Stream { get; set; }

        internal List<PdbStream> Streams { get; private set; }

        internal int AddStream(PdbStream stream)
        {
            Streams.Add(stream);

            return Streams.Count - 1;
        }
    }
}