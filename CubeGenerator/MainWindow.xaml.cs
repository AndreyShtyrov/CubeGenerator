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
        private int cycleI = 0;
        System.Windows.Threading.DispatcherTimer timmer;
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
            if (controller.IsProceedGeneration)
                return;
            controller.CreateNewCube(CubeType.Cube);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (controller.IsProceedGeneration)
                return;
            controller.CreateNewCube(CubeType.Border);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (controller.IsProceedGeneration)
                return;
            if (Cubes.SelectedItem is TableMatrix tcube)
            {
                tcube.RotToLeft();
                reDraw();
            }
        }

        private void OneGenerationStep()
        {
            if (Cubes.SelectedItem is TableMatrix tcube)
            {
                if (!(Borders.SelectedItem is TableMatrix))
                {
                    controller.CreateNewCube(CubeType.Border);
                    Borders.SelectedItem = controller.Borders[controller.Borders.Count - 1];
                }
                if (Borders.SelectedItem is TableMatrix tborder)
                {
                    List<(int X, int Y)> points = new();
                    var filled = tcube.Filled;
                    for (int i = 0; i < tcube.Size; i++)
                        for (int j = 0; j < tcube.Size; j++)
                        {
                            if (i == 1 || i == tcube.Size - 2 || j == 1 || j == tcube.Size - 2)
                            {
                                if (filled[i, j])
                                    points.Add((i, j));
                            }
                        }
                    foreach (var point in points)
                    {
                        tborder.AddSquar(point.X, point.Y);
                    }
                }
                cycleI += 1;
                reDraw();
            }
            else
            {
                timmer.Stop();
                controller.IsProceedGeneration = false;
                MouseDown += LCanvas.OnMouseDown;
                MouseDown += RCanvas.OnMouseDown;
                GenerationStatus.Content = "Generation Ended";
                return;
            }
        }
        private void GenerateShape()
        {
            timmer.Stop();
            if (cycleI > 3)
            {
                timmer.Stop();
                controller.IsProceedGeneration = false;
                MouseDown += LCanvas.OnMouseDown;
                MouseDown += RCanvas.OnMouseDown;
                GenerationStatus.Content = "Generation Ended";
                return;
            }
            if (Cubes.SelectedItem is TableMatrix tcube)
            {
                tcube.RotToLeft();
                OneGenerationStep();
            }
            else
            {
                timmer.Stop();
                controller.IsProceedGeneration = false;
                MouseDown += LCanvas.OnMouseDown;
                MouseDown += RCanvas.OnMouseDown;
                GenerationStatus.Content = "Generation Ended";
                return;
            }
            if (controller.IsProceedGeneration)
                timmer.Start();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            GenerationStatus.Content = "GenerationProceed";
            controller.IsProceedGeneration = true;
            MouseDown -= LCanvas.OnMouseDown;
            MouseDown -= RCanvas.OnMouseDown;
            Cubes.SelectedItem = controller.Cubes[0];
            OneGenerationStep();
            reDraw();
            cycleI = 0;
            timmer = new System.Windows.Threading.DispatcherTimer();
            timmer.Tick += (o, e) => {
                GenerateShape();
            };
            timmer.Interval = new TimeSpan(0, 0, 0, 5);
            timmer.Start();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            controller.Save();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            controller.Load();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            controller.CreateNewCube(CubeType.Cube);
            controller.CreateNewCube(CubeType.Cube);
            controller.CreateNewCube(CubeType.Cube);
            controller.CreateNewCube(CubeType.Border);
            controller.CreateNewCube(CubeType.Border);
            controller.CreateNewCube(CubeType.Border);
            controller.CreateNewCube(CubeType.Border);
            controller.CreateNewCube(CubeType.Border);
            controller.CreateNewCube(CubeType.Border);
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            var nSlots = int.Parse(SlotCount.Text);
            if (controller.Borders.Count == 0 || controller.Cubes.Count == 0)
                return;
            controller.FillPasses(nSlots);
            reDraw();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            if (controller.Borders.Count == 0 || controller.Cubes.Count == 0)
                return;
            if (Borders.SelectedItem is TableMatrix tborder)
            {
                var res = controller.Check(tborder);
                GenerationStatus.Content = res.ToString();
            }
        }
    }
}
