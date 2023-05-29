using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Security.Policy;
using System.Xml.Linq;

namespace WPF_Автосалон
{
    /// <summary>
    /// Логика взаимодействия для CarsForm.xaml
    /// </summary>
    public partial class CarsForm : Window
    {
        private string connectionString = Properties.Settings.Default.sqlString;
        Car nowSelected = null;
        public CarsForm()
        {
            InitializeComponent();
            GetCars();
        }
        private void GetCars()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Товар", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                ObservableCollection<Car> buyers = new ObservableCollection<Car>();

                foreach (DataRow row in dataTable.Rows)
                {
                    Car buyer = new Car();
                    buyer.ID = (int)row["ID"];
                    buyer.Brand = (string)row["Brand"];
                    buyer.Model = (string)row["Model"];
                    buyer.Color = (string)row["Color"];
                    buyer.Price = Convert.ToDouble(row["Price"].ToString().Replace(".", ","));

                    buyers.Add(buyer);
                }
                dataGrid.ItemsSource = buyers;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                Car selectedBuyer = dataGrid.SelectedItem as Car;
                if (selectedBuyer != null)
                {
                    nowSelected = selectedBuyer;
                    brand.Text = selectedBuyer.Brand;
                    model.Text = selectedBuyer.Model;
                    color.Text = selectedBuyer.Color;
                    price.Text = selectedBuyer.Price.ToString();
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            String brand1 = brand.Text;
            String model1 = model.Text;
            String color1 = color.Text;
            String price1 = price.Text;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = "UPDATE Товар SET Brand = '" + brand1 + "', Model = '" + model1 + "', Color = '" + color1 + "', Price = '" + price1 + "' WHERE ID = " + nowSelected.ID;
                command.Connection = connection;
                command.ExecuteNonQuery();
                GetCars();
                ClearData();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (nowSelected == null)
            {
                MessageBox.Show("Загрузите строку для удаления.");
                return;
            }
            MessageBoxResult rsltMessageBox = MessageBox.Show("Вы действительно хотите удалить эту строку?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("DELETE FROM Товар WHERE ID = " + nowSelected.ID, connection);
                        command.ExecuteNonQuery();
                        GetCars();
                        ClearData();
                    }
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String brand1 = brand.Text;
            string model1 = model.Text;
            String color1 = color.Text;
            String price1 = price.Text;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = "INSERT INTO Товар (Brand, Model, Color, Price) VALUES ('" + brand1 + "', '" + model1 + "', '" + color1 + "', '" + price1 + "')";
                command.Connection = connection;
                command.ExecuteNonQuery();
                GetCars();
                ClearData();
            }
        }
        private void ClearData()
        {
            nowSelected = null;
            brand.Text = "";
            model.Text = "";
            color.Text = "";
            price.Text = "";
        }
    }
}
