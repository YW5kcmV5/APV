using System;
using System.Windows.Forms;

namespace APV.Math.Tests.Primitive3D
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            primitive3DPanel1.AngleX += (float)System.Math.PI/36f;
            primitive3DPanel1.Invalidate();
        }

        private void ButtonRotateY_Click(object sender, EventArgs e)
        {
            primitive3DPanel1.AngleY += (float)System.Math.PI / 36f;
            primitive3DPanel1.Invalidate();
        }

        private void ButtonRotateZ_Click(object sender, EventArgs e)
        {
            primitive3DPanel1.AngleZ += (float)System.Math.PI / 36f;
            primitive3DPanel1.Invalidate();
        }

        private void buttonPauseResume_Click(object sender, EventArgs e)
        {
            primitive3DPanel1.Pause = (!primitive3DPanel1.Pause);
            primitive3DPanel1.Invalidate();
        }
    }
}