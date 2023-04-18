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
        public List<string> PreviosFiles;
        public bool IsCheckMirror
        { get; set; }

        public string SavePath
        { get; set; }

        public bool IsProceedGeneration = false;

        public int CubeSize;


        public void CreateDefaultDirectory()
        {
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
        }

        public bool Check(TableMatrix matrix)
        {
            var _filled = matrix.Filled;
            var _mirror_filled = matrix.MirrorFilled;
            foreach (var cube in Cubes)
            {
                cube.ResetRotation();
                for(int i = 0; i < 4; i++)
                {
                    var slots = cube.getSlots();
                    bool notBlocked = true;
                    foreach(var slot in slots)
                    {
                        if (IsCheckMirror)
                        {
                            if (_mirror_filled[slot.X, slot.Y])
                            {
                                notBlocked = false;
                            }
                        }
                        if (_filled[slot.X, slot.Y])
                        {
                           
                            notBlocked = false;
                        }
                    }
                    if (notBlocked)
                    {
                        foreach (var cube1 in Cubes)
                            cube1.ResetRotation();
                        return false;
                    }
                    cube.RotToLeft();
                }
            }
            foreach (var cube1 in Cubes)
                cube1.ResetRotation();
            return true;
        }

        public void FillPasses(int slotsAmount)
        {
            foreach(var border in Borders)
            {
                for (int i = 0; i < border.Size; i++)
                    border.AddSquar(i, 1);
                for (int i = 0; i < border.Size; i++)
                    border.AddSquar(i, border.Size - 2);
                for (int j = 0; j < border.Size; j++)
                    border.AddSquar(1, j);
                for (int j = 0; j < border.Size; j++)
                    border.AddSquar(border.Size - 2, j);
            }
            var mid = CubeSize / 2 + 3;
            Random r = new Random();
            foreach (var border in Borders)
            {
                for (int i = 0; i < slotsAmount; i++)
                {
                    var bordInt = 0;
                    var rInt = 3;
                    while (true)
                    {
                        rInt = r.Next(2, border.Size - 2);
                        bordInt = r.Next(0, 3);
                        if (mid != rInt)
                            break;
                    }
                    switch(bordInt)
                    {
                        case 0:
                            border.RemoveSquar(1, rInt);
                            break;
                        case 1:
                            border.RemoveSquar(rInt, 1);
                            break;
                        case 2:
                            border.RemoveSquar(border.Size - 2, rInt);
                            break;
                        case 3:
                            border.RemoveSquar(rInt, border.Size - 2);
                            break;
                    }
                    if (!Check(border))
                        switch (bordInt)
                        {
                            case 0:
                                border.ChangeSquar(1, rInt);
                                break;
                            case 1:
                                border.ChangeSquar(rInt, 1);
                                break;
                            case 2:
                                border.ChangeSquar(border.Size - 2, rInt);
                                break;
                            case 3:
                                border.ChangeSquar(rInt, border.Size - 2);
                                break;
                        }
                }
            }
        }

        private void UpdateProgramData()
        {
            CreateDefaultDirectory();
            var data = new ProgramSettingSave();
            data.previosPaths = PreviosFiles;
            data.defaultSize = 7;
            var json = JsonSerializer.Serialize(data);
            using (StreamWriter sw = new StreamWriter(DataFile))
                sw.WriteLine(json);
        }

        private void LoadProgramData()
        {
            string json = "";
            using (StreamReader sr = new StreamReader(DataFile))
                json = sr.ReadLine();
            var data = JsonSerializer.Deserialize<ProgramSettingSave>(json);
            PreviosFiles = data.previosPaths;
        }
        public void Save()
        {
            UpdateProgramData();
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

        public List<List<int>> GenerateAllPosiblePositions(int deep, int i, List<int> prev_pos)
        {
            List<List<int>> vs = new();
            if (i < deep)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (!prev_pos.Contains(j))
                    {
                        
                        if (i < deep)
                        {
                            var seq = new List<int>(prev_pos);
                            seq.Add(j);
                            vs.AddRange(GenerateAllPosiblePositions(deep, i + 1, seq));
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < 16; j++)
                {
                    if (!prev_pos.Contains(j))
                    {
                        var seq = new List<int>(prev_pos);
                        seq.Add(j);
                        vs.Add(seq);
                    }
                }
            }
            return vs;
        }

        public void FullGeneratation(int slots)
        {
            var seqs = GenerateAllPosiblePositions(slots, 1, new List<int>());
            int count = 0;
            foreach(var seq in seqs)
            {
                Borders.Add(TableMatrix.GenerateFromSeq(seq, CubeSize, "cube" + count.ToString()));
                count++;
            }
        }
        public DataController()
        {
            appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CubeGenerator");
            DataFile = Path.Combine(appDataFolder, "cubeData.json");
            Cubes = new ObservableCollection<TableMatrix>();
            Borders = new ObservableCollection<TableMatrix>();
            PreviosFiles = new();
            CubeSize = 7;
            SavePath = "C://Users//Andronet//Documents//CubeG//save.json";
            IsCheckMirror = true;
            //LoadProgramData();
        }

        public void CreateNewCube(CubeType cubeType)
        {
            if (cubeType is CubeType.Cube)
                Cubes.Add(new TableMatrix(CubeSize, CubeType.Cube, "Cube " + Cubes.Count));
            else
                Borders.Add(new TableMatrix(CubeSize, CubeType.Border, "Border " + Borders.Count));
        }

        public class ProgramSettingSave
        {
            public List<string> previosPaths
            { get; set; }
            public int defaultSize
            { get; set; }

            public ProgramSettingSave()
            {
                previosPaths = new();
            }
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
