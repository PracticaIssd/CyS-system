
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace Login
{
    public partial class Form18 : Form
    {
        private WebView2 mapa;
        private Label estado;

        public Form18()
        {
            Text = "C.E SYSTEM - Gestión de Envíos y Rutas";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(950, 620);
            BackColor = Color.FromArgb(211, 233, 247);
            Font = new Font("Segoe UI", 10);

            // ENCABEZADO
            var cabecera = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.FromArgb(10, 42, 73)
            };

            var titulo = new Label
            {
                Text = "  C.E SYSTEM  |  Gestión de Envíos y Rutas",
                Dock = DockStyle.Top,
                Height = 48,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var subtitulo = new Label
            {
                Text = "     Depósito principal: Julio A. Roca 651, Córdoba Capital",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(202, 224, 241),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cabecera.Controls.Add(subtitulo);
            cabecera.Controls.Add(titulo);

            // MENÚ LATERAL
            var lateral = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var encabezado = new Label
            {
                Dock = DockStyle.Top,
                Height = 55,
                Text = "PLANIFICADOR DE RUTAS",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(10, 42, 73)
            };

            var ayuda = new Label
            {
                Dock = DockStyle.Top,
                Height = 190,
                Text =
                    "Punto de partida:\n" +
                    "Imprenta Central\n" +
                    "Julio A. Roca 651, Córdoba Capital\n\n" +
                    "Próximo paso:\n" +
                    "Cargar los pedidos listos para entregar\n" +
                    "y calcular la ruta óptima.",
                ForeColor = Color.FromArgb(50, 65, 85)
            };

            estado = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                Text = "Mapa de demostración.\nSin entregas cargadas todavía.",
                ForeColor = Color.DimGray
            };

            var volver = new Button
            {
                Text = "← Volver al inicio",
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = Color.FromArgb(10, 42, 73),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            volver.Click += (s, e) =>
            {
                new Form5().Show();
                Close();
            };

            lateral.Controls.Add(ayuda);
            lateral.Controls.Add(encabezado);
            lateral.Controls.Add(estado);
            lateral.Controls.Add(volver);

            // MAPA
            mapa = new WebView2
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(mapa);
            Controls.Add(lateral);
            Controls.Add(cabecera);

            Shown += async (s, e) =>
            {
                try
                {
                    await mapa.EnsureCoreWebView2Async(null);
                    mapa.NavigateToString(HtmlMapa());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "No se pudo cargar el mapa.\n" + ex.Message,
                        "C.E SYSTEM",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            };
        }

        // EVENTO DEL DISEÑADOR
        private void Form18_Load(object sender, EventArgs e)
        {
        }

        private string HtmlMapa()
        {
            return @"<!DOCTYPE html>
<html lang='es'>
<head>
<meta charset='UTF-8'/>
<meta name='viewport'
content='width=device-width, initial-scale=1.0'/>

<link rel='stylesheet'
href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/>

<style>
html, body, #map {
    height: 100%;
    margin: 0;
}

body {
    font-family: Segoe UI, Arial, sans-serif;
}

.leaflet-popup-content {
    font-size: 14px;
}

.leyenda {
    position: absolute;
    z-index: 1000;
    right: 18px;
    top: 18px;
    background: white;
    padding: 13px 18px;
    border-radius: 10px;
    box-shadow: 0 2px 14px #0002;
    color: #0a2a49;
    font-weight: 600;
}
</style>
</head>

<body>

<div id='map'></div>

<div class='leyenda'>
📍 Depósito principal · Córdoba Capital
</div>

<script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>

<script>

// COORDENADAS EXACTAS DE IMPRENTA CENTRAL
var latitudDeposito = -31.428735594577322;
var longitudDeposito = -64.20066415582254;

// INICIALIZAR MAPA EN CÓRDOBA CAPITAL
var map = L.map('map').setView(
    [latitudDeposito, longitudDeposito],
    16
);

// MAPA MAPTILER
L.tileLayer(
    'https://api.maptiler.com/maps/streets-v4/256/{z}/{x}/{y}.png?key=RSsN9bntPwBa0KqG1M45',
    {
        maxZoom: 19,
        attribution: '&copy; MapTiler &copy; OpenStreetMap contributors'
    }
).addTo(map);

// MARCADOR DEL DEPÓSITO PRINCIPAL
var deposito = L.marker([
    latitudDeposito,
    longitudDeposito
])
.addTo(map)
.bindPopup(
    '<b>Imprenta Central</b><br>' +
    'Julio A. Roca 651<br>' +
    'Córdoba Capital, Argentina<br>' +
    'Inicio de los repartos'
);

// ABRIR INFORMACIÓN DEL DEPÓSITO
deposito.openPopup();

</script>

</body>
</html>";
        }
    }
}
