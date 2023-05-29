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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Автосалон
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBuyer_Click(object sender, RoutedEventArgs e)
        {
            BuyerForm buyerForm = new BuyerForm();
            Hide();
            buyerForm.ShowDialog();
            Show();
        }

        private void ButtonWorkers_Click(object sender, RoutedEventArgs e)
        {
            SellerForm workersForm = new SellerForm();
            Hide();
            workersForm.ShowDialog();
            Show();
        }

        private void ButtonPurchase_Click(object sender, RoutedEventArgs e)
        {
            SellForm form = new SellForm();
            Hide();
            form.ShowDialog();
            Show();
        }

        private void ButtonGoods_Click(object sender, RoutedEventArgs e)
        {
            CarsForm workersForm = new CarsForm();
            Hide();
            workersForm.ShowDialog();
            Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReportForm form = new ReportForm();
            Hide();
            form.ShowDialog();
            Show();
        }
    }
}
