namespace StreamUtilities
{
    partial class BlinkLabel
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

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.type = new StreamUtilities.HitLabel();
            this.owner = new StreamUtilities.HitLabel();
            this.message = new StreamUtilities.HitLabel();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.type);
            this.flowLayoutPanel1.Controls.Add(this.owner);
            this.flowLayoutPanel1.Controls.Add(this.message);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(185, 17);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // type
            // 
            this.type.AutoSize = true;
            this.type.BackColor = System.Drawing.Color.Navy;
            this.type.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.type.ForeColor = System.Drawing.Color.White;
            this.type.Location = new System.Drawing.Point(0, 0);
            this.type.Margin = new System.Windows.Forms.Padding(0);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(46, 17);
            this.type.TabIndex = 1;
            this.type.Text = "[TYPE]";
            // 
            // owner
            // 
            this.owner.AutoSize = true;
            this.owner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.owner.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.owner.ForeColor = System.Drawing.Color.DarkGray;
            this.owner.Location = new System.Drawing.Point(46, 0);
            this.owner.Margin = new System.Windows.Forms.Padding(0);
            this.owner.Name = "owner";
            this.owner.Size = new System.Drawing.Size(60, 17);
            this.owner.TabIndex = 2;
            this.owner.Text = "[Owner]";
            // 
            // message
            // 
            this.message.AutoSize = true;
            this.message.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.message.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.message.ForeColor = System.Drawing.Color.White;
            this.message.Location = new System.Drawing.Point(106, 0);
            this.message.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(69, 17);
            this.message.TabIndex = 3;
            this.message.Text = "[Message]";
            // 
            // BlinkLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.flowLayoutPanel1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "BlinkLabel";
            this.Size = new System.Drawing.Size(185, 17);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private HitLabel type;
        private HitLabel owner;
        private HitLabel message;
    }
}
