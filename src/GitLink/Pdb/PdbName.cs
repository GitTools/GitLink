// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbName.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Pdb
{
    public class PdbName
    {
        public PdbName(string name = "")
        {
            Name = name;
        }

        public int Stream { get; set; }
        public string Name { get; set; }
        public int FlagIndex { get; set; }
    }
}