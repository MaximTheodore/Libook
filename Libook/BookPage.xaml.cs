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

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для BookPage.xaml
    /// </summary>
    public partial class BookPage : Page
    {
        UsersTableAdapter usersTableAdapter = new UsersTableAdapter();
        BookTableAdapter bookTableAdapter = new BookTableAdapter();
        AuthorTableAdapter authorTableAdapter = new AuthorTableAdapter();
        GenreTableAdapter GenreTableAdapter = new GenreTableAdapter();
        AgeRatingTableAdapter AgeRatingTableAdapter = new AgeRatingTableAdapter();
        BookStorageTableAdapter bookStorageTableAdapter = new BookStorageTableAdapter();
        private int _role;
        private MainWindowLibrian _mainWindowLibrian;
        private MainWindowReader _mainWindowReader;

        public BookPage(int role, MainWindowLibrian mainWindow, MainWindowReader windowReader)
        {
            InitializeComponent();
            _role = role;
            this._mainWindowLibrian = mainWindow;
            _mainWindowReader = windowReader; 
            if (role == 1 ) 
            {
                AddBookBtn.Visibility = Visibility.Visible;
                AddBookBtn.IsEnabled = true;
            }
            if (role == 2 ) 
            { 
                AddBookBtn.Visibility = Visibility.Hidden;
                AddBookBtn.IsEnabled = false;
            }
            BookList();
            GenreComboBox();
            ARcomboBox();
            

        }
        private void BookList()
        {
            var allbooks = bookTableAdapter.GetData().Rows;
            for (int i = 0; i < allbooks.Count; i++)
            {
                byte[] imageData = (byte[])allbooks[i][2];
                int currentBookId = (int)allbooks[i][0];
                BookControl1 bookControl = new BookControl1();
                if (_role == 1)
                {
                    bookControl.LocalInfobtn.Visibility = Visibility.Visible;
                    bookControl.Deletebtn.Visibility = Visibility.Visible;
                }
                if (_role == 2)
                {
                    bookControl.LocalInfobtn.Visibility = Visibility.Hidden;
                    bookControl.Deletebtn.Visibility = Visibility.Hidden;
                }
                bookControl.idBook.Text = allbooks[i][0].ToString();
                bookControl.ImgBookCard.Source = DisplayImage(imageData);
                bookControl.NameBookCard.Text = allbooks[i][3].ToString();
                bookControl.AuthorBookCard.Text = SeachAuthorBook((int)allbooks[i][1]);
                bookControl.Deletebtn.Click += (sender, args) => HandleDeleteButtonClick(currentBookId); 
                bookControl.LocalInfobtn.Click += (sender, args) => ViewLocation(currentBookId); 

                BooksListBox.Items.Add(bookControl);

            }
        }
        private void HandleDeleteButtonClick(int bookId)
        {
            foreach (var storage in bookStorageTableAdapter.GetData())
            {
                if(storage.id_book == bookId)
                {
                    bookStorageTableAdapter.DeleteBookStorageQuery(storage.id_book);
                }
            }
            bookTableAdapter.DeleteBookQuery(bookId);

            BookList();

        }
        private void ViewLocation(int bookId)
        {
            foreach (var storage in bookStorageTableAdapter.GetData())
            {
                if (storage.id_book == bookId)
                {
                    MessageBox.Show($"Расположение книги {storage.lidlocation}.\n Статус книги {storage.status}");
                }
            }
        }
        private void GenreComboBox()
        {
            FilterGenre.ItemsSource = GenreTableAdapter.GetData().Rows;
            FilterGenre.DisplayMemberPath = "Name_genre"; 
            FilterGenre.SelectedValuePath = "id_genre";
        }
        private void ARcomboBox()
        {
            FilterAR.ItemsSource = AgeRatingTableAdapter.GetData().Rows;
            FilterAR.DisplayMemberPath = "name_ar";
            FilterAR.SelectedValuePath = "id_ar";


        }
        private string SeachAuthorBook(int idauthor)
        {
            var allauthor = authorTableAdapter.GetData().Rows;
            for( int i = 0; i< allauthor.Count; i++)
            {
                if ((int)allauthor[i][0] == idauthor)
                {
                    return allauthor[i][2].ToString();
                }
            }
            return "Неизвестный автор";
        }
        private BitmapImage DisplayImage(byte[] imageData)
        {
            if (imageData != null && imageData.Length > 0)
            {
                BitmapImage bitmapImage = new BitmapImage();

                using (MemoryStream memoryStream = new MemoryStream(imageData))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                }
                return bitmapImage;
            }
            return LoadDefaultImage();
        }
        private BitmapImage LoadDefaultImage()
        {
            BitmapImage defaultImage = new BitmapImage();
            defaultImage.BeginInit();
            defaultImage.UriSource = new Uri("/Libook;component/images/unknownbook.jpg", UriKind.Relative);
            defaultImage.CacheOption = BitmapCacheOption.OnLoad;
            defaultImage.EndInit();
            return defaultImage;
        }

        private void FilterAR_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
        private void ApplyFilters()
        {
            int selectedGenreId = (int)FilterGenre.SelectedValue;
            int selectedARId = (int)FilterAR.SelectedValue;
            string searchText = SearchField.Text.ToLower();

            BooksListBox.Items.Clear();

            var filteredBooks = bookTableAdapter.GetData().Rows.Cast<LibookDataSet.BookRow>()
                .Where(book => (selectedGenreId == 0 || book.genre == selectedGenreId) &&
                               (selectedARId == 0 || book.agerating == selectedARId) &&
                               (string.IsNullOrEmpty(searchText) ||
                                book.title.ToLower().Contains(searchText) ||
                                SeachAuthorBook(book.id_author).ToLower().Contains(searchText)));

            foreach (var book in filteredBooks)
            {
                byte[] imageData = book.imgbook;
                BookControl1 bookControl = new BookControl1();

                if (_role == 1)
                {
                    bookControl.LocalInfobtn.Visibility = Visibility.Visible;
                    bookControl.Deletebtn.Visibility = Visibility.Visible;
                }

                if (_role == 2)
                {
                    bookControl.LocalInfobtn.Visibility = Visibility.Hidden;
                    bookControl.Deletebtn.Visibility = Visibility.Hidden;
                }
                bookControl.idBook.Text = book.id_book.ToString();
                bookControl.ImgBookCard.Source = DisplayImage(imageData);
                bookControl.NameBookCard.Text = book.title;
                bookControl.AuthorBookCard.Text = SeachAuthorBook(book.id_author);
                bookControl.Deletebtn.Click += (sender, args) => HandleDeleteButtonClick(book.id_book);

                BooksListBox.Items.Add(bookControl);
            }
        }

        private void SearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void AddBookBtn_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowLibrian.PageLibrian.Content = new BookAddPage(_mainWindowLibrian);
        }

        private void BooksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksListBox.SelectedItem is BookControl1 selectedBookControl)
            {
                int idbook = Convert.ToInt32(selectedBookControl.idBook.Text);
                if (_role==1)
                {
                    _mainWindowLibrian.PageLibrian.Content = new BookViewPage(idbook, _role, _mainWindowLibrian);

                }
                if (_role==2)
                {
                    _mainWindowReader.PageReader.Content = new BookViewPage(idbook, _role, null);

                }
                BooksListBox.SelectedItem = null;
            }
        }

    }
}
