// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hex.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Pdb
{
    using System;
    using System.Linq;

    public static class Hex
    {
        public static string Encode(byte[] buf)
        {
            return BitConverter.ToString(buf).Replace("-", string.Empty);
        }

        public static byte[] Decode(string hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException();
            }

            if (String.IsNullOrEmpty(hex))
            {
                return new byte[0];
            }

            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}