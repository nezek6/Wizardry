namespace TileCutter
{
	partial class frmTileCutter
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFile_Open = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFile_Close = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.mnuFile_Exit = new System.Windows.Forms.ToolStripMenuItem();
			this.panControls = new System.Windows.Forms.Panel();
			this.btnEngage = new System.Windows.Forms.Button();
			this.cmbOutFileType = new System.Windows.Forms.ComboBox();
			this.txtOutFileBaseName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.nudCutHeight = new System.Windows.Forms.NumericUpDown();
			this.nudCutWidth = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.lblFileName = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.menuStrip1.SuspendLayout();
			this.panControls.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)( this.nudCutHeight ) ).BeginInit();
			( (System.ComponentModel.ISupportInitialize)( this.nudCutWidth ) ).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile} );
			this.menuStrip1.Location = new System.Drawing.Point( 0, 0 );
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size( 284, 24 );
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// mnuFile
			// 
			this.mnuFile.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile_Open,
            this.mnuFile_Close,
            this.exitToolStripMenuItem,
            this.mnuFile_Exit} );
			this.mnuFile.Name = "mnuFile";
			this.mnuFile.Size = new System.Drawing.Size( 37, 20 );
			this.mnuFile.Text = "File";
			// 
			// mnuFile_Open
			// 
			this.mnuFile_Open.Name = "mnuFile_Open";
			this.mnuFile_Open.Size = new System.Drawing.Size( 103, 22 );
			this.mnuFile_Open.Text = "Open";
			this.mnuFile_Open.Click += new System.EventHandler( this.mnuFile_Open_Click );
			// 
			// mnuFile_Close
			// 
			this.mnuFile_Close.Name = "mnuFile_Close";
			this.mnuFile_Close.Size = new System.Drawing.Size( 103, 22 );
			this.mnuFile_Close.Text = "Close";
			this.mnuFile_Close.Click += new System.EventHandler( this.mnuFile_Close_Click );
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size( 100, 6 );
			// 
			// mnuFile_Exit
			// 
			this.mnuFile_Exit.Name = "mnuFile_Exit";
			this.mnuFile_Exit.Size = new System.Drawing.Size( 103, 22 );
			this.mnuFile_Exit.Text = "Exit";
			this.mnuFile_Exit.Click += new System.EventHandler( this.mnuFile_Exit_Click );
			// 
			// panControls
			// 
			this.panControls.Controls.Add( this.btnEngage );
			this.panControls.Controls.Add( this.cmbOutFileType );
			this.panControls.Controls.Add( this.txtOutFileBaseName );
			this.panControls.Controls.Add( this.label6 );
			this.panControls.Controls.Add( this.label5 );
			this.panControls.Controls.Add( this.label4 );
			this.panControls.Controls.Add( this.label3 );
			this.panControls.Controls.Add( this.nudCutHeight );
			this.panControls.Controls.Add( this.nudCutWidth );
			this.panControls.Controls.Add( this.label2 );
			this.panControls.Controls.Add( this.lblFileName );
			this.panControls.Controls.Add( this.label1 );
			this.panControls.Location = new System.Drawing.Point( 13, 28 );
			this.panControls.Name = "panControls";
			this.panControls.Size = new System.Drawing.Size( 259, 168 );
			this.panControls.TabIndex = 1;
			// 
			// btnEngage
			// 
			this.btnEngage.Location = new System.Drawing.Point( 74, 122 );
			this.btnEngage.Name = "btnEngage";
			this.btnEngage.Size = new System.Drawing.Size( 111, 41 );
			this.btnEngage.TabIndex = 11;
			this.btnEngage.Text = "Engage!";
			this.btnEngage.UseVisualStyleBackColor = true;
			this.btnEngage.Click += new System.EventHandler( this.btnEngage_Click );
			// 
			// cmbOutFileType
			// 
			this.cmbOutFileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbOutFileType.FormattingEnabled = true;
			this.cmbOutFileType.Items.AddRange( new object[] {
            "png",
            "jpeg"} );
			this.cmbOutFileType.Location = new System.Drawing.Point( 172, 84 );
			this.cmbOutFileType.Name = "cmbOutFileType";
			this.cmbOutFileType.Size = new System.Drawing.Size( 68, 21 );
			this.cmbOutFileType.TabIndex = 10;
			// 
			// txtOutFileBaseName
			// 
			this.txtOutFileBaseName.Location = new System.Drawing.Point( 3, 84 );
			this.txtOutFileBaseName.Name = "txtOutFileBaseName";
			this.txtOutFileBaseName.Size = new System.Drawing.Size( 159, 20 );
			this.txtOutFileBaseName.TabIndex = 9;
			this.txtOutFileBaseName.KeyDown += new System.Windows.Forms.KeyEventHandler( this.txtOutFileBaseName_KeyDown );
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point( 175, 67 );
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size( 53, 13 );
			this.label6.TabIndex = 8;
			this.label6.Text = "File Type:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point( 3, 67 );
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size( 124, 13 );
			this.label5.TabIndex = 7;
			this.label5.Text = "Output Files Base Name:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point( 118, 42 );
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size( 41, 13 );
			this.label4.TabIndex = 6;
			this.label4.Text = "Height:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point( 3, 42 );
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size( 38, 13 );
			this.label3.TabIndex = 5;
			this.label3.Text = "Width:";
			// 
			// nudCutHeight
			// 
			this.nudCutHeight.Location = new System.Drawing.Point( 165, 38 );
			this.nudCutHeight.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			this.nudCutHeight.Name = "nudCutHeight";
			this.nudCutHeight.Size = new System.Drawing.Size( 52, 20 );
			this.nudCutHeight.TabIndex = 4;
			this.nudCutHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudCutHeight.Value = new decimal( new int[] {
            64,
            0,
            0,
            0} );
			// 
			// nudCutWidth
			// 
			this.nudCutWidth.Location = new System.Drawing.Point( 47, 38 );
			this.nudCutWidth.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			this.nudCutWidth.Name = "nudCutWidth";
			this.nudCutWidth.Size = new System.Drawing.Size( 52, 20 );
			this.nudCutWidth.TabIndex = 3;
			this.nudCutWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudCutWidth.Value = new decimal( new int[] {
            64,
            0,
            0,
            0} );
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point( 3, 24 );
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size( 49, 13 );
			this.label2.TabIndex = 2;
			this.label2.Text = "Cut Size:";
			// 
			// lblFileName
			// 
			this.lblFileName.AutoSize = true;
			this.lblFileName.Location = new System.Drawing.Point( 82, 3 );
			this.lblFileName.Name = "lblFileName";
			this.lblFileName.Size = new System.Drawing.Size( 33, 13 );
			this.lblFileName.TabIndex = 1;
			this.lblFileName.Text = "blank";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 3, 3 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 77, 13 );
			this.label1.TabIndex = 0;
			this.label1.Text = "Original Image:";
			// 
			// frmTileCutter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 284, 206 );
			this.Controls.Add( this.panControls );
			this.Controls.Add( this.menuStrip1 );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "frmTileCutter";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Tile Cutter App";
			this.menuStrip1.ResumeLayout( false );
			this.menuStrip1.PerformLayout();
			this.panControls.ResumeLayout( false );
			this.panControls.PerformLayout();
			( (System.ComponentModel.ISupportInitialize)( this.nudCutHeight ) ).EndInit();
			( (System.ComponentModel.ISupportInitialize)( this.nudCutWidth ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem mnuFile;
		private System.Windows.Forms.ToolStripMenuItem mnuFile_Open;
		private System.Windows.Forms.ToolStripSeparator exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnuFile_Exit;
		private System.Windows.Forms.Panel panControls;
		private System.Windows.Forms.ToolStripMenuItem mnuFile_Close;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblFileName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown nudCutHeight;
		private System.Windows.Forms.NumericUpDown nudCutWidth;
		private System.Windows.Forms.ComboBox cmbOutFileType;
		private System.Windows.Forms.TextBox txtOutFileBaseName;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnEngage;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
	}
}

