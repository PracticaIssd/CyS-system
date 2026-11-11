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
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using iTextSharp.text.pdf;
using Document = iTextSharp.text.Document;
using PdfParagraph = iTextSharp.text.Paragraph;
using PdfPhrase = iTextSharp.text.Phrase;
using BaseColor = iTextSharp.text.BaseColor;
using FontFactory = iTextSharp.text.FontFactory;
using Element = iTextSharp.text.Element;
using PageSize = iTextSharp.text.PageSize;


namespace Login
{
    public partial class Form16 : Form
    {

        private readonly string cadenaConexion =
    @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";



        private readonly Color azulOscuro = Color.FromArgb(8, 43, 73);
        private readonly Color celeste = Color.FromArgb(174, 220, 241);
        private readonly Color fondo = Color.FromArgb(234, 245, 252);

        private DataGridView dgvCriterios;
        private DataGridView dgvDefectos;

        private Button btnAgregarDefecto;
        private Button btnQuitarDefecto;
        private Button btnGuardarControl;
        private Button btnLimpiar;

        private ComboBox cboCriterioDefecto;
        private ComboBox cboGravedad;
        private ComboBox cboTipoDefecto;
        private TextBox txtDescripcionDefecto;
        private NumericUpDown nudCantidadAfectada;

        private NumericUpDown nudCantidadRevisada;
        private NumericUpDown nudCantidadAprobada;
        private NumericUpDown nudCantidadDefectuosa;
        private ComboBox cboResultado;
        private TextBox txtObservaciones;

        private TabControl tabControlCalidad;
        private TabPage tabRegistrar;
        private TabPage tabConsultar;

        private DataGridView dgvHistorial;
        private DataGridView dgvConsultaCriterios;
        private DataGridView dgvConsultaDefectos;

        private TextBox txtFiltroPedido;
        private ComboBox cboFiltroResultado;
        private DateTimePicker dtpFiltroDesde;
        private DateTimePicker dtpFiltroHasta;

        private TabPage tabInforme;

        private ComboBox cboMesInforme;
        private NumericUpDown nudAnioInforme;

        private Label lblTotalRevisadas;
        private Label lblTotalAprobadas;
        private Label lblTotalDefectuosas;
        private Label lblPorcentajeDefectos;

        private DataGridView dgvInformeMensual;
        private Panel panelGraficosInforme;

        private Chart graficoDistribucion;
        private Chart graficoDefectos;

        public Form16()
        {

            InitializeComponent();
            ConstruirInterfaz();

            cboPedido.SelectionChangeCommitted +=
                cboPedido_SelectionChangeCommitted;

            CargarPedidos();
            CargarCriteriosDefectos();
        }


        private void ConstruirInterfaz()
        {
            Controls.Clear();

            Text = "C.E SYSTEM - Control de Calidad";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 700);
            BackColor = fondo;
            Font = new Font("Segoe UI", 10);

            // Crear las pestañas principales.
            tabControlCalidad = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Padding = new Point(18, 8)
            };

            tabRegistrar = new TabPage("Registrar control")
            {
                BackColor = fondo,
                Padding = new Padding(5)
            };

            tabConsultar = new TabPage("Consultar controles")
            {
                BackColor = fondo,
                Padding = new Padding(5)
            };

            tabInforme = new TabPage("Informe mensual")
            {
                BackColor = fondo,
                Padding = new Padding(5)
            };

            tabControlCalidad.TabPages.Add(tabRegistrar);
            tabControlCalidad.TabPages.Add(tabConsultar);
            tabControlCalidad.TabPages.Add(tabInforme);

            Controls.Add(tabControlCalidad);

