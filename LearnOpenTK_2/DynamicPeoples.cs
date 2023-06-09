﻿using Dynamics_peoples;
using System;
using System.Collections.Generic;

namespace LearnOpenTK_2
{
    public class DynamicPeoples
    {
        public SaveData saveData = new SaveData();

        public LineIntersection lineInter = new LineIntersection();

        public double dt = 0.015;
        double time = 0;//буду здесь фиксировать прошедшее время
        public double rasstoyanie = 10;
        public int list_count = 0;

        public double rasstoyanie2 = 10;//просто расстояние, потом пересчитывается
        public double x_nado = 0;
        public double y_nado = 0;
        public double[] massiv = new double[50];// 50 - число людей, который заходят вметро (на самом деле можно меньше)


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

        public double f_vector = 100;//модуль направляющей силы

        //Сила взаимодействия, соответствующая Потенциалу Леннарда-Джонса
        public double force_Lennard = 0;
        public double potential_Lennard = 0;
        public double d = 3; // энергия связи
        public double d2 = 1; // энергия связи со стеной?
        public double a = 1; //длина связи

        //собственное пространство частицы
        public double square = 0;

        public double scale = 0.5;// множитель масшттаба

        //точка для притягивания людишек в комнате - передается прямо в метод
        // private double xx = 1.8;
        //private double yy = 0;



        double per = 0;// просто нужна внутри метода
        public void Force(int key, List<People> list, List<People> listInput, double xx, double yy, double xx1, double yy1, double xx2, double yy2, Barrier barriers)
        {
            // key отвечает за кол-во эскалаторов на подъем и спуск:
            // key = 0 - два подъема, key = 1 - два спуска

            if (key == 1)
            {
                xx2 = xx1;
                yy2 = yy1;
            }

            if (list_count == 0)
            {
                list_count = list.Count;
            }

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Fx = 0;
                list[i].Fy = 0;
                list[i].Pressure = 0;
            }


            double s = 0;

            // считаю силы взаимодействия между i-тым челом и всеми остальными
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

                    s = 1.95 *(r/scale) / distance_peop; //
                    potential_Lennard = d * (Math.Pow(s, 12) - Math.Pow(s, 6));
                    force_Lennard =  12 * d * (Math.Pow(s, 14) - Math.Pow(s, 8)) / Math.Pow(a,2) ;//на вектор взаимодействия умножается ниже

                    

                    if (potential_Lennard >= 0)
                    {
                        if (force_Lennard > 250)
                        {         
                            force_Lennard = 250;
                        }
                        list[i].Fx += (force_Lennard) * pr_x;
                        list[i].Fy += (force_Lennard) * pr_y;
                        list[j].Fx -= (force_Lennard) * pr_x;
                        list[j].Fy -= (force_Lennard) * pr_y;

                        

                        list2.Add(list[j]);

                        square = 1;
                        
                        list[i].Pressure += (-distance_peop) * Math.Sqrt(Math.Pow(list[i].Fx, 2) + Math.Pow(list[i].Fy, 2)) / (4 * square);
                        list[j].Pressure -= list[i].Pressure;
                    }

                }


