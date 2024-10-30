using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CNPM
{
    public partial class DonHang : UserControl
    {
        private DataTable orderDataTable;

        public DonHang()
        {
            InitializeComponent();
            LoadOrderData(); // Tải dữ liệu đơn hàng khi khởi tạo
            TimKiem.TextChanged += TimKiem_TextChanged; // Gắn sự kiện tìm kiếm
            LoadOrderStatusCounts(); // Load the counts when initializing the control

        }


        // Hàm tải dữ liệu đơn hàng vào DataGridView và ẩn TrongPicture nếu có dữ liệu
        private void LoadOrderData()
        {
            DataGridViewDonhang.AutoGenerateColumns = false;
            DataGridViewDonhang.Columns.Clear();  // Xóa các cột đã có để tránh bị trùng

            string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

            try
            {
                string query = @"
                SELECT 
                    o.OrderID AS 'Mã đơn hàng', 
                    c.Name AS 'Tên người nhận', 
                    c.Phone AS 'Số điện thoại', 
                    s.ShippingCode AS 'Mã vận chuyển',
                    s.ShippingCo AS 'Đơn vị vận chuyển', 
                    o.OrderDate AS 'Thời gian đặt hàng'
                FROM 
                    Orders o
                JOIN 
                    Customers c ON o.CustomerID = c.CustomerID
                LEFT JOIN 
                    Shipping2 s ON s.OrderID = o.OrderID";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            orderDataTable = new DataTable();
                            adapter.Fill(orderDataTable);

                            if (orderDataTable.Rows.Count > 0)
                            {
                                DataGridViewDonhang.DataSource = orderDataTable;

                                // Định nghĩa các cột cho DataGridView
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Mã đơn hàng",
                                    DataPropertyName = "Mã đơn hàng",
                                    HeaderText = "Mã đơn hàng"
                                });
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Tên người nhận",
                                    DataPropertyName = "Tên người nhận",
                                    HeaderText = "Tên người nhận"
                                });
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Số điện thoại",
                                    DataPropertyName = "Số điện thoại",
                                    HeaderText = "Số điện thoại"
                                });
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Mã vận chuyển",
                                    DataPropertyName = "Mã vận chuyển",
                                    HeaderText = "Mã vận chuyển"
                                });
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Đơn vị vận chuyển",
                                    DataPropertyName = "Đơn vị vận chuyển",
                                    HeaderText = "Đơn vị vận chuyển"
                                });
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Thời gian đặt hàng",
                                    DataPropertyName = "Thời gian đặt hàng",
                                    HeaderText = "Thời gian đặt hàng"
                                });

                                // Ẩn TrongPicture nếu có dữ liệu
                                TrongPicture.Visible = false;
                            }
                            else
                            {
                                // Hiện TrongPicture nếu không có dữ liệu
                                TrongPicture.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                // Hiện TrongPicture nếu xảy ra lỗi
                TrongPicture.Visible = true;
            }
        }
        private void LoadOrderStatusCounts()
        {
            // Dictionary to hold the count for each order status
            var statusCounts = new Dictionary<string, int>
            {
                { "Tất cả", 0 },
                { "Đã hủy", 0 },
                { "Cần xử lí", 0 },
                { "Đã xác nhận", 0 },
                { "Đang chuẩn bị", 0 },
                { "Chờ gửi hàng", 0 },
                { "Đã gửi", 0 },
                { "Đã nhận", 0 }
            };

            try
            {
                string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Query to get the count of each order status
                    string query = @"SELECT OrderStatus, COUNT(*) AS StatusCount 
                                     FROM Orders 
                                     GROUP BY OrderStatus";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string status = reader["OrderStatus"].ToString();
                                int count = Convert.ToInt32(reader["StatusCount"]);

                                // Update the dictionary with counts
                                if (statusCounts.ContainsKey(status))
                                {
                                    statusCounts[status] = count;
                                }
                            }
                        }
                    }
                }

                // Update each label based on the status count dictionary
                number1.Text = statusCounts["Tất cả"].ToString();
                number2.Text = statusCounts["Đã hủy"].ToString();
                number3.Text = statusCounts["Cần xử lí"].ToString();
                number4.Text = statusCounts["Đã xác nhận"].ToString();
                number5.Text = statusCounts["Đang chuẩn bị"].ToString();
                number6.Text = statusCounts["Chờ gửi hàng"].ToString();
                number7.Text = statusCounts["Đã gửi"].ToString();
                number8.Text = statusCounts["Đã nhận"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private void TimKiem_TextChanged(object sender, EventArgs e)
        {
            string filterText = TimKiem.Text.Trim();

            if (orderDataTable != null)
            {
                if (!string.IsNullOrEmpty(filterText))
                {
                    // Lọc theo mã đơn hàng, tên người nhận, số điện thoại, mã vận chuyển
                    orderDataTable.DefaultView.RowFilter =
                        $"[Mã đơn hàng] LIKE '%{filterText}%' OR " +
                        $"[Tên người nhận] LIKE '%{filterText}%' OR " +
                        $"[Số điện thoại] LIKE '%{filterText}%' OR " +
                        $"[Mã vận chuyển] LIKE '%{filterText}%'";
                }
                else
                {
                    // Bỏ lọc nếu không có văn bản tìm kiếm
                    orderDataTable.DefaultView.RowFilter = string.Empty;
                }

                // Cập nhật DataGridView
                DataGridViewDonhang.DataSource = orderDataTable.DefaultView;
            }
        }

        private void DonHang_Load(object sender, EventArgs e)
        {

        }
    }
}
