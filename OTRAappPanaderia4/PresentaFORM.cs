using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OTRAappPanaderia2
{

    
    public partial class PresentaFORM : Form
    {
        private Timer temporizador;


        public PresentaFORM()
        {
            InitializeComponent();

            temporizador = new Timer();
            temporizador.Interval = 3000;

            temporizador.Tick += Temporizador_Tick;
            temporizador.Start();

        }

        private void Temporizador_Tick(object sender, EventArgs e)
        {

            // Detener el temporizador
            temporizador.Stop();
            this.Hide();


        }
        
    }
}
