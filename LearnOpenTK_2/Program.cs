using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace LearnOpenTK_2
{
    class Program
    {
        public double w = 1400;
        public double h = 1000;
        public int countPeople = 60;                           //число людей в 2-х вагонах (в 1-м сверху и в 1-м вагоне снизу)
        public int countPeopleInput = 10;
        public int countVagons = 1;                             //число вагонов поезда (всего 2 поезда => 2 * countVagons вагона)
        public int count_train_coming = 0;                      //ставлю = 4, чтобы в сумме с первым было 5

        public List<People> peoples = new List<People>();       //будет содержать всех имеющихся людей 
        public List<People> peoplesInput = new List<People>();  //будет содержать всех заходящих людей 


        public Barrier metro = new Barrier();//модель местности возле эскалатора на подъеме из метро "Политехническая"


        //размеры комнаты
        public double x_right = 0.9;
        public double x_left = -0.9;
        public double y_top = 0.9;
        public double y_bot = -0.9;

        //параметр, на который делятся размеры комнаты для увеличения диапазона
        public double scale = 0.5;

        //координаты центра выхода в комнате
        //public double xx = x_right
        public double yy = 0;

        //координаты движения точек для выхода из метро
        public double xxMetro = 0;
        public double yyMetro = (17.3 * 0.05 - 8 * (0.05) - 2.85 * 2 * 0.05);//(y_top(metro) * r - 8 * (r) - 2.85 * 2 * r); - общая формула
        //далее коорды 2-х выходов из метро
        public double xx1Metro = 0 + 7 * 2 * 0.05; // x_right(metro) + 7 * 2 * r
        public double xx2Metro = 0 + 7 * 2 * 0.05;// x_right(metro) + 7 * 2 * r
        public double yy1Metro = 17.3*0.05 - 8 * (0.05) - 1.05 * 2 * 0.05; // (y_top - 8 * (r) - 1.05 * 2 * r)
        public double yy2Metro = 17.3*0.05 - 8 * (0.05) - 4.65 * 2 * 0.05; // (y_top - 8 * (r) - 4.65 * 2 * r)

        public double eps = 0.1;//половина высоты прохода? в room

        public string flag = "metro";//моделируется метро или нет

        public double r = 0.05; //радиус людей

        //переменные для регулирования диапазона создания людей в Metro
        public double minValueMetro = -0.9 + 0.05 * 2;//+r надо
        public double maxValueMetro = 0 - 0.05 * 2;
        public double maxValueMetroY = 0.9 - 0.05 * 2;





        public void CreatePeopleMetro()//создаст людей и запихнет в массив (спавнятся в квадрате  minValue:maxValue)
        {
            Random rnd = new Random();
            double[] abc = new double[2];
            int counts = 0;

            
            while (peoples.Count != countPeople)
            {
                double x = (rnd.NextDouble() * (maxValueMetro - minValueMetro) + minValueMetro) / scale;
                double y = (rnd.NextDouble() * (maxValueMetroY - minValueMetro) + minValueMetro) / scale;
                counts = 0;

                //смотрю, чтобы люди не появлялись друг в друге
                for (int i = 0; i < peoples.Count; i++)
                {
                    if ((x > peoples[i].X + r / scale || x < peoples[i].X - r / scale) && (y > peoples[i].Y + r / scale || y < peoples[i].Y - r / scale))
                    {
                        counts++;
                    }
                }

                if (counts == peoples.Count)
                {
                    peoples.Add(new People(x, y));
                }
            }

        }

        public void CreatePeopleMetroStatic(int countVagon)//создаст людей и запихнет в массив (спавнятся в квадрате  minValue:maxValue)
        {
            Random rnd = new Random();
            int halfCountPeople = countPeople / 2;
            int secondHalfCountPeople = countPeople - halfCountPeople;

            int[][] massiv2 = new int[countVagon][];//всего countVagon вагонов, по 4 выхода(дальше инициализирую)

            //инициализирую каждому вагону метро по 4 выхода
            for (int i = 0; i < massiv2.Length; i++)
            {
                massiv2[i] = new int[4];
            }


            int a = 0;//для рандома

            //для каждого вагона метро:
            for (int j = 0; j < massiv2.Length; j++)
            {
                //добавляю людей, вышедших из поезда снизу
                for (int i = 0; i < halfCountPeople; i++)
                {
                    // a - условно отвечает за номер двери при выходе из выгона
                    a = rnd.Next(0, 4);
                    //+ r*rnd.Next(0,2) - смещение на r вправо
                    double x = (a * ((-8 * 2 * r)) - 4 * 2 * r + Math.Pow(-1, rnd.Next(1, 2)) * r * rnd.Next(0, 2) - j * (30 * 2 * r)) /scale;

                    //massiv[a]++;
                    massiv2[j][a]++;

                    //double y = -(9 + (massiv[a] - 1)) * 2 * r / scale;
                    double y = -(9 + (massiv2[j][a] - 1)) * 2 * r / scale;

                    peoples.Add(new People(x, y));
                }

                //обнуляю массив, чтобы добавить людей, вышедших из поезда сверху
                for (int i = 0; i < massiv2[j].Length; i++)
                {
                    massiv2[j][i] = 0;
                }

                for (int i = 0; i < secondHalfCountPeople; i++)
                {
                    a = rnd.Next(0, 4);
                    //+ Math.Pow(-1, rnd.Next(1, 2))*r*rnd.Next(0,2) - смещение на r вправо-влево
                    double x = (a * ((-8 * 2 * r)) - 4 * 2 * r + Math.Pow(-1, rnd.Next(1, 2)) * r * rnd.Next(0, 2) - j * (30 * 2 * r)) / scale;

                    //massiv[a]++;
                    massiv2[j][a]++;


                    //double y = (9 + (massiv[a] - 1)) * 2 * r / scale;
                    double y = (9 + (massiv2[j][a] - 1)) * 2 * r / scale;

                    peoples.Add(new People(x, y));
                }
            }
           
        }


        //будет создавать людей, входящих внутрь метро
        public void LoadingPeopleInsideMetro(double vx)
        {
            // буду спавнить каждый тик по одному или сразу пак людей?
            // пока что хочу сразу пак

            double x = 0;
            double y = (-17.3 * r + 8 * (r) + 2.1 * 2 * r - 2*r) / scale; // -0.41

            for (int i = 0; i < countPeopleInput; i++)
            {
                // (9 * 2 * r) - конец видимого эскалатора
                x =   (20 * 2 * r) + (i) * 2 * (2 * r) /scale;

                peoplesInput.Add(new People(x, y, vx, 0));
            }
             

        }


        public void CreateMetro(int key)
        {
            // key = 0 - 2 подъема; key = 1 - 2 спуска


            //нужно изменить переменные для регулирования диапазона создания людей


            //eps - половина расстояния прохода
            //размеры комнаты
            double x_right = 0;
            double x_left = -0.9;
            double y_top = 17.3 * r;
            double y_bot = -17.3 * r;

            double[] stena1 = { x_left / scale, y_top / scale, x_right / scale, y_top / scale };//верхняя горизонт
            double[] stena2 = { x_left / scale, y_bot / scale, x_right / scale, y_bot / scale };//нижняя горизонт

            double[] stena3 = { x_right / scale, y_top / scale, x_right / scale, (y_top - 8 * (r)) / scale };//левая верхняя веретикальная 
            double[] stena4 = { x_right / scale, y_bot / scale, x_right / scale, (y_bot + 8 * (r)) / scale };//левая нижняя веретикальная 

            double[] stena5 = { x_right / scale, (y_top - 8 * (r)) / scale, (x_right + 7 * 2 * r) / scale, (y_top - 8 * (r)) / scale};
            double[] stena6 = { x_right / scale, (y_bot + 8 * (r)) / scale, (x_right + 7 * 2 * r) / scale, (y_bot + 8 * (r)) / scale };

            //эскалаторы
            double[] stena7 = { (x_right + 7 * 2 * r) / scale, (y_top - 8 * (r)) / scale, (x_right + 11 * 2 * r) / scale, (y_top - 8 * (r)) / scale };
            double[] stena9 = { (x_right + 7 * 2 * r) / scale, (y_top - 8 * (r) - 2.1 * 2 * r) / scale, (x_right + 11 * 2 * r) / scale, (y_top - 8 * (r) - 2.1 * 2 * r) / scale };
            double[] stena11 = { (x_right + 7 * 2 * r) / scale, (y_top - 8 * (r) - 3.6 * 2 * r) / scale, (x_right + 11 * 2 * r) / scale, (y_top - 8 * (r) - 3.6 * 2 * r) / scale };
            double[] stena12 = { (x_right + 7 * 2 * r) / scale, (y_top - 8 * (r) - 5.7 * 2 * r) / scale, (x_right + 11 * 2 * r) / scale, (y_top - 8 * (r) - 5.7 * 2 * r) / scale };
            double[] stena10 = { (x_right + 7 * 2 * r) / scale, (y_bot + 8 * (r) + 2.1 * 2 * r) / scale, (x_right + 11 * 2 * r) / scale, (y_bot + 8 * (r) + 2.1 * 2 * r) / scale };
            double[] stena8 = { (x_right + 7 * 2 * r) / scale, (y_bot + 8 * (r)) / scale, (x_right + 11 * 2 * r) / scale, (y_bot + 8 * (r)) / scale };

            //перегородки между эскалаторами
            double[] stena13 = { (x_right + 7 * 2 * r) / scale, (y_top - 8 * (r) - 2.1 * 2 * r) / scale, (x_right + 7 * 2 * r) / scale, (y_top - 8 * (r) - 3.6 * 2 * r) / scale };
            double[] stena14 = { (x_right + 7 * 2 * r) / scale, (y_bot + 8 * (r) + 2.1 * 2 * r) / scale, (x_right + 7 * 2 * r) / scale, (y_top - 8 * (r) - 5.7 * 2 * r) / scale };

            //разделитель между входящим-выходящим потоком людей
            double[] stena15 = { (x_right + 1 * 2 * r) / scale, 0, (x_right + 4 * 2 * r) / scale, 0};
            double[] stena16 = { (x_right + 4 * 2 * r) / scale, 0, (x_right + 7 * 2 * r) / scale, (y_top - 8 * (r) - 6.45 * 2 * r) / scale};
            //2 эскалатора на спускк и 1 на подъем
            double[] stena17 = { (x_right + 4 * 2 * r) / scale, 0, (x_right + 7 * 2 * r) / scale, (y_top - 8 * (r) - 2.85 * 2 * r) / scale };

            metro.coordPair.Add(stena1);
            metro.coordPair.Add(stena2);
            metro.coordPair.Add(stena3);
            metro.coordPair.Add(stena4);
            metro.coordPair.Add(stena5);
            metro.coordPair.Add(stena6);

            //стенки для эскалаторов
            metro.coordPair.Add(stena7);
            metro.coordPair.Add(stena9);

            metro.coordPair.Add(stena11);
            metro.coordPair.Add(stena12);

            metro.coordPair.Add(stena8);
            metro.coordPair.Add(stena10);

            //перегородки между эскалатоорами
            metro.coordPair.Add(stena13);
            metro.coordPair.Add(stena14);

            //разделители потоков
            metro.coordPair.Add(stena15);
            //наклонный разделитель потоков
            if (key == 1)
            {
                metro.coordPair.Add(stena17);
            }
            else
            {
                metro.coordPair.Add(stena16);
            }

            ////////////////////////////////////////////////////////////////////////////
            ///Добавляю точки-барьеры на стыках линий
            double radiusBar = 0.01;
            double[] proba0 = { stena15[2] - radiusBar / scale, stena15[1], radiusBar };//начало наклонной
            double[] proba1 = { stena15[0] - radiusBar/scale, stena15[1], radiusBar };
            double[] proba2 = { stena3[0] - radiusBar / scale, stena3[3], radiusBar };
            double[] proba3 = { stena3[0] - radiusBar / scale, stena3[1], radiusBar };
            double[] proba4 = { stena4[0] - radiusBar / scale, stena4[1], radiusBar };
            double[] proba5 = { stena4[0] - radiusBar / scale, stena4[3], radiusBar };
            double[] proba10 = { stena7[0] - radiusBar / scale, stena7[1], radiusBar };
            double[] proba11 = { stena8[0] - radiusBar / scale, stena8[1], radiusBar };

            double[] proba6 = { stena13[0] - radiusBar / scale, stena13[1], radiusBar };
            double[] proba7 = { stena13[0] - radiusBar / scale, stena13[3], radiusBar };

            double[] proba8 = { stena14[0] - radiusBar / scale, stena14[1], radiusBar };
            double[] proba9 = { stena14[0] - radiusBar / scale, stena14[3], radiusBar };

            metro.circles.Add(proba0);
            metro.circles.Add(proba1);
            metro.circles.Add(proba2);
            metro.circles.Add(proba3);
            metro.circles.Add(proba4);
            metro.circles.Add(proba5);
            metro.circles.Add(proba6);
            metro.circles.Add(proba7);
            metro.circles.Add(proba8);
            metro.circles.Add(proba9);
            metro.circles.Add(proba10);
            metro.circles.Add(proba11);

            ////////////////////////////////////////////////////////////////////////////
            ///Добавляю точки-барьеры на наклонной линии =)

            double m = -((y_top - 8 * (r) - 6.45 * 2 * r)) / ((x_right + 7 * 2 * r) - (x_right + 4 * 2 * r));

            double[] proba00 = new double[] { stena15[2] + 0.5 * radiusBar / scale, stena15[1] + m * 0.5 * radiusBar / scale, radiusBar };
            double[] proba01 = new double[] { stena15[2] + 2 * radiusBar / scale, stena15[1] + m * 2 * radiusBar / scale, radiusBar };
            double[] proba02 = { stena15[2] + 3.5 * radiusBar / scale, stena15[1] + m * 3.5 * radiusBar / scale, radiusBar };
            double[] proba03 = { stena15[2] + 5 * radiusBar / scale, stena15[1] + m * 5 * radiusBar / scale, radiusBar };
            double[] proba04 = { stena15[2] + 6.5 * radiusBar / scale, stena15[1] + m * 6.5 * radiusBar / scale, radiusBar };
            double[] proba05 = { stena15[2] + 8 * radiusBar / scale, stena15[1] + m * 8 * radiusBar / scale, radiusBar };
            double[] proba06 = { stena15[2] + 9.5 * radiusBar / scale, stena15[1] + m * 9.5 * radiusBar / scale, radiusBar };
            double[] proba07 = { stena15[2] + 11 * radiusBar / scale, stena15[1] + m * 11 * radiusBar / scale, radiusBar };
            double[] proba08 = { stena15[2] + 12.5 * radiusBar / scale, stena15[1] + m * 12.5 * radiusBar / scale, radiusBar };
            double[] proba09 = { stena15[2] + 14 * radiusBar / scale, stena15[1] + m * 14 * radiusBar / scale, radiusBar };
            double[] proba010 = { stena15[2] + 15.5 * radiusBar / scale, stena15[1] + m * 15.5 * radiusBar / scale, radiusBar };
            double[] proba011 = { stena15[2] + 17 * radiusBar / scale, stena15[1] + m * 17 * radiusBar / scale, radiusBar };
            double[] proba012 = { stena15[2] + 18.5 * radiusBar / scale, stena15[1] + m * 18.5 * radiusBar / scale, radiusBar };
            double[] proba013 = { stena15[2] + 20 * radiusBar / scale, stena15[1] + m * 20 * radiusBar / scale, radiusBar };
            double[] proba014 = { stena15[2] + 21.5 * radiusBar / scale, stena15[1] + m * 21.5 * radiusBar / scale, radiusBar };
            double[] proba015 = { stena15[2] + 23 * radiusBar / scale, stena15[1] + m * 23 * radiusBar / scale, radiusBar };
            double[] proba016 = { stena15[2] + 24.5 * radiusBar / scale, stena15[1] + m * 24.5 * radiusBar / scale, radiusBar };
            double[] proba017 = { stena15[2] + 26 * radiusBar / scale, stena15[1] + m * 26 * radiusBar / scale, radiusBar };
            double[] proba018 = { stena15[2] + 27.5 * radiusBar / scale, stena15[1] + m * 27.5 * radiusBar / scale, radiusBar };
            double[] proba019 = { stena15[2] + 29 * radiusBar / scale, stena15[1] + m * 29 * radiusBar / scale, radiusBar };

            if (key == 0) // один подъем
            {
                m = -m;
                proba00 = new double[]{ stena15[2] + 0.5 * radiusBar / scale, stena15[1] + m * 0.5 * radiusBar / scale, radiusBar };
                proba01 = new double[]{ stena15[2] + 2 * radiusBar / scale, stena15[1] + m * 2 * radiusBar / scale, radiusBar };
                proba02 = new double[] { stena15[2] + 3.5 * radiusBar / scale, stena15[1] + m * 3.5 * radiusBar / scale, radiusBar };
                proba03 = new double[] { stena15[2] + 5 * radiusBar / scale, stena15[1] + m * 5 * radiusBar / scale, radiusBar };
                proba04 = new double[] { stena15[2] + 6.5 * radiusBar / scale, stena15[1] + m * 6.5 * radiusBar / scale, radiusBar };
                proba05 = new double[] { stena15[2] + 8 * radiusBar / scale, stena15[1] + m * 8 * radiusBar / scale, radiusBar };
                proba06 = new double[] { stena15[2] + 9.5 * radiusBar / scale, stena15[1] + m * 9.5 * radiusBar / scale, radiusBar };
                proba07 = new double[] { stena15[2] + 11 * radiusBar / scale, stena15[1] + m * 11 * radiusBar / scale, radiusBar };
                proba08 = new double[] { stena15[2] + 12.5 * radiusBar / scale, stena15[1] + m * 12.5 * radiusBar / scale, radiusBar };
                proba09 = new double[] { stena15[2] + 14 * radiusBar / scale, stena15[1] + m * 14 * radiusBar / scale, radiusBar };
                proba010 = new double[] { stena15[2] + 15.5 * radiusBar / scale, stena15[1] + m * 15.5 * radiusBar / scale, radiusBar };
                proba011 = new double[] { stena15[2] + 17 * radiusBar / scale, stena15[1] + m * 17 * radiusBar / scale, radiusBar };
                proba012 = new double[] { stena15[2] + 18.5 * radiusBar / scale, stena15[1] + m * 18.5 * radiusBar / scale, radiusBar };
                proba013 = new double[] { stena15[2] + 20 * radiusBar / scale, stena15[1] + m * 20 * radiusBar / scale, radiusBar };
                proba014 = new double[] { stena15[2] + 21.5 * radiusBar / scale, stena15[1] + m * 21.5 * radiusBar / scale, radiusBar };
                proba015 = new double[] { stena15[2] + 23 * radiusBar / scale, stena15[1] + m * 23 * radiusBar / scale, radiusBar };
                proba016 = new double[] { stena15[2] + 24.5 * radiusBar / scale, stena15[1] + m * 24.5 * radiusBar / scale, radiusBar };
                proba017 = new double[] { stena15[2] + 26 * radiusBar / scale, stena15[1] + m * 26 * radiusBar / scale, radiusBar };
                proba018 = new double[] { stena15[2] + 27.5 * radiusBar / scale, stena15[1] + m * 27.5 * radiusBar / scale, radiusBar };
                proba019 = new double[] { stena15[2] + 29 * radiusBar / scale, stena15[1] + m * 29 * radiusBar / scale, radiusBar };
            }

            metro.circles.Add(proba00);
            metro.circles.Add(proba01);
            metro.circles.Add(proba02);
            metro.circles.Add(proba03);
            metro.circles.Add(proba04);
            metro.circles.Add(proba05);
            metro.circles.Add(proba06);
            metro.circles.Add(proba07);
            metro.circles.Add(proba08);
            metro.circles.Add(proba09);
            metro.circles.Add(proba010);
            metro.circles.Add(proba011);
            metro.circles.Add(proba012);
            metro.circles.Add(proba013);
            metro.circles.Add(proba014);
            metro.circles.Add(proba015);
            metro.circles.Add(proba016);
            metro.circles.Add(proba017);
            metro.circles.Add(proba018);
            metro.circles.Add(proba019);

        }



        static void Main(string[] args)
        {
            Program prog1 = new Program();//только для взятия размеров экрана программы
            

            var nativeWindowSettings = new NativeWindowSettings()//просто настройки окна OpenGL
            {
                Size = new Vector2i((int)prog1.w, (int)prog1.h),
                Title = "Bugagagagagagaga",
                Profile = ContextProfile.Compatability,
                Flags = ContextFlags.Default

            };

            using (Game game = new Game(GameWindowSettings.Default, nativeWindowSettings))
            {
                game.CenterWindow();
                game.Run();
            }
        }
    }


    public class Game : GameWindow
    {
        private double frameTime = 0;
        public double timeWorking = 0;              //время работы программы 
        public double time_moving = 0;              //время работы программы 
        public double timePreviousTrain = 0;        //время, прошедшее с предыдущего поезда !!!!! Задаю в Onload
        public double dt = 0.015;
        private int fps = 0;
        Program prog = new Program();
        DynamicPeoples dynamics = new DynamicPeoples();
        

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.On; // ограничение на количество фпс: если on - то это такое, сколько экран может показать (Update метод вызывается меньше раз)
        }

        //инициализация ресурсов
        protected override void OnLoad()
        {
            // множитель варьировал так: 0.9;0.8;0.7;0.6;0.5;0.4;0.35 - критическая
            // для 2-х вагонов множитель варьировал так: 0.9;0.8;0.7;0.65;0.645;0.63 - критич
            // для 3-х вагонов множитель варьировал так: 0.8,0.745,0.755
            timePreviousTrain = prog.countVagons * 37.05 * 0.81 + 20.4; // время прошедшее с предыдущего поезда 
            //Console.WriteLine((prog.countVagons * 37.05 * 0.75 + 20.4 ));
            //prog.CreatePeopleMetro();
            prog.CreatePeopleMetroStatic(prog.countVagons);//в параметре количество вагонов
            prog.LoadingPeopleInsideMetro(0);//-0.5);
            prog.CreateMetro(1); // если передать 0, то будет два подъема, если 1, то будет 2 спуска
            

            //dynamics.Displacement(prog.peoples); // хз зачем он тут был, пока что закомментил, но помоему можно удалить


            GL.Scale(prog.scale,prog.scale,1);//увеличиваю раззмер поля
            base.OnLoad();
            GL.ClearColor(Color4.LightBlue);//задаем цвет, которым будет очищаться задний фон в OnRenderFrame
        }

        //Проверка, произошло ли изменение в размерах экрана/формы - если да, то нужна перерисовка
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
        }


        int count_train = 0;
        //проверка на нажатие клавиш, изменение положения объектов в пространстве, мат операции
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            frameTime += args.Time;
            timeWorking += args.Time;//время работы программы
            fps++;

            //по истечению какого-то промежутка времени буду запускать новый поезд

            time_moving += dt;

            // пояснение к условию (про timePreviousTrain): время выхода первого человека у всех примерно 20.4(20400) секунд. С увеличением числа вагонов время выхода
            // всех людей увеличивается примерно на 37 секунд (37000)
            //Console.WriteLine($"{(int)(time_moving * 100)}");

            if (((int)(time_moving * 100) == (int)(timePreviousTrain * 100) || (int)(time_moving * 100) == timePreviousTrain + dt*100/2) && count_train < prog.count_train_coming)
            {
                time_moving = 0;
                prog.CreatePeopleMetroStatic(prog.countVagons);//в параметре количество вагонов
                count_train++;
                time_moving += dt;
            }


            try
            {
                //первая цифра в Force отвечает за кол-во эскалаторов на подъем/спуск
                dynamics.Force(1, prog.peoples, prog.peoplesInput, prog.xxMetro / prog.scale, prog.yyMetro / prog.scale, prog.xx1Metro / prog.scale, prog.yy1Metro / prog.scale, prog.xx2Metro / prog.scale, prog.yy2Metro / prog.scale, prog.metro);
                dynamics.WallContact(prog.peoples, prog.metro);
                dynamics.Velocity(prog.peoples, prog.peoplesInput, prog.xxMetro / prog.scale, prog.yyMetro / prog.scale, prog.xx1Metro / prog.scale, prog.yy1Metro / prog.scale, prog.xx2Metro / prog.scale, prog.yy2Metro / prog.scale, (prog.r) / prog.scale);

                //dynamics.ContactCheckMetro(prog.peoples, prog.metro, prog.x_right / prog.scale, prog.yy / prog.scale);
                dynamics.Displacement(prog.peoples, 1, prog.countVagons, prog.count_train_coming, timePreviousTrain); // цифра отвечает тому же, что и в Force

                //для входящего потока
                dynamics.DisplacementInput(prog.peoplesInput);
            }
            catch (Exception)
            {

            }

            

            if (frameTime >= 1)//чтобк каждую секунду выводить коилчество кадров
            {
                Title = $"FPS = {fps}";
                frameTime = 0;
                fps = 0;

                
            }
            base.OnUpdateFrame(args);
        }


        //а тут уже отрисовка кадров
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            


            GL.Clear(ClearBufferMask.ColorBufferBit);//очистка формы ДАННЫМ выше(в OnLoad) цветом 

            DrawPoint(prog.peoples);//рисуются чуваки 
            DrawPoint(prog.peoplesInput);//рисуются чуваки входящие

            DrawMetro(); //рисуется метро(барьеры)
            DrawPointBarrier(prog.metro.circles);//рисуются круглые барьеры
            

            SwapBuffers();// в одном буфере рисует, другой показывает. Когда заканчивает рисовать, то меняет местами
            base.OnRenderFrame(args);
        }


        //метод удаления ресурсов, которые были созданы на этапе инициализации
        protected override void OnUnload()
        {
            base.OnUnload();
        }


        private void DrawMetro()
        {
            GL.LineWidth(5.0f);
            
            GL.Begin(PrimitiveType.Lines);
            for (int i = 0; i < prog.metro.coordPair.Count; i++)
            {
                GL.Color3(0.0, 0.0, 0.0);
                if (prog.metro.coordPair[i][0] >= 7 * 2 * prog.r / prog.scale && (prog.metro.coordPair[i][1] == prog.metro.coordPair[i][3]))
                {
                    GL.Color3(1.0, 1.0, 1.0);
                }
                GL.Vertex2(prog.metro.coordPair[i][0], prog.metro.coordPair[i][1]);//первая точка от линии
                GL.Vertex2(prog.metro.coordPair[i][2], prog.metro.coordPair[i][3]);//вторая точка от линии
            }
            GL.End();
            // => размеры комнаты можно сделать 2 * (0.9/w и 0.9/h), где w и h - желаемые размеры - сделано что-то подобное через scale
        }

        double max_force_dopusk = 180;
        double vecctor_force = 100;
        //рисует человечка-точку
        private void DrawPoint(List<People> list)
        {
            int cnt = 12;//количество граней для "круга"
            double da = Math.PI * 2.0 / cnt;
            

            People[] arrat = list.ToArray();
            double[] x = new double[list.Count];
            double[] y = new double[list.Count];

            for (int j = 0; j < list.Count; j++)
            {
                
                x[j] = arrat[j].X;
                y[j] = arrat[j].Y;

                if (list[j].X >= -1.1/prog.scale && list[j].X <= 1.1 / prog.scale && list[j].Y >= -1.1 / prog.scale && list[j].Y <= 1.1 / prog.scale)
                {
                    GL.Begin(PrimitiveType.TriangleFan);
                    GL.Color3(Math.Abs(arrat[j].BeforeForce - vecctor_force) / max_force_dopusk, 0.0, 0.0);
                    for (int i = 0; i <= cnt; i++)
                    {
                        x[j] += prog.r * Math.Sin(da * i);
                        y[j] += prog.r * Math.Cos(da * i);
                        GL.Vertex2(x[j], y[j]);
                    }
                    GL.End();
                }

                
            }   
        }

        //рисует барьер-точку
        private void DrawPointBarrier(List<double[]> circles)
        {
            int cnt = 12;//количество граней для "круга"
            double da = Math.PI * 2.0 / cnt;

            double[][] circles2 = circles.ToArray();
            double[] x = new double[circles2.Length];
            double[] y = new double[circles2.Length];


            for (int j = 0; j < circles2.Length; j++)
            {
                x[j] = circles2[j][0];
                y[j] = circles2[j][1];

                GL.Begin(PrimitiveType.TriangleFan);
                GL.Color3(0.0, 0.0, 0.0);
                for (int i = 0; i <= cnt; i++)
                {
                    x[j] += circles2[j][2] * Math.Sin(da * i);
                    y[j] += circles2[j][2] * Math.Cos(da * i);
                    GL.Vertex2(x[j], y[j]);
                }
                GL.End();
            }
        }
    }
}
