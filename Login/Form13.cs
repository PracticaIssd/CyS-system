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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Diagnostics;
using Font = System.Drawing.Font;


namespace Login
{
    public partial class Form13 : Form
    {
        // =====================================================
        // CONEXIÓN
        // =====================================================

        private readonly string conexion =
    @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";


        // =====================================================
        // VARIABLES
        // =====================================================

        private int idClienteSeleccionado = 0;
        private int idOperacionSeleccionada = 0;
        private decimal importeOperacionSeleccionada = 0;


        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public Form13()
        {
            InitializeComponent();

            // Eventos
            this.Load += Form13_Load;

            btnBuscarClienteFactura.Click += btnBuscarClienteFactura_Click;
            btnCargarFactura.Click += btnCargarFactura_Click;
            btnLimpiarFactura.Click += btnLimpiarFactura_Click;
            btnEmitirFactura.Click += btnEmitirFactura_Click;
            btnAbrirPdf.Click += btnAbrirPdf_Click;

            cmbTipoOperacionFactura.SelectedIndexChanged +=
                cmbTipoOperacionFactura_SelectedIndexChanged;
            cmbOperacionFactura.SelectedIndexChanged +=
    cmbOperacionFactura_SelectedIndexChanged;

            btnBuscarCobros.Click += btnBuscarCobros_Click;
            btnMostrarTodosCobros.Click += btnMostrarTodosCobros_Click;
        }
        private void cmbOperacionFactura_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            txtFormaPagoFactura.Clear();

            if (cmbOperacionFactura.SelectedValue == null ||
                cmbTipoOperacionFactura.SelectedItem == null)
                return;

            // Evita que se ejecute mientras se está cargando el DataSource
            if (!(cmbOperacionFactura.SelectedValue is int))
                return;

            int idOperacion =
                Convert.ToInt32(cmbOperacionFactura.SelectedValue);

            string tipo =
                cmbTipoOperacionFactura.SelectedItem.ToString();

            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query;

                    if (tipo == "Pedido")
                    {
                        query = @"
                    SELECT FormaPago
                    FROM Pedidos
                    WHERE IdPedido = @IdOperacion";
                    }
                    else
                    {
                        query = @"
                    SELECT FormaPago
                    FROM Alquileres
                    WHERE IdAlquiler = @IdOperacion";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdOperacion",
                            idOperacion);

                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null &&
                            resultado != DBNull.Value)
                        {
                            txtFormaPagoFactura.Text =
                                resultado.ToString();
                        }
                        else
                        {
                            txtFormaPagoFactura.Text =
                                "Sin registrar";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al obtener la forma de pago:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =====================================================
        // LOAD
        // =====================================================

        private void Form13_Load(object sender, EventArgs e)
        {
            // Tipo de operación
            cmbTipoOperacionFactura.Items.Clear();
            cmbTipoOperacionFactura.Items.Add("Pedido");
            cmbTipoOperacionFactura.Items.Add("Alquiler");
            cmbTipoOperacionFactura.SelectedIndex = 0;

            // Forma de pago: se trae automáticamente del Pedido/Alquiler
            txtFormaPagoFactura.ReadOnly = true;
            txtFormaPagoFactura.Clear();

            // Campos del cliente
            txtClienteFactura.ReadOnly = true;
            txtTelefonoFactura.ReadOnly = true;
            txtEmailFactura.ReadOnly = true;

            // Vista previa
            pnlVistaFactura.Visible = false;

            lblClienteVista.Text = "-";
            lblDniVista.Text = "-";
            lblFechaFactura.Text = "--/--/----";
            lblNumeroFactura.Text = "Nº FAC-000000";
            lblFormaPagoVista.Text = "-";
            lblTotalFactura.Text = "$0,00";

            // Consulta de cobros
            lblClienteConsultaCobro.Text = "-";
            lblCantidadCobros.Text = "0";
            lblTotalCobrado.Text = "$0,00";

            ConfigurarGridDetalle();
            ConfigurarGridCobros();

            // Al iniciar podemos mostrar todos los cobros
            CargarTodosLosCobros();
        }


        // =====================================================
        // CONFIGURAR GRID DETALLE FACTURA
        // =====================================================

        private void ConfigurarGridDetalle()
        {
            dgvDetalleFactura.ReadOnly = true;
            dgvDetalleFactura.AllowUserToAddRows = false;
            dgvDetalleFactura.AllowUserToDeleteRows = false;
            dgvDetalleFactura.RowHeadersVisible = false;
            dgvDetalleFactura.MultiSelect = false;

            dgvDetalleFactura.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvDetalleFactura.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvDetalleFactura.BackgroundColor = Color.White;
            dgvDetalleFactura.BorderStyle = BorderStyle.None;

            dgvDetalleFactura.EnableHeadersVisualStyles = false;

            dgvDetalleFactura.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(8, 43, 82);

            dgvDetalleFactura.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvDetalleFactura.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvDetalleFactura.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvDetalleFactura.ColumnHeadersHeight = 38;

            dgvDetalleFactura.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);

            dgvDetalleFactura.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 235, 245);

