using System;
using System.Windows;
using System.Configuration;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media.Imaging;

namespace PhotoViewer
{
    /// <summary>
    /// Логика взаимодействия для FullInfoWindow.xaml
    /// </summary>
    public partial class FullInfoWindow : Window
    {
        //cтрока подключения к БД
        string connectionString;

        //OriginalID изображений
        string OriginalID, MarkupID;

        /// <summary>
        /// Конструктор окна доп. информации
        /// </summary>
        public FullInfoWindow(int OriginalID, int MarkupID)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            this.OriginalID = Convert.ToString(OriginalID);
            this.MarkupID = Convert.ToString(MarkupID);
        }

        /// <summary>
        /// Загрузка окна доп. информации
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
                LoadImage(originalImage, SQL, connection);
                SQL = "SELECT Picturepath FROM dbo.Markups WHERE (OriginalID=" + OriginalID + ") AND (MarkupID=" + MarkupID + ")";
                LoadImage(markupImage, SQL, connection);

                //заполнение таблицы информацией
                SQL = "SELECT * FROM dbo.Originals";
                LoadTable(FullInfoTable, SQL, connection);
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
        /// Вывод информации из таблицы БД в таблицу в окне
        /// </summary>
        /// <param name="table">Элемент таблицы в окне</param>
        /// <param name="SQL">Запрос для извлечения данных</param>
        /// <param name="connection">Подключение к БД</param>
        private void LoadTable(DataGrid table, string SQL, SqlConnection connection)
        {
            try
            {
                //таблица для хранения информации, извлеченной из БД
                DataTable keepTable = new DataTable();
            
                //полная команда (SQL query + connection query) для извлечения информации
                SqlCommand command = new SqlCommand(SQL, connection);

                //адаптер для извлечения информации из БД в Таблицу
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                // установка команды на добавление для вызова хранимой процедуры
                adapter.InsertCommand = new SqlCommand("sp_InsertPhone", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime, 50, "Date"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 50, "Picturepath"));

                //???
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@OriginalID", SqlDbType.Int, 1, "OriginalID");
                parameter.Direction = ParameterDirection.Output;

                //заполнение адаптером таблицы хранения информации из БД
                adapter.Fill(keepTable);

                //заполнение элемента таблицы информацией из таблицы хранения
                table.ItemsSource = keepTable.DefaultView;
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
        private void LoadImage(Image image, string SQL, SqlConnection connection)
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

        /// <summary>
        /// Возвращение на главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MW = new MainWindow(OriginalID, MarkupID);
            MW.Show();
            this.Close();
        }
    }
}
