using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M = MTGRLibrary;


namespace MTScratchpadWMTouch
{
 
    class ConfusionMatrix
    {
        //Coded By Ruben

        private class Column //represents the column of a row and their given count
        {
             M.Gesture gesture;
             int count;

            
            public Column(M.Gesture gesture)
            {
                this.gesture = gesture;
                count = 0;
            }

            public int Count
            {
                get{
                    return count;
                }
            }

            public bool Equals(M.Gesture gesture)
            {
                return this.gesture.Name.Equals(gesture.Name);
            }
            public bool Equals(String gesture)
            {
                return this.gesture.Name.Equals(gesture);
            }
            public bool Equals(Column column)
            {
                return (this.gesture.Name.Equals(column.gesture.Name)) && (this.count == column.count);
            }

            public void IncCount()
            {
                count++;
            }

            public static List<Column> CreateColumns(List<M.Gesture> TestingSet)
            {
                List<Column> columns = new List<Column>();
                
                foreach(M.Gesture testGesture in TestingSet)
                {
                    columns.Add(new Column(testGesture));            
                }
                return columns;
            }
            
        }

        Dictionary<String, List<Column>> matrix = new Dictionary<String, List<Column>>();
        List<M.Gesture> TestingSet;

        public List<M.Gesture> TESTING_SET
        {
            get
            {
                return TestingSet;
            }
        }

        public ConfusionMatrix(List<M.Gesture> TestingSet){
            foreach(M.Gesture testGesture in TestingSet)
            {

                if (!matrix.ContainsKey(testGesture.Name))
                {
                    matrix.Add(testGesture.Name, Column.CreateColumns(TestingSet));
                }            
            }

            this.TestingSet = TestingSet;
        }

        /// <summary>
        /// Record one instance of a specific classification into the conusion matrix.
        /// </summary>
        /// <param name="expected">The correct/true label that the algorithm should have classified.</param>
        /// <param name="predicted">The actual label that the algorithm classified</param>
        public void RecordInstance(M.Gesture expected, M.Gesture predicted) 
        {
            if (!matrix.ContainsKey(expected.Name))
            {
                return; //expected gesture is not part of the current table's Testing Set
            }
            else
            {
                List<Column> columns;
                matrix.TryGetValue(expected.Name,out columns);
                
                foreach (Column column in columns)
                {
                    if(column.Equals(predicted))
                    {
                        column.IncCount();
                        break;
                    }
                }
            } //end else
        }

        public List<string> GetAllLabels()
        {
            return new List<string>(matrix.Keys);
        }

        public void RecordInstance(String expectedGesture, String predictedGesture)
        {
            if (!matrix.ContainsKey(expectedGesture))
            {
                return; //expected gesture is not part of the current table's Testing Set
            }
            else
            {
                List<Column> columns;
                matrix.TryGetValue(expectedGesture, out columns);

                foreach (Column column in columns)
                {
                    if (column.Equals(predictedGesture))
                    {
                        column.IncCount();
                        break;
                    }
                }
            } //end else

        }
        /// <summary>
        /// Get the amount of times a specific label was correctly classified from the confusion matrix.
        /// This is the element in the diagonal.
        /// </summary>
        /// <param name="gesture">The Specific Label</param>
        /// <returns>Amount of times a label was correctly classified.</returns>
        public int GetTruePositives(M.Gesture gesture)
        {
            if (!matrix.ContainsKey(gesture.Name))
            {
                return -1; //Error, gesture is not part of rows
            }
            else
            {
                List<Column> columns;
                matrix.TryGetValue(gesture.Name, out columns);

                foreach (Column column in columns)
                {
                    if (column.Equals(gesture))
                    {
                        return column.Count;
                    }
                }
                return -2; //error, gesture not part of columns
            }
        }

        public int GetTruePositives(String gesture)
        {
            if (!matrix.ContainsKey(gesture))
            {
                return -1; //Error, gesture is not part of rows
            }
            else
            {
                List<Column> columns;
                matrix.TryGetValue(gesture, out columns);

                foreach (Column column in columns)
                {
                    if (column.Equals(gesture))
                    {
                        return column.Count;
                    }
                }
                return -2; //error, gesture not part of columns
            }
        }

