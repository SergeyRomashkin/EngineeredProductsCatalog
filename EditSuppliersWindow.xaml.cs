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
    /// Interaction logic for EditSuppliersWindow.xaml
    /// </summary>
    public partial class EditSuppliersWindow : Window
    {
        private SqlDataAdapter adapter;
        string connectionString;

        public EditSuppliersWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void ListBoxResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxResults.SelectedIndex != -1)
            {
                StackPanelDetails.Visibility = Visibility.Visible;
                
                DataRowView dataRowView = ListBoxResults.SelectedItem as DataRowView; 
                string supplierId = "";

                if (dataRowView != null)
                {
                    supplierId = dataRowView.Row["id"].ToString();
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM supplier WHERE id = " + supplierId + ";";
                    SqlCommand command = new SqlCommand(query, connection);
                    DataTable dataTable = new DataTable();

                    try
                    {
                        connection.Open();
                        adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        GridDetails.DataContext = dataTable.DefaultView;
                        this.DataContext = dataTable.DefaultView;

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
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM supplier WHERE name LIKE '%" + TextBoxSearch.Text + "%';";
                SqlCommand command = new SqlCommand(query, connection);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();

                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable); // TODO: Add bindings
                    ListBoxResults.ItemsSource = dataTable.DefaultView;
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
                    ListBoxResults.ItemsSource = dataTable.DefaultView;
                    ListBoxResults.SelectedValuePath = "id";
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
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string name = null;
            string mail = null;
            string email = null;
            string salesDep = null;
            string salesMan = null;
            string website = null;
            string isManuf = null;

            // TODO: Remove? Maybe don't need it
            //if (!string.IsNullOrEmpty(TextBoxSupplierName.Text))
            {
                name = TextBoxSupplierName.Text;
            }

            //if (!string.IsNullOrEmpty(TextBoxDetails_mailing_address.Text))
            {
                mail = TextBoxDetails_mailing_address.Text;
            }

            //if (!string.IsNullOrEmpty(TextBoxDetails_email_address.Text))
            {
                email = TextBoxDetails_email_address.Text;
            }

            //if (!string.IsNullOrEmpty(TextBoxDetails_sales_department_phone_number.Text))
            {
                salesDep = TextBoxDetails_sales_department_phone_number.Text;
            }

            //if (!string.IsNullOrEmpty(TextBoxDetails_sales_manager_phone_number.Text))
            {
                salesMan = TextBoxDetails_sales_manager_phone_number.Text;
            }

            //if (!string.IsNullOrEmpty(TextBoxDetails_website.Text))
            {
                website = TextBoxDetails_website.Text;
            }

            //if (!string.IsNullOrEmpty(TextBoxDetails_is_manufacturer.Text))
            {
                isManuf = TextBoxDetails_is_manufacturer.Text;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE supplier SET name = @name, mailing_address = @mail, email_address = @email, sales_department_phone_number = @salesDep, sales_manager_phone_number = @salesMan, website = @website, is_manufacturer = @isManuf WHERE id = @id;";
                SqlCommand command = new SqlCommand(query, connection);
                DataTable dataTable = new DataTable();

                DataRowView dataRowView = ListBoxResults.SelectedItem as DataRowView;
                string supplierId = ""; ;

                if (dataRowView != null)
                {
                    supplierId = dataRowView.Row["id"].ToString();
                }

                command.Parameters.AddWithValue("@id", supplierId);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@mail", mail);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@salesDep", salesDep);
                command.Parameters.AddWithValue("@salesMan", salesMan);
                command.Parameters.AddWithValue("@website", website);
                command.Parameters.AddWithValue("@isManuf", isManuf);

                try
                {
                    connection.Open();
                    adapter = new SqlDataAdapter(command);
                    command.ExecuteNonQuery();
                    RefreshListBoxResults();
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
        }

        private void RefreshListBoxResults()
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
                    ListBoxResults.ItemsSource = dataTable.DefaultView;
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
        }

        private void ButtonAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            string supplierName = "Новый поставщик";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO supplier (name) VALUES ('" + supplierName + "');";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    RefreshListBoxResults();
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

        private void ButtonRemoveSupplier_Click(object sender, RoutedEventArgs e)
        {
            string supplierId = ListBoxResults.SelectedValue.ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM supplier WHERE id = " + supplierId+ ";";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    RefreshListBoxResults();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show("Не удалось удалить поставщика.\nВозможно, предложения этого поставщика не удалены?", "Не удалось удалить поставщика", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
        }
    }
}
