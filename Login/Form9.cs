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



namespace Login
{
    public partial class Form9 : Form
    {
        private readonly string conexion =
    @"Server=TAIGO-PC;Database=ClientesDB;Trusted_Connection=True;";

        // =====================================================
        // VARIABLES
        // =====================================================

        // Id del cliente seleccionado
        private int idClienteSeleccionado = 0;

        // Id del alquiler que estamos modificando
        private int idAlquilerModificar = 0;

        // Indica si estamos modificando o registrando
        private bool modoModificar = false;


        // =====================================================
        // CONSTRUCTOR NORMAL
        // =====================================================

        public Form9()
        {
            InitializeComponent();
        }


        // =====================================================
        // CONSTRUCTOR PARA MODIFICAR
        // =====================================================

        public Form9(int idAlquiler)
        {
            InitializeComponent();

            idAlquilerModificar = idAlquiler;
            modoModificar = true;
        }


        // =====================================================
        // CARGAR FORMULARIO
        // =====================================================

        private void Form9_Load(object sender, EventArgs e)
        {
            // =================================================
            // TIPOS DE RECURSO
            // =================================================

            cmbTipo.Items.Clear();

            cmbTipo.Items.Add("Maquinaria");
            cmbTipo.Items.Add("Espacio físico");





            // =================================================
            // FORMAS DE PAGO
            // =================================================

            cmbFormaPago.Items.Clear();

            cmbFormaPago.Items.Add("Efectivo");
            cmbFormaPago.Items.Add("Débito");
            cmbFormaPago.Items.Add("Crédito");
            cmbFormaPago.Items.Add("Transferencia");


            // =================================================
            // ESTADOS
            // =================================================

            cmbEstado.Items.Clear();

            cmbEstado.Items.Add("Reservado");
            cmbEstado.Items.Add("Activo");
            cmbEstado.Items.Add("Finalizado");
            cmbEstado.Items.Add("Cancelado");


            // =================================================
            // CONFIGURACIÓN INICIAL
            // =================================================

            cmbTipo.SelectedIndex = -1;
            cmbRecurso.SelectedIndex = -1;
            cmbFormaPago.SelectedIndex = -1;

            cmbEstado.SelectedIndex = 0;

            txtCliente.ReadOnly = true;

            dtpInicio.Value = DateTime.Today;
            dtpFin.Value = DateTime.Today;

            lblCantidad.Text = "1";
            lblTotal.Text = "$ 0,00";


            // =================================================
            // SI ES MODIFICAR
            // =================================================

            if (modoModificar)
            {
                CargarAlquiler();

                // Cambiar texto del botón si querés
                btnGuardar.Text = "Guardar cambios";
            }
            else
            {
                txtDni.Focus();
            }
        }


        // =====================================================
        // CARGAR ALQUILER PARA MODIFICAR
        // =====================================================

