using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Program
{
    public class LossSoftmax
    {
        //get grad: http://ufldl.stanford.edu/wiki/index.php/Softmax%E5%9B%9E%E5%BD%92
        public static void getGrad(Matrix logProbs, Matrix goldTags)
        {
            int goldIndex = GetGoldTag(goldTags);
            Matrix probs = GetSoftmaxProb(logProbs);
            for (int i = 0; i < probs.W.Length; i++)
            {
                logProbs.gradW[i] = probs.W[i];
            }
            logProbs.gradW[goldIndex] -= 1;
        }
        public static int getMax(Matrix logProbs)
        {
            int log1 = 0;
            double maxval = Double.NegativeInfinity;
            for (int i = 0; i < logProbs.W.Length; i++)
            {
                if (logProbs.W[i] > maxval)
                {
                    maxval = logProbs.W[i];
                    log1 = i;
                }
            }
            return log1;
        }
        public static int correctOrNot(Matrix logProbs, Matrix goldTags)
        {
            int log1 = 0, traget1 = 0;
            double maxval = Double.NegativeInfinity;
            for (int i = 0; i < logProbs.W.Length; i++)
            {
                if (logProbs.W[i] > maxval)
                {
                    maxval = logProbs.W[i];
                    log1 = i;
                }
            }
            maxval = Double.NegativeInfinity;
            for (int i = 0; i < goldTags.W.Length; i++)
            {
                if (goldTags.W[i] > maxval)
                {
                    maxval = goldTags.W[i];
                    traget1 = i;
                }
            }
           // Console.WriteLine(log1 + " " + traget1);
           // Global.swLog.WriteLine(log1 + " " + traget1);
            //Global.swLog.Flush();
            if (log1 == traget1) return 1;
            else
                return 0;
        }

        //get softmax loss: http://ufldl.stanford.edu/wiki/index.php/Softmax%E5%9B%9E%E5%BD%92
        public static double getLoss(Matrix logProbs, Matrix goldTags)
        {
            int targetIndex = GetGoldTag(goldTags);
            Matrix probs = GetSoftmaxProb(logProbs);
            double loss = -Math.Log(probs.W[targetIndex]);
            return loss;
        }


        public static Matrix GetSoftmaxProb(Matrix logprobs)
        {
            Matrix probs = new Matrix(logprobs.W.Length);
 
            double maxval = Double.NegativeInfinity;
            for (int i = 0; i < logprobs.W.Length; i++)
            {
                if (logprobs.W[i] > maxval)
                {
                    maxval = logprobs.W[i];
                }
            }
            double sum = 0;
            for (int i = 0; i < logprobs.W.Length; i++)
            {
                probs.W[i] = Math.Exp(logprobs.W[i]); //all inputs to exp() are non-positive
                sum += probs.W[i];
            }
            for (int i = 0; i < probs.W.Length; i++)
            {
                probs.W[i] /= sum;
            }
            return probs;
        }

        static int GetGoldTag(Matrix targetOutput)
        {
            for (int i = 0; i < targetOutput.W.Length; i++)
            {
                if (targetOutput.W[i] == 1.0)
                {
                    return i;
                }
            }
            throw new Exception("no target index selected");
        }

    }
}