                //спецаильная првоерка для последнего человека
                if ((list[list.Count - 1].X + r / scale) >= xx - r / scale && list[list.Count - 1].Y < yy + 4 * r / scale && list[list.Count - 1].Y > yy - 3 * r / scale && !list[list.Count - 1].Position)
                {
                    list[list.Count - 1].Position = true;
                }
                //далее для всех кроме последнего. Для последнего выше + ниже
                if ((list[i].X + r / scale) >= xx - r / scale && list[i].Y < yy + 4 * r / scale && list[i].Y > yy - 3 * r / scale && !list[i].Position)
                {
                    list[i].Position = true;
                }
                else if (list[i].Position)
                {
                    if (list[i].X <= 4 * 2 * r / scale - 1 * r / scale)
                    {
                        double y_nado = (17.3 * r - 8 * (r) - 2.1 * 2 * r) / scale;
                        double y_nado2 = (17.3 * r - 8 * (r) - 3.6 * 2 * r) / scale;
                        if (key == 1)
                        {
                            y_nado2 = yy1;
                        }

                        //чтобы верхние люди сразу начали двигаться в сторону верхнего выхода
                        if (list[i].Y >= (yy1 + yy2) / 2)
                        {
                            rasstoyanie = Math.Sqrt(Math.Pow(4 * 2 * r / scale - (list[i].X + r / scale), 2) + Math.Pow(yy1 - list[i].Y, 2));

                            //пробую уменьшить движущую силу людей, который встали в пробку
                            if (list[i].BeforeForce >= 150 || force_Lennard >= 150)
                            {
                                f_vector = 2;
                            }
                            else
                            {
                                f_vector = 100;
                            }


                            //идет в точку x = (4 * 2 * r), (...)/2
                            list[i].Fx += f_vector * (4 * 2 * r / scale - (list[i].X + r / scale)) / rasstoyanie;
                            list[i].Fy += f_vector * (yy1 - list[i].Y) / rasstoyanie;
                        }
                        else
                        {
                            rasstoyanie = Math.Sqrt(Math.Pow(4 * 2 * r / scale - (list[i].X + r / scale), 2) + Math.Pow(y_nado2 - list[i].Y, 2));
                            //идет в точку x = (4 * 2 * r), (yy1 + yy2)/2

                            //пробую уменьшить движущую силу людей, который встали в пробку
                            if (list[i].BeforeForce >= 150 || force_Lennard >= 150)
                            {
                                f_vector = 2; 
                            }
                            else
                            {
                                f_vector = 100;
                            }

                            list[i].Fx += f_vector * (4 * 2 * r / scale - (list[i].X + r / scale)) / rasstoyanie;
                            list[i].Fy += f_vector * (y_nado2 - list[i].Y) / rasstoyanie;
                        }
                            
                    }
                    else if (list[i].Y > (yy1 + yy2) /2)
                    {
                        rasstoyanie = Math.Sqrt(Math.Pow(xx1 - (list[i].X + r / scale), 2) + Math.Pow(yy1 - list[i].Y, 2));
                        //идет в точку xx1,yy1

                        //пробую уменьшить движущую силу людей, который встали в пробку
                        if (list[i].BeforeForce >= 150 || force_Lennard >= 150)
                        {
                            f_vector = 2; 
                        }
                        else
                        {
                            f_vector = 100;
                        }

                        list[i].Fx += f_vector * (xx1 - (list[i].X + r / scale)) / rasstoyanie;
                        list[i].Fy += f_vector * (yy1 - list[i].Y) / rasstoyanie;
                    }
                    else if (list[i].Y <= (yy1 + yy2) / 2)
                    {
                        rasstoyanie = Math.Sqrt(Math.Pow(xx2 - (list[i].X + r / scale), 2) + Math.Pow(yy2 - list[i].Y, 2));
                        //идет в точку xx2,yy2

                        //пробую уменьшить движущую силу людей, который встали в пробку
                        if (list[i].BeforeForce >= 150 || force_Lennard >= 150)
                        {
                            f_vector = 2;
                        }
                        else
                        {
                            f_vector = 100;
                        }

                        list[i].Fx += f_vector * (xx2 - (list[i].X + r / scale)) / rasstoyanie;
                        list[i].Fy += f_vector * (yy2 - list[i].Y) / rasstoyanie;
                    }
                }
                else
                {

                    //пробую уменьшить движущую силу людей, который встали в пробку
                    if (list[i].BeforeForce >= 150 || force_Lennard >= 150)
                    {
                        f_vector = 2;
                    }
                    else
                    {
                        f_vector = 100;
                    }

                    list[i].Fx += f_vector * (xx - (list[i].X + r / scale)) / rasstoyanie;
                    list[i].Fy += f_vector * (yy - list[i].Y) / rasstoyanie;
                }

