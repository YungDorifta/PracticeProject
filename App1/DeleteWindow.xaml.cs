using PhotoViewer;
using System.Windows;

namespace PhotoViewerPRCVI
{
    /// <summary>
    /// Логика взаимодействия для DeleteWindow.xaml
    /// </summary>
    public partial class DeleteWindow : Window
    {
        //ссылка на главное окно
        MainWindow MW;

        //тип удаляемого изображения
        string type;

        //ID удаляемого изображения
        int ID;
        
        /// <summary>
        /// Конструктор окна удаления информации
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        public DeleteWindow(int ID, string type, MainWindow MW)
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
            this.MW = MW;
            this.ID = ID;
        }
        
        /// <summary>
        /// Удалить и вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteAndBackToMain(object sender, RoutedEventArgs e)
        {
            PhotoViewerImage.DeleteImageInDB(type, ID);

            //проверка загрузки главного окна, восстановление если окно закрыто
            if (this.MW.IsLoaded)
            {
                MW.ReloadToDefaultImages();
                MW.Focus();
            }
            else
            {
                this.MW = new MainWindow();
                this.MW.Show();
            }
            this.Close();

            //сброс информации об открытом окне удаления
            this.MW.DW = null;
        }

        /// <summary>
        /// Вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToMain(object sender, RoutedEventArgs e)
        {
            //проверка загрузки главного окна, восстановление если окно закрыто
            if (this.MW.IsLoaded)
            {
                MW.ReloadToDefaultImages();
                MW.Focus();
            }
            else
            {
                this.MW = new MainWindow();
                this.MW.Show();
            }
            this.Close();

            //сброс информации об открытом окне удаления
            this.MW.DW = null;
        }
    }
}
