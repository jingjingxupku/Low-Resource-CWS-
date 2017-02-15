using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Program
{
    public class DataStep
    {
        public Matrix goldOutput = null;//1-hot gold output, a vector of # of tags
        public List<int> inputs = null;//inputs of word embedings
        public int wordindex = 0;
        public int relation = 0;

       

        public DataStep()
        {
            inputs = new List<int>();
        }

        public DataStep(List<int> input, Matrix targetOutput,int wordindex,int relation=0)
        {
            this.inputs = input;
            this.relation = relation;
            this.wordindex = wordindex;
  
            if (targetOutput != null)
            {
                this.goldOutput = targetOutput;
            }
        }



    }
}
