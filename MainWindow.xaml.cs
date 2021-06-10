using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace EngineeredProductsCatalog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString;
        SqlDataAdapter adapter;

        string selectedCategoryId;

        string selectedItemId;
        string selectedItemName;
        string selectedItemDetailsRaw;
        string selectedItemDetails;
        string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъьыэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЫЭЮЯ0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,:;= ";

        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM category WHERE parent_category_id IS NULL;";
                SqlCommand command = new SqlCommand(query, connection);

                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();

                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    ComboBoxCategory0.ItemsSource = dataTable.DefaultView;
                    ComboBoxCategory0.DisplayMemberPath = "name";
                    ComboBoxCategory0.SelectedValuePath = "id";
                    ComboBoxCategory0.UpdateLayout();
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

        private void ComboBoxCategory0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StackPanel1 != null)
            {
                StackPanel1.Visibility = Visibility.Visible;
            }

            if (StackPanel2 != null)
            {
                StackPanel2.Visibility = Visibility.Collapsed;
            }

            if (ComboBoxCategory1 != null && ComboBoxCategory2 != null)
            {
                ComboBoxCategory1.SelectedIndex = -1;
                ComboBoxCategory2.SelectedIndex = -1;
            }

            DataRowView dataRowView = ComboBoxCategory0.SelectedItem as DataRowView;

            if (dataRowView != null)
            {
                selectedItemId = dataRowView.Row["id"].ToString();
                selectedCategoryId = selectedItemId;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM category WHERE parent_category_id = " + selectedItemId + ";";
                SqlCommand command = new SqlCommand(query, connection);

                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();

                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    ComboBoxCategory1.ItemsSource = dataTable.DefaultView;
                    ComboBoxCategory1.DisplayMemberPath = "name";
                    ComboBoxCategory1.SelectedValuePath = "id";
                    ComboBoxCategory1.UpdateLayout();
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

            GetProductsInCategory(selectedCategoryId);
        }

        private void ComboBoxCategory1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox1 = ComboBoxCategory1;
            ComboBox comboBox2 = ComboBoxCategory2;

            if (comboBox2 != null)
            {
                comboBox2.SelectedIndex = -1;
            }

            DataRowView dataRowView = comboBox1.SelectedItem as DataRowView;

            if (dataRowView != null)
            {
                selectedItemId = dataRowView.Row["id"].ToString();
                selectedCategoryId = selectedItemId;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM category WHERE parent_category_id = " + selectedItemId + ";";
                SqlCommand command = new SqlCommand(query, connection);

                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();

                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count != 0)
                    {
                        comboBox2.ItemsSource = dataTable.DefaultView;
                        comboBox2.DisplayMemberPath = "name";
                        comboBox2.SelectedValuePath = "id";
                        comboBox2.UpdateLayout();

                        if (StackPanel2 != null)
                        {
                            StackPanel2.Visibility = Visibility.Visible;
                        }
                    }                    
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

            GetProductsInCategory(selectedCategoryId);
            //GetDataForFilters((ComboBox)sender);
        }

        private void ComboBoxCategory2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = ComboBoxCategory2;
            DataRowView dataRowView = comboBox.SelectedItem as DataRowView;

            if (dataRowView != null)
            {
                selectedItemId = dataRowView.Row["id"].ToString();
                selectedCategoryId = selectedItemId;
            }

            GetProductsInCategory(selectedCategoryId);
        }

        private void ListBoxResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StackPanelDetails.Visibility = Visibility.Visible;

            GridDetails.Children.Clear();

            // Very important thing (FOR COMBOBOX)
            DataRowView dataRowView = ListBoxResults.SelectedItem as DataRowView;

            if (dataRowView != null)
            {
                selectedItemId = dataRowView.Row["id"].ToString();
                selectedItemName = dataRowView.Row["name"].ToString();
                selectedItemDetailsRaw = dataRowView.Row["details"].ToString();
            }

            selectedItemDetails = "";
            for (int i = 0; i < selectedItemDetailsRaw.Length; i++)
            {
                if (alphabet.Contains(selectedItemDetailsRaw[i]))
                {
                    selectedItemDetails += selectedItemDetailsRaw[i];
                }
            }

            LabelProductName.Content = selectedItemName;



            // for demo
            //List<string> detailNames = new List<string>();
            //List<string> detailValues = new List<string>();

            string[] details = selectedItemDetails.Split(';');
            
            //foreach (var item in details)
            //{
            //    if (!string.IsNullOrEmpty(item) &&
            //        !string.IsNullOrWhiteSpace(item))
            //    {
            //        string[] detail = item.Split('=');

            //        detailNames.Add(detail[0]);
            //        detailValues.Add(detail[1]);
            //    }
            //}

            // not for demo, was here before
            //foreach (var item in details)
            //{
            //    Trace.WriteLine(item);
            //}

            int currentRow = 0;
            foreach (var item in details)
            {
                if (selectedItemDetails != null && !string.IsNullOrEmpty(selectedItemDetails) &&
                    item != null && !string.IsNullOrEmpty(item) && !string.IsNullOrWhiteSpace(item))
                {
                    string[] tmp = item.Split('=');

                    Label LabelTmp = new Label();
                    LabelTmp.Content = tmp[0];
                    GridDetails.Children.Add(LabelTmp);

                    Grid.SetColumn(LabelTmp, 0);
                    Grid.SetRow(LabelTmp, currentRow);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = tmp[1];
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    GridDetails.Children.Add(textBlock);
                    Grid.SetColumn(textBlock, 1);
                    Grid.SetRow(textBlock, currentRow);

                    currentRow++;
                    // TODO: В отчёт: сразу можно зайти на сайт, написать на почту поставщику
                }
            }

            string supplier_id = "";

            // For plain Grid
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    string query = "SELECT * FROM supplier WHERE id=(SELECT supplier_id FROM deal WHERE product_id = " + selectedItemId + ");";
            //    SqlCommand command = new SqlCommand(query, connection);
            //    DataTable dataTable = new DataTable();

            //    supplier_id = "";
            //    string name = "";
            //    string website = "";
            //    string email = "";
            //    string sales_dep = "";
            //    string sales_manager = "";
            //    string mailing = "";

            //    try
            //    {
            //        connection.Open();

            //        SqlDataReader reader = command.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            supplier_id = reader[0].ToString();
            //            name = reader[1].ToString();
            //            website = reader[6].ToString();
            //            email = reader[3].ToString();
            //            sales_dep = reader[4].ToString();
            //            sales_manager = reader[5].ToString();
            //            mailing = reader[2].ToString();
            //        }
            //        reader.Close();

            //        adapter = new SqlDataAdapter(command);
            //        adapter.Fill(dataTable); // TODO: Add bindings
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //    finally
            //    {
            //        if (connection != null)
            //            connection.Close();
            //    }

            //    //TextBoxSupplierName.Text = name;
            //    //TextBoxSupplierWebsite.Text = website;
            //    //TextBoxSupplierEmail.Text = email;
            //    //TextBoxSupplierSalesDep.Text = sales_dep;
            //    //TextBoxSupplierSalesManager.Text = sales_manager;
            //    //TextBoxSupplierAddress.Text = mailing;
            //}

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM supplier JOIN deal ON supplier.id = deal.supplier_id WHERE product_id = (SELECT id FROM product WHERE id = " + selectedItemId + ");";
                SqlCommand command = new SqlCommand(query, connection);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();

                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable); // TODO: Add bindings
                    ListViewDeals.ItemsSource = dataTable.DefaultView;
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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM supplier JOIN deal ON supplier.id = deal.supplier_id WHERE price = (SELECT MIN(price) FROM deal WHERE product_id = " + selectedItemId + ") AND product_id = (SELECT id FROM product WHERE id = " + selectedItemId + ");";
                SqlCommand command = new SqlCommand(query, connection);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();

                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable); // TODO: Add bindings
                    ListBoxMinPriceSuppliers.ItemsSource = dataTable.DefaultView;
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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM supplier JOIN deal ON supplier.id = deal.supplier_id WHERE manufacture_time = (SELECT MIN(manufacture_time) FROM deal WHERE product_id = " + selectedItemId + ") AND product_id = (SELECT id FROM product WHERE id = " + selectedItemId + ");";
                SqlCommand command = new SqlCommand(query, connection);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();

                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable); // TODO: Add bindings
                    ListBoxMinManufactureTimeSuppliers.ItemsSource = dataTable.DefaultView;
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

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string searchString = TextBoxGlobalSearch.Text;
                string query = "SELECT * FROM product WHERE name LIKE '%" + searchString + "%';";
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
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
        }

        private void ButtonSearchInCategory_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string searchString = TextBoxSearchInCategory.Text;
                string query = "SELECT * FROM product WHERE category_id = " + selectedCategoryId + " AND name LIKE '%" + searchString + "%';";
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
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
        }

        private void GetProductsInCategory(string categoryId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //List<ComboBox> comboBoxes = new List<ComboBox>();
                //List<ComboBox> comboBoxesWithSelectedItems = new List<ComboBox>();

                //comboBoxes.Add(ComboBoxCategory0);
                //comboBoxes.Add(ComboBoxCategory1);
                //comboBoxes.Add(ComboBoxCategory2);

                //foreach (var item in comboBoxes)
                //{
                //    if (item.SelectedIndex != -1)
                //    {
                //        comboBoxesWithSelectedItems.Add(item);
                //    }
                //}

                //int index = 0;
                //foreach (var item in comboBoxesWithSelectedItems)
                //{
                //    if (index != 0)
                //    {
                //        query += " UNION ";
                //    }

                //    //DataRowView dataRowView = item.SelectedItem as DataRowView;
                //    //string currentCategoryId = dataRowView.Row["id"].ToString();
                //    string currentCategoryId = item.SelectedValue.ToString();
                //    Trace.WriteLine("currentCategoryId = " + currentCategoryId);
                //    query += "(SELECT * FROM product WHERE category_id = " + currentCategoryId + ")";

                //    index++;
                //}

                //query += ";";



                string query = "SELECT * FROM product WHERE category_id = " + categoryId + ";";
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

        private void GetProductsInCategoryWithFilters(string categoryId, List<string> filtersNames, List<string> filtersValues)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM product WHERE category_id = " + categoryId + "";

                for (int i = 0; i < filtersNames.Count(); i++)
                {
                    if (filtersValues[i] != null)
                    {
                        query += "AND details LIKE \'%" + filtersNames[i] + "=" + filtersValues[i] + ";%\' ";
                    }
                }

                query += ";";



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

                TextBlockFiltersApplied.Visibility = Visibility.Visible;
            }
        }

        private void ListViewDeals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewDeals.SelectedIndex != -1)
            {
                DataRowView dataRowView = ListViewDeals.SelectedItem as DataRowView;

                string selectedSupplierName = "";
                string selectedSupplierId = "";
                string selectedProductId = "";

                if (dataRowView != null)
                {
                    selectedSupplierName = dataRowView.Row["name"].ToString();
                    selectedSupplierId = dataRowView.Row["supplier_id"].ToString();
                    selectedProductId = dataRowView.Row["product_id"].ToString();
                }

                SupplierWindow supplierWindow = new SupplierWindow(selectedSupplierName, selectedSupplierId, selectedProductId, connectionString);
                supplierWindow.Show();

                ListViewDeals.SelectedIndex = -1;
            }
        }

        private List<string> GetDataForFilters(ComboBox comboBoxCategory)
        {
            List<string> filtersList = new List<string>();

            string selectedCategoryId = comboBoxCategory.SelectedValue.ToString();
            //Trace.WriteLine(selectedCategoryId);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM product WHERE category_id = " + selectedCategoryId + ";";
                SqlCommand command = new SqlCommand(query, connection);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();

                    //adapter = new SqlDataAdapter(command);
                    //adapter.Fill(dataTable);

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string details = reader[2].ToString();

                        string currentItemDetails = "";
                        for (int i = 0; i < details.Length; i++)
                        {
                            if (alphabet.Contains(details[i]))
                            {
                                currentItemDetails += details[i];
                            }
                        }

                        string[] currentDetails = currentItemDetails.Split(';');
                        int currentRow = 0;
                        foreach (var detail in currentDetails)
                        {
                            if (currentDetails != null &&
                                detail != null &&
                                !string.IsNullOrEmpty(currentItemDetails) &&
                                !string.IsNullOrEmpty(detail) &&
                                !string.IsNullOrWhiteSpace(detail))
                            {
                                string[] tmp = detail.Split('=');

                                if (!filtersList.Contains(tmp[0]))
                                {
                                    filtersList.Add(tmp[0]);
                                }

                                currentRow++;
                            }
                        }
                    }
                    reader.Close();
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

            foreach (var item in filtersList)
            {
                Trace.WriteLine(item);
            }

            return filtersList;
        }

        private void ButtonOpenFilters_Click(object sender, RoutedEventArgs e)
        {
            List<string> filtersList = new List<string>();

            if (ComboBoxCategory2.SelectedIndex != -1)
            {
                filtersList = GetDataForFilters(ComboBoxCategory2);
            }
            else if (ComboBoxCategory1.SelectedIndex != -1)
            {
                filtersList = GetDataForFilters(ComboBoxCategory1);
            }
            else if (ComboBoxCategory0.SelectedIndex != -1)
            {
                filtersList = GetDataForFilters(ComboBoxCategory0);
            }

            FiltersWindow filtersWindow = new FiltersWindow(filtersList);
            filtersWindow.ShowDialog();

            bool areFiltersApplied = filtersWindow.areFiltersApplied;
            bool areFiltersReset = filtersWindow.areFiltersReset;

            if (areFiltersApplied)
            {
                Trace.WriteLine("Filters applied!");
                GetProductsInCategoryWithFilters(selectedCategoryId, filtersWindow.filtersList, filtersWindow.resultFilters);
            }
            else if (areFiltersReset)
            {
                GetProductsInCategory(selectedCategoryId);
            }
        }

        private void ListBoxMinPriceSuppliers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxMinPriceSuppliers.SelectedIndex != -1)
            {
                DataRowView dataRowView = ListBoxMinPriceSuppliers.SelectedItem as DataRowView;

                string selectedSupplierName = "";
                string selectedSupplierId = "";
                string selectedProductId = "";

                if (dataRowView != null)
                {
                    selectedSupplierName = dataRowView.Row["name"].ToString();
                    selectedSupplierId = dataRowView.Row["supplier_id"].ToString();
                    selectedProductId = dataRowView.Row["product_id"].ToString();
                }

                SupplierWindow supplierWindow = new SupplierWindow(selectedSupplierName, selectedSupplierId, selectedProductId, connectionString);
                supplierWindow.Show();

                ListBoxMinPriceSuppliers.SelectedIndex = -1;
            }
        }

        private void ListBoxMinManufactureTimeSuppliers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxMinManufactureTimeSuppliers.SelectedIndex != -1)
            {
                DataRowView dataRowView = ListBoxMinManufactureTimeSuppliers.SelectedItem as DataRowView;

                string selectedSupplierName = "";
                string selectedSupplierId = "";
                string selectedProductId = "";

                if (dataRowView != null)
                {
                    selectedSupplierName = dataRowView.Row["name"].ToString();
                    selectedSupplierId = dataRowView.Row["supplier_id"].ToString();
                    selectedProductId = dataRowView.Row["product_id"].ToString();
                }

                SupplierWindow supplierWindow = new SupplierWindow(selectedSupplierName, selectedSupplierId, selectedProductId, connectionString);
                supplierWindow.Show();

                ListBoxMinManufactureTimeSuppliers.SelectedIndex = -1;
            }
        }
    }
}
