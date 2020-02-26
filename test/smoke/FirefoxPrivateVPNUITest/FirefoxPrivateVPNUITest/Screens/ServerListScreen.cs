// <copyright file="ServerListScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using System;
    using System.Linq;
    using System.Threading;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for Setting screen.
    /// </summary>
    internal class ServerListScreen
    {
        private AppiumWebElement backButton;
        private AppiumWebElement title;
        private AppiumWebElement serverListView;
        private AppiumWebElement selectedServerCountry;
        private AppiumWebElement selectedServerCity;
        private AppiumWebElement selectedServerCountryCityList;
        private AppiumWebElement scrollDownButton;
        private WindowsDriver<WindowsElement> vpnSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerListScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public ServerListScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            this.vpnSession = vpnSession;
            this.scrollDownButton = vpnSession.FindElementByAccessibilityId("PART_LineDownButton");
            this.backButton = vpnSession.FindElementByName("Back");
            this.title = vpnSession.FindElementByName("Connection");
            this.serverListView = vpnSession.FindElementByAccessibilityId("CountryServerList");
            var countryList = this.serverListView.FindElementsByClassName("ListBoxItem");
            foreach (var country in countryList)
            {
                if (this.selectedServerCountry != null && this.selectedServerCity != null)
                {
                    break;
                }

                if (country.Displayed)
                {
                    this.selectedServerCountry = country;
                    var countryExpander = country.FindElementByClassName("Expander");
                    var expandedState = countryExpander.FindElementByClassName("Button");
                    if (!expandedState.Selected)
                    {
                        expandedState.Click();
                    }

                    this.selectedServerCountryCityList = countryExpander.FindElementByAccessibilityId("CityServerList");
                    var cityList = this.selectedServerCountryCityList.FindElementsByClassName("ListBoxItem");
                    foreach (var city in cityList)
                    {
                        var citySelection = city.FindElementByClassName("RadioButton");
                        if (citySelection.Selected)
                        {
                            this.selectedServerCity = city;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get title on server selection screen.
        /// </summary>
        /// <returns>The title string.</returns>
        public string GetTitle()
        {
            return this.title.Text;
        }

        /// <summary>
        /// Click the Back button.
        /// </summary>
        public void ClickBackButton()
        {
            this.backButton.Click();
        }

        /// <summary>
        /// Get the selected country name.
        /// </summary>
        /// <returns>Selected country name.</returns>
        public string GetSelectedCountry()
        {
            return this.selectedServerCountry.GetAttribute("Name");
        }

        /// <summary>
        /// Get selected city name.
        /// </summary>
        /// <returns>Selected city name.</returns>
        public string GetSelectedCity()
        {
            return this.selectedServerCity.GetAttribute("Name");
        }

        /// <summary>
        /// Random select a different city server in US.
        /// </summary>
        /// <param name="city">The city we want to select.</param>
        public void RandomSelectDifferentCityServer(string city = null)
        {
            var cityList = this.selectedServerCountryCityList.FindElementsByClassName("RadioButton");
            Func<int, bool> randomPickCondition = (i) =>
            {
                string currentCityName = cityList[i].GetAttribute("Name");
                if (city == null)
                {
                    return currentCityName != this.GetSelectedCity();
                }

                return currentCityName != this.GetSelectedCity() && currentCityName.Contains(city);
            };

            int randomIndex = Utils.RandomSelectIndex(Enumerable.Range(0, cityList.Count), randomPickCondition);
            var randomCity = cityList[randomIndex];
            while (!randomCity.Displayed)
            {
                this.scrollDownButton.Click();
                this.vpnSession.Mouse.MouseDown(null);

                // scroll down for 1 second
                Thread.Sleep(TimeSpan.FromSeconds(1));
                this.vpnSession.Mouse.MouseUp(null);
            }

            // In some extrem situation, the radio button might not fully show up on the screen and we need to click the scroll down button
            // one more time to let it fully show up.
            this.scrollDownButton.Click();

            // Click radio button to select the server
            this.selectedServerCity = randomCity;
            var citySelection = this.selectedServerCity.FindElementByClassName("RadioButton");
            citySelection.Click();
        }
    }
}
