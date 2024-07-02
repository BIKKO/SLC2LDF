using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSLC2LDF
{
    internal class Program
    {
        static void Main(string[] args)// \help - все аргументы; \ldf - создание ldf из SLC; \SLC - создание SLC из ldf
        {
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Не указаны параметры.\nИспользуйте \\help для справки.");
                return;
            }
            if (args[0] == "\\help")
            {
                Console.WriteLine("\\ldf [Путь к SLC файлу] {Путь к CSV файлу} [Папка для сохранеия] [Имя]");
                Console.WriteLine("\\SLC [Путь к ldf файлу] [Папка для сохранеия] [Имя]");
            }
            else if (args[0] == "\\ldf")
            {
                if (args[2].Contains(".CSV"))
                {
                    string path1 = args[1];
                    string path2 = args[2];
                    string ldf = CreateFile.Create(SLC2LDF.GetTextRang(path1), CreateFile.CreateDATA(SLC2LDF.GetData(path1)), CreateFile.CreateTEGS(path2));
                    CreateFile.ToFile(args[3] + "\\" + args[4]+".ldf", ldf);
                    Console.Write("Для равершения нажмите любую кнопку....");
                    Console.ReadKey();
                }
                else
                {
                    string path1 = args[1];
                    string ldf = CreateFile.Create(SLC2LDF.GetTextRang(path1), CreateFile.CreateDATA(SLC2LDF.GetData(path1)));
                    CreateFile.ToFile(args[2] + "\\" + args[3] + ".ldf", ldf);
                    Console.Write("Для равершения нажмите любую кнопку....");
                    Console.ReadKey();
                }
            }
            else if (args[0] == "\\SLC")
            {
                string path1 = args[1];
                string f = LDF2SLC.CreateData(CreateFile.Load(path1, Type.RANG), CreateFile.GetData(CreateFile.Load(path1, Type.DATA)));
                CreateFile.ToFile(args[2] + "\\" + args[3] + ".SLC", f);
                Console.Write("Для равершения нажмите любую кнопку....");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Не изветные параметры.\nИспользуйте \\help для справки.");
            }
            //string path_SLC = Console.ReadLine();
            //string path_CSV = Console.ReadLine();
            //string file = CreateFile.Create(SLC2LDF.GetTextRang(path_SLC), CreateFile.CreateDATA(SLC2LDF.GetData(path_SLC)), CreateFile.CreateTEGS(path_CSV));
            //Console.WriteLine(file);
            //Console.ReadLine();
            
            //CreateFile.ToFile(@"C:\Users\njnji\Documents\Работа\TEST.SLC", f);
            //Console.ReadKey();
        }
    }
}
