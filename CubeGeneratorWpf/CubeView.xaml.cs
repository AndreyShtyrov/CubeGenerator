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
using CubeGenerator;

namespace CubeGeneratorWpf
{
    /// <summary>
    /// Логика взаимодействия для CubeView.xaml
    /// </summary>
    public partial class CubeView : Page
    {
        public CubeView()
        {
            InitializeComponent();
        }

        public void ChangeView(TableMatrix table)
        {
            Field.Children.Clear();
            var width = Field.ActualWidth;
            var height = Field.ActualHeight;
            var nSupportLines = table.Size - 1;
            var widthStep = width / nSupportLines;
            var heightStep = height / nSupportLines;
            List<Line> lines = new List<Line>();
            for (var i = 0; i < nSupportLines; i++)
            {
                var line = new Line();
                line.X1 = (i + 1) * widthStep;
                line.X2 = (i + 1) * widthStep;
                line.Y1 = 0.0;
                line.Y2 = height;
                line.Stroke = Brushes.Blue;
                line.StrokeThickness = 1;
                lines.Add(line);
            }
            for (var i = 0; i < nSupportLines; i++)
            {
                var line = new Line();
                line.X1 = 0.0;
                line.X2 = width;
                line.Y1 = (i + 1) * heightStep;
                line.Y2 = (i + 1) * heightStep;
                line.Stroke = Brushes.Blue;
                line.StrokeThickness = 1;
                lines.Add(line);
            }
            List<Polygon> polygons = new List<Polygon>();
            for (var i = 0; i < table.Size; i++)
                for (var j = 0; j < table.Size; j++)
                {
                    if (!table.Filled[i, j])
                        continue;
                    var polygon = new Polygon();
                    polygon.Fill = Brushes.Black;
                    PointCollection points = new PointCollection();
                    points.Add(new Point(i * widthStep, j * heightStep));
                    points.Add(new Point((i + 1) * widthStep, j * heightStep));
                    points.Add(new Point((i + 1) * widthStep, (j + 1) * heightStep));
                    points.Add(new Point(i * widthStep, (j + 1) * heightStep));
                    polygons.Add(polygon);
                }
            foreach (var item in lines)
            {
                Field.Children.Add(item);
            }
            foreach (var item in polygons)
            {
                Field.Children.Add(item);
            }
        }
    }
}
