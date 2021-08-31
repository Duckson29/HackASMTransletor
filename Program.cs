using HackASMTransletor.Parsers;
using System;
using System.Collections.Generic;
using System.IO;

namespace HackASMTransletor
{
    class Program
    {
        static void Main(string[] args)
        {

            VMParsers vmparser = new VMParsers();
            //vmparser.Parse(null);

            string[] asmTests = { "StackTest", "StackTest" };
            string[] output = vmparser.Parse("StackTest");
            List<string> outputtrimed = new List<string>();
            foreach (var item in output)
            {
                if (item.Length > 1)
                {
                    outputtrimed.Add(item);
                }
            }
            outputtrimed.Add("(END)\n@END\n0;JMP");
            foreach (var item in outputtrimed)
            {
                Console.WriteLine(item);
            }
            File.WriteAllLines($"C:\\Users\\andi0137\\Desktop\\nand2tetris\\projects\\07\\StackArithmetic\\{ asmTests[1]}\\{ asmTests[0]}.asm",outputtrimed);
            //START: This is the asm parser..

            //AsmParser pars = new AsmParser();
            //string[] temp = new FileReader().ReadAsmFile($"C:\\Users\\andi0137\\Desktop\\nand2tetris\\projects\\06\\{asmTests[1]}\\{asmTests[0]}.asm");
            //List<string> fileOut = new List<string>();
            //Dictionary<int,string> pureCode = pars.ReadAllLines(temp);
            //foreach (var item in pureCode.Values)
            //{
            //    if (item.Length > 0)
            //    {
            //        fileOut.Add(pars.ConvertToBits(item,fileOut.Count));
            //    }
            //}
            //foreach (var item in fileOut)
            //{
            //    Console.WriteLine(item);
            //}
            //File.WriteAllLines($"C:\\Users\\andi0137\\Desktop\\nand2tetris\\projects\\06\\{asmTests[1]}\\outL.hack", fileOut);

            //END: this is the end of the parser.

        }
    }
}
