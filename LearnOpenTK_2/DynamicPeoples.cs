using System;
using System.Collections.Generic;

namespace LearnOpenTK_2
{
    public class DynamicPeoples
    {
        public double dt = 0.01;
        public double rasstoyanie = 10;
        public double m = 10;//средняя масса людей будет такой))

        public double pr_x = 0;//для проекции на х
        public double pr_y = 0;//для преокции на у
        public double distance_peop = 0;//расстояние между т-т

        public double a_const = 50;//константа для силы
        public double b_const = 0.08;//константа для силы
        public int k = 1200;//коэф упругости
        public double r = 0.05;//радиус т-т, также задается в program

        //public double f_ott = 0;//сила отталкивания людей друг от друга
        public double f_upr = 0;//сила упругости между людьми

        public double f_vector = 150;//модуль направляющей силы

        //Сила взаимодействия, соответствующая Потенциалу Леннарда-Джонса
        public double force_Lennard = 0;
        public double potential_Lennard = 0;
        public double d = 3; // энергия связи
        public double a = 1; //длина связи

        //собственное пространство частицы
        public double square = 0;

        public double scale = 0.5;// множитель масшттаба

        //точка для притягивания людишек - передается прямо в метод
       // private double xx = 1.8;
        //private double yy = 0;

        public void Force(List<People> list, double xx, double yy)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Fx = 0;
                list[i].Fy = 0;
                list[i].Pressure = 0;
            }

            double s = 0;
            for (int i = 0; i < list.Count - 1; i++)
            {
                List<People> list2 = new List<People>();//нужно для рассчитывания площади локальной

                square = 0;
                f_upr = 0;
                force_Lennard = 0;
                rasstoyanie = Math.Sqrt(Math.Pow(xx - (list[i].X + r/scale),2) + Math.Pow(yy - list[i].Y, 2));
                for (int j = i + 1; j < list.Count; j++)
                {

                    distance_peop = Math.Sqrt(Math.Pow((list[j].X - list[i].X), 2) + Math.Pow((list[j].Y - list[i].Y), 2));
                    pr_x = (list[i].X - list[j].X) / (distance_peop);
                    pr_y = (list[i].Y - list[j].Y) / (distance_peop);

                    s = 2*(r/scale) / distance_peop;
                    potential_Lennard = d * (Math.Pow(s, 12) - Math.Pow(s, 6));
                    force_Lennard =  12 * d * (Math.Pow(s, 14) - Math.Pow(s, 8)) / Math.Pow(a,2) ;//на вектор взаимодействия умножается ниже


                    //if (distance_peop <= 4*r)//сила упругости возникает только при контакте
                    //{
                    //    f_upr = k * (4 * r - distance_peop);
                    //}

                    if (potential_Lennard >= 0)
                    {
                        //Console.WriteLine($"{ (force_Lennard) * pr_x} && {(force_Lennard) * pr_y}");
                        list[i].Fx += (force_Lennard) * pr_x;
                        list[i].Fy += (force_Lennard) * pr_y;
                        list[j].Fx -= (force_Lennard) * pr_x;
                        list[j].Fy -= (force_Lennard) * pr_y;

                        list2.Add(list[j]);

                        square = 1;
                        
                        list[i].Pressure += (-distance_peop) * Math.Sqrt(Math.Pow(list[i].Fx, 2) + Math.Pow(list[i].Fy, 2)) / (4 * square);
                        list[j].Pressure -= list[i].Pressure;
                    }

                    //когда i-го человека сравнили со всеми остальными j, запсукается расчет площади?
                    if (j == list.Count - 1)
                    {
                        square = Square(list2, list[i]);
                        if (square == 0)
                        {
                            continue;
                        }
                        //square = 1;
                        //list[i].Pressure = (-distance_peop) * Math.Sqrt(Math.Pow(list[i].Fx, 2) + Math.Pow(list[i].Fy, 2)) / (4 * square);
                        //list[j].Pressure = list[i].Pressure;
                    }
                }
                //вектор силы в точку выхода
                list[i].Fx += f_vector * (xx - (list[i].X + r / scale)) / rasstoyanie;
                list[i].Fy += f_vector * (yy - list[i].Y) / rasstoyanie;
            }

