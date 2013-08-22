using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChessLG
{
    public partial class Promocion : Form
    {
        public Ficha seleccionada = null;
        public Peon peon;
        bool color;

        public Promocion(Peon peon)
        {
            InitializeComponent();
            this.color = peon.color;
            this.peon = peon;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0: // Reina
                    seleccionada = new Reina(color, peon.miCasilla);
                    break;
                case 1: // Caballo
                    seleccionada = new Caballo(color, peon.miCasilla);
                    break;
                case 2: // Torre
                    seleccionada = new Torre(color, peon.miCasilla);
                    break;
                case 3: // Alfil
                    seleccionada = new Alfil(color, peon.miCasilla);
                    break;
                default:
                    break;
            }

            seleccionada.miCasilla.ficha = seleccionada;

            seleccionada.actualizarAmenazas(seleccionada.miCasilla);
            seleccionada.actualizarMovimientos(seleccionada.miCasilla);

            this.Close();
        }
    }
}
