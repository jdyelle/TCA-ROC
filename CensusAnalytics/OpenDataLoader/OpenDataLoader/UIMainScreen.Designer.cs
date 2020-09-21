namespace OpenDataLoader
{
    partial class UIMainScreen
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.boxFileInfo = new System.Windows.Forms.GroupBox();
            this.cmbFileType = new System.Windows.Forms.ComboBox();
            this.btnFileSelect = new System.Windows.Forms.Button();
            this.cmbFileSource = new System.Windows.Forms.ComboBox();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.lblFileType = new System.Windows.Forms.Label();
            this.lblFileSource = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.txtDBServer = new System.Windows.Forms.TextBox();
            this.lblDBServer = new System.Windows.Forms.Label();
            this.boxDatabaseInfo = new System.Windows.Forms.GroupBox();
            this.cmbDatabaseType = new System.Windows.Forms.ComboBox();
            this.lblDatabaseType = new System.Windows.Forms.Label();
            this.lblDBCatalog = new System.Windows.Forms.Label();
            this.lblDBPassword = new System.Windows.Forms.Label();
            this.txtDBCatalog = new System.Windows.Forms.TextBox();
            this.lblDBUsername = new System.Windows.Forms.Label();
            this.btnSaveDBInfo = new System.Windows.Forms.Button();
            this.txtDBPassword = new System.Windows.Forms.TextBox();
            this.txtDBUsername = new System.Windows.Forms.TextBox();
            this.dgMessages = new System.Windows.Forms.DataGridView();
            this.btnCheckDBTable = new System.Windows.Forms.Button();
            this.btnLoadSelectedFile = new System.Windows.Forms.Button();
            this.btnExitApp = new System.Windows.Forms.Button();
            this.chkDebugMode = new System.Windows.Forms.CheckBox();
            this.btnPreviousFiles = new System.Windows.Forms.Button();
            this.boxFileInfo.SuspendLayout();
            this.boxDatabaseInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgMessages)).BeginInit();
            this.SuspendLayout();
            // 
            // boxFileInfo
            // 
            this.boxFileInfo.Controls.Add(this.cmbFileType);
            this.boxFileInfo.Controls.Add(this.btnFileSelect);
            this.boxFileInfo.Controls.Add(this.cmbFileSource);
            this.boxFileInfo.Controls.Add(this.txtFileName);
            this.boxFileInfo.Controls.Add(this.lblFileType);
            this.boxFileInfo.Controls.Add(this.lblFileSource);
            this.boxFileInfo.Controls.Add(this.lblFileName);
            this.boxFileInfo.Location = new System.Drawing.Point(13, 13);
            this.boxFileInfo.Name = "boxFileInfo";
            this.boxFileInfo.Size = new System.Drawing.Size(428, 169);
            this.boxFileInfo.TabIndex = 0;
            this.boxFileInfo.TabStop = false;
            this.boxFileInfo.Text = "File Details";
            // 
            // cmbFileType
            // 
            this.cmbFileType.FormattingEnabled = true;
            this.cmbFileType.Location = new System.Drawing.Point(78, 130);
            this.cmbFileType.Name = "cmbFileType";
            this.cmbFileType.Size = new System.Drawing.Size(339, 23);
            this.cmbFileType.TabIndex = 40;
            // 
            // btnFileSelect
            // 
            this.btnFileSelect.Location = new System.Drawing.Point(342, 49);
            this.btnFileSelect.Name = "btnFileSelect";
            this.btnFileSelect.Size = new System.Drawing.Size(75, 23);
            this.btnFileSelect.TabIndex = 20;
            this.btnFileSelect.Text = "Browse";
            this.btnFileSelect.UseVisualStyleBackColor = true;
            // 
            // cmbFileSource
            // 
            this.cmbFileSource.FormattingEnabled = true;
            this.cmbFileSource.Location = new System.Drawing.Point(78, 92);
            this.cmbFileSource.Name = "cmbFileSource";
            this.cmbFileSource.Size = new System.Drawing.Size(339, 23);
            this.cmbFileSource.TabIndex = 30;
            this.cmbFileSource.SelectedIndexChanged += new System.EventHandler(this.cmbFileSource_SelectedIndexChanged);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(73, 20);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(344, 23);
            this.txtFileName.TabIndex = 10;
            // 
            // lblFileType
            // 
            this.lblFileType.AutoSize = true;
            this.lblFileType.Location = new System.Drawing.Point(7, 133);
            this.lblFileType.Name = "lblFileType";
            this.lblFileType.Size = new System.Drawing.Size(52, 15);
            this.lblFileType.TabIndex = 0;
            this.lblFileType.Text = "File Type";
            // 
            // lblFileSource
            // 
            this.lblFileSource.AutoSize = true;
            this.lblFileSource.Location = new System.Drawing.Point(7, 95);
            this.lblFileSource.Name = "lblFileSource";
            this.lblFileSource.Size = new System.Drawing.Size(64, 15);
            this.lblFileSource.TabIndex = 0;
            this.lblFileSource.Text = "File Source";
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(7, 23);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(60, 15);
            this.lblFileName.TabIndex = 0;
            this.lblFileName.Text = "File Name";
            // 
            // txtDBServer
            // 
            this.txtDBServer.Location = new System.Drawing.Point(103, 20);
            this.txtDBServer.Name = "txtDBServer";
            this.txtDBServer.Size = new System.Drawing.Size(314, 23);
            this.txtDBServer.TabIndex = 110;
            // 
            // lblDBServer
            // 
            this.lblDBServer.AutoSize = true;
            this.lblDBServer.Location = new System.Drawing.Point(7, 23);
            this.lblDBServer.Name = "lblDBServer";
            this.lblDBServer.Size = new System.Drawing.Size(90, 15);
            this.lblDBServer.TabIndex = 0;
            this.lblDBServer.Text = "Database Server";
            // 
            // boxDatabaseInfo
            // 
            this.boxDatabaseInfo.Controls.Add(this.cmbDatabaseType);
            this.boxDatabaseInfo.Controls.Add(this.lblDatabaseType);
            this.boxDatabaseInfo.Controls.Add(this.lblDBCatalog);
            this.boxDatabaseInfo.Controls.Add(this.lblDBPassword);
            this.boxDatabaseInfo.Controls.Add(this.txtDBCatalog);
            this.boxDatabaseInfo.Controls.Add(this.lblDBUsername);
            this.boxDatabaseInfo.Controls.Add(this.btnSaveDBInfo);
            this.boxDatabaseInfo.Controls.Add(this.txtDBPassword);
            this.boxDatabaseInfo.Controls.Add(this.txtDBUsername);
            this.boxDatabaseInfo.Controls.Add(this.txtDBServer);
            this.boxDatabaseInfo.Controls.Add(this.lblDBServer);
            this.boxDatabaseInfo.Location = new System.Drawing.Point(447, 13);
            this.boxDatabaseInfo.Name = "boxDatabaseInfo";
            this.boxDatabaseInfo.Size = new System.Drawing.Size(428, 169);
            this.boxDatabaseInfo.TabIndex = 0;
            this.boxDatabaseInfo.TabStop = false;
            this.boxDatabaseInfo.Text = "Database Details";
            // 
            // cmbDatabaseType
            // 
            this.cmbDatabaseType.FormattingEnabled = true;
            this.cmbDatabaseType.Location = new System.Drawing.Point(267, 55);
            this.cmbDatabaseType.Name = "cmbDatabaseType";
            this.cmbDatabaseType.Size = new System.Drawing.Size(150, 23);
            this.cmbDatabaseType.TabIndex = 150;
            // 
            // lblDatabaseType
            // 
            this.lblDatabaseType.AutoSize = true;
            this.lblDatabaseType.Location = new System.Drawing.Point(298, 81);
            this.lblDatabaseType.Name = "lblDatabaseType";
            this.lblDatabaseType.Size = new System.Drawing.Size(82, 15);
            this.lblDatabaseType.TabIndex = 0;
            this.lblDatabaseType.Text = "Database Type";
            // 
            // lblDBCatalog
            // 
            this.lblDBCatalog.AutoSize = true;
            this.lblDBCatalog.Location = new System.Drawing.Point(7, 58);
            this.lblDBCatalog.Name = "lblDBCatalog";
            this.lblDBCatalog.Size = new System.Drawing.Size(90, 15);
            this.lblDBCatalog.TabIndex = 0;
            this.lblDBCatalog.Text = "Database Name";
            // 
            // lblDBPassword
            // 
            this.lblDBPassword.AutoSize = true;
            this.lblDBPassword.Location = new System.Drawing.Point(7, 133);
            this.lblDBPassword.Name = "lblDBPassword";
            this.lblDBPassword.Size = new System.Drawing.Size(75, 15);
            this.lblDBPassword.TabIndex = 0;
            this.lblDBPassword.Text = "DB Password";
            // 
            // txtDBCatalog
            // 
            this.txtDBCatalog.Location = new System.Drawing.Point(103, 55);
            this.txtDBCatalog.Name = "txtDBCatalog";
            this.txtDBCatalog.Size = new System.Drawing.Size(146, 23);
            this.txtDBCatalog.TabIndex = 120;
            // 
            // lblDBUsername
            // 
            this.lblDBUsername.AutoSize = true;
            this.lblDBUsername.Location = new System.Drawing.Point(7, 95);
            this.lblDBUsername.Name = "lblDBUsername";
            this.lblDBUsername.Size = new System.Drawing.Size(78, 15);
            this.lblDBUsername.TabIndex = 0;
            this.lblDBUsername.Text = "DB Username";
            // 
            // btnSaveDBInfo
            // 
            this.btnSaveDBInfo.Location = new System.Drawing.Point(267, 129);
            this.btnSaveDBInfo.Name = "btnSaveDBInfo";
            this.btnSaveDBInfo.Size = new System.Drawing.Size(150, 23);
            this.btnSaveDBInfo.TabIndex = 160;
            this.btnSaveDBInfo.Text = "Save DB Information";
            this.btnSaveDBInfo.UseVisualStyleBackColor = true;
            // 
            // txtDBPassword
            // 
            this.txtDBPassword.Location = new System.Drawing.Point(103, 130);
            this.txtDBPassword.Name = "txtDBPassword";
            this.txtDBPassword.PasswordChar = '*';
            this.txtDBPassword.Size = new System.Drawing.Size(146, 23);
            this.txtDBPassword.TabIndex = 140;
            // 
            // txtDBUsername
            // 
            this.txtDBUsername.Location = new System.Drawing.Point(103, 92);
            this.txtDBUsername.Name = "txtDBUsername";
            this.txtDBUsername.Size = new System.Drawing.Size(146, 23);
            this.txtDBUsername.TabIndex = 130;
            // 
            // dgMessages
            // 
            this.dgMessages.AllowUserToAddRows = false;
            this.dgMessages.AllowUserToDeleteRows = false;
            this.dgMessages.AllowUserToOrderColumns = true;
            this.dgMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgMessages.Location = new System.Drawing.Point(13, 189);
            this.dgMessages.Name = "dgMessages";
            this.dgMessages.ReadOnly = true;
            this.dgMessages.Size = new System.Drawing.Size(862, 197);
            this.dgMessages.TabIndex = 100;
            this.dgMessages.Text = "dataGridView1";
            // 
            // btnCheckDBTable
            // 
            this.btnCheckDBTable.Location = new System.Drawing.Point(13, 392);
            this.btnCheckDBTable.Name = "btnCheckDBTable";
            this.btnCheckDBTable.Size = new System.Drawing.Size(185, 23);
            this.btnCheckDBTable.TabIndex = 50;
            this.btnCheckDBTable.Text = "Check DB Table Record Count";
            this.btnCheckDBTable.UseVisualStyleBackColor = true;
            // 
            // btnLoadSelectedFile
            // 
            this.btnLoadSelectedFile.Location = new System.Drawing.Point(204, 392);
            this.btnLoadSelectedFile.Name = "btnLoadSelectedFile";
            this.btnLoadSelectedFile.Size = new System.Drawing.Size(185, 23);
            this.btnLoadSelectedFile.TabIndex = 60;
            this.btnLoadSelectedFile.Text = "Load Selected File";
            this.btnLoadSelectedFile.UseVisualStyleBackColor = true;
            // 
            // btnExitApp
            // 
            this.btnExitApp.Location = new System.Drawing.Point(725, 392);
            this.btnExitApp.Name = "btnExitApp";
            this.btnExitApp.Size = new System.Drawing.Size(150, 23);
            this.btnExitApp.TabIndex = 90;
            this.btnExitApp.Text = "Exit Application";
            this.btnExitApp.UseVisualStyleBackColor = true;
            // 
            // chkDebugMode
            // 
            this.chkDebugMode.AutoSize = true;
            this.chkDebugMode.Location = new System.Drawing.Point(624, 395);
            this.chkDebugMode.Name = "chkDebugMode";
            this.chkDebugMode.Size = new System.Drawing.Size(95, 19);
            this.chkDebugMode.TabIndex = 80;
            this.chkDebugMode.Text = "Debug Mode";
            this.chkDebugMode.UseVisualStyleBackColor = true;
            // 
            // btnPreviousFiles
            // 
            this.btnPreviousFiles.Location = new System.Drawing.Point(395, 392);
            this.btnPreviousFiles.Name = "btnPreviousFiles";
            this.btnPreviousFiles.Size = new System.Drawing.Size(185, 23);
            this.btnPreviousFiles.TabIndex = 70;
            this.btnPreviousFiles.Text = "Display Previously Loaded Files";
            this.btnPreviousFiles.UseVisualStyleBackColor = true;
            // 
            // UIMainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 425);
            this.Controls.Add(this.btnPreviousFiles);
            this.Controls.Add(this.chkDebugMode);
            this.Controls.Add(this.btnLoadSelectedFile);
            this.Controls.Add(this.btnCheckDBTable);
            this.Controls.Add(this.btnExitApp);
            this.Controls.Add(this.dgMessages);
            this.Controls.Add(this.boxDatabaseInfo);
            this.Controls.Add(this.boxFileInfo);
            this.Name = "UIMainScreen";
            this.Text = "Open Data Loader";
            this.boxFileInfo.ResumeLayout(false);
            this.boxFileInfo.PerformLayout();
            this.boxDatabaseInfo.ResumeLayout(false);
            this.boxDatabaseInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgMessages)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox boxFileInfo;
        private System.Windows.Forms.ComboBox cmbFileType;
        private System.Windows.Forms.Button btnFileSelect;
        private System.Windows.Forms.ComboBox cmbFileSource;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label lblFileType;
        private System.Windows.Forms.Label lblFileSource;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.TextBox txtDBServer;
        private System.Windows.Forms.Label lblDBServer;
        private System.Windows.Forms.GroupBox boxDatabaseInfo;
        private System.Windows.Forms.Label lblDBCatalog;
        private System.Windows.Forms.Label lblDBPassword;
        private System.Windows.Forms.TextBox txtDBCatalog;
        private System.Windows.Forms.Label lblDBUsername;
        private System.Windows.Forms.TextBox txtDBPassword;
        private System.Windows.Forms.TextBox txtDBUsername;
        private System.Windows.Forms.DataGridView dgMessages;
        private System.Windows.Forms.Button btnSaveDBInfo;
        private System.Windows.Forms.Button btnCheckDBTable;
        private System.Windows.Forms.Button btnLoadSelectedFile;
        private System.Windows.Forms.Button btnExitApp;
        private System.Windows.Forms.CheckBox chkDebugMode;
        private System.Windows.Forms.Button btnPreviousFiles;
        private System.Windows.Forms.Label lblDatabaseType;
        private System.Windows.Forms.ComboBox cmbDatabaseType;
    }
}

