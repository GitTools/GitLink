namespace GitLink.Pdb
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class PdbFile : IDisposable
    {
        private string path;

        private BinaryReader br;
        private BinaryWriter bw;
        private FileStream fs;

        private int pageByteCount;
        private int pageCount;
        private int pagesFree;
        private int rootByteCount;
        private int rootPage;
        private string srcsrv;

        private PdbRoot root;
        private PdbInfo _info;

        private SortedSet<int> freePages;
        private byte[] zerosPage;

        public PdbFile(string path)
        {
            this.path = path;
            srcsrv = "srcsrv";
            fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            br = new BinaryReader(fs, Encoding.UTF8, true);
            bw = new BinaryWriter(fs, Encoding.UTF8, true);

            CheckPdbHeader();
            ReadPdbHeader();
            CheckPdb();

            root = ReadRoot(RootPdbStream());
            freePages = new SortedSet<int>();
            zerosPage = new byte[pageByteCount];

        }

        private void CheckPdbHeader()
        {
            string msf = String.Format("Microsoft C/C++ MSF 7.00\r\n{0}DS\0\0\0", (char)0x1A);
            byte[] bytes = Encoding.UTF8.GetBytes(msf);
            if (!bytes.SequenceEqual(br.ReadBytes(32)))
            {
                throw new Exception("Pdb header didn't match");
            }
        }

        private void ReadPdbHeader()
        {
            pageByteCount = br.ReadInt32(); // 0x20
            pagesFree = br.ReadInt32(); // 0x24 TODO not sure meaning
            pageCount = br.ReadInt32(); // 0x28 for file
            rootByteCount = br.ReadInt32(); // 0x2C
            br.BaseStream.Position += 4; // 0
            rootPage = br.ReadInt32(); // 0x34
        }

        private void CheckPdb()
        {
            long length = fs.Length;
            if (length % pageByteCount != 0)
            {
                throw new Exception(String.Format("pdb length {0} bytes per page <> 0, {1}, {2}", length, pageByteCount,
                    pageCount));
            }
            if (length / pageByteCount != pageCount)
            {
                throw new Exception(
                    String.Format(
                        "pdb length does not match page count, length: {0}, bytes per page: {1}, page count: {2}",
                        length, pageByteCount, pageCount));
            }
        }

        private PdbRoot ReadRoot(PdbStream streamRoot)
        {
            var root = new PdbRoot(streamRoot);
            using (var brDirectory = StreamReader(streamRoot))
            {
                int streamCount = brDirectory.ReadInt32();
                if (streamCount != 0x0131CA0B)
                {
                    List<PdbStream> streams = root.Streams;
                    for (int i = 0; i < streamCount; i++)
                    {
                        var stream = new PdbStream();
                        streams.Add(stream);
                        int byteCount = brDirectory.ReadInt32();
                        stream.ByteCount = byteCount;
                        int pageCount = CountPages(byteCount);
                        stream.Pages = new int[pageCount];
                    }
                    for (int i = 0; i < streamCount; i++)
                    {
                        for (int j = 0; j < streams[i].Pages.Length; j++)
                        {
                            int page = brDirectory.ReadInt32();
                            streams[i].Pages[j] = page;
                        }
                    }
                }
            }

            return root;
        }

        public PdbStream RootPdbStream()
        {
            var pdbStream = new PdbStream();
            pdbStream.ByteCount = rootByteCount;
            pdbStream.Pages = new int[CountPages(rootByteCount)];
            GoToPage(rootPage);
            for (int i = 0; i < pdbStream.Pages.Length; i++)
            {
                pdbStream.Pages[i] = br.ReadInt32();
            }

            return pdbStream;
        }

        private PdbRoot GetRoot()
        {
            return ReadRoot(RootPdbStream());
        }

        #region Reading methods

        private int CountPages(int byteCount)
        {
            return (byteCount + pageByteCount - 1) / pageByteCount;
        }

        private void GoToPage(int page)
        {
            br.BaseStream.Position = page * pageByteCount;
        }

        private void GoToEnd()
        {
            fs.Seek(0L, SeekOrigin.End);
        }

        private void ReadPage(byte[] bytes, int page, int offset, int count)
        {
            GoToPage(page);
            int read = br.Read(bytes, offset, count);
            if (read != count)
            {
                throw new Exception(String.Format("tried reading {0} bytes at offset {1}, but only read {2}", count,
                    offset,
                    read));
            }
        }

        private byte[] ReadStreamBytes(PdbStream stream)
        {
            var bytes = new byte[stream.ByteCount];
            int[] pages = stream.Pages;

            if (pages.Length != 0)
            {
                for (int i = 0; i < pages.Length - 1; i++)
                {
                    ReadPage(bytes, pages[i], i * pageByteCount, pageByteCount);
                }

                int j = pages.Length - 1;
                ReadPage(bytes, pages[j], j * pageByteCount, (stream.ByteCount - j * pageByteCount));
            }

            return bytes;
        }

        private MemoryStream ReadStream(PdbStream stream)
        {
            return new MemoryStream(ReadStreamBytes(stream));
        }

        private BinaryReader StreamReader(PdbStream stream)
        {
            return new BinaryReader(ReadStream(stream));
        }

        #endregion

        private PdbInfo InternalInfo()
        {
            var info = new PdbInfo();
            using (var ms = new MemoryStream(ReadStreamBytes(GetRoot().Streams[1])))
            using (var br = new BinaryReader(ms))
            {
                info.Version = br.ReadInt32(); // 0x00 of stream
                info.Signature = br.ReadInt32(); // 0x04
                info.Age = br.ReadInt32(); // 0x08
                info.Guid = new Guid(br.ReadBytes(16)); // 0x0C
                var namesByteCount = br.ReadInt32(); // 0x16
                var namesByteStart = br.BaseStream.Position; // 0x20
                br.BaseStream.Position = namesByteStart + namesByteCount;
                var nameCount = br.ReadInt32();
                info.FlagIndexMax = br.ReadInt32();
                info.FlagCount = br.ReadInt32();
                var flags = new int[info.FlagCount]; // bit flags for each nameCountMax
                for (int i = 0; i < flags.Length; i++)
                {
                    flags[i] = br.ReadInt32();
                }

                br.BaseStream.Position += 4; // 0
                var positions = new List<Tuple<int, PdbName>>(nameCount);
                for (int i = 0; i < info.FlagIndexMax; i++)
                {
                    if ((flags[i / 32] & (1 << (i % 32))) != 0)
                    {
                        var position = br.ReadInt32();
                        var name = new PdbName();
                        name.FlagIndex = i;
                        name.Stream = br.ReadInt32();
                        positions.Add(new Tuple<int, PdbName>(position, name));
                    }
                }

                if (positions.Count != nameCount)
                    throw new Exception(String.Format("names count, {0} <> {1}", positions.Count, nameCount));

                var tailByteCount = GetRoot().Streams[1].ByteCount - br.BaseStream.Position;
                info.Tail = br.ReadBytes((int)tailByteCount);

                foreach (var tuple in positions)
                {
                    br.BaseStream.Position = namesByteStart + tuple.Item1;
                    tuple.Item2.Name = br.ReadCString();
                    info.AddName(tuple.Item2);
                }

                return info;
            }
        }

        private List<int> AllocPages(int n)
        {
            var pages = new List<int>();
            var free = freePages.ToList();
            var nFree = n <= free.Count ? n : free.Count;
            var nAlloc = n <= free.Count ? 0 : n - free.Count;
            for (int i = 0; i < nFree; i++)
            {
                var page = free[i];
                pages.Add(page);
                freePages.Remove(page);
            }

            if (nAlloc > 0)
            {
                GoToEnd();
                for (int i = 0; i < nAlloc; i++)
                {
                    var page = (int)fs.Position / pageByteCount;
                    pages.Add(page);
                    fs.Write(zerosPage, 0, zerosPage.Length);
                }
            }

            return pages;
        }

        public string Path { get { return path; } }
        public string PathSrcSrv { get { return path + ".srcsrv"; } }

        public byte[] ReadPdbStreamBytes(PdbStream pdbStream)
        {
            return ReadStreamBytes(pdbStream);
        }

        public byte[] ReadStreamBytes(int stream)
        {
            return ReadStreamBytes(root.Streams[stream]);
        }

        public PdbInfo Info
        {
            get { return _info ?? (_info = InternalInfo()); }
        }

        public bool HasSrcSrv { get { return Info.NameToPdbName.ContainsKey(srcsrv); } }

        public int SrcSrv { get { return Info.NameToPdbName[srcsrv].Stream; } }
        public int RootPage { get { return rootPage; } }
        public PdbRoot Root { get { return root; } }
        public PdbRoot Stream0 { get { return ReadRoot(root.Streams[0]); } }

        public int PagesFree { get { return pagesFree; } }
        public int PageCount { get { return pageCount; } }

        public void Dispose()
        {
            // Move to dispose
            bw.Close();
            br.Close();
            fs.Close();
        }
    }
}