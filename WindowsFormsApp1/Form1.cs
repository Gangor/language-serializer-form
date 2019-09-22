using System.Windows.Forms;
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
            //LanguageManager.SetLanguage(opt.config.language);

            Shown += (e, sender)        => LanguageManager.Register(this);
            FormClosed += (e, sender)   => LanguageManager.Unregister(this);

            comboBox1.Items.AddRange(LanguageManager.GetNames());
            comboBox1.SelectedIndex = LanguageManager.GetIndex();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (LanguageManager.SetLanguage(comboBox1.Text))
                LanguageManager.ApplyLocales();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            new Form2().Show();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show(LanguageManager.GetString("Errors.Test"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
