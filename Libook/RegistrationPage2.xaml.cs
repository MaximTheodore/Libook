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
using Libook.LibookDataSetTableAdapters;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage2.xaml
    /// </summary>
    public partial class RegistrationPage2 : Page
    {
        private UserInfoTableAdapter _adapter = new UserInfoTableAdapter();
        UsersTableAdapter usersTableAdapter = new UsersTableAdapter();
        LibCardTableAdapter lib = new LibCardTableAdapter();
        private MainWindow _window;
        private byte[] _image;
        private int _iduser;
        private int _idcard;
        
        public RegistrationPage2(MainWindow window, int iduser)
        {
            InitializeComponent();
            this._window = window;
            this._iduser = iduser;
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

        private void Registrat_Click(object sender, RoutedEventArgs e)
        {
            var nameCheck = @"^[а-яА-Яa-zA-Z]{2,50}$";
            var amountnameCheck = @"^.{2,50}$";
            var surnameCheck = @"^[а-яА-Яa-zA-Z]{2,100}$";
            var AmountsurnameCheck = @"^.{2,100}$";
            var firstPhoneCheck = @"^[7-8]";
            var amountPhoneCheck = @"^[7-8][0-9]{10}$";

            DateTime date = (DateTime)DatebornTxt.SelectedDate;
            string formattedDate = date.ToString("yyyy-MM-dd");
            

            if (Regex.IsMatch(Nametxt.Text, nameCheck)) notifier.ShowWarning("В имени должна использоваться латиница или кириллица");
            if (Regex.IsMatch(Nametxt.Text, amountnameCheck)) notifier.ShowWarning("В имени должно быть минимум 2 символа");
            if (Regex.IsMatch(Surnametxt.Text, surnameCheck)) notifier.ShowWarning("Фамилия должна содеражать кириллицу или латиницу");
            if (Regex.IsMatch(Surnametxt.Text, AmountsurnameCheck)) notifier.ShowWarning("В фамилии должно быть минимум 2 символа");
            if (Regex.IsMatch(Phonetxt.Text, firstPhoneCheck)) notifier.ShowWarning("Первая цифра телефона должна быть 7 или 8");
            if (Regex.IsMatch(Phonetxt.Text, amountPhoneCheck)) notifier.ShowWarning("Номер телефона должен содержать 11 цифр");
            if (!DatebornTxt.SelectedDate.HasValue) notifier.ShowError("Поле даты рождения не заполнено");
            if (Nametxt.Text == null) notifier.ShowError("Поле имени не заполнено");
            if (Surnametxt.Text == null) notifier.ShowError("Поле имени не заполнено");
            if (Imagetxt.Text == null) notifier.ShowError("Картинка не выбрана");
            if (Phonetxt.Text == null) notifier.ShowError("Поле номера телефона не заполнено");
            else
            {

                foreach(var item in lib.GetData())
                {
                    if (item.id_user == _iduser)
                    {
                        _idcard = item.id_libcard;
                    }
                }

                _adapter.InsertUserInfoQuery(_image, Nametxt.Text.ToString(), Surnametxt.Text.ToString(), formattedDate, Convert.ToInt64(Phonetxt.Text), _iduser, _idcard);
                _window.Page.Content = new AuthorizationPage(_window);
            }
        }

        private void Imagetxt_GotFocus(object sender, RoutedEventArgs e)
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

                Imagetxt.Text = imagePath;

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
    }
}
