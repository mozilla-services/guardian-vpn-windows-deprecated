// <copyright file="LogWindow.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Log window, used for debugging.
    /// </summary>
    public partial class LogWindow : Window
    {
        private static LogWindow globalLogWindow;
        private readonly Thread logDumpThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogWindow"/> class.
        /// </summary>
        public LogWindow()
        {
            DataContext = Manager.MainWindowViewModel;
            InitializeComponent();
            logDumpThread = new Thread(() =>
            {
                var cursor = WireGuard.Ringlogger.CursorAll;
                while (Thread.CurrentThread.IsAlive)
                {
                    var lines = ErrorHandling.ErrorHandler.Ringlogger.FollowFromCursor(ref cursor);
                    if (lines.Count > 0)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            foreach (var line in lines)
                            {
                                logView.Items.Add(line);
                            }

                            while (logView.Items.Count > 8192)
                            {
                                logView.Items.RemoveAt(0);
                            }

                            var scrollViewer = FindScrollViewer(logView);
                            if (scrollViewer != null && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                            {
                                scrollViewer.ScrollToBottom();
                            }
                        }));
                    }

                    Thread.Sleep(300);
                }
            });
            logDumpThread.Start();
        }

        /// <summary>
        /// Brings the log window to focus if already running, otherwise creates a new one.
        /// </summary>
        public static void ShowLog()
        {
            if (globalLogWindow != null)
            {
                globalLogWindow.Show();
                if (globalLogWindow.WindowState == WindowState.Minimized)
                {
                    globalLogWindow.WindowState = WindowState.Normal;
                }

                globalLogWindow.Activate();
                return;
            }

            globalLogWindow = new LogWindow();
            ShowLog();
        }

        private static ScrollViewer FindScrollViewer(DependencyObject dependencyObject)
        {
            if (dependencyObject is ScrollViewer)
            {
                return dependencyObject as ScrollViewer;
            }

            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(dependencyObject); i < count; ++i)
            {
                var scrollViewer = FindScrollViewer(VisualTreeHelper.GetChild(dependencyObject, i));
                if (scrollViewer != null)
                {
                    return scrollViewer;
                }
            }

            return null;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            logDumpThread.Abort();
            if (this == globalLogWindow)
            {
                globalLogWindow = null;
            }
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            var items = new List<WireGuard.Ringlogger.Entry>();

            items.AddRange(logView.SelectedItems.Cast<WireGuard.Ringlogger.Entry>().ToArray());

            var logEntries = items.OrderBy(i => i.Timestamp).ToList();
            foreach (var entry in logEntries)
            {
                sb.AppendLine(string.Format("{0}: {1}", entry.Timestamp, entry.Message));
            }

            try
            {
                Clipboard.SetText(sb.ToString());
            }
            catch
            {
            }
        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            logView.SelectAll();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            using (var saveDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = string.Concat(Manager.TranslationService.GetString("viewlog-save-dialog-filter"), "|*.txt"),
                Title = Manager.TranslationService.GetString("viewlog-save-dialog-title"),
                FileName = ProductConstants.DefaultViewLogSaveFilename,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            })
            {
                if (saveDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK || saveDialog.FileName == string.Empty)
                {
                    return;
                }

                try
                {
                    var logTxt = File.CreateText(saveDialog.FileName);
                    ErrorHandling.ErrorHandler.Ringlogger.WriteTo(logTxt);
                    logTxt.Close();
                }
                catch (Exception exception)
                {
                    ErrorHandling.ErrorHandler.Handle(string.Concat(Manager.TranslationService.GetString("viewlog-save-error"), ": ", exception.Message), ErrorHandling.LogLevel.Error);
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView gridView = logView.View as GridView;

            var width = logView.ActualWidth - (SystemParameters.VerticalScrollBarWidth * 2);
            var timestampColumn = 0.20;
            var logTextColumn = 0.80;

            gridView.Columns[0].Width = width * timestampColumn;
            gridView.Columns[1].Width = width * logTextColumn;
        }
    }
}
