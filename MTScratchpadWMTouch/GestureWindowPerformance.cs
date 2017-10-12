using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M = MTGRLibrary;

namespace MTScratchpadWMTouch
{
    class GestureWindowPerformance
    {
        /// <summary>
        /// Each template is has an associated ACC for each window count (1st , 2nd, 3rd,ect),
        /// templateName -> (windowCount->ACC)
        /// </summary>
        Dictionary<string,Dictionary<int, WindowPerf>> recognitionPerfomance;

        /// <summary>
        /// Struct represents the performance of the classifier for a given template at a certain capture window  
        /// </summary>
        public class WindowPerf
        {
            int totalAttempts;
            int correctGuesses;
            public double Accuracy;


            public void Correct()
            {
                this.totalAttempts++;
                this.correctGuesses++;
                updateACC();
            }

            public void Incorrect()
            {
                this.totalAttempts++;
                updateACC();
            }

            private void updateACC()
            {
                this.Accuracy = (double)correctGuesses / (double) totalAttempts;
            }


        }

        public GestureWindowPerformance(List<M.Gesture> TrainingSet)
        {
            recognitionPerfomance = new Dictionary<string, Dictionary<int, WindowPerf>>();
            foreach(M.Gesture template in TrainingSet)
            {
                recognitionPerfomance.Add(template.Name, new Dictionary<int, WindowPerf>());
            }
        }

        public GestureWindowPerformance(List<M.CircGesture> TrainingSet)
        {
            recognitionPerfomance = new Dictionary<string, Dictionary<int, WindowPerf>>();
            foreach (M.CircGesture template in TrainingSet)
            {
                recognitionPerfomance.Add(template.Name, new Dictionary<int, WindowPerf>());
            }
        }


        public void RecordPerformance(int windowNumber, string expected, string predicted)
        {
            if (!recognitionPerfomance[expected].ContainsKey(windowNumber))
            {
                recognitionPerfomance[expected].Add(windowNumber, new WindowPerf());                
            }


            if (expected.Equals(predicted))
            {
                recognitionPerfomance[expected][windowNumber].Correct();
            }
            else
            {
                recognitionPerfomance[expected][windowNumber].Incorrect();
            }
        }

        public double getPerformance(string template, int windowNumber)
        {
            if (recognitionPerfomance.ContainsKey(template))
                if (recognitionPerfomance[template].ContainsKey(windowNumber))
                    return recognitionPerfomance[template][windowNumber].Accuracy;


            return -1.0;
        }

        




    }
}
