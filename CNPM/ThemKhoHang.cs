using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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
            LoadCategories();  // Load categories if needed
            SetNextProductID(); // Set the next product ID when the form loads
        }

        private void LoadCategories()
        {
            // Optionally load categories into a ComboBox if required
            // string connectionString = @"YourConnectionString";
            // string query = "SELECT CategoryID, CategoryName FROM Category";
            // Populate the ComboBox if you have one for categories
        }

        private void SetNextProductID()
        {
            string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";
            string query = "SELECT ISNULL(MAX(ProductID), 0) + 1 FROM Products";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                int nextProductID = (int)cmd.ExecuteScalar(); // Get the next ProductID
                Masp.Text = nextProductID.ToString(); // Set the TextBox with the next ProductID
            }
        }

        private void ExitKhoHang_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LuuKhoHang_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Products (ProductID, ProductName, CategoryName, Price, Description, Origin, Quantity, Weight, Size) 
                                     VALUES (@ProductID, @ProductName, @CategoryName, @Price, @Description, @Origin, @Quantity, @Weight, @Size)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", Masp.Text); // Use the auto-generated ID
                        cmd.Parameters.AddWithValue("@ProductName", Tensp.Text);
                        cmd.Parameters.AddWithValue("@CategoryName", NganhHang.Text); // Directly using TextBox
                        cmd.Parameters.AddWithValue("@Price", decimal.Parse(GiaSp.Text));
                        cmd.Parameters.AddWithValue("@Description", MoTaKhoHang.Text);
                        cmd.Parameters.AddWithValue("@Origin", XuatXu.Text);
                        cmd.Parameters.AddWithValue("@Quantity", int.Parse(Soluong.Text));
                        cmd.Parameters.AddWithValue("@Weight", cannang.Text);
                        cmd.Parameters.AddWithValue("@Size", kichthuoc.Text);

                        cmd.ExecuteNonQuery(); // Execute the insert command
                    }
                }

                MessageBox.Show("Sản phẩm đã được thêm thành công!");
                this.Close(); // Close the form after saving
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm sản phẩm: " + ex.Message);
            }
        }

        private void nenChiTiet_Paint(object sender, PaintEventArgs e)
        {
            // Custom painting code can go here if needed
        }
    }
}
