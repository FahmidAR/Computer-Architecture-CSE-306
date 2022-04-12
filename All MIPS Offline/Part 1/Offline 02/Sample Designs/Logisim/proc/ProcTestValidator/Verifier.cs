using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace ProcUnitTests
{
    static class Verifier
    {
        public static bool Run(string[] codeLines, string[] resultLines, out string failure)
        {
            failure = "";
            // test result on error address
            foreach (string line in resultLines)
            {
                string[] resultFields = line.Split('\t');
                short pc = Convert.ToInt16(resultFields[0].Replace(" ", ""), 2);

                if (pc == 0xEEE)    // error
                {
                    failure = "HALT on error interrupt: 0xEEE";
                    return false;
                }
            }

            short[] regs = new short[16];
            short[] memory = new short[65536];

            Dictionary<int, string> codeBase = new Dictionary<int, string>();
            for (int i = 1; i < codeLines.Length; i++)
            {
                if (codeLines[i].Trim().Length == 0)
                    break;
                codeBase.Add(i - 1, codeLines[i].Trim());
            }
            
            for (int lineNo = 0, resultLineNo = 0; lineNo < codeBase.Count; lineNo++, resultLineNo++)
            {
                string codeLine = codeBase[lineNo];
                if (!"0000".Equals(codeLine))   // nop - nothing to do
                {
                    int opcode = int.Parse(codeLine[0].ToString(), NumberStyles.AllowHexSpecifier);
                    short r0, r1, r2, ja, jo;
                    r0 = short.Parse(codeLine[1].ToString(), NumberStyles.AllowHexSpecifier);
                    r1 = short.Parse(codeLine[2].ToString(), NumberStyles.AllowHexSpecifier);
                    r2 = short.Parse(codeLine[3].ToString(), NumberStyles.AllowHexSpecifier);
                    ja = short.Parse(codeLine.Substring(1, 3), NumberStyles.AllowHexSpecifier);
                    jo = short.Parse(codeLine.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                    sbyte imm = (sbyte)short.Parse(codeLine.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                    byte immu = (byte)short.Parse(codeLine.Substring(2, 2), NumberStyles.AllowHexSpecifier);

                    string resultLine = resultLines.ElementAt(resultLineNo);
                    string[] resultFields = resultLine.Split('\t');
                    short pc = Convert.ToInt16(resultFields[0].Replace(" ", ""), 2);
                    short reg0 = Convert.ToInt16(resultFields[1].Replace(" ", ""), 2);
                    short reg1 = Convert.ToInt16(resultFields[2].Replace(" ", ""), 2);
                    short mem = 0; 
                    if (opcode != 5)    // write memory instr, can't read memory at this time
                        mem = Convert.ToInt16(resultFields[3].Replace(" ", ""), 2);

                    if (lineNo == 0xEEE)    // error
                    {
                        failure = "HALT on error interrupt: 0xEEE";
                        return false;

                    }
                    
                    switch (opcode)
                    {
                        case 0xf:       // shli
                            regs[r0] <<= (r2 & 0xf);    // r2 represents immediate
                            if (!Test(reg0, regs[r0], lineNo, codeLine, out failure)) return false;
                            if (!Test(pc, (short)lineNo, lineNo, codeLine, out failure)) return false;
                            break;
                        case 0xc:       // addi
                            regs[r0] += imm;
                            if (!Test(reg0, regs[r0], lineNo, codeLine, out failure)) return false;
                            if (!Test(pc, (short)lineNo, lineNo, codeLine, out failure)) return false;
                            break;
                        case 0xd:       // addiu
                            regs[r0] += immu;
                            if (!Test(reg0, regs[r0], lineNo, codeLine, out failure)) return false;
                            if (!Test(pc, (short)lineNo, lineNo, codeLine, out failure)) return false;
                            break;
                        case 8:         // add
                            regs[r0] = (short)(regs[r1] + regs[r2]);
                            if (!Test(reg0, regs[r1], lineNo, codeLine, out failure)) return false;
                            if (!Test(reg1, regs[r2], lineNo, codeLine, out failure)) return false;
                            if (!Test(pc, (short)lineNo, lineNo, codeLine, out failure)) return false;
                            break;
                        case 1:
                            if (!Test(pc, (short)lineNo, lineNo, codeLine, out failure)) return false;
                            lineNo = ja - 1;    // loop will increment lineNo and make it exactly ja
                            break;
                        case 2:
                            if (!Test(pc, (short)lineNo, lineNo, codeLine, out failure)) return false;
                            if (regs[r2] == 0) lineNo += jo - 1;    // loop will increment lineNo and make it exactly (lineNo+jo)
                            break;
                        case 3:
                            if (!Test(pc, (short)lineNo, lineNo, codeLine, out failure)) return false;
                            if (!Test(reg1, regs[r2], lineNo, codeLine, out failure)) return false;
                            lineNo = regs[r2] - 1;    // loop will increment lineNo and make it exactly regs[r2]
                            break;
                        case 5:
                            memory[(ushort)regs[r1]] = regs[r2];
                            if (!Test(reg0, regs[r1], lineNo, codeLine, out failure)) return false;
                            if (!Test(reg1, regs[r2], lineNo, codeLine, out failure)) return false;
                            if (!Test(pc, (short)lineNo, lineNo, codeLine, out failure)) return false;
                            break;
                        case 4:
                            regs[r0] = memory[(ushort)regs[r1]];
                            if (!Test(reg0, regs[r1], lineNo, codeLine, out failure)) return false;
                            if (!Test(reg1, regs[r2], lineNo, codeLine, out failure)) return false;
                            if (!Test(mem, memory[(ushort)regs[r1]], lineNo, codeLine, out failure)) return false;
                            if (!Test(pc, (short)lineNo, lineNo, codeLine, out failure)) return false;
                            break;
                    }
                }
            }

            return true;
        }

        static bool Test(short result, short expected, int lineNo, string codeLine, out string failure)
        {
            if (result != expected)
            {
                failure = "line: " + lineNo + "; instr: " + codeLine + "; result: " + result + "; expected: " + expected;
                return false;
            }

            failure = "";
            return true;
        }
    }
}
