using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Help to manage the lifecycle of each login session
    /// </summary>
    public class LoginSessionManager
    {
        private CancellationTokenSource cancelSource;

        /// <summary>
        /// Start a new login session.
        /// </summary>
        public void StartNewSession()
        {
            CancelCurrentSession();

            cancelSource = new CancellationTokenSource();

            Login login = new Login(LoginResultCallback);
            login.StartLogin(cancelSource.Token);
        }

        /// <summary>
        /// Cancel the current login session, if any.
        /// </summary>
        public void CancelCurrentSession()
        {
            // No ongoing session
            if (cancelSource == null)
            {
                return;
            }

            // Already been cancelled
            if (cancelSource.IsCancellationRequested)
            {
                return;
            }

            cancelSource.Cancel();
        }

        private void LoginResultCallback(object sender, LoginState state)
        {
            cancelSource.Dispose();
            cancelSource = null;
        }
    }
}
