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
    public partial class KhoHang : UserControl
    {
        public KhoHang()
        {
            InitializeComponent();
        }

        private void buttonThem_Click(object sender, EventArgs e)
        {
            ThemKhoHang themKhoHang = new ThemKhoHang();
            themKhoHang.ShowDialog();
            this.Show();
            
        }

        private void ChiTietKhoHang_Click(object sender, EventArgs e)
        {
            ChiTietKhoHang ctkh = new ChiTietKhoHang();
            ctkh.ShowDialog();
            this.Show();
        }
    }
}
