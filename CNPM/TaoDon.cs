using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CNPM
{
    public partial class TaoDon : UserControl
    {
        private string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

        public TaoDon()
        {
            InitializeComponent();
            LoadProductData();
            
        }

        // Load sản phẩm từ database vào DataGridView
        private DataTable dataTable;
        private void LoadProductData()
        {
            try
            {
                // Không tự động tạo thêm cột nếu DataGridView đã có định nghĩa cột sẵn
                guna2DataGridView1.AutoGenerateColumns = false;

                // Chuỗi kết nối tới SQL Server
                string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

                // Câu lệnh truy vấn SQL
               string query = @"SELECT p.ProductID AS 'Mã sản phẩm', p.ProductName AS 'Tên sản phẩm', p.Stock - SUM(ISNULL(od.Quantity, 0)) AS 'Số lượng', 
                                p.Price AS 'Giá sản phẩm' 
                                FROM Products p
                                LEFT JOIN OrderDetails od ON p.ProductID = od.ProductID
                                GROUP BY p.ProductID, p.ProductName, p.Stock, p.Price";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            // Tạo DataTable để chứa kết quả truy vấn
                            dataTable = new DataTable();

                            // Điền dữ liệu từ truy vấn vào DataTable
                            adapter.Fill(dataTable);
                            foreach (DataRow dr in dataTable.Rows)
                            {
                                decimal giaSanPham = Convert.ToDecimal(dr["Giá sản phẩm"]);

                                // Kiểm tra nếu giá sản phẩm lớn hơn 1_000_000 thì chia để tính theo triệu
                                if (giaSanPham > 1_000_000)
                                {
                                    dr["Giá sản phẩm"] = giaSanPham / 1_000_000;  // Chuyển đổi thành triệu
                                }
                                // Nếu giá nhỏ hơn hoặc bằng 1_000_000 thì giữ nguyên
                            }

                            // Gán DataTable làm nguồn dữ liệu cho DataGridView
                            guna2DataGridView1.DataSource = dataTable;
                            this.Column1.DataPropertyName = "Mã sản phẩm";  
                            this.Column2.DataPropertyName = "Tên sản phẩm";  
                            this.Column3.DataPropertyName = "Số lượng"; 
                            this.Column4.DataPropertyName = "Giá sản phẩm"; 

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            string filterText = guna2TextBox1.Text.Trim().ToLower(); // Lấy dữ liệu từ searchbar và chuyển sang chữ thường

            // Sử dụng DataView để lọc dữ liệu
            DataView dv = dataTable.DefaultView;

            // Chỉnh sửa phần này: sử dụng tên cột khớp với tên cột trong DataTable
            dv.RowFilter = string.Format("[Tên sản phẩm] LIKE '%{0}%'", filterText); // Lọc theo cột 'Tên sản phẩm'

            guna2DataGridView1.DataSource = dv.ToTable(); // Cập nhật lại DataGridView với kết quả lọc
        }

    }
}
