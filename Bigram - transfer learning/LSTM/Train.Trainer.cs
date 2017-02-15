using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Program
{
    public class Trainer
    {
        public static double testAccuracy = 0, testAccuracy1 = 0,testAccuracy2=0,testAccuracy3=0;
        public static void train(DataSet X) 
        {
              if (Global.mode == "test")
            {
                testAccuracy1 = runtestIteration(X.Testing, false);
                Console.WriteLine("Epoch test  f-score: {0}", (testAccuracy1 * 100).ToString("f3"));
                Console.WriteLine("Epoch test  best f-score: {0}", (testAccuracy * 100).ToString("f3"));
                Global.swLog.WriteLine("Epoch test fscore: {0}", (testAccuracy1 * 100).ToString("f3"));
                Global.swLog.WriteLine("Epoch test best fscore: {0}", (testAccuracy * 100).ToString("f3"));
            }
              else if (Global.mode == "train")
              {
                  for (int iter = 0; iter < Global.trainIter; iter++)
                  {
                      DateTime begin = DateTime.Now;

                      Console.WriteLine("\niter: {0}", iter + 1);
                      Global.swLog.WriteLine("\niter: {0}", iter + 1);
                      
                      if(iter==0)
                      {
                          runfirsttrainIteration(X.Training, X.Testing, X.msrtraining,true, iter);
                      }
                      else
                      {
                          runtrainIteration(X.Training, X.Testing, X.msrtraining, true, iter);
                      }
                      testAccuracy1 = runtestIteration(X.Testing, false);

                      if (testAccuracy <= testAccuracy1)
                      {
                          LSTMLayer.SerializeWordembedding();
                          LSTMLayer.SerializeBigramWordembedding();

                          Global._UpLSTMLayer.saveLSTM("model\\lstmmodel.txt");
                          Global._UpLSTMLayerr.saveLSTM("model\\lstmmodelr.txt");
                          Global._GRNNLayer1.saveGRNN("model\\grnnmodel1.txt");
                          Global._GRNNLayer2.saveGRNN("model\\grnnmodel2.txt");
                          Global._GRNNLayer3.saveGRNN("model\\grnnmodel3.txt");
                          Global._GRNNLayer4.saveGRNN("model\\grnnmodel4.txt");
                          Global._feedForwardLayer.saveFFmodel("model\\feedforwardmodel.txt");
                          //Global._feedForwardLayer1.saveFFmodel("model\\feedforwardmodel1.txt");
                          testAccuracy = testAccuracy1;
                          //testAccuracy3 = testAccuracy1;
                      }

                      DateTime end = DateTime.Now;

                      Console.WriteLine("test  f-score: {0}", (testAccuracy1 * 100).ToString("f3"));
                      Console.WriteLine("test  best f-score: {0}", (testAccuracy * 100).ToString("f3"));
                     
                      Console.WriteLine("time used: {0}", end - begin);

                      Global.swLog.WriteLine("test fscore: {0}", (testAccuracy1 * 100).ToString("f3"));
                      Global.swLog.WriteLine("test best fscore: {0}", (testAccuracy * 100).ToString("f3"));
                     
                      Global.swLog.WriteLine("time used: {0}", end - begin);



                  }
              }
        }

        public static int random()
        {
            Random r = new Random();
            return r.Next(2);
        }
        public static List<List<DataStep>> shuffle(List<DataSeq> list)
        {
            Random rand = new Random();
            List<List<DataStep>> returnObject = new List<List<DataStep>>();

            for (int i = 0; i < list.Count; i++)
            {

                int length = list[i].datasteps.Count;
                int index = 0;

                while (index < length)
                {
                    List<DataStep> temp = new List<DataStep>();
                    int dim = 0;
                    //if (random() == 0)
                    //{
                    //    dim = 1;
                    //}
                    //else
                    //{
                    //    dim = 2;
                    //}
                     dim = length;
                    for (int k = 0; k < dim && index < length; k++, index++)
                    {
                        temp.Add(list[i].datasteps[index]);
                    }
                    returnObject.Add(temp);
                }
            }
            //for (int i = 0; i < list.Count; i++)
            //{
            //    for(int j=0;j<list[i].datasteps.Count;j++){
            //        returnObject.Add(list[i].datasteps[j]);
            //    }
            //}
            int n = returnObject.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                List<DataStep> value = returnObject[k];
                returnObject[k] = returnObject[n];
                returnObject[n] = value;
            }
            return returnObject;
        }
        public static double runfirsttrainIteration(List<DataSeq> X, List<DataSeq> Xtest, List<DataSeq> msrX, bool train, int inter)
        {
            List<List<DataStep>> x = new List<List<DataStep>>();
            if (train)
            {
                x = shuffle(X);//shuffle every window (point)
            }

            TrainThread runThread = new TrainThread(train);

            List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();
            List<DataStep> temp = new List<DataStep>();

            int i = 0, j = 0;
            int length = x.Count();
            while (i < length)
            {
                for (int k = 0; k < Global.nThread && i < length; k++, i++)
                {
                    ManualResetEvent mre = new ManualResetEvent(false);
                    param pa = new param();
                    pa.mre = mre;
                    pa.datastep = x[i];
                    pa.weight = 1;
                    manualEvents.Add(mre);
                    for (int m = 0; m < x[i].Count; m++)
                    {
                        temp.Add(x[i][m]);
                    }
                    ThreadPool.QueueUserWorkItem(new WaitCallback(runThread.run), pa);
                }
                WaitHandle.WaitAll(manualEvents.ToArray());
                if (train)
                {
                    UpdateWeight_rmProp(temp);
                }
                manualEvents.Clear();
                temp.Clear();
            }

            List<double> msrweight = new List<double>();
            for (int index = 0; index < msrX.Count(); index++)
            {
                msrweight.Add(0);
            }
            testAccuracy1 = runtestmsrIteration(msrX, false, msrweight);
            double e = 1 - testAccuracy1;
            double alpha = 0.5 * Math.Log((1 - e) / e);
            double num = 0;
            for (int index = 0; index < msrX.Count(); index++)
            {
                msrX[index].weight = (
                    Math.Exp(alpha) * msrweight[index]
                        + Math.Exp(-alpha) * (msrX[index].datasteps.Count - msrweight[index])
                    ) / msrX[index].datasteps.Count;
                num += msrX[index].weight;
            }

            for (int index = 0; index < msrX.Count(); index++)
            {
                msrX[index].weight = msrX[index].weight / num;

            }
            return runThread.accword / runThread.totalword;
        }
        public static double runtrainIteration(List<DataSeq> X, List<DataSeq> Xtest, List<DataSeq> msrX, bool train, int inter)
        {

            List<List<DataStep>> x = new List<List<DataStep>>();
            List<double> weights = new List<double>();

            List<List<DataStep>> x2 = shuffle(X);//shuffle every window (point)
            for (int i1 = 0; i1 < msrX.Count; i1++)
            {
                x.Add(msrX[i1].datasteps);
                weights.Add(msrX[i1].weight);
            }
            for (int i1 = 0; i1 < x2.Count; i1++)
            {
                x.Add(x2[i1]);
                weights.Add(1);
            }
             
            //List<List<DataStep>> x = new List<List<DataStep>>();
            //if (train)
            //{
            //  x=shuffle(X);//shuffle every window (point)
            //}       
    
            TrainThread runThread = new TrainThread(train);
            
             List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();
             List<DataStep> temp = new List<DataStep>();
             
             int i = 0, j = 0;
             int length=x.Count();
             while (i<length)
             {
                 //if (i % 1000 == 0)
                 //{
                 //    testAccuracy1 = runtestIteration(Xtest, false);

                 //    if (testAccuracy <= testAccuracy1)
                 //    {
                 //        LSTMLayer.SerializeWordembedding();
                 //        LSTMLayer.SerializeBigramWordembedding();

                 //        Global._UpLSTMLayer.saveLSTM("model\\lstmmodel.txt");
                 //        Global._UpLSTMLayerr.saveLSTM("model\\lstmmodelr.txt");
                 //        Global._GRNNLayer1.saveGRNN("model\\grnnmodel1.txt");
                 //        Global._GRNNLayer2.saveGRNN("model\\grnnmodel2.txt");
                 //        Global._GRNNLayer3.saveGRNN("model\\grnnmodel3.txt");
                 //        Global._GRNNLayer4.saveGRNN("model\\grnnmodel4.txt");
                 //        Global._feedForwardLayer.saveFFmodel("model\\feedforwardmodel.txt");
                 //        //Global._feedForwardLayer1.saveFFmodel("model\\feedforwardmodel1.txt");
                 //        testAccuracy = testAccuracy1;

                 //    }

                 //    Console.WriteLine("test  f-score: {0}", (testAccuracy1 * 100).ToString("f3"));
                 //    Console.WriteLine("test  best f-score: {0}", (testAccuracy * 100).ToString("f3"));
                 //    Global.swLog.WriteLine("test fscore: {0}", (testAccuracy1 * 100).ToString("f3"));
                 //    Global.swLog.WriteLine("test best fscore: {0}", (testAccuracy * 100).ToString("f3"));



                 //}
                 for (int k = 0; k < Global.nThread && i < length; k++,i++)
                 {
                     ManualResetEvent mre = new ManualResetEvent(false);
                     param pa = new param();
                     pa.mre = mre;
                     pa.datastep = x[i];
                     manualEvents.Add(mre);
                     for (int m = 0; m < x[i].Count; m++)
                     {
                         temp.Add(x[i][m]);
                     }
                     ThreadPool.QueueUserWorkItem(new WaitCallback(runThread.run), pa);
                 }
                 WaitHandle.WaitAll(manualEvents.ToArray());
                 if (train)
                 {
                     UpdateWeight_rmProp(temp);
                 }
                 manualEvents.Clear();
                 temp.Clear();
             }
             List<double> msrweight = new List<double>();
             for (int index = 0; index < msrX.Count(); index++)
             {
                 msrweight.Add(0);
             }
             testAccuracy1 = runtestmsrIteration(msrX, false, msrweight);
             double e = 1 - testAccuracy1;
             double alpha = 0.5 * Math.Log((1 - e) / e);
             double num = 0;
             for (int index = 0; index < msrX.Count(); index++)
             {
                 msrX[index].weight = (
                     Math.Exp(alpha) * msrweight[index]
                         + Math.Exp(-alpha) * (msrX[index].datasteps.Count - msrweight[index])
                     ) / msrX[index].datasteps.Count;
                 num += msrX[index].weight;
             }

             for (int index = 0; index < msrX.Count(); index++)
             {
                 msrX[index].weight = msrX[index].weight / num;

             }
       
             return runThread.accword / runThread.totalword;
        }
        public static double runtestIteration(List<DataSeq> X, bool train,List<double> xtest=null)
        {
            double _total = 0, _prTotal = 0, _correct = 0;
           
            double total = 0, accuracy = 0, recall = 0, f_score = 0;
            double _total4 = 0, _prTotal4 = 0, _correct4 = 0, accuracy4 = 0, f_score4 = 0, recall4 = 0;

            Global.threadList = new List<TrainThread>();


            TrainThread runThread = new TrainThread(train);


            List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();
            List<DataStep> temp = new List<DataStep>();
            StreamWriter sw = new StreamWriter("answer.txt");
            int k = 0;
            while (k < X.Count)
            {
                int thread = Global.nThread;

                if (k + Global.nThread >= X.Count)
                {
                    thread = X.Count - k;
                }
                if(Global.mode=="test")
                {
                    for (int i = 0; i < thread; i++)
                    {
                        param pa = new param();
                        pa.datastep = X[k + i].datasteps;
                        pa.sw = sw;
                        if (xtest != null)
                        {
                            xtest[k + i] = runThread.runtest(pa);
                        }

                        else
                        {
                            runThread.runtest(pa);
                        }

                    }
                    k += Global.nThread;
                }
                else
                {
                    Parallel.For(0, thread, i =>
                 
                    {
                        param pa = new param();
                        pa.datastep = X[k + i].datasteps;
                        pa.sw = sw;
                        if (xtest != null)
                        {
                            xtest[k + i] = runThread.runtest(pa);
                        }

                        else
                        {
                            runThread.runtest(pa);
                        }

                    });
                    k += Global.nThread;
                }
               
            }
            sw.Close();
            accuracy = runThread._correct / runThread._prTotal;
            recall = runThread._correct / runThread._total;

            accuracy4 = runThread._correct4 / runThread._prTotal4;
            recall4 = runThread._correct4 / runThread._total4;

            f_score = 2 * accuracy * recall / (accuracy + recall);
            f_score4 = 2 * accuracy4 * recall4 / (accuracy4 + recall4);
            Console.WriteLine("total windows: " + _total);
            Console.WriteLine("acc: " + (accuracy4 * 100).ToString("f3"));
            Console.WriteLine("recall: " + (recall4 * 100).ToString("f3"));
            Console.WriteLine("fscore: " + f_score4);
            Global.swLog.WriteLine("total windows: " + total);
            Global.swLog.WriteLine("acc: " + (accuracy4 * 100).ToString("f3"));
            Global.swLog.WriteLine("recall: " + (recall4 * 100).ToString("f3"));
            Global.swLog.WriteLine("fscore: " + f_score4);
            Global.swLog.Flush();
            return f_score4;
        }


        public static double runtestmsrIteration(List<DataSeq> X, bool train, List<double> xtest = null)
        {
            double _total = 0, _prTotal = 0, _correct = 0;

            double total = 0, accuracy = 0, recall = 0, f_score = 0;
            double _total4 = 0, _prTotal4 = 0, _correct4 = 0, accuracy4 = 0, f_score4 = 0, recall4 = 0;

            Global.threadList = new List<TrainThread>();


            TrainThread runThread = new TrainThread(train);


            List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();
            List<DataStep> temp = new List<DataStep>();
           
            int k = 0;
            while (k < X.Count)
            {
                int thread = Global.nThread;

                if (k + Global.nThread >= X.Count)
                {
                    thread = X.Count - k;
                }
                Parallel.For(0, thread, i =>
                //for (int i = 0; i < thread; i++)
                {
                    param pa = new param();
                    pa.datastep = X[k + i].datasteps;
                   
                    if (xtest != null)
                    {
                        xtest[k + i] = runThread.runmsrtest(pa);
                    }

                    else
                    {
                        runThread.runmsrtest(pa);
                    }

                });
                // }
                k += Global.nThread;
            }
           
            accuracy = runThread._correct / runThread._prTotal;
            recall = runThread._correct / runThread._total;

            accuracy4 = runThread._correct4 / runThread._prTotal4;
            recall4 = runThread._correct4 / runThread._total4;

            f_score = 2 * accuracy * recall / (accuracy + recall);
            f_score4 = 2 * accuracy4 * recall4 / (accuracy4 + recall4);
         
            return f_score4;
        }
        public static void UpdateWeight_rmProp(List<DataStep> x)
        {
           

            foreach (Matrix m in Global._GRNNLayer1.GetParameters(x))
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = 0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon);
                    m.W[i] +=1.5 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }
            foreach (Matrix m in Global._GRNNLayer2.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = 0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon);
                    m.W[i] += 1.5 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }
            foreach (Matrix m in Global._GRNNLayer3.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = 0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon);
                    m.W[i] += 1 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }

            foreach (Matrix m in Global._GRNNLayer4.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = 0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon);
                    m.W[i] += 1 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }
            foreach (Matrix m in Global._UpLSTMLayer.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = 0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon);
                    m.W[i] += 1 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }

            foreach (Matrix m in Global._UpLSTMLayerr.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = 0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon);
                    m.W[i] += 1 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }

            foreach (Matrix m in Global._feedForwardLayer.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = 0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon);
                    m.W[i] += 1 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }
        }
    }
}
