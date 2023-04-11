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
        private DataController controller;
        public MainWindow()
        {
            InitializeComponent();
            MouseDown += LCanvas.OnMouseDown;
            MouseDown += RCanvas.OnMouseDown;
            controller = new DataController();
            Cubes.ItemsSource = controller.Cubes;
            Borders.ItemsSource = controller.Borders;
            Cubes.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>{ reDraw(); };
            Borders.SelectionChanged += (object sender, SelectionChangedEventArgs e) => { reDraw(); };
        }

        private void reDraw()
        {
            if (Cubes.SelectedItem is TableMatrix tcube)
                LCanvas.ChangeView(tcube);
            if (Borders.SelectedItem is TableMatrix tborder)
                RCanvas.ChangeView(tborder);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            reDraw();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            controller.CreateNewCube(CubeType.Cube);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            controller.CreateNewCube(CubeType.Border);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (Cubes.SelectedItem is TableMatrix tcube)
            {
                tcube.RotToLeft();
                reDraw();
            }
        }
    }
}