                //если челы зашли под перегородку, то им надо выходить от туда
                if (list[i].Y <= barriers.coordPair[14][1] && (list[i].X + r/scale) > barriers.coordPair[14][0] && (list[i].X + r/scale) <= barriers.coordPair[14][2])
                {
                    list[i].Position = false;
                }

            }

           


            //указания для последнего человечка выходящего из метро
            if (!list[list.Count - 1].Position)
            {
                rasstoyanie = Math.Sqrt(Math.Pow(xx - (list[list.Count - 1].X + r / scale), 2) + Math.Pow(yy - list[list.Count - 1].Y, 2));


                //пробую уменьшить движущую силу людей, который встали в пробку
                if (list[list.Count - 1].BeforeForce >= 150 || force_Lennard >= 150)
                {
                    f_vector = 2;
                }
                else
                {
                    f_vector = 100;
                }

                list[list.Count - 1].Fx += f_vector * (xx - (list[list.Count - 1].X + r / scale)) / rasstoyanie;
                list[list.Count - 1].Fy += f_vector * (yy - list[list.Count - 1].Y) / rasstoyanie;
            }
            else
            {
                
                if (list[list.Count - 1].X <= 4 * 2 * r / scale - 1 * r / scale)
                {
                    double y_nado2 = (17.3 * r - 8 * (r) - 3.6 * 2 * r) / scale;
                    //чтобы верхние люди сразу начали двигаться в сторону верхнего выхода
                    if (list[list.Count - 1].Y >= (yy1 + yy2) / 2)
                    {
                        rasstoyanie = Math.Sqrt(Math.Pow(4 * 2 * r / scale - (list[list.Count - 1].X + r / scale), 2) + Math.Pow(yy1 - list[list.Count - 1].Y, 2));
                        //идет в точку x = (4 * 2 * r), (...)/2

                        //пробую уменьшить движущую силу людей, который встали в пробку
                        if (list[list.Count - 1].BeforeForce >= 150 || force_Lennard >= 150)
                        {
                            f_vector = 2;
                        }
                        else
                        {
                            f_vector = 100;
                        }


                        list[list.Count - 1].Fx += f_vector * (4 * 2 * r / scale - (list[list.Count - 1].X + r / scale)) / rasstoyanie;
                        list[list.Count - 1].Fy += f_vector * (yy1 - list[list.Count - 1].Y) / rasstoyanie;
                    }
                    else
                    {
                        rasstoyanie = Math.Sqrt(Math.Pow(4 * 2 * r / scale - (list[list.Count - 1].X + r / scale), 2) + Math.Pow(y_nado2 - list[list.Count - 1].Y, 2));
                        //идет в точку x = (4 * 2 * r), (yy1 + yy2)/2

                        //пробую уменьшить движущую силу людей, который встали в пробку
                        if (list[list.Count - 1].BeforeForce >= 150 || force_Lennard >= 150)
                        {
                            f_vector = 2;
                        }
                        else
                        {
                            f_vector = 100;
                        }

                        list[list.Count - 1].Fx += f_vector * (4 * 2 * r / scale - (list[list.Count - 1].X + r / scale)) / rasstoyanie;
                        list[list.Count - 1].Fy += f_vector * (y_nado2 - list[list.Count - 1].Y) / rasstoyanie;
                    }

                }
                else if (list[list.Count - 1].Y > (yy1 + yy2) / 2)
                {
                    rasstoyanie = Math.Sqrt(Math.Pow(xx1 - (list[list.Count - 1].X + r / scale), 2) + Math.Pow(yy1 - list[list.Count - 1].Y, 2));
                    //идет в точку xx1,yy1

                    //пробую уменьшить движущую силу людей, который встали в пробку
                    if (list[list.Count - 1].BeforeForce >= 150 || force_Lennard >= 150)
                    {
                        f_vector = 2;
                    }
                    else
                    {
                        f_vector = 100;
                    }

                    list[list.Count - 1].Fx += f_vector * (xx1 - (list[list.Count - 1].X + r / scale)) / rasstoyanie;
                    list[list.Count - 1].Fy += f_vector * (yy1 - list[list.Count - 1].Y) / rasstoyanie;
                }
                else if (list[list.Count - 1].Y <= (yy1 + yy2) / 2)
                {
                    rasstoyanie = Math.Sqrt(Math.Pow(xx2 - (list[list.Count - 1].X + r / scale), 2) + Math.Pow(yy2 - list[list.Count - 1].Y, 2));
                    //идет в точку xx2,yy2

                    //пробую уменьшить движущую силу людей, который встали в пробку
                    if (list[list.Count - 1].BeforeForce >= 150 || force_Lennard >= 150)
                    {
                        f_vector = 2;
                        
                    }
                    else
                    {
                        f_vector = 100;
                    }

                    list[list.Count - 1].Fx += f_vector * (xx2 - (list[list.Count - 1].X + r / scale)) / rasstoyanie;
                    list[list.Count - 1].Fy += f_vector * (yy2 - list[list.Count - 1].Y) / rasstoyanie;
                }
            }

            //если последний чел зашедл под перегородку, то им надо выходить от туда
            if (list[list.Count - 1].Y < barriers.coordPair[14][1] && (list[list.Count - 1].X + r/scale) > barriers.coordPair[14][0] && (list[list.Count - 1].X + r/scale)<= barriers.coordPair[14][2])
            {
                list[list.Count - 1].Position = false;
            }




            for (int i = 0; i < list.Count; i++)
            {
                per = Math.Sqrt(Math.Pow(list[i].Fx, 2) + Math.Pow(list[i].Fy, 2));//сила, оказываемая на i-го чела
                list[i].BeforeForce = per;

                if (list[i].MaxForce <= Math.Abs(per))
                {
                    list[i].MaxForce = Math.Abs(per);
                }
            }

        }

        double eps = 0.02;
        public void WallContact(List<People> list, Barrier barriers)
        {
            double s = 0;

            // считаю силы взаимодействия между i-тым челом и СТЕНАМИ
            for (int i = 0; i < list.Count; i++)
            {
                force_Lennard = 0;
                potential_Lennard = 0;


                for (int k = 0; k < barriers.circles.Count; k++)
                {

                    distance_peop = Math.Sqrt(Math.Pow((barriers.circles[k][0] - list[i].X), 2) + Math.Pow((barriers.circles[k][1] - list[i].Y), 2));
                    pr_x = (list[i].X - barriers.circles[k][0]) / (distance_peop);
                    pr_y = (list[i].Y - barriers.circles[k][1]) / (distance_peop);

                    s = 1.8 * (r / scale) / distance_peop; //правильно будет 2 * ...
                    potential_Lennard = d2 * (Math.Pow(s, 12) - Math.Pow(s, 6));
                    force_Lennard = 12 * d2 * (Math.Pow(s, 14) - Math.Pow(s, 8)) / Math.Pow(a, 2);//на вектор взаимодействия умножается ниже


                    if (potential_Lennard >= 0)
                    {
                        //Console.WriteLine($"{ (force_Lennard) * pr_x} && {(force_Lennard) * pr_y}");
                        list[i].Fx += (force_Lennard) * pr_x;
                        list[i].Fy += (force_Lennard) * pr_y;
                    }
                    
                }




                for (int j = 0; j < barriers.coordPair.Count; j++)
                {
                    // ПОКА ЧТО смотрю контакт только после X=0
                    if ((list[i].X + r / scale) + r / scale >= 0)
                    {
                        if (barriers.coordPair[j][0] == barriers.coordPair[j][2])// иксы точек равны, значит вертикальная прямая
                        {
                            // уравнение прямой будет x = barriers.coordPair[j][0]
                            // расстояние от точки до прямой: d = Math.Abs(A * Mx + B * My + C) / Math.Sqrt(A^2+B^2);
                            // где уравнение прямой в канон. виде: Ax+By+C=0, Mx и My - координаты центра человека
                            // в данном случае B = 0, x = -C/A
                            if ((list[i].Y >= barriers.coordPair[j][1] && list[i].Y < barriers.coordPair[j][3]) || (list[i].Y < barriers.coordPair[j][1] && list[i].Y >= barriers.coordPair[j][3]))
                            {
                                distance_peop = Math.Abs((list[i].X + r / scale) - barriers.coordPair[j][0]);
                                
                                if ((list[i].X + r/scale) + eps <= barriers.coordPair[j][0])
                                {
                                    pr_x = -1;
                                }
                                else
                                {
                                    pr_x = 1;
                                }
                                pr_y = 0;
                            }
                            else
                            {
                                pr_x = 0;
                                pr_y = 0;
                            }

                        }
                        else if (barriers.coordPair[j][1] == barriers.coordPair[j][3])// игреки точек равны, значит горизонтальная прямая
                        {
                            // уравнение прямой будет y = barriers.coordPair[j][3]
                            // расстояние от точки до прямой: d = Math.Abs(A * Mx + B * My + C) / Math.Sqrt(A^2+B^2);
                            // где уравнение прямой: Ax+By+C=0, Mx и My - координаты центра человека
                            // в данном случае A = 0, y = -C/B
                            if ((list[i].X + r / scale >= barriers.coordPair[j][0] && list[i].X + r / scale < barriers.coordPair[j][2]) || (list[i].X + r / scale < barriers.coordPair[j][0] && list[i].X + r / scale >= barriers.coordPair[j][2]))
                            {
                                distance_peop = Math.Abs(list[i].Y - barriers.coordPair[j][3]);

                                if (list[i].Y < barriers.coordPair[j][1])
                                {
                                    pr_y = -1;
                                }
                                else
                                {
                                    pr_y = 1;
                                }
                                pr_x = 0;
                            }
                            else
                            {
                                pr_x = 0;
                                pr_y = 0;
                            }
                        }
                        else//если наклонные прямые, пока что такое условие)
                        {
                            // уравнение прямой будет (barriers.coordPair[j][3]-barriers.coordPair[j][1])(x - barriers.coordPair[j][0]) - (barriers.coordPair[j][2]-barriers.coordPair[j][0])(y - barriers.coordPair[j][1]) = 0
                            // расстояние от точки до прямой: d = Math.Abs(A * Mx + B * My + C) / Math.Sqrt(A^2+B^2);
                            // где уравнение прямой: Ax+By+C=0, Mx и My - координаты центра человека
                            // в данном случае A = (barriers.coordPair[j][3]-barriers.coordPair[j][1]), y = -C/B

                            if (((list[i].X + r / scale + r / scale >= barriers.coordPair[j][0] && list[i].X + r / scale < barriers.coordPair[j][2])) && ((list[i].Y >= barriers.coordPair[j][1] && list[i].Y - r / scale <= barriers.coordPair[j][3])))
                            {
                                //double c = (barriers.coordPair[j][1] - barriers.coordPair[j][3]) * barriers.coordPair[j][0] + (barriers.coordPair[j][2] - barriers.coordPair[j][0]) * barriers.coordPair[j][1];
                                //double aa = (barriers.coordPair[j][3] - barriers.coordPair[j][1]) * (barriers.coordPair[j][3] - barriers.coordPair[j][1]);
                                //double bb = (barriers.coordPair[j][2] - barriers.coordPair[j][0]) * (barriers.coordPair[j][2] - barriers.coordPair[j][0]);
                                //distance_peop = Math.Abs(((barriers.coordPair[j][3] - barriers.coordPair[j][1]) * list[i].X + (barriers.coordPair[j][2] - barriers.coordPair[j][0]) * list[i].Y + c)/Math.Sqrt(aa + bb));


                                //pr_x = -(lineInter.Intersection(barriers.coordPair[j][0], barriers.coordPair[j][1], barriers.coordPair[j][2], barriers.coordPair[j][3],list[i].X,list[i].Y, r/scale, 0)) / (distance_peop);
                                //pr_y = (lineInter.Intersection(barriers.coordPair[j][0], barriers.coordPair[j][1], barriers.coordPair[j][2], barriers.coordPair[j][3], list[i].X, list[i].Y, r / scale, 1)) / (distance_peop);
                                pr_x = 0;
                                pr_y = 0;
                            }
                            else
                            {
                                pr_x = 0;
                                pr_y = 0;
                            }

                        }

                        s = 1.9 * (r / scale) / distance_peop;

                        if (list[i].X >= 6 * 2 * r / scale)
                        {
                            s = 1.5 * (r / scale) / distance_peop;

                        }
                        else if (list[i].X >= 3 * 2 * r / scale && list[i].X < 6 * 2 * r / scale)
                        {
                            s = 2 * (r / scale) / distance_peop;

                        }

                        potential_Lennard = d2 * (Math.Pow(s, 12) - Math.Pow(s, 6));
                        force_Lennard = 12 * d2 * (Math.Pow(s, 14) - Math.Pow(s, 8)) / Math.Pow(a, 2);//на вектор взаимодействия умножается ниже





                        if (potential_Lennard >= 0)
                        {
                            
                            if (force_Lennard > 250)
                            {
                                force_Lennard = 250;
                            }

                            list[i].Fx += (force_Lennard) * pr_x;
                            list[i].Fy += (force_Lennard) * pr_y;
                        }
                    }
                }

            }
        }





        public void Velocity(List<People> list, List<People> listInput, double xx, double yy, double xx1, double yy1, double xx2, double yy2, double h)
        {
            //var epsilon = 0.005;//маленькая величина для погрешности при выходе

            //для выходящих из метро людей
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Vy == 0 && list[i].Vx != 0) { continue; }//когда человек вышел, то он продолжает двигаться прямолинейно
                if ((xx1 - (list[i].X + r / scale)) <= r / scale && (list[i].Y) < yy1 + h && (list[i].Y) > yy1 - h)//условие для прохода людей через проход?
                {
                    list[i].Vx = 0.5;
                    list[i].Vy = 0;
                }
                else if((xx2 - (list[i].X + r / scale)) <= r / scale  && (list[i].Y) < yy2 + h && (list[i].Y) > yy2 - h)//условие для прохода людей через проход?
                {
                    list[i].Vx = 0.5;
                    list[i].Vy = 0;
                }
                else
                {
                    list[i].Vx = (list[i].Fx / m) * dt;
                    list[i].Vy = (list[i].Fy / m) * dt;
                    //list[i].Vx = 0;
                    //list[i].Vy = 0;

                    if (saveData.Velocity == 0)
                    {
                        saveData.Velocity = Math.Round((Math.Sqrt(list[i].Vx * list[i].Vx + list[i].Vy * list[i].Vy)),3);
                        Console.WriteLine(saveData.Velocity);
                        saveData.countPeoppleOutput = list_count;
                    }
                }
            }

            // для входящих в метро людей
            for (int j = 0; j < listInput.Count; j++)
            {
                if (listInput[j].X + r/scale < 7 * 2 * r / scale)
                {
                    listInput[j].Vx = 2 * (listInput[j].Fx / m) * dt;
                    listInput[j].Vy = 2 * (listInput[j].Fy / m) * dt;
                }
            }
            
        }


        double averageForce = 0;//нужны для подсчета дисперсии модуля силы
        double averageSquareForce = 0;
        int countPeopleExit = 0; //счетчик вышедших людей
        int counter = 0;//счетчик продеших поездов
        public void Displacement(List<People> list, int countEsc, int countVagon, int countTrain, double timePreviousTrain)
        {
            
            time += dt;

            if (list_count == 0)
            {
                list_count = list.Count;
            }

            for (int i = 0; i < list.Count; i++)
            {
                // если заспавнились новые люди, когда старые ещё не все вышли, то я буду это записывать
                if (list.Count > list_count && saveData.countPeopleLeft[counter] == 0)
                {
                    saveData.countPeopleLeft[counter] = list.Count - list_count;
                }


                //счетчик зашедших на эскалатор
                if (list[i].Vx != 0 && list[i].Vy == 0)
                {
                    if (list[i].X > 1/scale)
                    {
                        //для рассчета дисперсии модуля силы
                        averageSquareForce += Math.Pow(list[i].MaxForce, 2);
                        averageForce += list[i].MaxForce;

                        countPeopleExit++;
                        list.RemoveAt(i);
                    }
                   
                }

                //время первого зашедшегго на эскалатор
                if (countPeopleExit == 1 && saveData.timeFirstOut == 0)
                {
                    saveData.timeFirstOut = Math.Round(time,3);
                    if (countTrain == 0)
                    {
                        saveData.timePreviousTrain = 0;
                    }
                    else { saveData.timePreviousTrain = timePreviousTrain; }
                }

                //время всех зашедших на эскалатор
                if (countPeopleExit == list_count && saveData.timeOut == 0)
                {
                    saveData.timeOut = Math.Round(time, 3);
                }
                
                //когда вышли все люди из "counter" поезда, записываю в файл результаты
                if (countPeopleExit == list_count)
                {
                    saveData.countVagon = countVagon;
                    saveData.countTrain = countTrain+1;//+1 т.к. буду считать ещё изначальный поезд

                    //////// дисперсия модуля силы записывавется только после того, как вышли все люди из первого поезда - и только для них ( пока что)
                    averageSquareForce /= list_count;
                    averageForce /= list_count;

                    saveData.forceDispersion[counter] = averageSquareForce - Math.Pow(averageForce,2);
                    ////////

                    if (countEsc == 0)
                    {
                        saveData.countEscalator = 2; //кол-во эскалаторов на подъем
                    }
                    else
                    {
                        saveData.countEscalator = 1; //
                    }

                    counter++;              //увеличиваю число прибывших поездов
                    countPeopleExit = 0;    //обнуляю, чтобы считалось по новому для других поездов
                    averageSquareForce = 0;//обнуляю переменные, необходимые для дисперсии модуля силы
                    averageForce = 0;

                    Console.WriteLine(counter);
                    // если все люди из последнего поезда вышли и прошли все поезда, то записываем
                    if (counter == countTrain+1)
                    {
                        saveData.Save();
                    }
                }

                //пока хз нужно будет или нет
                list[i].X_old = list[i].X;
                list[i].Y_old = list[i].X;

                list[i].X += (list[i].Vx * dt);
                list[i].Y += (list[i].Vy * dt);
                //list[i].X += 0;
                //list[i].Y += 0;

            }


        }

        //displacement для входящих в метро
        public void DisplacementInput(List<People> list)
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
                        //если прошел через проход, то взаимодействия с вертикальными стенами не нужны
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
                else if ((barriers.coordPair[i][1] == barriers.coordPair[i][3]))//горизонтальные стены
                {
                    double eps = 0.0001;

                    for (int j = 0; j < list.Count; j++)
                    {
                        //если коорда начала горизонтальной линии <= координаты человека и смотрю контакты только после X = 0
                        if (barriers.coordPair[i][0] <= (list[j].X + r / scale) && barriers.coordPair[i][2] >= (list[j].X + r / scale) && list[j].X >= 0 && barriers.coordPair[i][0] >= 0)
                        {
                            //если человек подходит снизу вверх к стенке
                            if (list[j].Y + r / scale >= barriers.coordPair[i][1] - eps && list[j].Y < barriers.coordPair[i][1] - eps )
                            {
                                list[j].Vy = (-list[j].Vy);
                            }
                            //если сверху вниз
                            //else if (list[j].Y - r / scale <= barriers.coordPair[i][1] && list[j].Y > barriers.coordPair[i][1])
                            //{
                            //    list[j].Vy = (-list[j].Vy);
                            //}
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
