using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGRLibrary
{
    public class CircGR:Recognizer
    {
        List<CircGesture> Templates = null;
        

        public CircGR() : base("CircGR") { }


        public override void SetTemplates(List<Gesture> templates)
        {
            Templates = new List<CircGesture>();
            foreach(Gesture t in templates)
            {
                Templates.Add(new CircGesture(t));
            }
        }
        
        override public string Classify(Gesture inputGesture)
        {
            
            if (Templates == null)
            {
                return "Templates have not been set for " + this.Name;
            }

            CircGesture candidate = new CircGesture(inputGesture);

            double minDistance = Double.MaxValue;
            string c = "No Classification";
                        
            if (verbose)
                Console.WriteLine("========== Distances ==========");


            foreach (CircGesture t in Templates)
            {
                if (!InContext(t, candidate))
                    continue;

                double distance = 0;

                //calc distance
                foreach (CircGesture.Directions dir in Enum.GetValues(typeof(CircGesture.Directions)))
                {
                    //can be time or distance or both
                    //distance += CalculateTimeDistance(candidate, t, dir);
                    //distance += CalculateDistance(candidate, t, dir);
                    distance += CalculateBothDistance(candidate, t, dir);
                }

                //double distance = Math.Max(CalculateTimeDistance(candidate, t), CalculateDistance(candidate, t));

                if (distance < minDistance)
                {
                    c = t.Name;
                    minDistance = distance;                    
                }

                if (verbose)
                    Console.WriteLine(t.Name + " " + distance);
            }

            return c;
        }

        override public string gestureToString(Gesture inputGesture)
        {
            return new CircGesture(inputGesture).ToString();
        }


        private static double CalculateBothDistance(CircGesture candidate, CircGesture template, CircGesture.Directions direction)
        {
            double distance = 0;

            List<double> cDirections = candidate.directionalEvents.GetObservations(direction);
            List<double> tDirections = template.directionalEvents.GetObservations(direction);

            //cDirections.Sort();
            //tDirections.Sort();

            double tDistance = 0;

            List<double> cTDirections = candidate.temporalEvents.GetEvents(direction);
            List<double> tTDirections = template.temporalEvents.GetEvents(direction);


            Tuple<List<double>, List<double>> larger;
            Tuple<List<double>, List<double>> smaller;

            bool tBig;

            if (cDirections.Count > tDirections.Count)
            {
                larger = new Tuple<List<double>, List<double>>(cDirections, cTDirections);
                smaller = new Tuple<List<double>, List<double>>(tDirections, tTDirections);
                tBig = false;
            }
            else
            {
                larger = new Tuple<List<double>, List<double>>(tDirections, tTDirections);
                smaller = new Tuple<List<double>, List<double>>(cDirections, cTDirections);
                tBig = true;
            }

            for (int i = 0; i < smaller.Item1.Count(); i++)
            {

                distance += ObservationDistance(smaller.Item1.ElementAt(i), larger.Item1.ElementAt(i));
                tDistance += ObservationDistance(smaller.Item2.ElementAt(i), larger.Item2.ElementAt(i));

            }
            for (int i = smaller.Item1.Count(); i < larger.Item1.Count; i++)
            {
                if (tBig)
                    distance += ObservationDistance(larger.Item1.ElementAt(i), candidate.directionalEvents.GetResultant(direction).Angle);
                else
                    distance += ObservationDistance(larger.Item1.ElementAt(i), template.directionalEvents.GetResultant(direction).Angle);
                tDistance += Math.PI;
            }
            return distance + tDistance;
        }

        private static double ObservationDistance(double o1, double o2)
        {
            return Math.PI - Math.Abs(Math.PI - Math.Abs(o1 - o2));
        }



        #region Context
        private static bool InContext(CircGesture template, CircGesture candidate)
        {
            return !(GetGestureContext(candidate, template) == 0 || GetApplicationContext() == 0);
        }

        private static int GetApplicationContext()
        {
            return 1;
        }

        private static int GetGestureContext(CircGesture candidate, CircGesture template)
        {
            //if (candidate.NumOfTraces != template.NumOfTraces || candidate.NumOfAnchors != template.NumOfAnchors)
            //    return 0;
            if (candidate.NumOfFingers != template.NumOfFingers || candidate.NumOfTraces != template.NumOfTraces)
                return 0;
            return 1;
        }
        #endregion
    }
}
