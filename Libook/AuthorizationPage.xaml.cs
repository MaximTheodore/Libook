using System;
using System.Collections.Generic;
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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        private MainWindow mainWindow;
        UsersTableAdapter adapter = new UsersTableAdapter();
        public AuthorizationPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Into_Click(object sender, RoutedEventArgs e)
        {
            var allogins = adapter.GetData().Rows;

            if (LoginBox.Text == null) notifier.ShowWarning("Поле логина не заполнено");
            if(PasswordTxt.Password == null) notifier.ShowWarning("Поле пароля не заполнено");

            for (int i = 0; i < allogins.Count; i++)
            {
                if (allogins[i][1].ToString() == LoginBox.Text &&
                    allogins[i][2].ToString() == PasswordTxt.Password)
                {
                    if ((int)allogins[i][3] == 1)
                    {
                        MainWindowLibrian mainWindowLibrian = new MainWindowLibrian((int)allogins[i][0], (int)allogins[i][3]);
                        mainWindowLibrian.Show();
                        Window parentWindow = Window.GetWindow(this);

                        parentWindow?.Close();

                    }
                    if ((int)allogins[i][3] == 2)
                    {
                        MainWindowReader reader = new MainWindowReader((int)allogins[i][0], (int)allogins[i][3]);
                        reader.Show();
                        Window parentWindow = Window.GetWindow(this);

                        parentWindow?.Close();
                    }
                }
                
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
        private void Registrat_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.NavigateToRegistrationPage();
        }
    }
}
