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

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для AuthorViewPage.xaml
    /// </summary>
    public partial class AuthorViewPage : Page
    {
        private AuthorTableAdapter _authorAdapter = new AuthorTableAdapter();
        private MainWindowLibrian _windowLibrian;
        private int _idauthor;
        private byte[] _image;
        public AuthorViewPage(int idAuthor, MainWindowLibrian mainWindowLibrian)
        {
            InitializeComponent();
            this._windowLibrian = mainWindowLibrian;
            this._idauthor = idAuthor;
            InfoAuthor();
        }
        private void InfoAuthor()
        {
            foreach (var author  in _authorAdapter.GetData())
            {
                if (author.id_author == _idauthor)
                {
                    ImgAuthor.Source = DisplayImage(author.iconauthor);
                    NameAuthotxt.Text = author.nickname;
                    _image = author.iconauthor;
                }
            }

        }

        private void SaveupdateBtn_Click(object sender, RoutedEventArgs e)
        {
            _authorAdapter.UpdateAuthorQuery(_image, NameAuthotxt.Text.ToString() ,_idauthor);
            _windowLibrian.PageLibrian.Content = new AuthorPage(_windowLibrian);

        }
        private void ImgAuthor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChooseImage();
            ImgAuthor.Source = DisplayImage(_image);
        }
        private void ChooseImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                

                _image = File.ReadAllBytes(imagePath);


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
            defaultImage.UriSource = new Uri("pack://application:,,,/Libook;component/images/defaultperson.png");

            defaultImage.CacheOption = BitmapCacheOption.OnLoad;
            defaultImage.EndInit();
            return defaultImage;
        }

    }
}
