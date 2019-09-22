using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Modules;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static LanguageManager LanguageManager { get; set; }

        public Form1()
        {
            InitializeComponent();

            LanguageManager = new LanguageManager("Languages");
            LanguageManager.SetLanguage("es-ES");

            Shown += (e, sender)        => LanguageManager.Register(this);
            FormClosed += (e, sender)   => LanguageManager.Unregister(this);

            var selectedItem = LanguageManager.Current;

            toolStripComboBox1.ComboBox.DataSource = LanguageManager.Languages;
            toolStripComboBox1.ComboBox.DisplayMember = "Name";
            toolStripComboBox1.ComboBox.SelectedItem = selectedItem;
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (LanguageManager.SetLanguage((toolStripComboBox1.ComboBox.SelectedItem as Language).Locale))
                LanguageManager.ApplyLocales();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            new Form2().Show();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            LanguageManager.ShowMessageBox("Errors.Test", "Etc.Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
