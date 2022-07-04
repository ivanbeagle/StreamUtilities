namespace StreamUtilities
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCaptureWin = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Handle = new System.Windows.Forms.ColumnHeader();
            this.Caption = new System.Windows.Forms.ColumnHeader();
            this.btnRemoveWin = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.opacity = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnConnectTwitch = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOpacity = new System.Windows.Forms.Button();
            this.btnCaptureMode = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 10;
            this.trackBar1.Location = new System.Drawing.Point(15, 147);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(492, 45);
            this.trackBar1.TabIndex = 4;
            this.trackBar1.Value = 60;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 125);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Windows Opacity:";
            // 
            // btnCaptureWin
            // 
            this.btnCaptureWin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnCaptureWin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCaptureWin.Location = new System.Drawing.Point(336, 243);
            this.btnCaptureWin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCaptureWin.Name = "btnCaptureWin";
            this.btnCaptureWin.Size = new System.Drawing.Size(172, 47);
            this.btnCaptureWin.TabIndex = 7;
            this.btnCaptureWin.Text = "Capture this foreground window";
            this.btnCaptureWin.UseVisualStyleBackColor = false;
            this.btnCaptureWin.Click += new System.EventHandler(this.btnCaptureWin_Click);
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Handle,
            this.Caption});
            this.listView1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.listView1.FullRowSelect = true;
            this.listView1.HoverSelection = true;
            this.listView1.Location = new System.Drawing.Point(19, 243);
            this.listView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(310, 86);
            this.listView1.TabIndex = 6;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // Handle
            // 
            this.Handle.Text = "Handle";
            this.Handle.Width = 55;
            // 
            // Caption
            // 
            this.Caption.Text = "Caption";
            this.Caption.Width = 180;
            // 
            // btnRemoveWin
            // 
            this.btnRemoveWin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnRemoveWin.Location = new System.Drawing.Point(336, 303);
            this.btnRemoveWin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRemoveWin.Name = "btnRemoveWin";
            this.btnRemoveWin.Size = new System.Drawing.Size(172, 27);
            this.btnRemoveWin.TabIndex = 8;
            this.btnRemoveWin.Text = "Remove selected window";
            this.btnRemoveWin.UseVisualStyleBackColor = false;
            this.btnRemoveWin.Click += new System.EventHandler(this.btnRemoveWin_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.label2.Location = new System.Drawing.Point(98, 344);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(15, 344);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Shortcuts:";
            // 
            // opacity
            // 
            this.opacity.AutoSize = true;
            this.opacity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.opacity.Location = new System.Drawing.Point(127, 125);
            this.opacity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.opacity.Name = "opacity";
            this.opacity.Size = new System.Drawing.Size(19, 15);
            this.opacity.TabIndex = 0;
            this.opacity.Text = "%";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "...";
            this.notifyIcon1.BalloonTipTitle = "...";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Stream Utilities is working! :)";
            this.notifyIcon1.BalloonTipClicked += new System.EventHandler(this.notifyIcon1_BalloonTipClicked);
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // btnConnectTwitch
            // 
            this.btnConnectTwitch.BackColor = System.Drawing.Color.Navy;
            this.btnConnectTwitch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnConnectTwitch.Image = global::StreamUtilities.Properties.Resources.twitch_small;
            this.btnConnectTwitch.Location = new System.Drawing.Point(19, 31);
            this.btnConnectTwitch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnConnectTwitch.Name = "btnConnectTwitch";
            this.btnConnectTwitch.Size = new System.Drawing.Size(172, 47);
            this.btnConnectTwitch.TabIndex = 2;
            this.btnConnectTwitch.Text = "Connect Twitch!";
            this.btnConnectTwitch.UseVisualStyleBackColor = false;
            this.btnConnectTwitch.Click += new System.EventHandler(this.btnConnectTwitch_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnAbout.Location = new System.Drawing.Point(468, -2);
            this.btnAbout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(58, 27);
            this.btnAbout.TabIndex = 1;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = false;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSettings.Location = new System.Drawing.Point(411, 31);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(115, 47);
            this.btnSettings.TabIndex = 3;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 218);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Tracked windows";
            // 
            // btnOpacity
            // 
            this.btnOpacity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnOpacity.Location = new System.Drawing.Point(411, 119);
            this.btnOpacity.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOpacity.Name = "btnOpacity";
            this.btnOpacity.Size = new System.Drawing.Size(88, 27);
            this.btnOpacity.TabIndex = 5;
            this.btnOpacity.Text = "Test opacity";
            this.btnOpacity.UseVisualStyleBackColor = false;
            this.btnOpacity.Click += new System.EventHandler(this.btnOpacity_Click);
            // 
            // btnCaptureMode
            // 
            this.btnCaptureMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnCaptureMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCaptureMode.Location = new System.Drawing.Point(336, 189);
            this.btnCaptureMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCaptureMode.Name = "btnCaptureMode";
            this.btnCaptureMode.Size = new System.Drawing.Size(172, 47);
            this.btnCaptureMode.TabIndex = 9;
            this.btnCaptureMode.Text = "Switch capture mode OFF";
            this.btnCaptureMode.UseVisualStyleBackColor = false;
            this.btnCaptureMode.Click += new System.EventHandler(this.btnCaptureMode_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(524, 369);
            this.Controls.Add(this.btnCaptureMode);
            this.Controls.Add(this.btnOpacity);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnConnectTwitch);
            this.Controls.Add(this.opacity);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnRemoveWin);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.btnCaptureWin);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBar1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Opacity = 0.98D;
            this.Text = "StreamUtilities";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCaptureWin;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button btnRemoveWin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label opacity;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button btnConnectTwitch;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOpacity;
        private System.Windows.Forms.Button btnCaptureMode;
        private System.Windows.Forms.ColumnHeader Handle;
        private System.Windows.Forms.ColumnHeader Caption;
    }
}

