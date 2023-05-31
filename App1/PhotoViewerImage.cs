using System;
using System.Windows;
using System.Configuration;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace PhotoViewerPRCVI
{
    public partial class PhotoViewerImage
    {
        Uri Path;
        string Type;
        int OriginalID;
        int MarkupID;
        DateTime date;

        /// <summary>
        /// Конструктор объекта оригинального сниимка, содержащего данные
        /// </summary>
        /// <param name="OriginalID">ID оригинального снимка</param>
        /// <param name="connection">Подключение к серверу</param>
        public PhotoViewerImage(int OriginalID, SqlConnection connection)
        {
            this.Type = "original";

            //сохранение пути к изображению полученного из БД
            string SQL = "SELECT Picturepath FROM dbo.Originals WHERE OriginalID=" + OriginalID;
            SqlCommand command = new SqlCommand(SQL, connection);
            string imageUri = (string)command.ExecuteScalar();
            this.Path = new Uri(imageUri);

            //сохранение OriginalID в поле объекта
            this.OriginalID = OriginalID;

            //сохранение даты изображения полученной из БД
            /*
            SQL = "SELECT Date FROM dbo.Originals WHERE OriginalID=" + OriginalID;
            command = new SqlCommand(SQL, connection);
            this.date = (DateTime)command.ExecuteScalar();
            */
        }

        /// <summary>
        /// Конструктор объекта размеченного снимка, содержащего параметры
        /// </summary>
        /// <param name="OriginalID"></param>
        /// <param name="MarkupID"></param>
        /// <param name="connection"></param>
        public PhotoViewerImage(int OriginalID, int MarkupID, SqlConnection connection)
        {
            this.Type = "markup";

            //сохранение пути к изображению полученного из БД
            string SQL = "SELECT Picturepath FROM dbo.Markups WHERE OriginalID=" + OriginalID + ") AND (MarkupID=" + MarkupID + ")";
            SqlCommand command = new SqlCommand(SQL, connection);
            string imageUri = (string)command.ExecuteScalar();
            this.Path = new Uri(imageUri);

            //сохранение OriginalID в поле объекта
            this.OriginalID = OriginalID;
            this.MarkupID = MarkupID;

            //сохранение даты изображения полученной из БД
            /*
            SQL = "SELECT Date FROM dbo.Markups WHERE OriginalID=" + OriginalID + ") AND (MarkupID=" + MarkupID + ")"; ;
            command = new SqlCommand(SQL, connection);
            this.date = (DateTime)command.ExecuteScalar();
            */
        }
        
        /// <summary>
        /// Проверка снимка: оригинал или разметка
        /// </summary>
        /// <returns></returns>
        public bool isOriginal()
        {
            if (this.Type == "original") return true;
            else return false;
        }
        
        /// <summary>
        /// Получение ID оригинального фото
        /// </summary>
        /// <returns></returns>
        public int getOriginalID()
        {
            return this.OriginalID;
        }

        /// <summary>
        /// Получение ID размеченного фото
        /// </summary>
        /// <returns></returns>
        public int getMarkupID()
        {
            return this.MarkupID;
        }

        /// <summary>
        /// Загрузка объекта-изображения в окно
        /// </summary>
        /// <param name="image">Элемент-изображение в окне</param>
        public void LoadImage(System.Windows.Controls.Image image)
        {
            try
            {
                //вывод изображения по пути BMP
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = this.Path;
                myBitmapImage.EndInit();

                //вывод изображения из BMP в окно
                image.Source = myBitmapImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Вывод изображения по пути извлеченному из БД в окно
        /// </summary>
        /// <param name="image">Элемент изображения в окне</param>
        /// <param name="SQL">Запрос по которому извлекается путь к изображению</param>
        /// <param name="connection">Подключение к БД</param>
        public static void LoadImage(System.Windows.Controls.Image image, string SQL, SqlConnection connection)
        {
            try
            {
                //получение пути к изображению из БД
                SqlCommand command = new SqlCommand(SQL, connection);
                string imageUri = (string)command.ExecuteScalar();

                //вывод изображения из БД в BMP
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(imageUri);
                myBitmapImage.EndInit();

                //вывод изображения из BMP в окно
                image.Source = myBitmapImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
