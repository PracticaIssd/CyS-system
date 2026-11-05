using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Login
{
    public partial class Form4 : Form
    {

        string conexion =
            @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";
        public Form4()
        {
            InitializeComponent();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (tbUsuario.Text == "" || tbPassword.Text == "" || tbConfirmarPassword.Text == ""
                || tbNombre.Text == "" || tbEmail.Text == "")
            {
                MessageBox.Show("Completar todos los campos");
                return;
            }

            if (tbPassword.Text != tbConfirmarPassword.Text)
            {
                MessageBox.Show("Las contraseñas no coinciden");
                return;
            }

            if (tbPassword.Text.Length < 4)
            {
                MessageBox.Show("La contraseña debe tener al menos 4 caracteres");
                return;
            }

            if (!tbEmail.Text.Contains("@"))
            {
                MessageBox.Show("Email inválido");
                return;
            }

            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string check = "SELECT COUNT(*) FROM Usuarios WHERE Usuario=@user";
                    SqlCommand cmdCheck = new SqlCommand(check, conn);
                    cmdCheck.Parameters.AddWithValue("@user", tbUsuario.Text);

                    int existe = (int)cmdCheck.ExecuteScalar();

                    if (existe > 0)
                    {
                        MessageBox.Show("El usuario ya existe");
                        return;
                    }

                    string query = "INSERT INTO Usuarios (Usuario, Password, Nombre, Email) " +
                                   "VALUES (@user, @pass, @nombre, @email)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", tbUsuario.Text);
                    cmd.Parameters.AddWithValue("@pass", tbPassword.Text);
                    cmd.Parameters.AddWithValue("@nombre", tbNombre.Text);
                    cmd.Parameters.AddWithValue("@email", tbEmail.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Usuario creado correctamente");


                    tbUsuario.Clear();
                    tbPassword.Clear();
                    tbConfirmarPassword.Clear();
                    tbNombre.Clear();
                    tbEmail.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void btnVolverLogin_Click(object sender, EventArgs e)
        {
            Form3 frm = new Form3();
            frm.Show();
            this.Hide();
        }
    }
}    
