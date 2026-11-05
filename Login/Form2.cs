using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Login
{
    public partial class Form2 : Form
    {
        string conexion =
            @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";
        public Form2()
        {
            InitializeComponent();
        }

        private void CargarClientes()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT * FROM Clientes";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    dgvClientes.DataSource = dt;

                    dgvClientes.Columns["Id"].Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void BuscarClientes(string texto)
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT * FROM Clientes WHERE Nombre LIKE @texto";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@texto", "%" + texto + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    dgvClientes.DataSource = dt;

                    dgvClientes.Columns["Id"].Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CargarClientes();

            this.ActiveControl = txtBuscar;
        }

        private void btEliminar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show("Seleccioná un cliente");
                return;
            }

            int id = Convert.ToInt32(dgvClientes.CurrentRow.Cells["Id"].Value);

            DialogResult r = MessageBox.Show(
                "¿Eliminar cliente?",
                "Confirmar",
                MessageBoxButtons.YesNo
            );

            if (r == DialogResult.No)
                return;

            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();

                string query = "DELETE FROM Clientes WHERE Id=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Cliente eliminado");

                CargarClientes();
            }
        }

        
        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            BuscarClientes(txtBuscar.Text);

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Form5 frm = new Form5();
            frm.Show();
            this.Hide();
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            SaveFileDialog guardar = new SaveFileDialog();

            guardar.Filter = "PDF (*.pdf)|*.pdf";
            guardar.FileName = "ListadoClientes.pdf";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, new FileStream(guardar.FileName, FileMode.Create));

                doc.Open();

                Paragraph titulo = new Paragraph("LISTADO DE CLIENTES");
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                doc.Add(new Paragraph(" "));

                string[] columnas = { "Nombre", "DNI", "Telefono", "Gmail", "Ubicacion" };

                PdfPTable tabla = new PdfPTable(columnas.Length);

                 foreach (string col in columnas)
                {
                    tabla.AddCell(col);
                }

                
                foreach (DataGridViewRow fila in dgvClientes.Rows)
                {
                    if (fila.IsNewRow) continue;

                    foreach (string col in columnas)
                    {
                        tabla.AddCell(fila.Cells[col].Value?.ToString() ?? "");
                    }
                }

                doc.Add(tabla);
                doc.Close();

                MessageBox.Show("PDF generado correctamente");
            }
        }
    }
    }

