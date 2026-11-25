#nullable enable

namespace SistemaGestionCitas.src.Forms
{
    partial class FrmDashboard
    {
        private System.ComponentModel.IContainer? components;
        private Panel pnlSidebar = null!;
        private Panel pnlContenido = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlSidebar = new Panel();
            pnlContenido = new Panel();
            SuspendLayout();

            pnlSidebar.BackColor = Color.FromArgb(247, 250, 254);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Width = 236;

            pnlContenido.BackColor = Color.FromArgb(232, 241, 252);
            pnlContenido.Dock = DockStyle.Fill;
            pnlContenido.Name = "pnlContenido";
            pnlContenido.TabIndex = 1;

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = pnlContenido.BackColor;
            ClientSize = new Size(1280, 720);
            MinimumSize = new Size(760, 530);
            Controls.Add(pnlContenido);
            Controls.Add(pnlSidebar);
            Name = "FrmDashboard";
            Text = "Citorna — Inicio";
            ResumeLayout(false);
        }
    }
}
