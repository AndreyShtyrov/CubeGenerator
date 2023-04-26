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
    public class  DataController
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
                    if (slots.Count == 0)
                        continue;
                    bool notBlocked = true;
                    if (IsCheckMirror)
                    {
                        foreach (var slot in slots)
                        {
                            if (_mirror_filled[slot.X, slot.Y])
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
                    }
                    notBlocked = true;
                    foreach(var slot in slots)
                    {
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
                    cube.RotToRight();
                }
            }
            foreach (var cube1 in Cubes)
                cube1.ResetRotation();
            return true;
        }

        private List<List<int>> shiftSeq(List<int> seq)
        {
            List<List<int>> vs = new();
            for(int i = 1; i < 4; i++)
            {
                var _seq = new List<int>();
                for (int j = 0; j < seq.Count; j++)
                {
                    if (seq[j] + ( 4 * i) < 16)
                    {
                        _seq.Add(seq[j] + (4 * i));
                    }
                    else
                    {
                        _seq.Add(seq[j] + (4 * i) - 16);
                    }
                }
                vs.Add(_seq);
            }
            return vs;
        }
        public void RandomGeneration(int slotsAmount, int nstructs)
        {
            var seqs = new List<List<int>>();
            int maxTriesSlots = 500;
            int i_struct = 0;
            Int64 maxTriesStructs = nstructs * 2000;
            Random rnd = new Random();
            if (slotsAmount > 12)
                return;
            int borderI = 0;
            if (Borders.Count != 0)
            {
                var lastIndex = Borders[Borders.Count - 1].Name.Split(' ');
                int.TryParse(lastIndex[lastIndex.Length - 1], out borderI);
            }
            Int64 i = 0;
            while (true)
            {
                if (i > maxTriesStructs)
                    break;
                if (i_struct == nstructs)
                    break;
                i++;
                var seq = new List<int>();
                for (int j = 0; j < slotsAmount; j++)
                {
                    int nstep = 0;
                    while (true)
                    {
                        var pos = rnd.Next(0, 15);
                        if (!seq.Contains(pos))
                        {
                            var quart = pos / 4;
                            int face_slots = 0;
                            if (seq.Contains(quart * 4))
                               face_slots++;
                            if (seq.Contains(quart * 4 + 1))
                                face_slots++;
                            if (seq.Contains(quart * 4 + 2))
                                face_slots++;
                            if (seq.Contains(quart * 4 + 3))
                                face_slots++;
                            if (face_slots == 3)
                            {
                                nstep++;
                                continue;
                            }
                            seq.Add(pos);
                            break;
                        }
                        if (nstep > maxTriesSlots)
                            break;
                        nstep++;
                    }
                    
                }

                if (seq.Count == slotsAmount)
                {
                    var shifted = shiftSeq(seq);
                    if (seqs.Contains(seq) || seqs.Contains(shifted[0]) ||
                        seqs.Contains(shifted[1]) || seqs.Contains(shifted[2]))
                        continue;
                }
                seqs.Add(seq);
                var border = TableMatrix.GenerateFromSeq(seq, CubeSize, "Border " + borderI.ToString());
                if (Check(border))
                {
                    Borders.Add(border);
                    borderI++;
                    i_struct++;
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
            SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CubeG\\save.json");
            IsCheckMirror = true;
            //LoadProgramData();
        }

        public void CreateNewCube(CubeType cubeType)
        {

            if (cubeType is CubeType.Cube)
            {
                int borderI = Cubes.Count;
                if (Cubes.Count != 0)
                {
                    var lastIndex = Cubes[Cubes.Count - 1].Name.Split(' ');
                    int.TryParse(lastIndex[lastIndex.Length - 1], out borderI);
                }
                Cubes.Add(new TableMatrix(CubeSize, CubeType.Cube, "Cube " + (borderI + 1)));
            }

            else
            {
                int borderI = Borders.Count;
                if (Borders.Count != 0)
                {
                    var lastIndex = Borders[Borders.Count - 1].Name.Split(' ');
                    int.TryParse(lastIndex[lastIndex.Length - 1], out borderI);
                }
                Borders.Add(new TableMatrix(CubeSize, CubeType.Border, "Border " + (borderI + 1)));
            }
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
