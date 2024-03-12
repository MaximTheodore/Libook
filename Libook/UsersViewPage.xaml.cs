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
    /// Логика взаимодействия для UsersViewPage.xaml
    /// </summary>
    public partial class UsersViewPage : Page
    {
        private int _iduserInfo;
        private MainWindowLibrian _windowLibrian;
        UserInfoTableAdapter userInfoTableAdapter = new UserInfoTableAdapter();
        UsersTableAdapter usersTableAdapter = new UsersTableAdapter();
        RolesTableAdapter rolesTableAdapter = new RolesTableAdapter();
        private byte[] _image;
        private int _idrole;
        private int _iduser;

        private string _login;
        private string _password;
        public UsersViewPage(int iduserInfo,MainWindowLibrian mainWindowLibrian)
        {
            InitializeComponent();
            this._iduserInfo = iduserInfo;
            this._windowLibrian = mainWindowLibrian;
            InfoUser();
            FullComboBox();
        }
        private void InfoUser()
        {
            foreach(var userInfo in userInfoTableAdapter.GetData())
            {
                if (userInfo.id_userinfo == _iduserInfo)
                {
                    ImgUserInfo.Source = DisplayImage(userInfo.iconlibrian);
                    NameUser.Text = userInfo.name;
                    SurnameUser.Text = userInfo.surname;
                    DateBorn.Text = userInfo.dateborn.ToString();
                    foreach (var user in usersTableAdapter.GetData())
                    {
                        if(user.id_user == userInfo.user_id)
                        {
                            _idrole = user.role;
                            _iduser = user.id_user;
                            _login = user.login;
                            _password = user.password;
                        }
                    }
                    
                }
            }
        }
        private void FullComboBox()
        {
            RoleComboBox.ItemsSource = rolesTableAdapter.GetData().Rows;
            RoleComboBox.DisplayMemberPath = "Name_role";
            RoleComboBox.SelectedValuePath = "Id_role";

            RoleComboBox.SelectedValue = _idrole;
        }

        private void UpdateBtnUser_Click(object sender, RoutedEventArgs e)
        {
            int selectedRoleId = (int)RoleComboBox.SelectedValue;
            usersTableAdapter.UpdateUsersQuery(_login, _password, selectedRoleId ,_iduser);
            _windowLibrian.PageLibrian.Content = new UsersPage(_windowLibrian);
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
