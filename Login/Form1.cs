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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Login
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbNombre.Text) ||
                string.IsNullOrWhiteSpace(tbDni.Text) ||
                string.IsNullOrWhiteSpace(tbTel.Text) ||
                string.IsNullOrWhiteSpace(tbGmail.Text) ||
                string.IsNullOrWhiteSpace(tbUbicacion.Text))
            {
                MessageBox.Show(
                    "Completar todos los campos.",
                    "Campos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }
            string conexion =
                @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = "INSERT INTO Clientes (Nombre, DNI, Telefono, Gmail, Ubicacion) " +
                                   "VALUES (@Nombre, @DNI, @Telefono, @Gmail, @Ubicacion)";

                    SqlCommand cmd = new SqlCommand(query, conn);
 
                    cmd.Parameters.AddWithValue("@Nombre", tbNombre.Text);
                    cmd.Parameters.AddWithValue("@DNI", tbDni.Text);
                    cmd.Parameters.AddWithValue("@Telefono", tbTel.Text);
                    cmd.Parameters.AddWithValue("@Gmail", tbGmail.Text);
                    cmd.Parameters.AddWithValue("@Ubicacion", tbUbicacion.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Cliente guardado correctamente");


                    tbNombre.Clear();
                    tbDni.Clear();
                    tbTel.Clear();
                    tbGmail.Clear();
                    tbUbicacion.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Form5 frm = new Form5();
            frm.Show();
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
    }

