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

/*
Доделать:

Везде: адаптивное изображение и размеры

Главная: вывод доп информаци под фото, надписи где какое фото
Доп. информация: вывод доп информации в таблицу доделать  

Поиск: поиск изображений по различной информации
Добавление: заполнение имен существующих оригиналов, разблокировка кнопки при заполнении инфоромации, добавление записи по нажатию кнопки
Изменение: заполнение таблицы информацией, сохранение изменений
Удаление: само удаление информации из таблиц при нажатии на кнопку

База Данных: добавить доп. поля
*/

namespace PhotoViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //cтрока подключения к БД
        string connectionString;

        //OriginalID изображений
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
        /// <param name="OriginalID">ID оригинального изображения</param>
        /// <param name="MarkupID">ID размеченного изображения</param>
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
                
                //PhotoViewerImage Orig = new PhotoViewerImage(1, connection);
                //Orig.LoadImage(originalImage);
                //PhotoViewerImage Mark = new PhotoViewerImage(1, 1, connection);
                //Mark.LoadImage(markupImage);
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
        /// Полная информация
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoWindowOpen(object sender, RoutedEventArgs e)
        {
            Button Btn = (Button)sender;
            FullInfoWindow FIW = new FullInfoWindow(2, 2);
            FIW.Show();
            this.Close();
        }
        
        /// <summary>
        /// Поиск изображений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchPhotosClick(object sender, RoutedEventArgs e)
        {
            SearchPhotos SPW = new SearchPhotos(OriginalID, MarkupID);
            SPW.Show();
            this.Close();
        }

        /// <summary>
        /// Добавить оригинал
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddOrigWidowOpen(object sender, RoutedEventArgs e)
        {
            AddWindow AW = new AddWindow("original");
            AW.Show();
            this.Close();
        }

        /// <summary>
        /// Добавить разметку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMarkupWidowOpen(object sender, RoutedEventArgs e)
        {
            AddWindow AW = new AddWindow("markup");
            AW.Show();
            this.Close();
        }

        /// <summary>
        /// Изменить информацию об оригинальном фото
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeOrigWindowOpen(object sender, RoutedEventArgs e)
        {
            ChangeWindow CW = new ChangeWindow(Convert.ToInt32(OriginalID), "original");
            CW.Show();
            this.Close();
        }

        /// <summary>
        /// Изменить информацию об оригинальном фото
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeMarkupWindowOpen(object sender, RoutedEventArgs e)
        {
            ChangeWindow CW = new ChangeWindow(Convert.ToInt32(MarkupID), "markup");
            CW.Show();
            this.Close();
        }
        
        /// <summary>
        /// Удалить оригинал
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteOrigWidowOpen(object sender, RoutedEventArgs e)
        {
            DeleteWindow DW = new DeleteWindow(Convert.ToInt32(OriginalID), "original", this);
            DW.Show();
        }

        /// <summary>
        /// Удалить разметку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMarkupWindowOpen(object sender, RoutedEventArgs e)
        {
            DeleteWindow DW = new DeleteWindow(Convert.ToInt32(MarkupID), "markup", this);
            DW.Show();
        }
    }
}
