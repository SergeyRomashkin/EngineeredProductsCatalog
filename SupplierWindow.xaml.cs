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
    /// Interaction logic for SupplierWindow.xaml
    /// </summary>
    /// 
    public partial class SupplierWindow : Window
    {
        string connectionString;
        SqlDataAdapter adapter;
        string supplierName;
        string supplierId;
        string productId;
        string currentWebsite;

        public SupplierWindow(string supplier_name, string supplier_id, string product_id, string connection)
        {
            InitializeComponent();
            connectionString = connection;
            supplierId = supplier_id;
            productId = product_id;
            supplierName = supplier_name;

            this.Title += " - " + supplierName;
        }

        private void ButtonCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM supplier WHERE id = " + supplierId + ";";
                SqlCommand command = new SqlCommand(query, connection);
                DataTable dataTable = new DataTable();

                
                string name = "";
                string website = "";
                string email = "";
                string sales_dep = "";
                string sales_manager = "";
                string mailing = "";
                string isManuf = "";

                try
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        name = reader[1].ToString();
                        website = reader[6].ToString();
                        email = reader[3].ToString();
                        sales_dep = reader[4].ToString();
                        sales_manager = reader[5].ToString();
                        mailing = reader[2].ToString();
                        isManuf = reader[7].ToString();
                    }
                    reader.Close();

                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable); // TODO: Add bindings
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
                currentWebsite = website;


                
                FlowDocument flowDocumentWebsite = new FlowDocument();
                FlowDocument flowDocumentEmail = new FlowDocument();
                

                if (!string.IsNullOrEmpty(website) && !string.IsNullOrWhiteSpace(website))
                {
                    Paragraph paragraph = new Paragraph();
                    Hyperlink linkWebsite = new Hyperlink();
                    linkWebsite.IsEnabled = true;
                    linkWebsite.Inlines.Add(website);
                    linkWebsite.NavigateUri = new Uri(website);
                    //hyperlink.RequestNavigate += OpenHyperlink(website);
                    paragraph.Inlines.Add(linkWebsite);
                    flowDocumentWebsite.Blocks.Add(paragraph);
                }

                if (!string.IsNullOrEmpty(website) && !string.IsNullOrWhiteSpace(website))
                {
                    Paragraph paragraph = new Paragraph();
                    Hyperlink linkEmail = new Hyperlink();
                    linkEmail.IsEnabled = true;
                    linkEmail.Inlines.Add(email);
                    linkEmail.NavigateUri = new Uri("mailto:" + email);
                    paragraph.Inlines.Add(linkEmail);
                    flowDocumentEmail.Blocks.Add(paragraph);
                }

                TextBoxSupplierName.Text = name;
                RichTextBoxSupplierWebsite.Document = flowDocumentWebsite;
                RichTextBoxSupplierEmail.Document = flowDocumentEmail;
                TextBoxSupplierSalesDep.Text = sales_dep;
                TextBoxSupplierSalesManager.Text = sales_manager;
                TextBoxSupplierAddress.Text = mailing;
                TextBlockSupplierIsManufacturer.Text = isManuf;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM deal WHERE supplier_id = " + supplierId + " AND product_id = " + productId + ";";
                SqlCommand command = new SqlCommand(query, connection);
                DataTable dataTable = new DataTable();
                
                string price = "";
                string manufacture_time = "";

                try
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        price = reader[3].ToString();
                        manufacture_time = reader[4].ToString();
                    }
                    reader.Close();

                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable); // TODO: Add bindings
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

                Trace.WriteLine(price);

                TextBlockPrice.Text = price + " руб.";
                TextBlockManufactureTime.Text = manufacture_time + " д.";
            }
        }

        private void OpenHyperlink(string link)
        {
            Process.Start(link);
        }

        private void RichTextBoxSupplierWebsite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenHyperlink(currentWebsite);
        }
    }
}
