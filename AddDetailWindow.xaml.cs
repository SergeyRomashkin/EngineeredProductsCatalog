using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddDetailWindow.xaml
    /// </summary>
    public partial class AddDetailWindow : Window
    {
        public string detailName;
        public bool isOk;
        public AddDetailWindow()
        {
            InitializeComponent();
            detailName = "";
            isOk = false;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                detailName = TextBoxDetailNameValue.Text;
                isOk = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
