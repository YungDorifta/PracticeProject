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
    /// Логика взаимодействия для ChangeWindow.xaml
    /// </summary>
    public partial class ChangeWindow : Window
    {
        //тип изменяемого фото
        string type;

        //ссылка на главное окно
        MainWindow MW;

        //изменяемое изображение
        PhotoViewerImage UpdatingImage;
        
        /// <summary>
        /// Конструктор окна для изменения информации о снимке
        /// </summary>
        public ChangeWindow(int ID, string type, MainWindow MW)
        {
            InitializeComponent();
            this.MW = MW;

            if (type == "markup")
            {
                this.type = type;
                this.Title = "Изменить информацию о размеченном снимке";
                UpdatingImage = new PhotoViewerImage("markup", ID);

                OriginalsBox.Visibility = Visibility.Visible;
                RegionBox.Visibility = Visibility.Hidden;
                RegionLabel.Visibility = Visibility.Hidden;
                SputnikBox.Visibility = Visibility.Hidden;
                OrigSputLabel.Content = "Оригинал:";

                this.MinWidth = 385;
                this.MaxWidth = 385;
                this.Width = 385;
            }
            else
            {
                this.type = "original";
                this.Title = "Изменить информацию об оригинальном снимке";
                UpdatingImage = new PhotoViewerImage("original", ID);

                OriginalsBox.Visibility = Visibility.Hidden;
                RegionBox.Visibility = Visibility.Visible;
                RegionLabel.Visibility = Visibility.Visible;
                SputnikBox.Visibility = Visibility.Visible;
                OrigSputLabel.Content = "Спутник:";

                this.MinWidth = 515;
                this.MaxWidth = 515;
                this.Width = 515;
            }
        }

        /// <summary>
        /// Загрузка информации при загрузке окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ImageNameLabel.Content = "Изменяемый снимок: " + UpdatingImage.GetName();
            ImageNameLabel.ToolTip = UpdatingImage.GetName();
            DateBox.DisplayDate = UpdatingImage.GetDate();
            DateBox.Text = DateBox.DisplayDate.ToString();
            if (type == "original")
            {
                SputnikBox.Text = UpdatingImage.GetSputnik().Split()[0];
                RegionBox.Text = UpdatingImage.GetRegion().Split()[0];
            }
            else
            {
                int selected = -1;
                int current = 0;
                string[] origNames = PhotoViewerImage.FindImageNamesAndIDs("original");
                foreach (string name in origNames)
                {
                    OriginalsBox.Items.Add(name);
                    if (name != null)
                    {
                        if (name.Split(':')[0] == UpdatingImage.GetOriginalIDforMarkup().ToString()) selected = current; 
                    }
                    current++;
                }
                if (selected != -1) OriginalsBox.SelectedIndex = selected;
                else if (OriginalsBox.Items.Count > 0) OriginalsBox.SelectedIndex = 0;
            }
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
                if (i == UpdatingImage.GetDate().Hour) hoursBox.SelectedIndex = i;
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
                if (i == UpdatingImage.GetDate().Minute) minutesBox.SelectedIndex = i;
            }
        }

        /// <summary>
        /// Сохранить изменения и вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAndBackToMain(object sender, RoutedEventArgs e)
        {
            //извлечение новой даты
            DateTime NewDate = Convert.ToDateTime(DateBox.SelectedDate);
            if (hoursBox.Text != "") NewDate = NewDate.AddHours(Convert.ToDouble(hoursBox.Text));
            if (minutesBox.Text != "") NewDate = NewDate.AddMinutes(Convert.ToDouble(minutesBox.Text));

            //зависимость сохранения от типа снимка
            if (type == "original")
            {
                //извлечение информации о спутнике
                string NewSputnik;
                if (SputnikBox.Text == "")
                {
                    NewSputnik = UpdatingImage.GetSputnik();
                }
                else
                {
                    NewSputnik = SputnikBox.Text;
                }

                //извлечение информации о регионе
                string NewRegion;
                if (RegionBox.Text == "")
                {
                    NewRegion = UpdatingImage.GetRegion();
                }
                else
                {
                    NewRegion = RegionBox.Text;
                }

                //сохранение изменений
                PhotoViewerImage.UpdateOrigImageDB(UpdatingImage.GetID(), NewDate, NewRegion, NewSputnik);
            }
            else
            {
                PhotoViewerImage.UpdateMarkupImageDB(UpdatingImage.GetID(), NewDate, Convert.ToInt32(OriginalsBox.Text.Split(':')[0]));
            }

            //проверка загрузки главного окна, восстановление если окно закрыто
            if (this.MW.IsLoaded) MW.Focus();
            else
            {
                this.MW = new MainWindow();
                this.MW.Show();
            }
            //сброс информации об открытом окне изменения
            this.MW.CW = null;
            this.Close();
        }

        /// <summary>
        /// Вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToMain(object sender, RoutedEventArgs e)
        {
            //проверка загрузки главного окна, восстановление если окно закрыто
            if (this.MW.IsLoaded) MW.Focus();
            else
            {
                this.MW = new MainWindow();
                this.MW.Show();
            }
            this.Close();

            //сброс информации об открытом окне изменения
            this.MW.CW = null;
        }

        protected override void OnClosed(EventArgs e)
        {
            this.MW.CW = null;
            base.OnClosed(e);
        }
    }
}
