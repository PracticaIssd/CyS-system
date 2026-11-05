using DocumentFormat.OpenXml.Office.Word;
using iTextSharp.text;
using iTextSharp.text.pdf;
 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Login
{
    public partial class Form12 : Form
    {
        // =====================================================
        // CONEXIÓN
        // =====================================================

        private readonly string conexion =
    @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";

        // =====================================================
        // VARIABLE QUE RECIBE EL ID DE LA FALLA
        // =====================================================

        private int idFalla;
        private int idOrden = 0;
        private bool ordenExistente = false;

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public Form12(int idFallaSeleccionada)
        {
            InitializeComponent();

            idFalla = idFallaSeleccionada;
        }

        // =====================================================
        // LOAD DEL FORMULARIO
        // =====================================================

        private void Form12_Load(object sender, EventArgs e)
        {
            CargarFalla();
            CargarTecnicos();
            CargarEstadosOrden();
            ConfigurarFechas();

            // Busca si esta falla ya tiene una OT
            CargarOrdenExistente();
        }
        private void CargarOrdenExistente()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT TOP 1
                    IdOrden,
                    IdTecnico,
                    FechaInicio,
                    FechaFin,
                    DescripcionTrabajo,
                    Resolucion,
                    Estado
                FROM OrdenesTrabajo
                WHERE IdFalla = @IdFalla
                ORDER BY IdOrden DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IdFalla", idFalla);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ordenExistente = true;

                                idOrden =
                                    Convert.ToInt32(reader["IdOrden"]);

                                lblNumeroOT.Text =
                                    "Nº OT-" +
                                    idOrden.ToString("0000");

                                if (reader["IdTecnico"] != DBNull.Value)
                                {
                                    cmbTecnicoOT.SelectedValue =
                                        Convert.ToInt32(
                                            reader["IdTecnico"]);
                                }

                                if (reader["FechaInicio"] != DBNull.Value)
                                {
                                    dtpFechaInicioOT.Value =
                                        Convert.ToDateTime(
                                            reader["FechaInicio"]);
                                }

                                if (reader["FechaFin"] != DBNull.Value)
                                {
                                    dtpFechaFinOT.Enabled = true;
                                    dtpFechaFinOT.Checked = true;

                                    dtpFechaFinOT.Value =
                                        Convert.ToDateTime(
                                            reader["FechaFin"]);
                                }
                                else
                                {
                                    dtpFechaFinOT.Checked = false;
                                }

                                txtTrabajoRealizadoOT.Text =
                                    reader["DescripcionTrabajo"] == DBNull.Value
                                    ? ""
                                    : reader["DescripcionTrabajo"].ToString();

                                txtResolucionOT.Text =
                                    reader["Resolucion"] == DBNull.Value
                                    ? ""
                                    : reader["Resolucion"].ToString();

                                cmbEstadoOrdenOT.Text =
                                    reader["Estado"].ToString();

                                btnGuardarOrden.Text =
                                    "ACTUALIZAR ORDEN DE TRABAJO";
                            }
                            else
                            {
                                // No existe OT: es una nueva
                                ordenExistente = false;
                                idOrden = 0;

                                lblNumeroOT.Text = "Nº OT-NUEVA";

                                btnGuardarOrden.Text =
                                    "GUARDAR ORDEN DE TRABAJO";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar la orden de trabajo:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        // =====================================================
        // CONFIGURAR FECHAS
        // =====================================================

        private void ConfigurarFechas()
        {
            // Fecha de inicio
            dtpFechaInicioOT.Enabled = true;
            dtpFechaInicioOT.Value = DateTime.Today;

            // Fecha final opcional
            dtpFechaFinOT.Enabled = false;
            dtpFechaFinOT.ShowCheckBox = true;
            dtpFechaFinOT.Checked = false;
            dtpFechaFinOT.Value = DateTime.Today;
        }

        // =====================================================
        // CARGAR INFORMACIÓN DE LA FALLA
        // =====================================================

        private void CargarFalla()
        {
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
                        WHERE F.IdFalla = @IdFalla";

                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdFalla",
                            idFalla);

                        using (SqlDataReader reader =
                               cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtMaquinariaOT.Text =
                                    reader["Maquinaria"].ToString();

                                dtpFechaFallaOT.Value =
                                    Convert.ToDateTime(
                                        reader["FechaFalla"]);

                                
                                txtDescripcionFallaOT.Text =
                                    reader["Descripcion"].ToString();

                                lblNumeroOT.Text =
                                    "Nº OT-" +
                                    idFalla.ToString("0000");
                            }
                            else
                            {
                                MessageBox.Show(
                                    "No se encontró la falla seleccionada.",
                                    "Falla no encontrada",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);

                                this.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar la información de la falla:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =====================================================
        // CARGAR TÉCNICOS
        // =====================================================

        private void CargarTecnicos()
        {
            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            IdTecnico,
                            Nombre
                        FROM Tecnicos
                        WHERE Estado <> 'Inactivo'
                        ORDER BY Nombre";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);

                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    cmbTecnicoOT.DataSource = dt;

                    cmbTecnicoOT.DisplayMember =
                        "Nombre";

                    cmbTecnicoOT.ValueMember =
                        "IdTecnico";

                    cmbTecnicoOT.SelectedIndex = -1;
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

        // =====================================================
        // ESTADOS DE LA ORDEN
        // =====================================================

        private void CargarEstadosOrden()
        {
            cmbEstadoOrdenOT.Items.Clear();

            cmbEstadoOrdenOT.Items.Add(
                "Pendiente");

            cmbEstadoOrdenOT.Items.Add(
                "En reparación");

            cmbEstadoOrdenOT.Items.Add(
                "Resuelta");

            cmbEstadoOrdenOT.SelectedIndex = 0;
        }

        // =====================================================
        // GUARDAR ORDEN
        // =====================================================

        private void btnGuardarOrden_Click(
    object sender,
    EventArgs e)
        {
            // =====================================================
            // VALIDACIONES
            // =====================================================

            if (cmbTecnicoOT.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione un técnico responsable.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbTecnicoOT.Focus();
                return;
            }

            if (cmbEstadoOrdenOT.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione el estado de la orden.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(txtTrabajoRealizadoOT.Text))
            {
                MessageBox.Show(
                    "Ingrese el trabajo realizado o a realizar.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtTrabajoRealizadoOT.Focus();
                return;
            }

            // =====================================================
            // VALIDAR FECHAS
            // =====================================================

            if (dtpFechaFinOT.Checked &&
                dtpFechaFinOT.Value.Date <
                dtpFechaInicioOT.Value.Date)
            {
                MessageBox.Show(
                    "La fecha de finalización no puede ser anterior a la fecha de inicio.",
                    "Fecha incorrecta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Si está resuelta, debe tener fecha final y resolución
            if (cmbEstadoOrdenOT.Text == "Resuelta")
            {
                if (!dtpFechaFinOT.Checked)
                {
                    MessageBox.Show(
                        "Si la orden está resuelta debe indicar la fecha de finalización.",
                        "Fecha requerida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (string.IsNullOrWhiteSpace(txtResolucionOT.Text))
                {
                    MessageBox.Show(
                        "Ingrese la resolución de la falla.",
                        "Resolución requerida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtResolucionOT.Focus();
                    return;
                }
            }

            // =====================================================
            // BASE DE DATOS
            // =====================================================

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    SqlTransaction transaction =
                        conn.BeginTransaction();

                    try
                    {
                        // =================================================
                        // SI NO EXISTE -> INSERT
                        // =================================================

                        if (!ordenExistente)
                        {
                            string queryOrden = @"
                        INSERT INTO OrdenesTrabajo
                        (
                            IdFalla,
                            IdTecnico,
                            FechaInicio,
                            FechaFin,
                            DescripcionTrabajo,
                            Resolucion,
                            Estado
                        )
                        VALUES
                        (
                            @IdFalla,
                            @IdTecnico,
                            @FechaInicio,
                            @FechaFin,
                            @DescripcionTrabajo,
                            @Resolucion,
                            @Estado
                        );

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            using (SqlCommand cmd =
                                   new SqlCommand(
                                       queryOrden,
                                       conn,
                                       transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@IdFalla",
                                    idFalla);

                                cmd.Parameters.AddWithValue(
                                    "@IdTecnico",
                                    Convert.ToInt32(
                                        cmbTecnicoOT.SelectedValue));

                                cmd.Parameters.AddWithValue(
                                    "@FechaInicio",
                                    dtpFechaInicioOT.Value.Date);

                                if (dtpFechaFinOT.Checked)
                                {
                                    cmd.Parameters.AddWithValue(
                                        "@FechaFin",
                                        dtpFechaFinOT.Value.Date);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue(
                                        "@FechaFin",
                                        DBNull.Value);
                                }

                                cmd.Parameters.AddWithValue(
                                    "@DescripcionTrabajo",
                                    txtTrabajoRealizadoOT.Text.Trim());

                                if (string.IsNullOrWhiteSpace(
                                    txtResolucionOT.Text))
                                {
                                    cmd.Parameters.AddWithValue(
                                        "@Resolucion",
                                        DBNull.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue(
                                        "@Resolucion",
                                        txtResolucionOT.Text.Trim());
                                }

                                cmd.Parameters.AddWithValue(
                                    "@Estado",
                                    cmbEstadoOrdenOT.Text);

                                // Guardamos el ID REAL de la nueva OT
                                idOrden =
                                    Convert.ToInt32(
                                        cmd.ExecuteScalar());
                            }

                            ordenExistente = true;
                        }

                        // =================================================
                        // SI YA EXISTE -> UPDATE
                        // =================================================

                        else
                        {
                            string queryOrden = @"
                        UPDATE OrdenesTrabajo
                        SET
                            IdTecnico = @IdTecnico,
                            FechaInicio = @FechaInicio,
                            FechaFin = @FechaFin,
                            DescripcionTrabajo = @DescripcionTrabajo,
                            Resolucion = @Resolucion,
                            Estado = @Estado
                        WHERE IdOrden = @IdOrden";

                            using (SqlCommand cmd =
                                   new SqlCommand(
                                       queryOrden,
                                       conn,
                                       transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@IdTecnico",
                                    Convert.ToInt32(
                                        cmbTecnicoOT.SelectedValue));

                                cmd.Parameters.AddWithValue(
                                    "@FechaInicio",
                                    dtpFechaInicioOT.Value.Date);

                                if (dtpFechaFinOT.Checked)
                                {
                                    cmd.Parameters.AddWithValue(
                                        "@FechaFin",
                                        dtpFechaFinOT.Value.Date);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue(
                                        "@FechaFin",
                                        DBNull.Value);
                                }

                                cmd.Parameters.AddWithValue(
                                    "@DescripcionTrabajo",
                                    txtTrabajoRealizadoOT.Text.Trim());

                                if (string.IsNullOrWhiteSpace(
                                    txtResolucionOT.Text))
                                {
                                    cmd.Parameters.AddWithValue(
                                        "@Resolucion",
                                        DBNull.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue(
                                        "@Resolucion",
                                        txtResolucionOT.Text.Trim());
                                }

                                cmd.Parameters.AddWithValue(
                                    "@Estado",
                                    cmbEstadoOrdenOT.Text);

                                cmd.Parameters.AddWithValue(
                                    "@IdOrden",
                                    idOrden);

                                cmd.ExecuteNonQuery();
                            }
                        }


                        // =================================================
                        // ACTUALIZAR ESTADO DE LA FALLA
                        // =================================================

                        string queryFalla = @"
                    UPDATE Fallas
                    SET Estado = @Estado
                    WHERE IdFalla = @IdFalla";

                        using (SqlCommand cmd =
                               new SqlCommand(
                                   queryFalla,
                                   conn,
                                   transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@Estado",
                                cmbEstadoOrdenOT.Text);

                            cmd.Parameters.AddWithValue(
                                "@IdFalla",
                                idFalla);

                            cmd.ExecuteNonQuery();
                        }


                        // =================================================
                        // ACTUALIZAR MAQUINARIA
                        // =================================================

                        string estadoMaquinaria;

                        if (cmbEstadoOrdenOT.Text == "Resuelta")
                        {
                            estadoMaquinaria = "Disponible";
                        }
                        else
                        {
                            estadoMaquinaria = "En mantenimiento";
                        }

                        string queryMaquinaria = @"
                    UPDATE Maquinarias
                    SET Estado = @EstadoMaquinaria
                    WHERE IdMaquinaria =
                    (
                        SELECT IdMaquinaria
                        FROM Fallas
                        WHERE IdFalla = @IdFalla
                    )";

                        using (SqlCommand cmd =
                               new SqlCommand(
                                   queryMaquinaria,
                                   conn,
                                   transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@EstadoMaquinaria",
                                estadoMaquinaria);

                            cmd.Parameters.AddWithValue(
                                "@IdFalla",
                                idFalla);

                            cmd.ExecuteNonQuery();
                        }


                        // =================================================
                        // ACTUALIZAR TÉCNICO
                        // =================================================

                        string estadoTecnico;

                        if (cmbEstadoOrdenOT.Text == "Resuelta")
                        {
                            estadoTecnico = "Disponible";
                        }
                        else
                        {
                            estadoTecnico = "Ocupado";
                        }

                        string queryTecnico = @"
                    UPDATE Tecnicos
                    SET Estado = @EstadoTecnico
                    WHERE IdTecnico = @IdTecnico";

                        using (SqlCommand cmd =
                               new SqlCommand(
                                   queryTecnico,
                                   conn,
                                   transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@EstadoTecnico",
                                estadoTecnico);

                            cmd.Parameters.AddWithValue(
                                "@IdTecnico",
                                Convert.ToInt32(
                                    cmbTecnicoOT.SelectedValue));

                            cmd.ExecuteNonQuery();
                        }


                        // =================================================
                        // CONFIRMAR
                        // =================================================

                        transaction.Commit();


                        // Mostrar número real
                        lblNumeroOT.Text =
                            "Nº OT-" +
                            idOrden.ToString("0000");


                        // Generar PDF con la misma OT
                        GenerarPDFOrdenTrabajo(idOrden);


                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        MessageBox.Show(
                            "No se pudo guardar la orden de trabajo:\n" +
                            ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error de conexión:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =====================================================
        // CANCELAR
        // =====================================================

        private void btnCancelarOrden_Click(
            object sender,
            EventArgs e)
        {
            DialogResult respuesta =
                MessageBox.Show(
                    "¿Desea cancelar la carga de la orden de trabajo?",
                    "Cancelar orden",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                this.Close();
            }
        }

        // =====================================================
        // CUANDO CAMBIA EL ESTADO
        // =====================================================

        private void cmbEstadoOrdenOT_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            bool resuelta =
                cmbEstadoOrdenOT.Text == "Resuelta";

            dtpFechaFinOT.Enabled = resuelta;
            dtpFechaFinOT.Checked = resuelta;

            txtResolucionOT.Enabled = resuelta;

            if (resuelta)
            {
                dtpFechaFinOT.Value = DateTime.Today;
            }
            else
            {
                dtpFechaFinOT.Checked = false;
                txtResolucionOT.Clear();
            }
        }



        private void GenerarPDFOrdenTrabajo(int idOrden)
        {
            SaveFileDialog guardar = new SaveFileDialog();

            guardar.Filter = "Archivo PDF (*.pdf)|*.pdf";
            guardar.FileName = "OT-" + idOrden.ToString("0000") + ".pdf";
            guardar.Title = "Guardar Orden de Trabajo";

            if (guardar.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                Document documento =
                    new Document(PageSize.A4, 45, 45, 40, 40);

                PdfWriter.GetInstance(
                    documento,
                    new FileStream(
                        guardar.FileName,
                        FileMode.Create));

                documento.Open();


                // =====================================================
                // COLORES
                // =====================================================

                BaseColor azulOscuro =
                    new BaseColor(8, 43, 82);

                BaseColor celeste =
                    new BaseColor(0, 177, 177);

                BaseColor grisClaro =
                    new BaseColor(240, 243, 246);

                BaseColor grisTexto =
                    new BaseColor(80, 80, 80);


                // =====================================================
                // FUENTES
                // =====================================================

                iTextSharp.text.Font fuenteTitulo =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA_BOLD,
                        18,
                        BaseColor.WHITE);

                iTextSharp.text.Font fuenteNumero =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA_BOLD,
                        13,
                        azulOscuro);

                iTextSharp.text.Font fuenteSeccion =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA_BOLD,
                        11,
                        BaseColor.WHITE);

                iTextSharp.text.Font fuenteEtiqueta =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA_BOLD,
                        10,
                        azulOscuro);

                iTextSharp.text.Font fuenteNormal =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA,
                        10,
                        grisTexto);

                iTextSharp.text.Font fuenteGrande =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA,
                        11,
                        grisTexto);


                // =====================================================
                // ENCABEZADO: LOGO + NÚMERO DE ORDEN
                // =====================================================

                PdfPTable encabezado =
                    new PdfPTable(2);

                encabezado.WidthPercentage = 100;

                encabezado.SetWidths(
                    new float[] { 50f, 50f });


                // -------------------------
                // LOGO
                // -------------------------

                PdfPCell celdaLogo =
                    new PdfPCell();

                celdaLogo.Border =
                    iTextSharp.text.Rectangle.NO_BORDER;

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

                        // FORZAMOS el tamaño real
                        logo.ScaleAbsoluteWidth(90f);
                        logo.ScaleAbsoluteHeight(64f);

                        logo.Alignment =
                            iTextSharp.text.Image.ALIGN_LEFT;

                        // IMPORTANTE:
                        // NO usar celdaLogo.Image = logo;
                        celdaLogo.AddElement(logo);

                        celdaLogo.PaddingLeft = 5;
                        celdaLogo.PaddingTop = 3;
                        celdaLogo.PaddingBottom = 3;

                        celdaLogo.VerticalAlignment =
                            Element.ALIGN_MIDDLE;
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

                encabezado.AddCell(celdaLogo);


                // -------------------------
                // NÚMERO Y FECHA
                // -------------------------

                PdfPCell celdaNumero =
                    new PdfPCell();

                celdaNumero.Border =
                    iTextSharp.text.Rectangle.NO_BORDER;

                celdaNumero.HorizontalAlignment =
                    Element.ALIGN_RIGHT;

                celdaNumero.VerticalAlignment =
                    Element.ALIGN_MIDDLE;


                Paragraph numero =
                    new Paragraph(
                        "OT-" +
                        idOrden.ToString("0000"),
                        fuenteNumero);

                numero.Alignment =
                    Element.ALIGN_RIGHT;


                Paragraph fechaEmision =
                    new Paragraph(
                        "Fecha de emisión\n" +
                        DateTime.Now.ToString(
                            "dd/MM/yyyy"),
                        fuenteNormal);

                fechaEmision.Alignment =
                    Element.ALIGN_RIGHT;


                celdaNumero.AddElement(numero);
                celdaNumero.AddElement(fechaEmision);

                encabezado.AddCell(celdaNumero);

                documento.Add(encabezado);


                documento.Add(
                    new Paragraph(" "));


                // =====================================================
                // TÍTULO PRINCIPAL
                // =====================================================

                PdfPTable tablaTitulo =
                    new PdfPTable(1);

                tablaTitulo.WidthPercentage = 100;


                PdfPCell celdaTitulo =
                    new PdfPCell(
                        new Phrase(
                            "ORDEN DE TRABAJO",
                            fuenteTitulo));

                celdaTitulo.BackgroundColor =
                    azulOscuro;

                celdaTitulo.HorizontalAlignment =
                    Element.ALIGN_CENTER;

                celdaTitulo.VerticalAlignment =
                    Element.ALIGN_MIDDLE;

                celdaTitulo.PaddingTop = 10;
                celdaTitulo.PaddingBottom = 10;

                celdaTitulo.Border =
                    iTextSharp.text.Rectangle.NO_BORDER;


                tablaTitulo.AddCell(celdaTitulo);

                documento.Add(tablaTitulo);


                documento.Add(
                    new Paragraph(" "));


                // =====================================================
                // INFORMACIÓN DE LA FALLA
                // =====================================================

                AgregarTituloSeccion(
                    documento,
                    "INFORMACIÓN DE LA FALLA",
                    azulOscuro,
                    fuenteSeccion);


                PdfPTable tablaFalla =
                    new PdfPTable(2);

                tablaFalla.WidthPercentage = 100;

                tablaFalla.SetWidths(
                    new float[] { 30f, 70f });


                AgregarFila(
                    tablaFalla,
                    "Maquinaria",
                    txtMaquinariaOT.Text,
                    fuenteEtiqueta,
                    fuenteNormal,
                    grisClaro);

                AgregarFila(
                    tablaFalla,
                    "Fecha de falla",
                    dtpFechaFallaOT.Value
                        .ToString("dd/MM/yyyy"),
                    fuenteEtiqueta,
                    fuenteNormal,
                    grisClaro);


                AgregarFila(
                    tablaFalla,
                    "Descripción",
                    txtDescripcionFallaOT.Text,
                    fuenteEtiqueta,
                    fuenteNormal,
                    grisClaro);


                documento.Add(tablaFalla);


                documento.Add(
                    new Paragraph(" "));


                // =====================================================
                // DATOS DEL MANTENIMIENTO
                // =====================================================

                AgregarTituloSeccion(
                    documento,
                    "DATOS DEL MANTENIMIENTO",
                    azulOscuro,
                    fuenteSeccion);


                PdfPTable tablaTrabajo =
                    new PdfPTable(2);

                tablaTrabajo.WidthPercentage = 100;

                tablaTrabajo.SetWidths(
                    new float[] { 30f, 70f });


                AgregarFila(
                    tablaTrabajo,
                    "Técnico responsable",
                    cmbTecnicoOT.Text,
                    fuenteEtiqueta,
                    fuenteNormal,
                    grisClaro);

                AgregarFila(
                    tablaTrabajo,
                    "Fecha de inicio",
                    dtpFechaInicioOT.Value
                        .ToString("dd/MM/yyyy"),
                    fuenteEtiqueta,
                    fuenteNormal,
                    grisClaro);


                string fechaFin =
                    dtpFechaFinOT.Checked
                    ? dtpFechaFinOT.Value
                        .ToString("dd/MM/yyyy")
                    : "Pendiente";


                AgregarFila(
                    tablaTrabajo,
                    "Fecha de finalización",
                    fechaFin,
                    fuenteEtiqueta,
                    fuenteNormal,
                    grisClaro);

                AgregarFila(
                    tablaTrabajo,
                    "Estado de la orden",
                    cmbEstadoOrdenOT.Text,
                    fuenteEtiqueta,
                    fuenteNormal,
                    grisClaro);


                documento.Add(tablaTrabajo);


                documento.Add(
                    new Paragraph(" "));


                // =====================================================
                // TRABAJO REALIZADO
                // =====================================================

                AgregarTituloSeccion(
                    documento,
                    "TRABAJO REALIZADO / A REALIZAR",
                    celeste,
                    fuenteSeccion);


                PdfPTable tablaDescripcion =
                    new PdfPTable(1);

                tablaDescripcion.WidthPercentage = 100;


                PdfPCell celdaDescripcion =
                    new PdfPCell(
                        new Phrase(
                            txtTrabajoRealizadoOT.Text,
                            fuenteGrande));

                celdaDescripcion.Padding = 12;
                celdaDescripcion.MinimumHeight = 60;

                celdaDescripcion.BorderColor =
                    new BaseColor(210, 210, 210);


                tablaDescripcion.AddCell(
                    celdaDescripcion);

                documento.Add(
                    tablaDescripcion);


                documento.Add(
                    new Paragraph(" "));


                // =====================================================
                // RESOLUCIÓN
                // =====================================================

                AgregarTituloSeccion(
                    documento,
                    "RESOLUCIÓN",
                    celeste,
                    fuenteSeccion);


                string resolucion =
                    string.IsNullOrWhiteSpace(
                        txtResolucionOT.Text)
                    ? "Pendiente"
                    : txtResolucionOT.Text;


                PdfPTable tablaResolucion =
                    new PdfPTable(1);

                tablaResolucion.WidthPercentage = 100;


                PdfPCell celdaResolucion =
                    new PdfPCell(
                        new Phrase(
                            resolucion,
                            fuenteGrande));

                celdaResolucion.Padding = 12;
                celdaResolucion.MinimumHeight = 60;

                celdaResolucion.BorderColor =
                    new BaseColor(210, 210, 210);


                tablaResolucion.AddCell(
                    celdaResolucion);

                documento.Add(
                    tablaResolucion);


                documento.Add(
                    new Paragraph("\n\n"));


                // =====================================================
                // FIRMAS
                // =====================================================

                PdfPTable tablaFirmas =
                    new PdfPTable(2);

                tablaFirmas.WidthPercentage = 100;

                tablaFirmas.SetWidths(
                    new float[] { 50f, 50f });


                PdfPCell firmaTecnico =
                    new PdfPCell(
                        new Phrase(
                            "\n\n____________________________\n" +
                            "Firma del técnico\n" +
                            cmbTecnicoOT.Text,
                            fuenteNormal));

                firmaTecnico.HorizontalAlignment =
                    Element.ALIGN_CENTER;

                firmaTecnico.Border =
                    iTextSharp.text.Rectangle.NO_BORDER;


                PdfPCell firmaResponsable =
                    new PdfPCell(
                        new Phrase(
                            "\n\n____________________________\n" +
                            "Firma del responsable",
                            fuenteNormal));

                firmaResponsable.HorizontalAlignment =
                    Element.ALIGN_CENTER;

                firmaResponsable.Border =
                    iTextSharp.text.Rectangle.NO_BORDER;


                tablaFirmas.AddCell(
                    firmaTecnico);

                tablaFirmas.AddCell(
                    firmaResponsable);


                documento.Add(tablaFirmas);


                // =====================================================
                // PIE
                // =====================================================

                documento.Add(
                    new Paragraph("\n"));


                Paragraph pie =
                    new Paragraph(
                        "Imprenta Central • Orden de Trabajo OT-" +
                        idOrden.ToString("0000"),
                        fuenteNormal);

                pie.Alignment =
                    Element.ALIGN_CENTER;

                documento.Add(pie);


                documento.Close();


                MessageBox.Show(
                    "Orden de trabajo guardada y PDF generado correctamente.",
                    "Orden de trabajo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "La orden se guardó, pero ocurrió un error al generar el PDF:\n" +
                    ex.Message,
                    "Error PDF",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void AgregarTituloSeccion(
    Document documento,
    string texto,
    BaseColor color,
    iTextSharp.text.Font fuente)
        {
            PdfPTable tabla =
                new PdfPTable(1);

            tabla.WidthPercentage = 100;

            PdfPCell celda =
                new PdfPCell(
                    new Phrase(
                        texto,
                        fuente));

            celda.BackgroundColor = color;
            celda.PaddingTop = 7;
            celda.PaddingBottom = 7;
            celda.PaddingLeft = 10;

            celda.Border =
                iTextSharp.text.Rectangle.NO_BORDER;

            tabla.AddCell(celda);

            documento.Add(tabla);
        }
        private void AgregarFila(
    PdfPTable tabla,
    string etiqueta,
    string valor,
    iTextSharp.text.Font fuenteEtiqueta,
    iTextSharp.text.Font fuenteValor,
    BaseColor fondoEtiqueta)
        {
            PdfPCell celdaEtiqueta =
                new PdfPCell(
                    new Phrase(
                        etiqueta,
                        fuenteEtiqueta));

            celdaEtiqueta.BackgroundColor =
                fondoEtiqueta;

            celdaEtiqueta.Padding = 8;

            celdaEtiqueta.BorderColor =
                new BaseColor(220, 220, 220);


            PdfPCell celdaValor =
                new PdfPCell(
                    new Phrase(
                        valor,
                        fuenteValor));

            celdaValor.Padding = 8;

            celdaValor.BorderColor =
                new BaseColor(220, 220, 220);


            tabla.AddCell(celdaEtiqueta);
            tabla.AddCell(celdaValor);
        }
    }
}