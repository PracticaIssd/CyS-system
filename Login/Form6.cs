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
using System.Globalization;
using System.Text.RegularExpressions;

namespace Login
{
    public partial class Form6 : Form
    {
        string conexion =
            @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";
        private string rutaImagen = "";
        private int idPedidoSeleccionado = 0;

        public Form6()
        {
            InitializeComponent();
            ConfigurarInsumos();
           


            cmbFormatoHoja.Items.Clear();

            cmbFormatoHoja.Items.AddRange(new object[]
            {
             "A5",
             "A4",
             "A3"
            });

            cmbFormatoHoja.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFormatoHoja.SelectedIndex = -1;
        }

        public Form6(int idPedido)
        {
            idPedidoSeleccionado = idPedido;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            dgvDetalle.Columns.Clear();

            dgvDetalle.Columns.Add("Trabajo", "Trabajo");
            dgvDetalle.Columns.Add("Medida", "Medida");
            dgvDetalle.Columns.Add("Papel", "Papel");
            dgvDetalle.Columns.Add("Cantidad", "Cantidad");
            dgvDetalle.Columns.Add("Precio", "Precio Unitario");

            dgvDetalle.Columns.Add("RutaImagen", "RutaImagen");
            dgvDetalle.Columns.Add("Observaciones", "Observaciones");


            dgvDetalle.Columns.Add("FormatoHoja", "FormatoHoja");
            dgvDetalle.Columns.Add("TipoImpresion", "TipoImpresion");
            dgvDetalle.Columns.Add("CoberturaTinta", "CoberturaTinta");

            dgvDetalle.Columns["FormatoHoja"].Visible = false;
            dgvDetalle.Columns["TipoImpresion"].Visible = false;
            dgvDetalle.Columns["CoberturaTinta"].Visible = false;

            dgvDetalle.Columns["RutaImagen"].Visible = false;
            dgvDetalle.Columns["Observaciones"].Visible = false;

            dgvDetalle.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDetalle.MultiSelect = false;

            dgvDetalle.CellDoubleClick += dgvDetalle_CellDoubleClick;
            dgvDetalle.SelectionChanged += dgvDetalle_SelectionChanged;

            if (idPedidoSeleccionado > 0)
                CargarPedido();
            else
                GenerarPedido();

            lblDomicilio.Visible = false;
            txtDomicilio.Visible = false;
        }


        private void ConfigurarInsumos()
        {
            cmbTipoImpresion.Items.Clear();
            cmbTipoImpresion.Items.AddRange(new object[]
            {
        "Blanco y negro",
        "Color (CMYK)"
            });

            cmbTipoImpresion.DropDownStyle =
                ComboBoxStyle.DropDownList;
            cmbTipoImpresion.SelectedIndex = -1;

            cmbCoberturaTinta.Items.Clear();
            cmbCoberturaTinta.Items.AddRange(new object[]
            {
        "Baja",
        "Media",
        "Alta"
            });

            cmbCoberturaTinta.DropDownStyle =
                ComboBoxStyle.DropDownList;
            cmbCoberturaTinta.SelectedIndex = -1;

            dgvInsumosCalculados.Columns.Clear();
            dgvInsumosCalculados.AutoGenerateColumns = false;

            dgvInsumosCalculados.Columns.Add(
                "Material", "Material");

            dgvInsumosCalculados.Columns.Add(
                "Cantidad", "Cantidad");

            dgvInsumosCalculados.Columns.Add(
                "Unidad", "Unidad");

            dgvInsumosCalculados.ReadOnly = true;
            dgvInsumosCalculados.AllowUserToAddRows = false;
            dgvInsumosCalculados.RowHeadersVisible = false;
            dgvInsumosCalculados.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvInsumosCalculados.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void dgvDetalle_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            string ruta = dgvDetalle.Rows[e.RowIndex]
                .Cells["RutaImagen"].Value?.ToString();

            if (!string.IsNullOrEmpty(ruta) && System.IO.File.Exists(ruta))
            {
                FormImagen frm = new FormImagen(ruta);
                frm.Show();
            }
            else
            {
                MessageBox.Show("No se encontró la imagen.");
            }
        }
        private void CargarPedido()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();