        private void CargarAlquiler()
        {
            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            A.IdCliente,
                            C.DNI,
                            C.Nombre,
                            A.TipoRecurso,
                            A.Recurso,
                            A.FechaInicio,
                            A.FechaFin,
                            A.FormaPago,
                            A.Estado
                        FROM Alquileres A
                        INNER JOIN Clientes C
                            ON A.IdCliente = C.Id
                        WHERE A.IdAlquiler = @idAlquiler";


                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(
                            "@idAlquiler",
                            SqlDbType.Int).Value =
                            idAlquilerModificar;


                        using (SqlDataReader reader =
                               cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // =================================
                                // CLIENTE
                                // =================================

                                idClienteSeleccionado =
                                    Convert.ToInt32(
                                        reader["IdCliente"]);

                                txtDni.Text =
                                    reader["DNI"].ToString();

                                txtCliente.Text =
                                    reader["Nombre"].ToString();


                                // =================================
                                // TIPO
                                // =================================

                                string tipo =
                                    reader["TipoRecurso"].ToString();

                                cmbTipo.SelectedItem = tipo;


                                // =================================
                                // RECURSO
                                // =================================

                                string recurso =
                                    reader["Recurso"].ToString();

                                cmbRecurso.SelectedItem =
                                    recurso;


                                // =================================
                                // FECHAS
                                // =================================

                                dtpInicio.Value =
                                    Convert.ToDateTime(
                                        reader["FechaInicio"]);

                                dtpFin.Value =
                                    Convert.ToDateTime(
                                        reader["FechaFin"]);


                                // =================================
                                // FORMA DE PAGO
                                // =================================

                                string formaPago =
                                    reader["FormaPago"].ToString();

                                cmbFormaPago.SelectedItem =
                                    formaPago;


                                // =================================
                                // ESTADO
                                // =================================

                                string estado =
                                    reader["Estado"].ToString();

                                cmbEstado.SelectedItem =
                                    estado;


                                // =================================
                                // RECALCULAR
                                // =================================

                                CalcularAlquiler();
                            }
                            else
                            {
                                MessageBox.Show(
                                    "No se encontró el alquiler.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                                this.Close();
                            }
                        }
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
        // BUSCAR CLIENTE POR DNI
        // =====================================================

        private void txtDni_Leave(
            object sender,
            EventArgs e)
        {
            BuscarCliente();
        }


        private void BuscarCliente()
        {
            string dni =
                txtDni.Text.Trim();

            if (dni == "")
            {
                txtCliente.Clear();

                idClienteSeleccionado = 0;

                return;
            }


            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT Id, Nombre
                        FROM Clientes
                        WHERE DNI = @dni";


                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(
                            "@dni",
                            SqlDbType.VarChar,
                            20).Value = dni;


                        using (SqlDataReader reader =
                               cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idClienteSeleccionado =
                                    Convert.ToInt32(
                                        reader["Id"]);

                                txtCliente.Text =
                                    reader["Nombre"].ToString();
                            }
                            else
                            {
                                idClienteSeleccionado = 0;

                                txtCliente.Clear();

                                MessageBox.Show(
                                    "No se encontró un cliente con ese DNI.",
                                    "Cliente no encontrado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al buscar el cliente:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }


        // =====================================================
        // CAMBIO DE TIPO
        // =====================================================

        private void cmbTipo_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            cmbRecurso.Items.Clear();
            cmbRecurso.SelectedIndex = -1;

            if (cmbTipo.SelectedIndex == -1)
                return;

            if (cmbTipo.Text == "Maquinaria")
            {
                CargarMaquinarias();
            }
            else if (cmbTipo.Text == "Espacio físico")
            {
                CargarEspaciosFisicos();
            }

            CalcularAlquiler();
        }

        private void CargarMaquinarias()
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT Nombre
                FROM Maquinarias
                ORDER BY Nombre";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbRecurso.Items.Add(
                                reader["Nombre"].ToString());
                        }
                    }

