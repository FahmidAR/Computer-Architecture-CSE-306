using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MyProcAssembler
{
    class Program
    {
        static void Main(string[] args)
        {
            File.WriteAllText(args[0] + ".out", Compiler.Compile(File.ReadLines(args[0])));
        }
    }
}
