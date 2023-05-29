using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Linq;

namespace WPF_Автосалон
{
    /// <summary>
    /// Логика взаимодействия для BuyerForm.xaml
    /// </summary>
    public partial class BuyerForm : Window
    {
        private string connectionString = Properties.Settings.Default.sqlString;
        Buyer nowSelected = null;
        public BuyerForm()
        {
            InitializeComponent();
            GetBuyers();
            GetGenders();
        }
        public void GetBuyers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM покупатели", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                ObservableCollection<Buyer> buyers = new ObservableCollection<Buyer>();

                foreach (DataRow row in dataTable.Rows)
                {
                    Buyer buyer = new Buyer();
                    buyer.ID = (int)row["ID"];
                    buyer.Surname = (string)row["Surname"];
                    buyer.Name = (string)row["Name"];
                    buyer.Patronymic = (string)row["Patronymic"];
                    buyer.PhoneNumber = (string)row["PhoneNumber"];
                    buyer.DateOfBirth = (DateTime)row["DateOfBirth"];
                    buyer.Address = (string)row["ResidentialAddress"];
                    buyer.Acquisition = (DateTime)row["Acquisition"];
                    buyer.Gender = (int)row["Gender"];

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
                DataGridComboBoxColumn comboBoxColumn = (DataGridComboBoxColumn)dataGrid.Columns[8];
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(nowSelected == null)
            {
                MessageBox.Show("Для начала загрузите строку из таблицы, которую хотите изменить.\n\nЕсли Вы ввели новые значения, и хотите их добавить в таблицу, нажмите 'Добавить'");
                return;
            }
            String sname = surname.Text;
            string Name = name.Text;
            String patr = patronymic.Text;
            String phone = Phone.Text;
            String birthday = Birthday.SelectedDate.ToString();
            String address = Address.Text;
            String datareg = DateReg.SelectedDate.ToString();

            var selectedValue = Gender.SelectedValue;
            if (selectedValue != null && int.TryParse(selectedValue.ToString(), out int selectedID))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.CommandText = "UPDATE покупатели SET Name = '" + Name + "', Surname = '" + sname + "', Patronymic = '" + patr + "', PhoneNumber = '" + phone + "', DateOfBirth = '" + birthday + "', " +
                        "ResidentialAddress = '" + address + "', Acquisition = '" + datareg + "', Gender = '" + selectedValue + "' WHERE ID = " + nowSelected.ID;
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                    GetBuyers();
                    ClearData();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String sname = surname.Text;
            string Name = name.Text;
            String patr = patronymic.Text;
            String phone = Phone.Text;
            String birthday = Birthday.SelectedDate.ToString();
            String address = Address.Text;
            String datareg = DateReg.SelectedDate.ToString();

            var selectedValue = Gender.SelectedValue;
            if (selectedValue != null && int.TryParse(selectedValue.ToString(), out int selectedID))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.CommandText = "INSERT INTO покупатели (Name, Surname, Patronymic, PhoneNumber, DateOfBirth, ResidentialAddress, Acquisition, Gender) VALUES (" +
                    "'" + Name + "', '" + sname + "', '" + patr + "', '" + phone + "', '" + birthday + "', '" + address + "', '" + datareg + "', '" + selectedValue + "')"; 
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                    GetBuyers();
                    ClearData();
                }
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                Buyer selectedBuyer = dataGrid.SelectedItem as Buyer;
                if (selectedBuyer != null)
                {
                    nowSelected = selectedBuyer;
                    surname.Text = selectedBuyer.Surname;
                    name.Text = selectedBuyer.Name;
                    patronymic.Text = selectedBuyer.Patronymic;
                    Phone.Text = selectedBuyer.PhoneNumber;
                    Birthday.Text = selectedBuyer.DateOfBirth.ToString();
                    Address.Text = selectedBuyer.Address;
                    DateReg.Text = selectedBuyer.Acquisition.ToString();
                    Gender.SelectedValue = selectedBuyer.Gender;
                }
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
                        SqlCommand command = new SqlCommand("DELETE FROM покупатели WHERE ID = " + nowSelected.ID, connection);
                        command.ExecuteNonQuery();
                        GetBuyers();
                        ClearData();
                    }
                    break;
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
            Address.Text = "";
            DateReg.Text = "";
            Gender.SelectedValue = null;
        }
    }
}