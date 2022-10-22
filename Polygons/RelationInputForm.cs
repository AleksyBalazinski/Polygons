namespace Polygons
{
    public partial class RelationInputForm : Form
    {
        public string Input
        {
            get => textBoxInput.Text;
            set => textBoxInput.Text = value;
        }
        public RelationInputForm()
        {
            InitializeComponent();
        }
    }
}
