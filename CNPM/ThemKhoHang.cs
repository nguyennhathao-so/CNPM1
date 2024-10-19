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
    public partial class ThemKhoHang : Form
    {
        public ThemKhoHang()
        {
            InitializeComponent();
        }

        private void ThemKhoHang_Load(object sender, EventArgs e)
        {
            nenChiTiet.FillColor = Color.FromArgb(153, 217, 217, 217);
        }

        private void ExitKhoHang_Click(object sender, EventArgs e)
        {
            this.Close();   
        }

        private void LuuKhoHang_Click(object sender, EventArgs e)
        {
            ChiTietKhoHang ctkh= new ChiTietKhoHang();
            ctkh.ShowDialog();
            this.Show();
            this.Close();
            

        }
    }
}
