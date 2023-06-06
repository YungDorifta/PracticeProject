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
        //подключение к БД
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        static SqlConnection connection = new SqlConnection(connectionString);

        //данные конкретного изображения
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
        /// Вывод изображения по пути извлеченному из БД в окно с помощью запроса SQL
        /// </summary>
        /// <param name="image">Элемент изображения в окне</param>
        /// <param name="SQL">Запрос по которому извлекается путь к изображению</param>
        public static void LoadImage(System.Windows.Controls.Image image, string SQL)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)connection.Open();

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

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        /// <summary>
        /// Удаление информации о снимке из БД
        /// </summary>
        /// <param name="type">Тип снимка</param>
        /// <param name="ID">ID снимка</param>
        public static void DeleteImageFromDB(string type, int ID)
        {
            try
            {
                //открытие подключения
                connection.Open();

                string SQL;

                //удаление записи из ДБ
                if (type == "original") SQL = "DELETE FROM dbo.Originals WHERE (OriginalID = " + ID + ")";
                else SQL = "DELETE FROM dbo.Markups WHERE (MarkupID = " + ID + ")";
                SqlCommand command = new SqlCommand(SQL, connection);
                command.ExecuteScalar();

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Загрузка изображений по умолчанию
        /// </summary>
        /// <param name="TheOriginalWindowImage">Элемент оригинального изображения в окне</param>
        /// <param name="TheMarkupWindowImage">Элемент размеченного изображения в окне</param>
        public static void LoadDefaultImages(System.Windows.Controls.Image TheOriginalWindowImage, System.Windows.Controls.Image TheMarkupWindowImage)
        {
            try
            {
                connection.Open();

                string SQL;
                SqlCommand command;
                string OriginalID, MarkupID;

                //получение ID оригинального снимка по умолчанию
                SQL = "SELECT Min(OriginalID) FROM dbo.Originals";
                command = new SqlCommand(SQL, connection);
                OriginalID = Convert.ToString(command.ExecuteScalar());

                //получение ID размеченного снимка по умолчанию
                SQL = "SELECT Min(MarkupID) FROM dbo.Markups WHERE (OriginalID = " + OriginalID + ")";
                command = new SqlCommand(SQL, connection);
                MarkupID = Convert.ToString(command.ExecuteScalar());

                connection.Close();

                //загрузка изображений по умолчанию 
                SQL = "SELECT Picturepath FROM dbo.Originals WHERE OriginalID=" + OriginalID;
                LoadImage(TheOriginalWindowImage, SQL);
                SQL = "SELECT Picturepath FROM dbo.Markups WHERE (OriginalID=" + OriginalID + ") AND (MarkupID=" + MarkupID + ")";
                LoadImage(TheMarkupWindowImage, SQL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
