using PhotoViewer;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PhotoViewerPRCVI
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        //тип добавляемого снимка
        string type;

        /// <summary>
        /// Конструктор окна добавления снимка
        /// </summary>
        public AddWindow()
        {
            InitializeComponent();
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
        }
        
        /// <summary>
        /// Заполнение выбора оригинала из существующих в БД при загрузке поля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrigName_Loaded(object sender, RoutedEventArgs e)
        {
            string[] names = PhotoViewerImage.FindImageNamesAndIDs("original");
            foreach (string name in names)
            {
                OrigName.Items.Add(name);
            }
            if (OrigName.Items.Count > 0) OrigName.SelectedIndex = 0;
        }

        /// <summary>
        /// Загрузка поля с часами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HoursBox_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i <= 23; i++)
            {
                hoursBox.Items.Add(i);
            }
        }

        /// <summary>
        /// Загрузка поля с минутами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinutesBox_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i <= 59; i++)
            {
                minutesBox.Items.Add(i);
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
            // Открыть диалоговое окно
            var dialog = new Microsoft.Win32.OpenFileDialog();
            // Имя файла по умолчанию
            dialog.FileName = "Image";
            // Расширение файла по умолчанию
            dialog.DefaultExt = ".jpg";
            // Фильтр файлоа по расширению
            dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG)|*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG|All files (*.*)|*.*"; 

            // Открыть диалоговое окно и по окончанию работы проверить результат
            bool? result = dialog.ShowDialog();

            // Внести имя файла в поле при наличии результата
            if (result == true)
            {
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
        
        /// <summary>
        /// Сохранить изображение и вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAndBackToMain(object sender, RoutedEventArgs e)
        {
            //данные для новой записи о снимке
            string AddingPath = AddFileName.Text;
            DateTime AddingDate = DateSelector.DisplayDate;
            double AddingHours;
            double AddingMinutes;

            //корректировка времени при отсутствии заполнения полей
            if (hoursBox.Text == "")
            {
                AddingHours = 0;
            }
            else
            {
                AddingHours = Convert.ToDouble(hoursBox.Text);
            }
            if (minutesBox.Text == "")
            {
                AddingMinutes = 0;
            }
            else
            {
                AddingMinutes = Convert.ToDouble(minutesBox.Text);
            }

            //добавление в дату времени
            AddingDate = AddingDate.AddHours(AddingHours);
            AddingDate = AddingDate.AddMinutes(AddingMinutes);

            //добавление записи о снимке
            if (this.type == "original")
            {
                string AddingSputnik = Sputnik.Text;
                string AddingRegion = Region.Text;
                PhotoViewerImage.AddOriginalImageInDB(AddingPath, AddingDate, AddingSputnik, AddingRegion);
            }
            else if (this.type == "markup")
            {
                int AddingOriginalID = Convert.ToInt32(OrigName.Text.Split(':')[0]);
                PhotoViewerImage.AddMarkupImageInDB(AddingPath, AddingDate, AddingOriginalID);
            }

            //завершение работы окна
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
