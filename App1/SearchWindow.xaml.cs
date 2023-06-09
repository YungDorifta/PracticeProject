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
        //ID снимков с главного окна
        string OriginalID, MarkupID;

        /// <summary>
        /// Конструктор окна поиска снимков
        /// </summary>
        /// <param name="OriginalID"></param>
        /// <param name="MarkupID"></param>
        public SearchPhotos(string OriginalID, string MarkupID)
        {
            InitializeComponent();
            this.OriginalID = OriginalID;
            this.MarkupID = MarkupID;
        }
        
        /// <summary>
        /// Загрузка названий оригиналов при загрузке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OriginalIDTB_Loaded(object sender, RoutedEventArgs e)
        {
            int selected = -1;
            int current = 0;

            string[] names = PhotoViewerImage.FindImageNamesAndIDs("original");
            foreach (string name in names)
            {
                if (name != null)
                {
                    OriginalIDTB.Items.Add(name);
                    if (name.Split(':')[0] == OriginalID) selected = current;
                    current++;
                }
            }
            if (selected != -1)
            {
                OriginalIDTB.SelectedIndex = selected;
            }
            else if (OriginalIDTB.Items.Count > 0) OriginalIDTB.SelectedIndex = 0;
        }

        /// <summary>
        /// Загрузка названий разметок при загрузке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarkupIDTB_Loaded(object sender, RoutedEventArgs e)
        {
            int selected = -1;
            int current = 0;

            string[] names = PhotoViewerImage.FindImageNamesAndIDs(Convert.ToInt32(OriginalID));
            
            foreach (string name in names)
            {
                if (name != null)
                {
                    MarkupIDTB.Items.Add(name);
                    if (name.Split(':')[0] == MarkupID) selected = current;
                    current++;
                }
            }
            if (selected != -1)
            {
                MarkupIDTB.SelectedIndex = selected;
            }
            else if (MarkupIDTB.Items.Count > 0) MarkupIDTB.SelectedIndex = 0;
        }

        /// <summary>
        /// Перезагрузка разметок при выборе другого оригинала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OriginalIDTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //очистить список
            MarkupIDTB.Items.Clear();
            
            if (OriginalIDTB.IsLoaded)
            {
                //извлечь новый ID, если возможно, иначе взять старый
                string NewOriginalID;
                if (OriginalIDTB.Text != null)
                {
                    if (OriginalIDTB.Text != "") NewOriginalID = OriginalIDTB.SelectedItem.ToString().Split(':')[0];
                    else NewOriginalID = OriginalID;
                }
                else NewOriginalID = OriginalID;

                //перезагрузить список разметок
                int selected = -1;
                int current = 0;

                string[] names = PhotoViewerImage.FindImageNamesAndIDs(Convert.ToInt32(NewOriginalID));
                foreach (string name in names)
                {
                    if (name != null)
                    {
                        MarkupIDTB.Items.Add(name);
                        if (name.Split(':')[0] == MarkupID) selected = current;
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

        //(конец того, что нужно доделать)

        /// <summary>
        /// Загрузка главного окна с найденными снимками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadAndBackToMain(object sender, RoutedEventArgs e)
        {
            MainWindow MW;

            OriginalID = OriginalIDTB.Text.Split(':')[0];
            MarkupID = MarkupIDTB.Text.Split(':')[0];
            
            try
            {
                if(OriginalID == null || MarkupID == null)
                {
                    MW = new MainWindow();
                }
                else
                {
                    MW = new MainWindow(Convert.ToInt32(OriginalID), Convert.ToInt32(MarkupID));
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
                MW = new MainWindow(Convert.ToInt32(OriginalID), Convert.ToInt32(MarkupID));

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
