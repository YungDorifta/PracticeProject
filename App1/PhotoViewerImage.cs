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
        string type;

        //конструктор
        PhotoViewerImage()
        {

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

        //получение ID картинки???...
    }
}
