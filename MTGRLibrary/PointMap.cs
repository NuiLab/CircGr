using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGRLibrary
{
    public class PointMap
    {
        private Dictionary<int, List<Point>> pointMap;
        private List<int> traceIDs;
        
        public int MaxNumberOfPoints
        {
            get
            {
                int max = int.MinValue;

                foreach(int id in pointMap.Keys)
                {
                    int numPts = pointMap[id].Count;
                    if (numPts > max)
                        max = numPts;
                }

                return max;
            }
        }

        public int NumberOfPoints
        {
            get
            {
                int numPoints = 0;
                foreach (int id in pointMap.Keys)
                {
                    numPoints += pointMap[id].Count;
                }

                return numPoints;
            }
        }

        public int NumOfTraces
        {
            get
            {
                return traceIDs.Count();
            }
        }

        public List<int> TraceIDs
        {
            get
            {
                return traceIDs;
            }
        }
        
        public List<Point> this[int id]
        {
           get
            {
                return pointMap[id];
            }
        }

        public Point[] concatPoints() {
            int numPoints = this.NumberOfPoints;
            int index = numPoints;

            Point[] points = new Point[numPoints];

            foreach (int id in pointMap.Keys)
            {
                List<Point> pts = pointMap[id];

                foreach(Point p in pts){

                    points[numPoints - index] = new Point(p.X,p.Y,p.StrokeID,p.timestamp);
                    index--;
                }
            }

            return points;
        }


        public PointMap()
        {
            pointMap = new Dictionary<int, List<Point>>();
            traceIDs = new List<int>();
        }

        public PointMap(Dictionary<int, List<Point>> ptMap)
        {
            pointMap = ptMap;
            traceIDs = new List<int>();

            foreach( int id in ptMap.Keys)
            {
                traceIDs.Add(id);
            }
        }

        public void Add(Point p)
        {
            if (pointMap.ContainsKey(p.StrokeID))
            {
                pointMap[p.StrokeID].Add(p);
            }
            else
            {
                pointMap.Add(p.StrokeID, new List<Point>());
                pointMap[p.StrokeID].Add(p);
                traceIDs.Add(p.StrokeID);
            }        
        }

        public List<Point> getPoints(int strokeID)
        {
            return pointMap[strokeID];
        }

        public PointMap getSubset(int count)
        {
            Dictionary<int, List<Point>> n_ptMap = new Dictionary<int, List<Point>>();
            
            foreach(int id in pointMap.Keys)
            {
                List<Point> pList = pointMap[id];

                int elements;

                if (pList.Count > count)
                    elements = count;
                else
                    elements = pList.Count;

                n_ptMap.Add(id,pList.GetRange(0, elements));
            }

            return new PointMap(n_ptMap);
        }

        public void Clear()
        {
            traceIDs = new List<int>();
            pointMap = new Dictionary<int, List<Point>>();
        }

       
    }
}
