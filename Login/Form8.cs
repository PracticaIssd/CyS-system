using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Login
{
    public partial class Form8 : Form
    {
        private readonly string conexion =
    @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";

        public Form8()
        {
            InitializeComponent();


            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            RedondearControl(pnlClientes, 18);
            RedondearControl(pnlPedidos, 18);
            RedondearControl(pnlVentas, 18);
            RedondearControl(pnlPendientes, 18);

            RedondearControl(pnlEstados, 15);
            RedondearControl(pnlPagos, 15);
            RedondearControl(pnlTrabajos, 15);

            RedondearControl(btnActualizar, 12);
            RedondearControl(btnVolver, 12);

            CargarDashboard();
        }

        private void CargarDashboard()
        {
            CargarIndicadores();
            CargarGraficoEstados();
            CargarGraficoPagos();
            CargarGraficoTrabajos(); ;
        }
        private void CargarGraficoPagos()
        {
            chartPagos.Series.Clear();
            chartPagos.Titles.Clear();
            chartPagos.Legends.Clear();
            chartPagos.ChartAreas.Clear();

            ChartArea area = new ChartArea("AreaPagos");
            chartPagos.ChartAreas.Add(area);

            Title titulo = new Title("Pedidos por forma de pago");
            titulo.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            chartPagos.Titles.Add(titulo);

            Legend leyenda = new Legend("LeyendaPagos");
            leyenda.Docking = Docking.Bottom;
            chartPagos.Legends.Add(leyenda);

            Series serie = new Series("Formas de pago");

            serie.ChartType = SeriesChartType.Pie;
            serie.IsValueShownAsLabel = true;
            serie.Label = "#PERCENT{P0}";
            serie.LegendText = "#VALX: #VAL";
            serie.Legend = "LeyendaPagos";

            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT FormaPago, COUNT(*) AS Cantidad
                FROM Pedidos
                WHERE FormaPago IS NOT NULL
                  AND LTRIM(RTRIM(FormaPago)) <> ''
                GROUP BY FormaPago
                ORDER BY Cantidad DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader lector = cmd.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            string formaPago = lector["FormaPago"].ToString();
                            int cantidad = Convert.ToInt32(lector["Cantidad"]);

                            serie.Points.AddXY(formaPago, cantidad);
                        }
                    }

                    chartPagos.Series.Add(serie);

                    if (serie.Points.Count == 0)
                    {
                        chartPagos.Titles.Add(
                            new Title("No hay formas de pago registradas"));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar el gráfico de pagos: " +
                        ex.Message);
                }
            }
        }
        private void CargarGraficoTrabajos()
        {
            chartTrabajos.Series.Clear();
            chartTrabajos.Titles.Clear();
            chartTrabajos.Legends.Clear();
            chartTrabajos.ChartAreas.Clear();

            ChartArea area = new ChartArea("AreaTrabajos");

            area.AxisX.Title = "Trabajo";
            area.AxisY.Title = "Cantidad";
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -30;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;

            chartTrabajos.ChartAreas.Add(area);

            Title titulo = new Title("Trabajos más solicitados");
            titulo.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            chartTrabajos.Titles.Add(titulo);

            Series serie = new Series("Trabajos");

            serie.ChartType = SeriesChartType.Column;
            serie.IsValueShownAsLabel = true;
            serie.Color = Color.DodgerBlue;

            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT TOP 10
                       Trabajo,
                       SUM(Cantidad) AS CantidadTotal
                FROM DetallePedido
                WHERE Trabajo IS NOT NULL
                  AND LTRIM(RTRIM(Trabajo)) <> ''
                GROUP BY Trabajo
                ORDER BY CantidadTotal DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader lector = cmd.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            string trabajo = lector["Trabajo"].ToString();

                            int cantidad =
                                Convert.ToInt32(lector["CantidadTotal"]);

                            serie.Points.AddXY(trabajo, cantidad);
                        }
                    }

                    chartTrabajos.Series.Add(serie);

                    if (serie.Points.Count == 0)
                    {
                        chartTrabajos.Titles.Add(
                            new Title("No hay trabajos registrados"));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar el gráfico de trabajos: " +
                        ex.Message);
                }
            }
        }
        private void CargarIndicadores()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    // TOTAL DE CLIENTES
                    string queryClientes =
                        "SELECT COUNT(*) FROM Clientes";

                    using (SqlCommand cmd =
                           new SqlCommand(queryClientes, conn))
                    {
                        lblTotalClientes.Text =
                            cmd.ExecuteScalar().ToString();
                    }

                    // TOTAL DE PEDIDOS
                    string queryPedidos =
                        "SELECT COUNT(*) FROM Pedidos";

                    using (SqlCommand cmd =
                           new SqlCommand(queryPedidos, conn))
                    {
                        lblTotalPedidos.Text =
                            cmd.ExecuteScalar().ToString();
                    }

                    // TOTAL VENDIDO
                    string queryVentas = @"
                SELECT ISNULL(SUM(ImporteTotal), 0)
                FROM Pedidos
                WHERE Estado <> 'Cancelado'";

                    using (SqlCommand cmd =
                           new SqlCommand(queryVentas, conn))
                    {
                        decimal total =
                            Convert.ToDecimal(cmd.ExecuteScalar());

                        string textoTotal =
                             total.ToString("N0");

                        lblTotalVendido.Text = textoTotal;

                        // Reduce la letra si el importe es muy largo
                        if (textoTotal.Length >= 14)
                        {
                            lblTotalVendido.Font =
                                new Font(
                                    "Segoe UI",
                                    12,
                                    FontStyle.Bold);
                        }
                        else if (textoTotal.Length >= 11)
                        {
                            lblTotalVendido.Font =
                                new Font(
                                    "Segoe UI",
                                    15,
                                    FontStyle.Bold);
                        }
                        else if (textoTotal.Length >= 8)
                        {
                            lblTotalVendido.Font =
                                new Font(
                                    "Segoe UI",
                                    18,
                                    FontStyle.Bold);
                        }
                        else
                        {
                            lblTotalVendido.Font =
                                new Font(
                                    "Segoe UI",
                                    21,
                                    FontStyle.Bold);
                        }
                    }

                    // PEDIDOS PENDIENTES
                    string queryPendientes = @"
                SELECT COUNT(*)
                FROM Pedidos
                WHERE Estado = 'Pendiente'";

                    using (SqlCommand cmd =
                           new SqlCommand(queryPendientes, conn))
                    {
                        lblPendientes.Text =
                            cmd.ExecuteScalar().ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar los indicadores: " +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void CargarGraficoEstados()
        {
            chartEstados.Series.Clear();
            chartEstados.Titles.Clear();
            chartEstados.Legends.Clear();
            chartEstados.ChartAreas.Clear();

            chartEstados.ChartAreas.Add(
                new ChartArea("AreaEstados"));

            Title titulo =
                new Title("Estados de los pedidos");

            titulo.Font =
                new Font("Segoe UI", 13, FontStyle.Bold);

            chartEstados.Titles.Add(titulo);

            Legend leyenda =
                new Legend("LeyendaEstados");

            leyenda.Docking = Docking.Bottom;

            chartEstados.Legends.Add(leyenda);

            Series serie =
                new Series("Estados");

            serie.ChartType =
                SeriesChartType.Doughnut;

            serie.IsValueShownAsLabel = true;
            serie.Label = "#PERCENT{P0}";
            serie.LegendText = "#VALX: #VAL";
            serie.Legend = "LeyendaEstados";

            serie["DoughnutRadius"] = "55";
            serie["PieLabelStyle"] = "Outside";

            string[] estados =
            {
                "Pendiente",
                "En Producción",
                "Finalizado",
                "Entregado",
                "Cancelado"
            };

            Color[] colores =
            {
                Color.Gold,
                Color.DodgerBlue,
                Color.MediumSeaGreen,
                Color.SeaGreen,
                Color.IndianRed
            };

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    for (int i = 0; i < estados.Length; i++)
                    {
                        string query = @"
                            SELECT COUNT(*)
                            FROM Pedidos
                            WHERE Estado = @estado";

                        using (SqlCommand cmd =
                               new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue(
                                "@estado",
                                estados[i]);

                            int cantidad =
                                Convert.ToInt32(
                                    cmd.ExecuteScalar());

                            if (cantidad > 0)
                            {
                                int posicion =
                                    serie.Points.AddXY(
                                        estados[i],
                                        cantidad);

                                serie.Points[posicion].Color =
                                    colores[i];
                            }
                        }
                    }

                    chartEstados.Series.Add(serie);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar el gráfico: " +
                        ex.Message);
                }
            }
        }

        private void RedondearControl(
            Control control,
            int radio)
        {
            System.Drawing.Drawing2D.GraphicsPath ruta =
                new System.Drawing.Drawing2D.GraphicsPath();

            ruta.StartFigure();

            ruta.AddArc(
                0,
                0,
                radio,
                radio,
                180,
                90);

            ruta.AddArc(
                control.Width - radio,
                0,
                radio,
                radio,
                270,
                90);

            ruta.AddArc(
                control.Width - radio,
                control.Height - radio,
                radio,
                radio,
                0,
                90);

            ruta.AddArc(
                0,
                control.Height - radio,
                radio,
                radio,
                90,
                90);

            ruta.CloseFigure();

            control.Region =
                new Region(ruta);
        }

        private void btnActualizar_Click(
            object sender,
            EventArgs e)
        {
            CargarDashboard();
        }

        private void btnVolver_Click(
            object sender,
            EventArgs e)
        {
            Form5 frm = new Form5();
            frm.Show();
            this.Close();
        }

        private void pnlEncabezado_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}