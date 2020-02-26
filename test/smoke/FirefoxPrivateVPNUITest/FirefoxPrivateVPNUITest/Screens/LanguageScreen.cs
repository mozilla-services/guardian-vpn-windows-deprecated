// <copyright file="LanguageScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using System.Linq;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for Language screen.
    /// </summary>
    internal class LanguageScreen
    {
        private AppiumWebElement backButton;
        private AppiumWebElement title;
        private AppiumWebElement defaultLanguageTitle;
        private AppiumWebElement defaultLanguageRadioButton;
        private AppiumWebElement defaultLanguageDetails;
        private AppiumWebElement additionalLanguagesListTitle;
        private AppiumWebElement additionalLanguagesListView;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public LanguageScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            var languageView = vpnSession.FindElementByClassName("LanguageView");
            this.backButton = languageView.FindElementByName("Back");
            this.title = languageView.FindElementByName("Language");
            var languagePane = languageView.FindElementByClassName("ScrollViewer");
            this.defaultLanguageTitle = languagePane.FindElementByName("Default");
            var defaultLanguagesElements = languagePane.FindElementsByName("English (United States)");
            this.defaultLanguageRadioButton = defaultLanguagesElements[0];
            this.defaultLanguageDetails = defaultLanguagesElements[1];
            this.additionalLanguagesListTitle = languagePane.FindElementByName("Additional");
            this.additionalLanguagesListView = languagePane.FindElementByAccessibilityId("SupportedLanguagesList");
        }

        /// <summary>
        /// Get title on Language screen.
        /// </summary>
        /// <returns>The title string.</returns>
        public string GetTitle()
        {
            return this.title.Text;
        }

        /// <summary>
        /// Get the text on default language pane.
        /// </summary>
        /// <returns>The text on default language pane.</returns>
        public string GetDefaultLanguageTitle()
        {
            return this.defaultLanguageTitle.Text;
        }

        /// <summary>
        /// Get the text on additional language pane.
        /// </summary>
        /// <returns>The text on additional language pane.</returns>
        public string GetAdditionalLanguagesPaneTitle()
        {
            return this.additionalLanguagesListTitle.Text;
        }

        /// <summary>
        /// Get the text on default language radio button.
        /// </summary>
        /// <returns>The text on default language radio button.</returns>
        public string GetTextOnDefaultLanguageRadioButton()
        {
            return this.defaultLanguageRadioButton.Text;
        }

        /// <summary>
        /// Get the details on default language radio button.
        /// </summary>
        /// <returns>The details on default language radio button.</returns>
        public string GetDetailsOnDefaultLanguageRadioButton()
        {
            return this.defaultLanguageDetails.Text;
        }

        /// <summary>
        /// Click the Back button.
        /// </summary>
        public void ClickBackButton()
        {
            this.backButton.Click();
        }

        /// <summary>
        /// Is default language radio button checked.
        /// </summary>
        /// <returns>Default language radio button checked or not.</returns>
        public bool IsDefaultLanguageRadioButtonChecked()
        {
            return this.defaultLanguageRadioButton.Selected;
        }

        /// <summary>
        /// Random pick a language from list.
        /// </summary>
        public void RandomPickAdditionalLanguage()
        {
            var additionalLanguages = this.additionalLanguagesListView.FindElementsByClassName("ListBoxItem");
            int randomIndex = Utils.RandomSelectIndex(Enumerable.Range(0, additionalLanguages.Count), (i) => true);
            var randomLanguage = additionalLanguages[randomIndex];
            var languageSelection = randomLanguage.FindElementByClassName("RadioButton");
            languageSelection.Click();
        }

        /// <summary>
        /// Click default language radio button.
        /// </summary>
        public void ClickDefaultLanguageRadioButton()
        {
            this.defaultLanguageRadioButton.Click();
        }
    }
}
