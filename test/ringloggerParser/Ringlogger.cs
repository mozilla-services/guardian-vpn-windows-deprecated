/* SPDX-License-Identifier: MIT
 *
 * Copyright (C) 2019 Edge Security LLC. All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace RingloggerParser
{
    /// <summary>
    /// Ringlogger allows logging to a memory mapped file.
    /// </summary>
    public class Ringlogger
    {
        private readonly Log log;
        private readonly string tag;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ringlogger"/> class.
        /// Used for logging to a binary log file.
        /// </summary>
        /// <param name="filename">Filename to log to.</param>
        /// <param name="tag">Tag to use when logging (e.g. "[FPN]").</param>
        public Ringlogger(string filename, string tag)
        {
            // Attempt to create a directory in the appdata folder if it does not exist
            Directory.CreateDirectory(new FileInfo(filename).Directory.FullName);

            var file = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
            file.SetLength(Log.Bytes);

            using (var mmap = MemoryMappedFile.CreateFromFile(file, null, 0, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, false))
            {
                var view = mmap.CreateViewAccessor(0, Log.Bytes, MemoryMappedFileAccess.ReadWrite);

                log = new Log(view);
                if (log.Magic != log.ExpectedMagic)
                {
                    log.Clear();
                    log.Magic = log.ExpectedMagic;
                }
            }

            this.tag = tag;
        }

        /// <summary>
        /// Exports a ringlogger file to an instantiated TextWriter object for later writing to a file.
        /// </summary>
        /// <param name="writer">An instantiated TextWriter object to be used for writing.</param>
        public void WriteTo(TextWriter writer)
        {
            var start = log.NextIndex;
            for (uint i = 0; i < log.LineCount; ++i)
            {
                var entry = log[i + start];
                if (entry.Timestamp.IsEmpty)
                {
                    continue;
                }

                var text = entry.ToString();
                if (text == null)
                {
                    continue;
                }

                writer.WriteLine(text);
            }
        }

        private struct UnixTimestamp
        {
            public UnixTimestamp(long nanoSeconds)
            {
                Nanoseconds = nanoSeconds;
            }

            public bool IsEmpty => Nanoseconds == 0;

            public long Nanoseconds { get; }

            public override string ToString()
            {
                return DateTimeOffset.FromUnixTimeSeconds(Nanoseconds / 1000000000).LocalDateTime.ToString("yyyy'-'MM'-'dd HH':'mm':'ss'.'") + ((Nanoseconds % 1000000000).ToString() + "00000").Substring(0, 6);
            }
        }

        private struct Line
        {
            private const int MaxLineLength = 512;
            private const int OffsetTimeNs = 0;
            private const int OffsetLine = 8;

            private readonly MemoryMappedViewAccessor view;
            private readonly int start;

            public Line(MemoryMappedViewAccessor viewAccessor, uint index)
            {
                (view, start) = (viewAccessor, (int)(Log.HeaderBytes + (index * Bytes)));
            }

            public static int Bytes => MaxLineLength + OffsetLine;

            public UnixTimestamp Timestamp
            {
                get => new UnixTimestamp(view.ReadInt64(start + OffsetTimeNs));
                set => view.Write(start + OffsetTimeNs, value.Nanoseconds);
            }

            public string Text
            {
                get
                {
                    var textBytes = new byte[MaxLineLength];

                    view.ReadArray(start + OffsetLine, textBytes, 0, textBytes.Length);
                    int nullByte = Array.IndexOf<byte>(textBytes, 0);

                    if (nullByte <= 0)
                    {
                        return null;
                    }

                    return Encoding.UTF8.GetString(textBytes, 0, nullByte);
                }

                set
                {
                    if (value == null)
                    {
                        view.WriteArray(start + OffsetLine, new byte[MaxLineLength], 0, MaxLineLength);
                        return;
                    }

                    var textBytes = Encoding.UTF8.GetBytes(value);
                    var bytesToWrite = Math.Min(MaxLineLength - 1, textBytes.Length);

                    view.Write(start + OffsetLine + bytesToWrite, (byte)0);
                    view.WriteArray(start + OffsetLine, textBytes, 0, bytesToWrite);
                }
            }

            public override string ToString()
            {
                var time = Timestamp;
                if (time.IsEmpty)
                {
                    return null;
                }

                var text = Text;
                if (text == null)
                {
                    return null;
                }

                return string.Format("{0}: {1}", time, text);
            }
        }

        private struct Log
        {
            private const uint MaxLines = 2048;
            private const uint MagicNumber = 0xbadbabe;

            private const int OffsetMagic = 0;
            private const int OffsetNextIndex = 4;
            private const int OffsetLines = 8;

            private readonly MemoryMappedViewAccessor view;

            public Log(MemoryMappedViewAccessor view)
            {
                this.view = view;
            }

            public static int HeaderBytes => OffsetLines;

            public static int Bytes => (int)(HeaderBytes + (Line.Bytes * MaxLines));

            public uint ExpectedMagic => MagicNumber;

            public uint Magic
            {
                get => view.ReadUInt32(OffsetMagic);
                set => view.Write(OffsetMagic, value);
            }

            public uint NextIndex
            {
                get => view.ReadUInt32(OffsetNextIndex);
                set => view.Write(OffsetNextIndex, value);
            }

            public uint LineCount => MaxLines;

            public Line this[uint i] => new Line(view, i % MaxLines);

            public void Clear() => view.WriteArray(0, new byte[Bytes], 0, Bytes);
        }
    }
}
