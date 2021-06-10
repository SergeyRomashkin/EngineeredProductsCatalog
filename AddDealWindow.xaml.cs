using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EngineeredProductsCatalog
{
    /// <summary>
    /// Interaction logic for AddDealWindow.xaml
    /// </summary>
    public partial class AddDealWindow : Window
    {
        public bool isOk;
        private string connectionString;
        private SqlDataAdapter adapter;
        private string productId;

        public AddDealWindow(string selectedProductId)
        {
            InitializeComponent();
            isOk = false;
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            productId = selectedProductId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM supplier;";
                SqlCommand command = new SqlCommand(query, connection);

                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();

                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    ComboBoxSupplier.ItemsSource = dataTable.DefaultView;
                    ComboBoxSupplier.DisplayMemberPath = "name";
                    ComboBoxSupplier.SelectedValuePath = "id";
                    ComboBoxSupplier.UpdateLayout();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO deal (product_id, supplier_id, price, manufacture_time) VALUES (@productId, @supplierId, @price, @manufactureTime);";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@productId", productId);
                command.Parameters.AddWithValue("@supplierId", ComboBoxSupplier.SelectedValue.ToString());
                command.Parameters.AddWithValue("@price", TextBoxPrice.Text);
                command.Parameters.AddWithValue("@manufactureTime", TextBoxTime.Text);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }

            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
