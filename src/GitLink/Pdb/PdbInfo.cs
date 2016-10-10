// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbInfo.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Pdb
{
    using System;
    using System.Collections.Generic;
    using Catel;

    internal class PdbInfo
    {
        internal PdbInfo()
        {
            Guid = default(Guid);
            StreamToPdbName = new SortedDictionary<int, PdbName>();
            NameToPdbName = new SortedDictionary<string, PdbName>();
            FlagIndexToPdbName = new SortedDictionary<int, PdbName>();
            FlagIndexes = new SortedSet<int>();
            SrcSrv = new string[0];
            Tail = new byte[0];
        }

        internal int Version { get; set; }

        internal int Signature { get; set; }

        internal Guid Guid { get; set; }

        internal int Age { get; set; }

        internal int FlagIndexMax { get; set; }

        internal int FlagCount { get; set; }

        internal IDictionary<int, PdbName> StreamToPdbName { get; private set; }

        internal IDictionary<string, PdbName> NameToPdbName { get; private set; }

        internal IDictionary<int, PdbName> FlagIndexToPdbName { get; private set; }

        internal SortedSet<int> FlagIndexes { get; private set; }

        internal string[] SrcSrv { get; set; }

        internal byte[] Tail { get; set; }

        internal void ClearFlags()
        {
            FlagIndexes.Clear();
            FlagIndexToPdbName.Clear();
        }

        internal void AddFlag(PdbName name)
        {
            Argument.IsNotNull(() => name);

            FlagIndexes.Add(name.FlagIndex);
            FlagIndexToPdbName.Add(name.FlagIndex, name);
        }

        internal void AddName(PdbName name)
        {
            Argument.IsNotNull(() => name);

            StreamToPdbName.Add(name.Stream, name);
            NameToPdbName.Add(name.Name, name);

            AddFlag(name);
        }
    }
}