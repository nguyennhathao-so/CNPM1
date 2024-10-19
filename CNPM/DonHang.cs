using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace CNPM
{
    public partial class DonHang : UserControl
    {
        string connectstring = @"Data Source=LAPTOP-Q7I59CRL\SQLEXPRESS;Integrated Security=True";
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adt;
        DataTable dt;
        public DonHang()
        {
            InitializeComponent();
        }
        private void DonHang_Load(object sender, EventArgs e)
        {
            con=new SqlConnection(connectstring);
        }

        public void hienthi()
        {
            try
            {
                // Mở kết nối
                con.Open();

                // Câu truy vấn SQL để lấy dữ liệu
                string query = "SELECT MaDonHang, TenKhachHang,SoDienThoai,MaVanChuyen,DonViVanChuyen, NgayDatHang FROM ()"; // Điều chỉnh theo bảng của bạn
                cmd = new SqlCommand(query, con);

                // Tạo SqlDataAdapter để lấy dữ liệu
                adt = new SqlDataAdapter(cmd);

                // Khởi tạo DataTable để chứa dữ liệu
                dt = new DataTable();

                // Đổ dữ liệu từ DataAdapter vào DataTable
                adt.Fill(dt);

                // Gán dữ liệu cho từng cột của Guna2DataGridView
                DataGridViewDonhang.AutoGenerateColumns = false;  // Tắt tự động tạo cột

                // Gán dữ liệu vào cột đã tạo sẵn
                DataGridViewDonhang.Columns[0].DataPropertyName = "MaDonHang";
                DataGridViewDonhang.Columns[1].DataPropertyName = "TenKhachHang";
                DataGridViewDonhang.Columns[2].DataPropertyName = "NgayDatHang";
                DataGridViewDonhang.Columns[3].DataPropertyName = "TongTien";
                /*DataGridViewDonhang.Columns[4].DataPropertyName =*/

                // Gán DataTable làm DataSource cho Guna2DataGridView
                /*DataGridViewDonhang.DataSource = dt;*/
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                // Đóng kết nối sau khi lấy dữ liệu
                con.Close();
            }
        }

        private void ChiTiet_Click(object sender, EventArgs e)
        {
            ChiTiet ct= new ChiTiet();  
            ct.ShowDialog();
            this.Show();
        }
    }
}
