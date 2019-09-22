using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Modules
{
    public class LanguageManager
    {
        public List<Control> Controls { get; private set; }
        public List<Language> Languages { get; private set; }

        /// <summary>
        /// Current language use
        /// </summary>
        public Language Current = null;

        /// <summary>
        /// Path to locale file
        /// </summary>
        private readonly string Path = string.Empty;

        /// <summary>
        /// Initialize manger language
        /// </summary>
        /// <param name="path"></param>
        public LanguageManager(string path)
        {
            Path = path;
            Controls = new List<Control>();
            Languages = new List<Language>();
            //
            Load();
        }

        /// <summary>
        /// Apply locale from one control
        /// </summary>
        private void ApplyLocale(Control control)
        {
            try
            {
                JObject obj = JObject.Parse(Current.Json);

                foreach (var grid in control.Controls.OfType<DataGridView>())
                    foreach (DataGridViewColumn column in grid.Columns)
                        column.HeaderText = obj["Grid"][column.Name].ToString();

                JsonConvert.PopulateObject(obj[control.Name].ToString(), control, 
                    new JsonSerializerSettings() { ContractResolver = new ContractAllResolver() });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Apply local to all control assigned
        /// </summary>
        public void ApplyLocales()
        {
            if (Current == null)
                return;

            foreach (var control in Controls)
                ApplyLocale(control);
        }

        /// <summary>
        /// Load available language
        /// </summary>
        private void Load()
        {
            try
            {
                if (!Directory.Exists(Path))
                    return;

                foreach (var file in Directory.GetFiles(Path))
                {
                    var json = File.ReadAllText(file);
                    var language = JsonConvert.DeserializeObject<Language>(json);

                    language.Json = json;
                    Languages.Add(language);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Get index of the current language
        /// </summary>
        /// <returns>Index of current language is defined otherwise 0</returns>
        public int GetIndex()
        {
            return Current != null ? Languages.IndexOf(Current) : 0;
        }

        /// <summary>
        /// Get string key
        /// </summary>
        /// <param name="key">Token path (Sample : 'Errors.Test')</param>
        /// <returns>Value of token is find otherwise 'null'</returns>
        public string GetString(string key)
        {
            string str = "null";

            try
            {
                str = JObject.Parse(Current.Json)
                    .SelectToken(key)
                    .ToString();
            }
            catch
            { }

            return str;
        }

        /// <summary>
        /// Link a control
        /// </summary>
        /// <param name="control">Control to add</param>
        /// <returns>Register result</returns>
        public bool Register(Control control)
        {
            if (Controls.Exists(u => u.Handle == control.Handle))
                return false;

            Controls.Add(control);

            if (Current != null)
                ApplyLocale(control);

            return true;
        }

        /// <summary>
        /// Change current language
        /// </summary>
        /// <param name="locale">Language to set</param>
        /// <returns>Update language result</returns>
        public bool SetLanguage(string locale)
        {
            var language = Languages.SingleOrDefault(u => u.Locale == locale);
            if (language == null)
                return false;

            Current = language;
            return true;
        }

        /// <summary>
        /// Show message box with localization
        /// </summary>
        /// <param name="text">Token text</param>
        /// <param name="caption">Token caption</param>
        /// <param name="buttons"></param>
        /// <param name="icon"></param>
        public void ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(GetString(text), GetString(caption), buttons, icon);
        }

        /// <summary>
        /// Link a control
        /// </summary>
        /// <param name="control">Control to remove</param>
        /// <returns>Unregister result</returns>
        public bool Unregister(Control control)
        {
            var c = Controls.SingleOrDefault(u => u.Handle == control.Handle);
            if (c == null)
                return false;

            Controls.Remove(c);
            return true;
        }
    }
}
