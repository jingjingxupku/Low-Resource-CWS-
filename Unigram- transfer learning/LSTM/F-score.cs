﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace Program
{
   
    class fscore
    {
        public static Dictionary<string, int> dic = new Dictionary<string, int>(); 

        public static bool Isfour(String str1)
        {
            for (int i = 0; i < Global.fourword.Count; i++)
            {
                if (Global.fourword[i].Trim() == str1.Trim())
                {
                    return true;
                }
            }
            return false;
        }


       public static void writer()
        {
            StreamWriter sw = new StreamWriter("wrong.txt");
            foreach (var item in dic)
            {
                sw.WriteLine(item.Key + "::::::"+item.Value);
            }
            sw.Close();

        }
        public static string getlabel(int index)
       {
           if (index == 0)
               return "S";
           else if(index==1)
               return "B";
           else if (index==2)
               return "M";
           else 
               return "E";
       }
        public static Boolean illeagal(string str)
        {
            if (str.Contains("SM") || str.Contains("SE") || str.Contains("BB") || str.Contains("BS") ||
                str.Contains("MB") || str.Contains("MS") || str.Contains("EM") || str.Contains("EE"))
                return true;
            else
                return false;
        }
        public static void count(int [] tagindex, int [] goldindex)
        {
            int i = 0;string word1="",word2="";
            for(i=0;i<tagindex.Length-2;i++)
            {
                word1 = getlabel(tagindex[i]) + "" + getlabel(tagindex[i + 1])+"" + getlabel(tagindex[i + 2]);
                word2 = getlabel(goldindex[i]) + "" + getlabel(goldindex[i + 1])+"" + getlabel(goldindex[i + 2]);
                if (word1 != word2 && illeagal(word1))
                {
                    string str = word1 + "----" + word2;
                    if(dic.ContainsKey(str))
                    {
                        dic[str] += 1;
                    }
                    else
                    {
                        dic.Add(str, 0);
                    }
                    
                }

            }
        }
      
        public static void backprocess(int[] wordindex, int[] res1,int [] igold4)
        {
            string temp = "";
            string x = "";
            for (int i = 0; i < res1.Length;i++ )
            {
                x += Global.word[wordindex[i]];
            }
           

            for (int i = 0; i < res1.Length - 3; i++)
            {
                temp = Global.word[wordindex[i]] + Global.word[wordindex[i + 1]] + Global.word[wordindex[i + 2]] + Global.word[wordindex[i + 3]];
                if (Isfour(temp))
                {
                    res1[i] = 1;
                    res1[i + 1] = 2;
                    res1[i + 2] = 2;
                    res1[i + 3] = 3;
                }
            }
        }
        public static string calcorrect(List<string> gold2, List<string> res1)
        {

            int goldnum = gold2.Count;
            int resnum = res1.Count;
            int correct = 0;
            for (int i = 0; i < resnum; i++)
            {
                if (gold2.Contains(res1[i]))
                {
                    correct++;
                }
            }
            return goldnum + " " + resnum + " " + correct;

        }


     
        public static List<string> getChunks4(int[] iresult)
        {
            List<string> chunk = new List<string>();
            int i = 0; string temp = " ";

            while (i < iresult.Length)
            {
                if (iresult[i] == 0)
                {
                    if (!temp.Trim().Equals(""))
                        chunk.Add(temp.Trim());
                    temp = "" + i;
                    chunk.Add(temp.Trim());
                    temp = " ";
                    i++;
                    continue;
                }
                else if (iresult[i] == 1)
                {
                    if (!temp.Trim().Equals(""))
                        chunk.Add(temp.Trim());
                    temp = (i + " ");
                    i++;
                    continue;
                }
                else if (iresult[i] == 2)
                {
                    temp += (i + " ");
                    i++;
                    continue;
                }
                else
                {
                    temp += (i + " ");
                    chunk.Add(temp.Trim());
                    temp = " ";
                    i++;
                    continue;
                }

            }
            if (!temp.Trim().Equals(""))
                chunk.Add(temp.Trim());


            //for (i = 0; i < chunk.Count; i++)
            //{
            //    Console.Write(chunk[i]+"--");
            //    Global.swLog.WriteLine(chunk[i] + "--");
            //}
            //Console.WriteLine();
            //Global.swLog.Flush();
            return chunk;
        }

   
        public static List<string> getChunks(int[] iresult)
        {
            List<string> chunk = new List<string>();
            int i = 0;
            while (i < iresult.Length)
            {
                string temp = "";
                while (i < iresult.Length)
                {
                    if (iresult[i] == 0)
                    {
                        temp += (" " + i);
                        chunk.Add(temp.Trim());
                        temp = "";
                        i++;
                        break;
                    }
                    else
                    {
                        temp += (" " + i);
                        i++;
                    }

                }
                if (i == iresult.Length && iresult[i - 1] == 1)
                {
                    chunk.Add(temp.Trim());
                }
            }
            //for (i = 0; i < chunk.Count; i++)
            //{
            //    Console.Write(chunk[i]+"--");
            //    Global.swLog.WriteLine(chunk[i] + "--");
            //}
            //Console.WriteLine();
            //Global.swLog.Flush();
            return chunk;
        }
    }
}
