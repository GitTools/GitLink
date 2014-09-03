// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryReaderExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public static class BinaryReaderExtensions
    {
        public static Guid ReadGuid(this BinaryReader binaryReader)
        {
            return new Guid(binaryReader.ReadBytes(16));
        }

        public static string ReadCString(this BinaryReader binaryReader)
        {
            var list = new List<byte>();
            byte b = binaryReader.ReadByte();
            while (b != '\n')
            {
                list.Add(b);
                b = binaryReader.ReadByte();
            }
            return Encoding.UTF8.GetString(list.ToArray());
        }
    }
}