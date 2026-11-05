using ClosedXML.Excel;
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
    public partial class Form7 : Form
    {
        string conexion =
            @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";
        public Form7()
        {
            InitializeComponent();
        }
        private void Form7_Load(object sender, EventArgs e)
        {
            CargarPedidos();

            dgvPedidos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPedidos.MultiSelect = false;
            dgvPedidos.ReadOnly = true;
        }

        private void CargarPedidos()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                    SELECT
                        IdPedido,
                        DniCliente,
                        Cliente,
                        FechaPedido,
                        FechaEntrega,
                        DomicilioEntrega,
                        Estado,
                        ImporteTotal
                    FROM Pedidos
                    ORDER BY IdPedido DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    dgvPedidos.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (txtBuscarDni.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese un DNI.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                    SELECT
                     IdPedido,
                     DniCliente,
                     Cliente,
                     FechaPedido,
                     FechaEntrega,
                     DomicilioEntrega,
                     Estado,
                     ImporteTotal
                    FROM Pedidos
                    WHERE DniCliente = @dni
                    ORDER BY IdPedido DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);

                    da.SelectCommand.Parameters.AddWithValue(
                        "@dni",
                        txtBuscarDni.Text);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    dgvPedidos.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No se encontraron pedidos para ese DNI.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnMostrarTodos_Click(object sender, EventArgs e)
        {
            CargarPedidos();
        }

        private void btnAbrirPedido_Click(object sender, EventArgs e)
        {
            int idPedido = Convert.ToInt32(
            dgvPedidos.CurrentRow.Cells["IdPedido"].Value);

            Form6 frm = new Form6(idPedido);

            frm.ShowDialog();

            CargarPedidos();
        }
        private void btnEliminarPedido_Click(object sender, EventArgs e)
        {
            if (dgvPedidos.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un pedido.");
                return;
            }

            int idPedido = Convert.ToInt32(
                dgvPedidos.CurrentRow.Cells["IdPedido"].Value);

            DialogResult r = MessageBox.Show(
                "¿Desea eliminar el pedido seleccionado?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r != DialogResult.Yes)
                return;


            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    string queryDetalle =
                        "DELETE FROM DetallePedido WHERE IdPedido=@id";

                    SqlCommand cmdDetalle =
                        new SqlCommand(queryDetalle, conn, trans);

                    cmdDetalle.Parameters.AddWithValue("@id", idPedido);

                    cmdDetalle.ExecuteNonQuery();

                    string queryPedido =
                        "DELETE FROM Pedidos WHERE IdPedido=@id";

                    SqlCommand cmdPedido =
                        new SqlCommand(queryPedido, conn, trans);

                    cmdPedido.Parameters.AddWithValue("@id", idPedido);

                    cmdPedido.ExecuteNonQuery();

                    trans.Commit();

                    MessageBox.Show("Pedido eliminado correctamente.");

                    CargarPedidos();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Form5 frm = new Form5();
            frm.Show();
            this.Hide();
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvPedidos.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            SaveFileDialog guardar = new SaveFileDialog();
            guardar.Filter = "Excel (*.xlsx)|*.xlsx";
            guardar.FileName = "Pedidos_Reporte.xlsx";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Pedidos");

                int fila = 1;

                
                ws.Cell(fila, 1).Value = "REPORTE DE PEDIDOS";
                ws.Range(fila, 1, fila, dgvPedidos.Columns.Count).Merge();
                ws.Cell(fila, 1).Style.Font.Bold = true;
                ws.Cell(fila, 1).Style.Font.FontSize = 16;
                ws.Cell(fila, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                fila += 2;

                for (int i = 0; i < dgvPedidos.Columns.Count; i++)
                {
                    ws.Cell(fila, i + 1).Value = dgvPedidos.Columns[i].HeaderText;
                    ws.Cell(fila, i + 1).Style.Font.Bold = true;
                    ws.Cell(fila, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Cell(fila, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                fila++;

                double totalImporte = 0;

                foreach (DataGridViewRow row in dgvPedidos.Rows)
                {
                    if (row.IsNewRow) continue;

                    for (int i = 0; i < dgvPedidos.Columns.Count; i++)
                    {
                        var value = row.Cells[i].Value?.ToString() ?? "";

                        ws.Cell(fila, i + 1).Value = value;

                        if (dgvPedidos.Columns[i].Name == "ImporteTotal")
                        {
                            if (double.TryParse(value, out double imp))
                                totalImporte += imp;
                        }
                    }

                    fila++;
                }

                ws.Cell(fila, dgvPedidos.Columns.Count - 1).Value = "TOTAL:";
                ws.Cell(fila, dgvPedidos.Columns.Count).Value = totalImporte;

                ws.Cell(fila, dgvPedidos.Columns.Count - 1).Style.Font.Bold = true;
                ws.Cell(fila, dgvPedidos.Columns.Count).Style.Font.Bold = true;
                ws.Cell(fila, dgvPedidos.Columns.Count).Style.Fill.BackgroundColor = XLColor.Yellow;

                var rango = ws.Range(3, 1, fila, dgvPedidos.Columns.Count);
                rango.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                ws.Columns().AdjustToContents();

                wb.SaveAs(guardar.FileName);

                MessageBox.Show("Excel generado correctamente.");
            }
        }
    }
}


       
        
  
