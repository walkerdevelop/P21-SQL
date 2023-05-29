using System;

namespace WPF_Автосалон
{
    public class Sells
    {
        public Sells()
        {

        }
        public int ID { get; set; }
        public int BuyerID { get; set; }
        public int SellerID { get; set; }
        public int CarID { get; set; }
        public String Car { get; set; }
        public double Сумма { get; set; }
        public String SellerName { get; set; }
        public String BuyerName { get; set; }
        public DateTime ДатаПокупки { get; set; }
    }
}
