using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Human? h = this.FindResource("human") as Human;
            if (h != null)
            {
                MessageBox.Show(h.Name);
                MessageBox.Show(h.Child.Name);

            }
        }
    }

    [TypeConverter(typeof(NameToHumanTypeConverter))]
    public class Human
    {
        public string Name { get; set; }
        public Human Child { get; set; }
    }

    public class NameToHumanTypeConverter : TypeConverter
    {
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string s)
            {
                Human h = new Human() { Name = s };
                return h;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}