                string query = @"
        SELECT *
        FROM Pedidos
        WHERE IdPedido = @id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", idPedidoSeleccionado);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    txtPedido.Text = dr["IdPedido"].ToString();
                    txtDni.Text = dr["DniCliente"].ToString();
                    txtNombre.Text = dr["Cliente"].ToString();

                    dtpFechaPedido.Value = Convert.ToDateTime(dr["FechaPedido"]);
                    dtpFechaEntrega.Value = Convert.ToDateTime(dr["FechaEntrega"]);

                    cboPago.Text = dr["FormaPago"].ToString();
                    cboEntrega.Text = dr["PuntoEntrega"].ToString();
                    cboEstado.Text = dr["Estado"].ToString();

                    txtImporte.Text = dr["ImporteTotal"].ToString();
                    txtDomicilio.Text =dr["DomicilioEntrega"].ToString();
                }

                dr.Close();


                string queryDetalle = @"
    SELECT
        Trabajo,
        Medida,
        Papel,
        Cantidad,
        RutaImagen,
        Observaciones,
        FormatoHoja,
        TipoImpresion,
        CoberturaTinta
    FROM DetallePedido
    WHERE IdPedido = @id";


                SqlCommand cmdDetalle =
                    new SqlCommand(queryDetalle, conn);

                cmdDetalle.Parameters.AddWithValue("@id", idPedidoSeleccionado);

                SqlDataReader drDetalle =
                    cmdDetalle.ExecuteReader();

                dgvDetalle.Rows.Clear();

