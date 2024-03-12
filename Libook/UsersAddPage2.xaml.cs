using Libook.LibookDataSetTableAdapters;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для UsersAddPage.xaml
    /// </summary>
    public partial class UsersAddPage2 : Page
    {
        private UserInfoTableAdapter _adapter = new UserInfoTableAdapter();
        UsersTableAdapter usersTableAdapter = new UsersTableAdapter();
        LibCardTableAdapter lib = new LibCardTableAdapter();
        private MainWindowLibrian _mainWindowLibrian1;
        private byte[] _image;
        private int _idcard;
        private int _iduser;

        public UsersAddPage2(MainWindowLibrian mainWindowLibrian, int iduser)
        {
            InitializeComponent();
            this._iduser = iduser;
            this._mainWindowLibrian1 = mainWindowLibrian;

            CreateLibCard();
        }
        private void CreateLibCard()
        {
            DateTime currentDate = DateTime.Now;
            DateTime futureDate = currentDate.AddYears(2);
            string startLibcard = currentDate.ToString("yyyy-MM-dd");
            string endLibcard = futureDate.ToString("yyyy-MM-dd");
            var allogins = usersTableAdapter.GetData().Rows;
            for (int i = 0; i < allogins.Count; i++)
            {
                if ((int)allogins[i][0] == _iduser)
                {
                    lib.InsertLibCardQuery((int)allogins[i][0], startLibcard, endLibcard);

                }

            }

        }

        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
            var nameCheck = @"^[а-яА-Яa-zA-Z]{2,50}$";
            var amountnameCheck = @"^.{2,50}$";
            var surnameCheck = @"^[а-яА-Яa-zA-Z]{2,100}$";
            var AmountsurnameCheck = @"^.{2,100}$";
            var firstPhoneCheck = @"^[7-8]";
            var amountPhoneCheck = @"^[7-8][0-9]{10}$";

            DateTime date = (DateTime)Dateborn.SelectedDate;
            string formattedDate = date.ToString("yyyy-MM-dd");


            if (Regex.IsMatch(nametxt.Text, nameCheck)) notifier.ShowWarning("В имени должна использоваться латиница или кириллица");
            if (Regex.IsMatch(nametxt.Text, amountnameCheck)) notifier.ShowWarning("В имени должно быть минимум 2 символа");
            if (Regex.IsMatch(Surnametxt.Text, surnameCheck)) notifier.ShowWarning("Фамилия должна содеражать кириллицу или латиницу");
            if (Regex.IsMatch(Surnametxt.Text, AmountsurnameCheck)) notifier.ShowWarning("В фамилии должно быть минимум 2 символа");
            if (Regex.IsMatch(Phonetxt.Text, firstPhoneCheck)) notifier.ShowWarning("Первая цифра телефона должна быть 7 или 8");
            if (Regex.IsMatch(Phonetxt.Text, amountPhoneCheck)) notifier.ShowWarning("Номер телефона должен содержать 11 цифр");
            if (!Dateborn.SelectedDate.HasValue) notifier.ShowError("Поле даты рождения не заполнено");
            if (nametxt.Text == null) notifier.ShowError("Поле имени не заполнено");
            if (Surnametxt.Text == null) notifier.ShowError("Поле имени не заполнено");
            if (Imgtxt.Text == null) notifier.ShowError("Картинка не выбрана");
            if (Phonetxt.Text == null) notifier.ShowError("Поле номера телефона не заполнено");
            else
            {

                foreach (var item in lib.GetData())
                {
                    if(item.id_user == _iduser)
                    {
                        _idcard = item.id_libcard;
                    }
                }

                _adapter.InsertUserInfoQuery(_image, nametxt.Text.ToString(), Surnametxt.Text.ToString(), formattedDate, Convert.ToInt64(Phonetxt.Text), _iduser, _idcard);
                _mainWindowLibrian1.PageLibrian.Content = new UsersTableAdapter();
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

        private void Imgtxt_GotFocus(object sender, RoutedEventArgs e)
        {
            ChooseImage();
        }
        private void ChooseImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                Imgtxt.Text = imagePath;

                _image = File.ReadAllBytes(imagePath);

            }
        }
    }
}
