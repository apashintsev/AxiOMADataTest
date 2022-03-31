using System;

namespace AxiOMADataTest
{
    public class AxiomaData
    {
        public string Filename { get; set; }
        public int FilePosition { get; set; }
        public int Level { get; set; }
        public int SubprogVersion { get; set; }

        public Coords Coords { get; set; }
    }

    public struct Coords
    {
        public double x, y, z;
        public DateTime Date;

        public Coords(double _x, double _y, double _z)
        {
            x = _x;
            y = _y;
            z = _z;
            Date = DateTime.UtcNow;
        }
    }
}
