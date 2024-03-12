using Libook.LibookDataSetTableAdapters;
using Microsoft.WindowsAPICodePack.Shell;
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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Libook
{
    /// <summary>
    /// Логика взаимодействия для UsersAddPage.xaml
    /// </summary>
    public partial class UsersAddPage : Page
    {
        RolesTableAdapter rolesTableAdapter = new RolesTableAdapter();
        UsersTableAdapter usersTableAdapter = new UsersTableAdapter();
        MainWindowLibrian windowLibrian1;
      

        public UsersAddPage(MainWindowLibrian windowLibrian)
        {
            InitializeComponent();
            this.windowLibrian1 = windowLibrian;
            FullComboBox();
        }

        private void Nextbtn_Click(object sender, RoutedEventArgs e)
        {
            
            var allogins = usersTableAdapter.GetData().Rows;
            for (int i = 0; i < allogins.Count; i++)
            {
                if (allogins[i][1].ToString() == Logintxt.Text &&
                allogins[i][2].ToString() == Passwordtxt.Text)
                {
                    windowLibrian1.PageLibrian.Content = new UsersAddPage2(windowLibrian1, (int)allogins[i][0]);
                }

            }
        }
        private void FullComboBox()
        {
            Role.ItemsSource = rolesTableAdapter.GetData().Rows;
            Role.DisplayMemberPath = "Name_role";
            Role.SelectedValuePath = "Id_role";

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

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            int selectedRoleId = (int)Role.SelectedValue;

            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9].*?[0-9].*?[0-9])(?=.*?[#?!@$%^&*-]).{7,}$");
            if (!validateGuidRegex.IsMatch(Passwordtxt.Text))
            {
                notifier.ShowInformation("В пароле необходимо хотя бы 3 цифры, минимум 7 символов, минимум одна заглавная и строковая латинская буква, минимум один спец символ #?!@$%^&*-");
            }
            if (selectedRoleId == null) notifier.ShowError("Роль не заполнена"); 
            if (Logintxt.Text == null) notifier.ShowError("Логин не заполнен"); 
            if (Passwordtxt.Text == null) notifier.ShowError("Пароль не заполнен"); 


            usersTableAdapter.InsertUserQuery(Logintxt.Text, Passwordtxt.Text, selectedRoleId);
            notifier.ShowSuccess("Логин успешно добавлен");
            
        }

    }
}
