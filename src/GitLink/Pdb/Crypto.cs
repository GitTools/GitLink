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
        public static Tuple<byte[], string> HashFile(HashAlgorithm ha, string file)
        {
            using (var fs = File.OpenRead(file))
            {
                return new Tuple<byte[], string>(ha.ComputeHash(fs), file);
            }
        }

        public static byte[] GetMd5HashForFile(string file)
        {
            Argument.IsNotNull(() => file);

            using (var ha = MD5.Create())
            {
                return HashFile(ha, file).Item1;
            }
        }

        public static Tuple<byte[], string>[] GetMd5HashForFiles(IEnumerable<string> files)
        {
            using (var ha = MD5.Create())
            {
                return files.Select(file => HashFile(ha, file)).ToArray();
            }
        }
    }
}