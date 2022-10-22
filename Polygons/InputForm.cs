namespace Polygons
{
    public partial class InputForm : Form
    {
        public string Input
        {
            get => textBoxInput.Text;
            set => textBoxInput.Text = value;
        }
        public InputForm()
        {
            InitializeComponent();
        }
    }
}
