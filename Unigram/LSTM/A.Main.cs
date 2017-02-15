using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MathNet.Numerics.Distributions;

namespace Program
{
    class Program
    {


        static void readfourword()
        {
            StreamReader sr = new StreamReader("idoim.txt", Encoding.UTF8);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                Global.fourword.Add(line.ToString().Trim());
            }
            sr.Close();
        }
        static void readword()
        {
            StreamReader sr = new StreamReader("word.txt", Encoding.Default);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                Global.word.Add(line.ToString());
            }
        }
        static void Main(string[] args)
        {
            //if (args[0] == "train")
            //{
            //    Global.mode = "train";
            //}
            //else
            //{
            //    Global.mode = "test";
            //}
            //if (args[1] == "read")
            //{
            //    Global.isRead = 1;
            //}
            //else
            //{
            //    Global.isRead = 0;
            //}
            Console.WriteLine("Choose running mode: 1. training, 2. testing");
            string mode=Console.ReadLine();
            if (mode == "1")
            {
                Global.mode = "train";
            }
            else if (mode == "2")
            {
                Global.mode = "test";
            }

            Console.WriteLine("Choose reading mode: 1. read saved model, 2. randomly initialize model");
            string read = Console.ReadLine();
            if (read == "1")
            {
                Global.isRead = 1;
            }
            else if (read == "2")
            {
                Global.isRead = 0;
            }
            Global.randn = new Normal(); 
            DataSet X = new DataSet();
            readfourword();
            readword();


            if (Global.isRead==1)
            {
                Global._UpLSTMLayer = LSTMLayer.readLSTM("model\\lstmmodel.txt");
                Global._UpLSTMLayerr = LSTMLayer.readLSTM("model\\lstmmodelr.txt");
                Global._GRNNLayer1 = GRNNLayer.readGRNN("model\\grnnmodel1.txt");
                Global._GRNNLayer2 = GRNNLayer.readGRNN("model\\grnnmodel2.txt");
                Global._GRNNLayer3 = GRNNLayer.readGRNN("model\\grnnmodel3.txt");
                Global._GRNNLayer4 = GRNNLayer.readGRNN("model\\grnnmodel4.txt");

                Global._feedForwardLayer = FeedForwardLayer.readFF("model\\feedforwardmodel.txt");

               // Global._feedForwardLayer1 = FeedForwardLayer.readFF("model\\feedforwardmodel1.txt");
                LSTMLayer.getSerializeWordembedding();
            }

            else
            {
               // LSTMLayer.getSerializeWordembedding();
                Global._GRNNLayer1 = new GRNNLayer();
                Global._GRNNLayer2 = new GRNNLayer();
                Global._GRNNLayer3 = new GRNNLayer();
                Global._GRNNLayer4 = new GRNNLayer();
                Global._UpLSTMLayer = new LSTMLayer();
                Global._UpLSTMLayerr = new LSTMLayer();
                Global._feedForwardLayer = new FeedForwardLayer();
            }
            
            Trainer.train(X);

            Global.swLog.Close();

            Console.Read();

        }
    }
}
