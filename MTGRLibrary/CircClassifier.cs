using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGRLibrary
{
    /// <summary>
    /// This class is under review. It is the first version of CircGR before Recognizer abstraction.
    /// It should be removed after all usefull functionality is extracted.
    /// </summary>
    public class CircClassifier
    {
        static int TrialNum = 0;
        static string LastClass = "No Classification";

        private static bool useHungarian = false;
        private static bool useDecay = false;
        private static bool useTimeScaling = false;
        private static bool usePercentAdjustment = false;


        public static string Classify(CircGesture candidate, List<CircGesture> templates, out bool dropWindow, int windowNum = 1, bool verbose = true,string expectedGesture = "")
        {
            dropWindow = false;


            if ((windowNum < 2 || LastClass.Equals("No Classification")) || !useDecay)
                LastClass =Classify6(candidate, templates,verbose);
            else
            {
                CircGesture tempt = templates.Find(t => t.Name.Equals(LastClass));
                double dist = ObservationDistance(tempt.GestureResultant.Angle, candidate.GestureResultant.Angle);
                if (dist > 0.52)
                {
                    LastClass = Classify4(candidate, templates, verbose);
                    dropWindow = true;
                }
            }

            return LastClass;
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

        /// <summary>
        /// Classification based on total distances from one set of observations to the other.
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="templates"></param>
        /// <returns></returns>
        public static string Classify2(CircGesture candidate, List<CircGesture> templates)
        {
            double minDistance = Double.MaxValue;
            string c = "No Classification";

            List<double> cObs = candidate.OBSERVATIONS;

            cObs.Sort();


            List<CircGesture> inContext = new List<CircGesture>();

            foreach (CircGesture template in templates)
            {
                if (GetGestureContext(candidate, template) == 0 || GetApplicationContext() == 0)
                    continue;

                inContext.Add(template);
            }

            Console.WriteLine("========== Distances ==========");
            foreach(CircGesture t in inContext)
            {

                if (t.OBSERVATIONS.Count != cObs.Count)
                    continue;

                double distance = 0;

                List<double> tObs = t.OBSERVATIONS;
                tObs.Sort();

                for(int i = 0; i < tObs.Count; i++)
                {
                    distance += ObservationDistance(cObs.ElementAt(i), tObs.ElementAt(i));
                }

                if (distance < minDistance)
                {
                    c = t.Name;
                    minDistance = distance;
                }

                Console.WriteLine(t.Name + " " + distance);
            }


            return c;
        }

        public static string Classify3(CircGesture candidate, List<CircGesture> templates)
        {
            double minDistance = Double.MaxValue;
            string c = "No Classification";

            List<double> cObs = candidate.OBSERVATIONS;


            List<CircGesture> inContext = new List<CircGesture>();

            foreach (CircGesture template in templates)
            {
                if (GetGestureContext(candidate, template) == 0 || GetApplicationContext() == 0)
                    continue;

                inContext.Add(template);
            }

            Console.WriteLine("========== Distances ==========");
            foreach (CircGesture t in inContext)
            {

                if (t.OBSERVATIONS.Count != cObs.Count)
                    continue;

                double distance = 0;

                List<double> tObs = new List<double>(t.OBSERVATIONS);
                

                for (int i = 0; i < tObs.Count; i++)
                {
                    var tup = HungarianMatch(cObs.ElementAt(i), tObs);
                    
                    distance += (1 / (i + 1)) * tup.Item2;

                    tObs.RemoveAt(tup.Item1);

                }



                if (distance < minDistance)
                {
                    c = t.Name;
                    minDistance = distance;
                }

                Console.WriteLine(t.Name + " " + distance);
            }


            return c;
        }



        private static double ObservationDistance(double o1, double o2)
        {
            return Math.PI - Math.Abs(Math.PI - Math.Abs(o1 - o2));
        }

        private static Tuple<int,double> HungarianMatch(double observation, List<double> observations)
        {
            double minDistance = double.MaxValue;
            int minIndex = -1;

            for(int i = 0; i < observations.Count; i++)
            {
                double distance = ObservationDistance(observation, observations.ElementAt(i));

                if (distance < minDistance)
                {
                    minIndex = i;
                    minDistance = distance;
                }
            }


            return new Tuple<int, double>(minIndex, minDistance);
        }

        


        /// <summary>
        /// Measure of disimmilarity implemented from "Statistical Analysis of Circular Data"  by Fisher pg. 122 
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        private static double GetCircularDisimilarity(CircGesture candidate, CircGesture template)
        {
            //Test statistic
            double W = 0;


            Tuple<double, double> cRanks = CalculateCircularRanks(candidate.OBSERVATIONS);
            Tuple<double, double> tRanks = CalculateCircularRanks(template.OBSERVATIONS);

            W += (Math.Pow(cRanks.Item1, 2) + Math.Pow(cRanks.Item2, 2)) / candidate.OBSERVATIONS.Count;
            W += (Math.Pow(tRanks.Item1, 2) + Math.Pow(tRanks.Item2, 2)) / template.OBSERVATIONS.Count;

            W *= 2;
            return W;
        }


        public static Tuple<double,double> CalculateCircularRanks(List<double> observations)
        {
            //i doubt this is necessary, the book says to sort them in the circular rank section, but this test does not actually depend on the observations
            observations.Sort();

            double C = 0, S = 0;

            for (int i = 1; i <= observations.Count; i++)
            {
                double gamma = (2 * Math.PI * i) / observations.Count;
                C += Math.Cos(gamma);
                S += Math.Sin(gamma);
            }
            return new Tuple<double, double>(C,S);
        }


        /// <summary>
        /// Dissimilarity over the decomposition of direction or time
        /// filterss based on context, the calculates distances
        /// contains more looping that necessayr as directions are iterated twice over
        /// context is binary
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="templates"></param>
        /// <returns></returns>
        public static string Classify4(CircGesture candidate, List<CircGesture> templates, bool verbose = false)
        {
            double minDistance = Double.MaxValue;
            string c = "No Classification";

            CircGesture gestureClass = candidate;

            Dictionary<CircGesture, double> results = new Dictionary<CircGesture, double>();

            if (verbose)
                Console.WriteLine("========== Distances ==========");
            foreach (CircGesture t in templates)
            {
                if (!InContext(t, candidate))
                    continue;

                //can be time or distance or both
                double distance = CalculateTimeDistance(candidate, t);
                //distance += CalculateDistance(candidate, t);
        
                //double distance = Math.Max(CalculateTimeDistance(candidate, t), CalculateDistance(candidate, t));

                if (distance < minDistance)
                {
                    c = t.Name;
                    minDistance = distance;
                    gestureClass = t;
                }
                results.Add(t, distance);

                if(verbose)
                    Console.WriteLine(t.Name + " " + distance);
            }

            //if (inContext.Count != 0)
            //{
                //double p_val = CalculateRubineRejection(gestureClass, results);
                //Console.WriteLine("P-Value: " + p_val);
                //if (p_val < 0.98)
                //    c = "Classification Rejected";

                //if (CalculateDixonsRejection(gestureClass, results))
                //    c = "Classification Rejected";


            //}

            return c;
        }

        private static bool InContext(CircGesture template, CircGesture candidate)
        {
            return !(GetGestureContext(candidate, template) == 0 || GetApplicationContext() == 0);
        }




        /// <summary>
        /// Dissimilarity over the decomposition of direction or time.
        /// Filters while looping and context can contribute to distance itself, rather than just filter.
        /// Context is fuzzy
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="templates"></param>
        /// <returns></returns>
        public static string Classify5(CircGesture candidate, List<CircGesture> templates)
        {
            double minDistance = Double.MaxValue;
            string c = "No Classification";

            CircGesture gestureClass = candidate;

            Dictionary<CircGesture, double> results = new Dictionary<CircGesture, double>();
            
            //Console.WriteLine("========== Distances ==========");
            foreach (CircGesture t in templates)
            {
                double context = GetFuzzyGestureContext(candidate, t);
                if ( context < 0 || GetApplicationContext() < 0)
                    continue;

                //can be time or distance or both
                double distance = CalculateTimeDistance(candidate, t);
                distance += context * CalculateDistance(candidate, t);

                //double distance = Math.Max(CalculateTimeDistance(candidate, t), CalculateDistance(candidate, t));

                if (distance < minDistance)
                {
                    c = t.Name;
                    minDistance = distance;
                    gestureClass = t;
                }
                results.Add(t, distance);
            }

            //if (inContext.Count != 0)
            //{
            //double p_val = CalculateRubineRejection(gestureClass, results);
            //Console.WriteLine("P-Value: " + p_val);
            //if (p_val < 0.98)
            //    c = "Classification Rejected";

            //if (CalculateDixonsRejection(gestureClass, results))
            //    c = "Classification Rejected";


            //}

            return c;
        }
        /// <summary>
        /// Returns context in non binary, or "fuzzy", terms. 
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        private static double GetFuzzyGestureContext(CircGesture candidate, CircGesture template)
        {
            if (candidate.NumOfFingers != template.NumOfFingers || candidate.NumOfTraces != template.NumOfTraces)
                return -1;
            double maxDisp = Math.Max(candidate.GestureResultant.Dispersion, template.GestureResultant.Dispersion);

            return 1.0 + Math.Abs((candidate.GestureResultant.Dispersion - template.GestureResultant.Dispersion) / maxDisp);
        }



        /// <summary>
        /// Leanest version of Classify, attempts to keep looping to a minimum
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="templates"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public static string Classify6(CircGesture candidate, List<CircGesture> templates, bool verbose = false)
        {
            double minDistance = Double.MaxValue;
            string c = "No Classification";

           
            if (verbose)
                Console.WriteLine("========== Distances ==========");
                        

            foreach (CircGesture t in templates)
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

        private static double CalculateDistance(CircGesture candidate, CircGesture template, CircGesture.Directions direction)
        {
            double distance = 0;

            List<double> cDirections = candidate.directionalEvents.GetObservations(direction);
            List<double> tDirections = template.directionalEvents.GetObservations(direction);

            List<double> larger;
            List<double> smaller;

            bool tBig;

            if (cDirections.Count > tDirections.Count)
            {
                larger = cDirections;
                smaller = tDirections;
                tBig = false;
            }
            else
            {
                larger = tDirections;
                smaller = cDirections;
                tBig = true;
            }


            if (!useHungarian)
            {
                larger.Sort();
                smaller.Sort();

                for (int i = 0; i < smaller.Count(); i++)
                {
                    if (usePercentAdjustment)
                        distance += (1 + Math.Abs(template.getPercentage(direction) - candidate.getPercentage(direction))) * ObservationDistance(smaller.ElementAt(i), larger.ElementAt(i));
                    else
                        distance += ObservationDistance(smaller.ElementAt(i), larger.ElementAt(i));
                }


                for (int i = smaller.Count(); i < larger.Count; i++)
                {
                    if (tBig)
                        distance += ObservationDistance(larger.ElementAt(i), candidate.directionalEvents.GetResultant(direction).Angle);
                    else
                        distance += ObservationDistance(larger.ElementAt(i), template.directionalEvents.GetResultant(direction).Angle);
                }

            }
            else
            {
                List<double> lObs = new List<double>(larger);

                

                foreach (double obs in smaller)
                {
                    Tuple<int, double> h = HungarianMatch(obs, lObs);
                    if (usePercentAdjustment)
                        distance += (1 + Math.Abs(template.getPercentage(direction) - candidate.getPercentage(direction))) * h.Item2;
                    else
                        distance += (lObs.Count / larger.Count) * h.Item2;

                    lObs.RemoveAt(h.Item1);
                }

                foreach (double obs in lObs)
                {
                    if (tBig)
                        distance += ObservationDistance(obs, candidate.directionalEvents.GetResultant(direction).Angle);
                    else
                        distance += ObservationDistance(obs, template.directionalEvents.GetResultant(direction).Angle);
                }
            }
            

            return distance;
        }

        private static double CalculateTimeDistance(CircGesture candidate, CircGesture template, CircGesture.Directions direction)
        {
            double distance = 0;

            List<double> cDirections, tDirections;


            if (useTimeScaling)
            {
            var scaledDist = ScaleTimeDistances(candidate, template, direction);
            cDirections = scaledDist.Item1;
            tDirections = scaledDist.Item2;
            }
            else
            {
            cDirections = candidate.temporalEvents.GetEvents(direction);
            tDirections = template.temporalEvents.GetEvents(direction);
            }


            List<double> larger;
            List<double> smaller;

            bool tBig;

            if (cDirections.Count > tDirections.Count)
            {
                larger = cDirections;
                smaller = tDirections;
                tBig = false;
            }
            else
            {
                larger = tDirections;
                smaller = cDirections;
                tBig = true;
            }

            //larger.Sort();
            //smaller.Sort();

            for (int i = 0; i < smaller.Count(); i++)
            {
                distance += ObservationDistance(smaller.ElementAt(i), larger.ElementAt(i));
            }

            for (int i = smaller.Count(); i < larger.Count; i++)
            {
            if (tBig)
                distance += Math.PI;//ObservationDistance(larger.ElementAt(i), candidate.temporalEvents.GetEventResultant(dir).Angle);
            else
                distance += Math.PI; //ObservationDistance(larger.ElementAt(i), template.temporalEvents.GetEventResultant(dir).Angle);
            }           

            return distance;
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


        /// <summary>
        /// Q95% Table for 3 - 10 values
        /// </summary>
        private static double[] QTable = { 0.941, 0.765, 0.642, 0.560, 0.507, 0.468, 0.437, 0.412 }; //{0.970, 0.829, 0.71, 0.625, 0.568, 0.526, 0.493,0.466};

        private static bool CalculateDixonsRejection(CircGesture gestureClass, Dictionary<CircGesture, double> results)
        {
            if (results.Keys.Count < 3)
                return false;

            List<double> r = new List<double>(results.Values);

            r.Sort();

            double Q = (r[1] - r[0]) / (r[r.Count - 1] - r[0]);


            return Q < QTable[r.Count-1];
        }


        public static double CalculateDistance(CircGesture candidate, CircGesture template)
        {
            double distance = 0;

            foreach (CircGesture.Directions dir in Enum.GetValues(typeof(CircGesture.Directions)))
            {

                List<double> cDirections = candidate.directionalEvents.GetObservations(dir);
                List<double> tDirections = template.directionalEvents.GetObservations(dir);

                List<double> larger;
                List<double> smaller;

                bool tBig;
                
                if(cDirections.Count > tDirections.Count)
                {
                    larger = cDirections;
                    smaller = tDirections;
                    tBig = false;
                }
                else
                {
                    larger = tDirections;
                    smaller = cDirections;
                    tBig = true;
                }


                if (!useHungarian)
                {
                    larger.Sort();
                    smaller.Sort();

                    for (int i = 0; i < smaller.Count(); i++)
                    {
                        if (usePercentAdjustment)
                            distance += (1+Math.Abs(template.getPercentage(dir) - candidate.getPercentage(dir)))*ObservationDistance(smaller.ElementAt(i), larger.ElementAt(i));
                        else
                            distance += ObservationDistance(smaller.ElementAt(i), larger.ElementAt(i));
                    }


                    for (int i = smaller.Count(); i < larger.Count; i++)
                    {
                        if (tBig)
                            distance += ObservationDistance(larger.ElementAt(i), candidate.directionalEvents.GetResultant(dir).Angle);
                        else
                            distance += ObservationDistance(larger.ElementAt(i), template.directionalEvents.GetResultant(dir).Angle);
                    }

                }
                else
                {
                    List<double> lObs = new List<double>(larger);

                    foreach(double obs in smaller)
                    {
                        Tuple<int, double> h = HungarianMatch(obs, lObs);
                        if (usePercentAdjustment)
                            distance += (1 + Math.Abs(template.getPercentage(dir) - candidate.getPercentage(dir))) * h.Item2;
                        else
                            distance += (lObs.Count/larger.Count) * h.Item2;
                        
                        lObs.RemoveAt(h.Item1);
                    }

                    foreach(double obs in lObs)
                    {
                        if (tBig)
                            distance += ObservationDistance(obs, candidate.directionalEvents.GetResultant(dir).Angle);
                        else
                            distance += ObservationDistance(obs, template.directionalEvents.GetResultant(dir).Angle);
                    }
                }
            }

            return distance;
        }



        public static double CalculateTimeDistance(CircGesture candidate, CircGesture template)
        {
            double distance = 0;

            foreach (CircGesture.Directions dir in Enum.GetValues(typeof(CircGesture.Directions)))
            {

                List<double> cDirections, tDirections;

                if (useTimeScaling)
                {
                    var scaledDist = ScaleTimeDistances(candidate, template, dir);
                    cDirections = scaledDist.Item1;
                    tDirections = scaledDist.Item2;
                }
                else
                {
                    cDirections = candidate.temporalEvents.GetEvents(dir);
                    tDirections = template.temporalEvents.GetEvents(dir);
                }


                List<double> larger;
                List<double> smaller;

                bool tBig;

                if (cDirections.Count > tDirections.Count)
                {
                    larger = cDirections;
                    smaller = tDirections;
                    tBig = false;
                }
                else
                {
                    larger = tDirections;
                    smaller = cDirections;
                    tBig = true;
                }

                //larger.Sort();
                //smaller.Sort();

                for (int i = 0; i < smaller.Count(); i++)
                {
                    distance += ObservationDistance(smaller.ElementAt(i), larger.ElementAt(i));
                }

                for (int i = smaller.Count(); i < larger.Count; i++)
                {
                    if (tBig)
                        distance += Math.PI;//ObservationDistance(larger.ElementAt(i), candidate.temporalEvents.GetEventResultant(dir).Angle);
                    else
                        distance += Math.PI; //ObservationDistance(larger.ElementAt(i), template.temporalEvents.GetEventResultant(dir).Angle);
                }

            }

            return distance;
        }


        public static Tuple<List<double>,List<double>> ScaleTimeDistances(CircGesture candidate, CircGesture template, CircGesture.Directions dir)
        {
            List<double> cDirections = candidate.temporalEvents.GetEvents(dir);
            List<double> tDirections = template.temporalEvents.GetEvents(dir);

            //time scaling based on just the template seems to work better in practice
            double maxDur = template.temporalEvents.TOTAL_DURATION; //Math.Max(candidate.temporalEvents.TOTAL_DURATION, template.temporalEvents.TOTAL_DURATION);

            for (int i = 0; i < cDirections.Count; i++)
            {
                cDirections[i] =( cDirections[i] * (candidate.temporalEvents.TOTAL_DURATION / maxDur)) % (Math.PI * 2);
            }

            //for (int i = 0; i < tDirections.Count; i++)
            //{
            //    tDirections[i] = (tDirections[i] * (template.temporalEvents.TOTAL_DURATION / maxDur)) % (Math.PI*2);

            //}

            return new Tuple<List<double>, List<double>>(cDirections, tDirections);

        }
    }
}
