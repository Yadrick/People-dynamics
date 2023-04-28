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
        public int countPeople = 50;
        public int countPeopleInput = 10;

        public List<People> peoples = new List<People>();//будет содержать всех имеющихся людей 
        public List<People> peoplesInput = new List<People>();//будет содержать всех заходящих людей 


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

        public void CreatePeopleMetroStatic()//создаст людей и запихнет в массив (спавнятся в квадрате  minValue:maxValue)
        {
            Random rnd = new Random();
            int halfCountPeople = countPeople / 2;
            int secondHalfCountPeople = countPeople - halfCountPeople;
            int[] massiv = new int[10]; // допустим, всего 10 выходов будет возможными. в rnd.next(a,b) определяется количество выходов

            int a = 0;//для рандома

            //добавляю людей, вышедших из поезда снизу
            for (int i = 0; i < halfCountPeople; i++)
            {
                // a - условно отвечает за номер двери при выходе из выгона
                a = rnd.Next(0, 4);
                //+ r*rnd.Next(0,2) - смещение на r вправо
                double x = (a * ((-5 * 2 * r)) - 4 * 2 * r + Math.Pow(-1, rnd.Next(1, 2)) * r * rnd.Next(0, 2)) / scale;

                massiv[a]++;

                double y = -(9 + (massiv[a] - 1)) * 2 * r / scale;


                peoples.Add(new People(x, y));
            }

            //обнуляю массив, чтобы добавить людей, вышедших из поезда сверзу
            for (int i = 0; i < massiv.Length; i++)
            {
                massiv[i] = 0;
            }

            for (int i = 0; i < secondHalfCountPeople; i++)
            {
                a = rnd.Next(0, 4);
                //+ Math.Pow(-1, rnd.Next(1, 2))*r*rnd.Next(0,2) - смещение на r вправо-влево
                double x = (a * ((-5 * 2 * r)) - 4 * 2 * r + Math.Pow(-1, rnd.Next(1, 2)) * r * rnd.Next(0, 2)) / scale;

                massiv[a]++;


                double y = (9 + (massiv[a] - 1)) * 2 * r / scale;

                peoples.Add(new People(x, y));
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

            double[] stena1 = { x_left / scale, y_top / scale, x_right / scale, y_top / scale };
            double[] stena2 = { x_left / scale, y_bot / scale, x_right / scale, y_bot / scale };

            double[] stena3 = { x_right / scale, y_top / scale, x_right / scale, (y_top - 8 * (r)) / scale };
            double[] stena4 = { x_right / scale, y_bot / scale, x_right / scale, (y_bot + 8 * (r)) / scale };

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
            //2 спуска и 1 подъем
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
        public double timeWorking = 0;//время работы программы
        private int fps = 0;
        Program prog = new Program();
        DynamicPeoples dynamics = new DynamicPeoples();
        

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.On; // ограничение на количество фпс: такое, сколько экран может показать (Update метод вызывается меньше раз)
        }

        //инициализация ресурсов
        protected override void OnLoad()
        {
            //prog.CreatePeopleMetro();
            prog.CreatePeopleMetroStatic();
            prog.LoadingPeopleInsideMetro(0);//-0.5);
            prog.CreateMetro(0); // если передать 0, то будет два подъема, если 1, то будет 2 спуска
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

        //проверка на нажатие клавиш, изменение положения объектов в пространстве, мат операции
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            frameTime += args.Time;
            timeWorking += args.Time;//время работы программы
            fps++;

            //первая цифра в Force отвечает за кол-во эскалаторов на подъем/спуск
            dynamics.Force(0, prog.peoples, prog.peoplesInput, prog.xxMetro / prog.scale, prog.yyMetro / prog.scale, prog.xx1Metro/prog.scale, prog.yy1Metro / prog.scale, prog.xx2Metro / prog.scale, prog.yy2Metro / prog.scale);
            dynamics.WallContact(prog.peoples, prog.metro);
            dynamics.Velocity(prog.peoples, prog.peoplesInput, prog.xxMetro / prog.scale, prog.yyMetro / prog.scale, prog.xx1Metro / prog.scale, prog.yy1Metro / prog.scale, prog.xx2Metro / prog.scale, prog.yy2Metro / prog.scale, (prog.r) / prog.scale);
            
            //dynamics.ContactCheckMetro(prog.peoples, prog.metro, prog.x_right / prog.scale, prog.yy / prog.scale);
            dynamics.Displacement(prog.peoples, timeWorking, 0); // цифра отвечает тому же, что и в Force

            //для входящего потока
            dynamics.DisplacementInput(prog.peoplesInput);

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

            DrawMetro();

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
                
                GL.Begin(PrimitiveType.TriangleFan);
                GL.Color3(Math.Abs(arrat[j].Pressure), 0.0, 0.0);
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
}
