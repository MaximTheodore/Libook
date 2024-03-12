using System;
using System.Collections.Generic;
using System.IO;
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
using Libook.LibookDataSetTableAdapters;
using Microsoft.Win32;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для AuthorAddPage.xaml
    /// </summary>
    public partial class AuthorAddPage : Page
    {
        private MainWindowLibrian _mainWindowLibrian;
        AuthorTableAdapter _authorAdapter = new AuthorTableAdapter();
        private byte[] _image;
        public AuthorAddPage(MainWindowLibrian mainWindow)
        {
            InitializeComponent();
            this._mainWindowLibrian = mainWindow;
        }

        private void SaveAuthorBtn_Click(object sender, RoutedEventArgs e)
        {
            _authorAdapter.InsertAuthorQuery(_image, NameAuthortxt.Text.ToString());
            _mainWindowLibrian.PageLibrian.Content = new AuthorPage(_mainWindowLibrian);
            
        }
        private void ImgAuthortxt_GotFocus(object sender, RoutedEventArgs e)
        {
            ChooseImage();
        }
        private void ChooseImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                ImgAuthortxt.Text = imagePath;

                _image = File.ReadAllBytes(imagePath);

            }
        }
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

    }
}
