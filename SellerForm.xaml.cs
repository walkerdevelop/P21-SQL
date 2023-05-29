using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
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
using System.Net;
using System.Diagnostics;

namespace WPF_Автосалон
{
    /// <summary>
    /// Логика взаимодействия для SellerForm.xaml
    /// </summary>
    public partial class SellerForm : Window
    {
        private string connectionString = Properties.Settings.Default.sqlString;
        Seller nowSelected = null;
        public SellerForm()
        {
            InitializeComponent();
            GetSellers();
            GetGenders();
        }
        private void GetSellers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM работники", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                ObservableCollection<Seller> buyers = new ObservableCollection<Seller>();

                foreach (DataRow row in dataTable.Rows)
                {
                    Seller buyer = new Seller();
                    buyer.ID = (int)row["ID"];
                    buyer.Surname = (string)row["Surname"];
                    buyer.Name = (string)row["Name"];
                    buyer.Patronymic = (string)row["Patronymic"];
                    buyer.PhoneNumber = (string)row["PhoneNumder"];
                    buyer.DateOfBirth = (DateTime)row["DateOfBirth"];
                    buyer.JobTitle = (string)row["JobTitle"];
                    buyer.Gender = (int)row["WorkerGender"];

                    buyers.Add(buyer);
                }
                dataGrid.ItemsSource = buyers;
            }
        }
        public void GetGenders()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM genderList", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                ObservableCollection<Gender> genders = new ObservableCollection<Gender>();

                foreach (DataRow row in dataTable.Rows)
                {
                    Gender buyer = new Gender();
                    buyer.ID = (int)row["ID"];
                    buyer.Name = (string)row["GenderName"];
                    genders.Add(buyer);
                }
                DataGridComboBoxColumn comboBoxColumn = (DataGridComboBoxColumn)dataGrid.Columns[7];
                comboBoxColumn.ItemsSource = genders;
                comboBoxColumn.DisplayMemberPath = "Name";
                comboBoxColumn.SelectedValuePath = "GenderID";

                DataGridColumn existingColumn = dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Gender");
                if (existingColumn != null)
                {
                    int columnIndex = dataGrid.Columns.IndexOf(existingColumn);
                    dataGrid.Columns[columnIndex] = comboBoxColumn;
                }
                Gender.ItemsSource = genders;
                Gender.DisplayMemberPath = "Name";
                Gender.SelectedValuePath = "ID";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String sname = surname.Text;
            string Name = name.Text;
            String patr = patronymic.Text;
            String phone = Phone.Text;
            String birthday = Birthday.SelectedDate.ToString();
            String address = Rank.Text;

            var selectedValue = Gender.SelectedValue;
            if (selectedValue != null && int.TryParse(selectedValue.ToString(), out int selectedID))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.CommandText = "INSERT INTO Работники (Name, Surname, Patronymic, PhoneNumder, DateOfBirth, JobTitle, WorkerGender) VALUES (" +
                    "'" + Name + "', '" + sname + "', '" + patr + "', '" + phone + "', '" + birthday + "', '" + address + "', '" + selectedValue + "')";
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                    GetSellers();
                    ClearData();
                }
            }
        }
        private void ClearData()
        {
            nowSelected = null;
            surname.Text = "";
            name.Text = "";
            patronymic.Text = "";
            Phone.Text = "";
            Birthday.Text = "";
            Rank.Text = "";
            Gender.SelectedValue = null;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(nowSelected == null)
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
                        SqlCommand command = new SqlCommand("DELETE FROM Работники WHERE ID = " + nowSelected.ID, connection);
                        command.ExecuteNonQuery();
                        GetSellers();
                        ClearData();
                    }
                    break;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                Seller selectedBuyer = dataGrid.SelectedItem as Seller;
                if (selectedBuyer != null)
                {
                    nowSelected = selectedBuyer;
                    surname.Text = selectedBuyer.Surname;
                    name.Text = selectedBuyer.Name;
                    patronymic.Text = selectedBuyer.Patronymic;
                    Phone.Text = selectedBuyer.PhoneNumber;
                    Birthday.Text = selectedBuyer.DateOfBirth.ToString();
                    Rank.Text = selectedBuyer.JobTitle;
                    Gender.SelectedValue = selectedBuyer.Gender;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (nowSelected == null)
            {
                MessageBox.Show("Для начала загрузите строку из таблицы, которую хотите изменить.\n\nЕсли Вы ввели новые значения, и хотите их добавить в таблицу, нажмите 'Добавить'");
                return;
            }
            String sname = surname.Text;
            string Name = name.Text;
            String patr = patronymic.Text;
            String phone = Phone.Text;
            String birthday = Birthday.SelectedDate.ToString();
            String address = Rank.Text;

            var selectedValue = Gender.SelectedValue;
            if (selectedValue != null && int.TryParse(selectedValue.ToString(), out int selectedID))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.CommandText = "UPDATE Работники SET Name = '" + Name + "', Surname = '" + sname + "', Patronymic = '" + patr + "', PhoneNumder = '" + phone + "', DateOfBirth = '" + birthday + "', " +
                        "JobTitle = '" + address + "', WorkerGender = '" + selectedValue + "' WHERE ID = " + nowSelected.ID;
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                    GetSellers();
                    ClearData();
                }
            }
        }
    }
}
