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
        public int countVagon { get; set; }             //кол-во вагонов в поезде
        public int countPeoppleOutput { get; set; }     // кол-во выходящих людей
        public double Velocity { get; set; }            // скорость людей
        public int countEscalator { get; set; }         // кол-во эскалаторов на подьем
        public double timeFirstOut { get; set; }        // время выхода на эскалатор первого человека
        public double timeOut { get; set; }             // время выхода всех людей
        public double maxPressure { get; set; }         // максимальное давление
        public int countSufferer { get; set; }          // число пострадавших
        public string isolation = "_______________________________________________________________";// чтобы разграничивать эксперементы в файле
        public bool flag = false;
        public bool flag2 = false;
        

        string path = @"D:\POLITECH\4 course\Diplom\data.txt";


        public void Save()
        {
            if (flag == false)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        if (reader.Peek() != -1)
                        {
                        }
                        else
                        {
                            flag2 = true;
                        }
                    }
                }
                catch (Exception)
                {
                    flag2 = true;
                }

                using (StreamWriter writer = new StreamWriter(path, true)) // true - добавлять данные в конец файла, false - перезаписывать
                {
   
                    if (flag2)
                    {
                        writer.WriteLine("людей_выходят;скорость;кол-во_вагонов;кол-во_эскалаторов_на_подъем;время_выхода_первого_человека;время_выхода_всех_людей;delta_t;максимальное_давление_оказываемое_на_человека;число_пострадавших");
                    }

                    writer.WriteLine($"{countPeoppleOutput};{Velocity};{countVagon};{countEscalator};{timeFirstOut};{timeOut};{timeOut-timeFirstOut};{maxPressure};{countSufferer}");


                    Console.WriteLine("записал");
                }

                

                flag = true;
            }
            

            
        }
    }
}
