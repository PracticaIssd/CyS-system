using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Data.SqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Login
{
    public partial class Form14 : Form
    {
        private string conexion =
        @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";

        private int? idProveedorEditando = null;
        private int? idProveedorSeleccionado = null;

        public Form14()
        {
            InitializeComponent();
            tabControlProveedores.SelectedIndexChanged += 
            tabControlProveedores_SelectedIndexChanged;

            // BOTONES DE REGISTRO
            RedondearControl(btnGuardarProveedor, 10);
            RedondearControl(btnLimpiarProveedor, 10);

            // TEXTBOX DE REGISTRO
            RedondearControl(txtRazonSocial, 8);
            RedondearControl(txtCUIT, 8);
            RedondearControl(txtTelefonoProveedor, 8);
            RedondearControl(txtEmailProveedor, 8);
            RedondearControl(txtDireccionProveedor, 8);
            RedondearControl(txtLocalidadProveedor, 8);

            // BOTONES DE CONSULTA Y ABM
            RedondearControl(btnBuscarProveedor, 10);
            RedondearControl(btnMostrarTodosProveedores, 10);
            RedondearControl(btnEditarProveedor, 10);
            RedondearControl(btnDarBajaProveedor, 10);
            RedondearControl(btnReactivarProveedor, 10);

            // DATAGRIDVIEW
            ConfigurarDgvProveedores();
            RedondearControl(dgvProveedores, 15);

            dgvProveedores.SizeChanged += (s, e) =>
            {
                if (dgvProveedores.Width > 30 && dgvProveedores.Height > 30)
                {
                    RedondearControl(dgvProveedores, 15);
                }
            };

            // EVENTOS DE LA GRILLA
            dgvProveedores.SelectionChanged += dgvProveedores_SelectionChanged;

            // CARGAR PROVEEDORES
            CargarProveedores();

            // CARGAR COMBOBOX DE PRODUCTOS OFRECIDOS
            CargarProveedoresCatalogo();
            CargarMaterialesCatalogo();

            CargarMaterialesProveedor();

            ConfigurarDgvCatalogoProveedor();

            CargarProveedoresCompra();
            CargarMaterialesCompra();
            CargarCompras();
        }
        private void tabControlProveedores_SelectedIndexChanged(
     object sender, EventArgs e)
        {
            if (tabControlProveedores.SelectedTab == tabProductosProveedor)
            {
                CargarProveedoresCatalogo();
                CargarMaterialesCatalogo();
                txtBuscarCatalogo.Clear();
                CargarMaterialesProveedor();
            }
        }
        private void ConfigurarDgvProveedores()
        {
            dgvProveedores.BackgroundColor = Color.White;
            dgvProveedores.BorderStyle = BorderStyle.None;

            dgvProveedores.ReadOnly = true;
            dgvProveedores.AllowUserToAddRows = false;
            dgvProveedores.AllowUserToDeleteRows = false;

            // Impedir que cambien el tamaño de las columnas y filas
            dgvProveedores.AllowUserToResizeColumns = false;
            dgvProveedores.AllowUserToResizeRows = false;

            dgvProveedores.MultiSelect = false;
            dgvProveedores.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            // Evitar que las columnas se compriman
            dgvProveedores.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.None;

            dgvProveedores.AutoGenerateColumns = true;
            dgvProveedores.ScrollBars = ScrollBars.Both;
            dgvProveedores.RowHeadersVisible = false;

            // Encabezados
            dgvProveedores.EnableHeadersVisualStyles = false;
            dgvProveedores.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvProveedores.ColumnHeadersHeight = 45;

            dgvProveedores.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(8, 43, 82);
            dgvProveedores.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;
            dgvProveedores.ColumnHeadersDefaultCellStyle.Font =
                new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvProveedores.ColumnHeadersDefaultCellStyle.WrapMode =
                DataGridViewTriState.False;

            // Filas
            dgvProveedores.RowTemplate.Height = 38;
            dgvProveedores.DefaultCellStyle.Font =
              new System.Drawing.Font("Segoe UI", 10);
            dgvProveedores.DefaultCellStyle.ForeColor = Color.Black;
            dgvProveedores.DefaultCellStyle.Padding =
                new Padding(5, 0, 5, 0);

            // Selección
            dgvProveedores.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(36, 110, 170);
            dgvProveedores.DefaultCellStyle.SelectionForeColor =
                Color.White;

            dgvProveedores.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(240, 245, 250);

            dgvProveedores.GridColor =
                Color.FromArgb(225, 230, 235);
            dgvProveedores.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;
        }

        private void CargarProveedores(string filtro = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string consulta = @"
                SELECT
                    IdProveedor,
                    RazonSocial AS [Razón social],
                    CUIT,
                    Telefono AS [Teléfono],
                    Email AS [Correo electrónico],
                    Direccion AS [Dirección],
                    Localidad,
                    Estado
                FROM Proveedores
                WHERE RazonSocial LIKE @Filtro
                   OR CUIT LIKE @Filtro
                ORDER BY RazonSocial";

                    using (SqlCommand cmd = new SqlCommand(consulta, con))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Filtro", "%" + filtro + "%"
                        );

                        SqlDataAdapter adaptador =
                            new SqlDataAdapter(cmd);

                        DataTable tabla = new DataTable();
                        adaptador.Fill(tabla);


                        dgvProveedores.DataSource = tabla;

                        // Ocultar el identificador del proveedor
                        if (dgvProveedores.Columns.Contains("IdProveedor"))
                            dgvProveedores.Columns["IdProveedor"].Visible = false;

                        // Establecer el ancho de cada columna
                        dgvProveedores.Columns["Razón social"].Width = 190;
                        dgvProveedores.Columns["CUIT"].Width = 130;
                        dgvProveedores.Columns["Teléfono"].Width = 140;
                        dgvProveedores.Columns["Correo electrónico"].Width = 220;
                        dgvProveedores.Columns["Dirección"].Width = 180;
                        dgvProveedores.Columns["Localidad"].Width = 140;
                        dgvProveedores.Columns["Estado"].Width = 100;

                        // Bloquear el cambio de tamaño
                        foreach (DataGridViewColumn columna in dgvProveedores.Columns)
                        {
                            columna.Resizable = DataGridViewTriState.False;
                        }

                        // Limpiar selección
                        dgvProveedores.ClearSelection();
                        dgvProveedores.CurrentCell = null;
                        idProveedorSeleccionado = null;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al consultar proveedores: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        private void CargarProveedoresCatalogo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string consulta = @"
                SELECT IdProveedor, RazonSocial
                FROM Proveedores
                WHERE Estado = 'Activo'
                ORDER BY RazonSocial";

                    SqlDataAdapter adaptador = new SqlDataAdapter(consulta, con);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    DataRow filaTodos = tabla.NewRow();
                    filaTodos["IdProveedor"] = 0;
                    filaTodos["RazonSocial"] = "Todos los proveedores";
                    tabla.Rows.InsertAt(filaTodos, 0);

                    cmbProveedorCatalogo.DisplayMember = "RazonSocial";
                    cmbProveedorCatalogo.ValueMember = "IdProveedor";
                    cmbProveedorCatalogo.DataSource = tabla;
                    cmbProveedorCatalogo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores: " + ex.Message);
            }
        }


        private void CargarMaterialesCatalogo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string consulta = @"
                SELECT IdMaterial, Nombre
                FROM Materiales
                WHERE Estado = 'Activo'
                ORDER BY Nombre";

                    SqlDataAdapter adaptador = new SqlDataAdapter(consulta, con);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    DataRow filaTodos = tabla.NewRow();
                    filaTodos["IdMaterial"] = 0;
                    filaTodos["Nombre"] = "Todos los materiales";
                    tabla.Rows.InsertAt(filaTodos, 0);

                    cmbMaterialCatalogo.DisplayMember = "Nombre";
                    cmbMaterialCatalogo.ValueMember = "IdMaterial";
                    cmbMaterialCatalogo.DataSource = tabla;
                    cmbMaterialCatalogo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar materiales: " + ex.Message);
            }
        }


        private void ConfigurarDgvCatalogoProveedor()
        {
            dgvCatalogoProveedor.BackgroundColor = Color.White;
            dgvCatalogoProveedor.BorderStyle = BorderStyle.None;

            dgvCatalogoProveedor.ReadOnly = true;
            dgvCatalogoProveedor.AllowUserToAddRows = false;
            dgvCatalogoProveedor.AllowUserToDeleteRows = false;
            dgvCatalogoProveedor.AllowUserToResizeRows = false;
            dgvCatalogoProveedor.AllowUserToResizeColumns = false;

            dgvCatalogoProveedor.MultiSelect = false;
            dgvCatalogoProveedor.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvCatalogoProveedor.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvCatalogoProveedor.RowHeadersVisible = false;
            dgvCatalogoProveedor.EnableHeadersVisualStyles = false;

            dgvCatalogoProveedor.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvCatalogoProveedor.ColumnHeadersHeight = 40;

            dgvCatalogoProveedor.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(8, 43, 82);

            dgvCatalogoProveedor.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvProveedores.ColumnHeadersDefaultCellStyle.Font =
      new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);

            dgvCatalogoProveedor.RowTemplate.Height = 35;

            dgvCatalogoProveedor.DefaultCellStyle.Font =
              new System.Drawing.Font("Segoe UI", 10);

            dgvCatalogoProveedor.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(36, 110, 170);

            dgvCatalogoProveedor.DefaultCellStyle.SelectionForeColor =
                Color.White;

            dgvCatalogoProveedor.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(240, 245, 250);
        }


        private void dgvProveedores_SelectionChanged(object sender, EventArgs e)
        {
            idProveedorSeleccionado = null;

            if (dgvProveedores.SelectedRows.Count == 0)
                return;

            DataGridViewRow fila = dgvProveedores.SelectedRows[0];

            if (fila.Cells["IdProveedor"].Value != null)
            {
                idProveedorSeleccionado =
                    Convert.ToInt32(fila.Cells["IdProveedor"].Value);
            }
        }
        private int? ObtenerIdProveedorSeleccionado()
        {
            if (!idProveedorSeleccionado.HasValue ||
                dgvProveedores.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Primero debe seleccionar un proveedor de la lista.",
                    "Atención",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return null;
            }

            return idProveedorSeleccionado.Value;
        }
        private void RedondearControl(Control control, int radio)
        {
            GraphicsPath path = new GraphicsPath();

            int diametro = radio * 2;

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(
            0, 0,
            control.Width,
            control.Height
            );
            

            path.AddArc(rect.X, rect.Y, diametro, diametro, 180, 90);
            path.AddArc(rect.Right - diametro, rect.Y, diametro, diametro, 270, 90);
            path.AddArc(rect.Right - diametro, rect.Bottom - diametro, diametro, diametro, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diametro, diametro, diametro, 90, 90);

            path.CloseFigure();

            control.Region = new Region(path);
            path.Dispose();
        }

        private void tabRegistrarProveedor_Click(object sender, EventArgs e)
        {

        }

        private void btnGuardarProveedor_Click(object sender, EventArgs e)
        {
            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(txtRazonSocial.Text) ||
                string.IsNullOrWhiteSpace(txtCUIT.Text))
            {
                MessageBox.Show(
                    "Debe completar la razón social y el CUIT.",
                    "Campos obligatorios",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string razonSocial = txtRazonSocial.Text.Trim();
            string cuit = txtCUIT.Text.Trim();
            string telefono = txtTelefonoProveedor.Text.Trim();
            string email = txtEmailProveedor.Text.Trim();
            string direccion = txtDireccionProveedor.Text.Trim();
            string localidad = txtLocalidadProveedor.Text.Trim();

            // Validar formato del CUIT
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                cuit, @"^\d{2}-?\d{8}-?\d$"))
            {
                MessageBox.Show(
                    "El CUIT debe tener 11 números. Ejemplo: 30-12345678-9.",
                    "CUIT inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Normalizar CUIT para evitar duplicados con distintos formatos
            cuit = cuit.Replace("-", "");

            // Validar correo electrónico si fue ingresado
            if (!string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    var correo = new System.Net.Mail.MailAddress(email);

                    if (correo.Address != email)
                        throw new FormatException();
                }
                catch (FormatException)
                {
                    MessageBox.Show(
                        "Ingrese un correo electrónico válido.",
                        "Correo inválido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }

            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();


                    string consulta;

                    if (idProveedorEditando.HasValue)
                    {
                        // MODIFICAR proveedor existente
                        consulta = @"
        UPDATE Proveedores
        SET RazonSocial = @RazonSocial,
            CUIT = @CUIT,
            Telefono = @Telefono,
            Email = @Email,
            Direccion = @Direccion,
            Localidad = @Localidad
        WHERE IdProveedor = @IdProveedor";
                    }
                    else
                    {
                        // REGISTRAR proveedor nuevo
                        consulta = @"
        INSERT INTO Proveedores
        (RazonSocial, CUIT, Telefono, Email, Direccion, Localidad)
        VALUES
        (@RazonSocial, @CUIT, @Telefono, @Email,
         @Direccion, @Localidad)";
                    }


                    using (SqlCommand cmd = new SqlCommand(consulta, con))
                    {
                        cmd.Parameters.AddWithValue("@RazonSocial", razonSocial);
                        cmd.Parameters.AddWithValue("@CUIT", cuit);
                        cmd.Parameters.AddWithValue("@Telefono", telefono);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Direccion", direccion);

                        cmd.Parameters.AddWithValue("@Localidad", localidad);

                        if (idProveedorEditando.HasValue)
                        {
                            cmd.Parameters.AddWithValue(
                                "@IdProveedor",
                                idProveedorEditando.Value
                            );
                        }

                        cmd.ExecuteNonQuery();

                    }
                }


                bool estabaEditando = idProveedorEditando.HasValue;

                MessageBox.Show(
                    estabaEditando
                        ? "Proveedor modificado correctamente."
                        : "Proveedor registrado correctamente.",
                    "Operación exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                RestablecerModoRegistro();
                CargarProveedores();

                // Si estábamos modificando, volver a Consultar proveedores
                if (estabaEditando)
                {
                    tabControlProveedores.SelectedTab = tabConsultarProveedores;
                }
            }

            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show(
                        "Ya existe un proveedor registrado con ese CUIT.",
                        "Proveedor duplicado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Error al registrar el proveedor: " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
        private void LimpiarCamposProveedor()
        {
            txtRazonSocial.Clear();
            txtCUIT.Clear();
            txtTelefonoProveedor.Clear();
            txtEmailProveedor.Clear();
            txtDireccionProveedor.Clear();
            txtLocalidadProveedor.Clear();

            txtRazonSocial.Focus();
        }

        private void RestablecerModoRegistro()
        {
            idProveedorEditando = null;

            btnGuardarProveedor.Text = "GUARDAR PROVEEDOR";
            btnLimpiarProveedor.Text = "LIMPIAR";

            LimpiarCamposProveedor();
        }


        private void btnLimpiarProveedor_Click(object sender, EventArgs e)
        {

            RestablecerModoRegistro();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Form5 frm = new Form5();

            frm.Show();

            this.Close();
        }

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            CargarProveedores(txtBuscarProveedor.Text.Trim());

        }

        private void btnMostrarTodosProveedores_Click(object sender, EventArgs e)
        {
            txtBuscarProveedor.Clear();
            CargarProveedores();
        }

    private void btnEditarProveedor_Click(object sender, EventArgs e)
        {
            int? id = ObtenerIdProveedorSeleccionado();

            if (!id.HasValue)
                return;

            DataGridViewRow fila = dgvProveedores.SelectedRows[0];

            // Guardamos el ID del proveedor que vamos a modificar
            idProveedorEditando = id.Value;

            // Cargamos los datos en los TextBox de Registrar
            txtRazonSocial.Text =
                Convert.ToString(fila.Cells["Razón social"].Value);

            txtCUIT.Text =
                Convert.ToString(fila.Cells["CUIT"].Value);

            txtTelefonoProveedor.Text =
                Convert.ToString(fila.Cells["Teléfono"].Value);

            txtEmailProveedor.Text =
                Convert.ToString(fila.Cells["Correo electrónico"].Value);

            txtDireccionProveedor.Text =
                Convert.ToString(fila.Cells["Dirección"].Value);

            txtLocalidadProveedor.Text =
                Convert.ToString(fila.Cells["Localidad"].Value);

            // Cambiamos a la pestaña Registrar
            tabControlProveedores.SelectedTab = tabRegistrarProveedor;

            // Activamos el modo modificación
            btnGuardarProveedor.Text = "GUARDAR CAMBIOS";
            btnLimpiarProveedor.Text = "CANCELAR";

            txtRazonSocial.Focus();
        }


        private void btnDarBajaProveedor_Click(object sender, EventArgs e)
        {
            int? id = ObtenerIdProveedorSeleccionado();

            if (!id.HasValue)
                return;

            string estadoActual = Convert.ToString(
                dgvProveedores.SelectedRows[0].Cells["Estado"].Value
            );

            if (estadoActual == "Inactivo")
            {
                MessageBox.Show("El proveedor ya está dado de baja.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de que desea dar de baja este proveedor?",
                "Confirmar baja",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                CambiarEstadoProveedor(id.Value, "Inactivo");
            }
        }


        private void btnReactivarProveedor_Click(object sender, EventArgs e)
        {
            int? id = ObtenerIdProveedorSeleccionado();

            if (!id.HasValue)
                return;

            string estadoActual = Convert.ToString(
                dgvProveedores.SelectedRows[0].Cells["Estado"].Value
            );

            if (estadoActual == "Activo")
            {
                MessageBox.Show("El proveedor ya se encuentra activo.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea reactivar este proveedor?",
                "Confirmar reactivación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                CambiarEstadoProveedor(id.Value, "Activo");
            }
        }

        private void CambiarEstadoProveedor(int idProveedor, string nuevoEstado)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    string consulta = @"
                UPDATE Proveedores
                SET Estado = @Estado
                WHERE IdProveedor = @IdProveedor";

                    using (SqlCommand cmd = new SqlCommand(consulta, con))
                    {
                        cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
                        cmd.Parameters.AddWithValue("@IdProveedor", idProveedor);

                        int filas = cmd.ExecuteNonQuery();

                        if (filas == 0)
                        {
                            MessageBox.Show("No se encontró el proveedor.");
                            return;
                        }
                    }
                }

                MessageBox.Show(
                    nuevoEstado == "Activo"
                        ? "Proveedor reactivado correctamente."
                        : "Proveedor dado de baja correctamente.",
                    "Operación exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                CargarProveedores();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al actualizar el proveedor: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void Form14_Load(object sender, EventArgs e)
        {

        }



        private void btnAsociarMaterial_Click(object sender, EventArgs e)
        {
            if (cmbProveedorCatalogo.SelectedIndex <= 0 ||
                cmbMaterialCatalogo.SelectedIndex <= 0)
            {
                MessageBox.Show(
                    "Debe seleccionar un proveedor y un material.",
                    "Campos obligatorios",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            int idProveedor =
    Convert.ToInt32(cmbProveedorCatalogo.SelectedValue);
            int idMaterial =
                Convert.ToInt32(cmbMaterialCatalogo.SelectedValue);

            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    string consulta = @"
                INSERT INTO ProveedorMateriales
                (IdProveedor, IdMaterial)
                VALUES (@IdProveedor, @IdMaterial)";

                    using (SqlCommand cmd = new SqlCommand(consulta, con))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdProveedor", idProveedor);

                        cmd.Parameters.AddWithValue(
                            "@IdMaterial", idMaterial);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Material asociado correctamente al proveedor.",
                    "Operación exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                cmbMaterialCatalogo.SelectedIndex = -1;
                CargarMaterialesProveedor();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show(
                        "Este material ya está asociado al proveedor.",
                        "Material duplicado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Error al asociar material: " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }



        private void CargarMaterialesProveedor()
        {
            try
            {
                int idProveedor = 0;
                int idMaterial = 0;

                if (cmbProveedorCatalogo.SelectedValue is int)
                    idProveedor = (int)cmbProveedorCatalogo.SelectedValue;

                if (cmbMaterialCatalogo.SelectedValue is int)
                    idMaterial = (int)cmbMaterialCatalogo.SelectedValue;

                string busqueda = txtBuscarCatalogo.Text.Trim();

                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string consulta = @"
                SELECT
                    PM.IdProveedorMaterial,
                    P.RazonSocial AS [Proveedor],
                    M.Nombre AS [Material],
                    M.UnidadMedida AS [Unidad]
                FROM ProveedorMateriales PM
                INNER JOIN Proveedores P
                    ON PM.IdProveedor = P.IdProveedor
                INNER JOIN Materiales M
                    ON PM.IdMaterial = M.IdMaterial
                WHERE
                    (@IdProveedor = 0 OR P.IdProveedor = @IdProveedor)
                    AND (@IdMaterial = 0 OR M.IdMaterial = @IdMaterial)
                    AND (
                        P.RazonSocial LIKE @Busqueda
                        OR M.Nombre LIKE @Busqueda
                    )
                ORDER BY M.Nombre, P.RazonSocial";

                    using (SqlCommand cmd = new SqlCommand(consulta, con))
                    {
                        cmd.Parameters.AddWithValue("@IdProveedor", idProveedor);
                        cmd.Parameters.AddWithValue("@IdMaterial", idMaterial);
                        cmd.Parameters.AddWithValue("@Busqueda", "%" + busqueda + "%");

                        SqlDataAdapter adaptador = new SqlDataAdapter(cmd);
                        DataTable tabla = new DataTable();
                        adaptador.Fill(tabla);

                        dgvCatalogoProveedor.DataSource = tabla;

                        dgvCatalogoProveedor.Columns[
                            "IdProveedorMaterial"].Visible = false;

                        dgvCatalogoProveedor.ClearSelection();
                        dgvCatalogoProveedor.CurrentCell = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al consultar el catálogo: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void txtBuscarCatalogo_TextChanged(object sender, EventArgs e)
        {
            CargarMaterialesProveedor();

        }

        private void cmbProveedorCatalogo_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarMaterialesProveedor();

        }

        private void cmbMaterialCatalogo_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarMaterialesProveedor();

        }


        private void btnQuitarMaterial_Click(object sender, EventArgs e)
        {
            if (dgvCatalogoProveedor.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Debe seleccionar una asociación de la lista.",
                    "Atención",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DataGridViewRow fila = dgvCatalogoProveedor.SelectedRows[0];

            int idAsociacion = Convert.ToInt32(
                fila.Cells["IdProveedorMaterial"].Value);

            string proveedor = Convert.ToString(
                fila.Cells["Proveedor"].Value);

            string material = Convert.ToString(
                fila.Cells["Material"].Value);

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de que desea quitar el material '" +
                material + "' del proveedor '" + proveedor + "'?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    string consulta = @"
                DELETE FROM ProveedorMateriales
                WHERE IdProveedorMaterial = @IdAsociacion";

                    using (SqlCommand cmd = new SqlCommand(consulta, con))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdAsociacion", idAsociacion);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            MessageBox.Show(
                                "La asociación ya no existe.",
                                "Atención",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            CargarMaterialesProveedor();
                            return;
                        }
                    }
                }

                MessageBox.Show(
                    "El material fue desvinculado correctamente del proveedor.",
                    "Operación exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                CargarMaterialesProveedor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al quitar la asociación: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        private void btnGenerarListadoProveedor_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridView grilla = dgvCatalogoProveedor;

                if (grilla.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "No hay materiales para incluir en el listado.",
                        "Listado vacío",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                using (SaveFileDialog guardar = new SaveFileDialog())
                {
                    guardar.Filter = "Archivo PDF (*.pdf)|*.pdf";
                    guardar.FileName =
                        "Listado_Productos_Proveedores_" +
                        DateTime.Now.ToString("yyyyMMdd_HHmm") + ".pdf";

                    if (guardar.ShowDialog() != DialogResult.OK)
                        return;

                    using (FileStream archivo = new FileStream(
                        guardar.FileName, FileMode.Create))
                    {
                        iTextSharp.text.Document documento =
                            new iTextSharp.text.Document(
                                iTextSharp.text.PageSize.A4.Rotate(),
                                35, 35, 40, 40
                            );

                        PdfWriter.GetInstance(documento, archivo);
                        documento.Open();

                        // FUENTES
                        BaseFont fuenteBase = BaseFont.CreateFont(
                            BaseFont.HELVETICA,
                            BaseFont.CP1252,
                            BaseFont.NOT_EMBEDDED
                        );

                        iTextSharp.text.Font fuenteTitulo =
                            new iTextSharp.text.Font(
                                fuenteBase, 17,
                                iTextSharp.text.Font.BOLD,
                                new BaseColor(8, 43, 82)
                            );

                        iTextSharp.text.Font fuenteNormal =
                            new iTextSharp.text.Font(
                                fuenteBase, 10,
                                iTextSharp.text.Font.NORMAL,
                                BaseColor.BLACK
                            );

                        iTextSharp.text.Font fuenteEncabezado =
                            new iTextSharp.text.Font(
                                fuenteBase, 10,
                                iTextSharp.text.Font.BOLD,
                                BaseColor.WHITE
                            );

                        // TÍTULO
                        Paragraph titulo = new Paragraph(
                            "C.E SYSTEM - IMRENTA CENTRAL\n" +
                            "LISTADO DE PRODUCTOS OFRECIDOS",
                            fuenteTitulo
                        );

                        titulo.Alignment = Element.ALIGN_CENTER;
                        titulo.SpacingAfter = 15;
                        documento.Add(titulo);

                        // FECHA
                        Paragraph fecha = new Paragraph(
                            "Fecha de emisión: " +
                            DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            fuenteNormal
                        );

                        fecha.SpacingAfter = 15;
                        documento.Add(fecha);

                        // TABLA
                        PdfPTable tabla = new PdfPTable(3);
                        tabla.WidthPercentage = 100;
                        tabla.SetWidths(new float[] { 3, 3, 1.5f });

                        string[] encabezados =
                        {
                    "Proveedor", "Material", "Unidad de medida"
                };

                        foreach (string encabezado in encabezados)
                        {
                            PdfPCell celda = new PdfPCell(
                                new Phrase(encabezado, fuenteEncabezado)
                            );

                            celda.BackgroundColor =
                                new BaseColor(8, 43, 82);

                            celda.Padding = 9;
                            tabla.AddCell(celda);
                        }

                        // REGISTROS
                        foreach (DataGridViewRow fila in grilla.Rows)
                        {
                            if (fila.IsNewRow)
                                continue;

                            string[] valores =
                            {
                        Convert.ToString(fila.Cells["Proveedor"].Value),
                        Convert.ToString(fila.Cells["Material"].Value),
                        Convert.ToString(fila.Cells["Unidad"].Value)
                    };

                            foreach (string valor in valores)
                            {
                                PdfPCell celda = new PdfPCell(
                                    new Phrase(valor ?? "", fuenteNormal)
                                );

                                celda.Padding = 8;
                                tabla.AddCell(celda);
                            }
                        }

                        documento.Add(tabla);

                        Paragraph pie = new Paragraph(
                            "\nTotal de asociaciones: " +
                            grilla.Rows.Cast<DataGridViewRow>()
                                .Count(f => !f.IsNewRow),
                            fuenteNormal
                        );

                        documento.Add(pie);
                        documento.Close();
                    }

                    MessageBox.Show(
                        "El listado PDF se generó correctamente.",
                        "Operación exitosa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al generar el PDF: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

       

        private void CargarProveedoresCompra()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string sql = @"SELECT IdProveedor, RazonSocial
                       FROM Proveedores
                       WHERE Estado = 'Activo'
                       ORDER BY RazonSocial";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbProveedorCompra.DataSource = dt;
                cmbProveedorCompra.DisplayMember = "RazonSocial";
                cmbProveedorCompra.ValueMember = "IdProveedor";
                cmbProveedorCompra.SelectedIndex = -1;
            }
        }

        private void CargarMaterialesCompra()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string sql = @"SELECT IdMaterial,
                              Nombre + ' (' + UnidadMedida + ')' AS Material
                       FROM Materiales
                       WHERE Estado = 'Activo'
                       ORDER BY Nombre";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbMaterialCompra.DataSource = dt;
                cmbMaterialCompra.DisplayMember = "Material";
                cmbMaterialCompra.ValueMember = "IdMaterial";
                cmbMaterialCompra.SelectedIndex = -1;
            }
        }

        private void CargarCompras()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string sql = @"
            SELECT
                c.IdCompra,
                c.FechaCompra AS Fecha,
                p.RazonSocial AS Proveedor,
                m.Nombre AS Material,
                c.Cantidad,
                c.CostoUnitario,
                c.Total,
                c.Estado
            FROM ComprasMateriales c
            INNER JOIN Proveedores p
                ON c.IdProveedor = p.IdProveedor
            INNER JOIN Materiales m
                ON c.IdMaterial = m.IdMaterial
            ORDER BY c.IdCompra DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvCompras.DataSource = dt;
                dgvCompras.ReadOnly = true;
                dgvCompras.AllowUserToAddRows = false;
                dgvCompras.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvCompras.MultiSelect = false;
                dgvCompras.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
            }
        }


        private void btnGuardarCompra_Click(object sender, EventArgs e)
        {
            if (cmbProveedorCompra.SelectedIndex == -1 ||
                cmbMaterialCompra.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un proveedor y un material.");
                return;
            }

            if (nudCantidadCompra.Value <= 0 ||
                nudCostoUnitarioCompra.Value <= 0)
            {
                MessageBox.Show("La cantidad y el costo deben ser mayores a cero.");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    string sql = @"
                INSERT INTO ComprasMateriales
                (IdProveedor, IdMaterial, Cantidad,
                 CostoUnitario, FechaCompra, Observaciones, Estado)
                VALUES
                (@Proveedor, @Material, @Cantidad,
                 @Costo, @Fecha, @Observaciones, 'Pendiente')";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Proveedor", cmbProveedorCompra.SelectedValue);

                        cmd.Parameters.AddWithValue(
                            "@Material", cmbMaterialCompra.SelectedValue);

                        cmd.Parameters.AddWithValue(
                            "@Cantidad", nudCantidadCompra.Value);

                        cmd.Parameters.AddWithValue(
                            "@Costo", nudCostoUnitarioCompra.Value);

                        cmd.Parameters.AddWithValue(
                            "@Fecha", dtpFechaCompra.Value.Date);

                        cmd.Parameters.AddWithValue(
                            "@Observaciones", txtObservacionesCompra.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Compra registrada correctamente. Estado: Pendiente.",
                    "Compra registrada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarCompras();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la compra: " + ex.Message);
            }
        }


        private void btnConfirmarRecepcion_Click(object sender, EventArgs e)
        {
            if (dgvCompras.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una compra.");
                return;
            }

            int idCompra = Convert.ToInt32(
                dgvCompras.CurrentRow.Cells["IdCompra"].Value);

            DialogResult respuesta = MessageBox.Show(
                "¿Confirma que recibió los materiales de esta compra?",
                "Confirmar recepción",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    using (SqlTransaction trans = con.BeginTransaction())
                    {
                        try
                        {
                            int idMaterial;
                            decimal cantidad;

                            string consultar = @"
                        SELECT IdMaterial, Cantidad
                        FROM ComprasMateriales WITH (UPDLOCK, HOLDLOCK)
                        WHERE IdCompra = @IdCompra
                          AND Estado = 'Pendiente'";

                            using (SqlCommand cmd =
                                new SqlCommand(consultar, con, trans))
                            {
                                cmd.Parameters.AddWithValue("@IdCompra", idCompra);

                                using (SqlDataReader dr = cmd.ExecuteReader())
                                {
                                    if (!dr.Read())
                                    {
                                        throw new Exception(
                                            "La compra ya fue recibida o no está pendiente.");
                                    }

                                    idMaterial = Convert.ToInt32(dr["IdMaterial"]);
                                    cantidad = Convert.ToDecimal(dr["Cantidad"]);
                                }
                            }

                            string actualizarStock = @"
                        UPDATE Materiales
                        SET StockActual = StockActual + @Cantidad
                        WHERE IdMaterial = @IdMaterial";

                            using (SqlCommand cmd =
                                new SqlCommand(actualizarStock, con, trans))
                            {
                                cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                                cmd.Parameters.AddWithValue("@IdMaterial", idMaterial);

                                if (cmd.ExecuteNonQuery() != 1)
                                    throw new Exception("No se encontró el material.");
                            }

                            string movimiento = @"
                        INSERT INTO MovimientosStock
                        (IdMaterial, TipoMovimiento, Cantidad, Observaciones)
                        VALUES
                        (@IdMaterial, 'Entrada', @Cantidad, @Observaciones)";

                            using (SqlCommand cmd =
                                new SqlCommand(movimiento, con, trans))
                            {
                                cmd.Parameters.AddWithValue("@IdMaterial", idMaterial);
                                cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                                cmd.Parameters.AddWithValue(
                                    "@Observaciones",
                                    "Recepción de compra N° " + idCompra);

                                cmd.ExecuteNonQuery();
                            }

                            string actualizarCompra = @"
                        UPDATE ComprasMateriales
                        SET Estado = 'Recibida',
                            FechaRecepcion = GETDATE()
                        WHERE IdCompra = @IdCompra
                          AND Estado = 'Pendiente'";

                            using (SqlCommand cmd =
                                new SqlCommand(actualizarCompra, con, trans))
                            {
                                cmd.Parameters.AddWithValue("@IdCompra", idCompra);

                                if (cmd.ExecuteNonQuery() != 1)
                                    throw new Exception(
                                        "No se pudo actualizar el estado de la compra.");
                            }

                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }

                MessageBox.Show(
                    "Recepción confirmada. El stock fue actualizado correctamente.",
                    "Recepción registrada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarCompras();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al confirmar recepción: " + ex.Message);
            }
        }

        private void dgvCompras_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void nudCostoUnitarioCompra_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
    

