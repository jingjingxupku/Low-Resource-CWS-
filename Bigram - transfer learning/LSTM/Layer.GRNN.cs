using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Program
{
    [Serializable]
    public class GRNNLayer
    {
        int _inputDim;
        int _hiddenDim;

        //Matrix rl1w;
        //Matrix rl2w;
        //Matrix rr1w;
        //Matrix rr2w;
        Matrix _gl;
        Matrix _gr;

        Matrix hh1w;
        Matrix hh2w;

        Matrix hh3w;
        Matrix hh4w;


        Matrix _u1;
        Matrix _u2;
        Matrix _u3;

        ActivateFunc _activate = new ActivateFunc();

        public void saveGRNN(string path)
        {

            string Filepath = path;
            FileStream fs = new FileStream(Filepath, FileMode.Create);
            BinaryFormatter sl = new BinaryFormatter();
            sl.Serialize(fs, this);
            fs.Close();
        }

        public static GRNNLayer readGRNN(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            GRNNLayer ps = bf.Deserialize(fs) as GRNNLayer;
            fs.Close();
            return ps;
        }
        public GRNNLayer(int inputDim = Global.inputDim, int hiddenDim = Global.hiddenDim, double upbound = Global.upbound)
        {
            this._inputDim = inputDim;
            this._hiddenDim = hiddenDim;

            _gl = Matrix.newMatrix_random(_hiddenDim, _hiddenDim * 2, upbound);
            _gr = Matrix.newMatrix_random(_hiddenDim, _hiddenDim * 2, upbound);
            _u1 = Matrix.newMatrix_random(_hiddenDim, _hiddenDim * 3, upbound);
            _u2 = Matrix.newMatrix_random(_hiddenDim, _hiddenDim * 3, upbound);
            _u3 = Matrix.newMatrix_random(_hiddenDim, _hiddenDim * 3, upbound);
            //_wh = Matrix.newMatrix_random(_hiddenDim, _hiddenDim *2, upbound);
            //wi = Matrix.newMatrix_random(_hiddenDim, _hiddenDim , upbound);
            //wx = Matrix.newMatrix_random(_hiddenDim, _hiddenDim, upbound);
            //rl1w = Matrix.newMatrix_random(_hiddenDim, _hiddenDim, upbound);
            //rl2w = Matrix.newMatrix_random(_hiddenDim, _hiddenDim, upbound);
            //rr1w = Matrix.newMatrix_random(_hiddenDim, _hiddenDim, upbound);
            //rr2w = Matrix.newMatrix_random(_hiddenDim, _hiddenDim, upbound);
            hh1w = Matrix.newMatrix_random(_hiddenDim, _hiddenDim * 2, upbound);
            hh2w = Matrix.newMatrix_random(_hiddenDim, _hiddenDim * 3, upbound);

            hh3w = Matrix.newMatrix_random(_hiddenDim, _hiddenDim * 3, upbound);
            hh4w = Matrix.newMatrix_random(_hiddenDim, _hiddenDim * 3, upbound);
        }


        public List<Matrix> activate(List<Matrix> input, ForwdBackwdProp g)
        {

            List<Matrix> temp = new List<Matrix>();
            for (int i = 1; i < input.Count; i++)
            {
                Matrix concanate = g.ConcatVectors(input[i - 1], input[i]);
                //Matrix rl = g.sigNonlin(g.Add(g.Mul(rl1w, input[i - 1]), g.Mul(rl2w, input[i])));
                Matrix rl = g.sigNonlin(g.Mul(_gl, concanate));
                Matrix rr = g.sigNonlin(g.Mul(_gr, concanate));
                //Matrix rr = g.sigNonlin(g.Add(g.Mul(rr1w, input[i - 1]), g.Mul(rr2w, input[i])));
                //Matrix hh = g.tanhNonlin(g.Add(g.Mul(wi, input[i - 1]), g.Mul(wx, input[i])));
                Matrix hh = g.tanhNonlin(g.Mul(hh1w, g.ConcatVectors(g.Elmul(rl, input[i - 1]), g.Elmul(rr, input[i]))));

                //Matrix hhh = g.ConcatVectors(g.ConcatVectors(hh, input[i - 1]), input[i]);
                //Matrix zh1 = g.sigNonlin(g.Mul(hh2w, hhh));
                //Matrix zh2 = g.sigNonlin(g.Mul(hh3w, hhh));
                //Matrix zh3 = g.tanhNonlin(g.Mul(hh4w, hhh));
                //Matrix z1=g.Exp()


                Matrix concanate1 = g.ConcatVectors(g.ConcatVectors(hh, input[i - 1]), input[i]);

                Matrix z1 = g.sigNonlin(g.Mul(_u1, concanate1));
                Matrix z2 = g.sigNonlin(g.Mul(_u2, concanate1));
                Matrix z3 = g.sigNonlin(g.Mul(_u3, concanate1));


                //Matrix zl = g.SumDivid1(zh1, zh2, zh3);
                // Matrix zm = g.SumDivid2(zh1, zh2, zh3);
                //Matrix zr = g.SumDivid3(zh1, zh2, zh3);

                Matrix output = g.Add(g.Add(g.Elmul(z1, hh), g.Elmul(z2, input[i - 1])), g.Elmul(z3, input[i]));

                temp.Add(output);
            }


            return temp;
        }


        public List<Matrix> GetParameters(List<DataStep> x = null)
        {
            List<Matrix> result = new List<Matrix>();
            result.Add(_gl);
            result.Add(_gr);
            result.Add(_u1);
            result.Add(_u2);
            result.Add(_u3);
            //result.Add(_wh);
            //result.Add(rl1w);
            //result.Add(rl2w);
            //result.Add(rr1w);
            //result.Add(rr2w);
            result.Add(hh1w);
            result.Add(hh2w);
            result.Add(hh3w);
            result.Add(hh4w);
            //result.Add(_gl);
            //result.Add(_gl);
            if (x != null)
            {
                for (int j = 0; j < x.Count; j++)
                {
                    for (int i = 0; i < x[j].inputs.Count; i++)
                    {
                        result.Add(Global.wordEmbedding[x[j].inputs[i]]);

                    }

                    for (int i = 0; i < x[j].bigram.Count; i++)
                    {
                        result.Add(Global.BigramwordEmbedding[x[j].bigram[i]]);

                    }
                    for (int i = 0; i < x[j].bigram1.Count; i++)
                    {
                        result.Add(Global.BigramwordEmbedding[x[j].bigram1[i]]);

                    }
                    for (int i = 0; i < x[j].bigramlast.Count; i++)
                    {
                        result.Add(Global.BigramwordEmbedding[x[j].bigramlast[i]]);

                    }
                    for (int i = 0; i < x[j].bigramlast1.Count; i++)
                    {
                        result.Add(Global.BigramwordEmbedding[x[j].bigramlast1[i]]);

                    }
                }
            }
            return result;
        }
    }
}
