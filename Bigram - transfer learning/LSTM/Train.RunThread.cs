using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;


namespace Program
{
    public class param
    {
        public ManualResetEvent mre;
        public List<DataStep> datastep;
        public StreamWriter sw;
        public double weight;
    }
    class TrainThread
    {
        
        //public List<DataSeq> _X;//a dataSequence is a window
        public Object thislock = new object();
        public bool _train;
        public double _correct = 0;
        public double _total = 0;
        public double _prTotal = 0;


        public double _correct4 = 0;
        public double _total4 = 0;
        public double _prTotal4 = 0;



        public double accword = 0;
        public double totalword = 0;
        public double acc4word = 0;
   

        public double _decayRate = Global.decayRate;
        public double _smoothEpsilon = Global.SmoothEpsilon;
        public double _gradientClipValue = Global.GradientClipValue;

        public TrainThread(bool train)
        {
            //this._X = X;
            this._train = train;
        }

        public List<Matrix> reverse(List<Matrix> x)
        {
            List<Matrix> x1 = new List<Matrix>();
            for (int i = 0; i < x.Count(); i++)
            {
                x1.Add(x[x.Count() - 1 - i]);
            }

            return x1;

        }
        public void run(object param)
        {
            param pa = (param)param;
           List<DataStep> x1 = pa.datastep;
            List<Matrix> xpro = new List<Matrix>();
            ForwdBackwdProp g = new ForwdBackwdProp(_train);
            for (int i = 0; i < x1.Count; i++)
            {
                DataStep x = x1[i];

                List<Matrix> add = new List<Matrix>();
                for (int k = 0; k < 5; k++)
                {
                    Matrix x11 = g.ConcatVectors(g.ConcatVectors(Global.wordEmbedding[x.inputs[k]], Global.BigramwordEmbedding[x.bigram[k]]), Global.BigramwordEmbedding[x.bigramlast[k]]);
                    x11 = g.ConcatVectors(g.ConcatVectors(x11, Global.BigramwordEmbedding[x.bigram1[k]]), Global.BigramwordEmbedding[x.bigramlast1[k]]);
                    add.Add(x11);
                }
                List<Matrix> returnObj2 = Global._GRNNLayer1.activate(add, g);

                List<Matrix> returnObj3 = Global._GRNNLayer2.activate(returnObj2, g);
                List<Matrix> returnObj4 = Global._GRNNLayer3.activate(returnObj3, g);
                List<Matrix> returnObj5 = Global._GRNNLayer4.activate(returnObj4, g);

                xpro.Add(returnObj5[0]);
            }

            List<Matrix> returnObj6 = Global._UpLSTMLayer.activate(xpro, g);

            List<Matrix> returnObj7 = Global._UpLSTMLayerr.activate(reverse(xpro), g);


            List<Matrix> sum = new List<Matrix>();
            for (int inde = 0; inde < returnObj6.Count(); inde++)
            {
                sum.Add(g.Add(returnObj6[inde], returnObj7[returnObj7.Count - inde - 1]));
            }
            for (int i = 0; i < returnObj6.Count; i++)
            {
                Matrix returnObj9 = Global._feedForwardLayer.Activate(sum[i], g);
                double loss = LossSoftmax.getLoss(returnObj9, x1[i].goldOutput,pa.weight);
                if (Double.IsNaN(loss) || Double.IsInfinity(loss))
                {
                    Console.WriteLine("WARNING!!!");
                    Global.swLog.WriteLine("WARNING!!!");
                    pa.mre.Set();
                    return;
                }
                LossSoftmax.getGrad(returnObj9, x1[i].goldOutput);

            }
            g.backwardProp();
            pa.mre.Set();

        }
        public int runtest(object param)
        {
            param pa = (param)param;
            List<DataStep> x1 = pa.datastep;
            int[] igold, ires, igold4, ires4, wordindeis;
            string str = "", str1 = "";
            igold = new int[x1.Count];
            ires = new int[x1.Count];
            igold4 = new int[x1.Count];
            ires4 = new int[x1.Count];
            wordindeis = new int[x1.Count];
            int index = 0, arraynum = 0;
            ForwdBackwdProp g = new ForwdBackwdProp(_train);
            int sentencenum = 0;
            while (index < x1.Count())
            {
                List<DataStep> temp = new List<DataStep>();
                int dim = 0;
                //if (Trainer.random() == 0)
                //{
                //    dim = 1;
                //}
                //else
                //{
                //    dim = 2;
                //}
                dim = x1.Count();
                for (int k = 0; k < dim && index < x1.Count(); k++, index++)
                {
                    temp.Add(x1[index]);
                }

                List<Matrix> xpro = new List<Matrix>();
                for (int i = 0; i < temp.Count; i++)
                {
                    List<Matrix> add = new List<Matrix>();

                    for (int k = 0; k < 5; k++)
                    {
                        Matrix x11 = g.ConcatVectors(g.ConcatVectors(Global.wordEmbedding[temp[i].inputs[k]], Global.BigramwordEmbedding[temp[i].bigram[k]]), Global.BigramwordEmbedding[temp[i].bigramlast[k]]);
                        x11 = g.ConcatVectors(g.ConcatVectors(x11, Global.BigramwordEmbedding[temp[i].bigram1[k]]), Global.BigramwordEmbedding[temp[i].bigramlast1[k]]);
                        add.Add(x11);
                    }
                    List<Matrix> returnObj2 = Global._GRNNLayer1.activate(add, g);

                    List<Matrix> returnObj3 = Global._GRNNLayer2.activate(returnObj2, g);
                    List<Matrix> returnObj4 = Global._GRNNLayer3.activate(returnObj3, g);
                    List<Matrix> returnObj5 = Global._GRNNLayer4.activate(returnObj4, g);

                    xpro.Add(returnObj5[0]);
                }
                List<Matrix> returnObj6 = Global._UpLSTMLayer.activate(xpro, g);

                List<Matrix> returnObj7 = Global._UpLSTMLayerr.activate(reverse(xpro), g);


                List<Matrix> sum = new List<Matrix>();
                for (int inde = 0; inde < returnObj6.Count(); inde++)
                {
                    sum.Add(g.Add(returnObj6[inde], returnObj7[returnObj7.Count - inde - 1]));
                }
                
                if (xpro.Count != x1.Count)
                {
                    Console.WriteLine("test");
                }
                for (int i = 0; i < xpro.Count; i++)
                {
                    Matrix returnObj9 = Global._feedForwardLayer.Activate(sum[i], g);

                    igold4[arraynum] = LossSoftmax.getMax(temp[i].goldOutput);
                    ires4[arraynum] = LossSoftmax.getMax(returnObj9);

                    wordindeis[arraynum] = temp[i].wordindex;
                    lock (thislock)
                    {
                       
                        pa.sw.Flush();
                        if (igold4[arraynum] == ires4[arraynum])
                        {
                            acc4word++;
                            sentencenum++;

                        }
                        totalword++;

                    }
                    arraynum++;
                }
               
                pa.sw.WriteLine();


            }

            fscore.backprocess(wordindeis, ires4);
            if(Global.mode=="test")
            {
                pa.sw.WriteLine("BOS O O");
                for (int i = 0; i < ires4.Count(); i++)
                {
                    pa.sw.WriteLine(Global.word[x1[i].wordindex] + " " + ires4[i] + " " + igold4[i]);
                }
                pa.sw.WriteLine("EOS O O");
                pa.sw.WriteLine();
            }
            List<string> res = fscore.getChunks4(ires4);
           
                str1 = fscore.calcorrect(fscore.getChunks4(igold4), res);

            

            lock (thislock)
            {
               

                string[] strs1 = str1.Split();
                _total4 += Int32.Parse(strs1[0]);
                _prTotal4 += Int32.Parse(strs1[1]);
                _correct4 += Int32.Parse(strs1[2]);
            }
            return sentencenum;

        }

