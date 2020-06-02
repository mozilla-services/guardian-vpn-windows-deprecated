// <copyright file="LoginSessionManager.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Help to manage the lifecycle of each login session.
    /// </summary>
    public class LoginSessionManager
    {
        private Dictionary<Login, CancellationTokenSource> sessions = new Dictionary<Login, CancellationTokenSource>();

        /// <summary>
        /// Start a new login session.
        /// </summary>
        public void StartNewSession()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CancelCurrentSession();

                Login login = new Login(LoginResultCallback);
                CancellationTokenSource tokenSource = new CancellationTokenSource();

                sessions.Add(login, tokenSource);

                if (!login.StartLogin(tokenSource.Token))
                {
                    CancelCurrentSession();
                }
            });
        }

        /// <summary>
        /// Cancel the current login session, if any.
        /// </summary>
        public void CancelCurrentSession()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var session in sessions)
                {
                    session.Value.Cancel();
                    session.Value.Dispose();
                }

                sessions.Clear();
            });
        }

        private void LoginResultCallback(object sender, Login session, LoginState state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (sessions.ContainsKey(session))
                {
                    sessions[session].Dispose();
                    sessions.Remove(session);
                }
            });
        }
    }
}
