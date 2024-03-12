using System;
using System.Collections.Generic;
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
using Libook.LibookDataSetTableAdapters;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        UsersTableAdapter useradapter = new UsersTableAdapter();
        MainWindow mainWindow1;
        private bool isready=false;
        public RegistrationPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow1 = mainWindow;
            
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

            if (isready == true)
            {
                var allogins = useradapter.GetData().Rows;
                for (int i = 0; i < allogins.Count; i++)
                {
                    if (allogins[i][1].ToString() == LoginRegtxt.Text &&
                    allogins[i][2].ToString() == PasswordRegtxt.Text)
                    {
                        mainWindow1.Page.Content = new RegistrationPage2(mainWindow1, (int)allogins[i][0]);
                    }

                }

            }
            else notifier.ShowWarning("Вы не зарегестрировали логин и пароль");


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

        private void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            
            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9].*?[0-9].*?[0-9])(?=.*?[#?!@$%^&*-]).{7,}$");
            if (!validateGuidRegex.IsMatch(PasswordRegtxt.Text))
            {
                notifier.ShowInformation("В пароле необходимо хотя бы 3 цифры, минимум 7 символов, минимум одна заглавная и строковая латинская буква, минимум один спец символ #?!@$%^&*-");
            }
            useradapter.InsertUserQuery(LoginRegtxt.Text.ToString(), PasswordRegtxt.Text.ToString(), 2);
            notifier.ShowWarning("Вы зарегестрировали логин и пароль");
            isready = true;


        }
    }
}
