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
    public partial class ChiTietKhoHang : Form
    {
        public ChiTietKhoHang()
        {
            InitializeComponent();
        }

        private void ChiTietKhoHang_Load(object sender, EventArgs e)
        {
            nenChiTiet.FillColor= Color.FromArgb(153,255,255,255);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Sua_Click(object sender, EventArgs e)
        {

        }
    }
}
