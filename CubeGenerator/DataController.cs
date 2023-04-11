using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeGenerator
{
    public class DataController
    {
        public readonly string appDataFolder;
        public readonly string DataFile;
        public ObservableCollection<TableMatrix> Cubes;
        public ObservableCollection<TableMatrix> Borders;

        public bool IsProceedGeneration = false;
        public TableMatrix ChoosenBorder
        { 
            get {
                if (Borders.Count == 0)
                    return null;
                if (currentBorderI > Borders.Count)
                    currentBorderI = 0;
                return Borders[currentBorderI];
            } }

        private int currentCubeI = 0;
        private int currentBorderI = 0;

        public int CubeSize;
        public TableMatrix ChoosenCube
        { get {
                if (Cubes.Count == 0)
                    return null;
                if (currentCubeI > Cubes.Count)
                    currentCubeI = 0;
                return Cubes[currentCubeI];
            } }

        public DataController()
        {
            appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CubeGenerator");
            DataFile = Path.Combine(appDataFolder, "cubeData.json");
            Cubes = new ObservableCollection<TableMatrix>();
            Borders = new ObservableCollection<TableMatrix>();
            CubeSize = 7;
        }

        public void CreateNewCube(CubeType cubeType)
        {
            if (cubeType is CubeType.Cube)
                Cubes.Add(new TableMatrix(CubeSize, CubeType.Cube, "Cube " + Cubes.Count));
            else
                Borders.Add(new TableMatrix(CubeSize, CubeType.Border, "Border " + Borders.Count));
        }
    }
}
