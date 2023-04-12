using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CubeGenerator
{
    public class DataController
    {
        public readonly string appDataFolder;
        public readonly string DataFile;
        public ObservableCollection<TableMatrix> Cubes;
        public ObservableCollection<TableMatrix> Borders;

        public string SavePath
        { get; set; }

        public bool IsProceedGeneration = false;

        public int CubeSize;

        public void Save()
        {
            var data = new DataContainerSave();
            data.Size = CubeSize;
            foreach (var cube in Cubes)
            {
                data.CubesPoints.Add(cube.ToListTablePoints());
                data.CubesNames.Add(cube.Name);
            }
            foreach (var border in Borders)
            {
                data.BordesPoints.Add(border.ToListTablePoints());
                data.BorderNames.Add(border.Name);
            }
            string json = JsonSerializer.Serialize(data);
            using (StreamWriter sw = new StreamWriter(SavePath))
                sw.WriteLine(json);
        }

        public void Load()
        {
            string json = "";
            using (StreamReader sr = new StreamReader(SavePath))
                json = sr.ReadLine();
            var data = JsonSerializer.Deserialize<DataContainerSave>(json);
            for (int i = 0; i < data.CubesPoints.Count; i++)
            {
                var cube = new TableMatrix(data.Size, CubeType.Cube, data.CubesNames[i]);
                cube.ApplyFromListPoints(data.CubesPoints[i]);
                Cubes.Add(cube);
            }
            for (int i = 0; i < data.BordesPoints.Count; i++)
            {
                var border = new TableMatrix(data.Size, CubeType.Border, data.BorderNames[i]);
                border.ApplyFromListPoints(data.BordesPoints[i]);
                Borders.Add(border);
            }
        }


        public DataController()
        {
            appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CubeGenerator");
            DataFile = Path.Combine(appDataFolder, "cubeData.json");
            Cubes = new ObservableCollection<TableMatrix>();
            Borders = new ObservableCollection<TableMatrix>();
            CubeSize = 7;
            SavePath = "C://Users//WorkPlace//Documents//CubeG//save.json";
        }

        public void CreateNewCube(CubeType cubeType)
        {
            if (cubeType is CubeType.Cube)
                Cubes.Add(new TableMatrix(CubeSize, CubeType.Cube, "Cube " + Cubes.Count));
            else
                Borders.Add(new TableMatrix(CubeSize, CubeType.Border, "Border " + Borders.Count));
        }


        public class DataContainerSave
        {

            public DataContainerSave()
            {
                CubesPoints = new();
                BordesPoints = new();
                CubesNames = new();
                BorderNames = new();
                SavePath = "";
                Size = 0;
            }
            public int Size
            { get; set; }
            public List<List<TablePoint>> CubesPoints
            { get; set; }
            public List<String> CubesNames
            { get; set; }
            public List<List<TablePoint>> BordesPoints
            { get; set; }
            public List<String> BorderNames
            { get; set; }
            public string SavePath
            { get; set; }
        }
    }
}
