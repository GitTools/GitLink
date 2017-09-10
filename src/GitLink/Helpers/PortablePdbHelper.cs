// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortablePdbHelper.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink
{
    using System.IO;

    internal static class PortablePdbHelper
    {
        /// <summary>
        /// Is the given .pdb using the new Portable format ? (https://github.com/dotnet/corefx/blob/master/src/System.Reflection.Metadata/specs/PortablePdb-Metadata.md)
        /// </summary>
        /// <param name="pdbPath">.pdb file path</param>
        /// <returns>Returns if it's a Portable PDB</returns>
        public static bool IsPortablePdb(string pdbPath)
        {
            using (var fs = File.Open(pdbPath, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs))
            {
                // More infos in chapter II.24.2 of ECMA-335 (http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-335.pdf)
                var signature = 0x424A5342;
                if (br.ReadUInt32() != signature)
                {
                    return false;
                }

                var majorVersion = br.ReadUInt16();
                if (majorVersion != 1)
                {
                    return false;
                }

                var minorVersion = br.ReadUInt16();
                if (minorVersion != 1)
                {
                    return false;
                }

                var reserved = br.ReadUInt32();
                if (reserved != 0)
                {
                    return false;
                }

                var versionLength = br.ReadUInt32();
                var version = System.Text.Encoding.UTF8.GetString(br.ReadBytes((int)versionLength));

                return version.StartsWith("PDB v1.0");
            }
        }
    }
}