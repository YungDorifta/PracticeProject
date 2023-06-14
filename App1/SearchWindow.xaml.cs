using PhotoViewer;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для SearchPhotos.xaml
    /// </summary>
    public partial class SearchPhotos : Window
    {
        //снимки с главного окна
        PhotoViewerImage OriginalImage, MarkupImage;


        //загрузка окна
        /// <summary>
        /// Конструктор окна поиска снимков
        /// </summary>
        /// <param name="OriginalID"></param>
        /// <param name="MarkupID"></param>
        public SearchPhotos(string OriginalID, string MarkupID)
        {
            InitializeComponent();
            OriginalImage = new PhotoViewerImage("original", Convert.ToInt32(OriginalID));
            MarkupImage = new PhotoViewerImage("markup", Convert.ToInt32(MarkupID));
        }

        /// <summary>
        /// Загрузка названий оригиналов при загрузке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OriginalIDTB_Loaded(object sender, RoutedEventArgs e)
        {
            //загрузка списка
            int selected = -1;
            int current = 0;
            string[] names = PhotoViewerImage.FindImageNamesAndIDs("original");
            foreach (string name in names)
            {
                if (name != null)
                {
                    OriginalIDTB.Items.Add(name);
                    if (name.Split(':')[0] == OriginalImage.GetID().ToString()) selected = current;
                    current++;
                }
            }

            //выбор фото с главного окна
            if (selected != -1)
            {
                OriginalIDTB.SelectedIndex = selected;
            }
            else if (OriginalIDTB.Items.Count > 0) OriginalIDTB.SelectedIndex = 0;
        }
        
        /// <summary>
        /// Перезагрузка разметок при выборе другого оригинала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OriginalIDTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OriginalIDTB.IsLoaded)
            {
                //очистить список
                MarkupIDTB.Items.Clear();

                //извлечь новый ID
                int NewOriginalID = -1;
                if (OriginalIDTB.SelectedItem != null)
                {
                    if (OriginalIDTB.SelectedItem.ToString() != "") NewOriginalID = Convert.ToInt32(OriginalIDTB.SelectedItem.ToString().Split(':')[0]);
                }

                //перезагрузить список разметок
                int selected = -1;
                int current = 0;
                string[] names = new string[0];
                if (NewOriginalID != -1) names = PhotoViewerImage.FindImageNamesAndIDs(NewOriginalID);
                foreach (string name in names)
                {
                    if (name != null)
                    {
                        MarkupIDTB.Items.Add(name);
                        if (name.Split(':')[0] == MarkupImage.GetID().ToString()) selected = current;
                        current++;
                    }
                }
                
                //выбрать элемент, если совпадает с загруженным
                if (selected != -1)
                {
                    MarkupIDTB.SelectedIndex = selected;
                }
                else if (MarkupIDTB.Items.Count > 0) MarkupIDTB.SelectedIndex = 0;
            }
        }


        //фильтры поиска
        /// <summary>
        /// Загрузка фильтра со всеми существующими датами оригинала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrigDate_Loaded(object sender, RoutedEventArgs e)
        {
            string[] origDates = PhotoViewerImage.FindAllOrigDates();
            foreach (string date in origDates)
            {
                OrigDate.Items.Add(date);
            }
        }

        /// <summary>
        /// Загрузка фильтра со всеми существующими датами разметки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarkupDate_Loaded(object sender, RoutedEventArgs e)
        {
            string[] markDates = PhotoViewerImage.FindAllMarkDates();
            foreach (string date in markDates)
            {
                MarkupDate.Items.Add(date);
            }
        }
        
        /// <summary>
        /// Загрузка фильтра со всеми существующими регионами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Region_Loaded(object sender, RoutedEventArgs e)
        {
            string[] regions= PhotoViewerImage.FindAllRegions();
            foreach (string region in regions)
            {
                Region.Items.Add(region);
            }
        }

        /// <summary>
        /// Загрузка фильтра со всеми существующими спутниками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sputnik_Loaded(object sender, RoutedEventArgs e)
        {
            string[] sputniks = PhotoViewerImage.FindAllSputniks();
            foreach (string sputnik in sputniks)
            {
                Sputnik.Items.Add(sputnik);
                //if (sputnik == OriginalImage.GetSputnik()) Sputnik.SelectedIndex = Sputnik.Items.Count - 1;
            }
        }

        //!!!доделать: изменение полей при изменении фильтров
        private void OrigFiltersChanged(object sender, SelectionChangedEventArgs e)
        {
            OriginalIDTB.Items.Clear();

            string date = null;
            string region = null;
            string sputnik = null;

            if (OrigDate.SelectedItem != null)
            {
                if (OrigDate.SelectedItem.ToString() != "") date = OrigDate.SelectedItem.ToString();
            }
            if (Sputnik.SelectedItem != null)
            {
                if (Sputnik.SelectedItem.ToString() != "") sputnik = Sputnik.SelectedItem.ToString();
            }
            if (Region.SelectedItem != null)
            {
                if (Region.SelectedItem.ToString() != "") region = Region.SelectedItem.ToString();
            }
            
            string[] NewOrigs = PhotoViewerImage.FindImageNamesAndIDs("original", date, region, sputnik);
            foreach (string orig in NewOrigs)
            {
                OriginalIDTB.Items.Add(orig);
                
            }
            if (OriginalIDTB.Items.Count > 0) OriginalIDTB.SelectedIndex = 0;
        }
        //

        /// <summary>
        /// Перезагрузка списка при изменении фильтров разметки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarkupFiltersChanged(object sender, SelectionChangedEventArgs e)
        {
            //очистка списка
            MarkupIDTB.Items.Clear();

            //извлечение аргументов
            string dateIn = null;
            if (MarkupDate.SelectedItem != null)
            {
                if (MarkupDate.SelectedItem.ToString() != "") dateIn = MarkupDate.SelectedItem.ToString();
            }

            //поиск значений
            string[] NewMarks;
            NewMarks = PhotoViewerImage.FindImageNamesAndIDs(OriginalImage.GetID());
            if (dateIn != null)
            {
                if (dateIn != "")
                {
                    DateTime NewDate = Convert.ToDateTime(dateIn);
                    NewMarks = PhotoViewerImage.FindImageNamesAndIDs(OriginalImage.GetID(), NewDate);
                }
            }

            //заполнение списка
            foreach (string mark in NewMarks)
            {
                MarkupIDTB.Items.Add(mark);

            }
            if (MarkupIDTB.Items.Count > 0) MarkupIDTB.SelectedIndex = 0;
        }
        

        //выход
        /// <summary>
        /// Загрузка главного окна с найденными снимками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadAndBackToMain(object sender, RoutedEventArgs e)
        {
            //получение ID новых изображений
            string NewOriginalID = OriginalIDTB.Text.Split(':')[0];
            string NewMarkupID = MarkupIDTB.Text.Split(':')[0];

            //создание и переход к главному окну с новыми изображениями
            MainWindow MW;
            try
            {
                //при отсутствии новых ID - создать главное окно по умолчанию
                if(NewOriginalID == null || NewMarkupID == null)
                {
                    MW = new MainWindow();
                }
                else
                {
                    MW = new MainWindow(Convert.ToInt32(NewOriginalID), Convert.ToInt32(NewMarkupID));
                }
                MW.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        /// <summary>
        /// Загрузка главного окна с прошлыми снимками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToMain(object sender, RoutedEventArgs e)
        {
            MainWindow MW;
            try
            {
                MW = new MainWindow(OriginalImage.GetID(), MarkupImage.GetID());
                if (MW == null)
                {
                    MW = new MainWindow();
                }
                MW.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
