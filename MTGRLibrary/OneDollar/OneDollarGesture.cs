using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGRLibrary.OneDollar
{
    class OneDollarGesture : Gesture
    {
        Point[] stroke;

        public Point[] Points
        {
            get
            {
                return stroke;
            }
        }
        public OneDollarGesture(PointMap traces, string gestureName, eGestureType gestureType, string expectedAs = "") : base(traces, gestureName, gestureType,expectedAs)
        {
            int firstTrace = rawTraces.TraceIDs[0];
            stroke = Scale(RotateToZero(Resample(rawTraces[firstTrace].ToArray(), rawTraces[firstTrace].Count)));
        }

        public OneDollarGesture(Gesture inputGesture) : this(inputGesture.rawTraces, inputGesture.Name, inputGesture.GestureType, inputGesture.ExpectedAs) { }

        public static Point[] RotateToZero(Point[] points) {
            Point c = Centroid(points);
            double angle = Math.Atan2(c.Y - points[0].Y, c.X - points[0].X);

            Point[] newPoints = RotateBy(points, (-1) * angle,c);
            return newPoints;
        }

        public static Point[] RotateBy(Point[] points,double angle, Point centroid = null)
        {
            Point c;
            if (centroid == null)
                c = Centroid(points);
            else
                c = centroid;

            Point[] newPoints = new Point[points.Length];

            for(int i = 0; i < points.Length; i++)
            {
                Point p = points[i];
                double X = (p.X - c.X) * Math.Cos(angle) - (p.Y - c.Y) * Math.Sin(angle) + c.X;
                double Y = (p.X - c.X) * Math.Sin(angle) + (p.Y - c.Y) * Math.Cos(angle) + c.Y;

                newPoints[i] = new Point(X, Y);
            }
            return newPoints;
        }
        
    }
}

