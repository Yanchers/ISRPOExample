using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
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

namespace SpringPractice2
{
    /// <summary>
    /// Логика взаимодействия для DataTimeChart.xaml
    /// </summary>
    public partial class DataTimeChart : UserControl
    {
        public ChartValues<DataModel> ChartData { get; set; }
        public string ChartName { get; set; }
        public Func<double, string> Formatter { get; set; }
        public SeriesCollection Series { get; set; }
        public DataTimeChart()
        {
            InitializeComponent();
            ChartData = new ChartValues<DataModel>();
            var dayConfig = Mappers.Xy<DataModel>()
                .X(dm => dm.Date.Ticks)
                .Y(dm => dm.Value);

            Series = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    Title = ChartName,
                    Values = ChartData
                }
            };

            Formatter = value => new DateTime((long)value).ToString("yyyy-MM-dd HH:mm:ss");
            DataContext = this;
        }

        public void AddItem(double value, DateTime time)
        {
            ChartData.Add(new DataModel
            {
                Date = time,
                Value = value
            });
        }
        public void Clear() => ChartData.Clear();
    }
}
