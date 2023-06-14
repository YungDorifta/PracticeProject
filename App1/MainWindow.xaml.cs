using PhotoViewerPRCVI;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

/*
Доделать:
Поиск:      Доп.фильтры + часы    

(?):
Главное окно: поля закрыть (сделать геттеры, сеттеры, функции сброса полей окон в null)
База Данных: добавить доп. поля 
Везде:      адаптивное изображение и размеры


Сделано:
Главное окно
Окно доп. информации
Окно добавления 
Окно изменения
Окно удаления
*/

namespace PhotoViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //изображения (+ID)
        public PhotoViewerImage OriginalImage, MarkupImage;

        //ссылки на функциональные окна
        public FullInfoWindow FIW;
        public ChangeWindow CW;
        public DeleteWindow DW;

        /// <summary>
        /// Конструктор главного окна по умолчанию
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            OriginalImage = null;
            MarkupImage = null;

            FIW = null;
            CW = null;
            DW = null;
        }

        /// <summary>
        /// Конструктор главного окна с указанными избражениями
        /// </summary>
        /// <param name="OriginalID">ID оригинального изображения</param>
        /// <param name="MarkupID">ID размеченного изображения</param>
        public MainWindow(int OriginalID, int MarkupID)
        {
            InitializeComponent();
            OriginalImage = new PhotoViewerImage("original", OriginalID);
            MarkupImage = new PhotoViewerImage("markup", MarkupID);

            FIW = null;
            CW = null;
            DW = null;
        }
        
        /// <summary>
        /// Загрузка главного окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //найти картики по умочанию, если ID не заданы
                if (OriginalImage == null || MarkupImage == null)
                {
                    PhotoViewerImage.ResetToDefaultImagesInMain(this);

                }
                //вывести картинки в окно
                OriginalImage.LoadImage(TheOriginalWindowImage);
                MarkupImage.LoadImage(TheMarkupWindowImage);
                
                //вывести информацию о картинках
                OriginalLabel.Content = "Оригинал: " + OriginalImage.GetName();
                MarkupLabel.Content = "Разметка: " + MarkupImage.GetName();
                OriginalLabel.ToolTip = OriginalImage.GetName();
                MarkupLabel.ToolTip = MarkupImage.GetName();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //открытие функциональных окон

        /// <summary>
        /// Полная информация
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoWindowOpen(object sender, RoutedEventArgs e)
        {
            //закрыть все вспомогательные окна
            if (this.DW != null)
            {
                this.DW.Close();
                this.DW = null;
            }
            if (this.CW != null)
            {
                this.CW.Close();
                this.CW = null;
            }

            //открыть окно полной информации, если его нет, если есть - переключиться на него
            if (this.FIW == null)
            {
                FIW = new FullInfoWindow(OriginalImage.GetID(), MarkupImage.GetID(), this);
                FIW.Show();
            }
            else this.FIW.Focus();
        }
        
        /// <summary>
        /// Поиск изображений (закрывает главное окно)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchPhotosClick(object sender, RoutedEventArgs e)
        {
            //закрыть все вспомогательные окна
            if (this.DW != null)
            {
                this.DW.Close();
                this.DW = null;
            }
            if (this.CW != null)
            {
                this.CW.Close();
                this.CW = null;
            }
            if (this.FIW != null)
            {
                this.FIW.Close();
                this.FIW = null;
            }

            //открыть окно поиска снимков
            SearchPhotos SPW = new SearchPhotos(OriginalImage.GetID().ToString(), MarkupImage.GetID().ToString());
            SPW.Show();
            this.Close();
        }

        /// <summary>
        /// Добавить снимок (закрывает главное окно)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddWindowOpen(object sender, RoutedEventArgs e)
        {
            //закрыть все вспомогательные окна
            if (this.DW != null)
            {
                this.DW.Close();
                this.DW = null;
            }
            if (this.CW != null)
            {
                this.CW.Close();
                this.CW = null;
            }
            if (this.FIW != null)
            {
                this.FIW.Close();
                this.FIW = null;
            }

            //открыть окно добавления фотографий
            AddWindow AW = new AddWindow();
            AW.Show();
            this.Close();
        }
        
        /// <summary>
        /// Изменить информацию об оригинальном фото
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeOrigWindowOpen(object sender, RoutedEventArgs e)
        {
            //закрыть все вспомогательные окна
            if (this.DW != null)
            {
                this.DW.Close();
                this.DW = null;
            }
            if (this.FIW != null)
            {
                this.FIW.Close();
                this.FIW = null;
            }

            //открыть окно изменения, если его еще нет
            if (this.CW == null)
            {
                this.CW = new ChangeWindow(OriginalImage.GetID(), "original", this);
                this.CW.Show();
            }
            else this.CW.Focus();
        }

        /// <summary>
        /// Изменить информацию об оригинальном фото
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeMarkupWindowOpen(object sender, RoutedEventArgs e)
        {
            //закрыть все вспомогательные окна
            if (this.DW != null)
            {
                this.DW.Close();
                this.DW = null;
            }
            if (this.FIW != null)
            {
                this.FIW.Close();
                this.FIW = null;
            }

            //открыть окно изменения, если его еще нет
            if (this.CW == null)
            {
                this.CW = new ChangeWindow(MarkupImage.GetID(), "markup", this);
                this.CW.Show();
            }
            else this.CW.Focus();
        }

        /// <summary>
        /// Удалить оригинал
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteOrigWidowOpen(object sender, RoutedEventArgs e)
        {
            //закрыть все вспомогательные окна
            if (this.CW != null)
            {
                this.CW.Close();
                this.CW = null;
            }
            if (this.FIW != null)
            {
                this.FIW.Close();
                this.FIW = null;
            }

            //открыть окно удаления, если его нет
            if (this.DW == null)
            {
                this.DW = new DeleteWindow(OriginalImage.GetID(), "original", this);
                this.DW.Show();
            }
            else this.DW.Focus();
        }

        /// <summary>
        /// Удалить разметку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMarkupWindowOpen(object sender, RoutedEventArgs e)
        {
            //закрыть все вспомогательные окна
            if (this.CW != null)
            {
                this.CW.Close();
                this.CW = null;
            }
            if (this.FIW != null)
            {
                this.FIW.Close();
                this.FIW = null;
            }

            //открыть окно удаления, если его нет
            if (this.DW == null)
            {
                this.DW = new DeleteWindow(MarkupImage.GetID(), "markup", this);
                this.DW.Show();
            }
            else this.DW.Focus();
        }


        //действия с изображениями

        /// <summary>
        /// Сброс до изображений по умолчанию
        /// </summary>
        public void ReloadToDefaultImages()
        {
            PhotoViewerImage.ResetToDefaultImagesInMain(this);
            OriginalImage.LoadImage(TheOriginalWindowImage);
            MarkupImage.LoadImage(TheMarkupWindowImage);
        }
    }
}
