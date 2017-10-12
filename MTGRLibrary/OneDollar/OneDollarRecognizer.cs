using System;
using System.Collections.Generic;

namespace MTGRLibrary.OneDollar
{
    public class OneDollarRecognizer : Recognizer
    {
        public bool useProtractor = false;        
        private static readonly double Diagonal = Math.Sqrt(2);
        private static readonly double HalfDiagonal = 0.5 * Diagonal;
        //For Golden Section Search
        private static readonly double Phi = 0.5 * (-1.0 + Math.Sqrt(5.0)); // Golden Ratio
        private const double DefaultUpperBounds = (-45.0 * Math.PI) / 180.0;
        private const double DefaultLowerBounds = (45.0 * Math.PI) / 180.0;
        private const double DefaultThreshold = (2.0 * Math.PI) / 180.0;

        public OneDollarRecognizer() : base("$1") { }

        List<OneDollarGesture> Templates;

        public override string Classify(Gesture inputGesture)
        {
            string classification = "No Classification";
            OneDollarGesture candidate = new OneDollarGesture(inputGesture);
            double maxScore = Double.MinValue; 

            foreach (OneDollarGesture t in Templates)
            {
                double distance = GoldenSectionSearch(candidate.Points, t.Points)[0];

                double score = 1.0 - distance / HalfDiagonal;
                if (maxScore < score)
                {
                    maxScore = score;
                    classification = t.Name;
                }

            }
            return classification;
        }

        public override void SetTemplates(List<Gesture> templates)
        {
            Templates = new List<OneDollarGesture>();
            foreach(Gesture t in templates)
            {
                if (t.NumOfTraces > 1)
                    continue;
                Templates.Add(new OneDollarGesture(t));
            }
        }

        public override string gestureToString(Gesture inputGesture)
        {
            return "$1 Gesture has no string representation at this moment.";
        }

        private double[] GoldenSectionSearch(Point[] candidate, Point[] template, double a = DefaultUpperBounds, double b = DefaultLowerBounds, double threshold = DefaultThreshold)
        {
            double x1 = Phi * a + (1 - Phi) * b;
            Point[] newPoints = OneDollarGesture.RotateBy(candidate, x1);
            double fx1 = PathDistance(newPoints, template);

            double x2 = (1 -Phi) * a + (Phi) * b;
            newPoints = OneDollarGesture.RotateBy(candidate, x2);
            double fx2 = PathDistance(newPoints, template);

            while (Math.Abs(b - a) > threshold)
            {
                if (fx1 < fx2)
                {
                    b = x2;
                    x2 = x1;
                    fx2 = fx1;
                    x1 = Phi * a + (1 - Phi) * b;
                    newPoints = OneDollarGesture.RotateBy(candidate, x1);
                    fx1 = PathDistance(newPoints, template);
                }
                else
                {
                    a = x1;
                    x1 = x2;
                    fx1 = fx2;
                    x2 = (1 - Phi) * a + Phi * b;
                    newPoints = OneDollarGesture.RotateBy(candidate, x2);
                    fx2 = PathDistance(newPoints, template);
                }
            }
            return new double[2] { Math.Min(fx1, fx2), (b + a) / 2 }; // distance, angle (radians)

        }
                
        private static double PathDistance(Point[] A, Point[] B)
        {
            double d = 0;
            for (int i = 0; i < Math.Min(A.Length,B.Length); i++)
                d += Geometry.EuclideanDistance(A[i], B[i]);

            return d/A.Length;
        }


    }

}
