using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics_peoples
{
    public class SaveData
    {
        public int countPeoppleInput { get; set; }      //кол-во входящих в метро людей
        public int countPeoppleOutput { get; set; }     // кол-во выходящих людей
        public double Velocity { get; set; }            // скорость людей
        public int countEscalator { get; set; }         // кол-во эскалаторов на подьем
        public double timeFirstOut { get; set; }        // время выхода на эскалатор первого человека
        public double timeHalfOut { get; set; }         // время выхода половины всех людей
        public double maxPressure { get; set; }         // максимальное давление
        public int countSufferer { get; set; }          // число пострадавших
        public string isolation = "_______________________________________________________________";// чтобы разграничивать эксперементы в файле
        public bool flag = false;
        

        string path = @"D:\POLITECH\4 course\Diplom\data.txt";


        public void Save()
        {
            if (flag == false)
            {
                using (StreamWriter writer = new StreamWriter(path, true)) // true - добавлять данные в конец файла, false - перезаписывать
                {
                    writer.WriteLine($"людей заходят;{countPeoppleInput}");
                    writer.WriteLine($"людей выходят;{countPeoppleOutput}");
                    writer.WriteLine($"скорость;{Velocity}");
                    writer.WriteLine($"кол-во эскалаторов на подъем;{countEscalator}");
                    writer.WriteLine($"время выхода первого человека;{timeFirstOut}");
                    writer.WriteLine($"время выхода половины всех людей;{timeHalfOut}");
                    writer.WriteLine($"максимальное давление, оказываемое на человека;{maxPressure}");
                    writer.WriteLine($"число пострадавших;{countSufferer}");
                    writer.WriteLine($"{isolation}");

                    Console.WriteLine("записал");
                }

                flag = true;
            }
            

            
        }
    }
}
