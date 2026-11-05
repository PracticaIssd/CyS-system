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
    public partial class Form15 : Form

    {
        private string conexion = @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";
        public Form15()
        {
            InitializeComponent();

            cmbUnidadMedida.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbUnidadMedida.Items.Clear();
            cmbUnidadMedida.Items.AddRange(new object[]
            {
             "Unidad",
             "Resma",
             "Paquete",
             "Litro",
             "Kilogramo",
             "Metro"
            });

            cmbUnidadMedida.SelectedIndex = -1;

            nudStockInicial.DecimalPlaces = 2;
            nudStockInicial.Minimum = 0;
            nudStockInicial.Maximum = 1000000;

            nudStockMinimo.DecimalPlaces = 2;
            nudStockMinimo.Minimum = 0;
            nudStockMinimo.Maximum = 1000000;
        }

        private void lblTitulo_Click(object sender, EventArgs e)
        {

        }

        private void Form15_Load(object sender, EventArgs e)
        {
            CargarHistorialMovimientos();
        }

        private void CargarHistorialMovimientos()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string sql = @"
                SELECT
                    ms.IdMovimiento,
                    ms.FechaMovimiento AS Fecha,
                    m.Nombre AS Material,
                    ms.TipoMovimiento AS Movimiento,
                    ms.Cantidad,
                    m.UnidadMedida AS Unidad,
                    ms.Observaciones
                FROM MovimientosStock ms
                INNER JOIN Materiales m
                    ON ms.IdMaterial = m.IdMaterial
                ORDER BY ms.FechaMovimiento DESC,
                         ms.IdMovimiento DESC";

                    SqlDataAdapter da = new SqlDataAdapter(sql, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvHistorialMovimientos.DataSource = dt;

                    dgvHistorialMovimientos.ReadOnly = true;
                    dgvHistorialMovimientos.AllowUserToAddRows = false;
                    dgvHistorialMovimientos.AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.Fill;

                    dgvHistorialMovimientos.Columns["IdMovimiento"].Visible = false;

                    dgvHistorialMovimientos.Columns["Fecha"].DefaultCellStyle.Format =
                        "dd/MM/yyyy HH:mm";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar el historial: " + ex.Message);
            }
        }



        private void btnGuardarMaterial_Click(object sender, EventArgs e)
        {
            // 1. Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(txtNombreMaterial.Text) ||
                cmbUnidadMedida.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Debe ingresar el nombre y la unidad de medida.",
                    "Campos obligatorios",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string nombre = txtNombreMaterial.Text.Trim();
            string descripcion = txtDescripcionMaterial.Text.Trim();
            string unidad = cmbUnidadMedida.SelectedItem.ToString();

            decimal stockInicial = nudStockInicial.Value;
            decimal stockMinimo = nudStockMinimo.Value;

            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    // Comprobar si ya existe el material
                    string verificar = @"
                SELECT COUNT(*)
                FROM Materiales
                WHERE UPPER(LTRIM(RTRIM(Nombre))) = UPPER(@Nombre)";

                    using (SqlCommand cmdVerificar =
                        new SqlCommand(verificar, con))
                    {
                        cmdVerificar.Parameters.AddWithValue("@Nombre", nombre);

                        int existe = Convert.ToInt32(
                            cmdVerificar.ExecuteScalar());

                        if (existe > 0)
                        {
                            MessageBox.Show(
                                "Ya existe un material con ese nombre. " +
                                "Para agregar existencias, utilice Registrar movimiento.",
                                "Material existente",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                    }

                    // Guardar material y movimiento en una misma transacción
                    using (SqlTransaction transaccion = con.BeginTransaction())
                    {
                        try
                        {

                            string insertar = @"
                           INSERT INTO Materiales
                           (
                            Nombre,
                            Descripcion,
                            UnidadMedida,
                            StockActual,
                            StockMinimo,
                            UnidadesPorPresentacion
                            )
                           OUTPUT INSERTED.IdMaterial
                           VALUES
                           (
                            @Nombre,
                            @Descripcion,
                            @UnidadMedida,
                            @StockActual,
                            @StockMinimo,
                            @UnidadesPorPresentacion
                            )";


                            int idMaterial;

                            using (SqlCommand cmd =
                                new SqlCommand(insertar, con, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@Nombre", nombre);
                                cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                                cmd.Parameters.AddWithValue("@UnidadMedida", unidad);
                                cmd.Parameters.AddWithValue("@StockActual", stockInicial);
                                cmd.Parameters.AddWithValue("@StockMinimo", stockMinimo);

                                cmd.Parameters.AddWithValue(
                                    "@UnidadesPorPresentacion",
                                    unidad == "Resma" ? (object)500 : DBNull.Value);



                                idMaterial = Convert.ToInt32(
                                    cmd.ExecuteScalar());
                            }

                            // Registrar la entrada inicial, si hay existencias
                            if (stockInicial > 0)
                            {
                                string movimiento = @"
                            INSERT INTO MovimientosStock
                            (IdMaterial, TipoMovimiento,
                             Cantidad, Observaciones)
                            VALUES
                            (@IdMaterial, 'Entrada',
                             @Cantidad, @Observaciones)";

                                using (SqlCommand cmdMovimiento =
                                    new SqlCommand(movimiento, con, transaccion))
                                {
                                    cmdMovimiento.Parameters.AddWithValue(
                                        "@IdMaterial", idMaterial);

                                    cmdMovimiento.Parameters.AddWithValue(
                                        "@Cantidad", stockInicial);

                                    cmdMovimiento.Parameters.AddWithValue(
                                        "@Observaciones", "Stock inicial del material");

                                    cmdMovimiento.ExecuteNonQuery();
                                }
                            }

                            transaccion.Commit();
                        }
                        catch
                        {
                            transaccion.Rollback();
                            throw;
                        }
                    }
                }

                MessageBox.Show(
                    "Material registrado correctamente.",
                    "Registro exitoso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LimpiarCamposMaterial();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al registrar el material: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private void LimpiarCamposMaterial()
        {
            txtNombreMaterial.Clear();
            txtDescripcionMaterial.Clear();

            cmbUnidadMedida.SelectedIndex = -1;

            nudStockInicial.Value = 0;
            nudStockMinimo.Value = 0;

            txtNombreMaterial.Focus();
        }

        private void btnLimpiarMaterial_Click(object sender, EventArgs e)
        {
            LimpiarCamposMaterial();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnVerHistorial_Click(object sender, EventArgs e)
        {

        }

        private void txtDescripcionMaterial_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvHistorialMovimientos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            CargarHistorialMovimientos();

        }

        private void FiltrarHistorialPorFechas()
        {
            DateTime desde = dtpFechaDesde.Value.Date;
            DateTime hasta = dtpFechaHasta.Value.Date.AddDays(1);

            if (desde >= hasta)
            {
                MessageBox.Show(
                    "La fecha Desde no puede ser posterior a la fecha Hasta.");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string sql = @"
                SELECT
                    ms.IdMovimiento,
                    ms.FechaMovimiento AS Fecha,
                    m.Nombre AS Material,
                    ms.TipoMovimiento AS Movimiento,
                    ms.Cantidad,
                    m.UnidadMedida AS Unidad,
                    ms.Observaciones
                FROM MovimientosStock ms
                INNER JOIN Materiales m
                    ON ms.IdMaterial = m.IdMaterial
                WHERE ms.FechaMovimiento >= @Desde
                  AND ms.FechaMovimiento < @Hasta
                ORDER BY ms.FechaMovimiento DESC,
                         ms.IdMovimiento DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
                        cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = hasta;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvHistorialMovimientos.DataSource = dt;

                        dgvHistorialMovimientos.Columns["IdMovimiento"].Visible = false;
                        dgvHistorialMovimientos.Columns["Fecha"]
                            .DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar el historial: " + ex.Message);
            }
        }

        private void btnFiltrarHistorial_Click(object sender, EventArgs e)
        {
            FiltrarHistorialPorFechas();

        }

        private void btnActualizarHistorial_Click(object sender, EventArgs e)
        {
            CargarHistorialMovimientos();

        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Form5 frm = new Form5();
            frm.Show();
            this.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
