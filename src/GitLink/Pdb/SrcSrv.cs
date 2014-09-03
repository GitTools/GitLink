// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SrcSrv.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Pdb
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class SrcSrv
    {
        private static string CreateTarget(string rawUrl, string revision)
        {
            return String.Format(rawUrl, revision);
        }

        public static byte[] Create(string rawUrl, string revision, IEnumerable<Tuple<string, string>> paths)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    var scheme = new Uri(rawUrl).Scheme;
                    sw.WriteLine("SRCSRV: ini ------------------------------------------------");
                    sw.WriteLine("VERSION=2");
                    sw.WriteLine("SRCSRV: variables ------------------------------------------");
                    sw.WriteLine("SRCSRVVERCTRL={0}", scheme);
                    sw.WriteLine("SRCSRVTRG={0}", CreateTarget(rawUrl, revision));
                    sw.WriteLine("SRCSRV: source files ---------------------------------------");
                    foreach (var tuple in paths)
                    {
                        sw.WriteLine("{0}*{1}", tuple.Item1, tuple.Item2);
                    }
                    sw.WriteLine("SRCSRV: end ------------------------------------------------");
                    sw.Flush();
                    return ms.ToArray();
                }
            }
        }
    }
}