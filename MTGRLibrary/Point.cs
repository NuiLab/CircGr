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
    /// <summary>
    /// Implements a 2D Point that exposes X, Y, and StrokeID properties.
    /// StrokeID is the stroke index the point belongs to (e.g., 0, 1, 2, ...) that is filled by counting pen down/up events.
    /// </summary>
    /// 
    [Serializable]
    public class Point
    {
        //added for compatibility with $N
        public static readonly Point Empty = new Point(0,0);

        public float X, Y;
        public int StrokeID;
        private float _x,_y; //temporary, just to check 
        private int countFromAverage = 1;
        public long timestamp;

        #region Constructors-Float (original)
        public Point(float x, float y)
        {
            initPoints(x, y, -1,-1);
        }
        public Point(float x, float y, int strokeId)
        {
            initPoints(x, y, strokeId,-1); 
        }
        
        public Point(float x, float y, int strokeId, long timestamp)
        {
            initPoints(x, y, strokeId, timestamp);
        }
        #endregion

        //Point was originally written using floats, that might be worth changing that
        //convinience constructor for doubles since some integrated classifer came with double implements
        #region Constructors-Double
        public Point(double x, double y)
        {
            initPoints((float)x, (float)y, -1, -1);
        }
        public Point(double x, double y, int strokeId)
        {
            initPoints((float)x, (float)y, strokeId, -1);
        }

        public Point(double x, double y, int strokeId, long timestamp)
        {
            initPoints((float)x, (float)y, strokeId, timestamp);
        }
        #endregion

        public Point(Point p)
        {
            initPoints(p.X, p.Y, p.StrokeID, p.timestamp);
        }


        //Point was originally written using floats, that might be worth changing that
        //convinience constructor for doubles



       
        private void initPoints(float x,float y,int strokeId, long timestamp)
        {
            this._x = this.X = x;
            this._y = this.Y = y;
            this.StrokeID = strokeId;
            this.timestamp = timestamp;

        }
        public int CountFromAverage
        {
            get
            {
                return countFromAverage;
            }
            set
            {
                countFromAverage = value; 
            }
        }


        public static long TimeDifference(Point a, Point b)
        {
            return a.timestamp - b.timestamp;
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, b.Y - b.Y);
        }
    }
}
