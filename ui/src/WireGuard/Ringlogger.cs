// <copyright file="Ringlogger.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

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

namespace FirefoxPrivateNetwork.WireGuard
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
        /// Gets or sets the cursor which encompasses all log entries.
        /// </summary>
        public static uint CursorAll { get; set; } = uint.MaxValue;

        /// <summary>
        /// Writes a line to the ringlogger log.
        /// </summary>
        /// <param name="line">Text to write to the log.</param>
        public void Write(string line)
        {
            var time = UnixTimestamp.Now;
            var entry = log[log.InsertNextIndex() - 1];

            entry.Timestamp = UnixTimestamp.Empty;
            entry.Text = null;
            entry.Text = string.Format("[{0}] {1}", tag, line.Trim());
            entry.Timestamp = time;
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

        /// <summary>
        /// Retrieves a list of log entries starting based on the provided cursor.
        /// </summary>
        /// <param name="cursor">Cursor position from which the read will occur.</param>
        /// <returns>List of Entry objects containing timestamps and log message contents.</returns>
        public List<Entry> FollowFromCursor(ref uint cursor)
        {
            var lines = new List<Entry>((int)log.LineCount);
            var i = cursor;
            var all = cursor == CursorAll;

            if (all)
            {
                i = log.NextIndex;
            }

            for (uint l = 0; l < log.LineCount; ++l, ++i)
            {
                if (!all && i % log.LineCount == log.NextIndex % log.LineCount)
                {
                    break;
                }

                var entry = log[i];
                if (entry.Timestamp.IsEmpty)
                {
                    if (all)
                    {
                        continue;
                    }

                    break;
                }

                cursor = (i + 1) % log.LineCount;
                var text = entry.Text.ToString();
                var timestamp = entry.Timestamp.ToString();
                if (text == null || timestamp == null)
                {
                    continue;
                }

                lines.Add(new Entry { Timestamp = timestamp, Message = text });
            }

            return lines;
        }

        /// <summary>
        /// Log entry structure, containing a timestamp and a message.
        /// </summary>
        public struct Entry
        {
            /// <summary>
            /// Gets or sets the timestamp in text form indicating when the log event has occured.
            /// </summary>
            public string Timestamp { get; set; }

            /// <summary>
            /// Gets or sets the log message.
            /// </summary>
            public string Message { get; set; }
        }

        private struct UnixTimestamp
        {
            public UnixTimestamp(long nanoSeconds)
            {
                Nanoseconds = nanoSeconds;
            }

            public static UnixTimestamp Empty => new UnixTimestamp(0);

            public static UnixTimestamp Now
            {
                get
                {
                    var now = DateTimeOffset.UtcNow;
                    var ns = (now.Subtract(DateTimeOffset.FromUnixTimeSeconds(0)).Ticks * 100) % 1000000000;

                    return new UnixTimestamp((now.ToUnixTimeSeconds() * 1000000000) + ns);
                }
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

            public unsafe uint InsertNextIndex()
            {
                byte* pointer = null;
                view.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
                var ret = (uint)Interlocked.Increment(ref Unsafe.AsRef<int>(pointer + OffsetNextIndex));
                view.SafeMemoryMappedViewHandle.ReleasePointer();
                return ret;
            }
        }
    }
}
