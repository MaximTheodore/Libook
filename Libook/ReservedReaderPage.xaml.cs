using Libook.LibookDataSetTableAdapters;
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
using ToastNotifications;
using ToastNotifications.Position;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для ReservedReaderPage.xaml
    /// </summary>
    public partial class ReservedReaderPage : Page
    {
        BookTableAdapter bookTableAdapter= new BookTableAdapter();
        ChoosenBooksTableAdapter choosenBooksTableAdapter = new ChoosenBooksTableAdapter();
        ReservationBookTableAdapter reservationBookTableAdapter = new ReservationBookTableAdapter();
        int idreader;
        LibHistoryTableAdapter libHistoryTableAdapter = new LibHistoryTableAdapter();
        public ReservedReaderPage(int idreader)
        {
            InitializeComponent();
            this.idreader = idreader;
            BookCombo();
        }
        private void BookCombo()
        {
            NamebookCombo.ItemsSource = bookTableAdapter.GetData().Rows;
            NamebookCombo.DisplayMemberPath = "title";
            NamebookCombo.SelectedValuePath = "id_book";

            TypechooseCombo.Text = "reservation";
            TypechooseCombo.IsReadOnly = true;
        }

        private void NamebookCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedBookId = (int)NamebookCombo.SelectedValue;
            choosenBooksTableAdapter.InsertChoosenBookQuery( idreader , selectedBookId, TypechooseCombo.Text.ToString());
        }

        private void Reservedbtn_Click(object sender, RoutedEventArgs e)
        {
           
            var sortedChoosenBooks = choosenBooksTableAdapter.GetData().OrderByDescending(choose => choose.id_choosenbooks);

            var lastChoosenBook = sortedChoosenBooks.FirstOrDefault();

            

            if (lastChoosenBook != null)
            {
                int lastInsertedId = lastChoosenBook.id_choosenbooks;

                reservationBookTableAdapter.InsertReservationBookQuery(lastInsertedId, StartReservedPick.Text, EndReservedPick.Text);
                libHistoryTableAdapter.InsertLibHistoryQuery(DateTime.Now, lastInsertedId);
                notifier.ShowSuccess("Книга успешно зарезервирована");

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
