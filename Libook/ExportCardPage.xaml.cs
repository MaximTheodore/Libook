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
    /// Логика взаимодействия для ExportCardPage.xaml
    /// </summary>
    public partial class ExportCardPage : Page
    {
        UserInfoTableAdapter userInfoTableAdapter = new UserInfoTableAdapter();
        UsersTableAdapter usersTableAdapter = new UsersTableAdapter();
        RolesTableAdapter rolesTableAdapter = new RolesTableAdapter();
        LibCardTableAdapter libCardadapter = new LibCardTableAdapter();
        private int _idUserInfo;
        private int _iduser;
        public ExportCardPage(int _idUserInfo, int iduser)
        {
            InitializeComponent();
            this._idUserInfo = _idUserInfo;
            this._iduser = iduser;
            SeachUseInfo();
        }
        private void SeachUseInfo()
        {

            var allUserInfo = userInfoTableAdapter.GetData().Rows;
            var allusers = usersTableAdapter.GetData().Rows;
            var allroles = rolesTableAdapter.GetData().Rows;
            var allLibcard = libCardadapter.GetData().Rows;
            for(int i = 0; i< allUserInfo.Count; i++)
            {
                if ((int)allUserInfo[i][0] == _idUserInfo)
                {
                    byte[] imageData = (byte[])allUserInfo[i][1];
                    ImgUserExp.Source = DisplayImage(imageData);
                    NameUserExp.Text ="Имя: "+allUserInfo[i][2].ToString();
                    SurnameUserExp.Text = "Фамилия: " + allUserInfo[i][3].ToString();

                    for (int t = 0; t< allLibcard.Count; t++)
                    {
                        if ((int)allLibcard[t][0] == (int)allUserInfo[i][7])
                        {
                            DatesStartEnd.Text = ((DateTime)allLibcard[t][2]).ToString("dd-MM-yyyy") + " / " + ((DateTime)allLibcard[t][3]).ToString("dd-MM-yyyy");
                        }
                    }

                    for(int j = 0; j < allusers.Count; j++)
                    {
                        if ((int)allusers[j][0] == _iduser)
                        {
                            for(int k = 0; k < allroles.Count; k++)
                            {
                                if ((int)allroles[k][0]== (int)allusers[j][3])
                                {
                                    RoleUserExp.Text = "Роль: " + allroles[k][1].ToString();
                                }
                            }
                        }
                    }

                }
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
            defaultImage.UriSource = new Uri("/Libook;component/images/defaultperson.png", UriKind.Relative);
            defaultImage.CacheOption = BitmapCacheOption.OnLoad;
            defaultImage.EndInit();
            return defaultImage;
        }
        private void ExportCardBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".png",
                Filter = "PNG Files (*.png)|*.png|All files (*.*)|*.*"
            };

            
            bool? result = saveFileDialog.ShowDialog();

          
            if (result == true)
            {
               
                ExportElement.Width = 600; 
                ExportElement.Height = 400;

                
                ExportElement.Measure(new Size(ExportElement.Width, ExportElement.Height));
                ExportElement.Arrange(new Rect(0, 0, ExportElement.Width, ExportElement.Height));

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                    (int)ExportElement.ActualWidth,
                    (int)ExportElement.ActualHeight,
                    96, 96, PixelFormats.Pbgra32);

             
                renderBitmap.Render(ExportElement);

            
                CroppedBitmap croppedBitmap = new CroppedBitmap(
                    renderBitmap,
                    new Int32Rect(0, 0, (int)ExportElement.ActualWidth, (int)ExportElement.ActualHeight));

               
                PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(croppedBitmap));

                using (Stream fileStream = File.Create(saveFileDialog.FileName))
                {
                    pngEncoder.Save(fileStream);
                }

                ExportElement.Width = double.NaN;
                ExportElement.Height = double.NaN;
            }
        }
    }
}
