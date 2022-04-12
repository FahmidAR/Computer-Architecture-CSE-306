using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProcAssembler
{
    public static class Compiler
    {
        static Dictionary<string, int> _labels = new Dictionary<string, int>();
        static int _instrAddress;

        public static string Compile(IEnumerable<string> lines)
        {
            List<string> copy = new List<string>(lines);


            // first pass - process labels
            ProcessLabels(copy);

            return GenerateCode(copy);
        }

        static void ProcessLabels(IEnumerable<string> lines)
        {
            _instrAddress = 0;
            foreach (string line in lines)
            {
                if (line.Length == 0)   // empty line
                    continue;

                string[] tokens = line.Split(new char[] { ' ' }, 2);
                switch (tokens[0])
                {
                    case ":":   // label
                        _labels.Add(tokens[1], _instrAddress);
                        break;
                    case ";":   // comment
                        break;
                    case "":    // empty line, skip
                        break;
                    case "li":    // pseudo  op, expands to 3 ops
                        _instrAddress += 4;
                        break;
                    case "push":    // pseudo op, expanded to 2 ops
                        _instrAddress += 2;
                        break;
                    case "pop":     // pseudo op, expanded to 2 ops
                        _instrAddress += 2;
                        break;
                    case "ret":
                        _instrAddress += 3;
                        break;
                    case "call":
                        _instrAddress += 7;
                        break;
                    default:
                        _instrAddress++;
                        break;
                }
            }
        }
        static string GenerateCode(IEnumerable<string> lines)
        {
            _instrAddress = 0;
            StringBuilder output = new StringBuilder("v2.0 raw").AppendLine();

            foreach (string line in lines)
            {
                if (line.Length == 0)
                    continue;

                string[] tokens = line.Split(new char[] { ',', ' ' });

                try
                {
                    switch (tokens[0])
                    {
                        case "nop":
                            output.Append("0000");
                            break;
                        case "j":
                            output.AppendFormat("1{0,3:X3}",
                                _labels.ContainsKey(tokens[1]) ? _labels[tokens[1]] : int.Parse(tokens[1]));
                            break;
                        case "jz":
                            output.AppendFormat("2{0,2:X2}{1,1:X1}",
                                _labels.ContainsKey(tokens[2]) 
                                    ? _labels[tokens[2]] - _instrAddress
                                    : int.Parse(tokens[2]) & 0xff, 
                                int.Parse(tokens[1]));
                            break;
                        case "jr":
                            output.AppendFormat("300{0,1:X1}", int.Parse(tokens[1]));
                            break;
                        case "lm":
                            output.AppendFormat("4{0,1:X1}{1,1:X1}0",
                                int.Parse(tokens[1]), int.Parse(tokens[2]));
                            break;
                        case "sm":
                            output.AppendFormat("50{0,1:X1}{1,1:X1}",
                                int.Parse(tokens[1]), int.Parse(tokens[2]));
                            break;
                        case "add":
                            output.AppendFormat("8{0,1:X1}{1,1:X1}{2,1:X1}",
                                int.Parse(tokens[1]), int.Parse(tokens[2]), int.Parse(tokens[3]));
                            break;
                        case "and":
                            output.AppendFormat("9{0,1:X1}{1,1:X1}{2,1:X1}",
                                int.Parse(tokens[1]), int.Parse(tokens[2]), int.Parse(tokens[3]));
                            break;
                        case "or":
                            output.AppendFormat("A{0,1:X1}{1,1:X1}{2,1:X1}",
                                int.Parse(tokens[1]), int.Parse(tokens[2]), int.Parse(tokens[3]));
                            break;
                        case "shl":
                            output.AppendFormat("B{0,1:X1}{1,1:X1}{2,1:X1}",
                                int.Parse(tokens[1]), int.Parse(tokens[2]), int.Parse(tokens[3]));
                            break;
                        case "addi":
                            output.AppendFormat("C{0,1:X1}{1,2:X2}",
                                int.Parse(tokens[1]), int.Parse(tokens[2]) & 0xFF);
                            break;
                        case "addiu":
                            output.AppendFormat("D{0,1:X1}{1,2:X2}",
                                int.Parse(tokens[1]), int.Parse(tokens[2]));
                            break;
                        case "shli":
                            output.AppendFormat("F{0,1:X1}{1,2:X2}",
                                int.Parse(tokens[1]), int.Parse(tokens[2]));
                            break;
                        case "li":    // pseudo op
                            // TODO: if reg is not null, need to set to null first 
                            int reg = int.Parse(tokens[1]);
                            int imm = int.Parse(tokens[2]);
                            output.AppendFormat("8{0,1:X1}DD",        // add reg,14,14 - 14 assumed zero reg
                                reg).AppendLine();
                            output.AppendFormat("C{0,1:X1}{1,2:X2}",        // addi
                                reg, imm >> 8).AppendLine();
                            output.AppendFormat("F{0,1:X1}08",              // shli
                                reg).AppendLine();
                            output.AppendFormat("D{0,1:X1}{1,2:X2}",        // addi
                                reg, imm & 0xFF);
                            _instrAddress += 3;
                            break;
                        case "push":    // pseudo op
                            output.Append("CFFF").AppendLine();       // addi sp,-1
                            output.AppendFormat("50F{0,1:X1}",              // sm sp,r
                                int.Parse(tokens[1]));
                            _instrAddress++;
                            break;
                        case "pop":    // pseudo op
                            output.AppendFormat("4{0,1:X1}F0",              // lm r,sp
                                int.Parse(tokens[1])).AppendLine();
                            output.Append("CF01");                    // addi sp,1
                            _instrAddress++;
                            break;
                        case "ret":
                            output.Append("4EF0").AppendLine();             // lm 14,sp
                            output.Append("CF01").AppendLine();       // addi sp,1
                            output.Append("300E");                    // jr 14
                            _instrAddress += 2;
                            break;
                        case "call":
                            // li
                            int addr = _instrAddress + 7;
                            output.Append("8EDD").AppendLine();        // add 15,14,14 - 14 assumed zero reg
                            output.AppendFormat("CE{0,2:X2}", addr >> 8).AppendLine();
                            output.Append("FE08").AppendLine();
                            output.AppendFormat("DE{0,2:X2}", addr & 0xFF).AppendLine();
                            // push 14
                            output.Append("CFFF").AppendLine();       // addi sp,-1
                            output.Append("50FE").AppendLine();
                            // jump
                            output.AppendFormat("1{0,3:X3}", 
                                _labels.ContainsKey(tokens[1]) ? _labels[tokens[1]] : int.Parse(tokens[1]));
                            _instrAddress += 6;
                            break;
                        case ":":       // label
                            continue;
                        case ";":       // comment
                            continue;
                        case "":        // empty line
                            continue;
                        default:
                            throw new Exception("Unknown op: " + tokens[0]);
                    }
                    output.AppendLine();
                    _instrAddress++;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + ". Line: " + line, e);
                }
            }
            return output.ToString();
        }
    }
}
