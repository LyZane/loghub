using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace Test.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcurrentDictionary<string, int> dic = new ConcurrentDictionary<string, int>();
            Parallel.For(0, 10000000, (i) => {
                try
                {
                    if (!dic.TryAdd(Path.GetRandomFileName(), 0))
                    {
                        Console.WriteLine("add fail");
                    }
                }
                catch (Exception ex)
                {
                }
            });
            Console.ReadLine();
        }
    }
}