            dgvDetalleFactura.DefaultCellStyle.SelectionForeColor =
                Color.Black;
        }


        // =====================================================
        // CONFIGURAR GRID COBROS
        // =====================================================

        private void ConfigurarGridCobros()
        {
            dgvCobros.ReadOnly = true;
            dgvCobros.AllowUserToAddRows = false;
            dgvCobros.AllowUserToDeleteRows = false;
            dgvCobros.RowHeadersVisible = false;
            dgvCobros.MultiSelect = false;

            dgvCobros.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvCobros.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvCobros.BackgroundColor = Color.White;
            dgvCobros.BorderStyle = BorderStyle.None;

            dgvCobros.EnableHeadersVisualStyles = false;

            dgvCobros.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(8, 43, 82);

            dgvCobros.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvCobros.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvCobros.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvCobros.ColumnHeadersHeight = 40;

            dgvCobros.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);

            dgvCobros.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 235, 245);

            dgvCobros.DefaultCellStyle.SelectionForeColor =
                Color.Black;
        }


        // =====================================================
        // BUSCAR CLIENTE PARA FACTURAR
        // =====================================================

        private void btnBuscarClienteFactura_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDniFactura.Text))
            {
                MessageBox.Show(
                    "Ingrese el DNI del cliente.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            Id,
                            Nombre,
                            DNI,
                            Telefono,
                            Gmail
                        FROM Clientes
                        WHERE DNI = @DNI";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@DNI",
                            txtDniFactura.Text.Trim());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idClienteSeleccionado =
                                    Convert.ToInt32(reader["Id"]);

                                txtClienteFactura.Text =
                                    reader["Nombre"].ToString();

                                txtTelefonoFactura.Text =
                                    reader["Telefono"].ToString();

                                txtEmailFactura.Text =
                                    reader["Gmail"].ToString();

                                lblClienteVista.Text =
                                    reader["Nombre"].ToString();

                                lblDniVista.Text =
                                    reader["DNI"].ToString();
                            }
                            else
                            {
                                MessageBox.Show(
                                    "No se encontró un cliente con ese DNI.",
                                    "Cliente no encontrado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                                LimpiarDatosCliente();

                                return;
                            }
                        }
                    }

                    CargarOperacionesActivas();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al buscar el cliente:\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }


        // =====================================================
        // CAMBIAR TIPO PEDIDO / ALQUILER
        // =====================================================

        private void cmbTipoOperacionFactura_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (idClienteSeleccionado > 0)
            {
                CargarOperacionesActivas();
            }
        }


        // =====================================================
        // CARGAR OPERACIONES ACTIVAS
        // =====================================================

        private void CargarOperacionesActivas()
        {
            if (idClienteSeleccionado == 0)
                return;

            if (cmbTipoOperacionFactura.SelectedItem == null)
                return;

            string tipo =
                cmbTipoOperacionFactura.SelectedItem.ToString();

            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    DataTable dt = new DataTable();

                    string query;

                    // =================================================
                    // PEDIDOS
                    // =================================================

                    if (tipo == "Pedido")
                    {
                        query = @"
                            SELECT
                                P.IdPedido AS IdOperacion,

                                'Pedido Nº ' +
                                CAST(P.IdPedido AS VARCHAR(20)) +
                                ' - $' +
                                CAST(P.ImporteTotal AS VARCHAR(30))
                                AS Descripcion,

                                P.ImporteTotal

                            FROM Pedidos P

                            WHERE P.DniCliente = @DNI

                            AND P.Estado <> 'Cancelado'

                            AND NOT EXISTS
                            (
                                SELECT 1
                                FROM Facturas F
                                WHERE F.TipoOperacion = 'Pedido'
                                AND F.IdOperacion = P.IdPedido
                            )

                            ORDER BY P.IdPedido DESC";
                    }

                    // =================================================
                    // ALQUILERES
                    // =================================================

                    else
                    {
                        query = @"
                            SELECT
                                A.IdAlquiler AS IdOperacion,

                                'Alquiler Nº ' +
                                CAST(A.IdAlquiler AS VARCHAR(20)) +
                                ' - ' +
                                A.Recurso +
                                ' - $' +
                                CAST(A.ImporteTotal AS VARCHAR(30))
                                AS Descripcion,

                                A.ImporteTotal

                            FROM Alquileres A

                            WHERE A.IdCliente = @IdCliente

                            AND A.Estado <> 'Cancelado'

                            AND NOT EXISTS
                            (
                                SELECT 1
                                FROM Facturas F
                                WHERE F.TipoOperacion = 'Alquiler'
                                AND F.IdOperacion = A.IdAlquiler
                            )

                            ORDER BY A.IdAlquiler DESC";
                    }


                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@DNI",
                            txtDniFactura.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@IdCliente",
                            idClienteSeleccionado);

                        SqlDataAdapter da =
                            new SqlDataAdapter(cmd);

                        da.Fill(dt);
                    }


                    cmbOperacionFactura.DataSource = null;


                    if (dt.Rows.Count > 0)
                    {
                        cmbOperacionFactura.DisplayMember =
                            "Descripcion";

                        cmbOperacionFactura.ValueMember =
                            "IdOperacion";

                        cmbOperacionFactura.DataSource = dt;
                    }
                    else
                    {
                        cmbOperacionFactura.DataSource = null;

                        MessageBox.Show(
                            "El cliente no posee " +
                            tipo.ToLower() +
                            "s disponibles para facturar.",
                            "Sin operaciones",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar las operaciones:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }


        // =====================================================
        // CARGAR VISTA PREVIA
        // =====================================================

        private void btnCargarFactura_Click(object sender, EventArgs e)
        {
            if (idClienteSeleccionado == 0)
            {
                MessageBox.Show(
                    "Primero debe buscar un cliente.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (cmbOperacionFactura.SelectedValue == null)
            {
                MessageBox.Show(
                    "Seleccione una operación para facturar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            idOperacionSeleccionada =
                Convert.ToInt32(cmbOperacionFactura.SelectedValue);

            string tipo =
                cmbTipoOperacionFactura.SelectedItem.ToString();

            txtFormaPagoFactura.Clear();

            if (tipo == "Pedido")
                CargarVistaPedido();
            else
                CargarVistaAlquiler();

            if (string.IsNullOrWhiteSpace(txtFormaPagoFactura.Text))
            {
                MessageBox.Show(
                    "La operación seleccionada no tiene una forma de pago registrada.",
                    "Forma de pago no encontrada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                pnlVistaFactura.Visible = false;
                return;
            }

            lblFechaFactura.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblFormaPagoVista.Text = txtFormaPagoFactura.Text;
            pnlVistaFactura.Visible = true;
        }


        // =====================================================
        // VISTA PREVIA PEDIDO
        // =====================================================

        private void CargarVistaPedido()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    // =============================================
                    // OBTENER TOTAL Y FORMA DE PAGO
                    // =============================================

                    string queryPedido = @"
                    SELECT ImporteTotal, FormaPago
                    FROM Pedidos
                    WHERE IdPedido = @IdPedido";


                    using (SqlCommand cmd = new SqlCommand(queryPedido, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdPedido",
                            idOperacionSeleccionada);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                importeOperacionSeleccionada =
                                    Convert.ToDecimal(reader["ImporteTotal"]);

                                txtFormaPagoFactura.Text =
                                    reader["FormaPago"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("No se encontró el pedido.");
                                return;
                            }
                        }
                    }

                    // =============================================
                    // DETALLE DEL PEDIDO
                    // =============================================

                    string queryDetalle = @"
                SELECT
                    Trabajo AS Descripcion,
                    Medida,
                    Papel,
                    Cantidad,
                    Observaciones
                FROM DetallePedido
                WHERE IdPedido = @IdPedido";

                    using (SqlCommand cmd =
                           new SqlCommand(queryDetalle, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdPedido",
                            idOperacionSeleccionada);

                        DataTable dt = new DataTable();

                        SqlDataAdapter da =
                            new SqlDataAdapter(cmd);

                        da.Fill(dt);

                        dgvDetalleFactura.DataSource = dt;
                    }

                    lblTotalFactura.Text =
                        importeOperacionSeleccionada.ToString("C2");

                    lblNumeroFactura.Text =
                        "Pedido Nº " +
                        idOperacionSeleccionada;

                    FormatearColumnasDetallePedido();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar el pedido:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        private void FormatearColumnasDetallePedido()
        {
            if (dgvDetalleFactura.Columns.Contains("Descripcion"))
            {
                dgvDetalleFactura.Columns["Descripcion"].HeaderText =
                    "TRABAJO";

                dgvDetalleFactura.Columns["Descripcion"].FillWeight =
                    130;
            }

            if (dgvDetalleFactura.Columns.Contains("Medida"))
            {
                dgvDetalleFactura.Columns["Medida"].HeaderText =
                    "MEDIDA";

                dgvDetalleFactura.Columns["Medida"].FillWeight =
                    70;
            }

            if (dgvDetalleFactura.Columns.Contains("Papel"))
            {
                dgvDetalleFactura.Columns["Papel"].HeaderText =
                    "PAPEL";

                dgvDetalleFactura.Columns["Papel"].FillWeight =
                    80;
            }

            if (dgvDetalleFactura.Columns.Contains("Cantidad"))
            {
                dgvDetalleFactura.Columns["Cantidad"].HeaderText =
                    "CANT.";

                dgvDetalleFactura.Columns["Cantidad"].FillWeight =
                    55;
            }

            if (dgvDetalleFactura.Columns.Contains("Observaciones"))
            {
                dgvDetalleFactura.Columns["Observaciones"].HeaderText =
                    "OBSERVACIONES";

                dgvDetalleFactura.Columns["Observaciones"].FillWeight =
                    130;
            }
        }

        // =====================================================
        // VISTA PREVIA ALQUILER
        // =====================================================

        private void CargarVistaAlquiler()
        {
            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                    SELECT
                    Recurso AS Descripcion,
                    CantidadDias AS Cantidad,
                    PrecioPorDia AS Precio,
                    ImporteTotal AS Subtotal,
                    FormaPago
                    FROM Alquileres
                    WHERE IdAlquiler = @IdAlquiler";

                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdAlquiler",
                            idOperacionSeleccionada);

                        DataTable dt = new DataTable();

                        SqlDataAdapter da =
                            new SqlDataAdapter(cmd);

                        da.Fill(dt);
                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("No se encontró el alquiler.");
                            return;
                        }

                        txtFormaPagoFactura.Text =
                            dt.Rows[0]["FormaPago"].ToString();

                        importeOperacionSeleccionada =
                            Convert.ToDecimal(dt.Rows[0]["Subtotal"]);

                        // FormaPago se usa en el TextBox, no se muestra en la grilla
                        dt.Columns.Remove("FormaPago");
                        dgvDetalleFactura.DataSource = dt;

                        lblTotalFactura.Text =
                            importeOperacionSeleccionada
                            .ToString("C2");

                        lblNumeroFactura.Text =
                            "Alquiler Nº " +
                            idOperacionSeleccionada;

                        FormatearColumnasDetalle();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar el alquiler:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =====================================================
        // FORMATEAR COLUMNAS DETALLE
        // =====================================================

        private void FormatearColumnasDetalle()
        {
            if (dgvDetalleFactura.Columns.Contains("Descripcion"))
            {
                dgvDetalleFactura.Columns["Descripcion"].HeaderText =
                    "DESCRIPCIÓN";

                dgvDetalleFactura.Columns["Descripcion"].FillWeight =
                    180;
            }


            if (dgvDetalleFactura.Columns.Contains("Cantidad"))
            {
                dgvDetalleFactura.Columns["Cantidad"].HeaderText =
                    "CANTIDAD";

                dgvDetalleFactura.Columns["Cantidad"].FillWeight =
                    70;
            }


            if (dgvDetalleFactura.Columns.Contains("Precio"))
            {
                dgvDetalleFactura.Columns["Precio"].HeaderText =
                    "PRECIO";

                dgvDetalleFactura.Columns["Precio"].FillWeight =
                    80;

                dgvDetalleFactura.Columns["Precio"]
                    .DefaultCellStyle.Format = "C2";
            }


            if (dgvDetalleFactura.Columns.Contains("Subtotal"))
            {
                dgvDetalleFactura.Columns["Subtotal"].HeaderText =
                    "SUBTOTAL";

                dgvDetalleFactura.Columns["Subtotal"].FillWeight =
                    90;

                dgvDetalleFactura.Columns["Subtotal"]
                    .DefaultCellStyle.Format = "C2";
            }
        }


        // =====================================================
        // LIMPIAR FACTURA
        // =====================================================

        private void btnLimpiarFactura_Click(object sender, EventArgs e)
        {
            txtDniFactura.Clear();

            LimpiarDatosCliente();

            cmbTipoOperacionFactura.SelectedIndex = 0;
            txtFormaPagoFactura.Clear();

            txtDniFactura.Focus();
        }


        private void LimpiarDatosCliente()
        {
            idClienteSeleccionado = 0;
            idOperacionSeleccionada = 0;
            importeOperacionSeleccionada = 0;


            txtClienteFactura.Clear();
            txtTelefonoFactura.Clear();
            txtEmailFactura.Clear();
            txtFormaPagoFactura.Clear();


            cmbOperacionFactura.DataSource = null;


            dgvDetalleFactura.DataSource = null;


            lblClienteVista.Text = "-";
            lblDniVista.Text = "-";
            lblFechaFactura.Text = "--/--/----";
            lblNumeroFactura.Text = "Nº FAC-000000";
            lblFormaPagoVista.Text = "-";
            lblTotalFactura.Text = "$0,00";


            pnlVistaFactura.Visible = false;
        }


        // =====================================================
        // CONSULTAR COBROS POR DNI
        // =====================================================

        private void btnBuscarCobros_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                txtDniConsultaCobro.Text))
            {
                MessageBox.Show(
                    "Ingrese el DNI del cliente.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }


            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();


                    // =============================================
                    // PRIMERO BUSCAMOS EL CLIENTE
                    // =============================================

                    string queryCliente = @"
                        SELECT Id, Nombre
                        FROM Clientes
                        WHERE DNI = @DNI";


                    int idCliente = 0;


                    using (SqlCommand cmd =
                           new SqlCommand(queryCliente, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@DNI",
                            txtDniConsultaCobro.Text.Trim());


                        using (SqlDataReader reader =
                               cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idCliente =
                                    Convert.ToInt32(
                                        reader["Id"]);


                                lblClienteConsultaCobro.Text =
                                    reader["Nombre"].ToString();
                            }
                            else
                            {
                                MessageBox.Show(
                                    "No se encontró un cliente con ese DNI.",
                                    "Cliente no encontrado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);


                                lblClienteConsultaCobro.Text = "-";
                                dgvCobros.DataSource = null;

                                lblCantidadCobros.Text = "0";
                                lblTotalCobrado.Text = "$0,00";

                                return;
                            }
                        }
                    }


                    // =============================================
                    // BUSCAR COBROS DEL CLIENTE
                    // =============================================

                    string queryCobros = @"
                        SELECT
                            C.IdCobro,
                            C.FechaCobro,
                            F.NumeroFactura,
                            CL.Nombre AS Cliente,
                            CL.DNI,
                            F.TipoOperacion,
                            F.IdOperacion,
                            C.FormaPago,
                            C.Importe,
                            F.RutaPDF

                        FROM Cobros C

                        INNER JOIN Facturas F
                            ON C.IdFactura = F.IdFactura

                        INNER JOIN Clientes CL
                            ON F.IdCliente = CL.Id

                        WHERE CL.Id = @IdCliente

                        ORDER BY C.FechaCobro DESC,
                                 C.IdCobro DESC";


                    using (SqlCommand cmd =
                           new SqlCommand(queryCobros, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdCliente",
                            idCliente);


                        DataTable dt =
                            new DataTable();


                        SqlDataAdapter da =
                            new SqlDataAdapter(cmd);


                        da.Fill(dt);


                        dgvCobros.DataSource = dt;


                        ActualizarResumenCobros(dt);


                        BeginInvoke(
                            new MethodInvoker(() =>
                            {
                                FormatearColumnasCobros();
                            }));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al consultar los cobros:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }


        // =====================================================
        // MOSTRAR TODOS LOS COBROS
        // =====================================================

        private void btnMostrarTodosCobros_Click(
            object sender,
            EventArgs e)
        {
            txtDniConsultaCobro.Clear();

            lblClienteConsultaCobro.Text =
                "Todos los clientes";

            CargarTodosLosCobros();
        }


        // =====================================================
        // CARGAR TODOS LOS COBROS
        // =====================================================

        private void CargarTodosLosCobros()
        {
            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();


                    string query = @"
                        SELECT
                            C.IdCobro,
                            C.FechaCobro,
                            F.NumeroFactura,
                            CL.Nombre AS Cliente,
                            CL.DNI,
                            F.TipoOperacion,
                            F.IdOperacion,
                            C.FormaPago,
                            C.Importe,
                            F.RutaPDF

                        FROM Cobros C

                        INNER JOIN Facturas F
                            ON C.IdFactura = F.IdFactura

                        INNER JOIN Clientes CL
                            ON F.IdCliente = CL.Id

                        ORDER BY C.FechaCobro DESC,
                                 C.IdCobro DESC";


                    DataTable dt =
                        new DataTable();


                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);


                    da.Fill(dt);


                    dgvCobros.DataSource = dt;


                    ActualizarResumenCobros(dt);


                    BeginInvoke(
                        new MethodInvoker(() =>
                        {
                            FormatearColumnasCobros();
                        }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar los cobros:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }


        // =====================================================
        // RESUMEN DE COBROS
        // =====================================================

        private void ActualizarResumenCobros(DataTable dt)
        {
            int cantidad = dt.Rows.Count;

            decimal total = 0;


            foreach (DataRow fila in dt.Rows)
            {
                if (fila["Importe"] != DBNull.Value)
                {
                    total +=
                        Convert.ToDecimal(
                            fila["Importe"]);
                }
            }


            lblCantidadCobros.Text =
                cantidad.ToString();


            lblTotalCobrado.Text =
                total.ToString("C2");
        }


        // =====================================================
        // FORMATEAR COLUMNAS COBROS
        // =====================================================

        private void FormatearColumnasCobros()
        {
            if (dgvCobros.Columns.Contains("IdCobro"))
            {
                dgvCobros.Columns["IdCobro"].HeaderText =
                    "ID";

                dgvCobros.Columns["IdCobro"].FillWeight =
                    35;
            }


            if (dgvCobros.Columns.Contains("FechaCobro"))
            {
                dgvCobros.Columns["FechaCobro"].HeaderText =
                    "FECHA";

                dgvCobros.Columns["FechaCobro"]
                    .DefaultCellStyle.Format =
                    "dd/MM/yyyy";

                dgvCobros.Columns["FechaCobro"].FillWeight =
                    70;
            }


            if (dgvCobros.Columns.Contains("NumeroFactura"))
            {
                dgvCobros.Columns["NumeroFactura"].HeaderText =
                    "FACTURA";

                dgvCobros.Columns["NumeroFactura"].FillWeight =
                    80;
            }


            if (dgvCobros.Columns.Contains("Cliente"))
            {
                dgvCobros.Columns["Cliente"].HeaderText =
                    "CLIENTE";

                dgvCobros.Columns["Cliente"].FillWeight =
                    110;
            }


            if (dgvCobros.Columns.Contains("DNI"))
            {
                dgvCobros.Columns["DNI"].HeaderText =
                    "DNI";

                dgvCobros.Columns["DNI"].FillWeight =
                    70;
            }


            if (dgvCobros.Columns.Contains("TipoOperacion"))
            {
                dgvCobros.Columns["TipoOperacion"].HeaderText =
                    "TIPO";

                dgvCobros.Columns["TipoOperacion"].FillWeight =
                    70;
            }


            if (dgvCobros.Columns.Contains("IdOperacion"))
            {
                dgvCobros.Columns["IdOperacion"].HeaderText =
                    "Nº OP.";

                dgvCobros.Columns["IdOperacion"].FillWeight =
                    55;
            }


            if (dgvCobros.Columns.Contains("FormaPago"))
            {
                dgvCobros.Columns["FormaPago"].HeaderText =
                    "FORMA DE PAGO";

                dgvCobros.Columns["FormaPago"].FillWeight =
                    100;
            }


            if (dgvCobros.Columns.Contains("Importe"))
            {
                dgvCobros.Columns["Importe"].HeaderText =
                    "IMPORTE";

                dgvCobros.Columns["Importe"]
                    .DefaultCellStyle.Format =
                    "C2";

                dgvCobros.Columns["Importe"].FillWeight =
                    80;
            }


            // La ruta PDF la necesitamos internamente,
            // pero no hace falta mostrarla al usuario.
            if (dgvCobros.Columns.Contains("RutaPDF"))
            {
                dgvCobros.Columns["RutaPDF"].Visible =
                    false;
            }
        }

        private void btnEmitirFactura_Click(object sender, EventArgs e)
        {
            if (idClienteSeleccionado == 0)
            {
                MessageBox.Show(
                    "Primero debe seleccionar un cliente.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (idOperacionSeleccionada == 0)
            {
                MessageBox.Show(
                    "Primero debe cargar una operación.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(txtFormaPagoFactura.Text))
            {
                MessageBox.Show(
                    "La operación no tiene una forma de pago registrada.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (importeOperacionSeleccionada <= 0)
            {
                MessageBox.Show(
                    "El importe de la operación no es válido.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string tipoOperacion =
                cmbTipoOperacionFactura.SelectedItem.ToString();

            string formaPago =
                txtFormaPagoFactura.Text.Trim();

            DialogResult respuesta = MessageBox.Show(
                "¿Desea emitir la factura por " +
                importeOperacionSeleccionada.ToString("C2") + "?",
                "Confirmar factura",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();

                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // =====================================================
                    // VERIFICAR QUE NO ESTÉ FACTURADA
                    // =====================================================

                    string queryExiste = @"
                SELECT COUNT(*)
                FROM Facturas
                WHERE TipoOperacion = @TipoOperacion
                AND IdOperacion = @IdOperacion";

                    using (SqlCommand cmd =
                           new SqlCommand(queryExiste, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@TipoOperacion",
                            tipoOperacion);

                        cmd.Parameters.AddWithValue(
                            "@IdOperacion",
                            idOperacionSeleccionada);

                        int cantidad =
                            Convert.ToInt32(cmd.ExecuteScalar());

                        if (cantidad > 0)
                        {
                            transaction.Rollback();

                            MessageBox.Show(
                                "Esta operación ya fue facturada.",
                                "Factura existente",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            return;
                        }
                    }

                    // =====================================================
                    // CREAR FACTURA
                    // =====================================================

                    string queryFactura = @"
                INSERT INTO Facturas
                (
                    NumeroFactura,
                    IdCliente,
                    TipoOperacion,
                    IdOperacion,
                    FechaEmision,
                    ImporteTotal,
                    FormaPago,
                    RutaPDF
                )
                VALUES
                (
                    @NumeroFactura,
                    @IdCliente,
                    @TipoOperacion,
                    @IdOperacion,
                    @FechaEmision,
                    @ImporteTotal,
                    @FormaPago,
                    NULL
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    int idFactura;

                    // Primero ponemos un número temporal.
                    // Después usamos el ID generado para crear FAC-000001.
                    string numeroTemporal =
                        "TEMP-" + DateTime.Now.ToString("HHmmss");
                    using (SqlCommand cmd =
                           new SqlCommand(queryFactura, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@NumeroFactura",
                            numeroTemporal);

                        cmd.Parameters.AddWithValue(
                            "@IdCliente",
                            idClienteSeleccionado);

                        cmd.Parameters.AddWithValue(
                            "@TipoOperacion",
                            tipoOperacion);

                        cmd.Parameters.AddWithValue(
                            "@IdOperacion",
                            idOperacionSeleccionada);

                        cmd.Parameters.AddWithValue(
                            "@FechaEmision",
                            DateTime.Now);

                        cmd.Parameters.AddWithValue(
                            "@ImporteTotal",
                            importeOperacionSeleccionada);

                        cmd.Parameters.AddWithValue(
                            "@FormaPago",
                            formaPago);

                        idFactura =
                            Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // =====================================================
                    // GENERAR NÚMERO DE FACTURA
                    // =====================================================

                    string numeroFactura =
                        "FAC-" + idFactura.ToString("D6");

                    string queryNumero = @"
                UPDATE Facturas
                SET NumeroFactura = @NumeroFactura
                WHERE IdFactura = @IdFactura";

                    using (SqlCommand cmd =
                           new SqlCommand(queryNumero, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@NumeroFactura",
                            numeroFactura);

                        cmd.Parameters.AddWithValue(
                            "@IdFactura",
                            idFactura);

                        cmd.ExecuteNonQuery();
                    }

                    // =====================================================
                    // REGISTRAR COBRO
                    // =====================================================

                    string queryCobro = @"
                INSERT INTO Cobros
                (
                    IdFactura,
                    FechaCobro,
                    Importe,
                    FormaPago
                )
                VALUES
                (
                    @IdFactura,
                    @FechaCobro,
                    @Importe,
                    @FormaPago
                )";

                    using (SqlCommand cmd =
                           new SqlCommand(queryCobro, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdFactura",
                            idFactura);

                        cmd.Parameters.AddWithValue(
                            "@FechaCobro",
                            DateTime.Now);

                        cmd.Parameters.AddWithValue(
                            "@Importe",
                            importeOperacionSeleccionada);

                        cmd.Parameters.AddWithValue(
                            "@FormaPago",
                            formaPago);

                        cmd.ExecuteNonQuery();
                    }

                    // =====================================================
                    // GENERAR PDF
                    // =====================================================

                    string rutaPdf = GenerarFacturaPDF(
                        numeroFactura,
                        tipoOperacion,
                        idOperacionSeleccionada,
                        formaPago);

                    // =====================================================
                    // GUARDAR RUTA PDF
                    // =====================================================

                    string queryRuta = @"
                UPDATE Facturas
                SET RutaPDF = @RutaPDF
                WHERE IdFactura = @IdFactura";

                    using (SqlCommand cmd =
                           new SqlCommand(queryRuta, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@RutaPDF",
                            rutaPdf);

                        cmd.Parameters.AddWithValue(
                            "@IdFactura",
                            idFactura);

                        cmd.ExecuteNonQuery();
                    }

                    // =====================================================
                    // CONFIRMAR TODO
                    // =====================================================

                    transaction.Commit();

                    lblNumeroFactura.Text =
                        "Nº " + numeroFactura;

                    MessageBox.Show(
                        "Factura emitida correctamente.\n\n" +
                        "Factura: " + numeroFactura + "\n" +
                        "Total: " +
                        importeOperacionSeleccionada.ToString("C2"),
                        "Factura emitida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Actualizar consulta de cobros
                    CargarTodosLosCobros();

                    // Abrir automáticamente el PDF
                    AbrirPDF(rutaPdf);
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                    }

                    MessageBox.Show(
                        "No se pudo emitir la factura:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        private string GenerarFacturaPDF(
    string numeroFactura,
    string tipoOperacion,
    int idOperacion,
    string formaPago)
        {
            SaveFileDialog guardar = new SaveFileDialog();

            guardar.Filter = "PDF (*.pdf)|*.pdf";
            guardar.FileName = numeroFactura + ".pdf";

            if (guardar.ShowDialog() != DialogResult.OK)
            {
                throw new Exception("Se canceló la generación del PDF.");
            }

            Document doc = new Document(
                PageSize.A4,
                45,
                45,
                35,
                35);

            PdfWriter.GetInstance(
                doc,
                new FileStream(
                    guardar.FileName,
                    FileMode.Create));

            doc.Open();

            // =====================================================
            // COLORES
            // =====================================================

            BaseColor azul =
                new BaseColor(8, 43, 82);

            BaseColor celeste =
                new BaseColor(220, 235, 245);

            BaseColor grisLinea =
                new BaseColor(190, 190, 190);

            // =====================================================
            // FUENTES
            // =====================================================

            iTextSharp.text.Font fuenteNormal =
                FontFactory.GetFont(
                    FontFactory.HELVETICA,
                    10,
                    BaseColor.BLACK);

            iTextSharp.text.Font fuenteNegrita =
                FontFactory.GetFont(
                    FontFactory.HELVETICA_BOLD,
                    10,
                    BaseColor.BLACK);

            iTextSharp.text.Font fuenteNumero =
                FontFactory.GetFont(
                    FontFactory.HELVETICA_BOLD,
                    11,
                    BaseColor.BLACK);

            iTextSharp.text.Font fuenteCabecera =
                FontFactory.GetFont(
                    FontFactory.HELVETICA_BOLD,
                    9,
                    BaseColor.WHITE);

            iTextSharp.text.Font fuenteTotalTitulo =
                FontFactory.GetFont(
                    FontFactory.HELVETICA_BOLD,
                    13,
                    BaseColor.BLACK);

            iTextSharp.text.Font fuenteTotal =
                FontFactory.GetFont(
                    FontFactory.HELVETICA_BOLD,
                    17,
                    azul);

            // =====================================================
            // LOGO
            // =====================================================

            // =====================================================
            // LOGO
            // =====================================================

            PdfPTable tablaLogo = new PdfPTable(1);
            tablaLogo.WidthPercentage = 100;

            PdfPCell celdaLogo = new PdfPCell();

            celdaLogo.Border =
                iTextSharp.text.Rectangle.NO_BORDER;

            celdaLogo.HorizontalAlignment =
                Element.ALIGN_CENTER;

            celdaLogo.VerticalAlignment =
                Element.ALIGN_MIDDLE;

            try
            {
                string rutaLogo =
                    Path.Combine(
                        Application.StartupPath,
                        "Imagenes",
                        "logo.png");

                if (File.Exists(rutaLogo))
                {
                    iTextSharp.text.Image logo =
                        iTextSharp.text.Image.GetInstance(rutaLogo);

                    // Igual que en Orden de Trabajo
                    logo.ScaleAbsoluteWidth(90f);
                    logo.ScaleAbsoluteHeight(64f);

                    // Para factura lo queremos centrado
                    logo.Alignment =
                        iTextSharp.text.Image.ALIGN_CENTER;

                    // IMPORTANTE:
                    // usamos AddElement igual que en Form12
                    celdaLogo.AddElement(logo);

                    celdaLogo.PaddingTop = 3;
                    celdaLogo.PaddingBottom = 3;
                }
                else
                {
                    celdaLogo.Phrase =
                        new Phrase(
                            "IMPRENTA CENTRAL",
                            fuenteNumero);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo cargar el logo:\n" +
                    ex.Message);

                celdaLogo.Phrase =
                    new Phrase(
                        "IMPRENTA CENTRAL",
                        fuenteNumero);
            }

            tablaLogo.AddCell(celdaLogo);

            doc.Add(tablaLogo);

            // =====================================================
            // NÚMERO DE FACTURA
            // =====================================================

            Paragraph numero = new Paragraph(
                "Nº " + numeroFactura,
                fuenteNumero);

            numero.Alignment = Element.ALIGN_CENTER;
            numero.SpacingAfter = 10f;

            doc.Add(numero);

            // =====================================================
            // LÍNEA SUPERIOR
            // =====================================================

            PdfPTable lineaSuperior =
                new PdfPTable(1);

            lineaSuperior.WidthPercentage = 100;

            PdfPCell celdaLinea =
                new PdfPCell(new Phrase(" "));

            celdaLinea.FixedHeight = 1f;
            celdaLinea.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
            celdaLinea.BorderColor = grisLinea;
            celdaLinea.BorderWidthBottom = 0.8f;

            lineaSuperior.AddCell(celdaLinea);

            doc.Add(lineaSuperior);

            doc.Add(new Paragraph(" "));

            // =====================================================
            // DATOS DEL CLIENTE
            // =====================================================

            PdfPTable datos =
                new PdfPTable(2);

            datos.WidthPercentage = 100;

            datos.SetWidths(
                new float[] { 1.2f, 6f });

            // FECHA
            AgregarCeldaDato(
                datos,
                "Fecha:",
                DateTime.Now.ToString("dd/MM/yyyy"),
                fuenteNegrita,
                fuenteNormal);

            // CLIENTE
            AgregarCeldaDato(
                datos,
                "Cliente:",
                txtClienteFactura.Text,
                fuenteNegrita,
                fuenteNormal);

            // DNI
            AgregarCeldaDato(
                datos,
                "DNI:",
                txtDniFactura.Text,
                fuenteNegrita,
                fuenteNormal);

            doc.Add(datos);

            doc.Add(new Paragraph(" "));

            // =====================================================
            // LÍNEA ANTES DEL DETALLE
            // =====================================================

            PdfPTable lineaDetalle =
                new PdfPTable(1);

            lineaDetalle.WidthPercentage = 100;

            PdfPCell linea2 =
                new PdfPCell(new Phrase(" "));

            linea2.FixedHeight = 1f;
            linea2.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
            linea2.BorderColor = grisLinea;
            linea2.BorderWidthBottom = 0.8f;

            lineaDetalle.AddCell(linea2);

            doc.Add(lineaDetalle);

            doc.Add(new Paragraph(" "));

            // =====================================================
            // TABLA DE DETALLE
            // =====================================================

            int cantidadColumnas = 0;

            foreach (DataGridViewColumn columna
                     in dgvDetalleFactura.Columns)
            {
                if (columna.Visible)
                    cantidadColumnas++;
            }

            if (cantidadColumnas > 0)
            {
                PdfPTable tabla =
                    new PdfPTable(cantidadColumnas);

                tabla.WidthPercentage = 100;
                tabla.SpacingBefore = 3f;
                tabla.SpacingAfter = 15f;

                // ENCABEZADOS
                foreach (DataGridViewColumn columna
                         in dgvDetalleFactura.Columns)
                {
                    if (!columna.Visible)
                        continue;

                    PdfPCell celda =
                        new PdfPCell(
                            new Phrase(
                                columna.HeaderText,
                                fuenteCabecera));

                    celda.BackgroundColor = azul;
                    celda.HorizontalAlignment =
                        Element.ALIGN_CENTER;

                    celda.VerticalAlignment =
                        Element.ALIGN_MIDDLE;

                    celda.PaddingTop = 5f;
                    celda.PaddingBottom = 5f;

                    celda.BorderColor =
                        BaseColor.WHITE;

                    tabla.AddCell(celda);
                }

                // DATOS
                foreach (DataGridViewRow fila
                         in dgvDetalleFactura.Rows)
                {
                    if (fila.IsNewRow)
                        continue;

                    foreach (DataGridViewColumn columna
                             in dgvDetalleFactura.Columns)
                    {
                        if (!columna.Visible)
                            continue;

                        object valor =
                            fila.Cells[columna.Index].Value;

                        string texto =
                            valor == null
                            ? ""
                            : valor.ToString();

                        // Formato dinero
                        if ((columna.Name == "Precio" ||
                             columna.Name == "Subtotal") &&
                            decimal.TryParse(
                                texto,
                                out decimal importe))
                        {
                            texto =
                                importe.ToString("C2");
                        }

                        PdfPCell celda =
                            new PdfPCell(
                                new Phrase(
                                    texto,
                                    fuenteNormal));

                        celda.BackgroundColor = celeste;

                        celda.PaddingTop = 5f;
                        celda.PaddingBottom = 5f;
                        celda.PaddingLeft = 5f;

                        celda.BorderColor =
                            new BaseColor(
                                150,
                                170,
                                185);

                        // Cantidad y dinero alineados
                        if (columna.Name == "Cantidad")
                        {
                            celda.HorizontalAlignment =
                                Element.ALIGN_CENTER;
                        }
                        else if (
                            columna.Name == "Precio" ||
                            columna.Name == "Subtotal")
                        {
                            celda.HorizontalAlignment =
                                Element.ALIGN_RIGHT;
                        }
                        else
                        {
                            celda.HorizontalAlignment =
                                Element.ALIGN_LEFT;
                        }

                        tabla.AddCell(celda);
                    }
                }

                doc.Add(tabla);
            }

            // =====================================================
            // ESPACIO
            // =====================================================

            doc.Add(new Paragraph("\n\n"));

            // =====================================================
            // FORMA DE PAGO + TOTAL
            // =====================================================

            PdfPTable pie =
                new PdfPTable(2);

            pie.WidthPercentage = 100;

            pie.SetWidths(
                new float[] { 1.2f, 1f });

            // -------------------------
            // FORMA DE PAGO
            // -------------------------

            Paragraph pago =
                new Paragraph();

            pago.Add(
                new Chunk(
                    "Forma de pago:   ",
                    fuenteNegrita));

            pago.Add(
                new Chunk(
                    formaPago,
                    fuenteNormal));

            PdfPCell celdaPago =
                new PdfPCell(pago);

            celdaPago.Border =
                iTextSharp.text.Rectangle.NO_BORDER;

            celdaPago.VerticalAlignment =
                Element.ALIGN_MIDDLE;

            celdaPago.PaddingTop = 8f;

            pie.AddCell(celdaPago);

            // -------------------------
            // TOTAL
            // -------------------------

            Paragraph parrafoTotal =
                new Paragraph();

            parrafoTotal.Alignment =
                Element.ALIGN_RIGHT;

            parrafoTotal.Add(
                new Chunk(
                    "TOTAL:   ",
                    fuenteTotalTitulo));

            parrafoTotal.Add(
                new Chunk(
                    importeOperacionSeleccionada
                        .ToString("C2"),
                    fuenteTotal));

            PdfPCell celdaTotal =
                new PdfPCell(parrafoTotal);

            celdaTotal.Border =
iTextSharp.text.Rectangle.NO_BORDER;
            celdaTotal.HorizontalAlignment =
                Element.ALIGN_RIGHT;

            celdaTotal.PaddingTop = 5f;

            pie.AddCell(celdaTotal);

            doc.Add(pie);

            // =====================================================
            // PIE DEL DOCUMENTO
            // =====================================================

            Paragraph pieDocumento =
                new Paragraph(
                    "\n\nGracias por confiar en Imprenta Central",
                    fuenteNegrita);

            pieDocumento.Alignment =
                Element.ALIGN_CENTER;

            doc.Add(pieDocumento);

            Paragraph sistema =
                new Paragraph(
                    "Documento generado por C.E SYSTEM",
                    FontFactory.GetFont(
                        FontFactory.HELVETICA,
                        8,
                        BaseColor.GRAY));

            sistema.Alignment =
                Element.ALIGN_CENTER;

            doc.Add(sistema);

            // =====================================================
            // CERRAR
            // =====================================================

            doc.Close();

            return guardar.FileName;
        }

        private void AgregarCeldaDato(
    PdfPTable tabla,
    string titulo,
    string valor,
    iTextSharp.text.Font fuenteTitulo,
    iTextSharp.text.Font fuenteValor)
        {
            PdfPCell celdaTitulo =
                new PdfPCell(
                    new Phrase(
                        titulo,
                        fuenteTitulo));

            celdaTitulo.Border =
iTextSharp.text.Rectangle.NO_BORDER;
            celdaTitulo.PaddingBottom = 6f;

            tabla.AddCell(celdaTitulo);


            PdfPCell celdaValor =
                new PdfPCell(
                    new Phrase(
                        valor,
                        fuenteValor));

            celdaValor.Border =
iTextSharp.text.Rectangle.NO_BORDER;
            celdaValor.PaddingBottom = 6f;

            tabla.AddCell(celdaValor);
        }

        private string BuscarRutaLogo()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string[] candidatos =
            {
                Path.Combine(baseDir, "logo.png"),
                Path.Combine(baseDir, "Logo.png"),
                Path.Combine(baseDir, "logo.jpg"),
                Path.Combine(baseDir, "logo.jpeg"),
                Path.GetFullPath(Path.Combine(baseDir, @"..\..\logo.png")),
                Path.GetFullPath(Path.Combine(baseDir, @"..\..\Logo.png"))
            };

            foreach (string ruta in candidatos)
                if (File.Exists(ruta))
                    return ruta;

            return null;
        }

        private void AbrirPDF(string ruta)
        {
            try
            {
                if (!File.Exists(ruta))
                {
                    MessageBox.Show(
                        "No se encontró el archivo PDF.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = ruta,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "La factura fue generada, pero no se pudo abrir el PDF:\n" +
                    ex.Message,
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void btnAbrirPdf_Click(object sender, EventArgs e)
        {
            if (dgvCobros.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccione un cobro.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!dgvCobros.Columns.Contains("RutaPDF"))
            {
                MessageBox.Show(
                    "No se encontró la información del PDF.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            object valor =
                dgvCobros.CurrentRow.Cells["RutaPDF"].Value;

            if (valor == null ||
                valor == DBNull.Value ||
                string.IsNullOrWhiteSpace(valor.ToString()))
            {
                MessageBox.Show(
                    "Esta factura no tiene un PDF asociado.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            AbrirPDF(valor.ToString());
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}