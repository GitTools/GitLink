// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Pdb;

    public static class PdbExtensions
    {
        public static Dictionary<string, string> VerifyPdbFiles(this PdbFile pdbFile, IEnumerable<string> files)
        {
            Argument.IsNotNull(() => pdbFile);

            var missing = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var actualFileChecksums = (from x in files
                                       select new KeyValuePair<string, string>(Hex.Encode(Crypto.GetMd5HashForFiles(new[] { x }).First().Item1), x)).ToDictionary(x => x.Value, x => x.Key);

            foreach (var checksumInfo in pdbFile.GetChecksums())
            {
                var file = checksumInfo.Key;
                var checksum = checksumInfo.Value;

                if (!actualFileChecksums.ContainsValue(checksum))
                {
                    missing[file] = checksum;
                }
            }

            return missing;
        }

        public static Dictionary<string, string> GetChecksums(this PdbFile pdbFile)
        {
            Argument.IsNotNull(() => pdbFile);

            var checksums = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var file in pdbFile.GetFiles())
            {
                checksums.Add(file.Item1, Hex.Encode(file.Item2));
            }

            return checksums;
        }

        public static IEnumerable<Tuple<string, byte[]>> GetFiles(this PdbFile pdbFile)
        {
            Argument.IsNotNull(() => pdbFile);

            //const int LastInterestingByte = 47;
            const string FileIndicator = "/src/files/";

            var values = pdbFile.Info.NameToPdbName.Values;

            var results = new List<Tuple<string, byte[]>>();
            foreach (var value in values)
            {
                if (!value.Name.Contains(FileIndicator))
                {
                    continue;
                }

                var num = value.Stream;
                var name = value.Name.Substring(FileIndicator.Length);

                var bytes = pdbFile.ReadStreamBytes(num);
                if (bytes.Length != 88)
                {
                    continue;
                }

                // Get last 16 bytes for checksum
                var buffer = new byte[16];
                for (int i = 72; i < 88; i++)
                {
                    buffer[i-72] = bytes[i];
                }

                results.Add(new Tuple<string, byte[]>(name, buffer));
            }

            return results;
        }
    }
}