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
    public partial class Form11 : Form
    {
        // =====================================================
        // CONEXIÓN
        // =====================================================

        private readonly string conexion =
    @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";

        // =====================================================
        // VARIABLES
        // =====================================================

        private int idMaquinariaSeleccionada = 0;
        private int idTecnicoSeleccionado = 0;
        private int idFallaSeleccionada = 0;

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public Form11()
        {
            InitializeComponent();

            dgvMaquinarias.CellClick += dgvMaquinarias_CellClick;
            dgvTecnicos.CellClick += dgvTecnicos_CellClick;
            dgvFallas.CellClick += dgvFallas_CellClick;
        }

        // =====================================================
        // LOAD
        // =====================================================

        private void Form11_Load(object sender, EventArgs e)
        {
            ConfigurarGrillas();

            CargarEstadosMaquinaria();
            CargarEstadosTecnico();
            CargarEstadosFalla();

            CargarMaquinarias();
            CargarTecnicos();

            CargarComboMaquinarias();
            CargarFallas();

            ActualizarKPIs();

            txtBuscarMaquinaria.Focus();
        }

        // =====================================================
        // CONFIGURAR DATAGRIDVIEW
        // =====================================================

        private void ConfigurarGrillas()
        {
            ConfigurarDataGridView(dgvMaquinarias);
            ConfigurarDataGridView(dgvTecnicos);
            ConfigurarDataGridView(dgvFallas);
        }

        private void ConfigurarDataGridView(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = true;

            dgv.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgv.MultiSelect = false;
            dgv.ReadOnly = true;

            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;

            dgv.RowHeadersVisible = false;

            dgv.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;

            dgv.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;

            dgv.EnableHeadersVisualStyles = false;

            dgv.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(8, 43, 82);

            dgv.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgv.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgv.ColumnHeadersHeight = 40;
        }

        // =====================================================
        // KPI
        // =====================================================

        private void ActualizarKPIs()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    // MAQUINARIAS
                    SqlCommand cmdMaquinarias =
                        new SqlCommand(
                            "SELECT COUNT(*) FROM Maquinarias",
                            conn);

                    lblNumeroMaquinas.Text =
                        cmdMaquinarias.ExecuteScalar().ToString();


                    // TÉCNICOS
                    SqlCommand cmdTecnicos =
                        new SqlCommand(
                            "SELECT COUNT(*) FROM Tecnicos",
                            conn);

                    lblNumeroTecnicos.Text =
                        cmdTecnicos.ExecuteScalar().ToString();


                    // FALLAS PENDIENTES
                    SqlCommand cmdFallas =
                        new SqlCommand(@"
                            SELECT COUNT(*)
                            FROM Fallas
                            WHERE Estado = 'Pendiente'",
                            conn);

                    lblNumeroFallas.Text =
                        cmdFallas.ExecuteScalar().ToString();


                    // EN REPARACIÓN
                    SqlCommand cmdReparaciones =
                        new SqlCommand(@"
                            SELECT COUNT(*)
                            FROM Fallas
                            WHERE Estado = 'En reparación'",
                            conn);

                    lblNumeroReparaciones.Text =
                        cmdReparaciones.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar los indicadores:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =====================================================
        // =====================================================
        // MAQUINARIAS
        // =====================================================
        // =====================================================

        private void CargarEstadosMaquinaria()
        {
            cmbEstadoMaquinaria.Items.Clear();

            cmbEstadoMaquinaria.Items.Add("Disponible");
            cmbEstadoMaquinaria.Items.Add("En mantenimiento");
            cmbEstadoMaquinaria.Items.Add("Fuera de servicio");

            cmbEstadoMaquinaria.SelectedIndex = 0;
        }

        // =====================================================
        // CARGAR MAQUINARIAS
        // =====================================================

        private void CargarMaquinarias()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT
                    IdMaquinaria,
                    Nombre,
                    Descripcion,
                    Estado,
                    PrecioPorDia
                FROM Maquinarias
                ORDER BY IdMaquinaria DESC";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvMaquinarias.DataSource = dt;

                    BeginInvoke(new MethodInvoker(() =>
                    {
                        FormatearColumnasMaquinaria();
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar las maquinarias:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void FormatearColumnasMaquinaria()
        {
            if (dgvMaquinarias.Columns.Count == 0)
                return;

            if (dgvMaquinarias.Columns.Contains("IdMaquinaria"))
            {
                dgvMaquinarias.Columns["IdMaquinaria"].HeaderText = "ID";
                dgvMaquinarias.Columns["IdMaquinaria"].FillWeight = 35;
            }

            if (dgvMaquinarias.Columns.Contains("Nombre"))
            {
                dgvMaquinarias.Columns["Nombre"].HeaderText = "Maquinaria";
                dgvMaquinarias.Columns["Nombre"].FillWeight = 100;
            }

            if (dgvMaquinarias.Columns.Contains("Descripcion"))
            {
                dgvMaquinarias.Columns["Descripcion"].HeaderText = "Descripción";
                dgvMaquinarias.Columns["Descripcion"].FillWeight = 160;
            }

            if (dgvMaquinarias.Columns.Contains("Estado"))
            {
                dgvMaquinarias.Columns["Estado"].HeaderText = "Estado";
                dgvMaquinarias.Columns["Estado"].FillWeight = 80;
            }
            if (dgvMaquinarias.Columns.Contains("PrecioPorDia"))
            {
                dgvMaquinarias.Columns["PrecioPorDia"].HeaderText =
                    "Precio por día";

                dgvMaquinarias.Columns["PrecioPorDia"].FillWeight = 80;

                dgvMaquinarias.Columns["PrecioPorDia"]
                    .DefaultCellStyle.Format = "C2";
            }
        }

        // =====================================================
        // SELECCIONAR MAQUINARIA
        // =====================================================

        private void dgvMaquinarias_CellClick(
     object sender,
     DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvMaquinarias.CurrentRow == null)
                return;

            DataRowView fila =
                dgvMaquinarias.CurrentRow.DataBoundItem as DataRowView;

            if (fila == null)
                return;

            idMaquinariaSeleccionada =
                Convert.ToInt32(fila["IdMaquinaria"]);

            txtNombreMaquinaria.Text =
                fila["Nombre"]?.ToString() ?? "";

            txtDescripcionMaquinaria.Text =
                fila["Descripcion"]?.ToString() ?? "";

            cmbEstadoMaquinaria.Text =
                fila["Estado"]?.ToString() ?? "";

            if (fila["PrecioPorDia"] != DBNull.Value)
            {
                txtPrecioPorDiaMaquinaria.Text =
                    Convert.ToDecimal(
                        fila["PrecioPorDia"]).ToString("0.00");
            }
            else
            {
                txtPrecioPorDiaMaquinaria.Clear();
            }
        }

        // =====================================================
        // BUSCAR MAQUINARIA
        // =====================================================

        private void btnBuscarMaquinaria_Click(
            object sender,
            EventArgs e)
        {
            string buscar =
                txtBuscarMaquinaria.Text.Trim();

            if (string.IsNullOrWhiteSpace(buscar))
            {
                CargarMaquinarias();
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
                    IdMaquinaria,
                    Nombre,
                    Descripcion,
                    Estado,
                    PrecioPorDia
                    FROM Maquinarias
                    WHERE Nombre LIKE @Buscar
                    ORDER BY IdMaquinaria DESC";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);

                    da.SelectCommand.Parameters.AddWithValue(
                        "@Buscar",
                        "%" + buscar + "%");

                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    dgvMaquinarias.DataSource = dt;

                    FormatearColumnasMaquinaria();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al buscar:\n" +
                        ex.Message);
                }
            }
        }

        // =====================================================
        // MOSTRAR TODAS LAS MAQUINARIAS
        // =====================================================

        private void btnMostrarTodasMaquinarias_Click(
            object sender,
            EventArgs e)
        {
            txtBuscarMaquinaria.Clear();

            CargarMaquinarias();

            LimpiarMaquinaria();
        }

        // =====================================================
        private void btnRegistrarMaquinaria_Click(
     object sender,
     EventArgs e)
        {
            // ============================================
            // VALIDAR NOMBRE
            // ============================================

            if (string.IsNullOrWhiteSpace(
                txtNombreMaquinaria.Text))
            {
                MessageBox.Show(
                    "Ingrese el nombre de la maquinaria.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtNombreMaquinaria.Focus();
                return;
            }


            // ============================================
            // VALIDAR ESTADO
            // ============================================

            if (cmbEstadoMaquinaria.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione un estado.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbEstadoMaquinaria.Focus();
                return;
            }


            // ============================================
            // VALIDAR PRECIO POR DÍA
            // ============================================

            decimal precioPorDia;

            if (!decimal.TryParse(
                txtPrecioPorDiaMaquinaria.Text.Trim(),
                out precioPorDia))
            {
                MessageBox.Show(
                    "Ingrese un precio por día válido.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPrecioPorDiaMaquinaria.Focus();
                return;
            }

            if (precioPorDia <= 0)
            {
                MessageBox.Show(
                    "El precio por día debe ser mayor a $0.",
                    "Precio inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPrecioPorDiaMaquinaria.Focus();
                return;
            }


            // ============================================
            // REGISTRAR MAQUINARIA
            // ============================================

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                INSERT INTO Maquinarias
                (
                    Nombre,
                    Descripcion,
                    Estado,
                    PrecioPorDia
                )
                VALUES
                (
                    @Nombre,
                    @Descripcion,
                    @Estado,
                    @PrecioPorDia
                )";

                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Nombre",
                            txtNombreMaquinaria.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Descripcion",
                            txtDescripcionMaquinaria.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Estado",
                            cmbEstadoMaquinaria.Text);

                        cmd.Parameters.Add(
                            "@PrecioPorDia",
                            SqlDbType.Decimal).Value =
                            precioPorDia;

                        cmd.ExecuteNonQuery();
                    }


                    MessageBox.Show(
                        "Maquinaria registrada correctamente.",
                        "Registro exitoso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);


                    CargarMaquinarias();
                    CargarComboMaquinarias();

                    LimpiarMaquinaria();

                    ActualizarKPIs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al registrar la maquinaria:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }


        private void btnModificarMaquinaria_Click(
     object sender,
     EventArgs e)
        {
            if (idMaquinariaSeleccionada == 0)
            {
                MessageBox.Show(
                    "Seleccione una maquinaria.");

                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtNombreMaquinaria.Text))
            {
                MessageBox.Show(
                    "Ingrese el nombre de la maquinaria.");

                txtNombreMaquinaria.Focus();
                return;
            }

            if (cmbEstadoMaquinaria.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione un estado.");

                cmbEstadoMaquinaria.Focus();
                return;
            }

            // ============================================
            // VALIDAR PRECIO POR DÍA
            // ============================================

            decimal precioPorDia;

            if (!decimal.TryParse(
                txtPrecioPorDiaMaquinaria.Text.Trim(),
                out precioPorDia))
            {
                MessageBox.Show(
                    "Ingrese un precio por día válido.",
                    "Precio inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPrecioPorDiaMaquinaria.Focus();
                return;
            }

            if (precioPorDia <= 0)
            {
                MessageBox.Show(
                    "El precio por día debe ser mayor a $0.",
                    "Precio inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPrecioPorDiaMaquinaria.Focus();
                return;
            }


            DialogResult respuesta =
                MessageBox.Show(
                    "¿Desea modificar la maquinaria seleccionada?",
                    "Confirmar modificación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;


            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                UPDATE Maquinarias
                SET
                    Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Estado = @Estado,
                    PrecioPorDia = @PrecioPorDia
                WHERE IdMaquinaria = @Id";

                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Nombre",
                            txtNombreMaquinaria.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Descripcion",
                            txtDescripcionMaquinaria.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Estado",
                            cmbEstadoMaquinaria.Text);

                        cmd.Parameters.Add(
                            "@PrecioPorDia",
                            SqlDbType.Decimal).Value =
                            precioPorDia;

                        cmd.Parameters.AddWithValue(
                            "@Id",
                            idMaquinariaSeleccionada);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show(
                        "Maquinaria modificada correctamente.",
                        "Modificación exitosa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    CargarMaquinarias();
                    CargarComboMaquinarias();

                    LimpiarMaquinaria();

                    ActualizarKPIs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al modificar:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void btnEliminarMaquinaria_Click(
            object sender,
            EventArgs e)
        {
            if (idMaquinariaSeleccionada == 0)
            {
                MessageBox.Show(
                    "Seleccione una maquinaria.");

                return;
            }

            DialogResult respuesta =
                MessageBox.Show(
                    "¿Está seguro de eliminar esta maquinaria?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        DELETE FROM Maquinarias
                        WHERE IdMaquinaria = @Id";

                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Id",
                            idMaquinariaSeleccionada);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show(
                        "Maquinaria eliminada correctamente.");

                    CargarMaquinarias();
                    CargarComboMaquinarias();

                    LimpiarMaquinaria();

                    ActualizarKPIs();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547)
                    {
                        MessageBox.Show(
                            "No se puede eliminar esta maquinaria porque tiene fallas registradas.",
                            "No se puede eliminar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show(
                            ex.Message);
                    }
                }
            }
        }

        // =====================================================
        // LIMPIAR MAQUINARIA
        // =====================================================

        private void btnLimpiarMaquinaria_Click(
            object sender,
            EventArgs e)
        {
            LimpiarMaquinaria();
        }

        private void LimpiarMaquinaria()
        {
            idMaquinariaSeleccionada = 0;

            txtNombreMaquinaria.Clear();
            txtDescripcionMaquinaria.Clear();
            txtPrecioPorDiaMaquinaria.Clear();

            if (cmbEstadoMaquinaria.Items.Count > 0)
                cmbEstadoMaquinaria.SelectedIndex = 0;

            dgvMaquinarias.ClearSelection();

            txtNombreMaquinaria.Focus();
        }

        // =====================================================
        // =====================================================
        // TÉCNICOS
        // =====================================================
        // =====================================================

        private void CargarEstadosTecnico()
        {
            cmbEstadoTecnico.Items.Clear();

            cmbEstadoTecnico.Items.Add("Disponible");
            cmbEstadoTecnico.Items.Add("Ocupado");
            cmbEstadoTecnico.Items.Add("Inactivo");

            cmbEstadoTecnico.SelectedIndex = 0;
        }

        // =====================================================
        // CARGAR TÉCNICOS
        // =====================================================

        private void CargarTecnicos()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT
                    IdTecnico,
                    Nombre,
                    DNI,
                    Telefono,
                    Especialidad,
                    Estado
                FROM Tecnicos
                ORDER BY IdTecnico DESC";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvTecnicos.DataSource = dt;

                    // Esperamos a que el DataGridView termine de crear
                    // y enlazar las columnas
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        FormatearColumnasTecnicos();
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar los técnicos:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void FormatearColumnasTecnicos()
        {
            if (dgvTecnicos.Columns.Count == 0)
                return;

            if (dgvTecnicos.Columns.Contains("IdTecnico"))
            {
                dgvTecnicos.Columns["IdTecnico"].HeaderText = "ID";
                dgvTecnicos.Columns["IdTecnico"].FillWeight = 35;
            }

            if (dgvTecnicos.Columns.Contains("Nombre"))
            {
                dgvTecnicos.Columns["Nombre"].HeaderText = "Técnico";
                dgvTecnicos.Columns["Nombre"].FillWeight = 100;
            }

            if (dgvTecnicos.Columns.Contains("DNI"))
            {
                dgvTecnicos.Columns["DNI"].HeaderText = "DNI";
                dgvTecnicos.Columns["DNI"].FillWeight = 70;
            }

            if (dgvTecnicos.Columns.Contains("Telefono"))
            {
                dgvTecnicos.Columns["Telefono"].HeaderText = "Teléfono";
                dgvTecnicos.Columns["Telefono"].FillWeight = 80;
            }

            if (dgvTecnicos.Columns.Contains("Especialidad"))
            {
                dgvTecnicos.Columns["Especialidad"].HeaderText =
                    "Especialidad";

                dgvTecnicos.Columns["Especialidad"].FillWeight = 130;
            }

            if (dgvTecnicos.Columns.Contains("Estado"))
            {
                dgvTecnicos.Columns["Estado"].HeaderText = "Estado";
                dgvTecnicos.Columns["Estado"].FillWeight = 70;
            }
        }
        // =====================================================
        // SELECCIONAR TÉCNICO
        // =====================================================

        private void dgvTecnicos_CellClick(
    object sender,
    DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvTecnicos.CurrentRow == null)
                return;

            DataRowView fila =
                dgvTecnicos.CurrentRow.DataBoundItem as DataRowView;

            if (fila == null)
                return;

            idTecnicoSeleccionado =
                Convert.ToInt32(fila["IdTecnico"]);

            txtNombreTecnico.Text =
                fila["Nombre"]?.ToString() ?? "";

            txtDniTecnico.Text =
                fila["DNI"]?.ToString() ?? "";

            txtTelefonoTecnico.Text =
                fila["Telefono"]?.ToString() ?? "";

            txtEspecialidadTecnico.Text =
                fila["Especialidad"]?.ToString() ?? "";

            cmbEstadoTecnico.Text =
                fila["Estado"]?.ToString() ?? "";
        }

        // =====================================================
        // BUSCAR TÉCNICO
        // =====================================================

        private void btnBuscarTecnico_Click(
            object sender,
            EventArgs e)
        {
            string buscar =
                txtBuscarTecnico.Text.Trim();

            if (string.IsNullOrWhiteSpace(buscar))
            {
                CargarTecnicos();
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
                            IdTecnico,
                            Nombre,
                            DNI,
                            Telefono,
                            Especialidad,
                            Estado
                        FROM Tecnicos
                        WHERE Nombre LIKE @Buscar
                           OR DNI LIKE @Buscar
                        ORDER BY IdTecnico DESC";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);

                    da.SelectCommand.Parameters.AddWithValue(
                        "@Buscar",
                        "%" + buscar + "%");

                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    dgvTecnicos.DataSource = dt;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al buscar técnico:\n" +
                        ex.Message);
                }
            }
        }

        // =====================================================
        // MOSTRAR TODOS LOS TÉCNICOS
        // =====================================================

        private void btnMostrarTodosTecnicos_Click(
            object sender,
            EventArgs e)
        {
            txtBuscarTecnico.Clear();

            CargarTecnicos();

            LimpiarTecnico();
        }

        // =====================================================
        // REGISTRAR TÉCNICO
        // =====================================================

        private void btnRegistrarTecnico_Click(
            object sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                    txtNombreTecnico.Text) ||
                string.IsNullOrWhiteSpace(
                    txtDniTecnico.Text) ||
                string.IsNullOrWhiteSpace(
                    txtEspecialidadTecnico.Text))
            {
                MessageBox.Show(
                    "Complete nombre, DNI y especialidad.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (cmbEstadoTecnico.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione el estado del técnico.");

                return;
            }

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    // VALIDAR DNI REPETIDO

                    string existe = @"
                        SELECT COUNT(*)
                        FROM Tecnicos
                        WHERE DNI = @DNI";

                    using (SqlCommand cmdExiste =
                           new SqlCommand(existe, conn))
                    {
                        cmdExiste.Parameters.AddWithValue(
                            "@DNI",
                            txtDniTecnico.Text.Trim());

                        int cantidad =
                            Convert.ToInt32(
                                cmdExiste.ExecuteScalar());

                        if (cantidad > 0)
                        {
                            MessageBox.Show(
                                "Ya existe un técnico registrado con ese DNI.",
                                "DNI existente",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                            return;
                        }
                    }

                    string query = @"
                        INSERT INTO Tecnicos
                        (
                            Nombre,
                            DNI,
                            Telefono,
                            Especialidad,
                            Estado
                        )
                        VALUES
                        (
                            @Nombre,
                            @DNI,
                            @Telefono,
                            @Especialidad,
                            @Estado
                        )";

                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Nombre",
                            txtNombreTecnico.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@DNI",
                            txtDniTecnico.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Telefono",
                            txtTelefonoTecnico.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Especialidad",
                            txtEspecialidadTecnico.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Estado",
                            cmbEstadoTecnico.Text);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show(
                        "Técnico registrado correctamente.",
                        "Registro exitoso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    CargarTecnicos();

                    LimpiarTecnico();

                    ActualizarKPIs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al registrar técnico:\n" +
                        ex.Message);
                }
            }
        }

        // =====================================================
        // MODIFICAR TÉCNICO
        // =====================================================

        private void btnModificarTecnico_Click(
            object sender,
            EventArgs e)
        {
            if (idTecnicoSeleccionado == 0)
            {
                MessageBox.Show(
                    "Seleccione un técnico.");

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    txtNombreTecnico.Text) ||
                string.IsNullOrWhiteSpace(
                    txtDniTecnico.Text) ||
                string.IsNullOrWhiteSpace(
                    txtEspecialidadTecnico.Text))
            {
                MessageBox.Show(
                    "Complete los campos obligatorios.");

                return;
            }

            DialogResult respuesta =
                MessageBox.Show(
                    "¿Desea modificar el técnico seleccionado?",
                    "Confirmar modificación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        UPDATE Tecnicos
                        SET
                            Nombre = @Nombre,
                            DNI = @DNI,
                            Telefono = @Telefono,
                            Especialidad = @Especialidad,
                            Estado = @Estado
                        WHERE IdTecnico = @Id";

                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Nombre",
                            txtNombreTecnico.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@DNI",
                            txtDniTecnico.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Telefono",
                            txtTelefonoTecnico.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Especialidad",
                            txtEspecialidadTecnico.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Estado",
                            cmbEstadoTecnico.Text);

                        cmd.Parameters.AddWithValue(
                            "@Id",
                            idTecnicoSeleccionado);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show(
                        "Técnico modificado correctamente.");

                    CargarTecnicos();

                    LimpiarTecnico();

                    ActualizarKPIs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al modificar técnico:\n" +
                        ex.Message);
                }
            }
        }

        // =====================================================
        // ELIMINAR TÉCNICO
        // =====================================================

        private void btnEliminarTecnico_Click(
            object sender,
            EventArgs e)
        {
            if (idTecnicoSeleccionado == 0)
            {
                MessageBox.Show(
                    "Seleccione un técnico.");

                return;
            }

            DialogResult respuesta =
                MessageBox.Show(
                    "¿Desea eliminar este técnico?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        DELETE FROM Tecnicos
                        WHERE IdTecnico = @Id";

                    SqlCommand cmd =
                        new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue(
                        "@Id",
                        idTecnicoSeleccionado);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show(
                        "Técnico eliminado correctamente.");

                    CargarTecnicos();

                    LimpiarTecnico();

                    ActualizarKPIs();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547)
                    {
                        MessageBox.Show(
                            "No se puede eliminar este técnico porque posee órdenes de trabajo registradas.",
                            "No se puede eliminar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show(
                            ex.Message);
                    }
                }
            }
        }

        // =====================================================
        // LIMPIAR TÉCNICO
        // =====================================================

        private void btnLimpiarTecnico_Click(
            object sender,
            EventArgs e)
        {
            LimpiarTecnico();
        }

        private void LimpiarTecnico()
        {
            idTecnicoSeleccionado = 0;

            txtNombreTecnico.Clear();
            txtDniTecnico.Clear();
            txtTelefonoTecnico.Clear();
            txtEspecialidadTecnico.Clear();

            if (cmbEstadoTecnico.Items.Count > 0)
                cmbEstadoTecnico.SelectedIndex = 0;

            dgvTecnicos.ClearSelection();

            txtNombreTecnico.Focus();
        }

        // =====================================================
        // =====================================================
        // FALLAS
        // =====================================================
        // =====================================================

        private void CargarEstadosFalla()
        {
            cmbEstadoFalla.Items.Clear();

            cmbEstadoFalla.Items.Add("Pendiente");
            cmbEstadoFalla.Items.Add("En reparación");
            cmbEstadoFalla.Items.Add("Resuelta");

            cmbEstadoFalla.SelectedIndex = 0;
        }

        // =====================================================
        // COMBO MAQUINARIAS
        // =====================================================

        private void CargarComboMaquinarias()
        {
            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            IdMaquinaria,
                            Nombre
                        FROM Maquinarias
                        ORDER BY Nombre";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);

                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    cmbMaquinariaFalla.DataSource = dt;

                    cmbMaquinariaFalla.DisplayMember =
                        "Nombre";

                    cmbMaquinariaFalla.ValueMember =
                        "IdMaquinaria";

                    cmbMaquinariaFalla.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar las maquinarias:\n" +
                        ex.Message);
                }
            }
        }

        // =====================================================
        // CARGAR FALLAS
        // =====================================================

       
            private void CargarFallas()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT
                    F.IdFalla,
                    F.IdMaquinaria,
                    M.Nombre AS Maquinaria,
                    F.FechaFalla,
                    F.Descripcion,
                    F.Estado
                FROM Fallas F
                INNER JOIN Maquinarias M
                    ON F.IdMaquinaria = M.IdMaquinaria
                ORDER BY F.IdFalla DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvFallas.DataSource = dt;

                    BeginInvoke(new MethodInvoker(() =>
                    {
                        FormatearColumnasFallas();
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar fallas:\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void FormatearColumnasFallas()
        {
            if (dgvFallas.Columns.Count == 0)
                return;

            if (dgvFallas.Columns.Contains("IdFalla"))
            {
                dgvFallas.Columns["IdFalla"].HeaderText = "ID";
                dgvFallas.Columns["IdFalla"].FillWeight = 35;
            }

            if (dgvFallas.Columns.Contains("IdMaquinaria"))
            {
                dgvFallas.Columns["IdMaquinaria"].Visible = false;
            }

            if (dgvFallas.Columns.Contains("Maquinaria"))
            {
                dgvFallas.Columns["Maquinaria"].HeaderText = "Maquinaria";
                dgvFallas.Columns["Maquinaria"].FillWeight = 90;
            }

            if (dgvFallas.Columns.Contains("FechaFalla"))
            {
                dgvFallas.Columns["FechaFalla"].HeaderText = "Fecha";
                dgvFallas.Columns["FechaFalla"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvFallas.Columns["FechaFalla"].FillWeight = 70;
            }

            if (dgvFallas.Columns.Contains("Descripcion"))
            {
                dgvFallas.Columns["Descripcion"].HeaderText = "Descripción";
                dgvFallas.Columns["Descripcion"].FillWeight = 180;
            }

            if (dgvFallas.Columns.Contains("Estado"))
            {
                dgvFallas.Columns["Estado"].HeaderText = "Estado";
                dgvFallas.Columns["Estado"].FillWeight = 70;
            }
        }

        // =====================================================
        // SELECCIONAR FALLA
        // =====================================================

        private void dgvFallas_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow fila =
                dgvFallas.Rows[e.RowIndex];

            idFallaSeleccionada =
                Convert.ToInt32(
                    fila.Cells["IdFalla"].Value);

            cmbMaquinariaFalla.SelectedValue =
                Convert.ToInt32(
                    fila.Cells["IdMaquinaria"].Value);

            dtpFechaFalla.Value =
                Convert.ToDateTime(
                    fila.Cells["FechaFalla"].Value);

            txtDescripcionFalla.Text =
                fila.Cells["Descripcion"].Value.ToString();

            cmbEstadoFalla.Text =
                fila.Cells["Estado"].Value.ToString();
        }

        // =====================================================
        // BUSCAR FALLA
        // =====================================================

        private void btnBuscarFalla_Click(
            object sender,
            EventArgs e)
        {
            string buscar =
                txtBuscarFalla.Text.Trim();

            if (string.IsNullOrWhiteSpace(buscar))
            {
                CargarFallas();
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
                            F.IdFalla,
                            F.IdMaquinaria,
                            M.Nombre AS Maquinaria,
                            F.FechaFalla,
                            F.Descripcion,
                            F.Estado
                        FROM Fallas F
                        INNER JOIN Maquinarias M
                            ON F.IdMaquinaria =
                               M.IdMaquinaria
                        WHERE
                            M.Nombre LIKE @Buscar
                            OR F.Descripcion LIKE @Buscar
                            OR F.Estado LIKE @Buscar
                        ORDER BY F.IdFalla DESC";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);

                    da.SelectCommand.Parameters.AddWithValue(
                        "@Buscar",
                        "%" + buscar + "%");

                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    dgvFallas.DataSource = dt;

                    FormatearColumnasFallas();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al buscar falla:\n" +
                        ex.Message);
                }
            }
        }

        // =====================================================
        // MOSTRAR TODAS LAS FALLAS
        // =====================================================

        private void btnMostrarTodasFallas_Click(
            object sender,
            EventArgs e)
        {
            txtBuscarFalla.Clear();

            CargarFallas();

            LimpiarFalla();
        }

        // =====================================================
        // REGISTRAR FALLA
        // =====================================================

        private void btnRegistrarFalla_Click(
            object sender,
            EventArgs e)
        {
            if (cmbMaquinariaFalla.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione una maquinaria.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtDescripcionFalla.Text))
            {
                MessageBox.Show(
                    "Ingrese una descripción de la falla.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (cmbEstadoFalla.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione el estado de la falla.");

                return;
            }

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                conn.Open();

                SqlTransaction transaction =
                    conn.BeginTransaction();

                try
                {
                    int idMaquinaria =
                        Convert.ToInt32(
                            cmbMaquinariaFalla.SelectedValue);

                    string insertar = @"
                        INSERT INTO Fallas
                        (
                            IdMaquinaria,
                            FechaFalla,
                            Descripcion,
                            Estado
                        )
                        VALUES
                        (
                            @IdMaquinaria,
                            @Fecha,
                            @Descripcion,
                            @Estado
                        )";

                    using (SqlCommand cmd =
                           new SqlCommand(
                               insertar,
                               conn,
                               transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdMaquinaria",
                            idMaquinaria);

                        cmd.Parameters.AddWithValue(
                            "@Fecha",
                            dtpFechaFalla.Value.Date);

                        cmd.Parameters.AddWithValue(
                            "@Descripcion",
                            txtDescripcionFalla.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Estado",
                            cmbEstadoFalla.Text);

                        cmd.ExecuteNonQuery();
                    }

                    // AL REGISTRAR UNA FALLA
                    // LA MAQUINARIA PASA A MANTENIMIENTO

                    string actualizarMaquina = @"
                        UPDATE Maquinarias
                        SET Estado = 'En mantenimiento'
                        WHERE IdMaquinaria = @Id";

                    using (SqlCommand cmdMaquina =
                           new SqlCommand(
                               actualizarMaquina,
                               conn,
                               transaction))
                    {
                        cmdMaquina.Parameters.AddWithValue(
                            "@Id",
                            idMaquinaria);

                        cmdMaquina.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    MessageBox.Show(
                        "Falla técnica registrada correctamente.",
                        "Registro exitoso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    CargarFallas();
                    CargarMaquinarias();

                    LimpiarFalla();

                    ActualizarKPIs();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    MessageBox.Show(
                        "Error al registrar la falla:\n" +
                        ex.Message);
                }
            }
        }

        // =====================================================
        // MODIFICAR FALLA
        // =====================================================

        private void btnModificarFalla_Click(
            object sender,
            EventArgs e)
        {
            if (idFallaSeleccionada == 0)
            {
                MessageBox.Show(
                    "Seleccione una falla.");

                return;
            }

            if (cmbMaquinariaFalla.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione una maquinaria.");

                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtDescripcionFalla.Text))
            {
                MessageBox.Show(
                    "Ingrese una descripción.");

                return;
            }

            DialogResult respuesta =
                MessageBox.Show(
                    "¿Desea modificar la falla seleccionada?",
                    "Confirmar modificación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        UPDATE Fallas
                        SET
                            IdMaquinaria = @IdMaquinaria,
                            FechaFalla = @Fecha,
                            Descripcion = @Descripcion,
                            Estado = @Estado
                        WHERE IdFalla = @IdFalla";

                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdMaquinaria",
                            Convert.ToInt32(
                                cmbMaquinariaFalla.SelectedValue));

                        cmd.Parameters.AddWithValue(
                            "@Fecha",
                            dtpFechaFalla.Value.Date);

                        cmd.Parameters.AddWithValue(
                            "@Descripcion",
                            txtDescripcionFalla.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Estado",
                            cmbEstadoFalla.Text);

                        cmd.Parameters.AddWithValue(
                            "@IdFalla",
                            idFallaSeleccionada);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show(
                        "Falla modificada correctamente.");

                    CargarFallas();
                    CargarMaquinarias();

                    LimpiarFalla();

                    ActualizarKPIs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al modificar la falla:\n" +
                        ex.Message);
                }
            }
        }

        // =====================================================
        // ELIMINAR FALLA
        // =====================================================

        private void btnEliminarFalla_Click(
            object sender,
            EventArgs e)
        {
            if (idFallaSeleccionada == 0)
            {
                MessageBox.Show(
                    "Seleccione una falla.");

                return;
            }

            DialogResult respuesta =
                MessageBox.Show(
                    "¿Desea eliminar esta falla?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        DELETE FROM Fallas
                        WHERE IdFalla = @Id";

                    SqlCommand cmd =
                        new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue(
                        "@Id",
                        idFallaSeleccionada);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show(
                        "Falla eliminada correctamente.");

                    CargarFallas();

                    LimpiarFalla();

                    ActualizarKPIs();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547)
                    {
                        MessageBox.Show(
                            "No se puede eliminar esta falla porque posee una orden de trabajo registrada.",
                            "No se puede eliminar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show(
                            ex.Message);
                    }
                }
            }
        }

        // =====================================================
        // LIMPIAR FALLA
        // =====================================================

        private void btnLimpiarFalla_Click(
            object sender,
            EventArgs e)
        {
            LimpiarFalla();
        }

        private void LimpiarFalla()
        {
            idFallaSeleccionada = 0;

            cmbMaquinariaFalla.SelectedIndex = -1;

            dtpFechaFalla.Value = DateTime.Today;

            txtDescripcionFalla.Clear();

            if (cmbEstadoFalla.Items.Count > 0)
                cmbEstadoFalla.SelectedIndex = 0;

            dgvFallas.ClearSelection();
        }

        // =====================================================
        // EMITIR ORDEN DE TRABAJO
        // =====================================================

        private void btnEmitirOrdenTrabajo_Click(
            object sender,
            EventArgs e)
        {
            if (idFallaSeleccionada == 0)
            {
                MessageBox.Show(
                    "Seleccione primero una falla para emitir la orden de trabajo.",
                    "Seleccionar falla",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            Form12 orden = new Form12(idFallaSeleccionada);

            orden.ShowDialog();

            // AL CERRAR FORM12 RECARGAMOS TODO

            CargarFallas();
            CargarMaquinarias();
            CargarTecnicos();

            ActualizarKPIs();
        }

        // =====================================================
        // VOLVER
        // =====================================================

        private void btnVolver_Click(object sender,  EventArgs e)
        {
            Form5 menu =
                new Form5();

            menu.Show();

            this.Close();
        }

        private void dgvTecnicos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void btnVolver_Click_1(object sender, EventArgs e)
        {
            Form5 frm = new Form5();

            frm.Show();

            this.Close();
        }
    }
}


