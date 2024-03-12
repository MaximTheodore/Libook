using Libook.LibookDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для MainWindowLibrian.xaml
    /// </summary>
    public partial class MainWindowLibrian : Window
    {
        private int _role;
        private int _iduser;
        private int _idUserInfo;
        UserInfoTableAdapter _adapterUserInfo = new UserInfoTableAdapter();

        BookTableAdapter bookTableAdapter = new BookTableAdapter();
        AuthorTableAdapter authorTableAdapter = new AuthorTableAdapter();
        BookStorageTableAdapter bookStorageTableAdapter = new BookStorageTableAdapter();
        AgeRatingTableAdapter ageRatingTableAdapter = new AgeRatingTableAdapter();
        GenreTableAdapter genreTableAdapter = new GenreTableAdapter();

        public MainWindowLibrian(int iduser,int role)
        {
            InitializeComponent();
            this._role = role;
            this._iduser = iduser;
            Profile();
        }
        private void Profile()
        {
            var allUserinfo = _adapterUserInfo.GetData().Rows;
            for (int i = 0; i < allUserinfo.Count; i++)
            {
                if ((int)allUserinfo[i][6] == _iduser) 
                {
                    byte[] imageData = (byte[])allUserinfo[i][1];
                    ImgProfile.Source = DisplayImage(imageData);
                    _idUserInfo = (int)allUserinfo[i][0];
                }
            }
        }


        private void Books_Click(object sender, RoutedEventArgs e)
        {
            PageLibrian.Content = new BookPage(_role,this, null);
        }

        private void Authors_Click(object sender, RoutedEventArgs e)
        {
            PageLibrian.Content = new AuthorPage(this);
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            PageLibrian.Content = new UsersPage(this);
        }

        private void Reserved_Click(object sender, RoutedEventArgs e)
        {
            PageLibrian.Content = new ReservedLibPage();
        }

        private void ExportCard_Click(object sender, RoutedEventArgs e)
        {
            PageLibrian.Content = new ExportCardPage(_idUserInfo,_iduser);
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
            defaultImage.UriSource = new Uri("/Libook;component/images/defaultperson.png", UriKind.Relative);
            defaultImage.CacheOption = BitmapCacheOption.OnLoad;
            defaultImage.EndInit();
            return defaultImage;
        }

        private void ExportListBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = System.IO.Path.Combine(desktopPath, "BooksExport.txt");

                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine("ID \t Title\t Author\t Genre\t Published Year\t Age Rating\t Location");

                    var allBooks = bookTableAdapter.GetData();

                    foreach (var book in allBooks)
                    {
                        var author = authorTableAdapter.GetData().FindByid_author(book.id_author);
                        var genre = genreTableAdapter.GetData().FindByid_genre(book.genre);
                        var ageRating = ageRatingTableAdapter.GetData().FindByid_ar(book.agerating);

                        var bookStorage = bookStorageTableAdapter.GetData().FindByid_bookstorage(book.id_book);

                        sw.WriteLine($"{book.id_book}\t{book.title}\t{author.nickname}\t{genre.Name_genre}\t{book.published_year}\t{ageRating.name_ar}\t{bookStorage.lidlocation}");
                    }

                    MessageBox.Show("Файл успешно записан!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в экспорте данных: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
