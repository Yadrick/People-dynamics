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
        public double w = 1200;
        public double h = 1000;
        public int countPeople = 20;
        

        public List<People> peoples = new List<People>();//будет содержать всех имеющихся людей 
        public Barrier barrier = new Barrier();


        //размеры комнаты
        public double x_right = 0.9;
        public double x_left = -0.9;
        public double y_top = 0.9;
        public double y_bot = -0.9;

        //параметр, на который делятся размеры комнаты для увеличения диапазона
        public double scale = 0.5;

        //координаты центра выхода
        //public double xx = x_right
        public double yy = 0;

        public double eps = 0.1;//для задания толщины выхода



        public double r = 0.05;
        //переменные для регулирования диапазона создания людей
        public double minValue = -0.9+0.05*2;//+r надо
        public double maxValue = 0.9-0.05*2;
        

        public void CreatePeople()//создаст людей и запихнет в массив
        {
            Random rnd = new Random();
            for (int i = 0; i < countPeople; i++)
            {
                peoples.Add(new People((rnd.NextDouble()*(maxValue-minValue) + minValue)/scale, (rnd.NextDouble() * (maxValue - minValue) + minValue) / scale));
            }
        }

        //создаю комнату
        public void CreateBarrier()
        {
            double[] stena1 = { x_right / scale, yy+eps / scale, x_right / scale, y_top / scale };
            double[] stena2 = { x_right / scale, y_top / scale, x_left / scale, y_top / scale };
            double[] stena3 = { x_left / scale, y_top / scale, x_left / scale, y_bot / scale };
            double[] stena4 = { x_left / scale, y_bot / scale, x_right / scale, y_bot / scale };
            double[] stena5 = { x_right / scale, y_bot / scale, x_right / scale, yy-eps / scale };

            barrier.coordPair.Add(stena1);
            barrier.coordPair.Add(stena2);
            barrier.coordPair.Add(stena3);
            barrier.coordPair.Add(stena4);
            barrier.coordPair.Add(stena5);
        }

       

        static void Main(string[] args)
        {
            Program prog1 = new Program();//только для размеров экрана
            

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
            prog.CreatePeople();
            prog.CreateBarrier();
            //dynamics.Displacement(prog.peoples);


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
            fps++;

            dynamics.Force(prog.peoples, prog.x_right / prog.scale, prog.yy / prog.scale);
            dynamics.Velocity(prog.peoples, prog.x_right / prog.scale, prog.yy / prog.scale);
            dynamics.ContactCheck(prog.peoples, prog.barrier, prog.x_right / prog.scale, prog.yy / prog.scale);
            dynamics.Displacement(prog.peoples);

            if (frameTime >= 1)//чтобк каждую секунду выводить коилчество кадров
            {
                Title = $"Бог Ярослав, но FPS = {fps}";
                frameTime = 0;
                fps = 0;

                
            }
            base.OnUpdateFrame(args);
        }


        //а тут уже отрисовка кадров
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);//очистка формы ДАННЫМ выше(в OnLoad) цветом 

            DrawPoint(prog.peoples);
            
            //Console.WriteLine(prog.peoples[0].Vx);

            DrawRoom();

            SwapBuffers();// в одном буфере рисует, другой показывает. Когда заканчивает рисовать, то меняет местами
            base.OnRenderFrame(args);
        }


        //метод удаления ресурсов, которые были созданы на этапе инициализации
        protected override void OnUnload()
        {
            base.OnUnload();
        }


        private void DrawRoom()
        {
            GL.LineWidth(5.0f);
            GL.Color3(0.0, 0.0, 0.0);

            GL.Begin(PrimitiveType.LineStrip);
            for (int i = 0; i < prog.barrier.coordPair.Count; i++)
            {
                GL.Vertex2(prog.barrier.coordPair[i][0], prog.barrier.coordPair[i][1]);//первая точка от линии
                GL.Vertex2(prog.barrier.coordPair[i][2], prog.barrier.coordPair[i][3]);//вторая точка от линии
            }
            GL.End();
            // => размеры комнаты можно сделать 2 * (0.9/w и 0.9/h), где w и h - желаемые размеры
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
