using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Modules
{
    public class MyContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .Select(p => base.CreateProperty(p, memberSerialization))
                        .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                   .Select(f => base.CreateProperty(f, memberSerialization)))
                        .ToList();
            props.ForEach(p => { p.Writable = true; p.Readable = true; });
            return props;
        }
    }

    public class LanguageManager
    {
        public List<Control> Controls { get; private set; } = new List<Control>();
        public List<Language> Languages { get; private set; } = new List<Language>();

        /// <summary>
        /// Current language use
        /// </summary>
        private Language current = null;

        /// <summary>
        /// Path to locale file
        /// </summary>
        private string path = string.Empty;

        /// <summary>
        /// Initialize manger language
        /// </summary>
        /// <param name="folder"></param>
        public LanguageManager(string folder)
        {
            path = folder;
            Load();
        }

        /// <summary>
        /// Apply locale from one control
        /// </summary>
        private void ApplyLocale(Control control)
        {
            try
            {
                JObject obj = JObject.Parse(current.Json);
                string form = obj[control.Name].ToString();

                foreach (var grid in control.Controls.OfType<DataGridView>())
                    foreach (DataGridViewColumn column in grid.Columns)
                        column.HeaderText = obj["Grid"][column.Name].ToString();

                JsonConvert.PopulateObject(form, control, new JsonSerializerSettings()
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    ContractResolver = new MyContractResolver()
                });
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
            if (current == null)
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
                if (!Directory.Exists(path))
                    return;

                foreach (var file in Directory.GetFiles(path))
                {
                    var json = File.ReadAllText(file);
                    var language = JsonConvert.DeserializeObject<Language>(json);

                    language.Json = json;
                    Languages.Add(language);
                }
            }
            catch { }
        }

        /// <summary>
        /// Get all locale name
        /// </summary>
        /// <returns>Loaded locale name array</returns>
        public string[] GetNames()
        {
            return Languages.Select(u => u.Name).ToArray();
        }

        /// <summary>
        /// Get index of the current language
        /// </summary>
        /// <returns>Index of current language is defined otherwise 0</returns>
        public int GetIndex()
        {
            return current != null ? Languages.IndexOf(current) : 0;
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
                str = JObject.Parse(current.Json)
                    .SelectToken(key)
                    .ToString();                
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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

            if (current != null)
                ApplyLocale(control);

            return true;
        }

        /// <summary>
        /// Change current language
        /// </summary>
        /// <param name="name">Language to set</param>
        /// <returns>Update language result</returns>
        public bool SetLanguage(string name)
        {
            var language = Languages.SingleOrDefault(u => u.Name == name);
            if (language == null)
                return false;

            current = language;
            return true;
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
