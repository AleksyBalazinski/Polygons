using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
