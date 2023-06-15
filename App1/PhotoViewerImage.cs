﻿using System;
using System.Windows;
using System.Configuration;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media.Imaging;
using PhotoViewer;

namespace PhotoViewerPRCVI
{
    public partial class PhotoViewerImage
    {
        //создание подключения к БД
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        static SqlConnection connection = new SqlConnection(connectionString);
        //названия таблиц в БД
        //static string OriginalsTableName = "dbo.Originals";
        //static string MarkupsTableName = "dbo.Markups"; 
        static string OriginalsTableName = ConfigurationManager.ConnectionStrings["OriginalsTableName"].ToString();
        static string MarkupsTableName = ConfigurationManager.ConnectionStrings["MarkupsTableName"].ToString();

        //данные конкретного изображения
        string Type;
        int ID;
        int OriginalID;
        DateTime Date;
        string Sputnik;
        string Region;
        Uri Path;

        //методы для объекта класса

        /// <summary>
        /// Конструктор объекта снимка
        /// </summary>
        /// <param name="ID">ID оригинального снимка</param>
        /// <param name="connection">Подключение к серверу</param>
        public PhotoViewerImage(string Type, int ID)
        {
            //заполнение полей типа снимка и ID в таблице соответствующего типа
            this.Type = Type;
            this.ID = ID;

            //заполнение полей даты, пути к файлу, региона, спутника
            string SQL;
            SqlCommand command;
            if (Type == "original")
            {
                //формирование команды для извлечения информации об изображении
                SQL = "SELECT * FROM " + OriginalsTableName + " WHERE (OriginalID=" + ID + ")";
                command = new SqlCommand(SQL, connection);

                //адаптер для извлечения информации из БД в Таблицу
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                // установка команды на добавление для вызова хранимой процедуры
                adapter.InsertCommand = new SqlCommand("AllOriginalImage", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime, 50, "Date"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Region", SqlDbType.NChar, 50, "Region"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Sputnik", SqlDbType.NChar, 50, "Sputnik"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 100, "Picturepath"));

                //???
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@OriginalID", SqlDbType.Int, 1, "OriginalID");
                parameter.Direction = ParameterDirection.Output;

                //заполнение адаптером таблицы хранения информации из БД
                DataTable keepTable = new DataTable();
                adapter.Fill(keepTable);

                //заполнение полей информацией из таблицы
                foreach(DataRow row in keepTable.Rows)
                {
                    Date = row.Field<DateTime>("Date");
                    Sputnik = row.Field<string>("Sputnik");
                    Region = row.Field<string>("Region");
                    Path = new Uri(row.Field<string>("Picturepath"));
                }
            }
            else
            {
                //формирование команды для извлечения информации об изображении
                SQL = "SELECT * FROM " +  MarkupsTableName + " WHERE (MarkupID=" + ID + ")";
                command = new SqlCommand(SQL, connection);

                //адаптер для извлечения информации из БД в Таблицу
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                // установка команды на добавление для вызова хранимой процедуры
                adapter.InsertCommand = new SqlCommand("AllOriginalImage", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@OriginalID", SqlDbType.Int, 50, "OriginalID"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime, 50, "Date"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 100, "Picturepath"));

                //???
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@MarkupID", SqlDbType.Int, 1, "MarkupID");
                parameter.Direction = ParameterDirection.Output;

                //заполнение адаптером таблицы хранения информации из БД
                DataTable keepTable = new DataTable();
                adapter.Fill(keepTable);

                //заполнение полей информацией из таблицы
                foreach (DataRow row in keepTable.Rows)
                {
                    OriginalID = row.Field<int>("OriginalID");
                    Date = row.Field<DateTime>("Date");
                    Path = new Uri(row.Field<string>("Picturepath"));
                }
            }
        }
        
        /// <summary>
        /// Получение ID оригинального фото
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return ID;
        }

        /// <summary>
        /// Получение ID размеченного фото
        /// </summary>
        /// <returns></returns>
        public int GetOriginalIDforMarkup()
        {
            return OriginalID;
        }

        /// <summary>
        /// Получение даты создания/разметки снимка
        /// </summary>
        /// <returns></returns>
        public DateTime GetDate()
        {
            return Date;
        }

