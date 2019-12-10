// <copyright file="UpdateView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for UpdateView.xaml.
    /// </summary>
    public partial class UpdateView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateView"/> class.
        /// </summary>
        public UpdateView()
        {
            DataContext = Manager.MainWindowViewModel;
            InitializeComponent();
        }

        private void NavigateSettings(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new SettingsView(this), MainWindow.SlideDirection.Up);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Button updateButton = sender as Button;
            updateButton.IsEnabled = false;

            // Create an update toast
            var toast = new UI.Components.Toast.Toast(UI.Components.Toast.Style.Info, new ErrorHandling.UserFacingMessage("update-update-started"), priority: UI.Components.Toast.Priority.Important);
            Manager.ToastManager.Show(toast);
            ErrorHandling.ErrorHandler.WriteToLog(Manager.TranslationService.GetString("update-update-started"), ErrorHandling.LogLevel.Info);

            var updateTask = Task.Run(async () =>
            {
                var updateResult = await Update.Update.Run(ProductConstants.GetNumericVersion());

                switch (updateResult)
                {
                    case Update.Update.UpdateResult.DisconnectTimeout:
                    case Update.Update.UpdateResult.GeneralFailure:
                    case Update.Update.UpdateResult.HttpError:
                    case Update.Update.UpdateResult.InvalidSignature:
                    case Update.Update.UpdateResult.SuccessNoUpdate:
                    case Update.Update.UpdateResult.RunFailure:
                        ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("update-update-failed"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                        break;

                    case Update.Update.UpdateResult.Georestricted:
                        ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("update-update-failed-georestricted"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                        break;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    updateButton.IsEnabled = true;
                });
            });
        }
    }
}
