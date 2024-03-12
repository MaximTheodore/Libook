using Libook.LibookDataSetTableAdapters;
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
using System.Windows.Shapes;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для MainWindowReader.xaml
    /// </summary>
    public partial class MainWindowReader : Window
    {
        private int _role;
        private int _iduser;
        private int _idUserInfo;
        UserInfoTableAdapter _adapterUserInfo = new UserInfoTableAdapter();
        public MainWindowReader(int iduser,int role)
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
                    _idUserInfo = (int)allUserinfo[i][0];
                    byte[] imageData = (byte[])allUserinfo[i][1];
                    ImgProfile.Source = DisplayImage(imageData);
                }
            }
        }

        private void Messege_Click(object sender, RoutedEventArgs e)
        {
            PageReader.Content = new MessageReaderPage(_iduser);
        }

        private void Books_Click(object sender, RoutedEventArgs e)
        {
            PageReader.Content = new BookPage(_role, null, this);
        }

        private void Reserved_Click(object sender, RoutedEventArgs e)
        {
            PageReader.Content = new ReservedReaderPage(_iduser);
        }

        private void ExportCard_Click(object sender, RoutedEventArgs e)
        {
            PageReader.Content = new ExportCardPage(_idUserInfo, _iduser);
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

    }
}
