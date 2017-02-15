using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Program
{
    [Serializable]
    public class FeedForwardLayer
    {
        Matrix _w;
        Matrix _b;
        public void saveFFmodel(string path)
        {

            string Filepath = path;
            FileStream fs = new FileStream(Filepath, FileMode.Create);
            BinaryFormatter sl = new BinaryFormatter();
            sl.Serialize(fs, this);
            fs.Close();
        }

        public static FeedForwardLayer readFF(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            FeedForwardLayer ps = bf.Deserialize(fs) as FeedForwardLayer;
            fs.Close();
            return ps;
        }
        public FeedForwardLayer(int inputDimension=Global.hiddenDim, int outputDimension=Global.outputDimension, double upbound=Global.upbound)
        {
            _w = Matrix.newMatrix_random(outputDimension, inputDimension, upbound);
            _b = new Matrix(outputDimension);
        }

        public Matrix Activate(Matrix input, ForwdBackwdProp g)
        {
                Matrix sum = g.Add(g.Mul(_w, input), _b);
                Matrix returnObj = sum;
                return returnObj;
        }
        public Matrix Activate_1(Matrix input, ForwdBackwdProp g)
        {
            Matrix sum = g.Add(g.Mul(_w, input), _b);
            Matrix returnObj = g.tanhNonlin(sum);
            return returnObj;
        }

        public List<Matrix> GetParameters()
        {
            List<Matrix> result = new List<Matrix>();
            result.Add(_w);
            result.Add(_b);
            return result;
        }
    }
}
