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

!!!Главная:     вывод доп информаци под фото, надписи где какое фото
                ссылки на открываемые окна для того, чтобы нельзя было открыть окна одного типа кучу раз
                (?)связь окон: если открыто окно добавления и удаления, чтобы одно не мешало другому. Либо сделать так, чтобы можно было делать одно действие за один раз

!!!Удаление:   смена картинок в главном окне при удалении снимка/ов

Доп. информация: вывод доп информации в таблицу доделать

Поиск:      поиск изображений по различной информации

Изменение:  заполнение таблицы информацией,
            сохранение изменений

(?):
База Данных: добавить доп. поля 
Везде:      адаптивное изображение и размеры
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

        //ссылки на функциональные окна
        public ChangeWindow CW;
        public DeleteWindow DW;

        /// <summary>
        /// Конструктор главного окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            this.OriginalID = null;
            this.MarkupID = null;
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

                if (OriginalID == null || MarkupID == null)
                {
                    string PreSQL = "SELECT Min(OriginalID) FROM dbo.Originals";
                    SqlCommand preCommand = new SqlCommand(PreSQL, connection);
                    OriginalID = Convert.ToString(preCommand.ExecuteScalar());
                    PreSQL = "SELECT Min(MarkupID) FROM dbo.Markups WHERE (OriginalID = " + OriginalID + ")";
                    preCommand = new SqlCommand(PreSQL, connection);
                    MarkupID = Convert.ToString(preCommand.ExecuteScalar());
                }

                //вывод картинок по путям из БД
                string SQL = "SELECT Picturepath FROM dbo.Originals WHERE OriginalID=" + OriginalID;
                PhotoViewerImage.LoadImage(TheOriginalWindowImage, SQL);
                SQL = "SELECT Picturepath FROM dbo.Markups WHERE (OriginalID=" + OriginalID + ") AND (MarkupID=" + MarkupID + ")";
                PhotoViewerImage.LoadImage(TheMarkupWindowImage, SQL);
                
                //PhotoViewerImage Orig = new PhotoViewerImage(1, connection);
                //Orig.LoadImage(TheOriginalWindowImage);
                //PhotoViewerImage Mark = new PhotoViewerImage(1, 1, connection);
                //Mark.LoadImage(TheMarkupWindowImage);
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
        private void AddWindowOpen(object sender, RoutedEventArgs e)
        {
            if (this.DW == null)
            {
                if (this.CW == null)
                {
                    AddWindow AW = new AddWindow();
                    AW.Show();
                    this.Close();
                }
                else MessageBox.Show("Нельзя открыть окно добавления снимка\nдо закрытия окна изменения снимка", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else MessageBox.Show("Нельзя открыть окно добавления снимка\nдо закрытия окна удаления снимка", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
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
            this.DW = new DeleteWindow(Convert.ToInt32(OriginalID), "original", this);
            this.DW.Show();
        }

        /// <summary>
        /// Удалить разметку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMarkupWindowOpen(object sender, RoutedEventArgs e)
        {
            this.DW = new DeleteWindow(Convert.ToInt32(MarkupID), "markup", this);
            this.DW.Show();
        }

        /// <summary>
        /// Сброс до изображений по умолчанию
        /// </summary>
        public void ReloadToDefaultImages()
        {
            PhotoViewerImage.LoadDefaultImages(TheOriginalWindowImage, TheMarkupWindowImage);
        }
    }
}
