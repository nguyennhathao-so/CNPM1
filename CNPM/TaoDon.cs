using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CNPM
{
    public partial class TaoDon : UserControl
    {
        private decimal selectedProductPrice = 0;  // Lưu giá sản phẩm đã chọn
        private string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

        public TaoDon()
        {
            InitializeComponent();
            LoadProductData();
            // Sự kiện double click trên DataGridView
            guna2DataGridView1.CellDoubleClick += Guna2DataGridView1_CellDoubleClick;
        }

        // Tải sản phẩm từ cơ sở dữ liệu lên DataGridView
        private DataTable dataTable;
        private void LoadProductData()
        {
            try
            {
                guna2DataGridView1.AutoGenerateColumns = false;
                string query = @"SELECT p.ProductID AS 'Mã Sản Phẩm', p.ProductName AS 'Tên sản phẩm', 
                                p.Stock - SUM(ISNULL(od.Quantity, 0)) AS 'Số lượng', p.Price AS 'Giá sản phẩm' 
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
                            dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            guna2DataGridView1.DataSource = dataTable;
                            this.Column1.DataPropertyName = "Mã Sản Phẩm";
                            this.Column2.DataPropertyName = "Tên sản phẩm";
                            this.Column3.DataPropertyName = "Số lượng";
                            this.Column4.DataPropertyName = "Giá sản phẩm";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            string filterText = guna2TextBox1.Text.Trim().ToLower();
            DataView dv = dataTable.DefaultView;
            dv.RowFilter = string.Format("[Tên sản phẩm] LIKE '%{0}%'", filterText);
            guna2DataGridView1.DataSource = dv.ToTable();
        }

        // Xử lý sự kiện double click vào sản phẩm trong DataGridView
        private void Guna2DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataRow selectedRow = dataTable.Rows[e.RowIndex];
                decimal price = Convert.ToDecimal(selectedRow["Giá sản phẩm"]);
                selectedProductPrice = price;  // Lưu giá sản phẩm đã chọn

                // Thiết lập số lượng là 1 khi double-click
                int quantity = 1;

                // Hiển thị giá và số lượng trong giao diện (nếu cần)
                tien1.Text = price.ToString("#,##0");
            }
        }

        // Sinh OrderID theo định dạng GUID
        private string GenerateOrderId()
        {
            return Guid.NewGuid().ToString();  // Tạo GUID mới cho OrderID
        }

        // Thêm hoặc cập nhật khách hàng (CustomerID là cột IDENTITY)
        private int AddOrUpdateCustomer(SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"INSERT INTO Customers (Name, Phone, Email, DateOfBirth, Address) 
                             VALUES (@Name, @Phone, @Email, @DateOfBirth, @Address);
                             SELECT SCOPE_IDENTITY();";  // Lấy CustomerID vừa tạo (IDENTITY)

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Name", TextBoxKhachhang.Text);
                cmd.Parameters.AddWithValue("@Phone", TextBoxSDT.Text);
                cmd.Parameters.AddWithValue("@Email", TextBoxEmail.Text);
                cmd.Parameters.AddWithValue("@DateOfBirth", NgaySinh.Value);
                cmd.Parameters.AddWithValue("@Address", TextBoxDiaChi.Text);

                return Convert.ToInt32(cmd.ExecuteScalar());  // Trả về CustomerID từ SCOPE_IDENTITY()
            }
        }

        // Thêm thông tin vận chuyển vào bảng Shipping2 và trả về ShippingID
        private Guid AddShippingInfo(SqlConnection conn, SqlTransaction transaction)
        {
            Guid shippingId = Guid.NewGuid();  // Ensure this is a valid GUID

            string query = @"INSERT INTO Shipping2 (ShippingID, ShippingCo, ShippingFee, ShippingCode) 
                     VALUES (@ShippingID, @ShippingCo, @ShippingFee, @ShippingCode)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                // Ensure ShippingID is passed as a GUID
                cmd.Parameters.Add("@ShippingID", SqlDbType.UniqueIdentifier).Value = shippingId;
                cmd.Parameters.AddWithValue("@ShippingCo", TextBoxDonViVanChuyen.Text);
                cmd.Parameters.AddWithValue("@ShippingFee", decimal.Parse(TextBoxPhi.Text));
                cmd.Parameters.AddWithValue("@ShippingCode", TextBoxMaVanChuyen.Text);

                cmd.ExecuteNonQuery();
            }

            return shippingId;  // Return the valid GUID
        }

        // Thêm đơn hàng với OrderID tự sinh và liên kết với thông tin vận chuyển
        private string AddOrder(SqlConnection conn, SqlTransaction transaction, int customerId, Guid shippingId)
        {
            string orderId = GenerateOrderId();  // Tạo OrderID mới (GUID)

            // Chèn dữ liệu vào bảng Orders, bao gồm ShippingAddress, ShippingCo và ShippingID
            string query = @"INSERT INTO Orders (OrderID, CustomerID, OrderDate, ShippingID, ShippingAddress, ShippingCo, TotalPrice, OrderStatus) 
                     VALUES (@OrderID, @CustomerID, @OrderDate, @ShippingID, @ShippingAddress, @ShippingCo, @TotalPrice, @OrderStatus)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@OrderID", SqlDbType.UniqueIdentifier).Value = new Guid(orderId);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                cmd.Parameters.Add("@ShippingID", SqlDbType.UniqueIdentifier).Value = shippingId;  // Ensure this is passed as GUID
                cmd.Parameters.AddWithValue("@ShippingAddress", TextBoxDiaChi.Text);
                cmd.Parameters.AddWithValue("@ShippingCo", TextBoxDonViVanChuyen.Text);
                cmd.Parameters.AddWithValue("@TotalPrice", decimal.Parse(tien1.Text));
                cmd.Parameters.AddWithValue("@OrderStatus", "Processing");

                cmd.ExecuteNonQuery();
            }

            return orderId;  // Trả về OrderID đã tạo (GUID)
        }

        // Thêm chi tiết đơn hàng vào bảng OrderDetails
        private void AddOrderDetails(SqlConnection conn, SqlTransaction transaction, string orderId)
        {
            foreach (DataGridViewRow row in guna2DataGridView1.SelectedRows)
            {
                string query = @"INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice) 
                                 VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice)";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderId);  // Sử dụng GUID cho OrderID
                    cmd.Parameters.AddWithValue("@ProductID", row.Cells[0].Value);

                    // Thiết lập số lượng là 1
                    cmd.Parameters.AddWithValue("@Quantity", 1);  // Số lượng cố định là 1

                    cmd.Parameters.AddWithValue("@UnitPrice", row.Cells[3].Value);  // Giá sản phẩm

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Xử lý sự kiện nhấn nút "Tạo Đơn"
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();  // Bắt đầu giao dịch

                    // 1. Thêm hoặc cập nhật thông tin khách hàng và lấy CustomerID (IDENTITY)
                    int customerId = AddOrUpdateCustomer(conn, transaction);

                    // 2. Thêm thông tin vận chuyển và lấy ShippingID (GUID)
                    Guid shippingId = AddShippingInfo(conn, transaction);

                    // 3. Tạo đơn hàng và lấy OrderID (GUID)
                    string orderId = AddOrder(conn, transaction, customerId, shippingId);

                    // 4. Thêm chi tiết sản phẩm vào đơn hàng
                    AddOrderDetails(conn, transaction, orderId);

                    transaction.Commit();  // Xác nhận giao dịch nếu mọi thứ thành công
                    MessageBox.Show("Đơn hàng đã được tạo thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
                    if (transaction != null)
                    {
                        transaction.Rollback();  // Hoàn tác giao dịch nếu có lỗi
                    }
                }
            }
        }
    }
}
