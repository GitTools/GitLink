// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbFile.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Pdb
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Catel;
    using Catel.Logging;

    internal class PdbFile : IDisposable
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private const string SrcSrvContent = "srcsrv";

        private readonly BinaryReader _br;
        private readonly BinaryWriter _bw;
        private readonly FileStream _fs;

        private int _pageByteCount;
        private int _rootByteCount;

        private PdbInfo _info;

        private readonly SortedSet<int> _freePages;
        private readonly byte[] _zerosPage;

        internal PdbFile(string path)
        {
            Argument.IsNotNullOrWhitespace(() => path);

            Path = path;

            _fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            _br = new BinaryReader(_fs, Encoding.UTF8, true);
            _bw = new BinaryWriter(_fs, Encoding.UTF8, true);

            CheckPdbHeader();
            ReadPdbHeader();
            CheckPdb();

            Root = ReadRoot(GetRootPdbStream());
            _freePages = new SortedSet<int>();
            _zerosPage = new byte[_pageByteCount];
        }

        internal string Path { get; private set; }

        internal string PathSrcSrv
        {
            get { return Path + ".srcsrv"; }
        }

        internal bool HasSrcSrv
        {
            get { return Info.NameToPdbName.ContainsKey(SrcSrvContent); }
        }

        internal int SrcSrv
        {
            get { return Info.NameToPdbName[SrcSrvContent].Stream; }
        }

        internal int RootPage { get; private set; }

        internal PdbRoot Root { get; private set; }

        internal PdbRoot Stream0
        {
            get { return ReadRoot(Root.Streams[0]); }
        }

        internal int PagesFree { get; private set; }

        internal int PageCount { get; private set; }

        private void CheckPdbHeader()
        {
            var msf = String.Format("Microsoft C/C++ MSF 7.00\r\n{0}DS\0\0\0", (char)0x1A);
            var bytes = Encoding.UTF8.GetBytes(msf);
            if (!bytes.SequenceEqual(_br.ReadBytes(32)))
            {
                throw Log.ErrorAndCreateException<GitLinkException>("Pdb header didn't match");
            }
        }

        private void ReadPdbHeader()
        {
            // TODO: Create PdbHeader struct
            //// code here

            _pageByteCount = _br.ReadInt32(); // 0x20
            PagesFree = _br.ReadInt32(); // 0x24 TODO not sure meaning
            PageCount = _br.ReadInt32(); // 0x28 for file
            _rootByteCount = _br.ReadInt32(); // 0x2C
            _br.BaseStream.Position += 4; // 0
            RootPage = _br.ReadInt32(); // 0x34
        }

        private void CheckPdb()
        {
            var length = _fs.Length;
            if (length % _pageByteCount != 0)
            {
                throw Log.ErrorAndCreateException<GitLinkException>(
                    "pdb length {0} bytes per page <> 0, {1}, {2}",
                    length,
                    _pageByteCount,
                    PageCount);
            }

            if (length / _pageByteCount != PageCount)
            {
                throw Log.ErrorAndCreateException<GitLinkException>(
                    "pdb length does not match page count, length: {0}, bytes per page: {1}, page count: {2}",
                    length,
                    _pageByteCount,
                    PageCount);
            }
        }

        private PdbRoot ReadRoot(PdbStream streamRoot)
        {
            Argument.IsNotNull(() => streamRoot);

            var root = new PdbRoot(streamRoot);
            using (var brDirectory = StreamReader(streamRoot))
            {
                var streamCount = brDirectory.ReadInt32();
                if (streamCount != 0x0131CA0B)
                {
                    var streams = root.Streams;
                    for (var i = 0; i < streamCount; i++)
                    {
                        var stream = new PdbStream();
                        streams.Add(stream);

                        var byteCount = brDirectory.ReadInt32();
                        stream.ByteCount = byteCount;

                        var pageCount = CountPages(byteCount);
                        stream.Pages = new int[pageCount];
                    }

                    for (var i = 0; i < streamCount; i++)
                    {
                        for (var j = 0; j < streams[i].Pages.Length; j++)
                        {
                            var page = brDirectory.ReadInt32();
                            streams[i].Pages[j] = page;
                        }
                    }
                }
            }

            return root;
        }

        internal PdbStream GetRootPdbStream()
        {
            var pdbStream = new PdbStream();
            pdbStream.ByteCount = _rootByteCount;
            pdbStream.Pages = new int[CountPages(_rootByteCount)];

            GoToPage(RootPage);

            for (var i = 0; i < pdbStream.Pages.Length; i++)
            {
                pdbStream.Pages[i] = _br.ReadInt32();
            }

            return pdbStream;
        }

        private PdbRoot GetRoot()
        {
            var pdbRootStream = GetRootPdbStream();
            return ReadRoot(pdbRootStream);
        }

        #region Reading methods

        private int CountPages(int byteCount)
        {
            return (byteCount + _pageByteCount - 1) / _pageByteCount;
        }

        private void GoToPage(int page)
        {
            _br.BaseStream.Position = page * _pageByteCount;
        }

        private void GoToEnd()
        {
            _fs.Seek(0L, SeekOrigin.End);
        }

        private void ReadPage(byte[] bytes, int page, int offset, int count)
        {
            GoToPage(page);

            var read = _br.Read(bytes, offset, count);
            if (read != count)
            {
                throw Log.ErrorAndCreateException<GitLinkException>("tried reading {0} bytes at offset {1}, but only read {2}", count, offset, read);
            }
        }

        private byte[] ReadStreamBytes(PdbStream stream)
        {
            Argument.IsNotNull(() => stream);

            var bytes = new byte[stream.ByteCount];
            var pages = stream.Pages;

            if (pages.Length != 0)
            {
                for (var i = 0; i < pages.Length - 1; i++)
                {
                    ReadPage(bytes, pages[i], i * _pageByteCount, _pageByteCount);
                }

                var j = pages.Length - 1;
                ReadPage(bytes, pages[j], j * _pageByteCount, stream.ByteCount - (j * _pageByteCount));
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

            var root = GetRoot();
            if (root.Streams.Count <= 1)
            {
                throw Log.ErrorAndCreateException<GitLinkException>(
                    "Expected at least 2 streams inside the pdb root, but only found '{0}', cannot read pdb info",
                    root.Streams.Count);
            }

            using (var ms = new MemoryStream(ReadStreamBytes(root.Streams[1])))
            {
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
                    for (var i = 0; i < flags.Length; i++)
                    {
                        flags[i] = br.ReadInt32();
                    }

                    br.BaseStream.Position += 4; // 0
                    var positions = new List<Tuple<int, PdbName>>(nameCount);
                    for (var i = 0; i < info.FlagIndexMax; i++)
                    {
                        var flagIndex = i / 32;
                        if (flagIndex >= flags.Length)
                        {
                            break;
                        }

                        var flag = flags[flagIndex];
                        if ((flag & (1 << (i % 32))) != 0)
                        {
                            var position = br.ReadInt32();
                            var name = new PdbName();
                            name.FlagIndex = i;
                            name.Stream = br.ReadInt32();

                            positions.Add(new Tuple<int, PdbName>(position, name));
                        }
                    }

                    if (positions.Count != nameCount)
                    {
                        throw Log.ErrorAndCreateException<GitLinkException>("names count, {0} <> {1}", positions.Count, nameCount);
                    }

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
        }

        private List<int> AllocPages(int n)
        {
            var pages = new List<int>();
            var free = _freePages.ToList();
            var nFree = n <= free.Count ? n : free.Count;
            var nAlloc = n <= free.Count ? 0 : n - free.Count;
            for (var i = 0; i < nFree; i++)
            {
                var page = free[i];
                pages.Add(page);
                _freePages.Remove(page);
            }

            if (nAlloc > 0)
            {
                GoToEnd();

                for (var i = 0; i < nAlloc; i++)
                {
                    var page = (int)_fs.Position / _pageByteCount;
                    pages.Add(page);

                    _fs.Write(_zerosPage, 0, _zerosPage.Length);
                }
            }

            return pages;
        }

        internal byte[] ReadPdbStreamBytes(PdbStream pdbStream)
        {
            Argument.IsNotNull(() => pdbStream);

            return ReadStreamBytes(pdbStream);
        }

        internal byte[] ReadStreamBytes(int stream)
        {
            return ReadStreamBytes(Root.Streams[stream]);
        }

        internal PdbInfo Info
        {
            get { return _info ?? (_info = InternalInfo()); }
        }

        public void Dispose()
        {
            // Move to dispose
            _bw.Close();
            _br.Close();
            _fs.Close();
        }
    }
}