        /// <summary>
        /// Получение имени изображения
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            string name;
            int size;
            size = this.Path.ToString().Split('/').Length;
            name = this.Path.ToString().Split('/')[size - 1];
            return name;
        }

        /// <summary>
        /// Получить регион съемки
        /// </summary>
        /// <returns></returns>
        public string GetRegion()
        {
            return Region;
        }

        /// <summary>
        /// Получить спутник съемки
        /// </summary>
        /// <returns></returns>
        public string GetSputnik()
        {
            return Sputnik;
        }

        /// <summary>
        /// Загрузка объекта-изображения в окно
        /// </summary>
        /// <param name="image">Элемент-изображение в окне</param>
        public void LoadImage(System.Windows.Controls.Image image)
        {
            try
            {
                //вывод изображения по пути BMP
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = Path;
                myBitmapImage.EndInit();

                //вывод изображения из BMP в окно
                image.Source = myBitmapImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
        //статические методы для действий с БД

        /// <summary>
        /// Добавление оригинального снимка в БД
        /// </summary>
        /// <param name="AddingPath">Путь к изображению</param>
        /// <param name="AddingDate">Дата создания снимка</param>
        /// <param name="AddingSputnik">Спутник, с которого сделан снимок</param>
        /// <param name="AddingRegion">Регион, в котором сделан снимок</param>
        public static void AddOriginalImageInDB(string AddingPath, DateTime AddingDate, string AddingSputnik, string AddingRegion)
        {
            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                //поиск доступного ID для добавления картинки
                string SQL = "SELECT MAX(OriginalID) FROM " + OriginalsTableName;
                SqlCommand command = new SqlCommand(SQL, connection);
                int newID = (int)command.ExecuteScalar();
                newID++;

                //добавление записи о снимке в таблицу БД
                string AddingDateString = AddingDate.ToString("yyyy'-'dd'-'MM'\x020'HH':'mm");
                SQL = "INSERT INTO " + OriginalsTableName  + " (OriginalID, Date, Region, Sputnik, Picturepath) VALUES (" + newID + ", '" +
                      AddingDateString + "', '" + AddingRegion + "', '" + AddingSputnik + "', '" + AddingPath + "');";
                command = new SqlCommand(SQL, connection);
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }

        /// <summary>
        /// Добавление размеченного снимка в БД
        /// </summary>
        /// <param name="AddingPath">Путь к изображению</param>
        /// <param name="AddingDate">Дата разметки снимка</param>
        /// <param name="AddingOriginalID">ID оригинального снимка</param>
        public static void AddMarkupImageInDB(string AddingPath, DateTime AddingDate, int AddingOriginalID)
        {
            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                //поиск доступного ID для добавления картинки
                string SQL = "SELECT MAX(MarkupID) FROM " +  MarkupsTableName + "";
                SqlCommand command = new SqlCommand(SQL, connection);
                int newID = (int)command.ExecuteScalar();
                newID++;

                //добавление записи о снимке в таблицу БД
                string AddingDateString = AddingDate.ToString("yyyy'-'dd'-'MM'\x020'HH':'mm");
                SQL = "INSERT INTO " +  MarkupsTableName + " (MarkupID, OriginalID, Date, Picturepath) VALUES (" + newID + "," +
                    AddingOriginalID + ", '" + AddingDateString + "', '" + AddingPath + "');";
                command = new SqlCommand(SQL, connection);
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }
        
        /// <summary>
        /// Изменение информации об оригинальном снимке
        /// </summary>
        /// <param name="ID">ID снимка</param>
        /// <param name="date">Новая дата создания снимка</param>
        /// <param name="Region">Новый регион съемки</param>
        /// <param name="Sputnik">Новый спутник съемки</param>
        public static void UpdateOrigImageDB(int ID, DateTime date, string Region, string Sputnik)
        {
            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                //изменение записи в таблице
                string SQL = "UPDATE " + OriginalsTableName  + " SET Date = '" + date.ToString("yyyy'-'dd'-'MM'\x020'HH':'mm") + "', Sputnik = '" + Sputnik + "', Region = '" + Region + "' WHERE (OriginalID = " + ID + ")"; //, (Region = '" + Region + "'), (Sputnik = '" + Sputnik + "')
                SqlCommand command = new SqlCommand(SQL, connection);
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }

        /// <summary>
        /// Изменение информации об размеченном снимке
        /// </summary>
        /// <param name="ID">ID снимка</param>
        /// <param name="date">Новая дата создания снимка</param>
        /// <param name="origID">Новый ID оригинала</param>
        public static void UpdateMarkupImageDB(int ID, DateTime date, int origID)
        {
            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                //изменение записи в таблице
                string SQL = "UPDATE " +  MarkupsTableName + " SET Date = '" + date.ToString("yyyy'-'dd'-'MM'\x020'HH':'mm") + "', OriginalID = '" + origID + "' WHERE (MarkupID = " + ID + ")";
                SqlCommand command = new SqlCommand(SQL, connection);
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }
        
        /// <summary>
        /// Удаление информации о снимке из БД
        /// </summary>
        /// <param name="type">Тип снимка</param>
        /// <param name="ID">ID снимка</param>
        public static void DeleteImageInDB(string type, int ID)
        {
            try
            {
                //открытие подключения
                connection.Open();

                string SQL;

                //удаление записи из ДБ
                if (type == "original") SQL = "DELETE FROM " + OriginalsTableName  + " WHERE (OriginalID = " + ID + ")";
                else SQL = "DELETE FROM " +  MarkupsTableName + " WHERE (MarkupID = " + ID + ")";
                SqlCommand command = new SqlCommand(SQL, connection);
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }


        //статические методы для прогрузки информации в окне

        /// <summary>
        /// Сброс до изображений по умолчанию в главном окне
        /// </summary>
        /// <param name="TheOriginalWindowImage">Элемент оригинального изображения в окне</param>
        /// <param name="TheMarkupWindowImage">Элемент размеченного изображения в окне</param>
        public static void ResetToDefaultImagesInMain(MainWindow MW)
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                string SQL;
                SqlCommand command;
                int OriginalID, MarkupID;

                //получение ID оригинального снимка по умолчанию
                SQL = "SELECT Min(OriginalID) FROM " + OriginalsTableName;
                command = new SqlCommand(SQL, connection);
                OriginalID = (int)command.ExecuteScalar();
                MW.OriginalImage = new PhotoViewerImage("original", OriginalID);

                //получение ID размеченного снимка по умолчанию
                SQL = "SELECT Min(MarkupID) FROM " +  MarkupsTableName + " WHERE (OriginalID = " + OriginalID + ")";
                command = new SqlCommand(SQL, connection);
                MarkupID = (int)command.ExecuteScalar();
                MW.MarkupImage = new PhotoViewerImage("markup", MarkupID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }
        
        /// <summary>
        /// Поиск всех названий снимков (оригинальный/размеченный)
        /// </summary>
        /// <param name="type">Тип снимка</param>
        /// <returns></returns>
        public static string[] FindImageNamesAndIDs(string type)
        {
            string[] names = new string[0];

            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                if (type == "markup")
                {
                    //получение размера массива 
                    string SQL = "SELECT Count(MarkupID) FROM " +  MarkupsTableName + "";
                    SqlCommand command = new SqlCommand(SQL, connection);
                    int namesSize = Convert.ToInt32(command.ExecuteScalar());
                    names = new string[namesSize];

                    //получение ID и названия оригиналов из БД 
                    SQL = "SELECT MarkupID, Picturepath FROM " +  MarkupsTableName + "";
                    command = new SqlCommand(SQL, connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.InsertCommand = new SqlCommand("IDandPath", connection);
                    adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                    adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 75, "Picturepath"));
                    SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@MarkupID", SqlDbType.Int, 10, "MarkupID");
                    parameter.Direction = ParameterDirection.Output;

                    //вывод в таблицу хранения
                    DataTable IDPathTable = new DataTable();
                    adapter.Fill(IDPathTable);

                    int currentItem = 0;

                    //вывод ID оригиналов в item
                    foreach (DataRow row in IDPathTable.Rows)
                    {
                        string path = row.Field<string>("Picturepath");
                        string[] pathsplit = path.Split('\\');
                        string name = pathsplit[pathsplit.Length - 1];

                        names[currentItem] = row.Field<int>("MarkupID").ToString() + ": " + name;
                        currentItem++;
                    }
                }
                else
                {
                    //получение размера массива 
                    string SQL = "SELECT Count(OriginalID) FROM " + OriginalsTableName;
                    SqlCommand command = new SqlCommand(SQL, connection);
                    int namesSize = Convert.ToInt32(command.ExecuteScalar());
                    names = new string[namesSize];

                    //получение ID и названия оригиналов из БД 
                    SQL = "SELECT OriginalID, Picturepath FROM " + OriginalsTableName;
                    command = new SqlCommand(SQL, connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.InsertCommand = new SqlCommand("IDandPath", connection);
                    adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                    adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 75, "Picturepath"));
                    SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@OriginalID", SqlDbType.Int, 10, "OriginalID");
                    parameter.Direction = ParameterDirection.Output;

                    //вывод в таблицу хранения
                    DataTable IDPathTable = new DataTable();
                    adapter.Fill(IDPathTable);

                    int currentItem = 0;

                    //вывод ID оригиналов в item
                    foreach (DataRow row in IDPathTable.Rows)
                    {
                        string path = row.Field<string>("Picturepath");
                        string[] pathsplit = path.Split('\\');
                        string name = pathsplit[pathsplit.Length - 1];

                        names[currentItem] = row.Field<int>("OriginalID").ToString() + ": " + name;
                        currentItem++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return names;
        }

        /// <summary>
        /// Поиск размеченных снимков, связанных с выбранным оригиналом
        /// </summary>
        /// <param name="OriginalID"></param>
        /// <returns></returns>
        public static string[] FindImageNamesAndIDs(int OriginalID)
        {
            string[] names = new string[0];

            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                //получение размера массива 
                string SQL;
                SQL = "SELECT Count(MarkupID) FROM " +  MarkupsTableName + "";
                SqlCommand command = new SqlCommand(SQL, connection);
                int namesSize = Convert.ToInt32(command.ExecuteScalar());
                names = new string[namesSize];

                //получение ID и названия оригиналов из БД 
                SQL = "SELECT MarkupID, Picturepath FROM " +  MarkupsTableName + " WHERE (OriginalID = " + OriginalID.ToString() + ")";
                command = new SqlCommand(SQL, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.InsertCommand = new SqlCommand("IDandPath", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 75, "Picturepath"));
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@MarkupID", SqlDbType.Int, 10, "MarkupID");
                parameter.Direction = ParameterDirection.Output;

                //вывод в таблицу хранения
                DataTable IDPathTable = new DataTable();
                adapter.Fill(IDPathTable);

                int currentItem = 0;

                //вывод ID оригиналов в item
                foreach (DataRow row in IDPathTable.Rows)
                {
                    string path = row.Field<string>("Picturepath");
                    string[] pathsplit = path.Split('\\');
                    string name = pathsplit[pathsplit.Length - 1];

                    names[currentItem] = row.Field<int>("MarkupID").ToString() + ": " + name;
                    currentItem++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return names;
        }
        
        /// <summary>
        /// Поиск всех названий размеченных снимков (связь с датой)
        /// </summary>
        /// <param name="OriginalID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string[] FindImageNamesAndIDs(int OriginalID, DateTime date)
        {
            string[] names = new string[0];

            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                //получение размера массива 
                string SQL;
                SQL = "SELECT Count(MarkupID) FROM " +  MarkupsTableName + " WHERE (Date = '" + date.ToString("yyyy'-'dd'-'MM'\x020'HH':'mm") + "')";
                SqlCommand command = new SqlCommand(SQL, connection);
                int namesSize = Convert.ToInt32(command.ExecuteScalar());
                names = new string[namesSize];

                //получение ID и названия оригиналов из БД 
                SQL = "SELECT MarkupID, Picturepath FROM " +  MarkupsTableName + " WHERE (OriginalID = " + OriginalID.ToString() + ") AND (Date = '" + date.ToString("yyyy'-'dd'-'MM'\x020'HH':'mm") + "')";
                command = new SqlCommand(SQL, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.InsertCommand = new SqlCommand("IDandPath", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 75, "Picturepath"));
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@MarkupID", SqlDbType.Int, 10, "MarkupID");
                parameter.Direction = ParameterDirection.Output;

                //вывод в таблицу хранения
                DataTable IDPathTable = new DataTable();
                adapter.Fill(IDPathTable);

                int currentItem = 0;

                //вывод ID оригиналов в item
                foreach (DataRow row in IDPathTable.Rows)
                {
                    string path = row.Field<string>("Picturepath");
                    string[] pathsplit = path.Split('\\');
                    string name = pathsplit[pathsplit.Length - 1];

                    names[currentItem] = row.Field<int>("MarkupID").ToString() + ": " + name;
                    currentItem++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return names;
        }
        
        /// <summary>
        /// Поиск всех названий снимков (оригинальный/размеченный) с привязкой к дате, региону, спутнику
        /// </summary>
        /// <param name="type">Тип снимка</param>
        /// <returns></returns>
        public static string[] FindImageNamesAndIDs(string type, string date, string region, string sputnik)
        {
            //возвращаемый массив строк с ID и именами файлов
            string[] names = new string[0];

            //формирование доп. части строки с фильтрами
            string SQLplus = "";
            if (date != null) if (date != "") SQLplus += " WHERE (Date = '" + date + "')";
            if (region != null)
            {
                if (region != "")
                {
                    if (SQLplus != "") SQLplus += " AND ";
                    else SQLplus += "WHERE ";
                    SQLplus += "(Region = '" + region + "')";
                }
            }
            if (sputnik != null) {
                if (sputnik != "")
                {
                    if (SQLplus != "") SQLplus += " AND ";
                    else SQLplus += "WHERE ";
                    SQLplus += "(Sputnik = '" + sputnik + "')";
                }
            }

            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                //для размеченных снимков
                if (type == "markup")
                {
                    //получение размера массива 
                    string SQL = "SELECT Count(MarkupID) FROM " +  MarkupsTableName + " ";
                    if (date != null) if (date != "") SQL += " WHERE (Date = '" + date + "')";
                    SqlCommand command = new SqlCommand(SQL, connection);
                    int namesSize = Convert.ToInt32(command.ExecuteScalar());
                    names = new string[namesSize];

                    //получение ID и названия оригиналов из БД 
                    SQL = "SELECT MarkupID, Picturepath FROM " +  MarkupsTableName + " ";
                    if (date != null) if (date != "") SQL += " WHERE (Date = '" + date + "')";
                    command = new SqlCommand(SQL, connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.InsertCommand = new SqlCommand("IDandPath", connection);
                    adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                    adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 75, "Picturepath"));
                    SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@MarkupID", SqlDbType.Int, 10, "MarkupID");
                    parameter.Direction = ParameterDirection.Output;

                    //вывод в таблицу хранения
                    DataTable IDPathTable = new DataTable();
                    adapter.Fill(IDPathTable);
                    
                    //вывод ID оригиналов в item
                    int currentItem = 0;
                    foreach (DataRow row in IDPathTable.Rows)
                    {
                        string path = row.Field<string>("Picturepath");
                        string[] pathsplit = path.Split('\\');
                        string name = pathsplit[pathsplit.Length - 1];

                        names[currentItem] = row.Field<int>("MarkupID").ToString() + ": " + name;
                        currentItem++;
                    }
                }
                else
                {
                    //получение размера массива 
                    string SQL = "SELECT Count(OriginalID) FROM " + OriginalsTableName  + " " + SQLplus;
                    SqlCommand command = new SqlCommand(SQL, connection);
                    int namesSize = Convert.ToInt32(command.ExecuteScalar());
                    names = new string[namesSize];

                    //получение ID и названия оригиналов из БД 
                    SQL = "SELECT OriginalID, Picturepath FROM " + OriginalsTableName  + " " + SQLplus;
                    command = new SqlCommand(SQL, connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.InsertCommand = new SqlCommand("IDandPath", connection);
                    adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                    adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 75, "Picturepath"));
                    SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@OriginalID", SqlDbType.Int, 10, "OriginalID");
                    parameter.Direction = ParameterDirection.Output;

                    //вывод в таблицу хранения
                    DataTable IDPathTable = new DataTable();
                    adapter.Fill(IDPathTable);

                    int currentItem = 0;

                    //вывод ID оригиналов в item
                    foreach (DataRow row in IDPathTable.Rows)
                    {
                        string path = row.Field<string>("Picturepath");
                        string[] pathsplit = path.Split('\\');
                        string name = pathsplit[pathsplit.Length - 1];

                        names[currentItem] = row.Field<int>("OriginalID").ToString() + ": " + name;
                        currentItem++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return names;
        }

        /// <summary>
        /// Поиск всех дат создания оригинала
        /// </summary>
        /// <returns></returns>
        public static string[] FindAllOrigDates()
        {
            string[] names = new string[0];

            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                //получение размера массива 
                string SQL = "SELECT Count(DISTINCT Date) FROM " + OriginalsTableName;
                SqlCommand command = new SqlCommand(SQL, connection);
                int namesSize = Convert.ToInt32(command.ExecuteScalar());
                names = new string[namesSize];

                //получение
                SQL = "SELECT DISTINCT Date FROM " + OriginalsTableName;
                command = new SqlCommand(SQL, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.InsertCommand = new SqlCommand("OrigDates", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime, 50, "Date"));
                //SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@MarkupID", SqlDbType.Int, 10, "MarkupID");
                //parameter.Direction = ParameterDirection.Output;

                //вывод в таблицу хранения
                DataTable SputTable = new DataTable();
                adapter.Fill(SputTable);

                int currentItem = 0;

                //вывод в массив
                foreach (DataRow row in SputTable.Rows)
                {
                    names[currentItem] = row.Field<DateTime>("Date").ToString("dd'-'MM'-'yyyy'\x020'HH':'mm");
                    currentItem++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return names;
        }

        /// <summary>
        /// Поиск всех дат создания разметки
        /// </summary>
        /// <returns></returns>
        public static string[] FindAllMarkDates()
        {
            string[] names = new string[0];

            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                //получение размера массива 
                string SQL = "SELECT Count(DISTINCT Date) FROM " +  MarkupsTableName + "";
                SqlCommand command = new SqlCommand(SQL, connection);
                int namesSize = Convert.ToInt32(command.ExecuteScalar());
                names = new string[namesSize];

                //получение
                SQL = "SELECT DISTINCT Date FROM " +  MarkupsTableName + "";
                command = new SqlCommand(SQL, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.InsertCommand = new SqlCommand("MarkDates", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime, 50, "Date"));
                //SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@MarkupID", SqlDbType.Int, 10, "MarkupID");
                //parameter.Direction = ParameterDirection.Output;

                //вывод в таблицу хранения
                DataTable SputTable = new DataTable();
                adapter.Fill(SputTable);

                int currentItem = 0;

                //вывод в массив
                foreach (DataRow row in SputTable.Rows)
                {
                    names[currentItem] = row.Field<DateTime>("Date").ToString("dd'-'MM'-'yyyy'\x020'HH':'mm");
                    currentItem++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return names;
        }

        /// <summary>
        /// Поиск всех названий спутников
        /// </summary>
        /// <returns></returns>
        public static string[] FindAllSputniks()
        {
            string[] names = new string[0];

            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                //получение размера массива 
                string SQL = "SELECT Count(DISTINCT Sputnik) FROM " + OriginalsTableName;
                SqlCommand command = new SqlCommand(SQL, connection);
                int namesSize = Convert.ToInt32(command.ExecuteScalar());
                names = new string[namesSize];

                //получение ID и названия оригиналов из БД 
                SQL = "SELECT DISTINCT Sputnik FROM " + OriginalsTableName;
                command = new SqlCommand(SQL, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.InsertCommand = new SqlCommand("Sputniks", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Sputnik", SqlDbType.NChar, 75, "Sputnik"));
                //SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@MarkupID", SqlDbType.Int, 10, "MarkupID");
                //parameter.Direction = ParameterDirection.Output;

                //вывод в таблицу хранения
                DataTable SputTable = new DataTable();
                adapter.Fill(SputTable);

                int currentItem = 0;

                //вывод ID оригиналов в item
                foreach (DataRow row in SputTable.Rows)
                {
                    names[currentItem] = row.Field<string>("Sputnik");
                    currentItem++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return names;
        }

        /// <summary>
        /// Поиск всех названий регионов
        /// </summary>
        /// <returns></returns>
        public static string[] FindAllRegions()
        {
            string[] names = new string[0];

            try
            {
                //открытие подключения
                if (connection.State == ConnectionState.Closed) connection.Open();

                //получение размера массива 
                string SQL = "SELECT Count(DISTINCT Region) FROM " + OriginalsTableName;
                SqlCommand command = new SqlCommand(SQL, connection);
                int namesSize = Convert.ToInt32(command.ExecuteScalar());
                names = new string[namesSize];

                //получение ID и названия оригиналов из БД 
                SQL = "SELECT DISTINCT Region FROM " + OriginalsTableName;
                command = new SqlCommand(SQL, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.InsertCommand = new SqlCommand("Regions", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Region", SqlDbType.NChar, 75, "Region"));
                //SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@MarkupID", SqlDbType.Int, 10, "MarkupID");
                //parameter.Direction = ParameterDirection.Output;

                //вывод в таблицу хранения
                DataTable RegTable = new DataTable();
                adapter.Fill(RegTable);

                int currentItem = 0;

                //вывод ID оригиналов в item
                foreach (DataRow row in RegTable.Rows)
                {
                    names[currentItem] = row.Field<string>("Region");
                    currentItem++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return names;
        }

        /// <summary>
        /// Вывод подробной информации из таблицы БД в таблицу в окне
        /// </summary>
        /// <param name="table">Элемент таблицы в окне</param>
        /// <param name="SQL">Запрос для извлечения данных</param>
        /// <param name="connection">Подключение к БД</param>
        public static void LoadTableInInfo(DataGrid table, int OriginalID, int MarkupID)
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                PhotoViewerImage OriginalImage = new PhotoViewerImage("original", OriginalID);
                PhotoViewerImage MarkupImage = new PhotoViewerImage("markup", MarkupID);

                //таблица для хранения информации, извлеченной из БД
                DataTable keepTable = new DataTable();

                keepTable.Columns.Add("Parameter");
                keepTable.Columns.Add("Value");

                keepTable.Rows.Add("Название оригинала", OriginalImage.GetName().ToString());
                keepTable.Rows.Add("Название разметки", MarkupImage.GetName().ToString());
                keepTable.Rows.Add("Дата создания оригинала", OriginalImage.GetDate().ToString());
                keepTable.Rows.Add("Дата создания разметки", MarkupImage.GetDate().ToString());
                keepTable.Rows.Add("Регион съемки оригинала", OriginalImage.GetRegion().ToString());
                keepTable.Rows.Add("Спутник, с которого сфотографирован оригинал", OriginalImage.GetSputnik().ToString());

                //заполнение элемента таблицы информацией из таблицы хранения
                table.ItemsSource = keepTable.DefaultView;

                /*
                //полная команда (SQL query + connection query) для извлечения информации
                string SQL = "SELECT " + OriginalsTableName + ".Date, " +  MarkupsTableName + ".Date, Region, Sputnik, Region FROM " + OriginalsTableName  + ", " +  MarkupsTableName + "";
                SqlCommand command = new SqlCommand(SQL, connection);

                //адаптер для извлечения информации из БД в Таблицу
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                // установка команды на добавление для вызова хранимой процедуры
                adapter.InsertCommand = new SqlCommand("DateAndPicturepath", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime, 50, "Date"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 50, "Picturepath"));

                //???
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@OriginalID", SqlDbType.Int, 1, "OriginalID");
                parameter.Direction = ParameterDirection.Output;

                //заполнение адаптером таблицы хранения информации из БД
                adapter.Fill(keepTable);
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }
    }
}
