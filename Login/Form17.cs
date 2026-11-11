using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Login
{
    public partial class Form17 : Form
    {
        private readonly string cadenaConexion =
            @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";

        private readonly Color fondo = Color.FromArgb(244, 242, 250);
        private readonly Color azulPetroleo = Color.FromArgb(23, 43, 69);
        private readonly Color violeta = Color.FromArgb(109, 90, 230);
        private readonly Color lavanda = Color.FromArgb(232, 226, 255);
        private readonly Color texto = Color.FromArgb(48, 58, 82);
        private readonly Color grisSuave = Color.FromArgb(115, 124, 145);

        private TabControl pestanas;
        private TabPage tabPedidos, tabRecepciones, tabConsultas;

        private ComboBox cboPedido, cboTrabajo, cboProveedor;
        private NumericUpDown nudCantidad;
        private DateTimePicker dtpFechaEntrega;
        private TextBox txtObservaciones;
        private Button btnRegistrarPedido;

        private ComboBox cboPedidoTercero;
        private NumericUpDown nudCantidadRecibida;
        private DateTimePicker dtpFechaRecepcion;
        private TextBox txtObservacionesRecepcion;
        private Button btnRegistrarRecepcion;

        private DataGridView dgvPedidosTerceros, dgvRecepciones, dgvConsultaPedidos;
        private Label lblPendientes, lblEnProceso, lblRecibidos;

        private Label lblUnidadesSolicitadas;
        private Label lblUnidadesRecibidas;
        private Label lblUnidadesPendientes;

        private Label lblSeguimientoPendientes, lblSeguimientoParciales, lblSeguimientoRecibidos;
        private TextBox txtFiltroPedido;
        private ComboBox cboFiltroFabricante, cboFiltroEstado;
        private Button btnBuscarSeguimiento, btnMostrarTodosSeguimiento;
        private DataGridView dgvEntregasSeguimiento;
        private DataTable datosSeguimiento;
        private bool actualizandoFiltrosSeguimiento;

        private Button btnModificarPedido;
        private Button btnCancelarPedido;
        private int? idPedidoEnEdicion = null;

        public Form17()
        {
            InitializeComponent();
            ConstruirInterfaz();
            Load += Form17_Load;
        }

        private void ConstruirInterfaz()
        {
            Controls.Clear();
            Text = "C.E SYSTEM - Gestión de productos de terceros";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 700);
            BackColor = fondo;
            Font = new Font("Segoe UI", 10);

            pestanas = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Point(18, 8)
            };
            tabPedidos = new TabPage("Registrar pedido") { BackColor = fondo, Padding = new Padding(12) };
            tabRecepciones = new TabPage("Registrar recepción") { BackColor = fondo, Padding = new Padding(12) };
            tabConsultas = new TabPage("Seguimiento de pedidos") { BackColor = fondo, Padding = new Padding(12) };
            pestanas.TabPages.Add(tabPedidos);
            pestanas.TabPages.Add(tabRecepciones);
            pestanas.TabPages.Add(tabConsultas);
            Controls.Add(pestanas);

            CrearPantallaPedidos();
            CrearPantallaRecepciones();
            CrearPantallaConsultas();
        }

        private void Form17_Load(object sender, EventArgs e)
        {
            CargarPedidos();
            CargarProveedores();
            CargarPedidosTerceros();
            CargarHistorial();
            ActualizarIndicadoresRecepcion();
        }

        private TableLayoutPanel CrearEstructura(TabPage pagina)
        {
            TableLayoutPanel tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 105));
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 115));
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 295));
            tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            pagina.Controls.Add(tabla);
            return tabla;
        }

        private void AgregarIndicadoresRecepcion(TableLayoutPanel tabla)
        {
            TableLayoutPanel indicadores = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0)
            };

            for (int i = 0; i < 3; i++)
            {
                indicadores.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 33.333F));
            }

            indicadores.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            lblUnidadesSolicitadas =
                CrearIndicador("UNIDADES SOLICITADAS", "0");

            lblUnidadesRecibidas =
                CrearIndicador("UNIDADES RECIBIDAS", "0");

            lblUnidadesPendientes =
                CrearIndicador("UNIDADES PENDIENTES", "0");

            lblUnidadesRecibidas.ForeColor =
                Color.FromArgb(36, 150, 106);

            lblUnidadesPendientes.ForeColor =
                Color.FromArgb(217, 149, 39);

            indicadores.Controls.Add(
                (Control)lblUnidadesSolicitadas.Tag, 0, 0);

            indicadores.Controls.Add(
                (Control)lblUnidadesRecibidas.Tag, 1, 0);

            indicadores.Controls.Add(
                (Control)lblUnidadesPendientes.Tag, 2, 0);

            tabla.Controls.Add(indicadores, 0, 1);
        }

        private Panel CrearTarjeta()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(5),
                Padding = new Padding(16)
            };
        }

        private Control CrearEncabezado(string titulo, string subtitulo)
        {
            Panel encabezado = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = azulPetroleo,
                Margin = new Padding(5)
            };
            Label tituloLabel = new Label
            {
                Text = titulo,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 52,
                Padding = new Padding(24, 12, 0, 0)
            };
            Label subtituloLabel = new Label
            {
                Text = subtitulo,
                ForeColor = lavanda,
                Font = new Font("Segoe UI", 11),
                Dock = DockStyle.Fill,
                Padding = new Padding(25, 0, 0, 0)
            };
            encabezado.Controls.Add(subtituloLabel);
            encabezado.Controls.Add(tituloLabel);
            return encabezado;
        }

        private Label CrearTituloSeccion(string titulo)
        {
            return new Label
            {
                Text = titulo,
                Dock = DockStyle.Top,
                Height = 42,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = azulPetroleo,
                Padding = new Padding(5, 6, 0, 0)
            };
        }

        private Control CrearCampo(string titulo, Control control, int ancho = 230)
        {
            Panel contenedor = new Panel
            {
                Width = ancho,
                Height = 82,
                Margin = new Padding(9, 6, 9, 6)
            };
            Label etiqueta = new Label
            {
                Text = titulo,
                Location = new Point(0, 0),
                Size = new Size(ancho, 25),
                ForeColor = texto,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            control.Location = new Point(0, 29);
            control.Width = ancho - 10;
            contenedor.Controls.Add(etiqueta);
            contenedor.Controls.Add(control);
            return contenedor;
        }

        private Button CrearBoton(string titulo)
        {
            Button boton = new Button
            {
                Text = titulo,
                Size = new Size(205, 43),
                BackColor = violeta,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                UseVisualStyleBackColor = false
            };
            boton.FlatAppearance.BorderSize = 0;
            return boton;
        }

        private DataGridView CrearGrilla()
        {
            DataGridView grilla = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = azulPetroleo,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    SelectionBackColor = violeta,
                    SelectionForeColor = Color.White,
                    Font = new Font("Segoe UI", 10)
                }
            };
            grilla.RowTemplate.Height = 33;
            return grilla;
        }

        private Label CrearIndicador(TableLayoutPanel indicadores, int columna, string titulo)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(5),
                Padding = new Padding(14)
            };
            Label etiqueta = new Label
            {
                Text = titulo,
                Dock = DockStyle.Top,
                Height = 25,
                ForeColor = grisSuave,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            Label valor = new Label
            {
                Text = "0",
                Dock = DockStyle.Fill,
                ForeColor = violeta,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            panel.Controls.Add(valor);
            panel.Controls.Add(etiqueta);
            indicadores.Controls.Add(panel, columna, 0);
            return valor;
        }

        private void AgregarIndicadores(TableLayoutPanel tabla)
        {
            TableLayoutPanel indicadores = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            for (int i = 0; i < 3; i++)
                indicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));
            indicadores.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            lblPendientes = CrearIndicador(indicadores, 0, "PEDIDOS PENDIENTES");
            lblEnProceso = CrearIndicador(indicadores, 1, "EN PROCESO");
            lblRecibidos = CrearIndicador(indicadores, 2, "PEDIDOS RECIBIDOS");
            tabla.Controls.Add(indicadores, 0, 1);
        }

        // Reserva una fila independiente para los botones: no pueden quedar ocultos
        // ni ser movidos por FlowLayoutPanel a otra posición.
        private void PrepararTarjetaFormulario(
            Panel tarjeta, string titulo, FlowLayoutPanel campos, Button boton)
        {
            TableLayoutPanel contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = new Padding(0)
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));

            Label encabezado = CrearTituloSeccion(titulo);
            campos.Dock = DockStyle.Fill;
            campos.Padding = new Padding(4, 8, 4, 0);
            campos.WrapContents = true;
            campos.AutoScroll = true;

            Panel pie = new Panel { Dock = DockStyle.Fill };
            boton.Location = new Point(5, 6);
            boton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            pie.Controls.Add(boton);

            contenido.Controls.Add(encabezado, 0, 0);
            contenido.Controls.Add(campos, 0, 1);
            contenido.Controls.Add(pie, 0, 2);
            tarjeta.Controls.Add(contenido);
        }

        private void PrepararTarjetaGrilla(Panel tarjeta, string titulo, DataGridView grilla)
        {
            Panel contenido = new Panel { Dock = DockStyle.Fill };
            grilla.Dock = DockStyle.Fill;
            contenido.Controls.Add(grilla);
            contenido.Controls.Add(CrearTituloSeccion(titulo));
            tarjeta.Controls.Add(contenido);
        }

        private void CrearPantallaPedidos()
        {
            TableLayoutPanel tabla = CrearEstructura(tabPedidos);
            tabla.Controls.Add(CrearEncabezado(
                "GESTIÓN DE PRODUCTOS DE TERCEROS",
                "Pedidos realizados por Imprenta Central a fabricantes externos"), 0, 0);
            AgregarIndicadores(tabla);

            Panel tarjeta = CrearTarjeta();
            tabla.Controls.Add(tarjeta, 0, 2);
            FlowLayoutPanel campos = new FlowLayoutPanel();
            cboPedido = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cboTrabajo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cboProveedor = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            nudCantidad = new NumericUpDown { Minimum = 1, Maximum = 1000000, Value = 1 };
            dtpFechaEntrega = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(7)
            };
            txtObservaciones = new TextBox { Multiline = true, Height = 48 };
            cboTrabajo.SelectedIndexChanged += (s, e) =>
            {
                if (cboTrabajo.SelectedItem is DataRowView fila)
                {
                    int cantidad = Convert.ToInt32(fila["Cantidad"]);

                    if (cantidad >= nudCantidad.Minimum &&
                        cantidad <= nudCantidad.Maximum)
                    {
                        nudCantidad.Value = cantidad;
                    }
                }
            };

            campos.Controls.Add(CrearCampo("Pedido del cliente", cboPedido));
            campos.Controls.Add(CrearCampo("Producto solicitado", cboTrabajo));
            campos.Controls.Add(CrearCampo("Empresa fabricante", cboProveedor));
            campos.Controls.Add(CrearCampo("Cantidad solicitada", nudCantidad));
            campos.Controls.Add(CrearCampo("Entrega estimada", dtpFechaEntrega));
            campos.Controls.Add(CrearCampo("Observaciones", txtObservaciones, 300));

            btnRegistrarPedido = CrearBoton("Registrar pedido");
            PrepararTarjetaFormulario(tarjeta, "NUEVO PEDIDO A FABRICANTE", campos, btnRegistrarPedido);

            Panel historial = CrearTarjeta();
            tabla.Controls.Add(historial, 0, 3);
            dgvPedidosTerceros = CrearGrilla();

            PrepararTarjetaGrilla(
                historial,
                "PEDIDOS A FABRICANTES REGISTRADOS",
                dgvPedidosTerceros);

            // Panel de botones debajo de la grilla
            FlowLayoutPanel acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(10, 8, 10, 5),
                BackColor = Color.White
            };

            btnCancelarPedido = CrearBoton("Cancelar pedido");
            btnCancelarPedido.BackColor = Color.FromArgb(190, 55, 65);
            btnCancelarPedido.Width = 170;
            btnCancelarPedido.Height = 38;

            btnModificarPedido = CrearBoton("Modificar pedido");
            btnModificarPedido.BackColor = Color.FromArgb(109, 90, 230);
            btnModificarPedido.Width = 170;
            btnModificarPedido.Height = 38;

            acciones.Controls.Add(btnCancelarPedido);
            acciones.Controls.Add(btnModificarPedido);

            historial.Controls.Add(acciones);
            acciones.BringToFront();

            btnModificarPedido.Click += (s, e) => ModificarPedidoSeleccionado();
            btnCancelarPedido.Click += (s, e) => CancelarPedidoSeleccionado();

            cboPedido.SelectedIndexChanged += (s, e) =>
            {
                if (cboPedido.SelectedValue is int)
                    CargarTrabajos(Convert.ToInt32(cboPedido.SelectedValue));
            };
            btnRegistrarPedido.Click += (s, e) => RegistrarPedidoTercero();
        }
        private int? ObtenerPedidoSeleccionado()
        {
            if (dgvPedidosTerceros.CurrentRow == null ||
                dgvPedidosTerceros.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Seleccione un pedido de la grilla.");
                return null;
            }

            return Convert.ToInt32(
                dgvPedidosTerceros.CurrentRow.Cells["N.º Pedido"].Value);
        }
        private void CancelarPedidoSeleccionado()
        {
            int? id = ObtenerPedidoSeleccionado();
            if (!id.HasValue) return;

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de cancelar el pedido externo N.º " + id + "?",
                "Confirmar cancelación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    using (SqlCommand comando = new SqlCommand(@"
                UPDATE dbo.PedidosTerceros
                SET Estado = 'Cancelado'
                WHERE IdPedidoTercero = @Id
                  AND Estado = 'Pendiente'
                  AND NOT EXISTS (
                      SELECT 1
                      FROM dbo.RecepcionesTerceros
                      WHERE IdPedidoTercero = @Id
                  )", conexion))
                    {
                        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id.Value;

                        int filas = comando.ExecuteNonQuery();

                        if (filas == 0)
                        {
                            MessageBox.Show(
                                "El pedido no puede cancelarse porque ya tiene recepciones, " +
                                "está cancelado o no está pendiente.");
                            return;
                        }
                    }
                }

                MessageBox.Show("Pedido cancelado correctamente.");

                CargarPedidosTerceros();
                CargarHistorial();
                ActualizarIndicadoresRecepcion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cancelar el pedido:\n" + ex.Message);
            }
        }
        private void ModificarPedidoSeleccionado()
        {
            int? id = ObtenerPedidoSeleccionado();
            if (!id.HasValue) return;

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                using (SqlCommand comando = new SqlCommand(@"
            SELECT
                pt.IdPedido,
                pt.IdDetalle,
                pt.IdProveedor,
                pt.CantidadSolicitada,
                pt.FechaEntregaEstimada,
                pt.Observaciones
            FROM dbo.PedidosTerceros pt
            WHERE pt.IdPedidoTercero = @Id
              AND pt.Estado = 'Pendiente'
              AND NOT EXISTS (
                  SELECT 1
                  FROM dbo.RecepcionesTerceros r
                  WHERE r.IdPedidoTercero = pt.IdPedidoTercero
              )", conexion))
                {
                    comando.Parameters.Add("@Id", SqlDbType.Int).Value = id.Value;
                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (!lector.Read())
                        {
                            MessageBox.Show(
                                "Solo se pueden modificar pedidos pendientes sin recepciones.");
                            return;
                        }

                        int pedidoCliente = Convert.ToInt32(lector["IdPedido"]);
                        int detalle = Convert.ToInt32(lector["IdDetalle"]);
                        int proveedor = Convert.ToInt32(lector["IdProveedor"]);
                        int cantidad = Convert.ToInt32(lector["CantidadSolicitada"]);
                        DateTime fecha = Convert.ToDateTime(lector["FechaEntregaEstimada"]);
                        string observaciones = Convert.ToString(lector["Observaciones"]);

                        cboPedido.SelectedValue = pedidoCliente;
                        CargarTrabajos(pedidoCliente);
                        cboTrabajo.SelectedValue = detalle;
                        cboProveedor.SelectedValue = proveedor;
                        nudCantidad.Value = cantidad;
                        dtpFechaEntrega.Value = fecha;
                        txtObservaciones.Text = observaciones;
                    }
                }

                idPedidoEnEdicion = id.Value;
                btnRegistrarPedido.Text = "Guardar cambios";
                pestanas.SelectedTab = tabPedidos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el pedido:\n" + ex.Message);
            }
        }
        private void CrearPantallaRecepciones()
        {
            TableLayoutPanel tabla = CrearEstructura(tabRecepciones);
            tabla.Controls.Add(CrearEncabezado(
                "RECEPCIÓN DE PRODUCTOS EXTERNOS",
                "Registro de entregas realizadas por empresas fabricantes"), 0, 0);

            AgregarIndicadoresRecepcion(tabla);

            Panel tarjeta = CrearTarjeta();
            tabla.Controls.Add(tarjeta, 0, 2);
            FlowLayoutPanel campos = new FlowLayoutPanel();
            cboPedidoTercero = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            nudCantidadRecibida = new NumericUpDown { Minimum = 1, Maximum = 1000000, Value = 1 };
            dtpFechaRecepcion = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            txtObservacionesRecepcion = new TextBox { Multiline = true, Height = 48 };

            campos.Controls.Add(CrearCampo("Pedido al fabricante", cboPedidoTercero, 330));
            campos.Controls.Add(CrearCampo("Cantidad recibida", nudCantidadRecibida));
            campos.Controls.Add(CrearCampo("Fecha de recepción", dtpFechaRecepcion));
            campos.Controls.Add(CrearCampo("Observaciones de entrega", txtObservacionesRecepcion, 320));

            btnRegistrarRecepcion = CrearBoton("Registrar recepción");
            PrepararTarjetaFormulario(tarjeta, "REGISTRAR NUEVA RECEPCIÓN", campos, btnRegistrarRecepcion);

            Panel historial = CrearTarjeta();
            tabla.Controls.Add(historial, 0, 3);
            dgvRecepciones = CrearGrilla();
            PrepararTarjetaGrilla(historial, "HISTORIAL DE RECEPCIONES", dgvRecepciones);

            btnRegistrarRecepcion.Click += (s, e) => RegistrarRecepcion();
        }

        private void CrearPantallaConsultas()
        {
            TableLayoutPanel tabla = CrearEstructura(tabConsultas);
            tabla.RowStyles[1].Height = 115;
            tabla.RowStyles[2].Height = 245;
            tabla.Controls.Add(CrearEncabezado(
                "SEGUIMIENTO DE PEDIDOS EXTERNOS",
                "Control de pedidos realizados a fabricantes"), 0, 0);

            TableLayoutPanel indicadores = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0)
            };
            for (int i = 0; i < 3; i++)
                indicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));
            indicadores.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            lblSeguimientoPendientes = CrearIndicador(indicadores, 0, "PEDIDOS PENDIENTES");
            lblSeguimientoParciales = CrearIndicador(indicadores, 1, "ENTREGAS PARCIALES / EN PROCESO");
            lblSeguimientoRecibidos = CrearIndicador(indicadores, 2, "PEDIDOS RECIBIDOS");
            lblSeguimientoPendientes.ForeColor = Color.FromArgb(175, 55, 55);
            lblSeguimientoParciales.ForeColor = Color.FromArgb(190, 132, 25);
            lblSeguimientoRecibidos.ForeColor = Color.FromArgb(36, 150, 106);
            tabla.Controls.Add(indicadores, 0, 1);

            Panel tarjeta = CrearTarjeta();
            tabla.Controls.Add(tarjeta, 0, 2);
            TableLayoutPanel filtros = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            filtros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            filtros.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            filtros.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
            filtros.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            filtros.Controls.Add(CrearTituloSeccion("CONSULTAR PEDIDOS A FABRICANTES"), 0, 0);

            FlowLayoutPanel campos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = true,
                AutoScroll = true
            };
            txtFiltroPedido = new TextBox();
            cboFiltroFabricante = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cboFiltroEstado = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cboFiltroEstado.Items.Clear();

            cboFiltroEstado.Items.AddRange(new object[]
            {
            "Todos",
            "Pendiente",
            "Recibido parcialmente",
            "Recibido",
            "Cancelado"
            });

            cboFiltroEstado.SelectedIndex = 0;
            cboFiltroEstado.SelectedIndex = 0;
            campos.Controls.Add(CrearCampo("N.º pedido externo", txtFiltroPedido, 205));
            campos.Controls.Add(CrearCampo("Fabricante", cboFiltroFabricante, 300));
            campos.Controls.Add(CrearCampo("Estado", cboFiltroEstado, 260));
            filtros.Controls.Add(campos, 0, 1);

            FlowLayoutPanel acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false
            };
            btnBuscarSeguimiento = CrearBoton("Buscar");
            btnBuscarSeguimiento.Size = new Size(150, 39);
            btnMostrarTodosSeguimiento = CrearBoton("Mostrar todos");
            btnMostrarTodosSeguimiento.Size = new Size(175, 39);
            btnMostrarTodosSeguimiento.BackColor = azulPetroleo;
            acciones.Controls.Add(btnBuscarSeguimiento);
            acciones.Controls.Add(btnMostrarTodosSeguimiento);
            filtros.Controls.Add(acciones, 0, 2);
            tarjeta.Controls.Add(filtros);

            Panel historial = CrearTarjeta();
            tabla.Controls.Add(historial, 0, 3);
            TableLayoutPanel contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 62));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
            contenido.Controls.Add(CrearTituloSeccion("HISTORIAL GENERAL DE PEDIDOS"), 0, 0);
            dgvConsultaPedidos = CrearGrilla();
            contenido.Controls.Add(dgvConsultaPedidos, 0, 1);
            contenido.Controls.Add(CrearTituloSeccion("ENTREGAS DEL PEDIDO SELECCIONADO"), 0, 2);
            dgvEntregasSeguimiento = CrearGrilla();
            contenido.Controls.Add(dgvEntregasSeguimiento, 0, 3);
            historial.Controls.Add(contenido);

            btnBuscarSeguimiento.Click += (s, e) => AplicarFiltrosSeguimiento();
            btnMostrarTodosSeguimiento.Click += (s, e) =>
            {
                txtFiltroPedido.Clear();
                cboFiltroFabricante.SelectedIndex = 0;
                cboFiltroEstado.SelectedIndex = 0;
                AplicarFiltrosSeguimiento();
            };
            txtFiltroPedido.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    AplicarFiltrosSeguimiento();
                }
            };
            dgvConsultaPedidos.SelectionChanged += (s, e) => CargarEntregasSeleccionadas();
        }

        private void ActualizarFiltrosFabricantes()
        {
            if (cboFiltroFabricante == null || datosSeguimiento == null) return;
            string anterior = cboFiltroFabricante.SelectedItem as string ?? "Todos";
            actualizandoFiltrosSeguimiento = true;
            cboFiltroFabricante.Items.Clear();
            cboFiltroFabricante.Items.Add("Todos");
            foreach (DataRow fila in datosSeguimiento.Rows)
            {
                string fabricante = Convert.ToString(fila["Fabricante"]);
                if (!cboFiltroFabricante.Items.Contains(fabricante))
                    cboFiltroFabricante.Items.Add(fabricante);
            }
            int indice = cboFiltroFabricante.Items.IndexOf(anterior);
            cboFiltroFabricante.SelectedIndex = indice >= 0 ? indice : 0;
            actualizandoFiltrosSeguimiento = false;
        }

        private void AplicarFiltrosSeguimiento()
        {
            if (actualizandoFiltrosSeguimiento || datosSeguimiento == null) return;
            string pedido = txtFiltroPedido.Text.Trim();
            string fabricante = cboFiltroFabricante.SelectedItem as string ?? "Todos";
            string estado = cboFiltroEstado.SelectedItem as string ?? "Todos";

            if (pedido.Length > 0 && !int.TryParse(pedido, out _))
            {
                MessageBox.Show("Ingresá un número de pedido válido.");
                return;
            }

            DataTable filtrada = datosSeguimiento.Clone();
            foreach (DataRow fila in datosSeguimiento.Rows)
            {
                if (pedido.Length > 0 && Convert.ToString(fila["N.º Pedido"]) != pedido) continue;
                if (fabricante != "Todos" && Convert.ToString(fila["Fabricante"]) != fabricante) continue;
                if (estado != "Todos" && Convert.ToString(fila["Estado"]) != estado) continue;
                filtrada.ImportRow(fila);
            }
            dgvConsultaPedidos.DataSource = filtrada;
            ColorearPedidos(dgvConsultaPedidos);
            dgvConsultaPedidos.ClearSelection();
            dgvEntregasSeguimiento.DataSource = null;
        }

        private void CargarEntregasSeleccionadas()
        {
            if (dgvEntregasSeguimiento == null || dgvConsultaPedidos.CurrentRow == null ||
                dgvConsultaPedidos.SelectedRows.Count == 0) return;
            object valor = dgvConsultaPedidos.CurrentRow.Cells["N.º Pedido"].Value;
            if (valor == null || valor == DBNull.Value) return;
            int id = Convert.ToInt32(valor);
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                using (SqlCommand comando = new SqlCommand(@"
                    SELECT IdRecepcion AS [N.º Recepción],
                           FechaRecepcion AS [Fecha de entrega],
                           CantidadRecibida AS [Unidades recibidas],
                           Observaciones
                    FROM dbo.RecepcionesTerceros
                    WHERE IdPedidoTercero = @Id
                    ORDER BY FechaRecepcion DESC, IdRecepcion DESC", conexion))
                {
                    comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    DataTable tabla = new DataTable();
                    using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                        adaptador.Fill(tabla);
                    dgvEntregasSeguimiento.DataSource = tabla;
                    dgvEntregasSeguimiento.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar entregas: " + ex.Message);
            }
        }

        // Pendientes: se conectarán con las tablas de SQL Server en el siguiente paso.

        private void CargarPedidos()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    SqlDataAdapter adaptador = new SqlDataAdapter(
                        "SELECT IdPedido FROM dbo.Pedidos ORDER BY IdPedido DESC",
                        conexion);

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    cboPedido.DataSource = tabla;
                    cboPedido.DisplayMember = "IdPedido";
                    cboPedido.ValueMember = "IdPedido";
                    cboPedido.SelectedIndex = -1;
                    cboTrabajo.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pedidos: " + ex.Message);
            }
        }

        private void CargarTrabajos(int idPedido)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                using (SqlCommand comando = new SqlCommand(@"
            SELECT IdDetalle, Trabajo, Cantidad
            FROM dbo.DetallePedido
            WHERE IdPedido = @IdPedido
            ORDER BY IdDetalle", conexion))
                {
                    comando.Parameters.Add("@IdPedido", SqlDbType.Int)
                        .Value = idPedido;

                    DataTable tabla = new DataTable();

                    using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                        adaptador.Fill(tabla);

                    cboTrabajo.DataSource = tabla;
                    cboTrabajo.DisplayMember = "Trabajo";
                    cboTrabajo.ValueMember = "IdDetalle";
                    cboTrabajo.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private void CargarProveedores()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    SqlDataAdapter adaptador = new SqlDataAdapter(@"
                SELECT IdProveedor, RazonSocial
                FROM dbo.Proveedores
                ORDER BY RazonSocial", conexion);

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    cboProveedor.DataSource = tabla;
                    cboProveedor.DisplayMember = "RazonSocial";
                    cboProveedor.ValueMember = "IdProveedor";
                    cboProveedor.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar fabricantes: " + ex.Message);
            }
        }

        private void CargarPedidosTerceros()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    SqlDataAdapter adaptador = new SqlDataAdapter(@"
                        SELECT pt.IdPedidoTercero AS [N.º Pedido],
                               pt.IdPedido AS [Pedido cliente],
                               d.Trabajo AS Producto,
                               p.RazonSocial AS Fabricante,
                               pt.CantidadSolicitada AS Cantidad,
                               ISNULL(rec.Recibidas, 0) AS [Cantidad recibida],
                               pt.CantidadSolicitada - ISNULL(rec.Recibidas, 0) AS [Cantidad pendiente],
                               pt.FechaSolicitud AS [Fecha solicitud],
                               pt.FechaEntregaEstimada AS [Entrega estimada],
                               pt.Estado
                        FROM dbo.PedidosTerceros pt
                        INNER JOIN dbo.DetallePedido d ON pt.IdDetalle = d.IdDetalle
                        INNER JOIN dbo.Proveedores p ON pt.IdProveedor = p.IdProveedor
                        OUTER APPLY (
                            SELECT SUM(r.CantidadRecibida) AS Recibidas
                            FROM dbo.RecepcionesTerceros r
                            WHERE r.IdPedidoTercero = pt.IdPedidoTercero
                        ) rec
                        ORDER BY pt.IdPedidoTercero DESC", conexion);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    dgvPedidosTerceros.DataSource = tabla;
                    datosSeguimiento = tabla.Copy();
                    ColorearPedidos(dgvPedidosTerceros);
                    ActualizarFiltrosFabricantes();
                    AplicarFiltrosSeguimiento();

                    using (SqlCommand comando = new SqlCommand(@"
                        SELECT
                            SUM(CASE WHEN Estado = 'Pendiente' THEN 1 ELSE 0 END),
                            SUM(CASE WHEN Estado IN ('En proceso', 'Recibido parcialmente') THEN 1 ELSE 0 END),
                            SUM(CASE WHEN Estado = 'Recibido' THEN 1 ELSE 0 END)
                        FROM dbo.PedidosTerceros", conexion))
                    {
                        conexion.Open();
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                string pendientes = Convert.ToString(lector.IsDBNull(0) ? 0 : lector.GetValue(0));
                                string parciales = Convert.ToString(lector.IsDBNull(1) ? 0 : lector.GetValue(1));
                                string recibidos = Convert.ToString(lector.IsDBNull(2) ? 0 : lector.GetValue(2));
                                lblPendientes.Text = pendientes;
                                lblEnProceso.Text = parciales;
                                lblRecibidos.Text = recibidos;
                                lblSeguimientoPendientes.Text = pendientes;
                                lblSeguimientoParciales.Text = parciales;
                                lblSeguimientoRecibidos.Text = recibidos;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pedidos externos: " + ex.Message);
            }
        }

        private void RegistrarPedidoTercero()
        {
            if (cboPedido.SelectedValue == null ||
                cboTrabajo.SelectedValue == null ||
                cboProveedor.SelectedValue == null)
            {
                MessageBox.Show(
                    "Seleccione el pedido, el producto y la empresa fabricante.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (dtpFechaEntrega.Value.Date < DateTime.Today)
            {
                MessageBox.Show("La fecha estimada no puede ser anterior a hoy.");
                return;
            }

            int idPedido = Convert.ToInt32(cboPedido.SelectedValue);
            int idDetalle = Convert.ToInt32(cboTrabajo.SelectedValue);
            int idProveedor = Convert.ToInt32(cboProveedor.SelectedValue);
            int cantidad = (int)nudCantidad.Value;

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    using (SqlCommand comando = new SqlCommand(@"
                INSERT INTO dbo.PedidosTerceros
                (
                    IdPedido,
                    IdDetalle,
                    IdProveedor,
                    CantidadSolicitada,
                    FechaSolicitud,
                    FechaEntregaEstimada,
                    Estado,
                    Observaciones
                )
                OUTPUT INSERTED.IdPedidoTercero
                VALUES
                (
                    @IdPedido,
                    @IdDetalle,
                    @IdProveedor,
                    @Cantidad,
                    GETDATE(),
                    @FechaEntrega,
                    'Pendiente',
                    @Observaciones
                )", conexion))
                    {
                        comando.Parameters.Add("@IdPedido", SqlDbType.Int)
                            .Value = idPedido;

                        comando.Parameters.Add("@IdDetalle", SqlDbType.Int)
                            .Value = idDetalle;

                        comando.Parameters.Add("@IdProveedor", SqlDbType.Int)
                            .Value = idProveedor;

                        comando.Parameters.Add("@Cantidad", SqlDbType.Int)
                            .Value = cantidad;

                        comando.Parameters.Add("@FechaEntrega", SqlDbType.Date)
                            .Value = dtpFechaEntrega.Value.Date;

                        comando.Parameters.Add("@Observaciones", SqlDbType.NVarChar, -1)
                            .Value = string.IsNullOrWhiteSpace(txtObservaciones.Text)
                                ? (object)DBNull.Value
                                : txtObservaciones.Text.Trim();

                        int nuevoId = Convert.ToInt32(comando.ExecuteScalar());

                        MessageBox.Show(
                            "Pedido al fabricante registrado correctamente.\n" +
                            "N.º de pedido externo: " + nuevoId,
                            "C.E SYSTEM",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }

                cboPedido.SelectedIndex = -1;
                cboTrabajo.DataSource = null;
                cboProveedor.SelectedIndex = -1;
                nudCantidad.Value = 1;
                dtpFechaEntrega.Value = DateTime.Today.AddDays(7);
                txtObservaciones.Clear();

                CargarPedidosTerceros();
                CargarHistorial();
                ActualizarIndicadoresRecepcion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar el pedido:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void CargarHistorial()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    SqlDataAdapter adaptador = new SqlDataAdapter(@"
                SELECT
                    pt.IdPedidoTercero,
                    'Pedido externo N.º ' +
                    CAST(pt.IdPedidoTercero AS VARCHAR(10)) +
                    ' - ' + d.Trabajo +
                    ' - ' + p.RazonSocial AS Descripcion
                FROM dbo.PedidosTerceros pt
                INNER JOIN dbo.DetallePedido d
                    ON pt.IdDetalle = d.IdDetalle
                INNER JOIN dbo.Proveedores p
                    ON pt.IdProveedor = p.IdProveedor
                WHERE pt.Estado NOT IN ('Recibido', 'Cancelado')
                ORDER BY pt.IdPedidoTercero DESC", conexion);

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    cboPedidoTercero.DataSource = tabla;
                    cboPedidoTercero.DisplayMember = "Descripcion";
                    cboPedidoTercero.ValueMember = "IdPedidoTercero";
                    cboPedidoTercero.SelectedIndex = -1;
                }

                CargarRecepciones();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar pedidos para recepción:\n" + ex.Message);
            }
        }

        private void CargarRecepciones()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    SqlDataAdapter adaptador = new SqlDataAdapter(@"
                SELECT
                    r.IdRecepcion AS [N.º Recepción],
                    r.IdPedidoTercero AS [Pedido externo],
                    d.Trabajo AS Producto,
                    p.RazonSocial AS Fabricante,
                    r.FechaRecepcion AS Fecha,
                    r.CantidadRecibida AS [Cantidad recibida],
                    r.Observaciones
                FROM dbo.RecepcionesTerceros r
                INNER JOIN dbo.PedidosTerceros pt
                    ON r.IdPedidoTercero = pt.IdPedidoTercero
                INNER JOIN dbo.DetallePedido d
                    ON pt.IdDetalle = d.IdDetalle
                INNER JOIN dbo.Proveedores p
                    ON pt.IdProveedor = p.IdProveedor
                ORDER BY r.IdRecepcion DESC", conexion);

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    dgvRecepciones.DataSource = tabla;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar recepciones:\n" + ex.Message);
            }
        }

        private void RegistrarRecepcion()
        {
            if (cboPedidoTercero.SelectedValue == null ||
                !(cboPedidoTercero.SelectedValue is int))
            {
                MessageBox.Show(
                    "Seleccione un pedido realizado al fabricante.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (dtpFechaRecepcion.Value.Date > DateTime.Today)
            {
                MessageBox.Show(
                    "No puede registrar una recepción con fecha futura.");
                return;
            }

            int idPedidoTercero =
                Convert.ToInt32(cboPedidoTercero.SelectedValue);

            int cantidadNueva = (int)nudCantidadRecibida.Value;

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    using (SqlTransaction transaccion = conexion.BeginTransaction())
                    {
                        try
                        {
                            int cantidadSolicitada;
                            int cantidadAnterior;
                            string estadoActual;

                            // Bloquear el pedido mientras se registra la recepción.
                            using (SqlCommand comando = new SqlCommand(@"
                        SELECT
                            CantidadSolicitada,
                            Estado
                        FROM dbo.PedidosTerceros WITH (UPDLOCK, HOLDLOCK)
                        WHERE IdPedidoTercero = @Id", conexion, transaccion))
                            {
                                comando.Parameters.Add("@Id", SqlDbType.Int)
                                    .Value = idPedidoTercero;

                                using (SqlDataReader lector = comando.ExecuteReader())
                                {
                                    if (!lector.Read())
                                        throw new Exception(
                                            "El pedido externo ya no existe.");

                                    cantidadSolicitada = lector.GetInt32(0);
                                    estadoActual = lector.GetString(1);
                                }
                            }

                            if (estadoActual == "Recibido" ||
                                estadoActual == "Cancelado")
                            {
                                throw new Exception(
                                    "Este pedido ya está recibido o cancelado.");
                            }

                            // Sumar todas las recepciones anteriores.
                            using (SqlCommand comando = new SqlCommand(@"
                        SELECT ISNULL(SUM(CantidadRecibida), 0)
                        FROM dbo.RecepcionesTerceros
                        WHERE IdPedidoTercero = @Id",
                                conexion, transaccion))
                            {
                                comando.Parameters.Add("@Id", SqlDbType.Int)
                                    .Value = idPedidoTercero;

                                cantidadAnterior =
                                    Convert.ToInt32(comando.ExecuteScalar());
                            }

                            int cantidadAcumulada =
                                cantidadAnterior + cantidadNueva;

                            if (cantidadAcumulada > cantidadSolicitada)
                            {
                                throw new Exception(
                                    "La cantidad recibida supera lo solicitado.\n\n" +
                                    "Cantidad solicitada: " + cantidadSolicitada +
                                    "\nRecibido anteriormente: " + cantidadAnterior +
                                    "\nCantidad pendiente: " +
                                    (cantidadSolicitada - cantidadAnterior));
                            }

                            // Registrar la entrega.
                            using (SqlCommand comando = new SqlCommand(@"
                        INSERT INTO dbo.RecepcionesTerceros
                        (
                            IdPedidoTercero,
                            FechaRecepcion,
                            CantidadRecibida,
                            Observaciones
                        )
                        VALUES
                        (
                            @Id,
                            @Fecha,
                            @Cantidad,
                            @Observaciones
                        )", conexion, transaccion))
                            {
                                comando.Parameters.Add("@Id", SqlDbType.Int)
                                    .Value = idPedidoTercero;

                                comando.Parameters.Add("@Fecha", SqlDbType.DateTime)
                                    .Value = dtpFechaRecepcion.Value.Date;

                                comando.Parameters.Add("@Cantidad", SqlDbType.Int)
                                    .Value = cantidadNueva;

                                comando.Parameters.Add(
                                    "@Observaciones", SqlDbType.NVarChar, -1)
                                    .Value =
                                    string.IsNullOrWhiteSpace(
                                        txtObservacionesRecepcion.Text)
                                    ? (object)DBNull.Value
                                    : txtObservacionesRecepcion.Text.Trim();

                                comando.ExecuteNonQuery();
                            }

                            // Actualizar el estado del pedido.
                            string nuevoEstado =
                                cantidadAcumulada == cantidadSolicitada
                                ? "Recibido"
                                : "Recibido parcialmente";

                            using (SqlCommand comando = new SqlCommand(@"
                        UPDATE dbo.PedidosTerceros
                        SET Estado = @Estado
                        WHERE IdPedidoTercero = @Id",
                                conexion, transaccion))
                            {
                                comando.Parameters.Add(
                                    "@Estado", SqlDbType.NVarChar, 30)
                                    .Value = nuevoEstado;

                                comando.Parameters.Add("@Id", SqlDbType.Int)
                                    .Value = idPedidoTercero;

                                comando.ExecuteNonQuery();
                            }

                            transaccion.Commit();

                            MessageBox.Show(
                                "Recepción registrada correctamente.\n\n" +
                                "Pedido externo: " + idPedidoTercero +
                                "\nRecibido en esta entrega: " + cantidadNueva +
                                "\nTotal recibido: " + cantidadAcumulada +
                                " de " + cantidadSolicitada +
                                "\nEstado: " + nuevoEstado,
                                "Recepción exitosa",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        catch
                        {
                            transaccion.Rollback();
                            throw;
                        }
                    }
                }

                // Limpiar y actualizar todas las pantallas.
                nudCantidadRecibida.Value = 1;
                dtpFechaRecepcion.Value = DateTime.Today;
                txtObservacionesRecepcion.Clear();

                CargarPedidosTerceros();
                CargarHistorial();
                ActualizarIndicadoresRecepcion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar la recepción:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void ColorearPedidos(DataGridView grilla)
        {
            foreach (DataGridViewRow fila in grilla.Rows)
            {
                if (fila.IsNewRow) continue;

                string estado = Convert.ToString(
                    fila.Cells["Estado"].Value);

                Color fondoFila = Color.White;
                Color textoFila = Color.Black;

                switch (estado)
                {
                    case "Pendiente":
                        fondoFila = Color.FromArgb(255, 215, 215);
                        textoFila = Color.FromArgb(150, 25, 25);
                        break;

                    case "En proceso":
                    case "Recibido parcialmente":
                        fondoFila = Color.FromArgb(255, 244, 190);
                        textoFila = Color.FromArgb(130, 90, 0);
                        break;

                    case "Recibido":
                        fondoFila = Color.FromArgb(205, 245, 215);
                        textoFila = Color.FromArgb(20, 110, 55);
                        break;

                    case "Cancelado":
                        fondoFila = Color.FromArgb(225, 225, 225);
                        textoFila = Color.Gray;
                        break;
                }

                fila.DefaultCellStyle.BackColor = fondoFila;
                fila.DefaultCellStyle.ForeColor = textoFila;

                // Mantener el mismo color al seleccionar la fila.
                fila.DefaultCellStyle.SelectionBackColor = fondoFila;
                fila.DefaultCellStyle.SelectionForeColor = textoFila;
            }
        }


        private void ActualizarIndicadoresRecepcion()
        {
            try
            {
                using (SqlConnection conexion =
                    new SqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    using (SqlCommand comando = new SqlCommand(@"
                SELECT
                    ISNULL((
                        SELECT SUM(CantidadSolicitada)
                        FROM dbo.PedidosTerceros
                        WHERE Estado <> 'Cancelado'
                    ), 0) AS Solicitadas,

                    ISNULL((
                        SELECT SUM(r.CantidadRecibida)
                        FROM dbo.RecepcionesTerceros r
                        INNER JOIN dbo.PedidosTerceros p
                            ON r.IdPedidoTercero = p.IdPedidoTercero
                        WHERE p.Estado <> 'Cancelado'
                    ), 0) AS Recibidas", conexion))
                    {
                        using (SqlDataReader lector =
                            comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                long solicitadas = Convert.ToInt64(
                                    lector["Solicitadas"]);

                                long recibidas = Convert.ToInt64(
                                    lector["Recibidas"]);

                                long pendientes = Math.Max(
                                    0, solicitadas - recibidas);

                                lblUnidadesSolicitadas.Text =
                                    solicitadas.ToString("N0");

                                lblUnidadesRecibidas.Text =
                                    recibidas.ToString("N0");

                                lblUnidadesPendientes.Text =
                                    pendientes.ToString("N0");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al actualizar indicadores de recepción:\n" +
                    ex.Message);
            }
        }
        private Label CrearIndicador(string titulo, string valor)
        {
            Panel tarjeta = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(6),
                Padding = new Padding(15)
            };

            Label etiqueta = new Label
            {
                Text = titulo,
                Dock = DockStyle.Top,
                Height = 30,
                ForeColor = Color.FromArgb(115, 124, 145),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Label numero = new Label
            {
                Text = valor,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(109, 90, 230),
                Font = new Font("Segoe UI", 25, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            tarjeta.Controls.Add(numero);
            tarjeta.Controls.Add(etiqueta);

            numero.Tag = tarjeta;

            return numero;
        }
    }
}
