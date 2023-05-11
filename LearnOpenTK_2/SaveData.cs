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
        public int countVagon { get; set; }                             //кол-во вагонов в поезде
        public int countTrain { get; set; }                             //кол-во поездов
        public int countPeoppleOutput { get; set; }                     // кол-во выходящих людей
        public double Velocity { get; set; }                            // скорость людей
        public int countEscalator { get; set; }                         // кол-во эскалаторов на подьем
        public int[] countPeopleLeft { get; set; } = new int[5];         // кол-во людей неуспевших выйти
        public double timePreviousTrain { get; set; }                   // время_прошедшее_с_предыдущего_поезда
        public double timeFirstOut { get; set; }                        // время выхода на эскалатор первого человека
        public double timeOut { get; set; }                             // время выхода всех людей
        public double maxPressure { get; set; }                         // максимальное давление

        public double[] forceDispersion{get;set;} = new double[5];      // дисперсия модуля силы. Будет 5 поездов
        public int countSufferer { get; set; }                          // число пострадавших

        public string textForceDispersion { get; set; }                 //сюда запишу данные из массива forceDispersion
        public string textСountPeopleLeft { get; set; }                 //сюда запишу данные из массива countPeopleLeft

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
                        writer.WriteLine("людей_выходят;скорость;кол-во_поездов;кол-во_вагонов;кол-во_эскалаторов_на_подъем;время_прошедшее_с_предыдущего_поезда;кол-во_людей_неуспевших_выйти;время_выхода_первого_человека;время_выхода_всех_людей;delta_t;дисперсия_модуля_силы;число_пострадавших");
                    }

                    for (int i = 0; i < forceDispersion.Length; i++)
                    {
                        textForceDispersion += (int)forceDispersion[i];
                        textForceDispersion += "|";
                        textСountPeopleLeft += countPeopleLeft[i];
                        textСountPeopleLeft += "|";
                    }

                    writer.WriteLine($"{countPeoppleOutput};{Velocity};{countTrain};{countVagon};{countEscalator};{timePreviousTrain};{textСountPeopleLeft};{timeFirstOut};{timeOut};{timeOut-timeFirstOut};{textForceDispersion};{countSufferer}");
                    Console.WriteLine("записал");
                }

                

                flag = true;
            }
            

            
        }
    }
}
