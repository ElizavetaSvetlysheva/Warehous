using System.Windows;
using System.Windows.Media;

namespace Warehouse.Views
{
    public partial class InputDialog : Window
    {
        
        public bool RequireNonEmpty { get; set; } = false;

        public string Result { get; private set; }

        private static readonly Brush _errorBrush =
            new SolidColorBrush(Color.FromRgb(0xE8, 0x11, 0x23));

        public InputDialog(string prompt, string defaultText = "")
        {
            InitializeComponent();
            lblPrompt.Text = prompt;
            txtInput.Text  = defaultText;

            
            txtInput.TextChanged += (s, e) => txtInput.BorderBrush = SystemColors.ControlDarkBrush;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (RequireNonEmpty && string.IsNullOrWhiteSpace(txtInput.Text))
            {
                txtInput.BorderBrush = _errorBrush;
                txtInput.ToolTip     = "Поле не может быть пустым";
                txtInput.Focus();
                return;
            }

            Result       = txtInput.Text;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}