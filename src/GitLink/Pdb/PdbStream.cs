// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbStream.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Pdb
{
    internal class PdbStream
    {
        internal int ByteCount { get; set; }

        internal int[] Pages { get; set; }
    }
}