using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HackASMTransletor
{
    /// <summary>
    /// This handles the parseing of the asm file.
    /// </summary>
    class AsmParser
    {

        Dictionary<string, int> symbolyTable;
        Dictionary<int, string> scriptTable;
        Dictionary<string, int> variTabel;
        int varicount = 16;
        int rom;
        public AsmParser()
        {
            symbolyTable = new Dictionary<string, int>();
            scriptTable = new Dictionary<int, string>();
            variTabel = new Dictionary<string, int>();
            rom = 16;
        }
        #region Testing
        //FileReader reader = new FileReader();
        //ParseSymbols is for now just a testing thing.
        //public void ParseSymbols()
        //{
        //    string[] sortedasm = reader.ReadAsmFile(@"C:\Users\andi0137\Desktop\nand2tetris\projects\06\add\Add.asm");
        //    foreach (var item in sortedasm)
        //    {
        //        Console.WriteLine(item);
        //    }
        //}
        #endregion
        public string ConvertToBits(string valueToLookFor, int lines)
        {
            return XmlFilecomp(valueToLookFor, lines);
        }

        public Dictionary<int, string> ReadAllLines(string[] lists)
        {
            int linecount = 0;

            foreach (var item in lists)
            {
                string itemTemp = item;
                if (item.Contains("/"))
                {
                    string[] tempstring = item.Split("/");
                    itemTemp = tempstring[0];
                }
                if (!itemTemp.Contains("/"))
                {
                    if (itemTemp.Length > 1)
                    {

                        if (itemTemp.Contains('@'))
                        {
                            if (!symbolyTable.ContainsKey(itemTemp))
                            {

                                symbolyTable.Add(itemTemp.Trim(), rom);
                                rom++;

                            }
                        }
                        scriptTable.Add(linecount, itemTemp.Trim());
                        linecount++;
                    }
                }
            }
            // BTO
            Dictionary<int, string> scriptTableTemp = new Dictionary<int, string>();
            Dictionary<string, int> labelTable = new Dictionary<string, int>();

            int lineCount = 0;

            foreach (var line in scriptTable)
            {
                if (line.Value.Contains("("))
                {
                    labelTable.Add(line.Value, lineCount);
                }
                else
                {
                    scriptTableTemp.Add(lineCount, line.Value);
                    lineCount++;
                }
            }
            scriptTable = scriptTableTemp;
            symbolyTable = labelTable;
            // BTO END         
            return scriptTable;

        }
        #region Parser
        /// <summary>
        /// Main transletor
        /// </summary>
        /// <param name="valueToLookFor"></param>
        /// <returns></returns>
        private string XmlFilecomp(string valueToLookFor, int lines)
        {
            string resultString = ""; // This is final output.

            valueToLookFor = valueToLookFor.Trim();

            if (valueToLookFor.Contains('/'))
            {
                string[] test = valueToLookFor.Split('/');

                valueToLookFor = test[0].Trim();
                valueToLookFor.Trim();
            }


            //Cheacks ifs a A or C instru.
            if (valueToLookFor.StartsWith("@"))
            {
                //This is A instru               
                switch (valueToLookFor)
                {
                    case "@SP":
                        valueToLookFor = "256"; // nomale 256
                        break;
                    case "@LCL":
                        valueToLookFor = "@300";
                        //1
                        break;
                    case "@ARG":
                        valueToLookFor = "@400";
                        break;// 2
                    case "@THIS":
                        valueToLookFor = "@3000";
                        //3,
                        break;
                    case "@THAT":
                        valueToLookFor = "@3010";
                        //4,
                        break;
                    case "@SCREEN":
                        valueToLookFor = "@16384";
                        /*16384,*/
                        break;
                    case "@KBD":
                        valueToLookFor = "@24576";
                        // 24576,
                        break;
                    case "@R0":
                        valueToLookFor = "@0";
                        break;
                    default:
                        break;
                }

                if (valueToLookFor.StartsWith("@R"))
                {
                    int testbit;
                    if (int.TryParse(valueToLookFor.Replace("@R", ""), out testbit))
                    {
                        valueToLookFor = testbit.ToString();
                        //valueToLookFor = valueToLookFor.Replace("R", "");
                    }
                }

                if (symbolyTable.ContainsKey("(" + valueToLookFor.Replace("@", "") + ")"))
                {
                    valueToLookFor = "@" + (symbolyTable["(" + valueToLookFor.Replace("@", "") + ")"]).ToString();
                }
                if (valueToLookFor.StartsWith("@") && !symbolyTable.ContainsKey(valueToLookFor))
                {
                    if (valueToLookFor.Contains("RET"))
                    {
                        Console.WriteLine("sasfd");
                    }
                    int output = 0;
                    if (!int.TryParse(valueToLookFor.Replace("@", ""), out output))
                    {
                        if (!variTabel.ContainsKey(valueToLookFor))
                        {
                            variTabel.Add(valueToLookFor, varicount);
                            varicount++;
                            valueToLookFor = variTabel[valueToLookFor].ToString();
                        }
                        else if (variTabel.ContainsKey(valueToLookFor))
                        {
                            valueToLookFor = variTabel[valueToLookFor].ToString();
                        }
                    }
                }
                string temp = Convert.ToString(Int16.Parse(valueToLookFor.Replace("@", "")), 2);

                resultString = temp.PadLeft(16, '0');
                return resultString;
            }
            else if (valueToLookFor.Length >= 1 && !valueToLookFor.StartsWith("@"))
            {
                //This is then a C instru
                string insBit = "111";
                string dest = "000";
                string jump = "000";
                string comLine = "000000";
                string aBit = "0";
                //Gets the Dest and comp bits
                if (valueToLookFor.Contains("="))
                {
                    var parts = valueToLookFor.Split(new[] { '=' }, StringSplitOptions.None);
                    aBit = (parts[1].Contains('M')) ? "1" : "0";

                    if (aBit == "1")
                        comLine = xmlLookUp(parts[1], "compM");
                    else
                        comLine = xmlLookUp(parts[1], "comp");

                    dest = xmlLookUp(parts[0], "dest");
                }

                //If there is a jump instru then overwrites this.
                string comp = comLine;
                //Looks after a jump instru.
                if (valueToLookFor.Contains(";"))
                {
                    var parts = valueToLookFor.Split(new[] { ";" }, StringSplitOptions.None);
                    comp = xmlLookUp(parts[0], "comp");
                    jump = xmlLookUp(parts[1], "jump");
                }


                resultString = insBit + aBit + comp + dest + ((jump.Length < 1) ? "000" : jump);
                return resultString;

            }
            return "0000000000000000";
        }
        #endregion Parser
        #region XmlHandling
        /// <summary>
        /// Looks after the command in the xml doc
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="PlaceToLookfor"></param>
        /// <returns></returns>
        private string xmlLookUp(string thing, string PlaceToLookfor)
        {
            XDocument xml = XDocument.Load(@"C:\Users\andi0137\source\repos\HackASMTransletor\XMLFile1.xml");
            return xml.Root.Descendants(PlaceToLookfor)
            .Where(x => (string)x.Attribute("key") == thing
            .ToString())
            .FirstOrDefault().Value.ToString();
        }
        #endregion



    }
}
