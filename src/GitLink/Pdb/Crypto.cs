// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Crypto.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Pdb
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using Catel;

    public static class Crypto
    {
        public static byte[] HashFile(HashAlgorithm ha, string file)
        {
            using (var fs = File.OpenRead(file))
            {
                return ha.ComputeHash(fs);
            }
        }
    }
}