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
    /// Interaction logic for DbEditorWindow.xaml
    /// </summary>
    public partial class DbEditorWindow : Window
    {
        string connectionString;
        SqlDataAdapter adapter;

        int currentRowInGridDetails;

        List<Label> labels_details;
        List<TextBox> textBoxes_details;

        string selectedCategoryId;

        string selectedItemId;
        string selectedItemName;
        string selectedItemDetailsRaw;
        string selectedItemDetails;
        string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъьыэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЫЭЮЯ0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,:;= ";

        public DbEditorWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            currentRowInGridDetails = 0;
        }

        private void ComboBoxCategory0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ButtonAddProduct != null)
            {
                ButtonAddProduct.IsEnabled = true;
            }

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
            if (ComboBoxCategory1.SelectedIndex != -1)
            {
                if (StackPanel2 != null)
                {
                    StackPanel2.Visibility = Visibility.Visible;
                }

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
            if (ListBoxResults.SelectedIndex != -1)
            {
                StackPanelDetails.Visibility = Visibility.Visible;
                StackPanelSupplierEditor.Visibility = Visibility.Visible;

                GridDetails.Children.Clear();
                currentRowInGridDetails = 0;

                // Very important thing (FOR COMBOBOX)
                DataRowView dataRowView = ListBoxResults.SelectedItem as DataRowView;

                if (dataRowView != null)
                {
                    selectedItemId = dataRowView.Row["id"].ToString();
                    selectedItemName = dataRowView.Row["name"].ToString();
                    selectedItemDetailsRaw = dataRowView.Row["details"].ToString();
                }



                if (!string.IsNullOrEmpty(dataRowView.Row["details"].ToString()))
                {
                    selectedItemDetails = "";
                    for (int i = 0; i < selectedItemDetailsRaw.Length; i++)
                    {
                        if (alphabet.Contains(selectedItemDetailsRaw[i]))
                        {
                            selectedItemDetails += selectedItemDetailsRaw[i];
                        }
                    }

                    TextBoxProductName.Text = selectedItemName;

                    string[] details = selectedItemDetails.Split(';');

                    foreach (var item in details)
                    {
                        Trace.WriteLine(item);
                    }

                    labels_details = new List<Label>();
                    textBoxes_details = new List<TextBox>();

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

                            TextBox textBox = new TextBox();
                            textBox.Text = tmp[1];
                            textBox.Height = 18;
                            GridDetails.Children.Add(textBox);
                            Grid.SetColumn(textBox, 1);
                            Grid.SetRow(textBox, currentRow);

                            labels_details.Add(LabelTmp);
                            textBoxes_details.Add(textBox);

                            currentRow++;
                            currentRowInGridDetails = currentRow;
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(dataRowView.Row["details"].ToString()))
                {
                    Trace.WriteLine("Pass");
                    selectedItemDetails = "";

                    TextBoxProductName.Text = selectedItemName;

                    List<string> details = new List<string>();

                    if (ComboBoxCategory2 != null && ComboBoxCategory2.SelectedIndex != -1 && ComboBoxCategory2.Visibility == Visibility.Visible)
                    {
                        details = GetDataForFilters(ComboBoxCategory2);
                    }
                    else if (ComboBoxCategory1 != null && ComboBoxCategory1.SelectedIndex != -1 && ComboBoxCategory1.Visibility == Visibility.Visible)
                    {
                        details = GetDataForFilters(ComboBoxCategory1);
                    }
                    else if (ComboBoxCategory0 != null && ComboBoxCategory0.SelectedIndex != -1 && ComboBoxCategory0.Visibility == Visibility.Visible)
                    {
                        details = GetDataForFilters(ComboBoxCategory0);
                    }

                    labels_details = new List<Label>();
                    textBoxes_details = new List<TextBox>();

                    int currentRow = 0;
                    foreach (var item in details)
                    {
                        if (item != null && !string.IsNullOrEmpty(item) && !string.IsNullOrWhiteSpace(item))
                        {
                            Label LabelTmp = new Label();
                            LabelTmp.Content = item;
                            GridDetails.Children.Add(LabelTmp);

                            Grid.SetColumn(LabelTmp, 0);
                            Grid.SetRow(LabelTmp, currentRow);

                            TextBox textBox = new TextBox();
                            textBox.Text = "";
                            textBox.Height = 18;
                            GridDetails.Children.Add(textBox);
                            Grid.SetColumn(textBox, 1);
                            Grid.SetRow(textBox, currentRow);

                            labels_details.Add(LabelTmp);
                            textBoxes_details.Add(textBox);

                            currentRow++;
                            currentRowInGridDetails = currentRow;
                        }
                    }
                }

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
                        ListViewDeals.SelectedValuePath = "id";
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

                //TextBlockFiltersApplied.Visibility = Visibility.Visible;
            }
        }

        private void ListViewDeals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (ListViewDeals.SelectedIndex != -1)
            //{
            //    DataRowView dataRowView = ListViewDeals.SelectedItem as DataRowView;

            //    string selectedSupplierName = "";
            //    string selectedSupplierId = "";
            //    string selectedProductId = "";

            //    if (dataRowView != null)
            //    {
            //        selectedSupplierName = dataRowView.Row["name"].ToString();
            //        selectedSupplierId = dataRowView.Row["supplier_id"].ToString();
            //        selectedProductId = dataRowView.Row["product_id"].ToString();
            //    }

            //    SupplierWindow supplierWindow = new SupplierWindow(selectedSupplierName, selectedSupplierId, selectedProductId, connectionString);
            //    supplierWindow.Show();

            //    ListViewDeals.SelectedIndex = -1;
            //}
        }

        private List<string> GetDataForFilters(ComboBox comboBoxCategory)
        {
            List<string> filtersList = new List<string>();

            string selectedCategoryId = comboBoxCategory.SelectedValue.ToString();            

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM product WHERE category_id = " + selectedCategoryId + ";";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
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
                    Trace.WriteLine(ex.Message);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
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

        private void ButtonAddCategory0_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow addCategoryWindow = new AddCategoryWindow();
            addCategoryWindow.ShowDialog();

            string categoryName = addCategoryWindow.categoryName;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO category (name, parent_category_id) VALUES (\'" + categoryName + "\', NULL);";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Window_Loaded(this, e);
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

        private void ButtonAddCategory1_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow addCategoryWindow = new AddCategoryWindow();
            addCategoryWindow.ShowDialog();

            string categoryName = addCategoryWindow.categoryName;

            DataRowView dataRowView = ComboBoxCategory0.SelectedItem as DataRowView;

            string parentCategoryId = "";

            if (dataRowView != null)
            {
                parentCategoryId = dataRowView.Row["id"].ToString();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO category (name, parent_category_id) VALUES (\'" + categoryName + "\', " + parentCategoryId + ");";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Window_Loaded(this, e);
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

        private void ButtonAddCategory2_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow addCategoryWindow = new AddCategoryWindow();
            addCategoryWindow.ShowDialog();

            string categoryName = addCategoryWindow.categoryName;

            DataRowView dataRowView = ComboBoxCategory1.SelectedItem as DataRowView;

            string parentCategoryId = "";

            if (dataRowView != null)
            {
                parentCategoryId = dataRowView.Row["id"].ToString();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO category (name, parent_category_id) VALUES (\'" + categoryName + "\', " + parentCategoryId + ");";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Window_Loaded(this, e);
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

        private void ButtonAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxCategory0.SelectedIndex != -1)
            {
                string productName = "Новая деталь";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO product (name, details, category_id) VALUES (\'" + productName + "\', NULL, " + selectedCategoryId + ");";
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
        }

        private void ButtonRemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxResults.SelectedIndex != -1)
            {
                string productId = selectedItemId;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM product WHERE id = " + productId + ";";
                    SqlCommand command = new SqlCommand(query, connection);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        RefreshListBoxResults();
                        StackPanelDetails.Visibility = Visibility.Hidden;
                        StackPanelSupplierEditor.Visibility = Visibility.Hidden;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                        MessageBox.Show("Не удалось удалить деталь.\nВозможно, остались предложения от поставщиков?", "Не удалось удалить деталь", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        if (connection != null)
                            connection.Close();
                    }
                }
            }            
        }

        private void RefreshListBoxResults()
        {
            GetProductsInCategory(selectedCategoryId);
        }

        private void ButtonRemoveCategory0_Click(object sender, RoutedEventArgs e)
        {
            string categoryId = "";

            DataRowView dataRowView = ComboBoxCategory0.SelectedItem as DataRowView;

            if (dataRowView != null)
            {
                categoryId = dataRowView.Row["id"].ToString();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM category WHERE id = " + categoryId + ";";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Window_Loaded(this, e);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show("Не удалось удалить категорию.\nВозможно, в этой категории есть детали?", "Не удалось удалить категорию", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
        }

        private void ButtonRemoveCategory1_Click(object sender, RoutedEventArgs e)
        {
            string categoryId = "";

            DataRowView dataRowView = ComboBoxCategory1.SelectedItem as DataRowView;

            if (dataRowView != null)
            {
                categoryId = dataRowView.Row["id"].ToString();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM category WHERE id = " + categoryId + ";";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    ComboBoxCategory0_SelectionChanged(this, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show("Не удалось удалить категорию.\nВозможно, в этой категории есть детали?", "Не удалось удалить категорию", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
        }

        private void ButtonRemoveCategory2_Click(object sender, RoutedEventArgs e)
        {
            string categoryId = "";

            DataRowView dataRowView = ComboBoxCategory2.SelectedItem as DataRowView;

            if (dataRowView != null)
            {
                categoryId = dataRowView.Row["id"].ToString();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM category WHERE id = " + categoryId + ";";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    ComboBoxCategory1_SelectionChanged(this, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show("Не удалось удалить категорию.\nВозможно, в этой категории есть детали?", "Не удалось удалить категорию", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
        }

        private void ButtonAddDetail_Click(object sender, RoutedEventArgs e)
        {
            AddDetailWindow addDetailWindow = new AddDetailWindow();
            addDetailWindow.ShowDialog();

            if (addDetailWindow.isOk)
            {
                string detailName = addDetailWindow.detailName;
                Label label = new Label();
                label.Content = detailName;
                label.FontFamily = new FontFamily("Calibri");
                GridDetails.Children.Add(label);
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, currentRowInGridDetails);

                TextBox textBox = new TextBox();
                textBox.Text = "";
                textBox.FontFamily = new FontFamily("Calibri");
                textBox.Height = 18;
                GridDetails.Children.Add(textBox);
                Grid.SetColumn(textBox, 1);
                Grid.SetRow(textBox, currentRowInGridDetails);

                labels_details.Add(label);
                textBoxes_details.Add(textBox);

                currentRowInGridDetails++;
            }
            
        }

        private void ButtonSaveDetails_Click(object sender, RoutedEventArgs e)
        {
            string details = "";
            for (int i = 0; i < labels_details.Count(); i++)
            {
                if (!string.IsNullOrWhiteSpace(textBoxes_details[i].Text) && !string.IsNullOrEmpty(textBoxes_details[i].Text))
                {
                    details += labels_details[i].Content + "=" + textBoxes_details[i].Text + ";";
                }                
            }

            string name = TextBoxProductName.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE product SET name = \'" + name + "\', details = \'" + details + "\' WHERE id = " + selectedItemId +";";
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

        private void ButtonEditSuppliers_Click(object sender, RoutedEventArgs e)
        {
            EditSuppliersWindow editSuppliersWindow = new EditSuppliersWindow();
            editSuppliersWindow.ShowDialog();
        }

        private void ButtonAddDeal_Click(object sender, RoutedEventArgs e)
        {
            AddDealWindow addDealWindow = new AddDealWindow(selectedItemId);
            addDealWindow.ShowDialog();
            RefreshListViewDeals();
        }

        private void ButtonRemoveDeal_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = ListViewDeals.SelectedItem as DataRowView;
            string dealId = ListViewDeals.SelectedValue.ToString();

            Trace.WriteLine(dealId);


            //if (dataRowView != null)
            //{
            //    dealId = dataRowView.Row["id"].ToString();
            //}

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM deal WHERE id = " + dealId + ";";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    RefreshListViewDeals();
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

        private void RefreshListViewDeals()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM deal JOIN supplier ON supplier.id = deal.supplier_id WHERE product_id = (SELECT id FROM product WHERE id = " + selectedItemId + ");";
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
        }
    }
}
