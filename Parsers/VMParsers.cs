using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackASMTransletor.Parsers
{
    class VMParsers
    {
        string[] asmTests = { "BasicTest", "BasicTest" };

        Dictionary<string, int> tempSettor = new Dictionary<string, int> {
            {"Temp0",5 },{"Temp1",6 },{"Temp2",7 },{"Temp3",8 },{"Temp4",9 },{"Temp5",10 },{"Temp6",11 },{"Temp7",12 },
        };

        public VMParsers()
        {

        }
        /// <summary>
        /// This handlse the parseing of the .vm file
        /// </summary>
        public string[] Parse(string vmcode)
        {
            string[] vmLines = new FileReader().ReadAsmFile($"C:\\Users\\andi0137\\Desktop\\nand2tetris\\projects\\07\\StackArithmetic\\{ vmcode}\\{ vmcode}.vm");
            List<string> testParse = new List<string>();
            List<string> filOut = new List<string>();
            string tempLine = "";
            int index = 0;

            foreach (var item in vmLines)
            {
                tempLine = "";
                string[] temp = item.Split(" ");
                if (temp.Length != 1)
                {
                    switch (temp[1].ToLower())
                    {
                        case "constant":
                            tempLine += ConstantParser(temp[2]) + "\n";
                            break;
                        case "static":
                            tempLine += staticParser((int.Parse(temp[2]) + 16).ToString() + "\n");
                            break;
                        case "local":
                            tempLine += LocalParser() + "\n";
                            break;
                        case "this":
                            tempLine += ThisParser(temp[2]) + "\n";
                            break;
                        case "that":
                            tempLine += ThatParser(temp[2]) + "\n";
                            break;
                        case "arg":
                            tempLine += ArgParser(temp[2]) + "\n";
                            break;
                        default:
                            break;
                    }
                }
                switch (temp[0].ToLower())
                {
                    case "pop":
                        tempLine += PopParser() + "\n";
                        break;
                    case "push":
                        tempLine += PushParser() + "\n";
                        break;
                    case "add":
                        tempLine += AddParser() + "\n";
                        break;
                    case "sub":
                        tempLine += SubParser() + "\n";
                        break;
                    case "neg":
                        tempLine += negParser() + "\n";
                        break;
                    case "eq":
                        tempLine += EQParser(index) + "\n";
                        index++;
                        break;
                    case "gt":
                        tempLine += GTParser(index) + "\n";
                        index++;
                        break;
                    case "lt":
                        tempLine += LTParser(index) + "\n";
                        index++;
                        break;
                    case "and":
                        tempLine += AndParser() + "\n";
                        break;
                    case "or":
                        tempLine += OrParser() + "\n";
                        break;
                    case "not":
                        tempLine += NotParser() + "\n";
                        break;
                    default:
                        break;
                }


                filOut.Add(tempLine);

            }



            return filOut.ToArray();
        }
        string AddParser()
        {
            string temp = "@SP\nAM=M-1\nD=M\nA=A-1\nM=M+D";
            return temp;
        }
        string NotParser()
        {
            string temp = "@SP\nA=M-1\nM=!M";
            return temp;
        }
        string SubParser()
        {
            string temp = "@SP\nAM=M-1\nD=M\nA=A-1\nM=M-D";
            return temp;
        }
        string PopParser()
        {
            string temp = "@R13\nM=D\n@SP\nAM=M-1\nD=M\n@R13\nA=M\nM=D";
            return temp;
        }
        string PushParser()
        {
            string temp = "@SP\nA=M\nM=D\n@SP\nM=M+1";
            return temp;
        }
        string ConstantParser(string value)
        {
            string temp = $"@{value.Trim()}\nD=A";
            return temp;
        }
        string LocalParser()
        {
            string temp = "@LCL\nD=M\n@0\nD=D+A\n@R13\nM=D\n@SP\nAM=M-1\nD=M\n@R13\nA=M\nM=D";
            return temp;
        }
        string TempParser(string tempindex)
        {
            //tempindex is the temp mem sectors eg. temp 6 ==  tempSettor.key("temp6") = ram[11]
            string temp = $"@R5\nD=M\n@{tempindex.Trim()}\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1";
            return temp;
        }
        string OrParser()
        {
            string temp = "@SP\nAM=M-1\nD=M\nA=A-1\nM=M|D";
            return temp;
        }
        string staticParser(string value)
        {
            string temp = $"D=0\n@{value.Trim()}\nD=M\n"; // Resvere somplace in ram > R15 and translete the arg to the R place
            return temp;
        }
        string EQParser(int input)
        {
            int index = input;
            string temp = $"@SP\nAM=M-1\nD=M\n@SP\nAM=A-1\nD=M-D\n@FALSE{index}\nD;JNE\n@SP\nA=M-1\nM=-1\n@CONTINUE{index}\n0;JMP\n(FALSE{index})\n@SP\nA=M-1\nM=0\n(CONTINUE{index})";
            temp = $"@SP\nAM=M-1\nD=M\n@SP\nAM=M-1\nD=M-D\n@COMP.{index}.TRUE\n" +
                $"D;JEQ\n@COMP.{index}.FALSE\n0;JMP\n(COMP.{index}.TRUE)\n@SP\nA=M\n" +
                $"M=-1\n@SP\nM=M+1\n@COMP.{index}.END\n0;JMP\n(COMP.{index}.FALSE)\n" +
                $"@SP\nA=M\nM=0\n@SP\nM=M+1\n(COMP.{index}.END)";
            return temp;
        }
        string LTParser(int input)
        {
            int index = input;
            string temp = $"@SP\nAM=M-1\nD=M\nA=A-1\nD=M-D\n@FALSE{index}\nD;JGE\n@SP\nA=M-1\nM=-1\n@CONTINUE{index}\n0;JMP\n(FALSE{index})\n@SP\nA=M-1\nM=0\n(CONTINUE{index})";
            temp = $"@SP\nAM=M-1\nD=M\n@SP\nAM=M-1\nD=M-D\n@COMP.{index}.TRUE\n" +
                $"D;JLT\n@COMP.{index}.FALSE\n0;JMP\n(COMP.{index}.TRUE)\n@SP\nA=M\n" +
                $"M=-1\n@SP\nM=M+1\n@COMP.{index}.END\n0;JMP\n(COMP.{index}.FALSE)\n" +
                $"@SP\nA=M\nM=0\n@SP\nM=M+1\n(COMP.{index}.END)";
            return temp;
        }
        string GTParser(int input)
        {
            int index = input;
            string temp = $"@SP\nAM=M-1\nD=M\nA=A-1\nD=M-D\n@FALSE{index}\nD;JLE\n@SP\nA=M-1\nM=-1\n@CONTINUE{index}\n0;JMP\n(FALSE{index})\n@SP\nA=M-1\nM=0\n(CONTINUE{index})";
            temp = $"@SP\nAM=M-1\nD=M\n@SP\nAM=M-1\nD=M-D\n@COMP.{index}.TRUE\n"+
                $"D;JGT\n@COMP.{index}.FALSE\n0;JMP\n(COMP.{index}.TRUE)\n" +
                $"@SP\nA=M\nM=-1\n@SP\nM=M+1\n@COMP.{index}.END\n0;JMP\n"+
                $"(COMP.{index}.FALSE)\n@SP\nA=M\nM=0\n@SP\nM=M+1\n(COMP.{index}.END)";       
            return temp;
        }
        string ThisParser(string value)
        {
            string temp = $"@THIS\nD=M\n@{value.Trim()}\nD=D+A";
            return temp;
        }
        string ThatParser(string value)
        {
            string temp = $"@THAT\nD=M\n@{value.Trim()}\nD=D+A";
            return temp;

        }
        string ArgParser(string value)
        {
            string temp = $"@Arg\nD=M\n@{value.Trim()}\nD=D+A";
            return temp;

        }
        string negParser()
        {
            string temp = "D=0\n@SP\nA=M-1\nM=D-M";
            temp = "@SP\nAM=M-1\nM=-M\n@SP\nM=M+1";
            return temp;
        }
        string AndParser()
        {
            string temp = "@SP\nAM=M-1\nD=M\nA=A-1\nM=M&D";
            temp = "@SP\nAM=M-1\nD=M\n@SP\nAM=M-1\nM=D&M\n@SP\nM=M+1";
            return temp;
        }
    }
}
