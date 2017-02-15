using System;
using System.Collections.Generic;


namespace Program
{
    public class ForwdBackwdProp
    {
        public bool applyBackprop;
        public List<Runnable> Backprop;
    
        public ForwdBackwdProp(bool applyBackprop)
        {
            this.applyBackprop = applyBackprop;
            this.Backprop = new List<Runnable>();
        }

        public void backwardProp()
        {
            for (int i = Backprop.Count - 1; i >= 0; i--)
            {
                Backprop[i].Run();
            }
        }

        public Matrix ConcatVectors(Matrix m1, Matrix m2)
        {
            if (m1.nCol > 1 || m2.nCol > 1)
            {
                throw new Exception("Expected column vectors");
            }
            Matrix returnObj = new Matrix(m1.nRow + m2.nRow,1);

            int loc = 0;
            for (int i = 0; i < m1.W.Length; i++)
            {
                returnObj.W[loc] = m1.W[i];
                //returnObj.gradW[loc] += m1.gradW[i];
                //returnObj.rmPropStepCache[loc] += m1.rmPropStepCache[i];
                loc++;
            }
            for (int i = 0; i < m2.W.Length; i++)
            {
                returnObj.W[loc] = m2.W[i];
               // returnObj.gradW[loc] += m2.gradW[i];
                //returnObj.rmPropStepCache[loc] += m2.rmPropStepCache[i];
                loc++;
            }
            if (this.applyBackprop)
            {
                Runnable bp = new Runnable();
                bp.Run = delegate()
                {
                    int index0 = 0;
                    for (int i = 0; i < m1.W.Length; i++)
                    {
                       // m1.W[i] += returnObj.W[index0];
                        m1.gradW[i] += returnObj.gradW[index0];
                        //m1.rmPropStepCache[i] += returnObj.rmPropStepCache[index0];
                        index0++;
                    }
                    for (int i = 0; i < m2.W.Length; i++)
                    {
                        //m2.W[i] += returnObj.W[index0];
                        m2.gradW[i] += returnObj.gradW[index0];
                       // m2.rmPropStepCache[i] += returnObj.rmPropStepCache[index0];
                        index0++;
                    }
                };

                Backprop.Add(bp);
            }
            return returnObj;
        }
     
