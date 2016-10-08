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
    using System.Security.Cryptography;
    using Catel;
    using Pdb;

    public static class PdbExtensions
    {
        public static IEnumerable<string> FindMissingOrChangedSourceFiles(this PdbFile pdbFile)
        {
            Argument.IsNotNull(() => pdbFile);

            foreach (var checksumInfo in pdbFile.GetFilesAndChecksums())
            {
                string file = checksumInfo.Key;
                byte[] expectedChecksum = checksumInfo.Value;
                HashAlgorithm hasher = expectedChecksum.Length == 16 ? (HashAlgorithm)MD5.Create() : SHA1.Create();
                byte[] actualChecksum = File.Exists(file) ? Crypto.HashFile(hasher, file) : null;

                if (!AreEqualBuffers(expectedChecksum, actualChecksum))
                {
                    yield return file;
                }
            }
        }

        public static IReadOnlyDictionary<string, byte[]> GetFilesAndChecksums(this PdbFile pdbFile)
        {
            Argument.IsNotNull(() => pdbFile);

            //const int LastInterestingByte = 47;
            const string FileIndicator = "/src/files/";

            var values = pdbFile.Info.NameToPdbName.Values;

            var results = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
            foreach (var value in values)
            {
                if (!value.Name.StartsWith(FileIndicator))
                {
                    continue;
                }

                var num = value.Stream;
                var name = value.Name.Substring(FileIndicator.Length);

                // Get last bytes for checksum (it may be MD5 or SHA1)
                var bytes = pdbFile.ReadStreamBytes(num);
                int hashLength = bytes.Length - 72;
                var checksum = new byte[hashLength];
                Array.Copy(bytes, bytes.Length - hashLength, checksum, 0, hashLength);
                results.Add(name, checksum);
            }

            return results;
        }

        private static bool AreEqualBuffers(byte[] first, byte[] second)
        {
            Argument.IsNotNull(nameof(first), first);
            Argument.IsNotNull(nameof(second), second);

            if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}