                while (drDetalle.Read())
                {

                    dgvDetalle.Rows.Add(
                        drDetalle["Trabajo"].ToString(),
                        drDetalle["Medida"].ToString(),
                        drDetalle["Papel"].ToString(),
                        drDetalle["Cantidad"].ToString(),
                        ObtenerPrecio(drDetalle["Trabajo"].ToString()),
                        drDetalle["RutaImagen"].ToString(),
                        drDetalle["Observaciones"].ToString(),
                        drDetalle["FormatoHoja"].ToString(),
                        drDetalle["TipoImpresion"].ToString(),
                        drDetalle["CoberturaTinta"].ToString()
                    );

                }
                if (dgvDetalle.Rows.Count > 0)
                {
                    string ruta = dgvDetalle.Rows[0].Cells["RutaImagen"].Value?.ToString();

                    if (!string.IsNullOrEmpty(ruta) && System.IO.File.Exists(ruta))
                    {
                        picReferencia.Image = Image.FromFile(ruta);
                        picReferencia.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }
                if (dgvDetalle.Rows.Count > 0)
                {
                    dgvDetalle.Rows[0].Selected = true;
                    txtObservaciones.Text =
                        dgvDetalle.Rows[0].Cells["Observaciones"].Value?.ToString();
                }

                drDetalle.Close();
            }
        }

        private decimal ConvertirHojasAResmas(int idMaterial, int hojas)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();

                string sql = @"
            SELECT UnidadesPorPresentacion
            FROM dbo.Materiales
            WHERE IdMaterial = @IdMaterial
              AND UnidadMedida = 'Resma'";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdMaterial", idMaterial);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado == null ||
                        resultado == DBNull.Value ||
                        Convert.ToInt32(resultado) <= 0)
                    {
                        throw new Exception(
                            "El material no tiene configurada " +
                            "la cantidad de hojas por resma.");
                    }

                    int hojasPorResma = Convert.ToInt32(resultado);

                    return (decimal)hojas / hojasPorResma;
                }
            }
        }


        private int BuscarIdMaterial(string nombreMaterial)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();

                string sql = @"
            SELECT IdMaterial
            FROM dbo.Materiales
            WHERE Nombre = @Nombre
              AND Estado = 'Activo'";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@Nombre", nombreMaterial);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado == null)
                        return -1;

                    return Convert.ToInt32(resultado);
                }
            }
        }

        private bool ObtenerMedidas(string texto, out decimal ancho, out decimal alto)
        {
            ancho = 0;
            alto = 0;

            string medida = texto.Trim().ToUpperInvariant();

            // Formatos estándar, en centímetros
            switch (medida)
            {
                case "A6":
                    ancho = 10.5m;
                    alto = 14.8m;
                    return true;

                case "A5":
                    ancho = 14.8m;
                    alto = 21m;
                    return true;

                case "A4":
                    ancho = 21m;
                    alto = 29.7m;
                    return true;

                case "A3":
                    ancho = 29.7m;
                    alto = 42m;
                    return true;
            }

            // Medidas personalizadas: 9x5, 15 x 20, 9,5x14 cm
            medida = medida.Replace("CM", "").Trim();

            string[] partes = Regex.Split(medida, @"\s*[xX×]\s*");

            if (partes.Length != 2)
                return false;

            string valorAncho = partes[0].Trim().Replace(",", ".");
            string valorAlto = partes[1].Trim().Replace(",", ".");

            return decimal.TryParse(
                       valorAncho,
                       NumberStyles.Number,
                       CultureInfo.InvariantCulture,
                       out ancho)
                   &&
                   decimal.TryParse(
                       valorAlto,
                       NumberStyles.Number,
                       CultureInfo.InvariantCulture,
                       out alto)
                   &&
                   ancho > 0 &&
                   alto > 0;
        }

        private int CalcularHojasNecesarias(
            decimal anchoProducto,
            decimal altoProducto,
            decimal anchoHoja,
            decimal altoHoja,
            int cantidad,
            decimal porcentajeMerma)
        {
            // Calcular cuántos productos entran sin girarlos
            int opcion1 =
                (int)Math.Floor(anchoHoja / anchoProducto) *
                (int)Math.Floor(altoHoja / altoProducto);

            // Calcular cuántos entran girados
            int opcion2 =
                (int)Math.Floor(anchoHoja / altoProducto) *
                (int)Math.Floor(altoHoja / anchoProducto);

            int productosPorHoja = Math.Max(opcion1, opcion2);

            if (productosPorHoja <= 0)
                throw new Exception(
                    "El producto no entra en el formato de hoja seleccionado.");

            decimal hojasBase =
                Math.Ceiling((decimal)cantidad / productosPorHoja);

            decimal hojasConMerma =
                Math.Ceiling(hojasBase * (1 + porcentajeMerma / 100));

            return checked((int)hojasConMerma);
        }

        private int ObtenerHojasCalculadas()
        {
            if (!ObtenerMedidas(
                txtMedida.Text,
                out decimal anchoProducto,
                out decimal altoProducto))
            {
                throw new Exception(
                    "Ingrese una medida válida. Ejemplos: A5, A4 o 9x5.");
            }

            if (cmbFormatoHoja.SelectedIndex == -1)
            {
                throw new Exception(
                    "Seleccione el formato de hoja que utilizará.");
            }

            if (nudCantidad.Value <= 0)
            {
                throw new Exception(
                    "Ingrese una cantidad mayor a cero.");
            }

            if (nudCantidad.Value != Math.Truncate(nudCantidad.Value))
            {
                throw new Exception(
                    "La cantidad de productos debe ser un número entero.");
            }

            if (nudCantidad.Value > int.MaxValue)
            {
                throw new Exception(
                    "La cantidad ingresada es demasiado grande.");
            }

            decimal anchoHoja;
            decimal altoHoja;

            switch (cmbFormatoHoja.Text)
            {
                case "A5":
                    anchoHoja = 14.8m;
                    altoHoja = 21m;
                    break;

                case "A4":
                    anchoHoja = 21m;
                    altoHoja = 29.7m;
                    break;

                case "A3":
                    anchoHoja = 29.7m;
                    altoHoja = 42m;
                    break;

                default:
                    throw new Exception(
                        "El formato de hoja seleccionado no es válido.");
            }

            // Merma inicial estimada del 5 %
            decimal porcentajeMerma = 5m;

            return CalcularHojasNecesarias(
                anchoProducto,
                altoProducto,
                anchoHoja,
                altoHoja,
                (int)nudCantidad.Value,
                porcentajeMerma);
        }

        private void GenerarPedido()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();

                string query = "SELECT ISNULL(MAX(IdPedido),0)+1 FROM Pedidos";

                SqlCommand cmd = new SqlCommand(query, conn);

                txtPedido.Text = cmd.ExecuteScalar().ToString();
            }
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query =
                        "SELECT Nombre FROM Clientes WHERE DNI=@dni";

                    SqlCommand cmd =
                        new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@dni", txtDni.Text);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        txtNombre.Text = resultado.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Cliente no encontrado");
                        txtNombre.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cboTrabajo.SelectedIndex == -1 ||
            string.IsNullOrWhiteSpace(txtMedida.Text) ||
            cboPapel.SelectedIndex == -1 ||
            nudCantidad.Value <= 0)
            {
                MessageBox.Show(
                    "Completar todos los datos del trabajo.",
                    "Campos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            decimal precio = ObtenerPrecio(cboTrabajo.Text);
           

            dgvDetalle.Rows.Add(
             cboTrabajo.Text,
             txtMedida.Text,
             cboPapel.Text,
             nudCantidad.Value,
             precio,
             rutaImagen,
             txtObservaciones.Text
            );
            CalcularTotal();
        }
        private decimal ObtenerPrecio(string trabajo)
        {
            switch (trabajo)
            {
                case "Folletos":
                    return 10;

                case "Tarjetas":
                    return 15;

                case "Volantes":
                    return 12;

                case "Stickers":
                    return 20;

                case "Banners":
                    return 50;

                case "Talonarios":
                    return 30;

                case "Afiches":
                    return 25;

                case "Entradas":
                    return 15;

                case "Bolsas":
                    return 25;

                case "Cajas":
                    return 30;

                default:
                    return 0;
            }
        }
        private void CalcularTotal()
        {
            decimal total = 0;

            foreach (DataGridViewRow fila in dgvDetalle.Rows)
            {
                if (fila.IsNewRow)
                    continue;

                decimal cantidad =
                    Convert.ToDecimal(fila.Cells["Cantidad"].Value);

                decimal precio =
                    Convert.ToDecimal(fila.Cells["Precio"].Value);

                total += cantidad * precio;
            }

            txtImporte.Text = total.ToString("N2");
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (idPedidoSeleccionado == 0)
            {
                if (string.IsNullOrWhiteSpace(txtDni.Text) ||
                    string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    cboPago.SelectedIndex == -1 ||
                    cboEntrega.SelectedIndex == -1 ||
                    cboEstado.SelectedIndex == -1)
                {
                    MessageBox.Show(
                        "Completar todos los datos del pedido.",
                        "Campos incompletos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (cboEntrega.Text == "Domicilio" &&
                    string.IsNullOrWhiteSpace(txtDomicilio.Text))
                {
                    MessageBox.Show(
                        "Debe ingresar un domicilio de entrega.",
                        "Campos incompletos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (dgvDetalle.Rows.Count <= 1)
                {
                    MessageBox.Show(
                        "Debe agregar al menos un trabajo al pedido.",
                        "Pedido incompleto",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }
            }

            if (dgvDetalle.Rows.Count <= 1)
            {
                MessageBox.Show(
                    "Debe agregar al menos un trabajo al pedido.",
                    "Pedido incompleto",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    decimal importe = 0;
                    decimal.TryParse(txtImporte.Text, out importe);

                    int idPedido;
                    string domicilio;

                    if (cboEntrega.Text == "Sucursal")
                    {
                        domicilio = "Retira en sucursal";
                    }
                    else
                    {
                        domicilio = txtDomicilio.Text;
                    }

                    if (idPedidoSeleccionado > 0)
                    {
                        string queryUpdate = @"
                UPDATE Pedidos
                SET
                    DniCliente = @dni,
                    Cliente = @cliente,
                    FechaPedido = @fechaPedido,
                    FechaEntrega = @fechaEntrega,
                    FormaPago = @pago,
                    PuntoEntrega = @entrega,
                    DomicilioEntrega = @domicilio,
                    Estado = @estado,
                    ImporteTotal = @importe
                WHERE IdPedido = @id";

                        SqlCommand cmdPedido =
                            new SqlCommand(queryUpdate, conn, trans);

                        cmdPedido.Parameters.AddWithValue("@id", idPedidoSeleccionado);
                        cmdPedido.Parameters.AddWithValue("@dni", txtDni.Text);
                        cmdPedido.Parameters.AddWithValue("@cliente", txtNombre.Text);
                        cmdPedido.Parameters.AddWithValue("@fechaPedido", dtpFechaPedido.Value);
                        cmdPedido.Parameters.AddWithValue("@fechaEntrega", dtpFechaEntrega.Value);
                        cmdPedido.Parameters.AddWithValue("@pago", cboPago.Text);
                        cmdPedido.Parameters.AddWithValue("@entrega", cboEntrega.Text);
                        cmdPedido.Parameters.AddWithValue("@domicilio", txtDomicilio.Text);
                        cmdPedido.Parameters.AddWithValue("@estado", cboEstado.Text);
                        cmdPedido.Parameters.AddWithValue("@importe", importe);

                        cmdPedido.ExecuteNonQuery();

                        idPedido = idPedidoSeleccionado;

                        string deleteDetalle =
                            "DELETE FROM DetallePedido WHERE IdPedido = @id";

                        SqlCommand cmdDelete =
                            new SqlCommand(deleteDetalle, conn, trans);

                        cmdDelete.Parameters.AddWithValue("@id", idPedido);

                        cmdDelete.ExecuteNonQuery();
                    }
                    else
                    {
                       
                        string queryPedido =
                        @"INSERT INTO Pedidos
                (
                    DniCliente,
                    Cliente,
                    FechaPedido,
                    FechaEntrega,
                    FormaPago,
                    PuntoEntrega,
                    DomicilioEntrega,
                    Estado,
                    ImporteTotal
                )
                OUTPUT INSERTED.IdPedido
                VALUES
                (
                    @dni,
                    @cliente,
                    @fechaPedido,
                    @fechaEntrega,
                    @pago,
                    @entrega,
                    @domicilio,
                    @estado,
                    @importe
                )";

                        SqlCommand cmdPedido =
                            new SqlCommand(queryPedido, conn, trans);

                        cmdPedido.Parameters.AddWithValue("@dni", txtDni.Text);
                        cmdPedido.Parameters.AddWithValue("@cliente", txtNombre.Text);
                        cmdPedido.Parameters.AddWithValue("@fechaPedido", dtpFechaPedido.Value);
                        cmdPedido.Parameters.AddWithValue("@fechaEntrega", dtpFechaEntrega.Value);
                        cmdPedido.Parameters.AddWithValue("@pago", cboPago.Text);
                        cmdPedido.Parameters.AddWithValue("@entrega", cboEntrega.Text);
                        cmdPedido.Parameters.AddWithValue("@domicilio", domicilio);
                        cmdPedido.Parameters.AddWithValue("@estado", cboEstado.Text);
                        cmdPedido.Parameters.AddWithValue("@importe", importe);

                        idPedido =
                            Convert.ToInt32(cmdPedido.ExecuteScalar());
                    }

                    
                    foreach (DataGridViewRow fila in dgvDetalle.Rows)
                    {
                        if (fila.IsNewRow)
                            continue;


                        string queryDetalle = @"
    INSERT INTO DetallePedido
    (
        IdPedido,
        Trabajo,
        Medida,
        Papel,
        Cantidad,
        RutaImagen,
        Observaciones,
        FormatoHoja,
        TipoImpresion,
        CoberturaTinta
    )
    VALUES
    (
        @IdPedido,
        @trabajo,
        @medida,
        @papel,
        @cantidad,
        @ruta,
        @obs,
        @formato,
        @tipoImpresion,
        @cobertura
    )";


                        SqlCommand cmdDetalle =
                            new SqlCommand(queryDetalle, conn, trans);

                        cmdDetalle.Parameters.AddWithValue("@IdPedido", idPedido);
                        cmdDetalle.Parameters.AddWithValue("@trabajo", fila.Cells["Trabajo"].Value);
                        cmdDetalle.Parameters.AddWithValue("@medida", fila.Cells["Medida"].Value);
                        cmdDetalle.Parameters.AddWithValue("@papel", fila.Cells["Papel"].Value);
                        cmdDetalle.Parameters.AddWithValue("@cantidad", fila.Cells["Cantidad"].Value);
                        cmdDetalle.Parameters.AddWithValue("@ruta", fila.Cells["RutaImagen"].Value);
                        cmdDetalle.Parameters.AddWithValue("@obs", fila.Cells["Observaciones"].Value);

                        cmdDetalle.ExecuteNonQuery();
                    }

                    trans.Commit();

                    MessageBox.Show(
                        idPedidoSeleccionado > 0
                        ? "Pedido actualizado correctamente"
                        : "Pedido guardado correctamente");

                    if (idPedidoSeleccionado > 0)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    


                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtDni.Clear();
            txtNombre.Clear();

            cboPago.SelectedIndex = -1;
            cboEntrega.SelectedIndex = -1;
            cboEstado.SelectedIndex = -1;

            cboTrabajo.SelectedIndex = -1;
            cboTrabajo.Text = "";

            txtMedida.Clear();

            cboPapel.SelectedIndex = -1;
            cboPapel.Text = "";

            nudCantidad.Value = 0;

            txtObservaciones.Clear();
            txtDomicilio.Clear();

            txtImporte.Clear();

            rutaImagen = "";
            picReferencia.Image = null;

            dgvDetalle.Rows.Clear();
            dgvDetalle.ClearSelection();

            lblDomicilio.Visible = false;
            txtDomicilio.Visible = false;

            GenerarPedido();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Form5 frm = new Form5();
            frm.Show();
            this.Hide();
        }

        private void cboEntrega_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboEntrega.Text == "Domicilio")
            {
                lblDomicilio.Visible = true;
                txtDomicilio.Visible = true;

                lblDomicilio.BringToFront();
                txtDomicilio.BringToFront();
            }
            else
            {
                lblDomicilio.Visible = false;
                txtDomicilio.Visible = false;
                txtDomicilio.Clear();
            }
        }

        private void btnCargarImagen_Click(object sender, EventArgs e)
        {
            OpenFileDialog abrir = new OpenFileDialog();

            abrir.Filter =
                "Imágenes|*.jpg;*.jpeg;*.png;*.bmp";

            if (abrir.ShowDialog() == DialogResult.OK)
            {
                rutaImagen = abrir.FileName;

                picReferencia.Image =
                    Image.FromFile(rutaImagen);

                picReferencia.SizeMode =
                    PictureBoxSizeMode.Zoom;

                if (dgvDetalle.CurrentRow != null)
                {
                    dgvDetalle.CurrentRow.Cells["RutaImagen"].Value = rutaImagen;
                }
            }

        }
        private void dgvDetalle_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDetalle.CurrentRow == null ||
        dgvDetalle.CurrentRow.IsNewRow)
                return;

            cboTrabajo.Text =
                dgvDetalle.CurrentRow.Cells["Trabajo"].Value?.ToString();

            txtMedida.Text =
                dgvDetalle.CurrentRow.Cells["Medida"].Value?.ToString();

            cboPapel.Text =
                dgvDetalle.CurrentRow.Cells["Papel"].Value?.ToString();


            cmbFormatoHoja.Text =
                dgvDetalle.CurrentRow.Cells["FormatoHoja"].Value?.ToString() ?? "";

            cmbTipoImpresion.Text =
                dgvDetalle.CurrentRow.Cells["TipoImpresion"].Value?.ToString() ?? "";

            cmbCoberturaTinta.Text =
                dgvDetalle.CurrentRow.Cells["CoberturaTinta"].Value?.ToString() ?? "";


            nudCantidad.Value =
                Convert.ToDecimal(
                    dgvDetalle.CurrentRow.Cells["Cantidad"].Value);

            txtObservaciones.Text =
                dgvDetalle.CurrentRow.Cells["Observaciones"].Value?.ToString();

            string ruta =
                dgvDetalle.CurrentRow.Cells["RutaImagen"].Value?.ToString();

            rutaImagen = ruta;
                dgvDetalle.CurrentRow.Cells["RutaImagen"].Value?.ToString();

            if (!string.IsNullOrWhiteSpace(ruta) &&
                System.IO.File.Exists(ruta))
            {
                picReferencia.Image = Image.FromFile(ruta);
                picReferencia.SizeMode = PictureBoxSizeMode.Zoom;
            }

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvDetalle.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un trabajo.");
                return;
            }

            dgvDetalle.CurrentRow.Cells["Trabajo"].Value =
                cboTrabajo.Text;

            dgvDetalle.CurrentRow.Cells["Medida"].Value =
                txtMedida.Text;

            dgvDetalle.CurrentRow.Cells["Papel"].Value =
                cboPapel.Text;

            dgvDetalle.CurrentRow.Cells["Cantidad"].Value =
                nudCantidad.Value;

            dgvDetalle.CurrentRow.Cells["Precio"].Value =
                ObtenerPrecio(cboTrabajo.Text);

            dgvDetalle.CurrentRow.Cells["Observaciones"].Value =
                txtObservaciones.Text;

            if (picReferencia.Image != null &&
    !       string.IsNullOrWhiteSpace(rutaImagen))
            {
                dgvDetalle.CurrentRow.Cells["RutaImagen"].Value =
                    rutaImagen;
            }


            dgvDetalle.CurrentRow.Cells["FormatoHoja"].Value =
                cmbFormatoHoja.Text;

            dgvDetalle.CurrentRow.Cells["TipoImpresion"].Value =
                cmbTipoImpresion.Text;

            dgvDetalle.CurrentRow.Cells["CoberturaTinta"].Value =
                cmbCoberturaTinta.Text;


            CalcularTotal();

            MessageBox.Show("Trabajo modificado.");
        }

        private void btnEliminarTrabajo_Click(object sender, EventArgs e)
        {
            if (dgvDetalle.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un trabajo.");
                return;
            }

            DialogResult r = MessageBox.Show(
                "¿Desea eliminar el trabajo seleccionado?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                dgvDetalle.Rows.Remove(dgvDetalle.CurrentRow);

                CalcularTotal();

                if (dgvDetalle.Rows.Count > 0)
                {
                    dgvDetalle.ClearSelection();
                    dgvDetalle.Rows[0].Selected = true;

                    dgvDetalle_SelectionChanged(null, null);
                }
                else
                {
                    txtMedida.Clear();
                    txtObservaciones.Clear();

                    cboTrabajo.SelectedIndex = -1;
                    cboPapel.SelectedIndex = -1;

                    nudCantidad.Value = 0;

                    picReferencia.Image = null;

                    rutaImagen = "";
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblDomicilio_Click(object sender, EventArgs e)
        {

        }

        private void cboTrabajo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }




        private void btnCalcularMateriales_Click(object sender, EventArgs e)
        {
            dgvInsumosCalculados.Rows.Clear();

            try
            {
                if (cboTrabajo.SelectedIndex == -1 ||
                    cboPapel.SelectedIndex == -1 ||
                    cmbFormatoHoja.SelectedIndex == -1 ||
                    cmbTipoImpresion.SelectedIndex == -1 ||
                    cmbCoberturaTinta.SelectedIndex == -1)
                {
                    MessageBox.Show(
                        "Complete los datos del trabajo y la impresión.",
                        "Datos incompletos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string trabajo = cboTrabajo.Text.Trim();

                string[] trabajosCompatibles =
                {
            "Folletos",
            "Tarjetas",
            "Volantes",
            "Stickers",
            "Afiches",
            "Entradas"
        };

                if (!trabajosCompatibles.Contains(trabajo))
                {
                    MessageBox.Show(
                        "Este trabajo todavía no tiene configurado " +
                        "el cálculo automático de materiales.",
                        "Cálculo no disponible",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                if (!ObtenerMedidas(
                    txtMedida.Text,
                    out decimal ancho,
                    out decimal alto))
                {
                    MessageBox.Show(
                        "Ingrese una medida válida. Ejemplo: A5 o 10x10.",
                        "Medida incorrecta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                int hojas = ObtenerHojasCalculadas();
                int cantidad = (int)nudCantidad.Value;

                string nombreMaterial =
                "Papel " + cboPapel.Text.Trim().ToLower() +
                 " " + cmbFormatoHoja.Text.Trim().ToUpper();

                int idMaterial = BuscarIdMaterial(nombreMaterial);

                if (idMaterial == -1)
                {
                    throw new Exception(
                        "El material " + nombreMaterial +
                        " no está registrado en el inventario.");
                }

                decimal resmasNecesarias =
                    ConvertirHojasAResmas(idMaterial, hojas);

                VerificarDisponibilidadPapel(
                    idMaterial,
                    resmasNecesarias);

                decimal cobertura = ObtenerPorcentajeCobertura();

                decimal tintaTotal = CalcularTintaMl(
                    ancho,
                    alto,
                    cantidad,
                    cobertura
                );

                // Registrar papel calculado
                dgvInsumosCalculados.Rows.Add(
                    cboPapel.Text + " " + cmbFormatoHoja.Text,
                    hojas,
                    "Hojas"
                );

                if (cmbTipoImpresion.Text == "Blanco y negro")
                {
                    dgvInsumosCalculados.Rows.Add(
                        "Tinta negra",
                        tintaTotal.ToString("0.0000"),
                        "ml"
                    );
                }
                else
                {
                    // Distribución temporal para demostración.
                    // Las coberturas reales de CMYK son independientes.
                    decimal tintaPorColor = tintaTotal / 4m;

                    string[] colores =
                    {
                "Tinta cian",
                "Tinta magenta",
                "Tinta amarilla",
                "Tinta negra"
            };

                    foreach (string color in colores)
                    {
                        dgvInsumosCalculados.Rows.Add(
                            color,
                            tintaPorColor.ToString("0.0000"),
                            "ml"
                        );
                    }
                }

                dgvInsumosCalculados.ClearSelection();
            }
            catch (Exception ex)
            {
                dgvInsumosCalculados.Rows.Clear();

                MessageBox.Show(
                    "No se pudieron calcular los insumos: " +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private decimal ConsultarStockDisponible(int idMaterial)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();

                string sql = @"
            SELECT StockActual - StockReservado
            FROM dbo.Materiales
            WHERE IdMaterial = @IdMaterial
              AND Estado = 'Activo'";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdMaterial", idMaterial);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado == null || resultado == DBNull.Value)
                    {
                        throw new Exception(
                            "No se encontró el material en el inventario.");
                    }

                    return Convert.ToDecimal(resultado);
                }
            }
        }

        private void VerificarDisponibilidadPapel(
            int idMaterial,
            decimal cantidadNecesaria)
        {
            decimal disponible = ConsultarStockDisponible(idMaterial);

            if (disponible >= cantidadNecesaria)
            {
                MessageBox.Show(
                    "Stock suficiente para fabricar el trabajo.\n\n" +
                    "Necesario: " + cantidadNecesaria.ToString("0.####") +
                    " resmas\n" +
                    "Disponible: " + disponible.ToString("0.####") +
                    " resmas",
                    "Disponibilidad de materiales",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                decimal faltante = cantidadNecesaria - disponible;

                MessageBox.Show(
                    "No hay suficiente papel para fabricar el trabajo.\n\n" +
                    "Necesario: " + cantidadNecesaria.ToString("0.####") +
                    " resmas\n" +
                    "Disponible: " + disponible.ToString("0.####") +
                    " resmas\n" +
                    "Faltante: " + faltante.ToString("0.####") +
                    " resmas",
                    "Stock insuficiente",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }





        private decimal ObtenerPorcentajeCobertura()
        {
            switch (cmbCoberturaTinta.Text)
            {
                case "Baja":
                    return 0.10m;

                case "Media":
                    return 0.30m;

                case "Alta":
                    return 0.60m;

                default:
                    throw new Exception(
                        "Seleccione la cobertura de tinta.");
            }
        }


        private decimal CalcularTintaMl(
            decimal anchoCm,
            decimal altoCm,
            int cantidad,
            decimal cobertura)
        {
            // Superficie total del producto en metros cuadrados
            decimal superficieM2 =
                (anchoCm / 100m) *
                (altoCm / 100m) *
                cantidad;

            // Coeficiente ilustrativo:
            // ml por m² con cobertura completa
            decimal consumoPorM2 = 1m;

            decimal tintaMl =
                superficieM2 * cobertura * consumoPorM2;

            return tintaMl;
        }

    }
}

