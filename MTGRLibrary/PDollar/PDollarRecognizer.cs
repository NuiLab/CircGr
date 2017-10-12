using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGRLibrary.PDollar
{
    public class PDollarRecognizer : Recognizer
    {
        List<PDollarGesture> Templates;
        private bool useFilter = false;

        public PDollarRecognizer() : base("$P") { }


        public override void SetTemplates(List<Gesture> templates)
        {
            Templates = new List<PDollarGesture>();

            foreach(Gesture t in templates)
            {
                Templates.Add(new PDollarGesture(t));
            }
        }

        public override string Classify(Gesture inputGesture)
        {
            float minDistance = float.MaxValue;
            string gestureClass = "No Classification";
            PDollarGesture candidate = new PDollarGesture(inputGesture);
            foreach (PDollarGesture template in Templates)
            {
                if (useFilter && isFiltered(template, inputGesture))
                    continue;

                float dist = GreedyCloudMatch(candidate.PointCloud, template.PointCloud);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    gestureClass = template.Name;
                }
            }
            return gestureClass;
        }

        /// <summary>
        /// Implements greedy search for a minimum-distance matching between two point clouds
        /// </summary>
        /// <param name="points1"></param>
        /// <param name="points2"></param>
        /// <returns></returns>
        private static float GreedyCloudMatch(Point[] points1, Point[] points2)
        {
            int n = points1.Length; // the two clouds should have the same number of points by now
            float eps = 0.5f;       // controls the number of greedy search trials (eps is in [0..1])
            int step = (int)Math.Floor(Math.Pow(n, 1.0f - eps));
            float minDistance = float.MaxValue;
            for (int i = 0; i < n; i += step)
            {
                float dist1 = CloudDistance(points1, points2, i);   // match points1 --> points2 starting with index point i
                float dist2 = CloudDistance(points2, points1, i);   // match points2 --> points1 starting with index point i
                minDistance = Math.Min(minDistance, Math.Min(dist1, dist2));
            }
            return minDistance;
        }

        /// <summary>
        /// Computes the distance between two point clouds by performing a minimum-distance greedy matching
        /// starting with point startIndex
        /// </summary>
        /// <param name="points1"></param>
        /// <param name="points2"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static float CloudDistance(Point[] points1, Point[] points2, int startIndex)
        {
            int n = points1.Length;       // the two clouds should have the same number of points by now
            bool[] matched = new bool[n]; // matched[i] signals whether point i from the 2nd cloud has been already matched
            Array.Clear(matched, 0, n);   // no points are matched at the beginning

            float sum = 0;  // computes the sum of distances between matched points (i.e., the distance between the two clouds)
            int i = startIndex;
            do
            {
                int index = -1;
                float minDistance = float.MaxValue;
                for (int j = 0; j < n; j++)
                    if (!matched[j])
                    {
                        float dist = Geometry.SqrEuclideanDistance(points1[i], points2[j]);  // use squared Euclidean distance to save some processing time
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            index = j;
                        }
                    }
                matched[index] = true; // point index from the 2nd cloud is matched to point i from the 1st cloud
                float weight = 1.0f - ((i - startIndex + n) % n) / (1.0f * n);
                sum += weight * minDistance; // weight each distance with a confidence coefficient that decreases from 1 to 0
                i = (i + 1) % n;
            } while (i != startIndex);
            return sum;
        }


        public override string gestureToString(Gesture inputGesture)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Added as a way to add filtered capabilities to $P
        /// </summary>
        /// <param name="template"></param>
        /// <param name="candidate"></param>
        /// <returns></returns>
        private bool isFiltered(Gesture template, Gesture candidate) {
            if (template.NumOfTraces != candidate.NumOfTraces)
                return true;
            return false;
        }

    }
}
