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
using Libook.LibookDataSetTableAdapters;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для BookAddPage.xaml
    /// </summary>
    public partial class BookAddPage2 : Page
    {
        private int _idnewbook;
        private MainWindowLibrian _windowLibrian;
        BookStorageTableAdapter _storageadapter = new BookStorageTableAdapter();
        BookTableAdapter _bookadapter = new BookTableAdapter();
        public BookAddPage2(int idnewbook, MainWindowLibrian mainWindowLibrian)
        {
            InitializeComponent();
            this._idnewbook = idnewbook;
            this._windowLibrian = mainWindowLibrian;

            var book = _bookadapter.GetData().FirstOrDefault(b => b.id_book == _idnewbook);
            if (book != null)
            {
                NameBookCombox.Text = book.title;
            }
        }

        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
          
           int selectedBookId = _idnewbook;

           string location = LocationBox.Text;
           string status = StatusBook.Text;

           _storageadapter.InsertBookStorageQuery(selectedBookId, location, status);

           notifier.ShowSuccess("Книга успешно добавлена в хранилище");

            _windowLibrian.PageLibrian.Content = new BookPage(1, _windowLibrian, null);
            
            
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
