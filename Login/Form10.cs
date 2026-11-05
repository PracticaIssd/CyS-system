using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace Login
{
    public partial class Form10 : Form
    {
        private readonly string conexion =
                           @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";


        public Form10()
        {
            InitializeComponent();
        }

        // =====================================================
        // CARGAR FORMULARIO
        // =====================================================

        private void Form10_Load(object sender, EventArgs e)
        {
            dgvAlquileres.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvAlquileres.MultiSelect = false;

            dgvAlquileres.ReadOnly = true;
                        dgvAlquileres.AllowUserToAddRows = false;

            dgvAlquileres.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            CargarAlquileres();

            cmbEstadoFiltro.Items.Clear();

            cmbEstadoFiltro.Items.Add("Todos");
            cmbEstadoFiltro.Items.Add("Reservado");
            cmbEstadoFiltro.Items.Add("Activo");
            cmbEstadoFiltro.Items.Add("Finalizado");
            cmbEstadoFiltro.Items.Add("Cancelado");

            cmbEstadoFiltro.SelectedIndex = 0;

            txtBuscarDni.Focus();
        }

        // =====================================================
        // MOSTRAR TODOS LOS ALQUILERES
        // =====================================================

        private void CargarAlquileres()
        {
            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            A.IdAlquiler,
                            C.DNI,
                            C.Nombre AS Cliente,
                            A.TipoRecurso,
                            A.Recurso,
                            A.FechaInicio,
                            A.FechaFin,
                            A.CantidadDias,
                            A.PrecioPorDia,
                            A.ImporteTotal,
                            A.FormaPago,
                            A.Estado
                        FROM Alquileres A
                        INNER JOIN Clientes C
                            ON A.IdCliente = C.Id
                        ORDER BY A.IdAlquiler DESC";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);

                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    dgvAlquileres.DataSource = dt;

                    FormatearColumnas();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar los alquileres:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =====================================================
        // BUSCAR ALQUILER POR DNI
        // =====================================================

        private void btnBuscar_Click(
            object sender,
            EventArgs e)
        {
            string dni =
                txtBuscarDni.Text.Trim();

            if (dni == "")
            {
                MessageBox.Show(
                    "Ingrese un DNI.",
                    "Buscar alquiler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtBuscarDni.Focus();

                return;
            }

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            A.IdAlquiler,
                            C.DNI,
                            C.Nombre AS Cliente,
                            A.TipoRecurso,
                            A.Recurso,
                            A.FechaInicio,
                            A.FechaFin,
                            A.CantidadDias,
                            A.PrecioPorDia,
                            A.ImporteTotal,
                            A.FormaPago,
                            A.Estado
                        FROM Alquileres A
                        INNER JOIN Clientes C
                            ON A.IdCliente = C.Id
                        WHERE C.DNI = @dni
                        ORDER BY A.IdAlquiler DESC";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);

                    da.SelectCommand.Parameters.Add(
                        "@dni",
                        SqlDbType.VarChar,
                        20
                    ).Value = dni;

                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    dgvAlquileres.DataSource = dt;

                    FormatearColumnas();

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show(
                            "No se encontraron alquileres para ese DNI.",
                            "Sin resultados",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al buscar los alquileres:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =====================================================
        // MOSTRAR TODOS
        // =====================================================

        private void btnMostrarTodos_Click(
            object sender,
            EventArgs e)
        {
            txtBuscarDni.Clear();

            CargarAlquileres();
        }

        // =====================================================
        // FORMATO DEL DATAGRIDVIEW
        // =====================================================

        private void FormatearColumnas()
        {
            if (dgvAlquileres.Columns.Count == 0)
                return;

            dgvAlquileres.Columns["IdAlquiler"]
                .HeaderText = "N° Alquiler";

            dgvAlquileres.Columns["DNI"]
                .HeaderText = "DNI";

            dgvAlquileres.Columns["Cliente"]
                .HeaderText = "Cliente";

            dgvAlquileres.Columns["TipoRecurso"]
                .HeaderText = "Tipo";

            dgvAlquileres.Columns["Recurso"]
                .HeaderText = "Recurso";

            dgvAlquileres.Columns["FechaInicio"]
                .HeaderText = "Inicio";

            dgvAlquileres.Columns["FechaFin"]
                .HeaderText = "Fin";

            dgvAlquileres.Columns["CantidadDias"]
                .HeaderText = "Días";

            dgvAlquileres.Columns["PrecioPorDia"]
                .HeaderText = "Precio/Día";

            dgvAlquileres.Columns["ImporteTotal"]
                .HeaderText = "Total";

            dgvAlquileres.Columns["FormaPago"]
                .HeaderText = "Forma de pago";

            dgvAlquileres.Columns["Estado"]
                .HeaderText = "Estado";

            // =================================================
            // FORMATO DE FECHAS
            // =================================================

            dgvAlquileres.Columns["FechaInicio"]
                .DefaultCellStyle.Format = "dd/MM/yyyy";

            dgvAlquileres.Columns["FechaFin"]
                .DefaultCellStyle.Format = "dd/MM/yyyy";

            // =================================================
            // FORMATO DE DINERO
            // =================================================

            dgvAlquileres.Columns["PrecioPorDia"]
                .DefaultCellStyle.Format = "C2";

            dgvAlquileres.Columns["ImporteTotal"]
                .DefaultCellStyle.Format = "C2";
        }

        // =====================================================
        // BOTÓN VOLVER
        // =====================================================

        private void btnVolver_Click(
            object sender,
            EventArgs e)
        {
            Form5 frm = new Form5();

            frm.Show();

            this.Close();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvAlquileres.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un alquiler para eliminar.",
                    "Eliminar alquiler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int idAlquiler = Convert.ToInt32(
                dgvAlquileres.SelectedRows[0]
                .Cells["IdAlquiler"]
                .Value);

            DialogResult resultado = MessageBox.Show(
                "¿Está seguro de que desea eliminar este alquiler?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes)
                return;

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                DELETE FROM Alquileres
                WHERE IdAlquiler = @id";

                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(
                            "@id",
                            SqlDbType.Int).Value = idAlquiler;

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show(
                        "Alquiler eliminado correctamente.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    CargarAlquileres();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al eliminar el alquiler:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
          
        
            if (dgvAlquileres.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un alquiler para modificar.",
                    "Modificar alquiler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int idAlquiler = Convert.ToInt32(
                dgvAlquileres.SelectedRows[0]
                .Cells["IdAlquiler"]
                .Value);

            Form9 frm = new Form9(idAlquiler);

            frm.ShowDialog();

            CargarAlquileres();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtBuscarDni_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnFiltrarEstado_Click(object sender, EventArgs e)
        {
           
        
            if (cmbEstadoFiltro.SelectedItem == null)
            {
                MessageBox.Show(
                    "Seleccione un estado.",
                    "Filtrar alquileres",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string estado =
                cmbEstadoFiltro.SelectedItem.ToString();

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT
                    A.IdAlquiler,
                    C.DNI,
                    C.Nombre AS Cliente,
                    A.TipoRecurso,
                    A.Recurso,
                    A.FechaInicio,
                    A.FechaFin,
                    A.CantidadDias,
                    A.PrecioPorDia,
                    A.ImporteTotal,
                    A.FormaPago,
                    A.Estado
                FROM Alquileres A
                INNER JOIN Clientes C
                    ON A.IdCliente = C.Id
            ";

                    if (estado != "Todos")
                    {
                        query += " WHERE A.Estado = @estado ";
                    }

                    query += " ORDER BY A.IdAlquiler DESC";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);

                    if (estado != "Todos")
                    {
                        da.SelectCommand.Parameters.Add(
                            "@estado",
                            SqlDbType.VarChar,
                            50).Value = estado;
                    }

                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    dgvAlquileres.DataSource = dt;

                    FormatearColumnas();

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show(
                            "No se encontraron alquileres con ese estado.",
                            "Sin resultados",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al filtrar los alquileres:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        }
}




