using System;
using System.Collections.Generic;
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
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        public bool isOk;
        private string password = "123";
        public PasswordWindow()
        {
            InitializeComponent();
            isOk = false;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBoxPassword.Password == password)
            {
                isOk = true;
            }

            this.Close();
        }
    }
}
