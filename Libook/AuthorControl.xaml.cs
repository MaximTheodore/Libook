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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для AuthorControl.xaml
    /// </summary>
    public partial class AuthorControl : UserControl
    {
        public event EventHandler ItemClick;
        public AuthorControl()
        {
            InitializeComponent();
        }

        private void DeleteAuthorbtn_Click(object sender, RoutedEventArgs e)
        {
            ItemClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
