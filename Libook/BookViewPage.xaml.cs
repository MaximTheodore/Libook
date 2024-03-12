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
using static Libook.LibookDataSet;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для BookViewPage.xaml
    /// </summary>
    public partial class BookViewPage : Page
    {
        BookTableAdapter _bookAdapter = new BookTableAdapter();
        AuthorTableAdapter _authorAdapter = new AuthorTableAdapter();
        GenreTableAdapter _genreAdapter = new GenreTableAdapter();
        AgeRatingTableAdapter _AgeRatingTableAdapter = new AgeRatingTableAdapter();
        BookStorageTableAdapter bookStorageTableAdapter = new BookStorageTableAdapter();

        private int _role;
        private int _idbook;
        private int _idstoragebook;
        private byte[] _image;
        private string _title;
        private int _idauthor;
        private int _idgenre;
        private int _idagerating;
        private string _datepublish;

        private MainWindowLibrian windowLibrian;

        public BookViewPage(int idBook, int role, MainWindowLibrian mainWindowLibrian)
        {
            InitializeComponent();
            this._role = role;
            this._idbook = idBook;
            this.windowLibrian = mainWindowLibrian;
            ComponentsBook();
            VisiblComponents();
        }
        private void UpdatebtnLib_Click(object sender, RoutedEventArgs e)
        {
            int selectedAuthorId = (int)AuthorLibComboBox.SelectedValue;
            int selectedGenreId = (int)GenreLibComboBox.SelectedValue;
            int selectedARId = (int)ARComboBoxLib.SelectedValue;
            string selectedLocation = LocationRead.Text;
            byte[] image = GetImageBytesFromSource(ImgBookInfo.Source);

            _bookAdapter.UpdateBookQuery(selectedAuthorId, image, NameBook.Text.ToString(), selectedGenreId, DateBox.Text.ToString(), selectedARId, _idbook);
            bookStorageTableAdapter.UpdateBookStorageQuery(_idbook, selectedLocation, IsReadyRead.Text.ToString(), _idstoragebook);
            windowLibrian.PageLibrian.Content = new BookPage(1, windowLibrian, null);

        }
        private byte[] GetImageBytesFromSource(ImageSource imageSource)
        {
            if (imageSource == null)
                return null;

            var bitmapImage = (BitmapImage)imageSource;
            using (MemoryStream stream = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }


        private void VisiblComponents()
        {
            if(_role == 1) 
            {
                AuthorRead.Visibility = Visibility.Hidden;
                GenreReadbox.Visibility = Visibility.Hidden;
                ArBoxRead.Visibility = Visibility.Hidden;

                AuthorLibComboBox.Visibility = Visibility.Visible;
                GenreLibComboBox.Visibility = Visibility.Visible;
                ARComboBoxLib.Visibility = Visibility.Visible;  
                LocationRead.Visibility = Visibility.Visible;
                LocationRead.IsReadOnly = false;
                
               
                IsReadyRead.IsReadOnly = false;
                UpdatebtnLib.Visibility = Visibility.Visible;

                ViewBookLib();
            }
            if(_role == 2) 
            {
                AuthorRead.Visibility = Visibility.Visible;
                GenreReadbox.Visibility = Visibility.Visible;
                ArBoxRead.Visibility = Visibility.Visible;
                LocationRead.Visibility = Visibility.Visible;

                AuthorLibComboBox.Visibility = Visibility.Hidden;
                GenreLibComboBox.Visibility = Visibility.Hidden;
                ARComboBoxLib.Visibility = Visibility.Hidden;


                IsReadyRead.IsReadOnly = true;
                UpdatebtnLib.Visibility = Visibility.Hidden;

                ViewBookReader();
            }

        }
        private void FullComboBoxes()
        {
            GenreLibComboBox.ItemsSource = _genreAdapter.GetData().Rows;
            GenreLibComboBox.DisplayMemberPath = "Name_genre";
            GenreLibComboBox.SelectedValuePath = "id_genre";

            GenreLibComboBox.SelectedValue = _idgenre;

            ARComboBoxLib.ItemsSource = _AgeRatingTableAdapter.GetData().Rows;
            ARComboBoxLib.DisplayMemberPath = "name_ar";
            ARComboBoxLib.SelectedValuePath = "id_ar";

            ARComboBoxLib.SelectedValue = _idagerating;

            AuthorLibComboBox.ItemsSource = _authorAdapter.GetData().Rows;
            AuthorLibComboBox.DisplayMemberPath = "nickname";
            AuthorLibComboBox.SelectedValuePath = "id_author";

            AuthorLibComboBox.SelectedValue = _idauthor;

            

            
        }
        private void ViewBookLib()
        {
            ImgBookInfo.Source = DisplayImage(_image);
            NameBook.Text = _title;
            DateBox.Text = _datepublish;
            DateBox.IsEnabled = true;

            foreach(var bookstorage in bookStorageTableAdapter.GetData())
            {
                if (bookstorage.id_book== _idbook)
                {
                    _idstoragebook = bookstorage.id_bookstorage;
                    IsReadyRead.Text = bookstorage.status;
                    LocationRead.Text = bookstorage.lidlocation;
                    IsReadyRead.IsEnabled = true;
                }
            }


            FullComboBoxes();
        }
        private void ViewBookReader()
        {
            ImgBookInfo.Source = DisplayImage(_image);
            NameBook.Text = _title;
            DateBox.Text = _datepublish;
            DateBox.IsEnabled = true;
            foreach (var authors in _authorAdapter.GetData())
            {
                if(authors.id_author == _idauthor)
                {
                    AuthorRead.Text = authors.nickname;
                }
            }
            foreach(var genres in _genreAdapter.GetData())
            {
                if (genres.id_genre == _idgenre)
                {
                    GenreReadbox.Text = genres.Name_genre;
                }
            }
            foreach(var ar in _AgeRatingTableAdapter.GetData())
            {
                if (ar.id_ar == _idagerating)
                {
                    ArBoxRead.Text = ar.name_ar;
                }
            }
            foreach (var bookstorage in bookStorageTableAdapter.GetData())
            {
                if (bookstorage.id_book == _idbook)
                {
                    IsReadyRead.Text = bookstorage.status;
                    LocationRead.Text = bookstorage.lidlocation;
                    IsReadyRead.IsEnabled = false;

                }
            }
        }

        private void ComponentsBook()
        {
            var allbooks = _bookAdapter.GetData();
            foreach ( var book in allbooks )
            {
                if (book.id_book == _idbook)
                {
                    _image = book.imgbook;
                    _title = book.title;
                    _idgenre = book.genre;
                    _idauthor = book.id_author;
                    _idagerating = book.agerating;
                    _datepublish = book.published_year.ToString("dd-MM-yyyy");
                }
            }
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
            defaultImage.UriSource = new Uri("pack://application:,,,/Libook;component/images/unknownbook.png");

            defaultImage.CacheOption = BitmapCacheOption.OnLoad;
            defaultImage.EndInit();
            return defaultImage;
        }
    }
}
