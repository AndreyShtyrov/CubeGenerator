using System;

namespace CubeGenerator
{
    public enum CubeType 
    { 
        Cube,
        Border
    }

    public class TableMatrix
    {
        private bool[,] filled;
        private int rotAngle;
        private bool isOdd;

        public readonly int Size;
        public bool[,] Filled
        {
            get { return getRotatedMatrix(); }
        }

        public TableMatrix(int size, CubeType cubeType)
        {
            filled = new bool[size + 4, size + 4];
            if (Size % 2 == 0)
                isOdd = true;
            else
                isOdd = false;
            Size = size + 4;
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    filled[i, j] = false;
            if (cubeType == CubeType.Border)
                for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                    {
                        if ((i == 0 && j == 0) || ( i == Size && j == 0) ||
                                (i == 0 && j == Size) || ( i == Size && j == Size))
                            filled[i, j] = true;
                    }
            else
            {
                for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                        if ( i > 2 && i < Size - 2 && j > 2 && j < Size - 2)
                            filled[i, j] = true;

            }
            rotAngle = 0;
        }

        public void RotToLeft()
        {
            rotAngle += 1;
        }

        public void RotToRight()
        {
            rotAngle -= 1;
        }

        private (int X, int Y)[,] getZeroRotMatrix()
        {
            (int X, int Y)[,] rotM = new (int X, int Y)[Size, Size];
            var midSize = Size / 2;
            if (!isOdd)
                midSize += 1;
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    if (isOdd)
                        rotM[i, j] = (i - midSize, j - midSize);
                    else
                    {
                        var _i = i - midSize;
                        var _j = j - midSize;
                        if (_i < 0)
                            _i -= 1;
                        else
                            _i += 1;
                        if (_j < 0)
                            _j -= 1;
                        else
                            _j += 1;
                        rotM[i, j] = (_i, _j);
                    }
            return rotM;
        }

        private bool[,] bringView((int X, int Y)[,] rotated)
        { 
            bool[,] result = new bool[Size, Size];
            var midSize = Size / 2;
            if (!isOdd)
                midSize += 1;
            for (int i = 0; i < Size; i++)
                for(int j = 0; j < Size; j++)
                    result[midSize + rotated[i, j].X, midSize + rotated[i, j].Y] = filled[i, j];
            return result;
        }

        private bool[,] getRotatedMatrix()
        {
            var rotM = getZeroRotMatrix();
            var rots = rotAngle;
            if (rots > 0)
                rots = rots % 4;
            else
                rots = 4 + rots % 4;
            for (int t = 0; t < rots; t++)
            {
                for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                    {
                        var _i = rotM[i, j].X;
                        rotM[i, j].X = -1 * rotM[i, j].Y;
                        rotM[i, j].Y = _i;
                    }
            }
            return bringView(rotM);
        }

        public void AddSquar(int x, int y)
        {
            filled[x, y] = true;
        }

    }
}
