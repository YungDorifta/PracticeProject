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
            if (OriginalID != null && MarkupID != null)
            {
                if (OriginalID != "" && MarkupID != "")
                {
                    OriginalImage = new PhotoViewerImage("original", Convert.ToInt32(OriginalID));
                    MarkupImage = new PhotoViewerImage("markup", Convert.ToInt32(MarkupID));
                }
            }
        }

        /// <summary>
        /// Загрузка названий оригиналов при загрузке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OriginalIDTB_Loaded(object sender, RoutedEventArgs e)
        {
            try
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
                        if (OriginalImage != null)
                        {
                            if (name.Split(':')[0] == OriginalImage.GetID().ToString()) selected = current;
                        }
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        /// <summary>
        /// Перезагрузка разметок при выборе другого оригинала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OriginalIDTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (OriginalIDTB.IsLoaded)
                {
                    //очистить список
                    MarkupIDTB.Items.Clear();

                    //если выбран оригинал
                    if (OriginalIDTB.SelectedItem != null)
                    {
                        //если оригинал содержит значение
                        if (OriginalIDTB.SelectedItem.ToString() != "")
                        {
                            //извлечь ID
                            int NewOriginalID = Convert.ToInt32(OriginalIDTB.SelectedItem.ToString().Split(':')[0]);

                            //извлечение аргументов (дата)
                            string dateIn = null;
                            if (MarkupDate.SelectedItem != null)
                            {
                                if (MarkupDate.SelectedItem.ToString() != "") dateIn = MarkupDate.SelectedItem.ToString();
                            }

                            //извлечение нового списка разметок
                            string[] names = PhotoViewerImage.FindImageNamesAndIDs(NewOriginalID);
                            if (dateIn != null)
                            {
                                if (dateIn != "")
                                {
                                    DateTime NewDate = Convert.ToDateTime(dateIn);
                                    names = PhotoViewerImage.FindImageNamesAndIDs(Convert.ToInt32(OriginalIDTB.SelectedItem.ToString().Split(':')[0]), NewDate);
                                }
                            }

                            //перезагрузить список разметок
                            int selected = -1;
                            int current = 0;
                            foreach (string name in names)
                            {
                                if (name != null)
                                {
                                    MarkupIDTB.Items.Add(name);
                                    if (MarkupImage != null)
                                    {
                                        if (name.Split(':')[0] == MarkupImage.GetID().ToString()) selected = current;
                                    }
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

                    if (OriginalIDTB.SelectedItem != null && MarkupIDTB.SelectedItem != null)
                    {
                        if (OriginalIDTB.SelectedItem.ToString() != "" && MarkupIDTB.SelectedItem.ToString() != "") ButtonFind.IsEnabled = true;
                        else ButtonFind.IsEnabled = false;
                    }
                    else ButtonFind.IsEnabled = false;
                }
                else ButtonFind.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// При изменении разметки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarkupIDTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OriginalIDTB.SelectedItem != null && MarkupIDTB.SelectedItem != null)
            {
                if (OriginalIDTB.SelectedItem.ToString() != "" && MarkupIDTB.SelectedItem.ToString() != "") ButtonFind.IsEnabled = true;
                else ButtonFind.IsEnabled = false;
            }
            else ButtonFind.IsEnabled = false;
        }

        //фильтры поиска
        /// <summary>
        /// Загрузка фильтра со всеми существующими датами оригинала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrigDate_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] origDates = PhotoViewerImage.FindAllOrigDates();
                foreach (string date in origDates)
                {
                    OrigDate.Items.Add(date);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Загрузка фильтра со всеми существующими датами разметки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarkupDate_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] markDates = PhotoViewerImage.FindAllMarkDates();
                foreach (string date in markDates)
                {
                    MarkupDate.Items.Add(date);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        /// <summary>
        /// Загрузка фильтра со всеми существующими регионами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Region_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] regions= PhotoViewerImage.FindAllRegions();
                foreach (string region in regions)
                {
                    Region.Items.Add(region);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Загрузка фильтра со всеми существующими спутниками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sputnik_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] sputniks = PhotoViewerImage.FindAllSputniks();
                foreach (string sputnik in sputniks)
                {
                    Sputnik.Items.Add(sputnik);
                    //if (sputnik == OriginalImage.GetSputnik()) Sputnik.SelectedIndex = Sputnik.Items.Count - 1;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        /// <summary>
        /// Изменение фильтров оригинала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrigFiltersChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                OriginalIDTB.Items.Clear();

                //извлечение данных фильтров
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

                //извлечение нового списка из БД
                string[] NewOrigs = PhotoViewerImage.FindImageNamesAndIDs("original", date, region, sputnik);

                //загрузка списка из бд в окно
                foreach (string orig in NewOrigs)
                {
                    OriginalIDTB.Items.Add(orig);

                }
                if (OriginalIDTB.Items.Count > 0) OriginalIDTB.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Перезагрузка списка при изменении фильтров разметки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarkupFiltersChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //очистка списка
                MarkupIDTB.Items.Clear();

                //при имении выбранного оригинала
                if (OriginalIDTB.SelectedItem != null)
                {
                    //если выбранный оригинал имеет текст
                    if (OriginalIDTB.SelectedItem.ToString() != "")
                    {
                        //извлечение аргументов (дата)
                        string dateIn = null;
                        if (MarkupDate.SelectedItem != null)
                        {
                            if (MarkupDate.SelectedItem.ToString() != "") dateIn = MarkupDate.SelectedItem.ToString();
                        }

                        //поиск значений (при неимении даты - не учитывать)
                        string[] NewMarks;
                        NewMarks = PhotoViewerImage.FindImageNamesAndIDs(Convert.ToInt32(OriginalIDTB.SelectedItem.ToString().Split(':')[0]));
                        if (dateIn != null)
                        {
                            if (dateIn != "")
                            {
                                DateTime NewDate = Convert.ToDateTime(dateIn);
                                NewMarks = PhotoViewerImage.FindImageNamesAndIDs(Convert.ToInt32(OriginalIDTB.SelectedItem.ToString().Split(':')[0]), NewDate);
                            }
                        }

                        //заполнение списка полученными именами
                        foreach (string mark in NewMarks)
                        {
                            MarkupIDTB.Items.Add(mark);

                        }
                        if (MarkupIDTB.Items.Count > 0) MarkupIDTB.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        

        //выход
        /// <summary>
        /// Загрузка главного окна с найденными снимками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadAndBackToMain(object sender, RoutedEventArgs e)
        {
            try
            {
                //получение ID новых изображений
                string NewOriginalID = null;
                string NewMarkupID = null;
                if (OriginalIDTB.SelectedItem != null)
                {
                    if (OriginalIDTB.SelectedItem.ToString().Length > 0) NewOriginalID = OriginalIDTB.SelectedItem.ToString().Split(':')[0];
                }
                if (MarkupIDTB.SelectedItem != null)
                {
                    if (MarkupIDTB.SelectedItem.ToString().Length > 0) NewMarkupID = MarkupIDTB.SelectedItem.ToString().Split(':')[0];
                }

                //при отсутствии новых ID - создать главное окно по умолчанию
                MainWindow MW;
                if (NewOriginalID == "" || NewMarkupID == "" || NewOriginalID == null || NewMarkupID == null)
                {
                    MW = new MainWindow();
                }
                //иначе - с фотографиями с новыми ID
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
            try
            {
                MainWindow MW;
                if (OriginalImage != null && MarkupImage != null) MW = new MainWindow(OriginalImage.GetID(), MarkupImage.GetID());
                else MW = new MainWindow();
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