            rasstoyanie = Math.Sqrt(Math.Pow(xx - (list[list.Count - 1].X + r / scale), 2) + Math.Pow(yy - list[list.Count - 1].Y, 2));
            list[list.Count-1].Fx += f_vector * (xx - (list[list.Count - 1].X + r / scale)) / rasstoyanie;
            list[list.Count - 1].Fy += f_vector * (yy - list[list.Count - 1].Y) / rasstoyanie;
        }

        public void Velocity(List<People> list, double coordY_exit ,double xx1, double yy1)
        {
            var epsilon = 0.005;//маленькая величина для погрешности при выходе
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Vy == 0 && list[i].Vx != 0) { continue; }//когда человек вышел, то он продолжает двигаться прямолинейно
                if ((xx1 - (list[i].X +  r/scale)) <= r / scale + epsilon && (coordY_exit - Math.Abs(list[i].Y)) > r / scale + epsilon)//условие для прохода людей через проход?
                {
                    list[i].Vx = 1;
                    list[i].Vy = 0;
                }
                else
                {
                    list[i].Vx = 2*(list[i].Fx / m) * dt;
                    list[i].Vy = 2*(list[i].Fy / m) * dt;
                }
            }
        }

        public void Displacement(List<People> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                //пока хз нужно будет или нет
                list[i].X_old = list[i].X;
                list[i].Y_old = list[i].X;

                list[i].X += (list[i].Vx * dt);
                list[i].Y += (list[i].Vy * dt);
            }
        }

        double nado1 = 0;
        double nado2 = 0;
        public void ContactCheck(List<People> list, Barrier barriers, double xx, double yy)
        {
            nado1 = 0;
            nado2 = 0;
            // цифра 5 - количество рисуемых стен в массиве класса Барьеров
            //проверка на касание стен
            for (int i = 0; i < barriers.coordPair.Count; i++) 
            {
                if (barriers.coordPair[i][2] == barriers.coordPair[i][0])//вертикальные стены
                {
                    for (int j = 0; j < list.Count; j++)
                    {

                        //если прошел через проход, то взаимодействия не нужны
                        if (list[j].Vy == 0 && list[j].Vx != 0)
                        {
                            continue;
                        }

                        //nado1,2 - расстояние от крайних точек вертикальной стены до центра людей
                        nado1 = Math.Sqrt(Math.Pow(barriers.coordPair[i][0] - (list[j].X +  r/scale), 2) + Math.Pow(barriers.coordPair[i][1] - (list[j].Y), 2));
                        nado2 = Math.Sqrt(Math.Pow(barriers.coordPair[i][2] - (list[j].X +  r/scale), 2) + Math.Pow(barriers.coordPair[i][3] - (list[j].Y), 2));
                        
                        //рассматриваю правые стенки
                        if ((list[j].X + r/scale) + r/scale >= barriers.coordPair[i][0] && (list[j].X + r/scale) < barriers.coordPair[i][0])//&& (nado2 <= r / scale || nado1 <= r / scale))
                        {
                            //не могу понять, для чего это условие я писал в прошлый раз
                            //if (Math.Abs(list[j].Y) + r / scale <= Math.Abs(barriers.coordPair[i][1]) && Math.Abs(list[j].Y) + r / scale <= Math.Abs(barriers.coordPair[i][3]))
                            //{
                            //    Console.WriteLine("a?");
                            //    continue;
                            //}
                            list[j].Vx = (-list[j].Vx);
                        }

                        //левая стенка
                        if (Math.Abs(list[j].X + 2 * r) + 2 * r >= Math.Abs(barriers.coordPair[i][0]) && list[j].X + 2 * r > barriers.coordPair[i][0])
                        {
                            list[j].Vx = (-list[j].Vx);
                        }
                    }
                }

                //if(barriers.coordPair[i][3] == barriers.coordPair[i][1])//горизонтальные стенки
                //{
                //    for (int j = 0; j < list.Count; j++)
                //    {
                //        //верхняя стенка
                //        if (list[j].Y + 2 * r >= barriers.coordPair[i][1] && list[j].Y < barriers.coordPair[i][1])
                //        {
                //            list[j].Vy = (-list[j].Vy);
                //        }

                //        //нижняя
                //        if (Math.Abs(list[j].Y) + 2 * r >= Math.Abs(barriers.coordPair[i][1]) && list[j].Y > barriers.coordPair[i][1])
                //        {
                //            list[j].Vy = (-list[j].Vy);
                //        }
                //    }
                //}
            }
        }

        public void ContactCheckMetro(List<People> list, Barrier barriers, double xx, double yy)
        {
            nado1 = 0;
            nado2 = 0;

            //проверка на касание стен
            for (int i = 0; i < barriers.coordPair.Count; i++)
            {
                if ((barriers.coordPair[i][0] == barriers.coordPair[i][2]))//вертикальные стены
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        //если прошел через проход, то взаимодействия не нужны
                        if (list[j].Vy == 0 && list[j].Vx != 0)
                        {
                            continue;
                        }

                        //nado1,2 - расстояние от крайних точек вертикальной стены до центра людей (НЕ ПОМНЮ НАХ НАДО)
                        nado1 = Math.Sqrt(Math.Pow(barriers.coordPair[i][0] - (list[j].X + r / scale), 2) + Math.Pow(barriers.coordPair[i][1] - (list[j].Y), 2));
                        nado2 = Math.Sqrt(Math.Pow(barriers.coordPair[i][2] - (list[j].X + r / scale), 2) + Math.Pow(barriers.coordPair[i][3] - (list[j].Y), 2));

                        //рассматриваю стенки, которые расположены ниже 0 по оси OY
                        if ((barriers.coordPair[i][1] < 0 && list[j].Y <= 0) && (list[j].X + r / scale) + r / scale >= barriers.coordPair[i][0] && (list[j].X + r / scale) < barriers.coordPair[i][0] && ((((list[j].Y - 0.5 * r / scale))< (barriers.coordPair[i][1]) && ((list[j].Y - 0.5 * r / scale)) > (barriers.coordPair[i][3])) || (((list[j].Y - 0.5 * r / scale)) > (barriers.coordPair[i][1]) && ((list[j].Y - 0.5*r/scale)) < (barriers.coordPair[i][3]))))
                        {
                            if (Math.Abs(list[j].Y) + r / scale <= Math.Abs(barriers.coordPair[i][1]) && Math.Abs(list[j].Y) + r / scale <= Math.Abs(barriers.coordPair[i][3]))
                            {
                                Console.WriteLine("зашел");
                                continue;
                            }
                            list[j].Vx = -list[j].Vx;
                            continue;
                        }
                        //рассматриваю стенки, которые расположены выше 0 по оси OY
                        if ((barriers.coordPair[i][1] > 0 && list[j].Y > 0) && (list[j].X + r / scale) + r / scale >= barriers.coordPair[i][0] && (list[j].X + r / scale) < barriers.coordPair[i][0] && (((Math.Abs(list[j].Y)) + 0.5 * r / scale < Math.Abs(barriers.coordPair[i][1]) && (Math.Abs(list[j].Y)+ 0.5 * r / scale) + 0.5 * r / scale > Math.Abs(barriers.coordPair[i][3])) || ((Math.Abs(list[j].Y)) + 0.5 * r / scale > Math.Abs(barriers.coordPair[i][1]) && (Math.Abs(list[j].Y)) + 0.5 * r / scale < Math.Abs(barriers.coordPair[i][3]))))//&& (nado2 <= r / scale || nado1 <= r / scale))
                        {
                            list[j].Vx = -list[j].Vx;
                        }


                    }
                }

            }
        }

        public double Square(List<People> list, People ac)
        {
            //People ac - точка, вокруг которой ищу площадь
            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return 1;
            }
            double square1 = 0;
            double[] abc1 = new double[list.Count+1];//здесь x
            double[] abc2 = new double[list.Count+1];//здесь y

            abc1[list.Count] = list[0].X;
            abc2[list.Count] = list[0].Y;
            for (int i = 0; i < list.Count; i++)
            {
                abc1[i] = list[i].X;
                abc2[i] = list[i].Y;

                square1 += (abc1[i] * abc2[i+1] - abc2[i]*abc1[i+1]);
            }

            square1 = Math.Abs(square1) / 2; 
            return square1;
        }

    }
}
