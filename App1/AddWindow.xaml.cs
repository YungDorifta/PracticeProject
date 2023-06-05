using PhotoViewer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        //cтрока подключения к БД
        string connectionString;

        //подключение к БД
        SqlConnection connection;

        //тип добавляемого снимка
        string type;

        /// <summary>
        /// Конструктор окна добавления снимка
        /// </summary>
        public AddWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            type = "original";
            this.Title = "Добавить оригинальный снимок";
        }

        /// <summary>
        /// Загрузка окна добавления снимка в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddWindowLoading(object sender, RoutedEventArgs e)
        {
            SelectType.SelectedIndex = 0;

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
        /// Заполнение выбора оригинала из существующих в БД при загрузке поля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrigName_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                //открытие подключения
                connection.Open();

                //получение ID и названия оригиналов из БД 
                string SQL = "SELECT OriginalID, Picturepath FROM dbo.Originals";
                SqlCommand command = new SqlCommand(SQL, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.InsertCommand = new SqlCommand("IDandPath", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 75, "Picturepath"));
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@OriginalID", SqlDbType.Int, 10, "OriginalID");
                parameter.Direction = ParameterDirection.Output;

                //вывод в таблицу хранения
                DataTable IDPathTable = new DataTable();
                adapter.Fill(IDPathTable);

                //вывод ID оригиналов в item
                foreach (DataRow row in IDPathTable.Rows)
                {
                    string path = row.Field<string>("Picturepath");
                    string[] pathsplit = path.Split('\\');
                    string name = pathsplit[pathsplit.Length - 1];

                    OrigName.Items.Add(row.Field<int>("OriginalID").ToString() + ": " + name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        
        /// <summary>
        /// Изменение содержимого окна, когда выбран тип снимка - оригинал
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedTypeOriginal(object sender, RoutedEventArgs e)
        {
            type = "original";
            this.Title = "Добавить оригинальный снимок";

            OrigName.IsEnabled = false;
            OrigName.Visibility = Visibility.Hidden;

            RegOrigLabel.Content = "Регион:";
            Region.Visibility = Visibility.Visible;
            Region.IsEnabled = true;
            SputLabel.Visibility = Visibility.Visible;
            Sputnik.Visibility = Visibility.Visible;
            Sputnik.IsEnabled = true;
        }

        /// <summary>
        /// Изменение содержимого окна, когда выбран тип снимка - разметка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedTypeMarkup(object sender, RoutedEventArgs e)
        {
            this.Title = "Добавить размеченный снимок";
            this.type = "markup";

            RegOrigLabel.Content = "Оригинал:";
            Region.IsEnabled = false;
            Region.Visibility = Visibility.Hidden;
            SputLabel.Visibility = Visibility.Hidden;
            Sputnik.IsEnabled = false;
            Sputnik.Visibility = Visibility.Hidden;

            OrigName.Visibility = Visibility.Visible;
            OrigName.IsEnabled = true;
        }
        
        /// <summary>
        /// Показать файл пути в строке вывода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindAddImagePath(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Image"; // Default file name
            dialog.DefaultExt = ".jpg"; // Default file extension
            dialog.Filter = "Images (.jpg)|*.jpg"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                AddFileName.Text = filename;
            }
        }

        /// <summary> 
        /// Проверка заполнения всех полей и разблокировка кнопки сохранения (для текстовых полей)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckForAllInputsTB(object sender, TextChangedEventArgs e)
        {
            if (AddFileName.IsLoaded == true)
            {
                if ((AddFileName.Text != "") && (AddFileName.Text != "Путь к файлу") && (SelectType.IsLoaded == true))
                { 
                    switch (SelectType.Text)
                    {
                        case "Оригинальный":
                            {
                                if ((Region.Text != "") && (Sputnik.Text != "") && (DateSelector.Text != "")) SaveBtn.IsEnabled = true;
                                else SaveBtn.IsEnabled = false;
                                break;
                            }
                        case "Размеченный":
                            {
                                if ((OrigName.Text != "") && (DateSelector.Text != "")) SaveBtn.IsEnabled = true;
                                else SaveBtn.IsEnabled = false;
                                break;
                            }
                        default:
                            {
                                SaveBtn.IsEnabled = false;
                                break;
                            }
                    }
                }
                else
                {
                    SaveBtn.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Проверка заполнения всех полей и разблокировка кнопки сохранения (для выбора даты и оригинала)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckForAllInputsSelector(object sender, SelectionChangedEventArgs e)
        {
            if (AddFileName.IsLoaded == true)
            {
                if ((AddFileName.Text != "") && (AddFileName.Text != "Путь к файлу") && (SelectType.IsLoaded == true))
                {
                    switch (SelectType.Text)
                    {
                        case "Оригинальный":
                            {
                                if ((Region.Text != "") && (Sputnik.Text != "") && (DateSelector.Text != "")) SaveBtn.IsEnabled = true;
                                else SaveBtn.IsEnabled = false;
                                break;
                            }
                        case "Размеченный":
                            {
                                if ((OrigName.Text != "") && (DateSelector.Text != "")) SaveBtn.IsEnabled = true;
                                else SaveBtn.IsEnabled = false;
                                break;
                            }
                        default:
                            {
                                SaveBtn.IsEnabled = false;
                                break;
                            }
                    }
                }
                else
                {
                    SaveBtn.IsEnabled = false;
                }
            }
        }

        //доделать
        /// <summary>
        /// Сохранить изображение и вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAndBackToMain(object sender, RoutedEventArgs e)
        {
            string AddingPath = AddFileName.Text;
            DateTime AddingDate = DateSelector.DisplayDate;

            if (this.type == "original")
            {
                string AddingSputnik = Sputnik.Text;
                string AddingRegion = Region.Text;

                try
                {
                    //открытие подключения
                    connection.Open();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            else if (this.type == "markup")
            {
                int AddingOriginalID = Convert.ToInt32(OrigName.Text.Split(':')[0]);

                try
                {
                    //открытие подключения
                    connection.Open();

                    //поиск доступного ID для добавления картинки
                    string SQL = "SELECT MAX(MarkupID) FROM dbo.Markups";
                    SqlCommand command = new SqlCommand(SQL, connection);
                    int newID = (int)command.ExecuteScalar();
                    newID++;

                    //добавление записи о снимке в таблицу БД
                    SQL = "INSERT INTO dbo.Markups (MarkupID, OriginalID, Date, Picturepath) VALUES (" + newID + "," + 
                        AddingOriginalID + "," + AddingDate + "," + AddingPath +");";
                    command = new SqlCommand(SQL, connection);
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
            }

            MainWindow MW = new MainWindow();
            MW.Show();
            this.Close();
        }
        
        /// <summary>
        /// Вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToMain(object sender, RoutedEventArgs e)
        {
            MainWindow MW = new MainWindow();
            MW.Show();
            this.Close();
        }
    }
}
