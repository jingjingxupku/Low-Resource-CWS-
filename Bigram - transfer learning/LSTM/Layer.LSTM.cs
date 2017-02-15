using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Program
{
    [Serializable]
    public class LSTMLayer
    {
        int _inputDim;
        int _hiddenDim;

        Matrix _wix;
        Matrix _wih;
        Matrix _iBias;
        Matrix _wfx;
        Matrix _wfh;
        Matrix _fBias;
        Matrix _wox;
        Matrix _woh;
        Matrix _Bias;
        Matrix _wcx;
        Matrix _wch;
        Matrix _cBias;

        ActivateFunc _activate = new ActivateFunc();

        public static void SerializeWordembedding()
        {
            FileStream fs = new FileStream("model\\embedding.txt", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, Global.wordEmbedding);
            fs.Close();

        }
        public static void getSerializeWordembedding()
        {
            FileStream fs = new FileStream("model\\embedding.txt", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            Global.wordEmbedding = bf.Deserialize(fs) as Matrix[];
            fs.Close();

            

        }

        public static void SerializeBigramWord()
        {
            FileStream fs = new FileStream("model\\Bigramword.txt", FileMode.Create);

            StreamWriter sw = new StreamWriter(fs);
            String str = "";
            for (int i = 0; i < Global.bigramword.Count; i++)
            {
                str = Global.bigramword[i];
               
                sw.WriteLine(str.Trim());

            }
            sw.Close();
            fs.Close();

        }
        public static void getSerializeBigramWord()
        {
            FileStream fs = new FileStream("model\\Bigramword.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                Global.bigramword.Add(line.Trim());
            }
            sr.Close();
            fs.Close();

        }


        public static void SerializeBigramWordembedding()
        {
            FileStream fs = new FileStream("model\\Bigraembedding.txt", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, Global.BigramwordEmbedding);
            fs.Close();

        }
        public static void getSerializeBigramWordembedding()
        {
            FileStream fs = new FileStream("model\\Bigraembedding.txt", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            Global.BigramwordEmbedding = bf.Deserialize(fs) as List<Matrix>;
            fs.Close();

        }
        public void  saveLSTM(string path)
        {
            
            string Filepath = path;
            FileStream fs = new FileStream(Filepath, FileMode.Create);
            BinaryFormatter sl = new BinaryFormatter();
            sl.Serialize(fs, this);
            fs.Close();
        }

        public static LSTMLayer readLSTM(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
           BinaryFormatter bf = new BinaryFormatter();
           LSTMLayer ps = bf.Deserialize(fs) as LSTMLayer;
           fs.Close();
           return ps;
        }
        public LSTMLayer(int inputDim = Global.inputDim, int hiddenDim = Global.hiddenDim, double upbound = Global.upbound)
        {
            this._inputDim = inputDim;
            this._hiddenDim = hiddenDim;


            _wix = Matrix.newMatrix_randomorthor(_hiddenDim, _inputDim, upbound);
            _wih = Matrix.newMatrix_randomorthor(_hiddenDim, _hiddenDim, upbound);
            _iBias = new Matrix(_hiddenDim);

            _wfx = Matrix.newMatrix_randomorthor(_hiddenDim, _inputDim , upbound);
            _wfh = Matrix.newMatrix_randomorthor(_hiddenDim, _hiddenDim, upbound);
            //set forget bias to 1.0, as described here: http://jmlr.org/proceedings/papers/v37/jozefowicz15.pdf
            _fBias = Matrix.newMatrix_1(_hiddenDim, 1);

            _wox = Matrix.newMatrix_randomorthor(_hiddenDim, _inputDim , upbound);
            _woh = Matrix.newMatrix_randomorthor(_hiddenDim, _hiddenDim, upbound);
            _Bias = new Matrix(_hiddenDim);

            _wcx = Matrix.newMatrix_randomorthor(_hiddenDim, _inputDim , upbound);
            _wch = Matrix.newMatrix_randomorthor(_hiddenDim, _hiddenDim, upbound);
            _cBias = new Matrix(_hiddenDim);
        }


        public List<Matrix> activate(DataStep x, ForwdBackwdProp g)
        {
            List<int> input = x.inputs;
            List<int> bigram = x.bigram;


            Matrix final = new Matrix(Global.hiddenDim, 1);
            List<Matrix> outputs = new List<Matrix>();
            Matrix _h_tm1 = Matrix.newMatrix_0(_hiddenDim, 1);
            Matrix _s_tm1 = Matrix.newMatrix_0(_hiddenDim, 1);

            for (int i = 0; i < input.Count; i++)
            {
                //input gate
                //Matrix con = g.ConcatVectors(Global.wordEmbedding[input[i]], Global.BigramwordEmbedding[bigram[i]]);
                //Matrix inputt = g.tanhNonlin(g.Mul(_hw, con));
                Matrix inputt = Global.wordEmbedding[input[i]];
                Matrix sum0 = g.Mul(_wix, inputt);
               
                Matrix sum1 = g.Mul(_wih, _h_tm1);
                Matrix inputGate = g.sigNonlin(g.Add(g.Add(sum0, sum1), _iBias));

                //forget gate
                Matrix sum2 = g.Mul(_wfx, inputt);
                Matrix sum3 = g.Mul(_wfh, _h_tm1);
                Matrix forgetGate = g.sigNonlin(g.Add(g.Add(sum2, sum3), _fBias));

                //output gate
                Matrix sum4 = g.Mul(_wox, inputt);
                Matrix sum5 = g.Mul(_woh, _h_tm1);
                Matrix outputGate = g.sigNonlin(g.Add(g.Add(sum4, sum5), _Bias));

                //write operation on cells
                Matrix sum6 = g.Mul(_wcx, inputt);
                Matrix sum7 = g.Mul(_wch, _h_tm1);
                Matrix cellInput = g.tanhNonlin(g.Add(g.Add(sum6, sum7), _cBias));

                //compute new cell activation
                Matrix retainCell = g.Elmul(forgetGate, _s_tm1);
                Matrix writeCell = g.Elmul(inputGate, cellInput);
                Matrix cellAct = g.Add(retainCell, writeCell);

                //compute hidden state as gated, saturated cell activations
                Matrix output = g.Elmul(outputGate, g.tanhNonlin(cellAct));
                //if (i == 0)
                //{
                //    final = output;
                //}
                //else
                //{
                //    final = g.ConcatVectors(final, output);
                //}
                //final = g.Add(final, output);

                outputs.Add(output);
                //rollover activations for next iteration
                _h_tm1 = output;
                _s_tm1 = cellAct;
                //_h = g.Add(output, _h);
            }

            return outputs;
        }
        public List<Matrix> activate(List<Matrix> x, ForwdBackwdProp g)
        {
            // List<int> input = x.inputs;
            Matrix final = new Matrix(Global.hiddenDim, 1);
            List<Matrix> outputs = new List<Matrix>();
            Matrix _h_tm1 = Matrix.newMatrix_0(_hiddenDim, 1);
            Matrix _s_tm1 = Matrix.newMatrix_0(_hiddenDim, 1);

            for (int i = 0; i < x.Count; i++)
            {
                //input gate
                Matrix sum0 = g.Mul(_wix, x[i]);
                Matrix sum1 = g.Mul(_wih, _h_tm1);
                Matrix inputGate = g.sigNonlin(g.Add(g.Add(sum0, sum1), _iBias));

                //forget gate
                Matrix sum2 = g.Mul(_wfx, x[i]);
                Matrix sum3 = g.Mul(_wfh, _h_tm1);
                Matrix forgetGate = g.sigNonlin(g.Add(g.Add(sum2, sum3), _fBias));

                //output gate
                Matrix sum4 = g.Mul(_wox, x[i]);
                Matrix sum5 = g.Mul(_woh, _h_tm1);
                Matrix outputGate = g.sigNonlin(g.Add(g.Add(sum4, sum5), _Bias));

                //write operation on cells
                Matrix sum6 = g.Mul(_wcx, x[i]);
                Matrix sum7 = g.Mul(_wch, _h_tm1);
                Matrix cellInput = g.tanhNonlin(g.Add(g.Add(sum6, sum7), _cBias));

                //compute new cell activation
                Matrix retainCell = g.Elmul(forgetGate, _s_tm1);
                Matrix writeCell = g.Elmul(inputGate, cellInput);
                Matrix cellAct = g.Add(retainCell, writeCell);

                //compute hidden state as gated, saturated cell activations
                Matrix output = g.Elmul(outputGate, g.tanhNonlin(cellAct));
                //if (i == 0)
                //{
                //    final = output;
                //}
                //else
                //{
                //    final = g.ConcatVectors(final, output);
                //}
                //final = g.Add(final, output);

                outputs.Add(output);
                //rollover activations for next iteration
                _h_tm1 = output;
                _s_tm1 = cellAct;
                //_h = g.Add(output, _h);
            }

            return outputs;
        }

        public List<Matrix> GetParameters(List<DataStep> x=null)
        {
            List<Matrix> result = new List<Matrix>();
            result.Add(_wix);
            result.Add(_wih);
            result.Add(_iBias);
            result.Add(_wfx);
            result.Add(_wfh);
            result.Add(_fBias);
            result.Add(_wox);
            result.Add(_woh);
            result.Add(_Bias);
            result.Add(_wcx);
            result.Add(_wch);
            result.Add(_cBias);
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
                }
            }
            
            
            
            return result;
        }
    }
}
