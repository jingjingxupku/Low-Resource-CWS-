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
        public List<DataSeq> Training;//a dataSequence is a window'
        public List<DataSeq> msrTraining;
        public List<DataSeq> Testing;
        public List<DataSeq> Dev;
        int num = 0;
        //Matrix[] wordEmbedding = new Matrix[6000];//一个单词对应一个word embedding
        List<List<int>> trainData = new List<List<int>>();
        List<List<Matrix>> trainLabel = new List<List<Matrix>>();

        List<List<int>> msrtrainData = new List<List<int>>();
        List<List<Matrix>> msrtrainLabel = new List<List<Matrix>>();


        List<List<int>> testData = new List<List<int>>();
        List<List<Matrix>> testLabel = new List<List<Matrix>>();
       


        //指定输入输出的维度
        public DataSet()
        {
            InputDimension = Global.inputDim;
            OutputDimension = Global.outputDimension;
            getwordembedding();
            readTrainData();
            readmsrTrainData();


            readTestData();
            Training = GetTrainingData(1);
            Testing = GetTestData(1);
            msrTraining = GetmsrTrainingData(1);
            //Dev = GetDevData();
        }


        //读入wordembedding
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

       
        
        public void readmsrTrainData()
        {
            StreamReader sr = new StreamReader("msrdata/trainData.txt");
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                List<int> temp = new List<int>();
                string[] strs = line.Split(' ');
                for (int i = 0; i < strs.Length - 1; i++)
                {
                    temp.Add(Convert.ToInt32(strs[i]));
                }
                msrtrainData.Add(temp);

            }
            sr.Close();
            StreamReader sr1 = new StreamReader("msrdata/trainTag.txt");
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
                msrtrainLabel.Add(temp);

            }
            sr1.Close();
        }

        //读入训练数据
        public void readTrainData()
        {
            StreamReader sr = new StreamReader("trainData.txt");
            String line;
            while ((line = sr.ReadLine()) != null)
            {
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


        //读入测试数据
        public void readTestData()
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
                    temp.Add(j - 2 >= 0 ? trainData[i][j - 2] : num - 1);
                    temp.Add(j - 1 >= 0 ? trainData[i][j - 1] : num - 1);
                    temp.Add(trainData[i][j]);
                    temp.Add(j + 1 < trainData[i].Count ? trainData[i][j + 1] : num - 1);
                    temp.Add(j + 2 < trainData[i].Count ? trainData[i][j + 2] : num - 1);
                    tempSeq.datasteps.Add(new DataStep(temp, trainLabel[i][j],trainData[i][j]));
                    tempSeq.weight = 1;
                    index++;
                }
                result.Add(tempSeq);
            }
            return result;
        }


        public List<DataSeq> GetmsrTrainingData(double scale)
        {
            List<DataSeq> result = new List<DataSeq>();
            int index = 0;
            for (int i = 0; i < (int)msrtrainData.Count * scale; i++)
            {
                DataSeq tempSeq = new DataSeq();
                for (int j = 0; j < msrtrainData[i].Count; j++)
                {
                    List<int> temp = new List<int>();
                    temp.Add(j - 2 >= 0 ? msrtrainData[i][j - 2] : num - 1);
                    temp.Add(j - 1 >= 0 ? msrtrainData[i][j - 1] : num - 1);
                    temp.Add(msrtrainData[i][j]);
                    temp.Add(j + 1 < msrtrainData[i].Count ? msrtrainData[i][j + 1] : num - 1);
                    temp.Add(j + 2 < msrtrainData[i].Count ? msrtrainData[i][j + 2] : num - 1);
                    tempSeq.datasteps.Add(new DataStep(temp, msrtrainLabel[i][j], msrtrainData[i][j]));
                    tempSeq.weight = 1/(double)msrtrainData.Count;
                    index++;
                }
                result.Add(tempSeq);
            }
            return result;
        }


   
        public List<DataSeq> GetDevData()
        {
            List<DataSeq> result = new List<DataSeq>();
            int index = 0;
            for (int i = (int)(trainData.Count * Global.trainingScale * 0.9); i < (int)trainData.Count; i++)
            {
                DataSeq tempSeq = new DataSeq();
                for (int j = 0; j < trainData[i].Count; j++)
                {
                    List<int> temp = new List<int>();
                    temp.Add(j - 2 >= 0 ? trainData[i][j - 2] : num - 1);
                    temp.Add(j - 1 >= 0 ? trainData[i][j - 1] : num - 1);
                    temp.Add(trainData[i][j]);
                    temp.Add(j + 1 < trainData[i].Count ? trainData[i][j + 1] : num - 1);
                    temp.Add(j + 2 < trainData[i].Count ? trainData[i][j + 2] : num - 1);
                    tempSeq.datasteps.Add(new DataStep(temp, trainLabel[i][j], trainData[i][j]));
                    index++;
                }
                result.Add(tempSeq);
            }
            return result;
        }
        public List<DataSeq> GetTestData(double rate = 1)
        {

            List<DataSeq> result = new List<DataSeq>();
            int index = 0;
            for (int i = 0; i < (int)testData.Count * rate; i++)
            {
                DataSeq tempSeq = new DataSeq();
                tempSeq.sentenceindex = i;
                for (int j = 0; j < testData[i].Count; j++)
                {
                    List<int> temp = new List<int>();
                    
                    temp.Add(j - 2 >= 0 ? testData[i][j - 2] : num - 1);
                    temp.Add(j - 1 >= 0 ? testData[i][j - 1] : num - 1);
                    temp.Add(testData[i][j]);
                    temp.Add(j + 1 < testData[i].Count ? testData[i][j + 1] : num - 1);
                    temp.Add(j + 2 < testData[i].Count ? testData[i][j + 2] : num - 1);
                    tempSeq.datasteps.Add(new DataStep(temp, testLabel[i][j], testData[i][j]));
                    index++;
                }
                result.Add(tempSeq);
            }
            return result;
        }

    }
}
