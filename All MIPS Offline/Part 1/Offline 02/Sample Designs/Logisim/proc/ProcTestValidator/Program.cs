using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace ProcUnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string codeFile in Directory.GetFiles(".", "test-*-code.txt"))
            {
                Console.Write("Testing " + codeFile + "... ");
                string resultFile = codeFile + ".out";
                string failure;

                if (Verifier.Run(File.ReadAllLines(codeFile), File.ReadAllLines(resultFile), out failure))
                    Console.WriteLine("OK");
                else
                {
                    Console.WriteLine("FAILED. Details: " + failure);
                }
            }
            Console.WriteLine("Done");
        }
    }
}
