using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitLink.Pdb
{
    using System.IO;
    using System.Security.Cryptography;

    public static class Crypto
    {
        public static Tuple<byte[], string> Hash(HashAlgorithm ha, string file)
        {
            using (var fs = File.OpenRead(file))
            {
                return new Tuple<byte[], string>(ha.ComputeHash(fs), file);
            }
        }

        public static byte[] HashMD5(string file)
        {
            using (var ha = MD5.Create())
            {
                return Hash(ha, file).Item1;
            }
        }

        public static Tuple<byte[], string>[] HashesMD5(IEnumerable<string> files)
        {
            using (var ha = MD5.Create())
            {
                return files.Select(file => Hash(ha, file)).ToArray();
            }
        }

        //let hashesMD5 files =
        //    use ha = MD5.Create()
        //    files |> Seq.map (hash ha) |> Array.ofSeq

        //let hashSHA1 file =
        //    use ha = SHA1.Create()
        //    hash ha file |> fst

        //let hashesSHA1 files =
        //    use ha = SHA1.Create()
        //    files |> Seq.map (hash ha) |> Array.ofSeq
    }
}
