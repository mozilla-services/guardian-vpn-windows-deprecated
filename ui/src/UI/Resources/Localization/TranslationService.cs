// <copyright file="TranslationService.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Fluent.Net;

namespace FirefoxPrivateNetwork.UI.Resources.Localization
{
    /// <summary>
    /// Manages all the localized strings/language translations in the application.
    /// </summary>
    public class TranslationService
    {
        private CultureInfo culture;
        private IEnumerable<MessageContext> contexts;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationService"/> class.
        /// </summary>
        public TranslationService()
        {
            ConfigureCulture(Manager.Settings.Language.PreferredLanguage);
        }

        /// <summary>
        /// Gets the current locale used in the application.
        /// </summary>
        public string CurrentLocale => contexts.First().Locales.First();

        /// <summary>
        /// Gets the current language used in the application.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                if (culture == null)
                {
                    culture = new CultureInfo(CurrentLocale);
                }

                return culture;
            }
        }

        /// <summary>
        /// Gets the default language of the application if none is specified. Set to the user's OS language if supported, otherwise to en-US.
        /// </summary>
        public CultureInfo DefaultCulture { get; private set; } = new CultureInfo("en-US");

        /// <summary>
        /// Gets the user's installed language on their operating system.
        /// </summary>
        private CultureInfo OSLanguage => CultureInfo.InstalledUICulture;

        /// <summary>
        /// Optional Argument list for Fluent translation strings.
        /// </summary>
        /// <param name="name">Name of the Fluent parameter.</param>
        /// <param name="value">Value of the Fluent parameter.</param>
        /// <param name="args">Additional Fluent parameters.</param>
        /// <returns>A dictionary of key/value pairs for Fluent parameters.</returns>
        public static Dictionary<string, object> Args(string name, object value, params object[] args)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (args.Length % 2 != 0)
            {
                throw new ArgumentException("Expected a comma separated list of name, value arguments but the number of arguments is not a multiple of two", nameof(args));
            }

            var argsDic = new Dictionary<string, object>
            {
                { name, value },
            };

            for (int i = 0; i < args.Length; i += 2)
            {
                name = args[i] as string;

                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException($"Expected the argument at index {i} to be a non-empty string", nameof(args));
                }

                value = args[i + 1];
                if (value == null)
                {
                    throw new ArgumentNullException("args", $"Expected the argument at index {i + 1} to be a non-null value");
                }

                argsDic.Add(name, value);
            }

            return argsDic;
        }

        /// <summary>
        /// Configures the language of the application.
        /// </summary>
        /// <param name="lang">Name of the culture to be used.</param>
        public void ConfigureCulture(string lang)
        {
            ConfigureDefaultCulture();
            var cultureName = DefaultCulture.Name;

            if (IsCultureSupported(lang))
            {
                cultureName = lang;
            }

            contexts = new MessageContext[]
            {
                GetMessages(cultureName),
            };

            culture = new CultureInfo(cultureName);

            // Refresh system tray, if available
            if (Manager.TrayIcon != null)
            {
                Manager.TrayIcon.SetupMenu(true);
            }
        }

        /// <summary>
        /// Gets the localized string by Id.
        /// </summary>
        /// <param name="id">Id of the targetted Fluent string.</param>
        /// <param name="args">Argument list of Fluent parameters for the targetted string.</param>
        /// <param name="errors">Collection of Fluent errors.</param>
        /// <returns>The targetted localized string.</returns>
        public string GetString(string id, IDictionary<string, object> args = null, ICollection<FluentError> errors = null)
        {
            foreach (var context in contexts)
            {
                var msg = context.GetMessage(id);
                if (msg != null)
                {
                    return context.Format(msg, args, errors);
                }
            }

            return string.Empty;
        }

        private void ConfigureDefaultCulture()
        {
            if (IsCultureSupported(OSLanguage.Name))
            {
                DefaultCulture = OSLanguage;
            }
        }

        private bool IsCultureSupported(string cultureName)
        {
            var supportedLanguages = new List<string>(Application.Current.Resources["SupportedLanguages"] as string[]);
            return supportedLanguages.Any(lang => lang == cultureName);
        }

        private MessageContext GetMessages(string lang)
        {
            var translationResource = Application.Current.FindResource(lang).ToString();

            using (var sr = new StreamReader(Application.GetResourceStream(new Uri(translationResource)).Stream))
            {
                var options = new MessageContextOptions { UseIsolating = false };
                var mc = new MessageContext(lang, options);
                var errors = mc.AddMessages(sr);

                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }

                return mc;
            }
        }
    }
}