                    if (cmbRecurso.Items.Count == 0)
                    {
                        MessageBox.Show(
                            "No hay maquinarias registradas.",
                            "Maquinarias",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar las maquinarias:\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void CargarEspaciosFisicos()
        {
            cmbRecurso.Items.Clear();

            cmbRecurso.Items.Add("Salón");
            cmbRecurso.Items.Add("Sala de reuniones");
            cmbRecurso.Items.Add("Depósito");
        }
        // =====================================================
        // CAMBIO DE RECURSO
        // =====================================================

        private void cmbRecurso_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            CalcularAlquiler();

            if (cmbRecurso.SelectedIndex != -1)
                VerificarDisponibilidad();
        }


        // =====================================================
        // CAMBIO DE FECHA INICIO
        // =====================================================

        private void dtpInicio_ValueChanged(
    object sender,
    EventArgs e)
        {
            if (dtpFin.Value.Date < dtpInicio.Value.Date)
            {
                dtpFin.Value = dtpInicio.Value;
            }

            CalcularAlquiler();

            if (cmbRecurso.SelectedIndex != -1)
                VerificarDisponibilidad();
        }


        // =====================================================
        // CAMBIO DE FECHA FIN
        // =====================================================

        private void dtpFin_ValueChanged(
    object sender,
    EventArgs e)
        {
            if (dtpFin.Value.Date < dtpInicio.Value.Date)
            {
                dtpFin.Value = dtpInicio.Value;
            }

            CalcularAlquiler();

            if (cmbRecurso.SelectedIndex != -1)
                VerificarDisponibilidad();
        }


        // =====================================================
        // CALCULAR ALQUILER
        // =====================================================

        private void CalcularAlquiler()
        {
            int cantidadDias =
                (dtpFin.Value.Date -
                 dtpInicio.Value.Date).Days + 1;


            if (cantidadDias < 1)
                cantidadDias = 1;


            lblCantidad.Text =
                cantidadDias.ToString();


            decimal precioPorDia =
                ObtenerPrecioPorDia();


            decimal total =
                cantidadDias * precioPorDia;


            lblTotal.Text =
                total.ToString("C2");
        }


        // =====================================================
        // PRECIO POR DÍA
        // =====================================================

        private decimal ObtenerPrecioPorDia()
        {
            if (cmbRecurso.SelectedIndex == -1)
                return 0;

            string recurso = cmbRecurso.Text;

            // ============================================
            // MAQUINARIA
            // ============================================
            if (cmbTipo.Text == "Maquinaria")
            {
                using (SqlConnection conn =
                       new SqlConnection(conexion))
                {
                    try
                    {
                        conn.Open();

                        string query = @"
                    SELECT PrecioPorDia
                    FROM Maquinarias
                    WHERE Nombre = @Nombre";

                        using (SqlCommand cmd =
                               new SqlCommand(query, conn))
                        {
                            cmd.Parameters.Add(
                                "@Nombre",
                                SqlDbType.VarChar,
                                100).Value = recurso;

                            object resultado =
                                cmd.ExecuteScalar();

                            if (resultado != null &&
                                resultado != DBNull.Value)
                            {
                                return Convert.ToDecimal(resultado);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Error al obtener el precio de la maquinaria:\n" +
                            ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    return 0;
                }
            }

            // ============================================
            // ESPACIOS FÍSICOS
            // ============================================
            if (cmbTipo.Text == "Espacio físico")
            {
                switch (recurso)
                {
                    case "Salón":
                        return 20000;

                    case "Sala de reuniones":
                        return 12000;

                    case "Depósito":
                        return 15000;

                    default:
                        return 0;
                }
            }

            return 0;
        }


        // =====================================================
        // GUARDAR / MODIFICAR ALQUILER
        // =====================================================

        private void btnGuardar_Click(
            object sender,
            EventArgs e)
        {
            // =================================================
            // VALIDAR CLIENTE
            // =================================================

            if (txtDni.Text.Trim() == "")
            {
                MessageBox.Show(
                    "Ingrese el DNI del cliente.");

                txtDni.Focus();

                return;
            }


            if (idClienteSeleccionado == 0)
            {
                MessageBox.Show(
                    "El DNI ingresado no corresponde a un cliente registrado.");

                txtDni.Focus();

                return;
            }


            // =================================================
            // VALIDAR TIPO
            // =================================================

            if (cmbTipo.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione el tipo de recurso.");

                cmbTipo.Focus();

                return;
            }


            // =================================================
            // VALIDAR RECURSO
            // =================================================

            if (cmbRecurso.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione el recurso a alquilar.");

                cmbRecurso.Focus();

                return;
            }


            // =================================================
            // VALIDAR FORMA DE PAGO
            // =================================================

            if (cmbFormaPago.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione la forma de pago.");

                cmbFormaPago.Focus();

                return;
            }


            // =================================================
            // VALIDAR ESTADO
            // =================================================

            if (cmbEstado.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione el estado del alquiler.");

                cmbEstado.Focus();

                return;
            }


            // =================================================
            // VERIFICAR DISPONIBILIDAD
            // =================================================

            if (!VerificarDisponibilidad())
            {
                MessageBox.Show(
                    "No se puede guardar el alquiler porque el recurso " +
                    "ya se encuentra alquilado en ese período.",
                    "Recurso no disponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return; // MUY IMPORTANTE
            }


            // =================================================
            // DATOS
            // =================================================

            int cantidadDias =
                (dtpFin.Value.Date -
                 dtpInicio.Value.Date).Days + 1;


            decimal precioPorDia =
                ObtenerPrecioPorDia();


            decimal importeTotal =
                cantidadDias * precioPorDia;


            string tipoRecurso =
                cmbTipo.SelectedItem.ToString();


            string recurso =
                cmbRecurso.SelectedItem.ToString();


            string formaPago =
                cmbFormaPago.SelectedItem.ToString();


            string estado =
                cmbEstado.SelectedItem.ToString();


            // =================================================
            // CONEXIÓN
            // =================================================

            using (SqlConnection conn =
                   new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();


                    // =========================================
                    // MODIFICAR
                    // =========================================

                    if (modoModificar)
                    {
                        string query = @"
                            UPDATE Alquileres
                            SET
                                IdCliente = @idCliente,
                                TipoRecurso = @tipoRecurso,
                                Recurso = @recurso,
                                FechaInicio = @fechaInicio,
                                FechaFin = @fechaFin,
                                CantidadDias = @cantidadDias,
                                PrecioPorDia = @precioPorDia,
                                ImporteTotal = @importeTotal,
                                FormaPago = @formaPago,
                                Estado = @estado
                            WHERE IdAlquiler = @idAlquiler";


                        using (SqlCommand cmd =
                               new SqlCommand(query, conn))
                        {
                            cmd.Parameters.Add(
                                "@idAlquiler",
                                SqlDbType.Int).Value =
                                idAlquilerModificar;


                            cmd.Parameters.Add(
                                "@idCliente",
                                SqlDbType.Int).Value =
                                idClienteSeleccionado;


                            cmd.Parameters.Add(
                                "@tipoRecurso",
                                SqlDbType.VarChar).Value =
                                tipoRecurso;


                            cmd.Parameters.Add(
                                "@recurso",
                                SqlDbType.VarChar).Value =
                                recurso;


                            cmd.Parameters.Add(
                                "@fechaInicio",
                                SqlDbType.DateTime).Value =
                                dtpInicio.Value.Date;


                            cmd.Parameters.Add(
                                "@fechaFin",
                                SqlDbType.DateTime).Value =
                                dtpFin.Value.Date;


                            cmd.Parameters.Add(
                                "@cantidadDias",
                                SqlDbType.Int).Value =
                                cantidadDias;


                            cmd.Parameters.Add(
                                "@precioPorDia",
                                SqlDbType.Decimal).Value =
                                precioPorDia;


                            cmd.Parameters.Add(
                                "@importeTotal",
                                SqlDbType.Decimal).Value =
                                importeTotal;


                            cmd.Parameters.Add(
                                "@formaPago",
                                SqlDbType.VarChar).Value =
                                formaPago;


                            cmd.Parameters.Add(
                                "@estado",
                                SqlDbType.VarChar).Value =
                                estado;


                            cmd.ExecuteNonQuery();
                        }


                        MessageBox.Show(
                            "Alquiler modificado correctamente.",
                            "Modificación exitosa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);


                        this.Close();

                        return;
                    }


                    // =========================================
                    // REGISTRAR NUEVO
                    // =========================================

                    string insertQuery = @"
                        INSERT INTO Alquileres
                        (
                            IdCliente,
                            TipoRecurso,
                            Recurso,
                            FechaInicio,
                            FechaFin,
                            CantidadDias,
                            PrecioPorDia,
                            ImporteTotal,
                            FormaPago,
                            Estado
                        )
                        VALUES
                        (
                            @idCliente,
                            @tipoRecurso,
                            @recurso,
                            @fechaInicio,
                            @fechaFin,
                            @cantidadDias,
                            @precioPorDia,
                            @importeTotal,
                            @formaPago,
                            @estado
                        )";


                    using (SqlCommand cmd =
                           new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.Add(
                            "@idCliente",
                            SqlDbType.Int).Value =
                            idClienteSeleccionado;


                        cmd.Parameters.Add(
                            "@tipoRecurso",
                            SqlDbType.VarChar).Value =
                            tipoRecurso;


                        cmd.Parameters.Add(
                            "@recurso",
                            SqlDbType.VarChar).Value =
                            recurso;


                        cmd.Parameters.Add(
                            "@fechaInicio",
                            SqlDbType.DateTime).Value =
                            dtpInicio.Value.Date;


                        cmd.Parameters.Add(
                            "@fechaFin",
                            SqlDbType.DateTime).Value =
                            dtpFin.Value.Date;


                        cmd.Parameters.Add(
                            "@cantidadDias",
                            SqlDbType.Int).Value =
                            cantidadDias;


                        cmd.Parameters.Add(
                            "@precioPorDia",
                            SqlDbType.Decimal).Value =
                            precioPorDia;


                        cmd.Parameters.Add(
                            "@importeTotal",
                            SqlDbType.Decimal).Value =
                            importeTotal;


                        cmd.Parameters.Add(
                            "@formaPago",
                            SqlDbType.VarChar).Value =
                            formaPago;


                        cmd.Parameters.Add(
                            "@estado",
                            SqlDbType.VarChar).Value =
                            estado;


                        cmd.ExecuteNonQuery();
                    }


                    MessageBox.Show(
                        "Alquiler registrado correctamente.",
                        "Registro exitoso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);


                    LimpiarFormulario();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al guardar el alquiler:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }


        // =====================================================
        // LIMPIAR FORMULARIO
        // =====================================================

        private void LimpiarFormulario()
        {
            txtDni.Clear();

            txtCliente.Clear();

            idClienteSeleccionado = 0;

            cmbTipo.SelectedIndex = -1;

            cmbRecurso.SelectedIndex = -1;

            cmbFormaPago.SelectedIndex = -1;

            cmbEstado.SelectedIndex = 0;

            dtpInicio.Value =
                DateTime.Today;

            dtpFin.Value =
                DateTime.Today;

            lblCantidad.Text = "1";

            lblTotal.Text = "$ 0,00";

            txtDni.Focus();
        }
        private bool VerificarDisponibilidad()
        {
            // Si todavía no eligió recurso, no verificamos
            if (cmbRecurso.SelectedIndex == -1)
                return true;

            string recurso = cmbRecurso.Text;

            DateTime fechaInicio = dtpInicio.Value.Date;
            DateTime fechaFin = dtpFin.Value.Date;

            using (SqlConnection conn = new SqlConnection(conexion))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT TOP 1
                    IdAlquiler,
                    FechaInicio,
                    FechaFin
                FROM Alquileres
                WHERE Recurso = @Recurso
                  AND Estado <> 'Cancelado'
                  AND FechaInicio <= @FechaFin
                  AND FechaFin >= @FechaInicio";

                    // Si estamos MODIFICANDO, no debe compararse consigo mismo
                    if (modoModificar)
                    {
                        query += " AND IdAlquiler <> @IdAlquiler";
                    }

                    query += " ORDER BY FechaInicio";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(
                            "@Recurso",
                            SqlDbType.VarChar,
                            100).Value = recurso;

                        cmd.Parameters.Add(
                            "@FechaInicio",
                            SqlDbType.Date).Value = fechaInicio;

                        cmd.Parameters.Add(
                            "@FechaFin",
                            SqlDbType.Date).Value = fechaFin;

                        if (modoModificar)
                        {
                            cmd.Parameters.Add(
                                "@IdAlquiler",
                                SqlDbType.Int).Value = idAlquilerModificar;
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime ocupadoDesde =
                                    Convert.ToDateTime(reader["FechaInicio"]);

                                DateTime ocupadoHasta =
                                    Convert.ToDateTime(reader["FechaFin"]);

                                lblDisponibilidad.Text =
                                    "⚠ NO DISPONIBLE - Ocupado del " +
                                    ocupadoDesde.ToString("dd/MM/yyyy") +
                                    " al " +
                                    ocupadoHasta.ToString("dd/MM/yyyy");

                                lblDisponibilidad.ForeColor =
                                    Color.DarkRed;

                                return false;
                            }
                        }
                    }

                    lblDisponibilidad.Text =
                        "✓ Disponible para el período seleccionado";

                    lblDisponibilidad.ForeColor =
                        Color.DarkGreen;

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al verificar disponibilidad:\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
                }
            }
        }

        // =====================================================
        // CANCELAR
        // =====================================================

        private void btnCancelar_Click(
            object sender,
            EventArgs e)
        {
            this.Close();
        }


        // =====================================================
        // VOLVER
        // =====================================================

        private void btnVolver_Click(
            object sender,
            EventArgs e)
        {
            Form5 frm = new Form5();

            frm.Show();

            this.Close();
        }

        
    }
}