        public Matrix Exp(Matrix m)
        {
            Matrix returnObj = new Matrix(m.nRow, m.nCol);
            int n = m.W.Length;
            for (int i = 0; i < n; i++)
            {
                returnObj.W[i] = Math.Exp(m.W[i]);
            }

            if (this.applyBackprop)
            {
                Runnable bp = new Runnable();
                bp.Run = delegate()
                {
                    for (int i = 0; i < n; i++)
                    {
                        m.gradW[i] += Math.Exp(m.W[i]) * returnObj.gradW[i];
                    }

                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        public Matrix sigNonlin(Matrix m)
        {
            Matrix returnObj = new Matrix(m.nRow, m.nCol);
            int n = m.W.Length;
            for (int i = 0; i < n; i++)
            {
                returnObj.W[i] = ActivateFunc.sigForward(m.W[i]);
            }

            if (this.applyBackprop)
            {
                Runnable bp = new Runnable();
                bp.Run = delegate()
                {
                    for (int i = 0; i < n; i++)
                    {
                        m.gradW[i] += ActivateFunc.sigBackward(m.W[i]) * returnObj.gradW[i];
                    }

                };
                Backprop.Add(bp);
            }
            return returnObj;
        }


        public Matrix tanhNonlin(Matrix m)
        {
            Matrix returnObj = new Matrix(m.nRow, m.nCol);
            int n = m.W.Length;
            for (int i = 0; i < n; i++)
            {
                returnObj.W[i] = ActivateFunc.tanhForward(m.W[i]);
            }
            if (this.applyBackprop)
            {
                Runnable bp = new Runnable();
                bp.Run = delegate()
                {
                    for (int i = 0; i < n; i++)
                    {
                        m.gradW[i] += ActivateFunc.tanhBackward(m.W[i]) * returnObj.gradW[i];
                    }

                };
                Backprop.Add(bp);
            }
            return returnObj;
        }

        public Matrix Mul(Matrix m1, Matrix m2)
        {
            if (m1.nCol != m2.nRow)
                throw new Exception("matrix dimension mismatch");

            int m1Rows = m1.nRow;
            int m1Cols = m1.nCol;
            int m2Cols = m2.nCol;
            Matrix returnObj = new Matrix(m1Rows, m2Cols);
            int outcols = m2Cols;
            for (int i = 0; i < m1Rows; i++)
            {
                int m1Col = m1Cols * i;
                for (int j = 0; j < m2Cols; j++)
                {
                    double dot = 0;
                    for (int k = 0; k < m1Cols; k++)
                    {
                        dot += m1.W[m1Col + k] * m2.W[m2Cols * k + j];
                    }
                    returnObj.W[outcols * i + j] = dot;
                }
            }

            if (this.applyBackprop)
            {
                Runnable bp = new Runnable();
                bp.Run = delegate()
                {
                    for (int i = 0; i < m1.nRow; i++)
                    {
                        int outcol = outcols * i;
                        for (int j = 0; j < m2.nCol; j++)
                        {
                            double b = returnObj.gradW[outcol + j];
                            for (int k = 0; k < m1.nCol; k++)
                            {
                                m1.gradW[m1Cols * i + k] += m2.W[m2Cols * k + j] * b;
                                m2.gradW[m2Cols * k + j] += m1.W[m1Cols * i + k] * b;
                            }
                        }
                    }

                };
                Backprop.Add(bp);
            }
            return returnObj;
        }

        public Matrix Add(Matrix m1, Matrix m2)
        {
            if (m1.nRow != m2.nRow || m1.nCol != m2.nCol)
                throw new Exception("matrix dimension mismatch");
            
            Matrix returnObj = new Matrix(m1.nRow, m1.nCol);
            for (int i = 0; i < m1.W.Length; i++)
            {
                returnObj.W[i] = m1.W[i] + m2.W[i];
            }

            if (this.applyBackprop)
            {
                Runnable bp = new Runnable();
                bp.Run = delegate()
                {
                    for (int i = 0; i < m1.W.Length; i++)
                    {
                        m1.gradW[i] += returnObj.gradW[i];
                        m2.gradW[i] += returnObj.gradW[i];
                    }
                };
                Backprop.Add(bp);
            }

            return returnObj;
        }


        public Matrix Elmul(Matrix m1, Matrix m2)
        {
            if (m1.nRow != m2.nRow || m1.nCol != m2.nCol)
            {
                throw new Exception("matrix dimension mismatch");
            }
            Matrix returnObj = new Matrix(m1.nRow, m1.nCol);
            for (int i = 0; i < m1.W.Length; i++)
            {
                returnObj.W[i] = m1.W[i] * m2.W[i];
            }
            if (this.applyBackprop)
            {
                Runnable bp = new Runnable();
                bp.Run = delegate()
                {
                    for (int i = 0; i < m1.W.Length; i++)
                    {
                        m1.gradW[i] += m2.W[i] * returnObj.gradW[i];
                        m2.gradW[i] += m1.W[i] * returnObj.gradW[i];
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        public Matrix SumDivid1(Matrix m1, Matrix m2, Matrix m3)
        {
            if (m1.nRow != m2.nRow || m1.nCol != m2.nCol)
            {
                throw new Exception("matrix dimension mismatch");
            }
            Matrix returnObj = new Matrix(m1.nRow, m1.nCol);
            double sum = 0.0, divided;
            for (int i = 0; i < m1.W.Length; i++)
            {
                sum = (m1.W[i] + m2.W[i] + m3.W[i]);
                returnObj.W[i] = m1.W[i] / sum;
                //m2.W[i] = m2.W[i] / sum;
                //m3.W[i] = m3.W[i] / sum;
            }
            if (this.applyBackprop)
            {
                Runnable bp = new Runnable();
                bp.Run = delegate()
                {
                    for (int i = 0; i < m1.W.Length; i++)
                    {
                        divided = sum * sum;
                        m1.gradW[i] += returnObj.gradW[i] * (m2.W[i] + m3.W[i]) / divided;
                        m2.gradW[i] += returnObj.gradW[i] * -m1.W[i] / divided;
                        m3.gradW[i] += returnObj.gradW[i] * -m1.W[i] / divided;
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }

        public Matrix SumDivid2(Matrix m1, Matrix m2, Matrix m3)
        {
            if (m1.nRow != m2.nRow || m1.nCol != m2.nCol)
            {
                throw new Exception("matrix dimension mismatch");
            }
            Matrix returnObj = new Matrix(m2.nRow, m2.nCol);
            double sum = 0.0, divided;
            for (int i = 0; i < m1.W.Length; i++)
            {
                sum = (m1.W[i] + m2.W[i] + m3.W[i]);
                returnObj.W[i] = m2.W[i] / sum;
                //m2.W[i] = m2.W[i] / sum;
                //m3.W[i] = m3.W[i] / sum;
            }
            if (this.applyBackprop)
            {
                Runnable bp = new Runnable();
                bp.Run = delegate()
                {
                    for (int i = 0; i < m1.W.Length; i++)
                    {
                        divided = sum * sum;
                        m1.gradW[i] += returnObj.gradW[i] * -m2.W[i] / divided;
                        m2.gradW[i] += returnObj.gradW[i] * (m1.W[i] + m3.W[i]) / divided;
                        m3.gradW[i] += returnObj.gradW[i] * -m2.W[i] / divided;
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        public Matrix SumDivid3(Matrix m1, Matrix m2, Matrix m3)
        {
            if (m1.nRow != m2.nRow || m1.nCol != m2.nCol)
            {
                throw new Exception("matrix dimension mismatch");
            }
            Matrix returnObj = new Matrix(m3.nRow, m3.nCol);
            double sum = 0.0, divided;
            for (int i = 0; i < m1.W.Length; i++)
            {
                sum = (m1.W[i] + m2.W[i] + m3.W[i]);
                returnObj.W[i] = m3.W[i] / sum;
                //m2.W[i] = m2.W[i] / sum;
                //m3.W[i] = m3.W[i] / sum;
            }
            if (this.applyBackprop)
            {
                Runnable bp = new Runnable();
                bp.Run = delegate()
                {
                    for (int i = 0; i < m1.W.Length; i++)
                    {
                        divided = sum * sum;
                        m1.gradW[i] += returnObj.gradW[i] * -m3.W[i] / divided;
                        m2.gradW[i] += returnObj.gradW[i] * -m3.W[i] / divided;
                        m3.gradW[i] += returnObj.gradW[i] * (m1.W[i] + m2.W[i]) / divided;
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        public void SumDivid(Matrix m1, Matrix m2, Matrix m3)
        {
            if (m1.nRow != m2.nRow || m1.nCol != m2.nCol)
            {
                throw new Exception("matrix dimension mismatch");
            }

            double sum=0.0, divided;
            for (int i = 0; i < m1.W.Length; i++)
            {
                sum = (m1.W[i] + m2.W[i] + m3.W[i]);
                m1.W[i] = m1.W[i] / sum;
                m2.W[i] = m2.W[i] / sum;
                m3.W[i] = m3.W[i] / sum;
            }
            if (this.applyBackprop)
            {
                Runnable bp = new Runnable();
                bp.Run = delegate()
                {
                    for (int i = 0; i < m1.W.Length; i++)
                    {
                        divided = sum*sum;
                        m1.gradW[i] += m1.W[i] * (m2.W[i] + m3.W[i]-2)/ divided;
                        m2.gradW[i] += m2.W[i] * (m1.W[i] + m3.W[i]-2) / divided;
                        m3.gradW[i] += m3.W[i] * (m2.W[i] + m1.W[i]-2) / divided;
                    }
                };
                Backprop.Add(bp);
            }
        }

    }
}
