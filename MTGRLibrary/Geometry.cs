/**
 * The $P Point-Cloud Recognizer (.NET Framework 4.0 C# version)
 *
 * 	    Radu-Daniel Vatavu, Ph.D.
 *	    University Stefan cel Mare of Suceava
 *	    Suceava 720229, Romania
 *	    vatavu@eed.usv.ro
 *
 *	    Lisa Anthony, Ph.D.
 *      UMBC
 *      Information Systems Department
 *      1000 Hilltop Circle
 *      Baltimore, MD 21250
 *      lanthony@umbc.edu
 *
 *	    Jacob O. Wobbrock, Ph.D.
 * 	    The Information School
 *	    University of Washington
 *	    Seattle, WA 98195-2840
 *	    wobbrock@uw.edu
 *
 * The academic publication for the $P recognizer, and what should be 
 * used to cite it, is:
 *
 *	Vatavu, R.-D., Anthony, L. and Wobbrock, J.O. (2012).  
 *	  Gestures as point clouds: A $P recognizer for user interface 
 *	  prototypes. Proceedings of the ACM Int'l Conference on  
 *	  Multimodal Interfaces (ICMI '12). Santa Monica, California  
 *	  (October 22-26, 2012). New York: ACM Press, pp. 273-280.
 *
 * This software is distributed under the "New BSD License" agreement:
 *
 * Copyright (c) 2012, Radu-Daniel Vatavu, Lisa Anthony, and 
 * Jacob O. Wobbrock. All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *    * Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *    * Redistributions in binary form must reproduce the above copyright
 *      notice, this list of conditions and the following disclaimer in the
 *      documentation and/or other materials provided with the distribution.
 *    * Neither the names of the University Stefan cel Mare of Suceava, 
 *	    University of Washington, nor UMBC, nor the names of its contributors 
 *	    may be used to endorse or promote products derived from this software 
 *	    without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
 * IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Radu-Daniel Vatavu OR Lisa Anthony
 * OR Jacob O. Wobbrock BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
 * OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
**/
using System;

namespace MTGRLibrary
{
    public class Geometry
    {
        /// <summary>
        /// Computes the Squared Euclidean Distance between two points in 2D
        /// </summary>
        public static float SqrEuclideanDistance(Point a, Point b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }

        public static float SqrEuclideanDistance(float x1, float y1, float x2, float y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }

        /// <summary>
        /// Computes the Euclidean Distance between two points in 2D
        /// </summary>
        public static float EuclideanDistance(Point a, Point b)
        {
            return (float) Math.Sqrt(SqrEuclideanDistance(a, b));
        }

        public static float EuclideanDistance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(SqrEuclideanDistance(x1, y1, x2, y2));                
        }


        /// <summary>
        /// Computes the Area of any Iregular (or regular) Polygon
        /// </summary>
        public static float AreaPolygon(Point[] points)
        { 
            //last point must be the same as the first point
            float areaP = 0;
            if (points == null)
                throw new Exception("points cannot be null");
        
            int k = points.Length - 1;
            for (int i = 0; i < k; ++i)
                areaP += points[i].X * points[i + 1].Y - points[i + 1].X * points[i].Y;

            areaP += points[k].X * points[0].Y - points[0].X * points[k].Y;
            
            return areaP / 2.0f; ;
        }

        /// <summary>
        /// Computes the angle between Point a and b (as position vectors), assuming the tail of both is at the origin
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float AngleBetween(Point a, Point b)
        {
          
            Point origin = new Point(0.0f, 0.0f);

            float aMagnitude = EuclideanDistance(a, origin);
            float bMagnitude = EuclideanDistance(b, origin);

            float dot = (a.X * b.X) + (a.Y * b.Y);

            return (float) Math.Acos(dot / (aMagnitude * bMagnitude));
        }

        public static float AngleBetween(float x1, float y1, float x2, float y2)
        {
            Point origin = new Point(0.0f, 0.0f);

            float aMagnitude = EuclideanDistance(x1,y1,0,0);

            float bMagnitude = EuclideanDistance(x2,y2,0,0);
            
            float dot = (x1 * x2) + (y1 * y2);

            return (float)Math.Acos(dot / (aMagnitude * bMagnitude));
        }



        /// <summary>
        /// Compute the angle between vector a and b, assuming the tail of both is at point c
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>Angle Between vector a and b</returns>
        public static float AngleBetween(Point a, Point b, Point c)
        {
                       

            float aMagnitude = EuclideanDistance(a, c);
            float bMagnitude = EuclideanDistance(b, c);

            float dot = (a.X * b.X) + (a.Y * b.Y);
            float angle = (float)Math.Acos(dot / (aMagnitude * bMagnitude));
            return angle;
        }

        /// <summary>
        /// Returns true if a point p is in circle C with center at 'center' and radius r
        /// </summary>
        /// <param name="p"></param>
        /// <param name="center"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool InCircle(Point p, Point center, float r)
        {
            return EuclideanDistance(p, center) <= r;
        }



        /// <summary>
        /// If a line segment is defined by two points, l1 and l2, this method returns the distance
        /// from a point x, to that line segment.
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float PointToLineDistance(Point l1, Point l2, Point x)
        {
            float denominator = EuclideanDistance(l1, l2);

            float numerator = ((l2.Y - l1.Y) * x.X) - ((l2.X - l1.X) * x.Y)  + (l2.X * l1.Y) - (l2.Y * l1.X);

            numerator = Math.Abs(numerator);

            return numerator / denominator;

        }
    }
}