        public int runmsrtest(object param)
        {
            param pa = (param)param;
            List<DataStep> x1 = pa.datastep;
            int[] igold, ires, igold4, ires4, wordindeis;
            string str = "", str1 = "";
            igold = new int[x1.Count];
            ires = new int[x1.Count];
            igold4 = new int[x1.Count];
            ires4 = new int[x1.Count];
            wordindeis = new int[x1.Count];
            int index = 0, arraynum = 0;
            ForwdBackwdProp g = new ForwdBackwdProp(_train);
            int sentencenum = 0;
            while (index < x1.Count())
            {
                List<DataStep> temp = new List<DataStep>();
                int dim = 0;
                //if (Trainer.random() == 0)
                //{
                //    dim = 1;
                //}
                //else
                //{
                //    dim = 2;
                //}
                dim = x1.Count();
                for (int k = 0; k < dim && index < x1.Count(); k++, index++)
                {
                    temp.Add(x1[index]);
                }

                List<Matrix> xpro = new List<Matrix>();
                for (int i = 0; i < temp.Count; i++)
                {
                    List<Matrix> add = new List<Matrix>();

                    for (int k = 0; k < 5; k++)
                    {
                        Matrix x11 = g.ConcatVectors(g.ConcatVectors(Global.wordEmbedding[temp[i].inputs[k]], Global.BigramwordEmbedding[temp[i].bigram[k]]), Global.BigramwordEmbedding[temp[i].bigramlast[k]]);
                        x11 = g.ConcatVectors(g.ConcatVectors(x11, Global.BigramwordEmbedding[temp[i].bigram1[k]]), Global.BigramwordEmbedding[temp[i].bigramlast1[k]]);
                        add.Add(x11);
                    }
                    List<Matrix> returnObj2 = Global._GRNNLayer1.activate(add, g);

                    List<Matrix> returnObj3 = Global._GRNNLayer2.activate(returnObj2, g);
                    List<Matrix> returnObj4 = Global._GRNNLayer3.activate(returnObj3, g);
                    List<Matrix> returnObj5 = Global._GRNNLayer4.activate(returnObj4, g);

                    xpro.Add(returnObj5[0]);
                }
                List<Matrix> returnObj6 = Global._UpLSTMLayer.activate(xpro, g);

                List<Matrix> returnObj7 = Global._UpLSTMLayerr.activate(reverse(xpro), g);


                List<Matrix> sum = new List<Matrix>();
                for (int inde = 0; inde < returnObj6.Count(); inde++)
                {
                    sum.Add(g.Add(returnObj6[inde], returnObj7[returnObj7.Count - inde - 1]));
                }

                if (xpro.Count != x1.Count)
                {
                    Console.WriteLine("test");
                }
                for (int i = 0; i < xpro.Count; i++)
                {
                    Matrix returnObj9 = Global._feedForwardLayer.Activate(sum[i], g);

                    igold4[arraynum] = LossSoftmax.getMax(temp[i].goldOutput);
                    ires4[arraynum] = LossSoftmax.getMax(returnObj9);

                    wordindeis[arraynum] = temp[i].wordindex;
                    lock (thislock)
                    {

                        //pa.sw.Flush();
                        if (igold4[arraynum] == ires4[arraynum])
                        {
                            acc4word++;
                            sentencenum++;

                        }
                        totalword++;

                    }
                    arraynum++;
                }

               // pa.sw.WriteLine();


            }

            fscore.backprocess(wordindeis, ires4);
          
            List<string> res = fscore.getChunks4(ires4);

            str1 = fscore.calcorrect(fscore.getChunks4(igold4), res);

            str = fscore.calcorrect(fscore.getChunks(igold), fscore.getChunks(ires));

            lock (thislock)
            {
                string[] strs = str.Split();
                _total += Int32.Parse(strs[0]);
                _prTotal += Int32.Parse(strs[1]);
                _correct += Int32.Parse(strs[2]);


                string[] strs1 = str1.Split();
                _total4 += Int32.Parse(strs1[0]);
                _prTotal4 += Int32.Parse(strs1[1]);
                _correct4 += Int32.Parse(strs1[2]);
            }
            return sentencenum;

        }
    }
}
