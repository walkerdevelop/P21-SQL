using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using System.Security.Policy;
using System.Xml.Linq;
using System.Net;

namespace WPF_Автосалон
{
    /// <summary>
    /// Логика взаимодействия для SellForm.xaml
    /// </summary>
    public partial class SellForm : Window
    {
        private string connectionString = Properties.Settings.Default.sqlString;
        Sells nowSelected = null;
        public SellForm()
        {
            InitializeComponent();
            GetSells();
            GetCars();
            GetSellers();
            GetBuyers();
        }
        public void GetCars()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Товар", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                ObservableCollection<Gender> genders = new ObservableCollection<Gender>();

                foreach (DataRow row in dataTable.Rows)
                {
                    Gender buyer = new Gender();
                    buyer.ID = (int)row["ID"];
                    buyer.Name = (string)row["Brand"] + " " + (string)row["Model"];
                    genders.Add(buyer);
                }
                car.ItemsSource = genders;
                car.DisplayMemberPath = "Name";
                car.SelectedValuePath = "ID";
            }
        }
        public void GetSellers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Работники", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                ObservableCollection<Gender> genders = new ObservableCollection<Gender>();

                foreach (DataRow row in dataTable.Rows)
                {
                    Gender buyer = new Gender();
                    buyer.ID = (int)row["ID"];
                    buyer.Name = (string)row["Surname"] + " " + (string)row["Name"];
                    genders.Add(buyer);
                }
                seller.ItemsSource = genders;
                seller.DisplayMemberPath = "Name";
                seller.SelectedValuePath = "ID";
            }
        }
        public void GetBuyers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM покупатели", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                ObservableCollection<Gender> genders = new ObservableCollection<Gender>();

                foreach (DataRow row in dataTable.Rows)
                {
                    Gender buyer = new Gender();
                    buyer.ID = (int)row["ID"];
                    buyer.Name = (string)row["Surname"] + " " + (string)row["Name"];
                    genders.Add(buyer);
                }
                buyer.ItemsSource = genders;
                buyer.DisplayMemberPath = "Name";
                buyer.SelectedValuePath = "ID";
            }
        }

        private void GetSells()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT Покупатели.ID AS BuyerID, работники.ID AS SellerID, Товар.ID AS CarID, Покупки.soldID, Покупки.ДатаПокупки, CONCAT(Товар.Brand, ' ', Товар.Model) AS Car, Покупки.Сумма, CONCAT(Работники.Name, ' ', Работники.Surname) AS SellerName, CONCAT(покупатели.Name, ' ', покупатели.Surname) AS BuyerName FROM Покупки, покупатели, Товар, Работники WHERE Покупки.ПродавецID = Работники.ID AND Покупки.ПокупательID = покупатели.ID AND Товар.ID = Покупки.АвтоID", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                ObservableCollection<Sells> buyers = new ObservableCollection<Sells>();

                foreach (DataRow row in dataTable.Rows)
                {
                    Sells buyer = new Sells();
                    buyer.ID = (int)row["soldID"];
                    buyer.BuyerID = (int)row["BuyerID"];
                    buyer.SellerID = (int)row["SellerID"];
                    buyer.CarID = (int)row["CarID"];
                    buyer.Car = (string)row["Car"];
                    buyer.Сумма = Convert.ToDouble(row["Сумма"].ToString().Replace(".", ","));
                    buyer.SellerName = (string)row["SellerName"];
                    buyer.BuyerName = (string)row["BuyerName"];
                    buyer.ДатаПокупки = (DateTime)row["ДатаПокупки"];

                    buyers.Add(buyer);
                }
                dataGrid.ItemsSource = buyers;
            }

        }
        private void ClearData()
        {
            nowSelected = null;
            price.Text = "";
            date.Text = "";
            seller.SelectedValue = null;
            buyer.SelectedValue = null;
            car.SelectedValue = null;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Price = price.Text;
            String Date = date.SelectedDate.ToString();

            var selectedCar = car.SelectedValue;
            var selectedBuyer = buyer.SelectedValue;
            var selectedSeller = seller.SelectedValue;
            if (car.SelectedIndex == -1 || buyer.SelectedIndex == -1 || seller.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите значения в списках.");
                return;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = "INSERT INTO Покупки (ДатаПокупки, ПокупательID, ПродавецID, Сумма, АвтоID) VALUES (" +
                "'" + Date + "', '" + selectedBuyer + "', '" + selectedSeller + "', '" + Price + "', '" + selectedCar + "')";
                command.Connection = connection;
                command.ExecuteNonQuery();
                GetSells();
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
                        SqlCommand command = new SqlCommand("DELETE FROM Покупки WHERE soldID = " + nowSelected.ID, connection);
                        command.ExecuteNonQuery();
                        GetSells();
                        ClearData();
                    }
                    break;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                Sells selectedBuyer = dataGrid.SelectedItem as Sells;
                if (selectedBuyer != null)
                {
                    nowSelected = selectedBuyer;
                    price.Text = selectedBuyer.Сумма.ToString();
                    date.Text = selectedBuyer.ДатаПокупки.ToString();
                    car.SelectedValue = selectedBuyer.CarID;
                    seller.SelectedValue = selectedBuyer.SellerID;
                    buyer.SelectedValue = selectedBuyer.BuyerID;
                }
            }
        }
    }
}
