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

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для MessageReaderPage.xaml
    /// </summary>
    public partial class MessageReaderPage : Page
    {
        ReservationBookTableAdapter reservationBookTableAdapter = new ReservationBookTableAdapter();
        ChoosenBooksTableAdapter choosenBooksTableAdapter = new ChoosenBooksTableAdapter();
        BookTableAdapter bookTableAdapter = new BookTableAdapter();
        UsersTableAdapter usersTableAdapter = new UsersTableAdapter();
        UserInfoTableAdapter userInfoTableAdapter = new UserInfoTableAdapter();
        IssuedBooksTableAdapter issuedBooksTableAdapter = new IssuedBooksTableAdapter();

        public MessageReaderPage(int iduser)
        {
            InitializeComponent();
            FullDataGrid(iduser);
        }
        private void FullDataGrid(int userId)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Книга", typeof(string));
            dataTable.Columns.Add("Дата выдачи", typeof(string));
            dataTable.Columns.Add("Дата сдачи", typeof(string));
            dataTable.Columns.Add("Статус", typeof(string));

            var issuedBooksData = issuedBooksTableAdapter.GetData().Where(issuedBook =>
                choosenBooksTableAdapter.GetData()
                    .Where(choosenBook => choosenBook.id_user == userId)
                    .Select(choosenBook => choosenBook.id_choosenbooks)
                    .Contains(issuedBook.id_choosenbooks)
            );

            foreach (var issuedBook in issuedBooksData)
            {
                var choosenBook = choosenBooksTableAdapter.GetData().FindByid_choosenbooks(issuedBook.id_choosenbooks);
                var book = bookTableAdapter.GetData().FindByid_book(choosenBook.id_book);
                var user = usersTableAdapter.GetData().FindByid_user(choosenBook.id_user);

                dataTable.Rows.Add(book.title, issuedBook.issued_date.ToString("dd-MM-yyyy"), issuedBook.return_date.ToString("dd-MM-yyyy"), "Выдана");
            }

            MessageGrid.ItemsSource = dataTable.DefaultView;
        }

    }
}
