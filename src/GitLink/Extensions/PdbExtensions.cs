// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using Pdb;

    public static class PdbExtensions
    {
        public static IEnumerable<string> FindMissingOrChangedSourceFiles(this PdbFile pdbFile)
        {
            Argument.IsNotNull(() => pdbFile);

            foreach (var checksumInfo in pdbFile.GetChecksums())
            {
                string file = checksumInfo.Key;
                string expectedChecksum = checksumInfo.Value;
                string actualChecksum = File.Exists(file) ? Hex.Encode(Crypto.GetMd5HashForFile(file)) : string.Empty;

                if (expectedChecksum != actualChecksum)
                {
                    yield return file;
                }
            }
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
                if (!value.Name.StartsWith(FileIndicator))
                {
                    continue;
                }

                var num = value.Stream;
                var name = value.Name.Substring(FileIndicator.Length);

                // Get last 16 bytes for checksum
                var bytes = pdbFile.ReadStreamBytes(num);
                var checksum = new byte[16];
                Array.Copy(bytes, bytes.Length - 16, checksum, 0, 16);
                results.Add(Tuple.Create(name, checksum));
            }

            return results;
        }
    }
}