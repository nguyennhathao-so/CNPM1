using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CNPM
{
    public partial class NoiDungTrangChu : UserControl
    {
        private Chart myNewChart;

        public NoiDungTrangChu()
        {
            InitializeComponent();
            // Call LoadData() when the control is initialized to fetch and display data
            LoadData();
            
            CreateNewChart();  // Create the new chart
            LoadChartData();    // Load data into the new chart
        }

        // Method to retrieve data from the SQL database and display it in the DataGridView
        private void LoadData()
        {
            try
            {
                // Set AutoGenerateColumns to false to avoid creating new columns
                bangTrangChu.AutoGenerateColumns = false;

                // Connection string to SQL Server
                string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

                // SQL query to retrieve data
                string query = @"
                SELECT p.ProductName AS SanPham, 
                       SUM(od.Quantity) AS SoLuong, 
                       SUM(od.Quantity * od.UnitPrice) AS DoanhThu
                FROM OrderDetails od
                JOIN Products p ON od.ProductID = p.ProductID
                GROUP BY p.ProductName";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable data1 = new DataTable();
                            adapter.Fill(data1);

                            // Ensure that data is retrieved and not null
                            if (data1 != null && data1.Rows.Count > 0)
                            {
                                foreach (DataRow row in data1.Rows)
                                {
                                    // Convert DoanhThu to millions (triệu) for display
                                    row["DoanhThu"] = Convert.ToDecimal(row["DoanhThu"]) / 1_000_000;
                                }

                                // Bind the DataTable to the DataGridView without creating new columns
                                bangTrangChu.DataSource = data1;
                                this.Column1.DataPropertyName = "SanPham";  // For 'Sản phẩm' column
                                this.Column2.DataPropertyName = "SoLuong";  // For 'Số lượng' column
                                this.Column3.DataPropertyName = "DoanhThu"; // For 'Doanh Thu' column
                                //bangTrangChu.Columns["DoanhThu"].DefaultCellStyle.Format = "N2";

                            }
                            else
                            {
                                MessageBox.Show("No data found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Show detailed error message
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void BieuDo_Click(object sender, EventArgs e)
        {
        }


        // Method to create a new chart and add it to the form
        private void CreateNewChart()
        {
            myNewChart = new Chart();
            myNewChart.Size = new System.Drawing.Size(500,400);
            myNewChart.Location = new System.Drawing.Point(-10,161);  // Set the location for the new chart

            // Create a new chart area
            ChartArea chartArea = new ChartArea();
            chartArea.AxisX.Title = "Tháng";
            chartArea.AxisY.Title = "Doanh thu (VND)";
            myNewChart.ChartAreas.Add(chartArea);

            // Create a new series
            Series series = new Series();
            series.ChartType = SeriesChartType.Column;  // Set the type of chart (Column chart)
            myNewChart.Series.Add(series);

            // Add the chart to the form
            this.Controls.Add(myNewChart);
        }

        // Method to load data into the new chart
        private void LoadChartData()
        {
            try
            {
                // SQL query to retrieve DoanhThu by month (assuming OrderDate is the date of the order)
                string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

                // SQL query to sum DoanhThu for each month
                string query = @"
                SELECT MONTH(o.OrderDate) AS Thang, 
                       SUM(od.Quantity * od.UnitPrice) AS DoanhThu
                FROM OrderDetails od
                JOIN Orders o ON od.OrderID = o.OrderID
                WHERE MONTH(o.OrderDate) BETWEEN 1 AND 12  -- Only include valid months
                GROUP BY MONTH(o.OrderDate)
                ORDER BY MONTH(o.OrderDate)";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable data = new DataTable();
                            adapter.Fill(data);

                            // Clear previous data points from the chart
                            myNewChart.Series[0].Points.Clear();

                            // Add points to the chart
                            foreach (DataRow row in data.Rows)
                            {
                                // X value is the month (Thang), Y value is the DoanhThu
                                int month = Convert.ToInt32(row["Thang"]);
                                decimal revenue = Convert.ToDecimal(row["DoanhThu"]) / 1_000_000;

                                // Add the data to the chart series
                                myNewChart.Series[0].Points.AddXY(month, revenue);
                            }

                            // Optionally set additional chart formatting
                            myNewChart.ChartAreas[0].AxisX.Title = "Tháng";
                            myNewChart.ChartAreas[0].AxisY.Title = "Doanh thu (Triệu VND)";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading chart data: " + ex.Message);
            }
        }
    }
}
