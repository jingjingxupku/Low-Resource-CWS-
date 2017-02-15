using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Program
{
    public class DataSet
    {
        public int InputDimension;
        public int OutputDimension;
        public List<DataSeq> Training;//a dataSequence is a window
        public List<DataSeq> Testing;
        public static int num = 0;
        int bigramlength = 0;
        //Matrix[] wordEmbedding = new Matrix[6000];//一个单词对应一个word embedding
        List<List<int>> trainData = new List<List<int>>();
        List<List<int>> trainDatabigram1 = new List<List<int>>();
        List<List<int>> trainDatabigram2 = new List<List<int>>();
        List<List<int>> trainDatabigram3 = new List<List<int>>();
        List<List<int>> trainDatabigram4 = new List<List<int>>();
        List<List<Matrix>> trainLabel = new List<List<Matrix>>();


        List<List<int>> testData = new List<List<int>>();

        List<List<int>> testDatabigram1 = new List<List<int>>();
        List<List<int>> testDatabigram2 = new List<List<int>>();
        List<List<int>> testDatabigram3 = new List<List<int>>();
        List<List<int>> testDatabigram4 = new List<List<int>>();
        List<List<Matrix>> testLabel = new List<List<Matrix>>();


        //指定输入输出的维度
        public DataSet()
        {
            InputDimension = Global.inputDim;
            OutputDimension = Global.outputDimension;
            newbigramembedding();
            getwordembedding();

            if (!Global.isReadBigramfeature)
            {
                readTrainData();
                readTestData();
            }
            else
            {
                readTrainunigramdata();
                readtrainbigram1();
                readtrainbigram2();
                readtrainbigram3();
                readtrainbigram4();
                readTestUnigramData();
                readtestbigram1();
                readtestbigram2();
                readtestbigram3();
                readtestbigram4();
            }

          
            
            Training = GetTrainingData(Global.trainingScale);
            Testing = GetTestData(Global.testScale);
        }

        public void getwordembedding()
        {
            StreamReader sr = new StreamReader("vector.txt");
            String line; int j = 0;
            while ((line = sr.ReadLine()) != null)
            {
                double[] temp = new double[100];
                string[] strs = line.Split(' ');
                for (int i = 0; i < 100; i++)
                {
                    temp[i] = Convert.ToDouble(strs[i]);
                }
                Global.wordEmbedding[j] = (new Matrix(temp));
                j++;

            }
            Global.length = j;
            num = j;
            sr.Close();
        }

        public static void newbigramembedding()
        {
            for (int i = 0; i < Global.bigramword.Count; i++)
            {
                Global.BigramwordEmbedding.Add(Matrix.newMatrix_random(Global.bigramdim, 1, Global.upbound));
            }
        }
        public static  void unigramembeddingtobigramembeddng()
        {
             for(int i=0;i<Global.bigramword.Count;i++)
             {
                 String str = Global.bigramword[i];
                 int index1 = indexunigram(str[0] + "");
                 int index2 = indexunigram(str[1] + "");
                 double[] temp = new double[100];

                 for (int j = 0; j < Global.wordEmbedding[index1].W.Length; j++)
                 {
                     temp[j] = (Global.wordEmbedding[index1].W[j] + Global.wordEmbedding[index2].W[j]) / 2;
                 }
                 Global.BigramwordEmbedding.Add(new Matrix(temp));
             }
             LSTMLayer.SerializeBigramWordembedding();

        }
      
        public static int indexunigram(String str)
        {
            for (int i = 0; i < Global.word.Count; i++)
            {
                if (Global.word[i].Trim() == str.Trim())
                {
                    return i;
                }
            }
            return Global.word.Count-10;
        }
        
        public int indexbigram(String str)
        {
            for (int i = 0; i < Global.bigramword.Count; i++)
            {
                if(Global.bigramworddictionary.Keys.Contains(str.Trim()))
                {
                    return Global.bigramworddictionary[str.Trim()];
                }
               
            }
            return Global.bigramword.Count-10;
        }
        public void savetrainbigram1()
        {
            FileStream fs = new FileStream("model/msrtraindatabigram.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            String str = "";
            for (int i = 0; i < trainDatabigram1.Count;i++ )
            {
                str = "";
                for(int j=0;j<trainDatabigram1[i].Count;j++)
                {
                    str += (trainDatabigram1[i][j] + " ");
                }
                sw.WriteLine(str.Trim());

            }
            sw.Close();
            fs.Close();
        }
        public void savetrainbigram2()
        {
            FileStream fs = new FileStream("model/msrtraindatabigram2.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            String str = "";
            for (int i = 0; i < trainDatabigram2.Count; i++)
            {
                str = "";
                for (int j = 0; j < trainDatabigram2[i].Count; j++)
                {
                    str += (trainDatabigram2[i][j] + " ");
                }
                sw.WriteLine(str.Trim());

            }
            sw.Close();
            fs.Close();
        }

        public void savetrainbigram3()
        {
            FileStream fs = new FileStream("model/msrtraindatabigram3.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            String str = "";
            for (int i = 0; i < trainDatabigram3.Count; i++)
            {
                str = "";
                for (int j = 0; j < trainDatabigram3[i].Count; j++)
                {
                    str += (trainDatabigram3[i][j] + " ");
                }
                sw.WriteLine(str.Trim());

            }
            sw.Close();
            fs.Close();
        }
        public void savetrainbigram4()
        {
            FileStream fs = new FileStream("model/msrtraindatabigram4.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            String str = "";
            for (int i = 0; i < trainDatabigram4.Count; i++)
            {
                str = "";
                for (int j = 0; j < trainDatabigram4[i].Count; j++)
                {
                    str += (trainDatabigram4[i][j] + " ");
                }
                sw.WriteLine(str.Trim());

            }
            sw.Close();
            fs.Close();
        }

        public void savetestbigram1()
        {
            FileStream fs = new FileStream("model/msrtestdatabigram.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            String str = "";
            for (int i = 0; i < testDatabigram1.Count; i++)
            {
                str = "";
                for (int j = 0; j < testDatabigram1[i].Count; j++)
                {
                    str += (testDatabigram1[i][j] + " ");
                }
                sw.WriteLine(str.Trim());

            }
            sw.Close();
            fs.Close();
        }
        public void savetestbigram2()
        {
            FileStream fs = new FileStream("model/msrtestdatabigram2.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            String str = "";
            for (int i = 0; i < testDatabigram2.Count; i++)
            {
                str = "";
                for (int j = 0; j < testDatabigram2[i].Count; j++)
                {
                    str += (testDatabigram2[i][j] + " ");
                }
                sw.WriteLine(str.Trim());

            }
            sw.Close();
            fs.Close();
        }

        public void savetestbigram3()
        {
            FileStream fs = new FileStream("model/msrtestdatabigram3.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            String str = "";
            for (int i = 0; i < testDatabigram3.Count; i++)
            {
                str = "";
                for (int j = 0; j < testDatabigram3[i].Count; j++)
                {
                    str += (testDatabigram3[i][j] + " ");
                }
                sw.WriteLine(str.Trim());

            }
            sw.Close();
            fs.Close();
        }

        public void savetestbigram4()
        {
            FileStream fs = new FileStream("model/msrtestdatabigram4.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            String str = "";
            for (int i = 0; i < testDatabigram4.Count; i++)
            {
                str = "";
                for (int j = 0; j < testDatabigram4[i].Count; j++)
                {
                    str += (testDatabigram4[i][j] + " ");
                }
                sw.WriteLine(str.Trim());

            }
            sw.Close();
            fs.Close();
        }

        public void readtrainbigram1()
        {
            StreamReader sr = new StreamReader("model/msrtraindatabigram.txt");
             String line; int len = 0;
             while ((line = sr.ReadLine()) != null)
             {
                 List<int> bigramindex = new List<int>();
                 string[] strs = line.Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    bigramindex.Add(Convert.ToInt32(strs[i]));

                }
                trainDatabigram1.Add(bigramindex);
            }
            sr.Close();
        }
        public void readtrainbigram2()
        {
            StreamReader sr = new StreamReader("model/msrtraindatabigram2.txt");
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {
                List<int> bigramindex = new List<int>();
                string[] strs = line.Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    bigramindex.Add(Convert.ToInt32(strs[i]));

                }
                trainDatabigram2.Add(bigramindex);
            }
            sr.Close();
        }

        public void readtrainbigram3()
        {
            StreamReader sr = new StreamReader("model/msrtraindatabigram3.txt");
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {
                List<int> bigramindex = new List<int>();
                string[] strs = line.Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    bigramindex.Add(Convert.ToInt32(strs[i]));

                }
                trainDatabigram3.Add(bigramindex);
            }
            sr.Close();
        }
        public void readtrainbigram4()
        {
            StreamReader sr = new StreamReader("model/msrtraindatabigram4.txt");
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {
                List<int> bigramindex = new List<int>();
                string[] strs = line.Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    bigramindex.Add(Convert.ToInt32(strs[i]));

                }
                trainDatabigram4.Add(bigramindex);
            }
            sr.Close();
        }
        public void readtestbigram1()
        {
            StreamReader sr = new StreamReader("model/msrtestdatabigram.txt");
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {
                List<int> bigramindex = new List<int>();
                string[] strs = line.Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    bigramindex.Add(Convert.ToInt32(strs[i]));

                }
                testDatabigram1.Add(bigramindex);
            }
            sr.Close();
        }

        public void readtestbigram2()
        {
            StreamReader sr = new StreamReader("model/msrtestdatabigram2.txt");
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {
                List<int> bigramindex = new List<int>();
                string[] strs = line.Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    bigramindex.Add(Convert.ToInt32(strs[i]));

                }
                testDatabigram2.Add(bigramindex);
            }
            sr.Close();
        }
        public void readtestbigram3()
        {
            StreamReader sr = new StreamReader("model/msrtestdatabigram3.txt");
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {
                List<int> bigramindex = new List<int>();
                string[] strs = line.Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    bigramindex.Add(Convert.ToInt32(strs[i]));

                }
                testDatabigram3.Add(bigramindex);
            }
            sr.Close();
        }
        public void readtestbigram4()
        {
            StreamReader sr = new StreamReader("model/msrtestdatabigram4.txt");
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {
                List<int> bigramindex = new List<int>();
                string[] strs = line.Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    bigramindex.Add(Convert.ToInt32(strs[i]));

                }
                testDatabigram4.Add(bigramindex);
            }
            sr.Close();
        }
        //读入训练数据
        public void readTrainData()
        {
            StreamReader sr = new StreamReader("trainData.txt");
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {

               
                len++;
            
                List<int> temp = new List<int>();
                List<int>bigramindex1=new List<int>();
                List<int> bigramindex2 = new List<int>();
                List<int> bigramindex3 = new List<int>();
                List<int> bigramindex4 = new List<int>();
                string[] strs = line.Trim().Split(' ');
                int begin = 1955;
                int sentencebegin = 1955;
                int sentenceend = 1956; 
                String big = "";
 
                for (int i = 0; i < strs.Length; i++)
                {
                    temp.Add(Convert.ToInt32(strs[i]));

                    if (i-1>=0)
                    {
                        big = Global.word[Convert.ToInt32(strs[i - 1])] + Global.word[Convert.ToInt32(strs[i])];
                        bigramindex1.Add(indexbigram(big));
                    }
                    else
                    {
                        big = Global.word[sentencebegin] + Global.word[Convert.ToInt32(strs[i])];
                        bigramindex1.Add(indexbigram(big));
                    }


                    if (i - 2 >= 0)
                    {
                        big = Global.word[Convert.ToInt32(strs[i - 2])] + Global.word[Convert.ToInt32(strs[i])];
                        bigramindex2.Add(indexbigram(big));
                    }

                    else
                    {
                        big = Global.word[sentencebegin] + Global.word[Convert.ToInt32(strs[i])];
                        bigramindex2.Add(indexbigram(big));
                    }

                    if (i + 1 < strs.Length)
                    {
                        big =  Global.word[Convert.ToInt32(strs[i])]+Global.word[Convert.ToInt32(strs[i+1])] ;
                        bigramindex3.Add(indexbigram(big));
                    }

                    else
                    {
                        big = Global.word[Convert.ToInt32(strs[i])] + Global.word[sentenceend];
                        bigramindex3.Add(indexbigram(big));
                    }
                    if (i + 2 < strs.Length)
                    {
                        big = Global.word[Convert.ToInt32(strs[i])] + Global.word[Convert.ToInt32(strs[i + 2])];
                        bigramindex4.Add(indexbigram(big));
                    }

                    else
                    {
                        big = Global.word[Convert.ToInt32(strs[i])] + Global.word[sentenceend];
                        bigramindex4.Add(indexbigram(big));
                    }
                    

                }
                trainData.Add(temp);
                trainDatabigram1.Add(bigramindex1);
                trainDatabigram2.Add(bigramindex2);
                trainDatabigram3.Add(bigramindex3);
                trainDatabigram4.Add(bigramindex4);

            }
            sr.Close();

           //LSTMLayer.SerializeBigramWordembedding();
           // LSTMLayer.SerializeBigramWord();
            savetrainbigram1();
            savetrainbigram2();
            savetrainbigram3();
            savetrainbigram4();

            StreamReader sr1 = new StreamReader("trainTag.txt");
            String line1;
            while ((line1 = sr1.ReadLine()) != null)
            {
                List<Matrix> temp = new List<Matrix>();
                string[] strs = line1.Split(' ');
                for (int i = 0; i < strs.Length - 1; i++)
                {
                    int x = Convert.ToInt32(strs[i]);
                    if (x == 1)
                    {
                        temp.Add(new Matrix(new double[] { 1, 0, 0, 0 }));
                    }
                    else if (x == 2)
                    {
                        temp.Add(new Matrix(new double[] { 0, 1, 0, 0 }));
                    }
                    else if (x == 3)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 1, 0 }));
                    }
                    else if (x == 4)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 0, 1 }));
                    }
                    //if (x == 1)
                    //{
                    //    temp.Add(new Matrix(new double[] { 1, 0 }));
                    //}
                    //else if (x == 2)
                    //{
                    //    temp.Add(new Matrix(new double[] { 0, 1 }));
                    //}
                    //else if (x == 3)
                    //{
                    //    temp.Add(new Matrix(new double[] { 0, 1 }));
                    //}
                    //else if (x == 4)
                    //{
                    //    temp.Add(new Matrix(new double[] { 1, 0 }));
                    //}
                }
                trainLabel.Add(temp);

            }
            sr1.Close();
        }


        public void readTestData()
        {
            StreamReader sr = new StreamReader("testData.txt");
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {


                len++;
                // if (len > 20) { break; }
                List<int> temp = new List<int>();
                List<int> bigramindex1 = new List<int>();
                List<int> bigramindex2 = new List<int>();
                List<int> bigramindex3 = new List<int>();
                List<int> bigramindex4 = new List<int>();
                string[] strs = line.Trim().Split(' ');
                int begin = 1955;
                int sentencebegin =1955;
                int sentenceend= 1956;
                String big = "";

                for (int i = 0; i < strs.Length; i++)
                {
                    temp.Add(Convert.ToInt32(strs[i]));

                    if (i - 1 >= 0)
                    {
                        big = Global.word[Convert.ToInt32(strs[i - 1])] + Global.word[Convert.ToInt32(strs[i])];
                        bigramindex1.Add(indexbigram(big));
                    }
                    else
                    {
                        big = Global.word[sentencebegin] + Global.word[Convert.ToInt32(strs[i])];
                        bigramindex1.Add(indexbigram(big));
                    }


                    if (i - 2 >= 0)
                    {
                        big = Global.word[Convert.ToInt32(strs[i - 2])] + Global.word[Convert.ToInt32(strs[i])];
                        bigramindex2.Add(indexbigram(big));
                    }

                    else
                    {
                        big = Global.word[sentencebegin] + Global.word[Convert.ToInt32(strs[i])];
                        bigramindex2.Add(indexbigram(big));
                    }

                    if (i + 1 < strs.Length)
                    {
                        big = Global.word[Convert.ToInt32(strs[i])] + Global.word[Convert.ToInt32(strs[i + 1])];
                        bigramindex3.Add(indexbigram(big));
                    }

                    else
                    {
                        big = Global.word[Convert.ToInt32(strs[i])] + Global.word[sentenceend];
                        bigramindex3.Add(indexbigram(big));
                    }
                    if (i + 2 < strs.Length)
                    {
                        big = Global.word[Convert.ToInt32(strs[i])] + Global.word[Convert.ToInt32(strs[i + 2])];
                        bigramindex4.Add(indexbigram(big));
                    }

                    else
                    {
                        big = Global.word[Convert.ToInt32(strs[i])] + Global.word[sentenceend];
                        bigramindex4.Add(indexbigram(big));
                    }
                    

                    // begin = Convert.ToInt32(strs[i]);

                }
                testData.Add(temp);
                testDatabigram1.Add(bigramindex1);
                testDatabigram2.Add(bigramindex2);
                testDatabigram3.Add(bigramindex3);
                testDatabigram4.Add(bigramindex4);

            }
            sr.Close();

            //LSTMLayer.SerializeBigramWordembedding();
           // LSTMLayer.SerializeBigramWord();
            savetestbigram1();
            savetestbigram2();
            savetestbigram3();
            savetestbigram4();

            StreamReader sr1 = new StreamReader("testTag.txt");
            String line1;
            while ((line1 = sr1.ReadLine()) != null)
            {
                List<Matrix> temp = new List<Matrix>();
                string[] strs = line1.Split(' ');
                for (int i = 0; i < strs.Length - 1; i++)
                {
                    int x = Convert.ToInt32(strs[i]);
                    if (x == 1)
                    {
                        temp.Add(new Matrix(new double[] { 1, 0, 0, 0 }));
                    }
                    else if (x == 2)
                    {
                        temp.Add(new Matrix(new double[] { 0, 1, 0, 0 }));
                    }
                    else if (x == 3)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 1, 0 }));
                    }
                    else if (x == 4)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 0, 1 }));
                    }
                    //if (x == 1)
                    //{
                    //    temp.Add(new Matrix(new double[] { 1, 0 }));
                    //}
                    //else if (x == 2)
                    //{
                    //    temp.Add(new Matrix(new double[] { 0, 1 }));
                    //}
                    //else if (x == 3)
                    //{
                    //    temp.Add(new Matrix(new double[] { 0, 1 }));
                    //}
                    //else if (x == 4)
                    //{
                    //    temp.Add(new Matrix(new double[] { 1, 0 }));
                    //}
                }
                testLabel.Add(temp);

            }
            sr1.Close();
        }


        public void readTrainunigramdata()
        {
            StreamReader sr = new StreamReader("trainData.txt");
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {
                len++;
                List<int> temp = new List<int>();
                string[] strs = line.Split(' ');
                for (int i = 0; i < strs.Length - 1; i++)
                {
                    temp.Add(Convert.ToInt32(strs[i]));
                }
                trainData.Add(temp);
              
            }
            sr.Close();
            StreamReader sr1 = new StreamReader("trainTag.txt");
            String line1;
            while ((line1 = sr1.ReadLine()) != null)
            {
                List<Matrix> temp = new List<Matrix>();
                string[] strs = line1.Split(' ');
                for (int i = 0; i < strs.Length - 1; i++)
                {
                    int x = Convert.ToInt32(strs[i]);
                    if (x == 1)
                    {
                        temp.Add(new Matrix(new double[] { 1, 0, 0, 0 }));
                    }
                    else if (x == 2)
                    {
                        temp.Add(new Matrix(new double[] { 0, 1, 0, 0 }));
                    }
                    else if (x == 3)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 1, 0 }));
                    }
                    else if (x == 4)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 0, 1 }));
                    }
                    //if (x == 1)
                    //{
                    //    temp.Add(new Matrix(new double[] { 1, 0 }));
                    //}
                    //else if (x == 2)
                    //{
                    //    temp.Add(new Matrix(new double[] { 0, 1 }));
                    //}
                    //else if (x == 3)
                    //{
                    //    temp.Add(new Matrix(new double[] { 0, 1 }));
                    //}
                    //else if (x == 4)
                    //{
                    //    temp.Add(new Matrix(new double[] { 1, 0 }));
                    //}
                }
                trainLabel.Add(temp);

            }
            sr1.Close();
        }
        public void readTestUnigramData()
        {
            StreamReader sr = new StreamReader("testData.txt");
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                List<int> temp = new List<int>();
                string[] strs = line.Split(' ');
                for (int i = 0; i < strs.Length - 1; i++)
                {
                    temp.Add(Convert.ToInt32(strs[i]));  
                }
                testData.Add(temp);  

            }
            sr.Close();
            StreamReader sr1 = new StreamReader("testTag.txt");
            String line1;
            while ((line1 = sr1.ReadLine()) != null)
            {
                List<Matrix> temp = new List<Matrix>();
                string[] strs = line1.Split(' ');
                for (int i = 0; i < strs.Length - 1; i++)
                {
                    int x = Convert.ToInt32(strs[i]);
                    if (x == 1)
                    {
                        temp.Add(new Matrix(new double[] { 1, 0, 0, 0 }));
                    }
                    else if (x == 2)
                    {
                        temp.Add(new Matrix(new double[] { 0, 1, 0, 0 }));
                    }
                    else if (x == 3)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 1, 0 }));
                    }
                    else if (x == 4)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 0, 1 }));
                    }
                    //if (x == 1)
                    //{
                    //    temp.Add(new Matrix(new double[] { 1, 0 }));
                    //}
                    //else if (x == 2)
                    //{
                    //    temp.Add(new Matrix(new double[] { 0, 1 }));
                    //}
                    //else if (x == 3)
                    //{
                    //    temp.Add(new Matrix(new double[] { 0, 1 }));
                    //}
                    //else if (x == 4)
                    //{
                    //    temp.Add(new Matrix(new double[] { 1, 0 }));
                    //}
                }
                testLabel.Add(temp);

            }
            sr1.Close();
        }


        //读入测试数据
       


        //将训练数据变成模型的输入
        public List<DataSeq> GetTrainingData(double scale)
        {
            List<DataSeq> result = new List<DataSeq>();
            int index = 0;
            for (int i = 0; i < (int)trainData.Count * scale; i++)
            {
                DataSeq tempSeq = new DataSeq();
                for (int j = 0; j < trainData[i].Count; j++)
                {
                    List<int> temp = new List<int>();
                    List<int> bigramtemp1 = new List<int>();
                    List<int> bigramtemp2= new List<int>();
                    List<int> bigramtemp3 = new List<int>();
                    List<int> bigramtemp4 = new List<int>();



                    bigramtemp1.Add(j - 2 >= 0 ? trainDatabigram1[i][j - 2] : 1955);
                    bigramtemp1.Add(j - 1 >= 0 ? trainDatabigram1[i][j - 1] : 1955);
                    bigramtemp1.Add(trainDatabigram1[i][j]);
                    bigramtemp1.Add(j + 1 < trainDatabigram1[i].Count ? trainDatabigram1[i][j + 1] : 1956);
                    bigramtemp1.Add(j + 2 < trainDatabigram1[i].Count ? trainDatabigram1[i][j + 2] : 1956);

                    bigramtemp2.Add(j - 2 >= 0 ? trainDatabigram2[i][j - 2] : 1955);
                    bigramtemp2.Add(j - 1 >= 0 ? trainDatabigram2[i][j - 1] : 1955);
                    bigramtemp2.Add(trainDatabigram2[i][j]);
                    bigramtemp2.Add(j + 1 < trainDatabigram2[i].Count ? trainDatabigram2[i][j + 1] : 1956);
                    bigramtemp2.Add(j + 2 < trainDatabigram2[i].Count ? trainDatabigram2[i][j + 2] : 1956);

                    bigramtemp3.Add(j - 2 >= 0 ? trainDatabigram3[i][j - 2] : 1955);
                    bigramtemp3.Add(j - 1 >= 0 ? trainDatabigram3[i][j - 1] : 1955);
                    bigramtemp3.Add(trainDatabigram3[i][j]);
                    bigramtemp3.Add(j + 1 < trainDatabigram3[i].Count ? trainDatabigram3[i][j + 1] : 1956);
                    bigramtemp3.Add(j + 2 < trainDatabigram3[i].Count ? trainDatabigram3[i][j + 2] : 1956);

                    bigramtemp4.Add(j - 2 >= 0 ? trainDatabigram4[i][j - 2] : 1955);
                    bigramtemp4.Add(j - 1 >= 0 ? trainDatabigram4[i][j - 1] : 1955);
                    bigramtemp4.Add(trainDatabigram4[i][j]);
                    bigramtemp4.Add(j + 1 < trainDatabigram4[i].Count ? trainDatabigram4[i][j + 1] : 1956);
                    bigramtemp4.Add(j + 2 < trainDatabigram4[i].Count ? trainDatabigram4[i][j + 2] : 1956);


                    temp.Add(j - 2 >= 0 ? trainData[i][j - 2] : 1955);
                    temp.Add(j - 1 >= 0 ? trainData[i][j - 1] : 1955);
                    temp.Add(trainData[i][j]);
                    temp.Add(j + 1 < trainData[i].Count ? trainData[i][j + 1] : 1956);
                    temp.Add(j + 2 < trainData[i].Count ? trainData[i][j + 2] : 1956);
                    tempSeq.datasteps.Add(new DataStep(temp, bigramtemp1, bigramtemp2, bigramtemp3, bigramtemp4, trainLabel[i][j], trainData[i][j]));
                    index++;
                }
                result.Add(tempSeq);
            }
            return result;
        }


        public List<DataSeq> GetTestData(double scale=1)
        {
            List<DataSeq> result = new List<DataSeq>();
            int index = 0;
            for (int i = 0; i < (int)testData.Count * scale; i++)
            {
                DataSeq tempSeq = new DataSeq();
                for (int j = 0; j < testData[i].Count; j++)
                {
                    List<int> temp = new List<int>();
                    List<int> bigramtemp1 = new List<int>();
                    List<int> bigramtemp2 = new List<int>();
                    List<int> bigramtemp3 = new List<int>();
                    List<int> bigramtemp4 = new List<int>();



                    bigramtemp1.Add(j - 2 >= 0 ? testDatabigram1[i][j - 2] : 1955);
                    bigramtemp1.Add(j - 1 >= 0 ? testDatabigram1[i][j - 1] : 1955);
                    bigramtemp1.Add(testDatabigram1[i][j]);
                    bigramtemp1.Add(j + 1 < testDatabigram1[i].Count ? testDatabigram1[i][j + 1] : 1956);
                    bigramtemp1.Add(j + 2 < testDatabigram1[i].Count ? testDatabigram1[i][j + 2] : 1956);

                    bigramtemp2.Add(j - 2 >= 0 ? testDatabigram2[i][j - 2] : 1955);
                    bigramtemp2.Add(j - 1 >= 0 ? testDatabigram2[i][j - 1] : 1955);
                    bigramtemp2.Add(testDatabigram2[i][j]);
                    bigramtemp2.Add(j + 1 < testDatabigram2[i].Count ? testDatabigram2[i][j + 1] : 1956);
                    bigramtemp2.Add(j + 2 < testDatabigram2[i].Count ? testDatabigram2[i][j + 2] : 1956);

                    bigramtemp3.Add(j - 2 >= 0 ? testDatabigram3[i][j - 2] : 1955);
                    bigramtemp3.Add(j - 1 >= 0 ? testDatabigram3[i][j - 1] : 1955);
                    bigramtemp3.Add(testDatabigram3[i][j]);
                    bigramtemp3.Add(j + 1 < testDatabigram3[i].Count ? testDatabigram3[i][j + 1] : 1956);
                    bigramtemp3.Add(j + 2 < testDatabigram3[i].Count ? testDatabigram3[i][j + 2] : 1956);

                    bigramtemp4.Add(j - 2 >= 0 ? testDatabigram4[i][j - 2] : 1955);
                    bigramtemp4.Add(j - 1 >= 0 ? testDatabigram4[i][j - 1] : 1955);
                    bigramtemp4.Add(testDatabigram4[i][j]);
                    bigramtemp4.Add(j + 1 < testDatabigram4[i].Count ? testDatabigram4[i][j + 1] : 1956);
                    bigramtemp4.Add(j + 2 < testDatabigram4[i].Count ? testDatabigram4[i][j + 2] : 1956);


                    temp.Add(j - 2 >= 0 ? testData[i][j - 2] :1955);
                    temp.Add(j - 1 >= 0 ? testData[i][j - 1] :1955);
                    temp.Add(testData[i][j]);
                    temp.Add(j + 1 < testData[i].Count ? testData[i][j + 1] : 1956);
                    temp.Add(j + 2 < testData[i].Count ? testData[i][j + 2] : 1956);
                    tempSeq.datasteps.Add(new DataStep(temp, bigramtemp1, bigramtemp2, bigramtemp3, bigramtemp4, testLabel[i][j], testData[i][j]));
                    index++;
                }
                result.Add(tempSeq);
            }
            return result;
        }
    }
}
