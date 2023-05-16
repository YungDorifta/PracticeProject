using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace PhotoViewer
{
    /// <summary>
    /// Логика взаимодействия для FullInfoWindow.xaml
    /// </summary>
    public partial class FullInfoWindow : Window
    {

        string connectionString;
        SqlDataAdapter adapter;
        DataTable OriginalsTable;
        Label lab;
        
        public FullInfoWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            this.lab = Lab;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.lab.Content = "BRUH";

            string sql = "SELECT * FROM dbo.Originals";
            OriginalsTable = new DataTable();
            SqlConnection connection = null;


            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);

                // установка команды на добавление для вызова хранимой процедуры
                adapter.InsertCommand = new SqlCommand("sp_InsertPhone", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime, 50, "Date"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Picturepath", SqlDbType.NChar, 50, "Picturepath"));
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@OriginalID", SqlDbType.Int, 1, "OriginalID");
                parameter.Direction = ParameterDirection.Output;

                connection.Open();
                adapter.Fill(OriginalsTable);
                OriginalsGrid.ItemsSource = OriginalsTable.DefaultView;

                if (connection != null)
                {
                    lab.Content = "BRUH";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MW = new MainWindow();

            MW.Show();
            this.Close();
        }
    }
}
