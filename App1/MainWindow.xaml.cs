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

Главная:     вывод доп информаци под фото
                надписи где какое фото


Доп. информация: вывод доп информации в таблицу доделать, удалить картинки

Поиск:      поиск изображений по различной информации

Изменение:  заполнение таблицы информацией,
            сохранение изменений

(?):
База Данных: добавить доп. поля 
Везде:      адаптивное изображение и размеры


Сделано:
Окно добавления 
Окно удаления
*/

namespace PhotoViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //ID изображений
        public string OriginalID, MarkupID;

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
            this.OriginalID = null;
            this.MarkupID = null;

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
            this.OriginalID = Convert.ToString(OriginalID);
            this.MarkupID = Convert.ToString(MarkupID);

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
                //загрузить картики по умлчанию
                if (OriginalID == null || MarkupID == null)
                {
                    PhotoViewerImage.LoadDefaultImagesInMain(TheOriginalWindowImage, TheMarkupWindowImage, this);
                }
                //иначе - вывести картинки с индексами из полей
                else
                {
                    //вывод картинок по путям из БД
                    string SQL = "SELECT Picturepath FROM dbo.Originals WHERE OriginalID=" + OriginalID;
                    PhotoViewerImage.LoadImage(TheOriginalWindowImage, SQL);
                    SQL = "SELECT Picturepath FROM dbo.Markups WHERE (OriginalID=" + OriginalID + ") AND (MarkupID=" + MarkupID + ")";
                    PhotoViewerImage.LoadImage(TheMarkupWindowImage, SQL);

                    //PhotoViewerImage Orig = new PhotoViewerImage(1, connection);
                    //Orig.LoadImage(TheOriginalWindowImage);
                    //PhotoViewerImage Mark = new PhotoViewerImage(1, 1, connection);
                    //Mark.LoadImage(TheMarkupWindowImage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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
                FIW = new FullInfoWindow(Convert.ToInt32(this.OriginalID), Convert.ToInt32(this.MarkupID), this);
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
            SearchPhotos SPW = new SearchPhotos(OriginalID, MarkupID);
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
                this.CW = new ChangeWindow(Convert.ToInt32(OriginalID), "original", this);
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
                this.CW = new ChangeWindow(Convert.ToInt32(MarkupID), "markup", this);
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
                this.DW = new DeleteWindow(Convert.ToInt32(OriginalID), "original", this);
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
                this.DW = new DeleteWindow(Convert.ToInt32(MarkupID), "markup", this);
                this.DW.Show();
            }
            else this.DW.Focus();
        }

        /// <summary>
        /// Сброс до изображений по умолчанию
        /// </summary>
        public void ReloadToDefaultImages()
        {
            PhotoViewerImage.LoadDefaultImagesInMain(TheOriginalWindowImage, TheMarkupWindowImage, this);
        }
    }
}
