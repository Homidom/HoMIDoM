namespace HomiTest
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
            this.button1 = new System.Windows.Forms.Button();
            this.tb_hostName = new System.Windows.Forms.TextBox();
            this.nud_port = new System.Windows.Forms.NumericUpDown();
            this.lv_devices = new System.Windows.Forms.ListView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tp_Result = new System.Windows.Forms.TabPage();
            this.tp_Raw = new System.Windows.Forms.TabPage();
            this.tb_xml = new System.Windows.Forms.TextBox();
            this.btn_raw_update = new System.Windows.Forms.Button();
            this.tb_key = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ctlDisplayXML1 = new HomiTest.ctlDisplayXML();
            ((System.ComponentModel.ISupportInitialize)(this.nud_port)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tp_Result.SuspendLayout();
            this.tp_Raw.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(415, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(180, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "Get devices";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tb_hostName
            // 
            this.tb_hostName.Location = new System.Drawing.Point(10, 12);
            this.tb_hostName.Name = "tb_hostName";
            this.tb_hostName.Size = new System.Drawing.Size(224, 22);
            this.tb_hostName.TabIndex = 1;
            this.tb_hostName.Text = "localhost";
            // 
            // nud_port
            // 
            this.nud_port.Location = new System.Drawing.Point(240, 12);
            this.nud_port.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nud_port.Name = "nud_port";
            this.nud_port.Size = new System.Drawing.Size(64, 22);
            this.nud_port.TabIndex = 2;
            this.nud_port.Value = new decimal(new int[] {
            7999,
            0,
            0,
            0});
            // 
            // lv_devices
            // 
            this.lv_devices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lv_devices.Location = new System.Drawing.Point(10, 40);
            this.lv_devices.Name = "lv_devices";
            this.lv_devices.Size = new System.Drawing.Size(224, 402);
            this.lv_devices.TabIndex = 3;
            this.lv_devices.UseCompatibleStateImageBehavior = false;
            this.lv_devices.View = System.Windows.Forms.View.List;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tp_Result);
            this.tabControl1.Controls.Add(this.tp_Raw);
            this.tabControl1.Location = new System.Drawing.Point(240, 40);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(947, 402);
            this.tabControl1.TabIndex = 5;
            // 
            // tp_Result
            // 
            this.tp_Result.Controls.Add(this.ctlDisplayXML1);
            this.tp_Result.Location = new System.Drawing.Point(4, 25);
            this.tp_Result.Name = "tp_Result";
            this.tp_Result.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Result.Size = new System.Drawing.Size(939, 373);
            this.tp_Result.TabIndex = 0;
            this.tp_Result.Text = "Result";
            this.tp_Result.UseVisualStyleBackColor = true;
            // 
            // tp_Raw
            // 
            this.tp_Raw.Controls.Add(this.tb_xml);
            this.tp_Raw.Controls.Add(this.panel1);
            this.tp_Raw.Location = new System.Drawing.Point(4, 25);
            this.tp_Raw.Name = "tp_Raw";
            this.tp_Raw.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Raw.Size = new System.Drawing.Size(939, 373);
            this.tp_Raw.TabIndex = 1;
            this.tp_Raw.Text = "Raw";
            this.tp_Raw.UseVisualStyleBackColor = true;
            // 
            // tb_xml
            // 
            this.tb_xml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_xml.Location = new System.Drawing.Point(3, 3);
            this.tb_xml.Multiline = true;
            this.tb_xml.Name = "tb_xml";
            this.tb_xml.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_xml.Size = new System.Drawing.Size(933, 322);
            this.tb_xml.TabIndex = 0;
            // 
            // btn_raw_update
            // 
            this.btn_raw_update.Location = new System.Drawing.Point(3, 6);
            this.btn_raw_update.Name = "btn_raw_update";
            this.btn_raw_update.Size = new System.Drawing.Size(192, 27);
            this.btn_raw_update.TabIndex = 1;
            this.btn_raw_update.Text = "Update Result tab";
            this.btn_raw_update.UseVisualStyleBackColor = true;
            this.btn_raw_update.Click += new System.EventHandler(this.button2_Click);
            // 
            // tb_key
            // 
            this.tb_key.Location = new System.Drawing.Point(310, 12);
            this.tb_key.Name = "tb_key";
            this.tb_key.Size = new System.Drawing.Size(99, 22);
            this.tb_key.TabIndex = 1;
            this.tb_key.Text = "123456789";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_raw_update);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 325);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(933, 45);
            this.panel1.TabIndex = 2;
            // 
            // ctlDisplayXML1
            // 
            this.ctlDisplayXML1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlDisplayXML1.Location = new System.Drawing.Point(3, 3);
            this.ctlDisplayXML1.Name = "ctlDisplayXML1";
            this.ctlDisplayXML1.Size = new System.Drawing.Size(933, 367);
            this.ctlDisplayXML1.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1199, 448);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lv_devices);
            this.Controls.Add(this.nud_port);
            this.Controls.Add(this.tb_key);
            this.Controls.Add(this.tb_hostName);
            this.Name = "Form1";
            this.Text = "Homidroid Trace";
            ((System.ComponentModel.ISupportInitialize)(this.nud_port)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tp_Result.ResumeLayout(false);
            this.tp_Raw.ResumeLayout(false);
            this.tp_Raw.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tb_hostName;
        private System.Windows.Forms.NumericUpDown nud_port;
        private System.Windows.Forms.ListView lv_devices;
        private ctlDisplayXML ctlDisplayXML1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tp_Result;
        private System.Windows.Forms.TabPage tp_Raw;
        private System.Windows.Forms.TextBox tb_xml;
        private System.Windows.Forms.Button btn_raw_update;
        private System.Windows.Forms.TextBox tb_key;
        private System.Windows.Forms.Panel panel1;
    }
}

