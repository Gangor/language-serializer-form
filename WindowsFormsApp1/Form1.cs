using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public List<Language> Langs { get; set; }
        public Language Language { get; set; }

        public Form1()
        {
            InitializeComponent();

            /// Load language index info
            {
                string path = "languages/index.json";

                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);

                    Langs = JsonConvert.DeserializeObject<List<Language>>(json);

                    /// Fill combobox languages
                    for (int i = 0; i < Langs.Count; i++)
                        comboBox1.Items.Add(Langs[i].Name);
                }
            }

            comboBox1.SelectedIndex = 0;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Language = Langs[comboBox1.SelectedIndex];

            /// Load language properties
            {
                string path = $"languages/{Language.Flag}.json";

                if (File.Exists(path))
                {
                    /// Merge property inside current object
                    JsonConvert.PopulateObject(File.ReadAllText(path), this);
                    Refresh();
                }
                else throw new System.Exception("[Critical] File not found !!");
            }
        }
    }
}
