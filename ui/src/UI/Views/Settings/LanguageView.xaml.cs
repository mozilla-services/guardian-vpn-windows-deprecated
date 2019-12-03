// <copyright file="LanguageView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for LanguageView.xaml.
    /// </summary>
    public partial class LanguageView : UserControl
    {
        private UserControl parentView;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageView"/> class.
        /// </summary>
        /// <param name="parentView">Parent view control of the <see cref="LanguageView"/> instance.</param>
        public LanguageView(UserControl parentView)
        {
            this.parentView = parentView;
            InitializeComponent();
            DataContext = Manager.MainWindowViewModel;

            InitializeAdditionalLanguagesList();
        }

        /// <summary>
        /// Gets the default langauge of the application.
        /// </summary>
        public CultureInfo DefaultCulture
        {
            get
            {
                return Manager.TranslationService.DefaultCulture;
            }
        }

        /// <summary>
        /// Gets the currently used language of the application.
        /// </summary>
        public CultureInfo CurrentCulture
        {
            get
            {
                return Manager.TranslationService.Culture;
            }
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(parentView, MainWindow.SlideDirection.Right);
        }

        private void InitializeAdditionalLanguagesList()
        {
            var additionalLanguagesList = new List<CultureInfo>();

            foreach (var language in Application.Current.Resources["SupportedLanguages"] as string[])
            {
                if (DefaultCulture.Name != language)
                {
                    additionalLanguagesList.Add(new CultureInfo(language));
                }
            }

            Manager.MainWindowViewModel.AdditionalLanguagesList = additionalLanguagesList;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected language
            CultureInfo selectedLanguage;
            var selectedItem = ((sender as RadioButton)?.Tag as ListViewItem)?.DataContext;

            if (selectedItem != null)
            {
                selectedLanguage = selectedItem as CultureInfo;
            }
            else
            {
                selectedItem = (sender as RadioButton)?.Tag as string;

                if (selectedItem == null || !selectedItem.Equals("Default"))
                {
                    return;
                }

                selectedLanguage = DefaultCulture;
            }

            // Load the new translations
            Manager.TranslationService.ConfigureCulture(selectedLanguage.Name);

            // Refresh DataContext to refresh language on the current view
            DataContext = null;
            DataContext = Manager.MainWindowViewModel;
            parentView.DataContext = null;
            parentView.DataContext = Manager.MainWindowViewModel;

            // Refresh the strings in toasts
            Manager.ToastManager.RefreshAllToasts();

            // Save the new preferred language to user settings
            var languageSettings = Manager.Settings.Language;
            languageSettings.PreferredLanguage = selectedLanguage.Name;
            Manager.Settings.Language = languageSettings;
        }
    }
}