            // Mantener el diseño actual de Registrar control.
            TableLayoutPanel principal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12),
                BackColor = fondo
            };

            principal.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 80));

            principal.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 155));

            principal.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            principal.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 190));

            tabRegistrar.Controls.Add(principal);

            CrearEncabezado(principal);
            CrearDatosTrabajo(principal);
            CrearInspeccion(principal);
            CrearResultado(principal);

            CrearPantallaConsultas();
            CrearPantallaInforme();
        }



        private void CrearPantallaConsultas()
        {
            TableLayoutPanel principal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12),
                BackColor = fondo
            };

            principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));
            principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 115));
            principal.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            principal.RowStyles.Add(new RowStyle(SizeType.Percent, 45));

            tabConsultar.Controls.Add(principal);

            Panel encabezado = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = azulOscuro
            };

            encabezado.Controls.Add(new Label
            {
                Text = "HISTORIAL DE CONTROLES DE CALIDAD",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            });

            principal.Controls.Add(encabezado, 0, 0);

            Panel filtros = CrearSeccion("FILTROS DE BÚSQUEDA");
            principal.Controls.Add(filtros, 0, 1);

            FlowLayoutPanel campos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 8, 0, 0),
                AutoScroll = true
            };

            txtFiltroPedido = new TextBox { Width = 105 };

            cboFiltroResultado = new ComboBox
            {
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboFiltroResultado.Items.AddRange(new object[]
            {
        "Todos", "Aprobado", "Con defectos"
            });
            cboFiltroResultado.SelectedIndex = 0;

            dtpFiltroDesde = new DateTimePicker
            {
                Width = 125,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-1)
            };

            dtpFiltroHasta = new DateTimePicker
            {
                Width = 125,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            campos.Controls.Add(CrearCampoConsulta("N.º Pedido", txtFiltroPedido));
            campos.Controls.Add(CrearCampoConsulta("Resultado", cboFiltroResultado));
            campos.Controls.Add(CrearCampoConsulta("Desde", dtpFiltroDesde));
            campos.Controls.Add(CrearCampoConsulta("Hasta", dtpFiltroHasta));

            Button btnBuscar = CrearBoton("Buscar");
            Button btnTodos = CrearBoton("Mostrar todos");
            Button btnLimpiarFiltros = CrearBoton("Limpiar filtros");

            campos.Controls.Add(btnBuscar);
            campos.Controls.Add(btnTodos);
            campos.Controls.Add(btnLimpiarFiltros);

            filtros.Controls.Add(campos);
            campos.BringToFront();

            Panel historial = CrearSeccion("INSPECCIONES REGISTRADAS");
            principal.Controls.Add(historial, 0, 2);

            dgvHistorial = CrearGrillaConsulta();
            historial.Controls.Add(dgvHistorial);
            dgvHistorial.BringToFront();

            TableLayoutPanel detalle = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            detalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            detalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            principal.Controls.Add(detalle, 0, 3);

            Panel criterios = CrearSeccion("CRITERIOS EVALUADOS");
            Panel defectos = CrearSeccion("DEFECTOS DETECTADOS");

            detalle.Controls.Add(criterios, 0, 0);
            detalle.Controls.Add(defectos, 1, 0);

            dgvConsultaCriterios = CrearGrillaConsulta();
            dgvConsultaDefectos = CrearGrillaConsulta();

            criterios.Controls.Add(dgvConsultaCriterios);
            defectos.Controls.Add(dgvConsultaDefectos);

            dgvConsultaCriterios.BringToFront();
            dgvConsultaDefectos.BringToFront();

            btnBuscar.Click += (s, e) => CargarHistorial(true);
            btnTodos.Click += (s, e) => CargarHistorial();

            btnLimpiarFiltros.Click += (s, e) =>
            {
                txtFiltroPedido.Clear();
                cboFiltroResultado.SelectedIndex = 0;
                dtpFiltroDesde.Value = DateTime.Today.AddMonths(-1);
                dtpFiltroHasta.Value = DateTime.Today;
                CargarHistorial();
            };

            dgvHistorial.SelectionChanged += (s, e) =>
            {
                if (dgvHistorial.CurrentRow == null ||
                    !dgvHistorial.Columns.Contains("IdControl"))
                    return;

                object valor = dgvHistorial.CurrentRow.Cells["IdControl"].Value;

                if (valor != null && valor != DBNull.Value)
                    CargarDetalleConsulta(Convert.ToInt32(valor));
            };

            tabControlCalidad.SelectedIndexChanged += (s, e) =>
            {
                if (tabControlCalidad.SelectedTab == tabConsultar)
                    CargarHistorial();
            };
        }

        private void CrearPantallaInforme()
        {
            TableLayoutPanel principal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(12),
                BackColor = fondo
            };

            principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));
            principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 125));
            principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 115));
            principal.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            principal.RowStyles.Add(new RowStyle(SizeType.Percent, 45));

            tabInforme.Controls.Add(principal);

            // Encabezado
            Panel encabezado = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = azulOscuro
            };

            encabezado.Controls.Add(new Label
            {
                Text = "INFORME MENSUAL DE CONTROL DE CALIDAD",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            });

            principal.Controls.Add(encabezado, 0, 0);

            // Filtros
            Panel filtros = CrearSeccion("SELECCIONAR PERÍODO");
            principal.Controls.Add(filtros, 0, 1);

            FlowLayoutPanel campos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 12, 0, 0),
                AutoScroll = true,
                WrapContents = false
            };

            cboMesInforme = new ComboBox
            {
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboMesInforme.Items.AddRange(new object[]
            {
        "Enero", "Febrero", "Marzo", "Abril",
        "Mayo", "Junio", "Julio", "Agosto",
        "Septiembre", "Octubre", "Noviembre", "Diciembre"
            });

            cboMesInforme.SelectedIndex = DateTime.Today.Month - 1;

            nudAnioInforme = new NumericUpDown
            {
                Width = 100,
                Minimum = 2020,
                Maximum = 2100,
                Value = DateTime.Today.Year
            };

            Button btnGenerar = CrearBoton("Generar informe");
            Button btnExportar = CrearBoton("Exportar PDF");

            campos.Controls.Add(
                CrearCampoConsulta("Mes", cboMesInforme));

            campos.Controls.Add(
                CrearCampoConsulta("Año", nudAnioInforme));

            campos.Controls.Add(btnGenerar);
            campos.Controls.Add(btnExportar);

            filtros.Controls.Add(campos);
            campos.BringToFront();

            // Indicadores
            TableLayoutPanel indicadores = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1
            };

            for (int i = 0; i < 4; i++)
                indicadores.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 25));

            lblTotalRevisadas = CrearIndicadorInforme(
                indicadores, "UNIDADES REVISADAS", 0);

            lblTotalAprobadas = CrearIndicadorInforme(
                indicadores, "UNIDADES APROBADAS", 1);

            lblTotalDefectuosas = CrearIndicadorInforme(
                indicadores, "UNIDADES DEFECTUOSAS", 2);

            lblPorcentajeDefectos = CrearIndicadorInforme(
                indicadores, "PORCENTAJE DEFECTUOSO", 3);

            principal.Controls.Add(indicadores, 0, 2);

            // Espacio para los gráficos
            Panel seccionGraficos = CrearSeccion(
                "ANÁLISIS GRÁFICO DE CALIDAD");

            principal.Controls.Add(seccionGraficos, 0, 3);

            panelGraficosInforme = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12)
            };

            seccionGraficos.Controls.Add(panelGraficosInforme);
            panelGraficosInforme.BringToFront();

            CrearGraficosInforme();
            // Detalle por inspección
            Panel detalle = CrearSeccion(
                "DETALLE DE INSPECCIONES DEL MES");

            principal.Controls.Add(detalle, 0, 4);

            dgvInformeMensual = CrearGrillaConsulta();

            detalle.Controls.Add(dgvInformeMensual);
            dgvInformeMensual.BringToFront();

            // Por ahora solo conectamos Generar informe.
            btnGenerar.Click += (s, e) => GenerarInformeMensual();

            btnExportar.Click += (s, e) => ExportarInformePDF();
        }

        private Label CrearIndicadorInforme(
            TableLayoutPanel contenedor,
            string titulo,
            int columna)
        {
            Panel tarjeta = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(5)
            };

            Label etiqueta = new Label
            {
                Text = titulo,
                Dock = DockStyle.Top,
                Height = 35,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = azulOscuro,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label valor = new Label
            {
                Text = "0",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                TextAlign = ContentAlignment.MiddleCenter
            };

            tarjeta.Controls.Add(valor);
            tarjeta.Controls.Add(etiqueta);

            contenedor.Controls.Add(tarjeta, columna, 0);

            return valor;
        }

        private Control CrearCampoConsulta(string texto, Control control)
        {
            Panel panel = new Panel
            {
                Width = control.Width + 15,
                Height = 65,
                Margin = new Padding(5)
            };

            Label etiqueta = CrearEtiqueta(texto);
            etiqueta.Location = new Point(0, 0);

            control.Location = new Point(0, 28);

            panel.Controls.Add(etiqueta);
            panel.Controls.Add(control);

            return panel;
        }

        private DataGridView CrearGrillaConsulta()
        {
            return new DataGridView
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
                ColumnHeadersHeight = 38,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = azulOscuro,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                }
            };
        }

        private void CargarHistorial(bool filtrar = false)
        {
            try
            {
                string sql = @"
            SELECT
                c.IdControl,
                c.IdPedido AS Pedido,
                d.Trabajo,
                c.FechaControl AS Fecha,
                c.Inspector,
                c.CantidadRevisada AS Revisadas,
                c.CantidadAprobada AS Aprobadas,
                c.CantidadDefectuosa AS Defectuosas,
                c.Resultado,
                c.Observaciones
            FROM dbo.ControlCalidadPropios c
            INNER JOIN dbo.DetallePedido d
                ON c.IdDetalle = d.IdDetalle
            WHERE 1 = 1";

                if (filtrar)
                {
                    sql += @"
                AND c.FechaControl >= @Desde
                AND c.FechaControl < @Hasta";

                    if (!string.IsNullOrWhiteSpace(txtFiltroPedido.Text))
                        sql += " AND c.IdPedido = @Pedido";

                    if (cboFiltroResultado.Text != "Todos")
                        sql += " AND c.Resultado = @Resultado";
                }

                sql += " ORDER BY c.FechaControl DESC, c.IdControl DESC";

                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                using (SqlCommand cmd = new SqlCommand(sql, conexion))
                {
                    if (filtrar)
                    {
                        if (dtpFiltroDesde.Value.Date > dtpFiltroHasta.Value.Date)
                        {
                            MessageBox.Show("La fecha desde no puede superar la fecha hasta.");
                            return;
                        }

                        cmd.Parameters.Add("@Desde", SqlDbType.DateTime)
                            .Value = dtpFiltroDesde.Value.Date;

                        cmd.Parameters.Add("@Hasta", SqlDbType.DateTime)
                            .Value = dtpFiltroHasta.Value.Date.AddDays(1);

                        if (!string.IsNullOrWhiteSpace(txtFiltroPedido.Text))
                        {
                            int pedido;

                            if (!int.TryParse(txtFiltroPedido.Text.Trim(), out pedido)
                                || pedido <= 0)
                            {
                                MessageBox.Show("Ingrese un número de pedido válido.");
                                return;
                            }

                            cmd.Parameters.Add("@Pedido", SqlDbType.Int).Value = pedido;
                        }

                        if (cboFiltroResultado.Text != "Todos")
                            cmd.Parameters.Add("@Resultado", SqlDbType.NVarChar)
                                .Value = cboFiltroResultado.Text;
                    }

                    DataTable tabla = new DataTable();

                    using (SqlDataAdapter adaptador = new SqlDataAdapter(cmd))
                        adaptador.Fill(tabla);

                    dgvConsultaCriterios.DataSource = null;
                    dgvConsultaDefectos.DataSource = null;
                    dgvHistorial.DataSource = tabla;

                    if (dgvHistorial.Columns.Contains("IdControl"))
                        dgvHistorial.Columns["IdControl"].HeaderText = "N.º Control";

                    dgvHistorial.ClearSelection();

                    if (dgvHistorial.Rows.Count > 0)
                    {
                        dgvHistorial.Rows[0].Selected = true;
                        dgvHistorial.CurrentCell = dgvHistorial.Rows[0].Cells[0];

                        int id = Convert.ToInt32(
                            dgvHistorial.Rows[0].Cells["IdControl"].Value);

                        CargarDetalleConsulta(id);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar controles:\n" + ex.Message);
            }
        }

        private void CargarDetalleConsulta(int idControl)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string sqlCriterios = @"
                SELECT
                    c.Nombre AS Criterio,
                    d.Evaluacion
                FROM dbo.DetalleControlCalidad d
                INNER JOIN dbo.CriteriosCalidad c
                    ON d.IdCriterio = c.IdCriterio
                WHERE d.IdControl = @IdControl
                ORDER BY c.IdCriterio";

                    using (SqlCommand cmd = new SqlCommand(sqlCriterios, conexion))
                    {
                        cmd.Parameters.Add("@IdControl", SqlDbType.Int).Value = idControl;

                        DataTable tabla = new DataTable();

                        using (SqlDataAdapter adaptador = new SqlDataAdapter(cmd))
                            adaptador.Fill(tabla);

                        dgvConsultaCriterios.DataSource = tabla;
                    }

                    string sqlDefectos = @"
                SELECT
                    c.Nombre AS Criterio,
                    n.TipoDefecto AS Defecto,
                    n.Descripcion,
                    n.CantidadAfectada AS Cantidad,
                    n.Gravedad,
                    n.Estado
                FROM dbo.NoConformidades n
                LEFT JOIN dbo.CriteriosCalidad c
                    ON n.IdCriterio = c.IdCriterio
                WHERE n.IdControl = @IdControl
                ORDER BY n.IdNoConformidad";

                    using (SqlCommand cmd = new SqlCommand(sqlDefectos, conexion))
                    {
                        cmd.Parameters.Add("@IdControl", SqlDbType.Int).Value = idControl;

                        DataTable tabla = new DataTable();

                        using (SqlDataAdapter adaptador = new SqlDataAdapter(cmd))
                            adaptador.Fill(tabla);

                        dgvConsultaDefectos.DataSource = tabla;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el detalle:\n" + ex.Message);
            }
        }


        private Label CrearEtiqueta(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = azulOscuro,
                Margin = new Padding(5)
            };
        }

        private Panel CrearSeccion(string titulo)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(4)
            };

            Label encabezado = new Label
            {
                Text = "  " + titulo,
                Dock = DockStyle.Top,
                Height = 38,
                BackColor = celeste,
                ForeColor = azulOscuro,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            panel.Controls.Add(encabezado);
            RedondearControl(panel, 18);
            return panel;
        }

        private void CrearEncabezado(TableLayoutPanel principal)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = azulOscuro,
                Margin = new Padding(4)
            };

            Label titulo = new Label
            {
                Text = "GESTIÓN DE CONTROL DE CALIDAD",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 18)
            };

            panel.Controls.Add(titulo);
            principal.Controls.Add(panel, 0, 0);
        }

        private void CrearDatosTrabajo(TableLayoutPanel principal)
        {
            Panel seccion = CrearSeccion("DATOS DEL TRABAJO");
            principal.Controls.Add(seccion, 0, 1);

            TableLayoutPanel tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 2,
                Padding = new Padding(15, 10, 15, 10)
            };

            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 18));
            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 27));
            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 20));
            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 18));
            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 17));

            tabla.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 30));
            tabla.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            string[] etiquetas =
            {
                "Pedido N.º",
                "Trabajo",
                "Inspector",
                "Fecha del control",
                "Estado del pedido"
            };

            for (int i = 0; i < etiquetas.Length; i++)
                tabla.Controls.Add(CrearEtiqueta(etiquetas[i]), i, 0);

            cboPedido = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboTrabajo = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            txtInspector = new TextBox
            {
                Dock = DockStyle.Top
            };

            dtpFechaControl = new DateTimePicker
            {
                Dock = DockStyle.Top,
                Format = DateTimePickerFormat.Short
            };

            lblEstadoPedido = new Label
            {
                Text = "Sin seleccionar",
                Dock = DockStyle.Top,
                Height = 30,
                ForeColor = azulOscuro,
                TextAlign = ContentAlignment.MiddleLeft
            };

            tabla.Controls.Add(cboPedido, 0, 1);
            tabla.Controls.Add(cboTrabajo, 1, 1);
            tabla.Controls.Add(txtInspector, 2, 1);
            tabla.Controls.Add(dtpFechaControl, 3, 1);
            tabla.Controls.Add(lblEstadoPedido, 4, 1);

            // Primero se agrega la tabla y después el encabezado,
            // para conservar la franja superior de la sección.
            tabla.Dock = DockStyle.Fill;

            seccion.Controls.Add(tabla);
            tabla.BringToFront();

            tabla.RowStyles[0] = new RowStyle(SizeType.Absolute, 35);
            tabla.RowStyles[1] = new RowStyle(SizeType.Percent, 100);
        }

        private void CrearInspeccion(TableLayoutPanel principal)
        {
            TableLayoutPanel contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            contenido.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 58));
            contenido.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 42));

            principal.Controls.Add(contenido, 0, 2);

            Panel inspeccion =
                CrearSeccion("INSPECCIÓN TÉCNICA - LISTA DE VERIFICACIÓN");

            Panel defectos =
                CrearSeccion("NO CONFORMIDADES");

            contenido.Controls.Add(inspeccion, 0, 0);
            contenido.Controls.Add(defectos, 1, 0);

            CrearTablaCriterios(inspeccion);
            CrearPanelDefectos(defectos);
        }

        private void CrearTablaCriterios(Panel panel)
        {
            dgvCriterios = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvCriterios.Columns.Add(
                "Criterio", "Criterio de calidad");
            dgvCriterios.Columns.Add(
                "Descripcion", "Descripción");

            DataGridViewComboBoxColumn evaluacion =
                new DataGridViewComboBoxColumn
                {
                    Name = "Evaluacion",
                    HeaderText = "Evaluación"
                };

            evaluacion.Items.AddRange(
                "Conforme", "No conforme", "No aplica");

            dgvCriterios.Columns.Add(evaluacion);
            

            dgvCriterios.Columns["Criterio"].ReadOnly = true;
            dgvCriterios.Columns["Descripcion"].ReadOnly = true;

            dgvCriterios.ColumnHeadersDefaultCellStyle.BackColor =
                azulOscuro;
            dgvCriterios.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;
            dgvCriterios.EnableHeadersVisualStyles = false;
            dgvCriterios.ColumnHeadersHeight = 38;

            string[,] criterios =
            {
                { "Calidad de impresión", "Nitidez y manchas" },
                { "Fidelidad del color", "Colores solicitados" },
                { "Alineación", "Posición de elementos" },
                { "Precisión del corte", "Medidas y cortes" },
                { "Calidad del papel", "Arrugas y roturas" },
                { "Terminaciones", "Acabados del producto" },
                { "Cantidad producida", "Cantidad solicitada" },
                { "Presentación final", "Limpieza y embalaje" }
            };

            for (int i = 0; i < criterios.GetLength(0); i++)
            {
                dgvCriterios.Rows.Add(
                criterios[i, 0],
                criterios[i, 1],
                "Conforme");
            }

            panel.Controls.Add(dgvCriterios);
            dgvCriterios.BringToFront();


            dgvCriterios.AllowUserToResizeRows = false;
            dgvCriterios.AllowUserToResizeColumns = false;

            dgvCriterios.RowTemplate.Height = 38;

            // Corregir también las filas que ya existen
            foreach (DataGridViewRow fila in dgvCriterios.Rows)
            {
                fila.Height = 38;
            }

            dgvCriterios.ColumnHeadersHeight = 42;
            dgvCriterios.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvCriterios.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvCriterios.MultiSelect = false;

            dgvCriterios.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.None;

            dgvCriterios.DefaultCellStyle.Font =
                new Font("Segoe UI", 10);

            dgvCriterios.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(207, 234, 250);

            dgvCriterios.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(8, 43, 73);

            dgvCriterios.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(242, 249, 253);

            dgvCriterios.GridColor =
                Color.FromArgb(215, 230, 240);

            dgvCriterios.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;


            dgvCriterios.CurrentCellDirtyStateChanged +=
                dgvCriterios_CurrentCellDirtyStateChanged;

            dgvCriterios.CellValueChanged +=
                dgvCriterios_CellValueChanged;

            ColorearEvaluaciones();


        }

        private void CrearPanelDefectos(Panel panel)
        {
            TableLayoutPanel tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(10)
            };

            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 60));
            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 40));

            for (int i = 0; i < 8; i++)
                tabla.RowStyles.Add(
                    new RowStyle(SizeType.AutoSize));

            cboCriterioDefecto = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboGravedad = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboGravedad.Items.AddRange(
                new object[] { "Leve", "Moderada", "Critica" });

            cboTipoDefecto = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboCriterioDefecto.SelectedIndexChanged +=
               (sender, e) => CargarTiposDefecto();

            txtDescripcionDefecto = new TextBox
            {
                Dock = DockStyle.Top,
                Multiline = true,
                Height = 65
            };

            nudCantidadAfectada = new NumericUpDown
            {
                Dock = DockStyle.Top,
                Maximum = 1000000,
                Minimum = 1
            };

            tabla.Controls.Add(CrearEtiqueta("Criterio"), 0, 0);
            tabla.Controls.Add(CrearEtiqueta("Gravedad"), 1, 0);
            tabla.Controls.Add(cboCriterioDefecto, 0, 1);
            tabla.Controls.Add(cboGravedad, 1, 1);

            tabla.Controls.Add(CrearEtiqueta("Tipo de defecto"), 0, 2);
            tabla.Controls.Add(CrearEtiqueta("Cantidad afectada"), 1, 2);
            tabla.Controls.Add(cboTipoDefecto, 0, 3); tabla.Controls.Add(nudCantidadAfectada, 1, 3);

            tabla.Controls.Add(
                CrearEtiqueta("Detalle del defecto"), 0, 4);
            tabla.Controls.Add(txtDescripcionDefecto, 0, 5);
            tabla.SetColumnSpan(txtDescripcionDefecto, 2);

            btnAgregarDefecto = CrearBoton("Agregar defecto");
            btnQuitarDefecto = CrearBoton("Quitar");

            btnAgregarDefecto.Click += btnAgregarDefecto_Click;
            btnQuitarDefecto.Click += btnQuitarDefecto_Click;

            FlowLayoutPanel botones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true
            };

            botones.Controls.Add(btnAgregarDefecto);
            botones.Controls.Add(btnQuitarDefecto);

            tabla.Controls.Add(botones, 0, 6);
            tabla.SetColumnSpan(botones, 2);

            dgvDefectos = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvDefectos.Columns.Add("Criterio", "Criterio");
            dgvDefectos.Columns.Add("Defecto", "Defecto");
            dgvDefectos.Columns.Add("Descripcion", "Descripción");
            dgvDefectos.Columns.Add("Cantidad", "Cantidad");
            dgvDefectos.Columns.Add("Gravedad", "Gravedad");

            dgvDefectos.ReadOnly = true;
            dgvDefectos.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvDefectos.MultiSelect = false;

            tabla.Controls.Add(dgvDefectos, 0, 7);
            tabla.SetColumnSpan(dgvDefectos, 2);

            tabla.RowStyles[7] =
                new RowStyle(SizeType.Percent, 100);

            panel.Controls.Add(tabla);
            tabla.BringToFront();
        }

        private void CargarCriteriosDefectos()
        {
            cboCriterioDefecto.Items.Clear();

            foreach (DataGridViewRow fila in dgvCriterios.Rows)
            {
                if (fila.IsNewRow)
                    continue;

                string criterio = Convert.ToString(
                    fila.Cells["Criterio"].Value);

                if (!string.IsNullOrWhiteSpace(criterio))
                    cboCriterioDefecto.Items.Add(criterio);
            }

            cboCriterioDefecto.SelectedIndex = -1;
        }



        private Button CrearBoton(string texto)
        {
            Button boton = new Button
            {
                Text = texto,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                AutoSize = false,
                Size = new Size(150, 40),
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };

            boton.FlatAppearance.BorderSize = 0;

            RedondearControl(boton, 14);

            return boton;
        }

        private void btnAgregarDefecto_Click(
            object sender, EventArgs e)
        {
            if (cboCriterioDefecto.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione un criterio de calidad.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (cboTipoDefecto.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Ingrese el tipo de defecto.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescripcionDefecto.Text))
            {
                MessageBox.Show(
                    "Ingrese una descripción del defecto.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (cboGravedad.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione la gravedad del defecto.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string criterio = cboCriterioDefecto.Text;

            bool esNoConforme = false;

            foreach (DataGridViewRow fila in dgvCriterios.Rows)
            {
                if (Convert.ToString(
                        fila.Cells["Criterio"].Value) == criterio &&
                    Convert.ToString(
                        fila.Cells["Evaluacion"].Value) == "No conforme")
                {
                    esNoConforme = true;
                    break;
                }
            }

            if (!esNoConforme)
            {
                MessageBox.Show(
                    "Primero debe marcar el criterio como No conforme.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (nudCantidadRevisada.Value > 0 &&
                nudCantidadAfectada.Value > nudCantidadRevisada.Value)
            {
                MessageBox.Show(
                    "La cantidad afectada no puede superar " +
                    "la cantidad revisada.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            if (cboTipoDefecto.Text == "Otro" &&
                string.IsNullOrWhiteSpace(txtDescripcionDefecto.Text))
            {
                MessageBox.Show(
                    "Debe describir el defecto cuando selecciona Otro.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            int indice = dgvDefectos.Rows.Add(
                criterio,
                cboTipoDefecto.Text,
                txtDescripcionDefecto.Text.Trim(),
                nudCantidadAfectada.Value,
                cboGravedad.Text);

            if (cboGravedad.Text == "Critica")
            {
                dgvDefectos.Rows[indice].DefaultCellStyle.BackColor =
                    Color.FromArgb(255, 220, 225);
            }
            else if (cboGravedad.Text == "Moderada")
            {
                dgvDefectos.Rows[indice].DefaultCellStyle.BackColor =
                    Color.FromArgb(255, 244, 211);
            }

            cboTipoDefecto.SelectedIndex = -1;
            txtDescripcionDefecto.Clear();

            nudCantidadAfectada.Value = 1;
            cboGravedad.SelectedIndex = -1;
            cboCriterioDefecto.SelectedIndex = -1;
        }


        private void btnQuitarDefecto_Click(
            object sender, EventArgs e)
        {
            if (dgvDefectos.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un defecto para quitar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            dgvDefectos.Rows.RemoveAt(
                dgvDefectos.SelectedRows[0].Index);
        }


        private void RedondearControl(Control control, int radio)
        {
            control.Resize += (sender, e) =>
            {
                if (control.Width <= 0 || control.Height <= 0)
                    return;

                int r = Math.Min(radio, Math.Min(control.Width, control.Height));

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(0, 0, r, r, 180, 90);
                    path.AddArc(control.Width - r, 0, r, r, 270, 90);
                    path.AddArc(control.Width - r,
                                control.Height - r, r, r, 0, 90);
                    path.AddArc(0, control.Height - r, r, r, 90, 90);
                    path.CloseFigure();

                    Region regionAnterior = control.Region;
                    control.Region = new Region(path);
                    if (regionAnterior != null)
                        regionAnterior.Dispose();
                }
            };

            // Aplicar también al crear el control.
            control.PerformLayout();

            // Se actualizará cuando cambie de tamaño.
            Size tamaño = control.Size;
            control.Size = new Size(tamaño.Width + 1, tamaño.Height);
            control.Size = tamaño;
        }


        private void CrearResultado(TableLayoutPanel principal)
        {
            Panel seccion =
                CrearSeccion("RESULTADO DE LA INSPECCIÓN");

            principal.Controls.Add(seccion, 0, 3);

            TableLayoutPanel tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(15)
            };

            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 23));
            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 17));
            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 38));
            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 22));

            TableLayoutPanel cantidades = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(5)
            };

            cantidades.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 65));

            cantidades.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 35));

            for (int i = 0; i < 3; i++)
            {
                cantidades.RowStyles.Add(
                    new RowStyle(SizeType.Percent, 33.33F));
            }

            nudCantidadRevisada = CrearNumero();
            nudCantidadAprobada = CrearNumero();
            nudCantidadDefectuosa = CrearNumero();

            cantidades.Controls.Add(CrearEtiqueta("Revisadas"), 0, 0);
            cantidades.Controls.Add(nudCantidadRevisada, 1, 0);

            cantidades.Controls.Add(CrearEtiqueta("Aprobadas"), 0, 1);
            cantidades.Controls.Add(nudCantidadAprobada, 1, 1);

            cantidades.Controls.Add(CrearEtiqueta("Defectuosas"), 0, 2);
            cantidades.Controls.Add(nudCantidadDefectuosa, 1, 2);

            FlowLayoutPanel resultado = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
            };

            cboResultado = new ComboBox
            {
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboResultado.Items.AddRange(
            new object[] { "Aprobado", "Con defectos" });

            resultado.Controls.Add(CrearEtiqueta("Resultado final"));
            resultado.Controls.Add(cboResultado);

            FlowLayoutPanel observaciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
            };

            txtObservaciones = new TextBox
            {
                Multiline = true,
                Width = 300,
                Height = 85
            };

            observaciones.Controls.Add(
                CrearEtiqueta("Observaciones generales"));
            observaciones.Controls.Add(txtObservaciones);

            FlowLayoutPanel botones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill
            };

            btnGuardarControl = CrearBoton("Guardar control");
            btnGuardarControl.Click += btnGuardarControl_Click;
            botones.Controls.Add(btnGuardarControl);
            btnLimpiar = CrearBoton("Limpiar");
            btnLimpiar.Click += (sender, e) => LimpiarFormulario();
            botones.Controls.Add(btnLimpiar);

            tabla.Controls.Add(cantidades, 0, 0);
            tabla.Controls.Add(resultado, 1, 0);
            tabla.Controls.Add(observaciones, 2, 0);
            tabla.Controls.Add(botones, 3, 0);

            seccion.Controls.Add(tabla);
            tabla.BringToFront();
            nudCantidadDefectuosa.ValueChanged +=
             (sender, e) => ActualizarResultado();

            ActualizarResultado();
        }

        private bool ValidarControl()
        {
            if (cboPedido.SelectedIndex == -1 ||
                cboTrabajo.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un pedido y un trabajo.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtInspector.Text))
            {
                MessageBox.Show("Ingrese el nombre del inspector.");
                return false;
            }

            int revisadas = (int)nudCantidadRevisada.Value;
            int aprobadas = (int)nudCantidadAprobada.Value;
            int defectuosas = (int)nudCantidadDefectuosa.Value;

            if (revisadas <= 0 || aprobadas + defectuosas != revisadas)
            {
                MessageBox.Show(
                    "La cantidad revisada debe ser mayor a cero y " +
                    "coincidir con la suma de aprobadas y defectuosas.");
                return false;
            }

            if (cboResultado.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione el resultado final.");
                return false;
            }

            bool hayNoConformes = false;

            foreach (DataGridViewRow fila in dgvCriterios.Rows)
            {
                if (fila.IsNewRow) continue;

                string criterio = Convert.ToString(
                    fila.Cells["Criterio"].Value);

                string evaluacion = Convert.ToString(
                    fila.Cells["Evaluacion"].Value);

                if (evaluacion == "No conforme")
                {
                    hayNoConformes = true;
                    bool tieneDefecto = false;

                    foreach (DataGridViewRow defecto in dgvDefectos.Rows)
                    {
                        if (!defecto.IsNewRow &&
                            Convert.ToString(defecto.Cells["Criterio"].Value)
                            == criterio)
                        {
                            tieneDefecto = true;
                            break;
                        }
                    }

                    if (!tieneDefecto)
                    {
                        MessageBox.Show(
                            "Debe registrar un defecto para: " + criterio);
                        return false;
                    }

                }
               
            }

            if (hayNoConformes && defectuosas == 0)
            {
                MessageBox.Show(
                    "Existen criterios no conformes, pero no se " +
                    "registraron unidades defectuosas.");
                return false;
            }

            if (!hayNoConformes && defectuosas > 0)
            {
                MessageBox.Show(
                    "Hay unidades defectuosas, pero ningún criterio " +
                    "está marcado como No conforme.");
                return false;
            }

            if (defectuosas > 0 && cboResultado.Text != "Con defectos")
            {
                MessageBox.Show(
                    "Si existen unidades defectuosas, el resultado debe ser Con defectos.");
                return false;
            }

            if (defectuosas == 0 && cboResultado.Text != "Aprobado")
            {
                MessageBox.Show(
                    "Si no existen unidades defectuosas, el resultado debe ser Aprobado.");
                return false;
            }

            foreach (DataGridViewRow defecto in dgvDefectos.Rows)
            {
                if (defecto.IsNewRow) continue;

                int cantidad = Convert.ToInt32(
                    defecto.Cells["Cantidad"].Value);

                if (cantidad > defectuosas)
                {
                    MessageBox.Show(
                        "La cantidad afectada de cada defecto no puede " +
                        "superar las unidades defectuosas.");
                    return false;
                }
            }

            return true;
        }

        private void btnGuardarControl_Click(object sender, EventArgs e)
        {
            dgvCriterios.EndEdit();
            dgvDefectos.EndEdit();

            if (!ValidarControl())
                return;

            int idPedido = Convert.ToInt32(cboPedido.SelectedValue);
            int idDetalle = Convert.ToInt32(cboTrabajo.SelectedValue);

            try
            {
                using (SqlConnection conexion =
                    new SqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    using (SqlTransaction transaccion =
                        conexion.BeginTransaction())
                    {
                        try
                        {
                            // 1. Registrar la inspección principal.
                            string sqlControl = @"
                        INSERT INTO dbo.ControlCalidadPropios
                        (
                            IdPedido,
                            IdDetalle,
                            FechaControl,
                            CantidadRevisada,
                            CantidadAprobada,
                            CantidadDefectuosa,
                            Observaciones,
                            Resultado,
                            Inspector,
                            EstadoInspeccion,
                            TipoInspeccion,
                            IdControlAnterior
                        )
                        OUTPUT INSERTED.IdControl
                        VALUES
                        (
                            @IdPedido,
                            @IdDetalle,
                            @FechaControl,
                            @Revisada,
                            @Aprobada,
                            @Defectuosa,
                            @Observaciones,
                            @Resultado,
                            @Inspector,
                            @EstadoInspeccion,
                            @TipoInspeccion,
                            NULL
                        )";

                            int idControl;

                            using (SqlCommand cmd =
                                new SqlCommand(sqlControl, conexion, transaccion))
                            {
                                cmd.Parameters.Add("@IdPedido", SqlDbType.Int)
                                    .Value = idPedido;
                                cmd.Parameters.Add("@IdDetalle", SqlDbType.Int)
                                    .Value = idDetalle;
                                cmd.Parameters.Add("@FechaControl", SqlDbType.DateTime)
                                    .Value = dtpFechaControl.Value;
                                cmd.Parameters.Add("@Revisada", SqlDbType.Int)
                                    .Value = (int)nudCantidadRevisada.Value;
                                cmd.Parameters.Add("@Aprobada", SqlDbType.Int)
                                    .Value = (int)nudCantidadAprobada.Value;
                                cmd.Parameters.Add("@Defectuosa", SqlDbType.Int)
                                    .Value = (int)nudCantidadDefectuosa.Value;
                                cmd.Parameters.Add("@Observaciones", SqlDbType.NVarChar)
                                    .Value = txtObservaciones.Text.Trim();
                                cmd.Parameters.Add("@Resultado", SqlDbType.NVarChar)
                                    .Value = cboResultado.Text;
                                cmd.Parameters.Add("@Inspector", SqlDbType.NVarChar)
                                    .Value = txtInspector.Text.Trim();
                                cmd.Parameters.Add("@EstadoInspeccion", SqlDbType.NVarChar)
                                    .Value = "Finalizada";
                                cmd.Parameters.Add("@TipoInspeccion", SqlDbType.NVarChar)
                                    .Value = "Inicial";

                                idControl = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // 2. Guardar las evaluaciones de cada criterio.
                            foreach (DataGridViewRow fila in dgvCriterios.Rows)
                            {
                                if (fila.IsNewRow) continue;

                                string nombreCriterio = Convert.ToString(
                                    fila.Cells["Criterio"].Value);

                                int idCriterio = ObtenerIdCriterio(
                                    nombreCriterio, conexion, transaccion);

                                string sqlDetalle = @"
                            INSERT INTO dbo.DetalleControlCalidad
                            (IdControl, IdCriterio, Evaluacion)
                            VALUES (@IdControl, @IdCriterio, @Evaluacion)";

                                using (SqlCommand cmd =
                                    new SqlCommand(sqlDetalle, conexion, transaccion))
                                {
                                    cmd.Parameters.Add("@IdControl", SqlDbType.Int)
                                        .Value = idControl;
                                    cmd.Parameters.Add("@IdCriterio", SqlDbType.Int)
                                        .Value = idCriterio;
                                    cmd.Parameters.Add("@Evaluacion", SqlDbType.NVarChar)
                                        .Value = Convert.ToString(
                                            fila.Cells["Evaluacion"].Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // 3. Guardar los defectos detectados.
                            foreach (DataGridViewRow fila in dgvDefectos.Rows)
                            {
                                if (fila.IsNewRow) continue;

                                int idCriterio = ObtenerIdCriterio(
                                    Convert.ToString(fila.Cells["Criterio"].Value),
                                    conexion, transaccion);

                                string sqlDefecto = @"
                            INSERT INTO dbo.NoConformidades
                            (
                                IdControl,
                                IdCriterio,
                                TipoDefecto,
                                Descripcion,
                                CantidadAfectada,
                                Gravedad,
                                Estado,
                                FechaRegistro
                            )
                            VALUES
                            (
                                @IdControl,
                                @IdCriterio,
                                @TipoDefecto,
                                @Descripcion,
                                @Cantidad,
                                @Gravedad,
                                @Estado,
                                GETDATE()
                            )";

                                using (SqlCommand cmd =
                                    new SqlCommand(sqlDefecto, conexion, transaccion))
                                {
                                    cmd.Parameters.Add("@IdControl", SqlDbType.Int)
                                        .Value = idControl;
                                    cmd.Parameters.Add("@IdCriterio", SqlDbType.Int)
                                        .Value = idCriterio;
                                    cmd.Parameters.Add("@TipoDefecto", SqlDbType.NVarChar)
                                        .Value = Convert.ToString(
                                            fila.Cells["Defecto"].Value);
                                    cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar)
                                        .Value = Convert.ToString(
                                            fila.Cells["Descripcion"].Value);
                                    cmd.Parameters.Add("@Cantidad", SqlDbType.Int)
                                        .Value = Convert.ToInt32(
                                            fila.Cells["Cantidad"].Value);
                                    cmd.Parameters.Add("@Gravedad", SqlDbType.NVarChar)
                                        .Value = Convert.ToString(
                                            fila.Cells["Gravedad"].Value);
                                    cmd.Parameters.Add("@Estado", SqlDbType.NVarChar)
                                        .Value = "Abierta";

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaccion.Commit();

                            MessageBox.Show(
                                "Control de calidad registrado correctamente.\n" +
                                "N.º de control: " + idControl,
                                "Registro exitoso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            LimpiarFormulario();
                        }

                        catch
                        {
                            transaccion.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo guardar el control:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private int ObtenerIdCriterio(
            string nombre,
            SqlConnection conexion,
            SqlTransaction transaccion)
        {
            string consulta = @"
        SELECT IdCriterio
        FROM dbo.CriteriosCalidad
        WHERE Nombre = @Nombre";

            using (SqlCommand cmd =
                new SqlCommand(consulta, conexion, transaccion))
            {
                cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar)
                    .Value = nombre;

                object resultado = cmd.ExecuteScalar();

                if (resultado == null || resultado == DBNull.Value)
                {
                    throw new Exception(
                        "No se encontró el criterio de calidad: " + nombre);
                }

                return Convert.ToInt32(resultado);
            }
        }

        private NumericUpDown CrearNumero()
        {
            return new NumericUpDown
            {
                Width = 110,
                Maximum = 1000000,
                Minimum = 0
            };
        }

        // Eventos anteriores del diseñador.
        private void pnlEncabezado_Paint(
            object sender, PaintEventArgs e)
        {
        }

        private void label3_Click(
            object sender, EventArgs e)
        {
        }

        private void label4_Click(
            object sender, EventArgs e)
        {
        }

        private void CargarPedidos()
        {
            try
            {
                using (SqlConnection conexion =
                    new SqlConnection(cadenaConexion))
                {
                    string consulta = @"
                SELECT
                    IdPedido,
                    'Pedido N.º ' + CAST(IdPedido AS VARCHAR(20))
                    + ' - ' + Cliente AS Descripcion
                FROM Pedidos
                WHERE Estado IN ('En Producción', 'Confirmado')
                ORDER BY IdPedido DESC";

                    SqlDataAdapter adaptador =
                        new SqlDataAdapter(consulta, conexion);

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    cboPedido.DataSource = tabla;
                    cboPedido.DisplayMember = "Descripcion";
                    cboPedido.ValueMember = "IdPedido";
                    cboPedido.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar los pedidos: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CargarTrabajos(int idPedido)
        {
            try
            {
                using (SqlConnection conexion =
                    new SqlConnection(cadenaConexion))
                {
                    string consulta = @"
                SELECT
                    IdDetalle,
                    Trabajo + ' - ' +
                    CAST(Cantidad AS VARCHAR(20)) +
                    ' unidades' AS Descripcion
                FROM DetallePedido
                WHERE IdPedido = @IdPedido
                ORDER BY IdDetalle";

                    SqlDataAdapter adaptador =
                        new SqlDataAdapter(consulta, conexion);

                    adaptador.SelectCommand.Parameters.Add(
                        "@IdPedido", SqlDbType.Int).Value = idPedido;

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    cboTrabajo.DataSource = tabla;
                    cboTrabajo.DisplayMember = "Descripcion";
                    cboTrabajo.ValueMember = "IdDetalle";
                    cboTrabajo.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar los trabajos: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void MostrarEstadoPedido(int idPedido)
        {
            try
            {
                using (SqlConnection conexion =
                    new SqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string consulta =
                        "SELECT Estado FROM Pedidos WHERE IdPedido = @IdPedido";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.Add(
                            "@IdPedido", SqlDbType.Int).Value = idPedido;

                        object resultado = comando.ExecuteScalar();

                        lblEstadoPedido.Text =
                            resultado != null
                            ? resultado.ToString()
                            : "Sin seleccionar";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al consultar el estado: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void cboPedido_SelectionChangeCommitted(
            object sender, EventArgs e)
        {
            if (cboPedido.SelectedValue == null)
                return;

            int idPedido;

            if (!int.TryParse(
                cboPedido.SelectedValue.ToString(), out idPedido))
                return;

            CargarTrabajos(idPedido);
            MostrarEstadoPedido(idPedido);
        }

        private void ColorearEvaluaciones()
        {
            foreach (DataGridViewRow fila in dgvCriterios.Rows)
            {
                if (fila.IsNewRow) continue;

                string evaluacion = Convert.ToString(
                    fila.Cells["Evaluacion"].Value);

                Color fondo;
                Color texto;

                switch (evaluacion)
                {
                    case "Conforme":
                        fondo = Color.FromArgb(216, 245, 227);
                        texto = Color.FromArgb(23, 104, 58);
                        break;

                    case "No conforme":
                        fondo = Color.FromArgb(255, 217, 222);
                        texto = Color.FromArgb(167, 25, 48);
                        break;

                    default:
                        fondo = Color.FromArgb(233, 237, 242);
                        texto = Color.FromArgb(85, 100, 119);
                        break;
                }

                // Colorear toda la fila si hay una no conformidad
                if (evaluacion == "No conforme")
                {
                    fila.DefaultCellStyle.BackColor =
                        Color.FromArgb(255, 239, 241);
                }
                else
                {
                    fila.DefaultCellStyle.BackColor =
                        fila.Index % 2 == 0
                        ? Color.White
                        : Color.FromArgb(242, 249, 253);
                }

                DataGridViewCell celda =
                    fila.Cells["Evaluacion"];

                celda.Style.BackColor = fondo;
                celda.Style.ForeColor = texto;
                celda.Style.SelectionBackColor = fondo;
                celda.Style.SelectionForeColor = texto;
            }
        }

        private void dgvCriterios_CurrentCellDirtyStateChanged(
            object sender, EventArgs e)
        {
            if (dgvCriterios.IsCurrentCellDirty)
            {
                dgvCriterios.CommitEdit(
                    DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvCriterios_CellValueChanged(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvCriterios.Columns[e.ColumnIndex].Name
                == "Evaluacion")
            {
                ColorearEvaluaciones();
            }
        }


        private void CargarTiposDefecto()
        {
            cboTipoDefecto.Items.Clear();

            switch (cboCriterioDefecto.Text)
            {
                case "Calidad de impresión":
                    cboTipoDefecto.Items.AddRange(new object[]
                    {
                "Manchas",
                "Impresión borrosa",
                "Falta de tinta",
                "Otro"
                    });
                    break;

                case "Fidelidad del color":
                    cboTipoDefecto.Items.AddRange(new object[]
                    {
                "Color incorrecto",
                "Diferencia de tonalidad",
                "Otro"
                    });
                    break;

                case "Alineación":
                    cboTipoDefecto.Items.AddRange(new object[]
                    {
                "Texto descentrado",
                "Imagen desplazada",
                "Otro"
                    });
                    break;

                case "Precisión del corte":
                    cboTipoDefecto.Items.AddRange(new object[]
                    {
                "Corte desalineado",
                "Medidas incorrectas",
                "Bordes irregulares",
                "Otro"
                    });
                    break;

                case "Calidad del papel":
                    cboTipoDefecto.Items.AddRange(new object[]
                    {
                "Papel arrugado",
                "Papel roto",
                "Manchas",
                "Otro"
                    });
                    break;

                case "Terminaciones":
                    cboTipoDefecto.Items.AddRange(new object[]
                    {
                "Encuadernación defectuosa",
                "Laminado incorrecto",
                "Otro"
                    });
                    break;

                case "Cantidad producida":
                    cboTipoDefecto.Items.AddRange(new object[]
                    {
                "Faltante de unidades",
                "Excedente de unidades",
                "Otro"
                    });
                    break;

                case "Presentación final":
                    cboTipoDefecto.Items.AddRange(new object[]
                    {
                "Embalaje dañado",
                "Producto sucio",
                "Otro"
                    });
                    break;
            }

            cboTipoDefecto.SelectedIndex = -1;
        }
        private void ActualizarResultado()
        {
            if (nudCantidadDefectuosa.Value > 0)
            {
                cboResultado.SelectedItem = "Con defectos";
            }
            else
            {
                cboResultado.SelectedItem = "Aprobado";
            }
        }

        private void LimpiarFormulario()
        {
            // Datos del trabajo
            cboPedido.SelectedIndex = -1;
            cboTrabajo.DataSource = null;
            txtInspector.Clear();
            dtpFechaControl.Value = DateTime.Today;
            lblEstadoPedido.Text = "Sin seleccionar";

            // Reiniciar evaluaciones
            foreach (DataGridViewRow fila in dgvCriterios.Rows)
            {
                if (!fila.IsNewRow)
                {
                    fila.Cells["Evaluacion"].Value = "Conforme";
                    fila.DefaultCellStyle.BackColor = Color.White;
                }
            }

            dgvCriterios.ClearSelection();

            // Limpiar no conformidades
            dgvDefectos.Rows.Clear();
            cboCriterioDefecto.SelectedIndex = -1;
            cboTipoDefecto.Items.Clear();
            cboGravedad.SelectedIndex = -1;
            txtDescripcionDefecto.Clear();
            nudCantidadAfectada.Value = 1;

            // Reiniciar resultado
            nudCantidadRevisada.Value = 0;
            nudCantidadAprobada.Value = 0;
            nudCantidadDefectuosa.Value = 0;

            cboResultado.SelectedIndex = -1;
            txtObservaciones.Clear();
        }

        private void GenerarInformeMensual()
        {
            try
            {
                int mes = cboMesInforme.SelectedIndex + 1;
                int anio = (int)nudAnioInforme.Value;

                DateTime desde = new DateTime(anio, mes, 1);
                DateTime hasta = desde.AddMonths(1);

                string sql = @"
            SELECT
                c.IdControl AS [N.º Control],
                c.IdPedido AS [Pedido],
                d.Trabajo,
                c.FechaControl AS [Fecha],
                c.CantidadRevisada AS [Revisadas],
                c.CantidadAprobada AS [Aprobadas],
                c.CantidadDefectuosa AS [Defectuosas],
                c.Resultado
            FROM dbo.ControlCalidadPropios c
            INNER JOIN dbo.DetallePedido d
                ON c.IdDetalle = d.IdDetalle
            WHERE c.FechaControl >= @Desde
              AND c.FechaControl < @Hasta
            ORDER BY c.FechaControl DESC";

                DataTable tabla = new DataTable();

                using (SqlConnection conexion =
                    new SqlConnection(cadenaConexion))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conexion))
                {
                    cmd.Parameters.Add("@Desde", SqlDbType.DateTime)
                        .Value = desde;

                    cmd.Parameters.Add("@Hasta", SqlDbType.DateTime)
                        .Value = hasta;

                    using (SqlDataAdapter adaptador =
                        new SqlDataAdapter(cmd))
                    {
                        adaptador.Fill(tabla);
                    }
                }

                dgvInformeMensual.DataSource = tabla;

                long revisadas = 0;
                long aprobadas = 0;
                long defectuosas = 0;

                foreach (DataRow fila in tabla.Rows)
                {
                    revisadas += Convert.ToInt64(fila["Revisadas"]);
                    aprobadas += Convert.ToInt64(fila["Aprobadas"]);
                    defectuosas += Convert.ToInt64(fila["Defectuosas"]);
                }

                double porcentaje = revisadas > 0
                    ? (double)defectuosas / revisadas * 100
                    : 0;

                lblTotalRevisadas.Text = revisadas.ToString("N0");
                lblTotalAprobadas.Text = aprobadas.ToString("N0");
                lblTotalDefectuosas.Text = defectuosas.ToString("N0");
                lblPorcentajeDefectos.Text = porcentaje.ToString("N2") + "%";

                ActualizarGraficosInforme(
                aprobadas,
                defectuosas,
                desde,
                hasta);

                if (tabla.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "No se encontraron controles de calidad " +
                        "para el período seleccionado.",
                        "Sin resultados",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al generar el informe:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CrearGraficosInforme()
        {
            panelGraficosInforme.Controls.Clear();

            TableLayoutPanel contenedor = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            contenedor.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 40));

            contenedor.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 60));

            panelGraficosInforme.Controls.Add(contenedor);

            // Gráfico circular
            graficoDistribucion = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            ChartArea areaCircular = new ChartArea("Area1");

            // Ajustar el espacio disponible para el gráfico
            areaCircular.Position = new ElementPosition(5, 12, 72, 80);

            areaCircular.InnerPlotPosition =
                new ElementPosition(5, 5, 90, 90);

            graficoDistribucion.ChartAreas.Add(areaCircular);

            Legend leyenda = new Legend("Leyenda")
            {
                Docking = Docking.Right,
                Alignment = StringAlignment.Center
            };

            graficoDistribucion.Legends.Add(leyenda);

            Series serieCircular = new Series("Distribución")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                LabelFormat = "N0"
            };

            graficoDistribucion.Series.Add(serieCircular);

            serieCircular["PieDrawingStyle"] = "SoftEdge";
            serieCircular["PieLabelStyle"] = "Inside";

            graficoDistribucion.Titles.Add(
                "Distribución de unidades inspeccionadas");

            // Gráfico de barras
            graficoDefectos = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            ChartArea area = new ChartArea("Area1");
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -35;
            area.AxisY.Minimum = 0;
            area.AxisY.Title = "Cantidad afectada";

            graficoDefectos.ChartAreas.Add(area);

            Series serieBarras = new Series("Defectos")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                Color = Color.FromArgb(0, 120, 215)
            };

            graficoDefectos.Series.Add(serieBarras);

            graficoDefectos.Titles.Add(
                "Defectos por criterio de calidad");

            contenedor.Controls.Add(graficoDistribucion, 0, 0);
            contenedor.Controls.Add(graficoDefectos, 1, 0);
        }

        private void ActualizarGraficosInforme(
            long aprobadas,
            long defectuosas,
            DateTime desde,
            DateTime hasta)
        {
            Series circular = graficoDistribucion.Series["Distribución"];
            circular.Points.Clear();

            if (aprobadas + defectuosas > 0)
            {
                int aprobado = circular.Points.AddXY("Aprobadas", aprobadas);
                circular.Points[aprobado].Color = Color.FromArgb(46, 160, 100);

                int defectuoso = circular.Points.AddXY("Defectuosas", defectuosas);
                circular.Points[defectuoso].Color = Color.FromArgb(220, 75, 85);
            }

            Series barras = graficoDefectos.Series["Defectos"];
            barras.Points.Clear();

            string sql = @"
        SELECT
            c.Nombre AS Criterio,
            SUM(n.CantidadAfectada) AS Cantidad
        FROM dbo.NoConformidades n
        INNER JOIN dbo.ControlCalidadPropios cc
            ON n.IdControl = cc.IdControl
        INNER JOIN dbo.CriteriosCalidad c
            ON n.IdCriterio = c.IdCriterio
        WHERE cc.FechaControl >= @Desde
          AND cc.FechaControl < @Hasta
        GROUP BY c.Nombre
        ORDER BY Cantidad DESC";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
                cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = hasta;

                conexion.Open();

                using (SqlDataReader lector = cmd.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        string criterio = lector["Criterio"].ToString();
                        int cantidad = Convert.ToInt32(lector["Cantidad"]);

                        barras.Points.AddXY(criterio, cantidad);
                    }
                }
            }
        }


       

        private byte[] ObtenerImagenGrafico(
            System.Windows.Forms.DataVisualization.Charting.Chart grafico)
        {
            using (MemoryStream memoria = new MemoryStream())
            {
                grafico.SaveImage(
                    memoria,
                    System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);

                return memoria.ToArray();
            }
        }

        private PdfPCell CeldaPDF(string texto, bool encabezado = false)
        {
            BaseColor colorFondo = encabezado
                ? new BaseColor(8, 43, 73)
                : BaseColor.WHITE;

            iTextSharp.text.Font fuente = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA,
                8,
                encabezado
                    ? iTextSharp.text.Font.BOLD
                    : iTextSharp.text.Font.NORMAL,
                encabezado ? BaseColor.WHITE : BaseColor.BLACK);

            PdfPCell celda = new PdfPCell(
                new PdfPhrase(texto ?? "", fuente))
            {
                BackgroundColor = colorFondo,
                Padding = 7,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = new BaseColor(215, 230, 240)
            };

            return celda;
        }

        private void ExportarInformePDF()
        {
            if (dgvInformeMensual.DataSource == null)
            {
                MessageBox.Show(
                    "Primero debe generar el informe mensual.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            int mes = cboMesInforme.SelectedIndex + 1;
            int anio = (int)nudAnioInforme.Value;

            if (mes < 1)
            {
                MessageBox.Show("Seleccione un mes válido.");
                return;
            }

            DateTime desde = new DateTime(anio, mes, 1);
            DateTime hasta = desde.AddMonths(1);

            // Consultar los datos del período directamente desde SQL Server.
            DataTable inspecciones = new DataTable();
            DataTable defectos = new DataTable();

            try
            {
                using (SqlConnection conexion =
                    new SqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string sqlInspecciones = @"
                SELECT
                    c.IdControl AS Control,
                    c.IdPedido AS Pedido,
                    d.Trabajo,
                    c.FechaControl AS Fecha,
                    c.CantidadRevisada AS Revisadas,
                    c.CantidadAprobada AS Aprobadas,
                    c.CantidadDefectuosa AS Defectuosas,
                    c.Resultado
                FROM dbo.ControlCalidadPropios c
                INNER JOIN dbo.DetallePedido d
                    ON c.IdDetalle = d.IdDetalle
                WHERE c.FechaControl >= @Desde
                  AND c.FechaControl < @Hasta
                ORDER BY c.FechaControl DESC, c.IdControl DESC";

                    using (SqlCommand cmd =
                        new SqlCommand(sqlInspecciones, conexion))
                    {
                        cmd.Parameters.Add("@Desde", SqlDbType.DateTime)
                            .Value = desde;

                        cmd.Parameters.Add("@Hasta", SqlDbType.DateTime)
                            .Value = hasta;

                        using (SqlDataAdapter adaptador =
                            new SqlDataAdapter(cmd))
                        {
                            adaptador.Fill(inspecciones);
                        }
                    }

                    string sqlDefectos = @"
                SELECT
                    cc.IdPedido AS Pedido,
                    c.Nombre AS Criterio,
                    n.TipoDefecto AS Defecto,
                    n.Descripcion AS Descripcion,
                    n.CantidadAfectada AS Cantidad,
                    n.Gravedad,
                    n.Estado
                FROM dbo.NoConformidades n
                INNER JOIN dbo.ControlCalidadPropios cc
                    ON n.IdControl = cc.IdControl
                LEFT JOIN dbo.CriteriosCalidad c
                    ON n.IdCriterio = c.IdCriterio
                WHERE cc.FechaControl >= @Desde
                  AND cc.FechaControl < @Hasta
                ORDER BY cc.IdPedido, n.IdNoConformidad";

                    using (SqlCommand cmd =
                        new SqlCommand(sqlDefectos, conexion))
                    {
                        cmd.Parameters.Add("@Desde", SqlDbType.DateTime)
                            .Value = desde;

                        cmd.Parameters.Add("@Hasta", SqlDbType.DateTime)
                            .Value = hasta;

                        using (SqlDataAdapter adaptador =
                            new SqlDataAdapter(cmd))
                        {
                            adaptador.Fill(defectos);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron consultar los datos del informe:\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (inspecciones.Rows.Count == 0)
            {
                MessageBox.Show(
                    "No existen inspecciones para el período seleccionado.",
                    "Sin datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            long revisadas = 0;
            long aprobadas = 0;
            long defectuosas = 0;

            foreach (DataRow fila in inspecciones.Rows)
            {
                revisadas += Convert.ToInt64(fila["Revisadas"]);
                aprobadas += Convert.ToInt64(fila["Aprobadas"]);
                defectuosas += Convert.ToInt64(fila["Defectuosas"]);
            }

            double porcentaje = revisadas > 0
                ? defectuosas * 100.0 / revisadas
                : 0;

            // Actualizar los gráficos con los datos del período.
            try
            {
                ActualizarGraficosInforme(
                    aprobadas, defectuosas, desde, hasta);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron actualizar los gráficos:\n" +
                    ex.Message);
                return;
            }

            using (SaveFileDialog guardar = new SaveFileDialog())
            {
                guardar.Title = "Guardar informe mensual";
                guardar.Filter = "Documento PDF (*.pdf)|*.pdf";
                guardar.FileName =
                    "Informe_Calidad_" + anio + "_" +
                    mes.ToString("00") + ".pdf";

                if (guardar.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    Document documento = new Document(
                        PageSize.A4.Rotate(), 35, 35, 40, 45);

                    using (FileStream archivo =
                        new FileStream(guardar.FileName, FileMode.Create))
                    {
                        PdfWriter.GetInstance(documento, archivo);

                        documento.Open();

                        BaseColor azulPDF = new BaseColor(8, 43, 73);

                        iTextSharp.text.Font fuenteTitulo =
                            FontFactory.GetFont(
                                FontFactory.HELVETICA_BOLD,
                                18, azulPDF);

                        iTextSharp.text.Font fuenteSubtitulo =
                            FontFactory.GetFont(
                                FontFactory.HELVETICA_BOLD,
                                12, azulPDF);

                        iTextSharp.text.Font fuenteTexto =
                            FontFactory.GetFont(
                                FontFactory.HELVETICA,
                                10, BaseColor.DARK_GRAY);

                        // ====================================
                        // 1. ENCABEZADO INSTITUCIONAL
                        // ====================================

                        PdfPTable encabezado = new PdfPTable(2);
                        encabezado.WidthPercentage = 100;
                        encabezado.SetWidths(new float[] { 1, 5 });

                        PdfPCell celdaLogo = new PdfPCell
                        {
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };

                        using (MemoryStream memoria = new MemoryStream())
                        {
                            using (System.Drawing.Bitmap bitmap =
                                new System.Drawing.Bitmap(Properties.Resources.LogoSistema))
                            {
                                bitmap.Save(
                                    memoria,
                                    System.Drawing.Imaging.ImageFormat.Png);
                            }

                            iTextSharp.text.Image logo =
                                iTextSharp.text.Image.GetInstance(memoria.ToArray());

                            logo.ScaleToFit(90, 65);
                            celdaLogo.AddElement(logo);
                        }

                        encabezado.AddCell(celdaLogo);

                        PdfPCell celdaTitulo = new PdfPCell
                        {
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };

                        celdaTitulo.AddElement(new PdfParagraph(
                            "C.E SYSTEM - IMPRENTA CENTRAL",
                            fuenteTitulo));

                        celdaTitulo.AddElement(new PdfParagraph(
                            "INFORME MENSUAL DE CONTROL DE CALIDAD",
                            fuenteSubtitulo));

                        encabezado.AddCell(celdaTitulo);
                        documento.Add(encabezado);

                        documento.Add(new PdfParagraph(" "));

                        // ====================================
                        // 2. DATOS GENERALES
                        // ====================================

                        documento.Add(new PdfParagraph(
                            "Período evaluado: " +
                            cboMesInforme.Text + " " + anio,
                            fuenteTexto));

                        documento.Add(new PdfParagraph(
                            "Fecha de emisión: " +
                            DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            fuenteTexto));

                        documento.Add(new PdfParagraph(
                            "Cantidad de inspecciones realizadas: " +
                            inspecciones.Rows.Count,
                            fuenteTexto));

                        documento.Add(new PdfParagraph(" "));

                        // ====================================
                        // 3. RESUMEN ESTADÍSTICO
                        // ====================================

                        documento.Add(new PdfParagraph(
                            "RESUMEN ESTADÍSTICO",
                            fuenteSubtitulo));

                        PdfPTable indicadores = new PdfPTable(4);
                        indicadores.WidthPercentage = 100;
                        indicadores.SpacingBefore = 8;
                        indicadores.SpacingAfter = 12;

                        indicadores.AddCell(CeldaPDF("REVISADAS", true));
                        indicadores.AddCell(CeldaPDF("APROBADAS", true));
                        indicadores.AddCell(CeldaPDF("DEFECTUOSAS", true));
                        indicadores.AddCell(CeldaPDF("% DEFECTUOSO", true));

                        indicadores.AddCell(
                            CeldaPDF(revisadas.ToString("N0")));

                        indicadores.AddCell(
                            CeldaPDF(aprobadas.ToString("N0")));

                        indicadores.AddCell(
                            CeldaPDF(defectuosas.ToString("N0")));

                        indicadores.AddCell(
                            CeldaPDF(porcentaje.ToString("N2") + "%"));

                        documento.Add(indicadores);

                        // ====================================
                        // 4. GRÁFICOS
                        // ====================================

                        documento.Add(new PdfParagraph(
                            "ANÁLISIS GRÁFICO DE CALIDAD",
                            fuenteSubtitulo));

                        PdfPTable tablaGraficos = new PdfPTable(2);
                        tablaGraficos.WidthPercentage = 100;
                        tablaGraficos.SpacingBefore = 10;
                        tablaGraficos.SpacingAfter = 15;

                        iTextSharp.text.Image imagenCircular =
                            iTextSharp.text.Image.GetInstance(
                                ObtenerImagenGrafico(graficoDistribucion));

                        iTextSharp.text.Image imagenBarras =
                            iTextSharp.text.Image.GetInstance(
                                ObtenerImagenGrafico(graficoDefectos));

                        imagenCircular.ScaleToFit(300, 180);
                        imagenBarras.ScaleToFit(390, 180);

                        PdfPCell celdaCircular =
                            new PdfPCell(imagenCircular, true);

                        PdfPCell celdaBarras =
                            new PdfPCell(imagenBarras, true);

                        celdaCircular.Padding = 5;
                        celdaBarras.Padding = 5;

                        tablaGraficos.AddCell(celdaCircular);
                        tablaGraficos.AddCell(celdaBarras);

                        documento.Add(tablaGraficos);

                        // ====================================
                        // 5. DETALLE DE INSPECCIONES
                        // ====================================

                        documento.Add(new PdfParagraph(
                            "DETALLE DE INSPECCIONES",
                            fuenteSubtitulo));

                        PdfPTable tablaInspecciones = new PdfPTable(8);
                        tablaInspecciones.WidthPercentage = 100;
                        tablaInspecciones.SpacingBefore = 8;
                        tablaInspecciones.HeaderRows = 1;

                        string[] columnas =
                        {
                    "Control", "Pedido", "Trabajo", "Fecha",
                    "Revisadas", "Aprobadas",
                    "Defectuosas", "Resultado"
                };

                        foreach (string columna in columnas)
                        {
                            tablaInspecciones.AddCell(
                                CeldaPDF(columna, true));
                        }

                        foreach (DataRow fila in inspecciones.Rows)
                        {
                            foreach (DataColumn columna in inspecciones.Columns)
                            {
                                string valor;

                                if (columna.ColumnName == "Fecha")
                                {
                                    valor = Convert.ToDateTime(
                                        fila[columna]).ToString(
                                            "dd/MM/yyyy HH:mm");
                                }
                                else
                                {
                                    valor = Convert.ToString(fila[columna]);
                                }

                                tablaInspecciones.AddCell(
                                    CeldaPDF(valor));
                            }
                        }

                        documento.Add(tablaInspecciones);
                        documento.Add(new PdfParagraph(" "));

                        // ====================================
                        // 6. NO CONFORMIDADES
                        // ====================================

                        documento.Add(new PdfParagraph(
                            "NO CONFORMIDADES DETECTADAS",
                            fuenteSubtitulo));

                        if (defectos.Rows.Count > 0)
                        {
                            PdfPTable tablaDefectos =
                                new PdfPTable(defectos.Columns.Count);

                            tablaDefectos.WidthPercentage = 100;
                            tablaDefectos.SpacingBefore = 8;
                            tablaDefectos.HeaderRows = 1;

                            foreach (DataColumn columna in defectos.Columns)
                            {
                                tablaDefectos.AddCell(
                                    CeldaPDF(columna.ColumnName, true));
                            }

                            foreach (DataRow fila in defectos.Rows)
                            {
                                foreach (object valor in fila.ItemArray)
                                {
                                    tablaDefectos.AddCell(
                                        CeldaPDF(Convert.ToString(valor)));
                                }
                            }

                            documento.Add(tablaDefectos);
                        }
                        else
                        {
                            documento.Add(new PdfParagraph(
                                "No se registraron no conformidades " +
                                "durante el período seleccionado.",
                                fuenteTexto));
                        }

                        documento.Add(new PdfParagraph(" "));

                        // ====================================
                        // 7. CONCLUSIÓN AUTOMÁTICA
                        // ====================================

                        documento.Add(new PdfParagraph(
                            "CONCLUSIÓN DEL INFORME",
                            fuenteSubtitulo));

                        string conclusion =
                            "Durante " + cboMesInforme.Text + " de " +
                            anio + " se realizaron " +
                            inspecciones.Rows.Count +
                            " inspecciones de control de calidad. " +
                            "Se revisaron " + revisadas +
                            " unidades, de las cuales " +
                            aprobadas + " fueron aprobadas y " +
                            defectuosas + " presentaron defectos. " +
                            "El porcentaje de unidades defectuosas " +
                            "fue del " +
                            porcentaje.ToString("N2") + "%. ";

                        if (defectuosas > 0)
                        {
                            conclusion +=
                                "Se recomienda revisar las no conformidades " +
                                "detectadas y aplicar las medidas correctivas " +
                                "correspondientes.";
                        }
                        else
                        {
                            conclusion +=
                                "No se registraron unidades defectuosas " +
                                "en las inspecciones del período.";
                        }

                        documento.Add(new PdfParagraph(
                            conclusion, fuenteTexto));

                        documento.Add(new PdfParagraph(" "));

                        documento.Add(new PdfParagraph(
                            "Documento generado por C.E SYSTEM - " +
                            "Imprenta Central.",
                            fuenteTexto));

                        documento.Close();
                    }

                    MessageBox.Show(
                        "Informe mensual exportado correctamente.",
                        "C.E SYSTEM",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    System.Diagnostics.Process.Start(guardar.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "No se pudo exportar el PDF:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

    }
}

