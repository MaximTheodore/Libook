using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для ReservedLibPage.xaml
    /// </summary>
    public partial class ReservedLibPage : Page
    {
        ReservationBookTableAdapter reservationBookTableAdapter = new ReservationBookTableAdapter();
        ChoosenBooksTableAdapter choosenBooksTableAdapter = new ChoosenBooksTableAdapter();
        BookTableAdapter bookTableAdapter = new BookTableAdapter();
        UsersTableAdapter usersTableAdapter = new UsersTableAdapter();
        UserInfoTableAdapter userInfoTableAdapter = new UserInfoTableAdapter();
        IssuedBooksTableAdapter issuedBooksTableAdapter = new IssuedBooksTableAdapter();
        LibHistoryTableAdapter adapter = new LibHistoryTableAdapter();
        int idchoose;
        string nameuser;
        public ReservedLibPage()
        {
            InitializeComponent();
            FillReservedBooksDataGrid();


        }
        private void FillReservedBooksDataGrid()
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Имя\nчитателя", typeof(string));
            dataTable.Columns.Add("Книга", typeof(string));
            dataTable.Columns.Add("Start", typeof(string));
            dataTable.Columns.Add("End", typeof(string));

            var reservationData = reservationBookTableAdapter.GetData();

            foreach (var reservation in reservationData)
            {

                var choosenBook = choosenBooksTableAdapter.GetData().FindByid_choosenbooks(reservation.id_choosenbooks);


                var book = bookTableAdapter.GetData().FindByid_book(choosenBook.id_book);

                var user = usersTableAdapter.GetData().FindByid_user(choosenBook.id_user);

                var userInfo = userInfoTableAdapter.GetData().FindByid_userinfo(user.id_user);
                dataTable.Rows.Add(userInfo.name+" "+"("+choosenBook.id_choosenbooks+")", book.title, reservation.startreservation.ToString("dd-MM-yyyy"), reservation.endreservation.ToString("dd-MM-yyyy"));
            }

            ReservedBooksByreader.ItemsSource = dataTable.DefaultView;
        }

        private void Issuebtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in choosenBooksTableAdapter.GetData())
            {
                if (item.id_choosenbooks == idchoose)
                {
                    choosenBooksTableAdapter.UpdateChoosenBookQuery(item.id_user, item.id_book,"issue", idchoose);
                }
            }
            issuedBooksTableAdapter.InsertIssuedBookQuery(idchoose, DateIssueBookPicker.Text, DateReturnBookPicker.Text);
            adapter.InsertLibHistoryQuery(DateTime.Now, idchoose);
            notifier.ShowSuccess("Книга выдана пользователю " + nameuser);
            
        }

        private void ReservedBooksByreader_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReservedBooksByreader.SelectedItem != null)
            {
                DataRowView row = (DataRowView)ReservedBooksByreader.SelectedItem;

                string userData = row["Имя\nчитателя"].ToString();
                string choosenBookId = userData.Split('(')[1].Trim(')');
                string choosenBookName = row["Книга"].ToString();
                string userName = userData.Split('(')[0].Trim(); // Получаем название "Имя\nчитателя"

                idchoose = Convert.ToInt32(choosenBookId);

                // Заполняем ComboBox и устанавливаем выбранный элемент
                NameBookBox.ItemsSource = new List<string> { choosenBookName };
                NameBookBox.SelectedItem = choosenBookName;
                NameBookBox.Tag = choosenBookId;

                
                nameuser = userName;
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

