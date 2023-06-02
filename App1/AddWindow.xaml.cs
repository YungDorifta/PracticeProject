using PhotoViewer;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        //OriginalID изображений
        string OriginalID, MarkupID;

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
        /// Конструктор окна добавления снимка
        /// </summary>
        /// <param name="type"></param>
        public AddWindow(string type)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            if (type == "markup")
            {
                this.type = type;
                this.Title = "Добавить размеченный снимок";
            }
            else
            {
                this.type = "original";
                this.Title = "Добавить оригинальный снимок";
            }
        }

        /// <summary>
        /// Загрузка окна добавления снимка в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddWindowLoading(object sender, RoutedEventArgs e)
        {
            SelectType.SelectedIndex = 0;
            OrigName.Items.Add("item1");
            OrigName.Items.Add("item2");
        }

        /// <summary>
        /// Сохранить изображение и вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAndBackToMain(object sender, RoutedEventArgs e)
        {
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
        /// Изменение содержимого окна, когда выбран тип снимка - оригинал
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedTypeOriginal(object sender, RoutedEventArgs e)
        {
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
        /// Проверка заполнения всех полей и разблокировка кнопки сохранения (для даты)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckForAllInputsDate(object sender, SelectionChangedEventArgs e)
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
    }
}
