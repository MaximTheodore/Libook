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
    /// Логика взаимодействия для AuthorPage.xaml
    /// </summary>
    public partial class AuthorPage : Page
    {
        AuthorTableAdapter authorAdapter = new AuthorTableAdapter();
        BookTableAdapter bookTableAdapter = new BookTableAdapter();
        private MainWindowLibrian _mainWindowLibrian;
        public AuthorPage(MainWindowLibrian mainWindowLibrian)
        {
            InitializeComponent();
            this._mainWindowLibrian = mainWindowLibrian;
            ListAuthors();
        }
        private void ListAuthors()
        {
            var allauthors =  authorAdapter.GetData().Rows;
            for (int i = 0; allauthors.Count > i; i++)
            {
                byte[] imageData = (byte[])allauthors[i][1];
                int idAuthor =(int)allauthors[i][0];
                AuthorControl authControl = new AuthorControl();
                authControl.AuthorIdtxt.Text = allauthors[i][0].ToString();
                authControl.ImgAuthorCard.Source = DisplayImage(imageData);
                authControl.NameAuthorCard.Text = allauthors[i][2].ToString();
                authControl.DeleteAuthorbtn.Click += (sender, args) => DeleteAuthor(idAuthor);
                AuthorsListBox.Items.Add(authControl);
            }


        }
        private void DeleteAuthor(int idauthor)
        {
 
            var booksWithAuthor = bookTableAdapter.GetData().Where(book => book.id_author == idauthor);

            if (booksWithAuthor.Any())
            {
                MessageBox.Show("Невозможно удалить автора, так как существуют книги, связанные с этим автором.");
                return;
            }

            authorAdapter.DeleteAuthorQuery(idauthor);
            ListAuthors();

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

        private void AuthorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AuthorsListBox.SelectedItem is AuthorControl selectedAuthorControl)
            {
                int idAuthor = Convert.ToInt32(selectedAuthorControl.AuthorIdtxt.Text);

                _mainWindowLibrian.PageLibrian.Content = new AuthorViewPage(idAuthor, _mainWindowLibrian);
                AuthorsListBox.SelectedItem = null;
            }
        }

        private void AddAuthorBtn_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowLibrian.PageLibrian.Content = new AuthorAddPage(_mainWindowLibrian);
        }
    }
}
