// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbName.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Pdb
{
    internal class PdbName
    {
        internal PdbName(string name = "")
        {
            Name = name;
        }

        internal int Stream { get; set; }

        internal string Name { get; set; }

        internal int FlagIndex { get; set; }
    }
}