using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Login
{
    public partial class FormImagen : Form
    {
        private float zoom = 1f;
        private Image imagenOriginal;

        public FormImagen(string ruta)
        {
            InitializeComponent();

            if (System.IO.File.Exists(ruta))
            {
                imagenOriginal = Image.FromFile(ruta);
                picZoom.Image = imagenOriginal;
                picZoom.SizeMode = PictureBoxSizeMode.Zoom;
            }

            this.MouseWheel += FormImagen_MouseWheel;
            picZoom.MouseWheel += FormImagen_MouseWheel;
        }

        private void FormImagen_MouseWheel(object sender, MouseEventArgs e)
        {
            if (imagenOriginal == null) return;

            if (e.Delta > 0)
                zoom += 0.1f;
            else
                zoom -= 0.1f;

            if (zoom < 0.2f) zoom = 0.2f;
            if (zoom > 5f) zoom = 5f;

            picZoom.Width = (int)(imagenOriginal.Width * zoom);
            picZoom.Height = (int)(imagenOriginal.Height * zoom);

            picZoom.Image = imagenOriginal;
            picZoom.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void picZoom_Click(object sender, EventArgs e)
        {

        }
    }
}
