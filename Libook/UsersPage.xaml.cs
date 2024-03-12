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
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        private MainWindowLibrian _windowLibrian;
        private UserInfoTableAdapter _userInfoTableAdapter = new UserInfoTableAdapter();
        UsersTableAdapter userstableAdapter = new UsersTableAdapter();
        LibCardTableAdapter libCardTableAdapter = new LibCardTableAdapter();
        int userid;
        public UsersPage(MainWindowLibrian mainWindowLibrian)
        {
            InitializeComponent();
            this._windowLibrian = mainWindowLibrian;
            ListUsers();
        }
        private void ListUsers()
        {
            List<UserControl1> users = new List<UserControl1>();
            var allusers = _userInfoTableAdapter.GetData().Rows;
            for(int i=0; i < allusers.Count; i++)
            {
                byte[] imageData = (byte[])allusers[i][1];
                int iduser = (int) allusers[i][0];
                UserControl1 userControl1 = new UserControl1();
                userControl1.IdUsertxt.Text = allusers[i][0].ToString();
                userControl1.ImgUserCard.Source = DisplayImage(imageData);
                userControl1.NameUserCard.Text = allusers[i][2].ToString();
                userControl1.SurnameUserCard.Text = allusers[i][3].ToString();
                userControl1.DeleteUserbtn.Click += (sender, args) => DeleteUser(iduser);
                users.Add(userControl1);
            }
            UsersListBox.ItemsSource = users;

        }
        private void DeleteUser(int iduserInfo)
        {
            
            _userInfoTableAdapter.DeleteUserInfoQuery(iduserInfo);
            foreach( var userInfo in _userInfoTableAdapter.GetData())
            {
                if(userInfo.id_userinfo == iduserInfo)
                {
                    foreach (var libcard in libCardTableAdapter.GetData())
                    {
                        if (libcard.id_user == userInfo.user_id)
                        {
                            libCardTableAdapter.DeleteLibCardQuery(libcard.id_libcard);
                            userid = libcard.id_user;
                        }
                    }
                }
            }
            userstableAdapter.DeleteUsersQuery(userid);
            ListUsers();

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

        private void UsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersListBox.SelectedItem is UserControl1 selectedUserControl)
            {
                int iduser = Convert.ToInt32(selectedUserControl.IdUsertxt.Text);

                _windowLibrian.PageLibrian.Content = new UsersViewPage(iduser, _windowLibrian);
                UsersListBox.SelectedItem = null;
            }
        }

        private void AddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            _windowLibrian.PageLibrian.Content = new UsersAddPage(_windowLibrian);
        }
    }
}
