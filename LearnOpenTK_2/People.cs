
namespace LearnOpenTK_2
{
    public class People
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Vx { get; set; }
        public double Vy { get; set; }
        public double Fx { get; set; }
        public double Fy { get; set; }
        public double X_old { get; set; }
        public double Y_old { get; set; }
        public double Pressure { get; set; }
        public bool Position { get; set; }//отвечает, дошел ли человек до первой точки, куда я его направил

        public People()
        {
            X = 0;
            Y = 0;
            Vx = 0;
            Vy = 0;
            Fx = 0;
            Fy = 0;
            Pressure = 0;
            Position = false;
        }

        public People(double x, double y)
        {
            X = x;
            Y = y;
            X_old = x;
            Y_old = y;
            Vx = 0;
            Vy = 0;
            Fx = 0;
            Fy = 0;
            Pressure = 0;
            Position = false;
        }

        public People(double x, double y, double vx, double vy)
        {
            X = x;
            Y = y;
            X_old = x;
            Y_old = y;
            Vx = vx;
            Vy = vy;
            Fx = 0;
            Fy = 0;
            Pressure = 0;
            Position = false;
        }

    }
}