        /// <summary>
        /// Get the amount of times another label was incorrectly classified as being the expected label.
        /// This is the sum all counts in the column of the label expect the column in the diagonal.
        /// </summary>
        /// <param name="gesture">The expected label</param>
        /// <returns>Number of False Positives.</returns>
        public int GetFalsePositives(M.Gesture gesture) {
            int falsePositives = 0;

            if (!matrix.ContainsKey(gesture.Name))
            {
                return -1; //Error, gesture is not part of rows
            }
            else
            {

                List<String> keys = new List<string>(this.matrix.Keys);

                foreach (String key in keys) {
                    if (key.Equals(gesture.Name))
                        continue;

                    List<Column> columns;
                    matrix.TryGetValue(key, out columns);

                    foreach (Column column in columns)
                    {
                        if (column.Equals(gesture))
                        {
                            falsePositives += column.Count;
                            break;
                        }
                    }
                
                }             
            }
            return falsePositives;
        }


        public int GetFalsePositives(String gesture)
        {
            int falsePositives = 0;

            if (!matrix.ContainsKey(gesture))
            {
                return -1; //Error, gesture is not part of rows
            }
            else
            {

                List<String> keys = new List<string>(this.matrix.Keys);

                foreach (String key in keys)
                {
                    if (key.Equals(gesture))
                        continue;

                    List<Column> columns;
                    matrix.TryGetValue(key, out columns);

                    foreach (Column column in columns)
                    {
                        if (column.Equals(gesture))
                        {
                            falsePositives += column.Count;
                            break;
                        }
                    }

                }
            }
            return falsePositives;
        }

        /// <summary>
        /// The amount of times the expected label was incorrectly classified as being another label.
        /// This is the sum all counts in the rows of the label expect the column in the diagonal.
        /// </summary>
        /// <param name="gesture">The expected label.</param>
        /// <returns>Amount of False Negatives.</returns>
        public int GetFalseNegatives(M.Gesture gesture) {
            int falseNegatives = 0;

            if (!matrix.ContainsKey(gesture.Name))
            {
                return -1; //Error, gesture is not part of rows
            }
            else
            {
                List<Column> columns;
                matrix.TryGetValue(gesture.Name, out columns);

                    foreach (Column column in columns)
                    {
                        if (!column.Equals(gesture))
                        {
                            falseNegatives += column.Count;
                        }
                    }

                
            }        
            
            return falseNegatives;
        }
        public int GetFalseNegatives(String gesture)
        {
            int falseNegatives = 0;

            if (!matrix.ContainsKey(gesture))
            {
                return -1; //Error, gesture is not part of rows
            }
            else
            {
                List<Column> columns;
                matrix.TryGetValue(gesture, out columns);

                foreach (Column column in columns)
                {
                    if (!column.Equals(gesture))
                    {
                        falseNegatives += column.Count;
                    }
                }


            }

            return falseNegatives;
        }

        /// <summary>
        ///  Method calculates the total amount of labels correctly classified as NOT being the expected label.
        /// This is the sum of all counts in the rows AND columns not pertaining to the expected label.
        /// </summary>
        /// <param name="gesture">The expected label.</param>
        /// <returns>Amount of True Negatives.</returns>
        public int GetTrueNegatives(M.Gesture gesture)
        {
            int trueNegatives = 0;

            if (!matrix.ContainsKey(gesture.Name))
            {
                return -1; //Error, gesture is not part of rows
            }
            else
            {

                List<String> keys = new List<string>(this.matrix.Keys);

                foreach (String key in keys)
                {
                    if (key.Equals(gesture.Name))
                        continue;

                    List<Column> columns;
                    matrix.TryGetValue(key, out columns);

                    foreach (Column column in columns)
                    {
                        if (!column.Equals(gesture))
                        {
                            trueNegatives += column.Count;
                            
                        }
                    }

                }
            }
            return trueNegatives;
        }
        public int GetTrueNegatives(String gesture)
        {
            int trueNegatives = 0;

            if (!matrix.ContainsKey(gesture))
            {
                return -1; //Error, gesture is not part of rows
            }
            else
            {

                List<String> keys = new List<string>(this.matrix.Keys);

                foreach (String key in keys)
                {
                    if (key.Equals(gesture))
                        continue;

                    List<Column> columns;
                    matrix.TryGetValue(key, out columns);

                    foreach (Column column in columns)
                    {
                        if (!column.Equals(gesture))
                        {
                            trueNegatives += column.Count;

                        }
                    }

                }
            }
            return trueNegatives;
        }

