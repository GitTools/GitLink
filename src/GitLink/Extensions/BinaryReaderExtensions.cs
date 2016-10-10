// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryReaderExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Catel;

    internal static class BinaryReaderExtensions
    {
        internal static string ReadCString(this BinaryReader binaryReader)
        {
            Argument.IsNotNull(() => binaryReader);

            var list = new List<byte>();

            var b = binaryReader.ReadByte();
            while (b != '\0')
            {
                list.Add(b);
                b = binaryReader.ReadByte();
            }

            return Encoding.UTF8.GetString(list.ToArray());
        }
    }
}