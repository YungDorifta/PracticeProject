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
        public SearchPhotos(string Orig, string Mark)
        {
            InitializeComponent();
            OriginalID.Text = Orig;
            MarkupID.Text = Mark;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MW;

            try
            {
                MW = new MainWindow(OriginalID.Text, MarkupID.Text);
                if(MW == null)
                {
                    MW = new MainWindow("1", "1");
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
