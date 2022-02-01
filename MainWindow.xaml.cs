using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Xml.Linq;

namespace SpringPractice2
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

        private void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            if (startDate.SelectedDate == null || endDate.SelectedDate == null
                || endDate.SelectedDate <= startDate.SelectedDate)
            {
                MessageBox.Show("Даты некорректны!");
                return;
            }
            string stDate = startDate.SelectedDate.Value.ToString("dd/MM/yyyy");
            string enDate = endDate.SelectedDate.Value.ToString("dd/MM/yyyy");

            var item = (ComboBoxItem)currencyBox.SelectedItem;
            string currCode = item.Tag.ToString();
            string currname = item.Content.ToString();
            string query = string.Concat("http://www.cbr.ru/scripts/XML_dynamic.asp?date_req1=", stDate, "&date_req2=", enDate, "&VAL_NM_RQ=", currCode);

            XDocument xDoc = XDocument.Load(query);
            currencyChart.Clear();
            dg.Items.Clear();
            currencyChart.ChartName = currname;
            foreach (var element in xDoc.Element("ValCurs").Elements("Record"))
            {
                string strDate = element.Attribute("Date").Value.ToString();
                double val = double.Parse(element.Element("Value").Value.ToString());
                DateTime dt = DateTime.ParseExact(strDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                currencyChart.AddItem(val, dt);
                dg.Items.Add(new DataModel()
                {
                    Value = val,
                    Date = dt
                });
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            XDocument xDoc = XDocument
                .Load("http://www.cbr.ru/scripts/XML_daily.asp?date_req=02/03/2020");
            foreach (var element in xDoc.Element("ValCurs").Elements("Valute"))
            {
                var id = element.Attribute("ID");
                var name = element.Element("Name");

                currencyBox.Items.Add(new ComboBoxItem
                {
                    Content = name.Value.ToString(),
                    Tag = id.Value.ToString()
                });
            }

            DataGridTextColumn col1 = new DataGridTextColumn()
            {
                Header = "Value",
                Binding = new Binding("Value"),
                Width = 110
            };
            DataGridTextColumn col2 = new DataGridTextColumn()
            {
                Header = "Date",
                Binding = new Binding("Date"),
                Width = 110
            };
            dg.Columns.Add(col1);
            dg.Columns.Add(col2);
        }
    }
}
