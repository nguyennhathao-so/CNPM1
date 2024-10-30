using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public partial class ChiTietDonHang : Form
    {
        public ChiTietDonHang()
        {
            InitializeComponent();
        }

        private void ChiTiet_Load(object sender, EventArgs e)
        {
            nenChiTiet.FillColor = Color.FromArgb(153, 217, 217, 217);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();   
        }


    }
}
