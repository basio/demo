namespace WindowsFormsProject
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.wpfHost = new System.Windows.Forms.Integration.ElementHost();
            this.but_generate = new System.Windows.Forms.Button();
            this.but_reload = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // wpfHost
            // 
            this.wpfHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wpfHost.BackColor = System.Drawing.Color.White;
            this.wpfHost.Location = new System.Drawing.Point(0, 47);
            this.wpfHost.Name = "wpfHost";
            this.wpfHost.Size = new System.Drawing.Size(596, 301);
            this.wpfHost.TabIndex = 0;
            this.wpfHost.Text = "elementHost1";
            this.wpfHost.Child = null;
            // 
            // but_generate
            // 
            this.but_generate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.but_generate.Image = ((System.Drawing.Image)(resources.GetObject("but_generate.Image")));
            this.but_generate.Location = new System.Drawing.Point(602, 142);
            this.but_generate.Name = "but_generate";
            this.but_generate.Size = new System.Drawing.Size(65, 65);
            this.but_generate.TabIndex = 1;
            this.but_generate.UseVisualStyleBackColor = true;
            this.but_generate.Visible = false;
            this.but_generate.Click += new System.EventHandler(this.but_generate_Click);
            // 
            // but_reload
            // 
            this.but_reload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.but_reload.Image = ((System.Drawing.Image)(resources.GetObject("but_reload.Image")));
            this.but_reload.Location = new System.Drawing.Point(602, 71);
            this.but_reload.Name = "but_reload";
            this.but_reload.Size = new System.Drawing.Size(65, 65);
            this.but_reload.TabIndex = 2;
            this.but_reload.UseVisualStyleBackColor = true;
            this.but_reload.Visible = false;
            this.but_reload.Click += new System.EventHandler(this.but_reload_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(25, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(531, 20);
            this.textBox1.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(577, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Go";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 349);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.but_reload);
            this.Controls.Add(this.but_generate);
            this.Controls.Add(this.wpfHost);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GraphX WF Interop Sample Project v1.0";
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost wpfHost;
        private System.Windows.Forms.Button but_generate;
        private System.Windows.Forms.Button but_reload;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
    }
}

