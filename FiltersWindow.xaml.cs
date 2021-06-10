using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace EngineeredProductsCatalog
{
    /// <summary>
    /// Interaction logic for FiltersWindow.xaml
    /// </summary>
    public partial class FiltersWindow : Window
    {
        public List<string> filtersList;
        public DataTable resultDataTable;
        public List<string> resultFilters;

        public bool areFiltersApplied;
        public bool areFiltersReset;
        List<TextBox> textBoxes;

        public FiltersWindow(List<string> filters)
        {
            InitializeComponent();
            filtersList = filters;
            textBoxes = new List<TextBox>();
            resultFilters = new List<string>();
            areFiltersApplied = false;
            areFiltersReset = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int currentRow = 0;
            foreach (var item in filtersList)
            {
                if (item != null &&
                    !string.IsNullOrEmpty(item) &&
                    !string.IsNullOrWhiteSpace(item))
                {
                    Label label = new Label();
                    label.Name = "LabelDetailName" + currentRow; 
                    label.Content = item;
                    label.FontFamily = new FontFamily("Calibri");
                    GridFilters.Children.Add(label);

                    Grid.SetColumn(label, 0);
                    Grid.SetRow(label, currentRow);

                    TextBox textBox = new TextBox();
                    textBox.Name = "TextBoxDetailValue" + currentRow;
                    textBox.Text = "";
                    textBox.Height = 18;
                    textBox.FontFamily = new FontFamily("Calibri");
                    GridFilters.Children.Add(textBox);
                    Grid.SetColumn(textBox, 1);
                    Grid.SetRow(textBox, currentRow);

                    textBoxes.Add(textBox);

                    currentRow++;
                }
            }
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in textBoxes)
            {
                string text = item.Text;
                if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrEmpty(text))
                {
                    resultFilters.Add(text);
                }
                else
                {
                    resultFilters.Add(null);
                }
            }

            areFiltersApplied = true;
            this.Close();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            areFiltersReset = true;
            this.Close();
        }
    }
}
