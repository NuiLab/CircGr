using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGRLibrary.PDollar
{
    class PDollarGesture : Gesture
    {
        Point[] Points;

        public Point[] PointCloud
        {
            get
            {
                return Points;
            }
        }


        public PDollarGesture(PointMap traces, string name, eGestureType type, string expectedAs = "") : base(traces, name, type, expectedAs)
        {
            Points = Scale(traces.concatPoints());
            Points = Resample(TranslateTo(Points, Centroid(Points)),SAMPLING_RESOLUTION);
        }

        public PDollarGesture(Gesture inputGesture) : this(inputGesture.rawTraces, inputGesture.Name, inputGesture.GestureType, inputGesture.ExpectedAs) { }

    }
}
