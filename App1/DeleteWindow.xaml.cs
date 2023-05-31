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
    /// Логика взаимодействия для DeleteWindow.xaml
    /// </summary>
    public partial class DeleteWindow : Window
    {
        string type;
        MainWindow mw;

        /// <summary>
        /// Конструктор окна удаления информации
        /// </summary>
        public DeleteWindow(MainWindow mw)
        {
            InitializeComponent();
            this.type = "markup";
            this.mw = mw;
            DeleteWarnLabel.Content = "Удалить информацию о текущем размеченном снимке?";
        }
        
        /// <summary>
        /// Конструктор окна удаления информации
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        public DeleteWindow(int ID, string type, MainWindow mw)
        {
            InitializeComponent();

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
                //удалить оригинальный и все размеченные
                //перейти на другой в главнм окне
                mw.Activate();
                this.Close();
            }
            else
            {
                //удалить размеченный
                //перейти на другой в главном
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
