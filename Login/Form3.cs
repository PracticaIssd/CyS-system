using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Login
{
    public partial class Form3 : Form
    {
        string conexion =
            @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";
        public Form3()
        {
            InitializeComponent();
        }
        private void btLogin_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT Nombre FROM Usuarios WHERE Usuario=@user AND Password=@pass";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", tbUsuario.Text);
                    cmd.Parameters.AddWithValue("@pass", tbPassword.Text);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        string nombreUsuario = resultado.ToString();

                        MessageBox.Show("Bienvenido " + nombreUsuario);

                        Form5 frm = new Form5();
                        frm.Show();

                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos");

                        tbPassword.Clear();
                        tbUsuario.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btAgregar_Click(object sender, EventArgs e)
        {
            Form4 frm = new Form4();
            frm.Show();
            this.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }


}