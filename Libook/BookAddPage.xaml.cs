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
using ToastNotifications.Messages;
using ToastNotifications.Position;
namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для BookAddPage.xaml
    /// </summary>
    public partial class BookAddPage : Page
    {
        BookTableAdapter bookAdapter = new BookTableAdapter();
        AuthorTableAdapter authorAdapter = new AuthorTableAdapter();
        GenreTableAdapter genreAdapter = new GenreTableAdapter();
        AgeRatingTableAdapter ageRaitingadapter = new AgeRatingTableAdapter();
        private byte[] _image;
        private MainWindowLibrian _windowLibrian;
        public BookAddPage(MainWindowLibrian windowLibrian)
        {
            InitializeComponent();
            this._windowLibrian = windowLibrian;
            FullAgeRatingComboBox();
            FullAuthorComboBox();
            FullGenreComboBox();


        }
        private void FullAuthorComboBox()
        {
            AuthorComboBox.ItemsSource = authorAdapter.GetData().Rows;
            AuthorComboBox.DisplayMemberPath = "nickname";
            AuthorComboBox.SelectedValuePath = "id_author";

        }
        private void FullGenreComboBox()
        {
            GenreComboBox.ItemsSource = genreAdapter.GetData().Rows;
            GenreComboBox.DisplayMemberPath = "Name_genre";
            GenreComboBox.SelectedValuePath = "id_genre";

        }
        private void FullAgeRatingComboBox()
        {
            ARComboBox.ItemsSource = ageRaitingadapter.GetData().Rows;
            ARComboBox.DisplayMemberPath = "name_ar";
            ARComboBox.SelectedValuePath = "id_ar";

        }

        private void Nextbtn_Click(object sender, RoutedEventArgs e)
        {
            var allbooks = bookAdapter.GetData().Rows;
            string bookTitle = NameBook.Text;
            var newBook = allbooks.Cast<LibookDataSet.BookRow>()
                              .FirstOrDefault(b => b.title == bookTitle);

            if (newBook != null)
            {
                _windowLibrian.PageLibrian.Content = new BookAddPage2(newBook.id_book, _windowLibrian);
            }


        }
        private void ChooseImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                ImgBook.Text = imagePath;

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

        private void ImgBook_GotFocus(object sender, RoutedEventArgs e)
        {
            ChooseImage();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AuthorComboBox.SelectedValue == null || GenreComboBox.SelectedValue == null || ARComboBox.SelectedValue == null ||
                 DatePublishBox.SelectedDate == null || string.IsNullOrEmpty(NameBook.Text))
            {
                notifier.ShowError("Все поля должны быть заполнены");
                return;
            }


            int selectedAuthorId = (int)AuthorComboBox.SelectedValue;
            int selectedGenreId = (int)GenreComboBox.SelectedValue;
            int selectedARId = (int)ARComboBox.SelectedValue;

            DateTime date = (DateTime)DatePublishBox.SelectedDate;
            string formattedDate = date.ToString("yyyy-MM-dd");
            string bookTitle = NameBook.Text;
            bookAdapter.InsertBookQuery(selectedAuthorId, _image, bookTitle, selectedGenreId, formattedDate, selectedARId);
        }
    }
}
