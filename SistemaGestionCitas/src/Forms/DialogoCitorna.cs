namespace SistemaGestionCitas.src.Forms;

internal static class CampoCitorna
{
    internal static Control Enmarcar(Control campo, string nombre, int alto = 38)
    {
        var marco = new TarjetaAgenda { Dock = DockStyle.Fill, Radio = 9, ResaltarFoco = true, Height = alto,
            Margin = Padding.Empty, Padding = new Padding(10, 4, 10, 4), TabStop = false };
        campo.AccessibleName = nombre;
        campo.Margin = Padding.Empty;
        campo.ForeColor = EstiloAgenda.Tinta;
        if (campo is TextBox texto)
        {
            texto.BorderStyle = BorderStyle.None;
            texto.BackColor = Color.White;
            if (texto.Multiline) texto.ScrollBars = ScrollBars.Vertical;
        }
        if (campo is ComboBox combo) combo.FlatStyle = FlatStyle.Flat;

        void Alinear()
        {
            int altura = campo is TextBox { Multiline: true } ? marco.ClientSize.Height - marco.Padding.Vertical : campo.PreferredSize.Height;
            campo.SetBounds(marco.Padding.Left, Math.Max(marco.Padding.Top, (marco.Height - altura) / 2),
                Math.Max(1, marco.ClientSize.Width - marco.Padding.Horizontal), Math.Max(1, altura));
        }

        marco.Controls.Add(campo);
        marco.Resize += (_, _) => Alinear();
        campo.FontChanged += (_, _) => Alinear();
        marco.Enter += (_, _) => marco.Invalidate();
        marco.Leave += (_, _) => marco.Invalidate();
        marco.Click += (_, _) => campo.Focus();
        Alinear();
        return marco;
    }

    internal static Label Rotulo(string texto, bool secundario = false) => new()
    {
        Text = texto, AutoSize = true, Dock = DockStyle.Fill,
        ForeColor = secundario ? EstiloAgenda.Secundario : EstiloAgenda.Tinta, Margin = Padding.Empty
    };
}

