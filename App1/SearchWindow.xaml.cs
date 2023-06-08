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
            OriginalIDTB.Text = OriginalID;
            MarkupIDTB.Text = MarkupID;
            this.OriginalID = OriginalID;
            this.MarkupID = MarkupID;
        }

        //доделать

        /// <summary>
        /// Загрузка названий оригиналов при загрузке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OriginalIDTB_Loaded(object sender, RoutedEventArgs e)
        {

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