        /// <summary>
        /// Get the true positive rate or "sensitivity" for a given label..
        /// </summary>
        /// <param name="gesture">The label.</param>
        /// <returns>True positive rate.</returns>
        public double GetTruePositiveRate(M.Gesture gesture){
            int TP = GetTruePositives(gesture);
            
            if (TP == 0) 
                return 0.0;

            int FN = GetFalseNegatives(gesture);

            return TP / (double)(TP + FN);
        }
        public double GetTruePositiveRate(String gesture)
        {
            int TP = GetTruePositives(gesture);

            if (TP == 0)
                return 0.0;

            int FN = GetFalseNegatives(gesture);

            return TP / (double)(TP + FN);
        }

        /// <summary>
        /// Get the true neative rate or "specificity" for a given label.
        /// </summary>
        /// <param name="gesture">The label.</param>
        /// <returns>True positive rate.</returns>
        public double GetTrueNegativeRate(M.Gesture gesture)
        {
            int TN = GetTrueNegatives(gesture);
            
            if (TN == 0)
                return 0.0;

            int FP = GetFalsePositives(gesture);
            
            return TN / (double)(FP + TN);    
        }
        public double GetTrueNegativeRate(String gesture)
        {
            int TN = GetTrueNegatives(gesture);

            if (TN == 0)
                return 0.0;
            
            int FP = GetFalsePositives(gesture);

            return TN / (double)(FP + TN);
        }

        /// <summary>
        ///  Get the positive predictive value or "precision" of the given label.
        /// </summary>
        /// <param name="gesture">The label.</param>
        /// <returns></returns>
        public double GetPositivePredictiveValue(M.Gesture gesture) 
        {
            int TP = GetTruePositives(gesture);
            if (TP == 0)
                return 0.0;
            int FP = GetFalsePositives(gesture);

            return TP / (double)(TP + FP);        
        }
        public double GetPositivePredictiveValue(String gesture)
        {
            int TP = GetTruePositives(gesture);

            if (TP == 0)
                return 0.0;
            int FP = GetFalsePositives(gesture);

            return TP / (double)(TP + FP);
        }
       
        /// <summary>
        /// Get the negative predictive value of a given label.
        /// </summary>
        /// <param name="gesture">The label.</param>
        /// <returns>Negative predictive Value</returns>
        public double GetNegativePredictiveValue(M.Gesture gesture)
        {
            int TN = GetTrueNegatives(gesture);
            if (TN == 0)

                return 0.0;

            int FN = GetFalseNegatives(gesture);

            return TN / (double)(TN + FN);
        }
        public double GetNegativePredictiveValue(String gesture)
        {
            int TN = GetTrueNegatives(gesture);
            if (TN == 0)
                return 0.0;
            int FN = GetFalseNegatives(gesture);

            return TN / (double)(TN + FN);
        }

        /// <summary>
        /// Get the False Positive rate or "Fall-out" for a given label.
        /// </summary>
        /// <param name="gesture">The label</param>
        /// <returns>The False Positive Rate</returns>
        public double GetFalsePositiveRate(M.Gesture gesture)
        {
            return 1.0 - GetTrueNegativeRate(gesture);
        }
        public double GetFalsePositiveRate(String gesture)
        {
            return 1.0 - GetTrueNegativeRate(gesture);
        }

        /// <summary>
        /// Get the Positive Predictive Value of a given label.
        /// </summary>
        /// <param name="gesture"></param>
        /// <returns></returns>
        public double GetFalseDiscoveryRate(M.Gesture gesture)
        {
            return 1.0 - GetPositivePredictiveValue(gesture);
        }
        public double GetFalseDiscoveryRate(String gesture)
        {
            return 1.0 - GetPositivePredictiveValue(gesture);
        }

        /// <summary>
        /// GEt the false negative rate or "miss rate" of a given label.
        /// </summary>
        /// <param name="gesture">The Label</param>
        /// <returns>False Negative Rate</returns>
        public double GetFalseNegativeRate(M.Gesture gesture)
        {
            int FN = GetFalseNegatives(gesture);
            if (FN == 0)
                return 0.0;

            int TP = GetTruePositives(gesture);

            return FN / (double)(FN + TP);
        }
        public double GetFalseNegativeRate(String gesture)
        {
            int FN = GetFalseNegatives(gesture);
            if (FN == 0)
                return 0.0;

            int TP = GetTruePositives(gesture);

            return FN / (double)(FN + TP);
        }