internal static class DialogoCitorna
{
    internal static void Configurar(Form formulario, string titulo, string subtitulo, Size tamano, Control cuerpo,
        Button principal, Button cancelar, Button? auxiliar = null)
    {
        formulario.SuspendLayout();
        formulario.Text = titulo;
        formulario.ClientSize = tamano;
        formulario.MinimumSize = new Size(365, 325);
        formulario.AutoScaleMode = AutoScaleMode.Dpi;
        formulario.AutoScaleDimensions = new SizeF(96F, 96F);
        formulario.StartPosition = FormStartPosition.CenterParent;
        formulario.ShowInTaskbar = false;
        formulario.MaximizeBox = false;
        formulario.MinimizeBox = false;
        formulario.BackColor = Color.White;
        formulario.ForeColor = EstiloAgenda.Tinta;
        var fuente = new Font("Segoe UI", 11F);
        var fuenteTitulo = new Font("Segoe UI", 21F, FontStyle.Bold);
        formulario.Font = fuente;
        formulario.Disposed += (_, _) => { fuenteTitulo.Dispose(); fuente.Dispose(); };

        var estructura = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24, 19, 24, 14),
            ColumnCount = 1, RowCount = 4 };
        estructura.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        estructura.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        estructura.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        estructura.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        estructura.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

        Label encabezado = CampoCitorna.Rotulo(titulo);
        encabezado.Font = fuenteTitulo;
        encabezado.Margin = new Padding(0, 0, 0, 4);
        Label descripcion = CampoCitorna.Rotulo(subtitulo, true);
        descripcion.Margin = new Padding(0, 0, 0, 13);
        var superficie = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Margin = Padding.Empty };
        cuerpo.Dock = DockStyle.Top;
        superficie.Controls.Add(cuerpo);

        var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft,
            Margin = new Padding(0, 8, 0, 0), WrapContents = auxiliar != null };
        principal.Margin = new Padding(7, 2, 0, 0);
        cancelar.Margin = new Padding(7, 2, 0, 0);
        cancelar.DialogResult = DialogResult.Cancel;
        acciones.Controls.Add(principal);
        acciones.Controls.Add(cancelar);
        if (auxiliar != null)
        {
            acciones.Controls.Add(auxiliar);
            acciones.SizeChanged += (_, _) =>
            {
                int minimo = acciones.Controls.Cast<Button>().Sum(b => b.PreferredSize.Width + b.Margin.Horizontal);
                int altura = formulario.LogicalToDeviceUnits(acciones.Width < minimo ? 108 : 60);
                if (estructura.RowStyles[3].Height != altura) estructura.RowStyles[3].Height = altura;
            };
        }

        estructura.Controls.Add(encabezado, 0, 0);
        estructura.Controls.Add(descripcion, 0, 1);
        estructura.Controls.Add(superficie, 0, 2);
        estructura.Controls.Add(acciones, 0, 3);
        formulario.Controls.Add(estructura);
        formulario.AcceptButton = principal;
        formulario.CancelButton = cancelar;
        formulario.Shown += (_, _) =>
        {
            Rectangle espacio = Screen.FromControl(formulario).WorkingArea;
            int margen = formulario.LogicalToDeviceUnits(12);
            formulario.Size = new Size(Math.Min(formulario.Width, espacio.Width - 2 * margen),
                Math.Min(formulario.Height, espacio.Height - 2 * margen));
            formulario.Location = new Point(
                Math.Clamp(formulario.Left, espacio.Left, Math.Max(espacio.Left, espacio.Right - formulario.Width)),
                Math.Clamp(formulario.Top, espacio.Top, Math.Max(espacio.Top, espacio.Bottom - formulario.Height)));
        };
        formulario.ResumeLayout(true);
    }

    internal static Control Campos(int columnas, params (string Nombre, Control Campo, int Alto)[] campos)
    {
        var celdas = campos.Select(c =>
        {
            var celda = Tabla();
            celda.RowCount = 2;
            celda.Margin = new Padding(0, 0, columnas == 1 ? 0 : 10, 9);
            celda.Controls.Add(CampoCitorna.Rotulo(c.Nombre), 0, 0);
            var marco = CampoCitorna.Enmarcar(c.Campo, c.Nombre, c.Alto);
            marco.Dock = DockStyle.Top;
            marco.Margin = new Padding(0, 3, 0, 0);
            celda.Controls.Add(marco, 0, 1);
            return celda;
        }).ToArray();

        return DisponerColumnas(columnas, celdas);
    }

    internal static TableLayoutPanel Tabla()
    {
        var tabla = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink, ColumnCount = 1, Margin = Padding.Empty };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        return tabla;
    }

    internal static Control DisponerColumnas(int columnas, params Control[] grupos)
    {
        var tabla = Tabla();
        int modoActual = 0;
        void Disponer()
        {
            int modo = columnas > 1 && tabla.Width >= tabla.LogicalToDeviceUnits(490) ? columnas : 1;
            if (modoActual == modo) return;
            modoActual = modo;
            tabla.SuspendLayout();
            tabla.Controls.Clear();
            tabla.ColumnStyles.Clear();
            tabla.RowStyles.Clear();
            tabla.ColumnCount = modo;
            tabla.RowCount = (grupos.Length + modo - 1) / modo;
            for (int i = 0; i < modo; i++) tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / modo));
            for (int i = 0; i < tabla.RowCount; i++) tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            for (int i = 0; i < grupos.Length; i++)
            {
                grupos[i].Dock = DockStyle.Top;
                tabla.Controls.Add(grupos[i], i % modo, i / modo);
            }
            tabla.ResumeLayout(true);
        }

        Disponer();
        tabla.SizeChanged += (_, _) => Disponer();
        return tabla;
    }
}
