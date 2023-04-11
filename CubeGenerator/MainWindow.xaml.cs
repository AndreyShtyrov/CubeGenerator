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

namespace CubeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TableMatrix table;
        private TableMatrix table1;
        public MainWindow()
        {
            InitializeComponent();
            MouseDown += LCanvas.OnMouseDown;
            MouseDown += RCanvas.OnMouseDown;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (table is null)
                table = new TableMatrix(7, CubeType.Cube);
            if (table1 is null)
                table1 = new TableMatrix(7, CubeType.Border);
            LCanvas.ChangeView(table);
            RCanvas.ChangeView(table1);
        }
    }
}