        /// <summary>
        /// Get the accuracy of an algorithm in identifying a given label.
        /// Accuracy is heavily affected by unbalanced data. Consider using
        /// other metrics such as true positive rate or true negative rate.
        /// </summary>
        /// <param name="gesture">The given label.</param>
        /// <returns></returns>
        public double GetAccuracy(M.Gesture gesture)
        {
            int TP = GetTruePositives(gesture);
            int TN = GetTrueNegatives(gesture);
            double P = (float)TP + GetFalseNegatives(gesture);
            double N = GetFalsePositives(gesture) + (float) TN;

            return (TP + TN) / (P + N);
        }
        public double GetAccuracy(String gesture)
        {
            int TP = GetTruePositives(gesture);
            int TN = GetTrueNegatives(gesture);
            double P = TP + GetFalseNegatives(gesture);
            double N = GetFalsePositives(gesture) + TN;

            return (TP + TN) / (P + N);
        }

        /// <summary>
        /// Get the F-measure of the classifiers accuracy.
        /// F1 is based on the weighted average of precision(positive predictive value) and recall(true positive rate).
        /// Best value is 1, worst is 0.
        /// </summary>
        /// <param name="gesture"></param>
        /// <returns></returns>
        public double GetF1Score(M.Gesture gesture)
        {
            int TP = GetTruePositives(gesture);
            int FP = GetFalsePositives(gesture);
            int FN = GetFalseNegatives(gesture);

            if (TP == 0)
                return 0.0;

            return (2 * TP) / (double)( (2 * TP) + FP + FN);
        }
        public double GetF1Score(String gesture)
        {
            int TP = GetTruePositives(gesture);
            int FP = GetFalsePositives(gesture);
            int FN = GetFalseNegatives(gesture);
            
            if (TP == 0)
                return 0.0;

            return 2 * TP / (double)((2 * TP) + FP + FN);
        }

        /// <summary>
        /// Get the Mathews Correlation Coefficient of a given gesture.
        /// Values for MCC range from -1 to +1.
        /// A value of +1 means perfect prediction.
        /// A value of 0 means predictor is not any better than random prediction.
        /// A value of -1 means total disagreement between prediction and observation.
        /// </summary>
        /// <param name="gesture"></param>
        /// <returns>MCC</returns>
        public double GetMCC(M.Gesture gesture) 
        {
            int TP = GetTruePositives(gesture);
            int TN = GetTrueNegatives(gesture);
            int FP = GetFalsePositives(gesture);
            int FN = GetFalseNegatives(gesture);

            double numerator = (TP * TN) - (FP * FN);
            double d1, d2;
            d1 = (TP + FP) * (TP + FN);
            d2 = (TN + FP) * (TN + FN);
            //multiplicaiton all in one line 
            //double denominator = (TP + FP) * (TP + FN) * (TN + FP) * (TN + FN);

            double denominator = d1 * d2;


            if (denominator <=-0)
                denominator = 1;


            return numerator / Math.Sqrt(denominator);
        }
        public double GetMCC(String gesture)
        {
            int TP = GetTruePositives(gesture);
            int TN = GetTrueNegatives(gesture);
            int FP = GetFalsePositives(gesture);
            int FN = GetFalseNegatives(gesture);

            double numerator = (TP * TN) - (FP * FN);
            double d1, d2;
            d1 = (TP + FP) * (TP + FN);
            d2 = (TN + FP) * (TN + FN);
            //multiplicaiton all in one line 
            //double denominator = (TP + FP) * (TP + FN) * (TN + FP) * (TN + FN);

            double denominator = d1 * d2;

            if (denominator <= 0)
                denominator = 1;

            return numerator / Math.Sqrt(denominator);
        }




        /// <summary>
        /// Return a random label from the confusion matrix.
        /// </summary>
        /// <returns>Label</returns>
        public M.Gesture GetRandomElement()
        {
            M.Gesture[] tSet = TestingSet.ToArray();
            Random rand = new Random();
            int r = rand.Next(0, tSet.Length);

            return tSet[r];
        }




        /// <summary>
        /// Returns a string representation of Confusion Matrix.
        /// </summary>
        /// <returns></returns>
        public override String ToString() 
        {
            String stringMatrix = "";

            List<String> keys = new List<string>(this.matrix.Keys);


            foreach (String key in keys)
            {
                stringMatrix += key + " ";

            }

            stringMatrix += Environment.NewLine;

            stringMatrix += "[";

            foreach (String key in keys)
            {
                

                List<Column> columns;

                matrix.TryGetValue(key, out columns);

                foreach(Column col in columns)
                {
                    stringMatrix += col.Count + " ";
                }

                stringMatrix += ";";//+ Environment.NewLine;

            }
            stringMatrix = stringMatrix.Remove(stringMatrix.Length-2, 2);
            stringMatrix += "]";
            return stringMatrix;
        }
    
    
    }




}
