namespace Login
{
    partial class Form15
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form15));
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.btnVolver = new System.Windows.Forms.Button();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.tabControlMateriales = new System.Windows.Forms.TabControl();
            this.tabRegistrarMaterial = new System.Windows.Forms.TabPage();
            this.btnGuardarMaterial = new System.Windows.Forms.Button();
            this.btnLimpiarMaterial = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDescripcionMaterial = new System.Windows.Forms.TextBox();
            this.nudStockMinimo = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudStockInicial = new System.Windows.Forms.NumericUpDown();
            this.cmbUnidadMedida = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNombreMaterial = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.tabConsultarMateriales = new System.Windows.Forms.TabPage();
            this.dgvStockMateriales = new System.Windows.Forms.DataGridView();
            this.btnMostrarTodosMateriales = new System.Windows.Forms.Button();
            this.btnBuscarMaterial = new System.Windows.Forms.Button();
            this.txtBuscarMaterial = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.tabHistorialMovimientos = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.dgvHistorialMovimientos = new System.Windows.Forms.DataGridView();
            this.btnActualizarHistorial = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnFiltrarHistorial = new System.Windows.Forms.Button();
            this.dtpFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlEncabezado.SuspendLayout();
            this.tabControlMateriales.SuspendLayout();
            this.tabRegistrarMaterial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockMinimo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockInicial)).BeginInit();
            this.panel3.SuspendLayout();
            this.tabConsultarMateriales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockMateriales)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabHistorialMovimientos.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialMovimientos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlEncabezado
            // 
            this.pnlEncabezado.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(38)))), ((int)(((byte)(74)))));
            this.pnlEncabezado.CausesValidation = false;
            this.pnlEncabezado.Controls.Add(this.btnVolver);
            this.pnlEncabezado.Controls.Add(this.lblSubtitulo);
            this.pnlEncabezado.Controls.Add(this.lblTitulo);
            this.pnlEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEncabezado.Location = new System.Drawing.Point(0, 0);
            this.pnlEncabezado.Name = "pnlEncabezado";
            this.pnlEncabezado.Size = new System.Drawing.Size(849, 84);
            this.pnlEncabezado.TabIndex = 5;
            // 
            // btnVolver
            // 
            this.btnVolver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVolver.AutoSize = true;
            this.btnVolver.BackColor = System.Drawing.Color.White;
            this.btnVolver.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(169)))), ((int)(((byte)(78)))));
            this.btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolver.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolver.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(127)))), ((int)(((byte)(42)))));
            this.btnVolver.Location = new System.Drawing.Point(717, 9);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(120, 45);
            this.btnVolver.TabIndex = 13;
            this.btnVolver.Text = "← Volver";
            this.btnVolver.UseVisualStyleBackColor = false;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(220)))), ((int)(((byte)(235)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(12, 46);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(0, 17);
            this.lblSubtitulo.TabIndex = 1;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(8, 17);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(341, 37);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "GESTIÓN DE MATERIALES";
            this.lblTitulo.Click += new System.EventHandler(this.lblTitulo_Click);
            // 
            // tabControlMateriales
            // 
            this.tabControlMateriales.Controls.Add(this.tabRegistrarMaterial);
            this.tabControlMateriales.Controls.Add(this.tabConsultarMateriales);
            this.tabControlMateriales.Controls.Add(this.tabHistorialMovimientos);
            this.tabControlMateriales.Location = new System.Drawing.Point(15, 103);
            this.tabControlMateriales.Name = "tabControlMateriales";
            this.tabControlMateriales.SelectedIndex = 0;
            this.tabControlMateriales.Size = new System.Drawing.Size(822, 470);
            this.tabControlMateriales.TabIndex = 6;
            // 
            // tabRegistrarMaterial
            // 
            this.tabRegistrarMaterial.Controls.Add(this.btnGuardarMaterial);
            this.tabRegistrarMaterial.Controls.Add(this.btnLimpiarMaterial);
            this.tabRegistrarMaterial.Controls.Add(this.label4);
            this.tabRegistrarMaterial.Controls.Add(this.txtDescripcionMaterial);
            this.tabRegistrarMaterial.Controls.Add(this.nudStockMinimo);
            this.tabRegistrarMaterial.Controls.Add(this.label3);
            this.tabRegistrarMaterial.Controls.Add(this.label2);
            this.tabRegistrarMaterial.Controls.Add(this.nudStockInicial);
            this.tabRegistrarMaterial.Controls.Add(this.cmbUnidadMedida);
            this.tabRegistrarMaterial.Controls.Add(this.label1);
            this.tabRegistrarMaterial.Controls.Add(this.txtNombreMaterial);
            this.tabRegistrarMaterial.Controls.Add(this.label11);
            this.tabRegistrarMaterial.Controls.Add(this.panel3);
            this.tabRegistrarMaterial.Controls.Add(this.pictureBox1);
            this.tabRegistrarMaterial.Location = new System.Drawing.Point(4, 22);
            this.tabRegistrarMaterial.Name = "tabRegistrarMaterial";
            this.tabRegistrarMaterial.Padding = new System.Windows.Forms.Padding(3);
            this.tabRegistrarMaterial.Size = new System.Drawing.Size(814, 444);
            this.tabRegistrarMaterial.TabIndex = 0;
            this.tabRegistrarMaterial.Text = "Registrar material";
            this.tabRegistrarMaterial.UseVisualStyleBackColor = true;
            // 
            // btnGuardarMaterial
            // 
            this.btnGuardarMaterial.BackColor = System.Drawing.Color.SteelBlue;
            this.btnGuardarMaterial.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.btnGuardarMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardarMaterial.Location = new System.Drawing.Point(295, 390);
            this.btnGuardarMaterial.Name = "btnGuardarMaterial";
            this.btnGuardarMaterial.Size = new System.Drawing.Size(165, 23);
            this.btnGuardarMaterial.TabIndex = 41;
            this.btnGuardarMaterial.Text = "GUARDAR MATERIAL";
            this.btnGuardarMaterial.UseVisualStyleBackColor = false;
            this.btnGuardarMaterial.Click += new System.EventHandler(this.btnGuardarMaterial_Click);
            // 
            // btnLimpiarMaterial
            // 
            this.btnLimpiarMaterial.BackColor = System.Drawing.Color.Silver;
            this.btnLimpiarMaterial.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnLimpiarMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiarMaterial.Location = new System.Drawing.Point(158, 390);
            this.btnLimpiarMaterial.Name = "btnLimpiarMaterial";
            this.btnLimpiarMaterial.Size = new System.Drawing.Size(108, 23);
            this.btnLimpiarMaterial.TabIndex = 40;
            this.btnLimpiarMaterial.Text = "LIMPIAR";
            this.btnLimpiarMaterial.UseVisualStyleBackColor = false;
            this.btnLimpiarMaterial.Click += new System.EventHandler(this.btnLimpiarMaterial_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(12, 261);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(177, 20);
            this.label4.TabIndex = 39;
            this.label4.Text = "Descripción del material";
            // 
            // txtDescripcionMaterial
            // 
            this.txtDescripcionMaterial.BackColor = System.Drawing.Color.LightCyan;
            this.txtDescripcionMaterial.Location = new System.Drawing.Point(16, 284);
            this.txtDescripcionMaterial.Multiline = true;
            this.txtDescripcionMaterial.Name = "txtDescripcionMaterial";
            this.txtDescripcionMaterial.Size = new System.Drawing.Size(541, 89);
            this.txtDescripcionMaterial.TabIndex = 38;
            this.txtDescripcionMaterial.TextChanged += new System.EventHandler(this.txtDescripcionMaterial_TextChanged);
            // 
            // nudStockMinimo
            // 
            this.nudStockMinimo.DecimalPlaces = 2;
            this.nudStockMinimo.Location = new System.Drawing.Point(328, 209);
            this.nudStockMinimo.Name = "nudStockMinimo";
            this.nudStockMinimo.Size = new System.Drawing.Size(229, 20);
            this.nudStockMinimo.TabIndex = 37;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(324, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 20);
            this.label3.TabIndex = 36;
            this.label3.Text = "Stock mínimo";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(12, 174);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 20);
            this.label2.TabIndex = 35;
            this.label2.Text = "Stock inicial";
            // 
            // nudStockInicial
            // 
            this.nudStockInicial.DecimalPlaces = 2;
            this.nudStockInicial.Location = new System.Drawing.Point(16, 209);
            this.nudStockInicial.Name = "nudStockInicial";
            this.nudStockInicial.Size = new System.Drawing.Size(229, 20);
            this.nudStockInicial.TabIndex = 34;
            // 
            // cmbUnidadMedida
            // 
            this.cmbUnidadMedida.BackColor = System.Drawing.Color.LightCyan;
            this.cmbUnidadMedida.FormattingEnabled = true;
            this.cmbUnidadMedida.Location = new System.Drawing.Point(328, 117);
            this.cmbUnidadMedida.Name = "cmbUnidadMedida";
            this.cmbUnidadMedida.Size = new System.Drawing.Size(229, 21);
            this.cmbUnidadMedida.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(324, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 20);
            this.label1.TabIndex = 32;
            this.label1.Text = "Unidad de medida";
            // 
            // txtNombreMaterial
            // 
            this.txtNombreMaterial.BackColor = System.Drawing.Color.LightCyan;
            this.txtNombreMaterial.Location = new System.Drawing.Point(16, 117);
            this.txtNombreMaterial.Multiline = true;
            this.txtNombreMaterial.Name = "txtNombreMaterial";
            this.txtNombreMaterial.Size = new System.Drawing.Size(229, 29);
            this.txtNombreMaterial.TabIndex = 31;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(12, 81);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(154, 20);
            this.label11.TabIndex = 30;
            this.label11.Text = "Nombre del material";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.PowderBlue;
            this.panel3.Controls.Add(this.label10);
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(814, 54);
            this.panel3.TabIndex = 29;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(12, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(175, 21);
            this.label10.TabIndex = 6;
            this.label10.Text = "REGISTRAR MATERIAL";
            // 
            // tabConsultarMateriales
            // 
            this.tabConsultarMateriales.Controls.Add(this.dgvStockMateriales);
            this.tabConsultarMateriales.Controls.Add(this.btnMostrarTodosMateriales);
            this.tabConsultarMateriales.Controls.Add(this.btnBuscarMaterial);
            this.tabConsultarMateriales.Controls.Add(this.txtBuscarMaterial);
            this.tabConsultarMateriales.Controls.Add(this.label6);
            this.tabConsultarMateriales.Controls.Add(this.panel1);
            this.tabConsultarMateriales.Location = new System.Drawing.Point(4, 22);
            this.tabConsultarMateriales.Name = "tabConsultarMateriales";
            this.tabConsultarMateriales.Padding = new System.Windows.Forms.Padding(3);
            this.tabConsultarMateriales.Size = new System.Drawing.Size(814, 444);
            this.tabConsultarMateriales.TabIndex = 1;
            this.tabConsultarMateriales.Text = "Consultar material";
            this.tabConsultarMateriales.UseVisualStyleBackColor = true;
            // 
            // dgvStockMateriales
            // 
            this.dgvStockMateriales.AllowUserToAddRows = false;
            this.dgvStockMateriales.AllowUserToDeleteRows = false;
            this.dgvStockMateriales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStockMateriales.Location = new System.Drawing.Point(16, 208);
            this.dgvStockMateriales.MultiSelect = false;
            this.dgvStockMateriales.Name = "dgvStockMateriales";
            this.dgvStockMateriales.ReadOnly = true;
            this.dgvStockMateriales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStockMateriales.Size = new System.Drawing.Size(779, 216);
            this.dgvStockMateriales.TabIndex = 43;
            // 
            // btnMostrarTodosMateriales
            // 
            this.btnMostrarTodosMateriales.BackColor = System.Drawing.Color.SteelBlue;
            this.btnMostrarTodosMateriales.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.btnMostrarTodosMateriales.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMostrarTodosMateriales.Location = new System.Drawing.Point(642, 153);
            this.btnMostrarTodosMateriales.Name = "btnMostrarTodosMateriales";
            this.btnMostrarTodosMateriales.Size = new System.Drawing.Size(142, 23);
            this.btnMostrarTodosMateriales.TabIndex = 42;
            this.btnMostrarTodosMateriales.Text = "MOSTRAR TODOS";
            this.btnMostrarTodosMateriales.UseVisualStyleBackColor = false;
            // 
            // btnBuscarMaterial
            // 
            this.btnBuscarMaterial.BackColor = System.Drawing.Color.Silver;
            this.btnBuscarMaterial.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnBuscarMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarMaterial.Location = new System.Drawing.Point(518, 153);
            this.btnBuscarMaterial.Name = "btnBuscarMaterial";
            this.btnBuscarMaterial.Size = new System.Drawing.Size(108, 23);
            this.btnBuscarMaterial.TabIndex = 41;
            this.btnBuscarMaterial.Text = "BUSCAR";
            this.btnBuscarMaterial.UseVisualStyleBackColor = false;
            // 
            // txtBuscarMaterial
            // 
            this.txtBuscarMaterial.BackColor = System.Drawing.Color.LightCyan;
            this.txtBuscarMaterial.Location = new System.Drawing.Point(16, 116);
            this.txtBuscarMaterial.Multiline = true;
            this.txtBuscarMaterial.Name = "txtBuscarMaterial";
            this.txtBuscarMaterial.Size = new System.Drawing.Size(779, 20);
            this.txtBuscarMaterial.TabIndex = 32;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(12, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 20);
            this.label6.TabIndex = 31;
            this.label6.Text = "Buscar material";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.PowderBlue;
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(814, 54);
            this.panel1.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(12, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(279, 21);
            this.label5.TabIndex = 6;
            this.label5.Text = "CONSULTAR STOCK DE MATERIALES";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // tabHistorialMovimientos
            // 
            this.tabHistorialMovimientos.Controls.Add(this.dtpFechaHasta);
            this.tabHistorialMovimientos.Controls.Add(this.dtpFechaDesde);
            this.tabHistorialMovimientos.Controls.Add(this.btnFiltrarHistorial);
            this.tabHistorialMovimientos.Controls.Add(this.label9);
            this.tabHistorialMovimientos.Controls.Add(this.label8);
            this.tabHistorialMovimientos.Controls.Add(this.btnActualizarHistorial);
            this.tabHistorialMovimientos.Controls.Add(this.dgvHistorialMovimientos);
            this.tabHistorialMovimientos.Controls.Add(this.panel2);
            this.tabHistorialMovimientos.Location = new System.Drawing.Point(4, 22);
            this.tabHistorialMovimientos.Name = "tabHistorialMovimientos";
            this.tabHistorialMovimientos.Padding = new System.Windows.Forms.Padding(3);
            this.tabHistorialMovimientos.Size = new System.Drawing.Size(814, 444);
            this.tabHistorialMovimientos.TabIndex = 2;
            this.tabHistorialMovimientos.Text = "Historial de movimientos";
            this.tabHistorialMovimientos.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.PowderBlue;
            this.panel2.Controls.Add(this.label7);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(814, 54);
            this.panel2.TabIndex = 31;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(12, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(202, 21);
            this.label7.TabIndex = 6;
            this.label7.Text = "MOVIMIENTOS DE STOCK";
            // 
            // dgvHistorialMovimientos
            // 
            this.dgvHistorialMovimientos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorialMovimientos.Location = new System.Drawing.Point(7, 149);
            this.dgvHistorialMovimientos.Name = "dgvHistorialMovimientos";
            this.dgvHistorialMovimientos.Size = new System.Drawing.Size(801, 289);
            this.dgvHistorialMovimientos.TabIndex = 32;
            this.dgvHistorialMovimientos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHistorialMovimientos_CellContentClick);
            // 
            // btnActualizarHistorial
            // 
            this.btnActualizarHistorial.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btnActualizarHistorial.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnActualizarHistorial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActualizarHistorial.Location = new System.Drawing.Point(612, 120);
            this.btnActualizarHistorial.Name = "btnActualizarHistorial";
            this.btnActualizarHistorial.Size = new System.Drawing.Size(178, 23);
            this.btnActualizarHistorial.TabIndex = 47;
            this.btnActualizarHistorial.Text = "Actualizar Historial";
            this.btnActualizarHistorial.UseVisualStyleBackColor = false;
            this.btnActualizarHistorial.Click += new System.EventHandler(this.btnActualizarHistorial_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(6, 68);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 20);
            this.label8.TabIndex = 48;
            this.label8.Text = "Desde:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(230, 68);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 20);
            this.label9.TabIndex = 49;
            this.label9.Text = "Hasta:";
            // 
            // btnFiltrarHistorial
            // 
            this.btnFiltrarHistorial.BackColor = System.Drawing.Color.Silver;
            this.btnFiltrarHistorial.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnFiltrarHistorial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFiltrarHistorial.Location = new System.Drawing.Point(463, 88);
            this.btnFiltrarHistorial.Name = "btnFiltrarHistorial";
            this.btnFiltrarHistorial.Size = new System.Drawing.Size(108, 23);
            this.btnFiltrarHistorial.TabIndex = 50;
            this.btnFiltrarHistorial.Text = "FILTRAR";
            this.btnFiltrarHistorial.UseVisualStyleBackColor = false;
            this.btnFiltrarHistorial.Click += new System.EventHandler(this.btnFiltrarHistorial_Click);
            // 
            // dtpFechaDesde
            // 
            this.dtpFechaDesde.Location = new System.Drawing.Point(7, 92);
            this.dtpFechaDesde.Name = "dtpFechaDesde";
            this.dtpFechaDesde.Size = new System.Drawing.Size(207, 20);
            this.dtpFechaDesde.TabIndex = 51;
            // 
            // dtpFechaHasta
            // 
            this.dtpFechaHasta.Location = new System.Drawing.Point(234, 91);
            this.dtpFechaHasta.Name = "dtpFechaHasta";
            this.dtpFechaHasta.Size = new System.Drawing.Size(207, 20);
            this.dtpFechaHasta.TabIndex = 52;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(413, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(401, 646);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 42;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // Form15
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 595);
            this.Controls.Add(this.tabControlMateriales);
            this.Controls.Add(this.pnlEncabezado);
            this.Name = "Form15";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form15";
            this.Load += new System.EventHandler(this.Form15_Load);
            this.pnlEncabezado.ResumeLayout(false);
            this.pnlEncabezado.PerformLayout();
            this.tabControlMateriales.ResumeLayout(false);
            this.tabRegistrarMaterial.ResumeLayout(false);
            this.tabRegistrarMaterial.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockMinimo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockInicial)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabConsultarMateriales.ResumeLayout(false);
            this.tabConsultarMateriales.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockMateriales)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabHistorialMovimientos.ResumeLayout(false);
            this.tabHistorialMovimientos.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialMovimientos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.TabControl tabControlMateriales;
        private System.Windows.Forms.TabPage tabRegistrarMaterial;
        private System.Windows.Forms.TabPage tabConsultarMateriales;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtNombreMaterial;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudStockInicial;
        private System.Windows.Forms.ComboBox cmbUnidadMedida;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDescripcionMaterial;
        private System.Windows.Forms.NumericUpDown nudStockMinimo;
        private System.Windows.Forms.Button btnLimpiarMaterial;
        private System.Windows.Forms.Button btnGuardarMaterial;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dgvStockMateriales;
        private System.Windows.Forms.Button btnMostrarTodosMateriales;
        private System.Windows.Forms.Button btnBuscarMaterial;
        private System.Windows.Forms.TextBox txtBuscarMaterial;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabPage tabHistorialMovimientos;
        private System.Windows.Forms.DataGridView dgvHistorialMovimientos;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnActualizarHistorial;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dtpFechaHasta;
        private System.Windows.Forms.DateTimePicker dtpFechaDesde;
        private System.Windows.Forms.Button btnFiltrarHistorial;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}