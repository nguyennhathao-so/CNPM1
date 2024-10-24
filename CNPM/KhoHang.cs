using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CNPM
{
    public partial class KhoHang : UserControl
    {
        public KhoHang()
        {
            InitializeComponent();
            LoadProductData(); // Load product data when the user control is initialized
        }

        private void buttonThem_Click(object sender, EventArgs e)
        {
            ThemKhoHang themKhoHang = new ThemKhoHang();
            themKhoHang.ShowDialog();
            LoadProductData(); // Reload data after adding a product
        }

        private void ChiTietKhoHang_Click(object sender, EventArgs e)
        {
            ChiTietKhoHang ctkh = new ChiTietKhoHang();
            ctkh.ShowDialog();
            LoadProductData(); // Reload data after viewing details
        }

        // Load product data into the DataGridView
        private void LoadProductData()
        {
            bangKhoHang.AutoGenerateColumns = false; // Ensure columns are manually defined in the designer
            string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

            try
            {
                // SQL query to fetch product data
                string query = @"
        SELECT 
            p.ProductID AS 'Mã sản phẩm', 
            p.ProductName AS 'Tên sản phẩm', 
            c.CategoryName AS 'Ngành hàng', 
            (p.Stock - ISNULL(SUM(od.Quantity), 0)) AS 'Tồn kho',  -- Calculate remaining stock
            p.Trademark AS 'Thương hiệu', 
            ISNULL(SUM(od.Quantity), 0) AS 'Đã bán'  -- Sum of quantity sold
        FROM 
            Products p
        JOIN 
            Category c ON p.CategoryID = c.CategoryID
        LEFT JOIN 
            OrderDetails od ON od.ProductID = p.ProductID  -- Use LEFT JOIN to include products with no orders
        GROUP BY 
            p.ProductID, 
            p.ProductName, 
            c.CategoryName, 
            p.Stock, 
            p.Trademark;";  // Ensure correct grouping

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open(); // Open the connection
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable data = new DataTable();
                            adapter.Fill(data); // Fill the DataTable with data

                            if (data.Rows.Count > 0)
                            {
                                bangKhoHang.DataSource = data; // Bind data to the DataGridView

                                // Set DataPropertyName for columns (ensure column names match exactly)
                                this.Column1.DataPropertyName = "Mã sản phẩm";  // For 'Mã sản phẩm' column
                                this.Column2.DataPropertyName = "Tên sản phẩm";  // For 'Tên sản phẩm' column
                                this.Column3.DataPropertyName = "Ngành hàng";    // For 'Ngành hàng' column
                                this.Column4.DataPropertyName = "Tồn kho";       // For 'Tồn kho' column
                                this.Column5.DataPropertyName = "Thương hiệu";    // For 'Thương hiệu' column
                                this.Column6.DataPropertyName = "Đã bán";         // For 'Đã bán' column
                            }
                            else
                            {
                                MessageBox.Show("Không có dữ liệu để hiển thị.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message); // Show error message
            }
        }


        private void KhoHang_Load(object sender, EventArgs e)
        {
            // Optionally load data here again if needed
        }
    }
}
