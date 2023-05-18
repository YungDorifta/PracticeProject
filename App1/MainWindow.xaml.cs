using PhotoViewerPRCVI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace PhotoViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //cтрока подключения к БД
        string connectionString;

        //ID изображений
        string OriginalID, MarkupID;

        /// <summary>
        /// Конструктор главного окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            this.OriginalID = Convert.ToString(1);
            this.MarkupID = Convert.ToString(1);
        }

        /// <summary>
        /// Конструктор главного окна
        /// </summary>
        public MainWindow(string OriginalID, string MarkupID)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            this.OriginalID = OriginalID;
            this.MarkupID = MarkupID;
        }
        
        /// <summary>
        /// Загрузка главного окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //сброс соединения
            SqlConnection connection = null;

            try
            {
                //подключение к БД используя строку подключения
                connection = new SqlConnection(connectionString);

                //открытие подключения
                connection.Open();

                //вывод картинок по путям из БД
                string SQL = "SELECT Picturepath FROM dbo.Originals WHERE OriginalID=" + OriginalID;
                PhotoViewerImage.LoadImage(originalImage, SQL, connection);
                SQL = "SELECT Picturepath FROM dbo.Markups WHERE (OriginalID=" + OriginalID + ") AND (MarkupID=" + MarkupID + ")";
                PhotoViewerImage.LoadImage(markupImage, SQL, connection);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        /// <summary>
        /// Переход в окно полной информации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickMeBtn_Click(object sender, RoutedEventArgs e)
        {
            Button Btn = (Button)sender;
            FullInfoWindow FIW = new FullInfoWindow(2, 2);
            FIW.Show();
            this.Close();
        }
    }
}
