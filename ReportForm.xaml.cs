using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace WPF_Автосалон
{
    /// <summary>
    /// Логика взаимодействия для ReportForm.xaml
    /// </summary>
    public partial class ReportForm : Window
    {
        public ReportForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {// подключение к базе данных
            string connString = Properties.Settings.Default.sqlString;
            SqlConnection conn = new SqlConnection(connString);

            // запрос на получение нужных данных
            string query = "SELECT CONCAT(Brand, ' ', Model) AS myCar, sum(Сумма) as total FROM Покупки, Товар WHERE Покупки.АвтоID = Товар.ID GROUP BY CONCAT(Brand, ' ', Model)";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            // создание круговой диаграммы
            var pieChart = new LiveCharts.Wpf.PieChart();

            // заполнение данных круговой диаграммы
            foreach (DataRow row in dt.Rows)
            {
                pieChart.Series.Add(new LiveCharts.Wpf.PieSeries
                {
                    Title = row["myCar"].ToString(),
                    Values = new LiveCharts.ChartValues<double> { Convert.ToDouble(row["total"]) },
                    DataLabels = true
                });
            }

            // добавление круговой диаграммы на окно
            this.Content = pieChart;
        }
    }
}
