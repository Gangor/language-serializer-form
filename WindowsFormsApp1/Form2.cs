using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            
            Shown += (e, sender)        => Form1.LanguageManager.Register(this);
            FormClosed += (e, sender)   => Form1.LanguageManager.Unregister(this);
        }
    }
}
