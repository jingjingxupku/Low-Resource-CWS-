using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MathNet.Numerics.Distributions;

namespace Program
{
    class Global
    {
        public static string optimizer = "rmprop";//rmprop, sgd
        public const int nThread = 15;
        public const double trainingScale =1;
        public const double testScale = 1;
        public static string mode = "train";//  "train" or "test";
        public static Boolean isReadBigramfeature = false;//  "train" or "test";
        public const int inputDim = 220;
        public const int hiddenDim = 220;
        public const int bigramdim = 30;
        public const int outputDimension = 4;//B, M(middle), E, S(single)
        public const double learningRate0 = 0.0001;//0.001
        public static double decayRate = 0.95;

        public const double upbound = 0.01;
   
        public const int reportEveryNthEpoch = 1;
        public const int trainIter = 50;

        public static double SmoothEpsilon = 0.0001;
        public static double GradientClipValue = 5;
        public static double L2Reg = 0.000001; // L2 regularization strength
        public static int updatetimes = 0;
        public static int isRead = 0;


        //global variable
       
        //public static LSTMLayer _LSTMLayer;
        //public static LSTMLayer _LSTMLayerr;
        public static LSTMLayer _UpLSTMLayer;
        public static LSTMLayer _UpLSTMLayerr;
        public static FeedForwardLayer _feedForwardLayer;
        //public static FeedForwardLayer _feedForwardLayer1;
        public static GRNNLayer _GRNNLayer1;
        public static GRNNLayer _GRNNLayer2;
        public static GRNNLayer _GRNNLayer3;
        public static GRNNLayer _GRNNLayer4;
        public static StreamWriter swLog = new StreamWriter("log.txt");
        public static List<TrainThread> threadList;
        public static Normal randn;
        public static Matrix[] wordEmbedding = new Matrix[6000];
        public static int length;

        public static List<Matrix> BigramwordEmbedding = new List<Matrix>();
        //public static int Bigramlength;
        public static List<string> bigramword = new List<string>();
        public static Dictionary<string, int> bigramworddictionary = new Dictionary<string, int>();

        public static List<string> fourword = new List<string>();
     
        public static List<string> word = new List<string>();

        public static List<List<string>> traingold = new List<List<string>>();
    }
}
