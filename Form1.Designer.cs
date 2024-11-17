using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace akf_mode
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        
        private void InitializeComponent()
        {
            this.txtKey = new System.Windows.Forms.TextBox();
            this.btnVerificar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.btnExecutar = new System.Windows.Forms.Button();
            
            this.SuspendLayout();
            
            // label1 - Título
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(65, 20);
            this.label1.Text = "DarkCloud LPR";
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            
            // txtKey - Campo de key
            this.txtKey.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtKey.Location = new System.Drawing.Point(30, 60);
            this.txtKey.Size = new System.Drawing.Size(240, 30);
            this.txtKey.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.txtKey.ForeColor = System.Drawing.Color.White;
            this.txtKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKey.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtKey.PlaceholderText = "Digite sua key de acesso";
            
            // btnVerificar
            this.btnVerificar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerificar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnVerificar.Location = new System.Drawing.Point(30, 100);
            this.btnVerificar.Size = new System.Drawing.Size(240, 40);
            this.btnVerificar.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnVerificar.ForeColor = System.Drawing.Color.White;
            this.btnVerificar.Text = "Executar é Entrar";
            this.btnVerificar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(0, 150, 240);
            this.btnVerificar.FlatAppearance.BorderSize = 1;
            this.btnVerificar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerificar.Click += new System.EventHandler(this.btnVerificar_Click);
            this.btnVerificar.TextAlign = ContentAlignment.MiddleCenter;
            
            // btnExecutar - Novo botão para executar o script
            this.btnExecutar = new System.Windows.Forms.Button();
            this.btnExecutar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecutar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnExecutar.Location = new System.Drawing.Point(30, 145);
            this.btnExecutar.Size = new System.Drawing.Size(240, 40);
            this.btnExecutar.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnExecutar.ForeColor = System.Drawing.Color.White;
            this.btnExecutar.Text = "Instalar lib2pr";
            this.btnExecutar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(0, 150, 240);
            this.btnExecutar.FlatAppearance.BorderSize = 1;
            this.btnExecutar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExecutar.Click += new System.EventHandler(this.btnExecutar_Click);
            this.btnExecutar.Enabled = false;
            this.btnExecutar.TextAlign = ContentAlignment.MiddleCenter;
            
            // statusLabel
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.statusLabel.ForeColor = System.Drawing.Color.LightGray;
            this.statusLabel.Location = new System.Drawing.Point(30, 190);
            this.statusLabel.Size = new System.Drawing.Size(240, 20);
            this.statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            
            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            this.ClientSize = new System.Drawing.Size(300, 240);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnVerificar);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.btnExecutar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            
            try
            {
                if (File.Exists("app.ico"))
                {
                    this.Icon = new System.Drawing.Icon("app.ico");
                }
            }
            catch { }
            
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DarkCloud";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Button btnVerificar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button btnExecutar;
    }
}