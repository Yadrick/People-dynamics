using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics_peoples
{
    public class LineIntersection
    {
        public double Intersection(double x1, double y1, double x2, double y2, double list_x, double list_y, double r, int key)
        {
            double[] proex_radius = new double[2];

            double xIntersection1 = 0;
            double yIntersection1 = 0;
            double xIntersection2 = 0;
            double yIntersection2 = 0;

            double projectionX = 0;
            double projectionY = 0;
            double projectionX2 = 0;
            double projectionY2 = 0;

            // Координаты двух крайних точек наклонной прямой
            double x11 = x1;
            double y11 = y1;
            double x22 = x2;
            double y22 = y2;

            // Координаты центра окружности
            double x0 = list_x;
            double y0 = list_y;
            double radius = r; // Радиус окружности

            // Нахождение уравнения наклонной прямой (y = mx + b)
            double m = (y22 - y11) / (x22 - x11);
            double b = y11 - m * x11;


            // Нахождение координат точки касания окружности и наклонной прямой
            double A = 1 + m * m;
            double B = -2 * x0 + 2 * m * (b - y0);
            double C = x0 * x0 + (b - y0) * (b - y0) - radius * radius;

            double discriminant = B * B - 4 * A * C;

            xIntersection1 = (-B + Math.Sqrt(discriminant)) / (2 * A);
            yIntersection1 = m * xIntersection1 + b;

            xIntersection2 = (-B - Math.Sqrt(discriminant)) / (2 * A);
            yIntersection2 = m * xIntersection2 + b;

            if (discriminant >= 0)
            {
                xIntersection1 = (-B + Math.Sqrt(discriminant)) / (2 * A);
                yIntersection1 = m * xIntersection1 + b;

                xIntersection2 = (-B - Math.Sqrt(discriminant)) / (2 * A);
                yIntersection2 = m * xIntersection2 + b;

                if (xIntersection1 == xIntersection2 && yIntersection1 == yIntersection2)
                {
                    projectionX = Math.Abs(xIntersection1 - x0);
                    projectionY = Math.Abs(yIntersection1 - y0);
                    proex_radius[0] = projectionX;
                    proex_radius[1] = projectionY;
                }
                else
                {
                    projectionX2 = Math.Abs(xIntersection2 - x0);
                    projectionY2 = Math.Abs(yIntersection2 - y0);

                    proex_radius[0] = (projectionX + projectionX2) / 2;
                    proex_radius[1] = (projectionY + projectionY2) / 2;
                }
            }


            if (key == 0)
            {
                return proex_radius[0];//вернет x
            }
            else
            {
                return proex_radius[1];//вернет y
            }
        }
    }
}
