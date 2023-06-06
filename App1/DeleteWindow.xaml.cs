using PhotoViewer;
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
using System.Windows.Shapes;

namespace PhotoViewerPRCVI
{
    /// <summary>
    /// Логика взаимодействия для DeleteWindow.xaml
    /// </summary>
    public partial class DeleteWindow : Window
    {
        //ссылка на главное окно
        MainWindow mw;
        
        //cтрока подключения к БД
        string connectionString;

        //подключение к БД
        SqlConnection connection;

        //тип удаляемого изображения
        string type;

        //ID удаляемого изображения
        int ID;
        
        /// <summary>
        /// Конструктор окна удаления информации
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        public DeleteWindow(int ID, string type, MainWindow mw)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            if (type == "markup")
            {
                this.type = type;
                DeleteWarnLabel.Content = "Удалить информацию о текущем размеченном снимке?";
            }
            else
            {
                this.type = "original";
                DeleteWarnLabel.Content = "Удалить информацию о текущем оригинальном снимке?\nИнформация о всех размеченных снимках\nтекущего оригинального также будет удалена!";
            }
            this.mw = mw;
            this.ID = ID;
        }

        /// <summary>
        /// Загрузка окна удаления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //подключение к БД используя строку подключения
                connection = new SqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Удалить и вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteAndBackToMain(object sender, RoutedEventArgs e)
        {
            if (type == "original")
            {
                try
                {
                    //открытие подключения
                    connection.Open();

                    //добавление записи о снимке в таблицу БД
                    string SQL = "DELETE FROM dbo.Originals WHERE (OriginalID = " + ID + ")";
                    SqlCommand command = new SqlCommand(SQL, connection);
                    command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                mw.Activate();
                this.Close();
            }
            else
            {
                try
                {
                    //открытие подключения
                    connection.Open();

                    //добавление записи о снимке в таблицу БД
                    string SQL = "DELETE FROM dbo.Markups WHERE (MarkupID = " + ID + ")";
                    SqlCommand command = new SqlCommand(SQL, connection);
                    command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                mw.Activate();
                this.Close();
            }
        }

        /// <summary>
        /// Вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToMain(object sender, RoutedEventArgs e)
        {
            mw.Activate();
            this.Close();
        }
    }
}
