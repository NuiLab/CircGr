using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGRLibrary
{
    public class CircGesture : Gesture
    {
        public enum Directions
        {
            U, //observation direction  U,D,L,R and its combinations 
            D,
            L,
            R,
            UL,
            UR,
            DL,
            DR,
            I, // Centripetal Direction: Inside- toward a centroid
            O, //outside - away from a centroid
            //C, //directions relative to the centroid
            CW,//clockwise movement
            CCW //counter-clockwise movement
            //COMPLEMENT //observation's direction is the  exact opposite of previous one
        }
       


        public int NumOfAnchors
        {
            get
            {
                return anchorPoints.Count;
            }
        }
        
        public int NumOfFingers
        {
            get
            {
                return Traces.Count + anchorPoints.Count;
            }
        }



        public List<double> OBSERVATIONS
        {
            get
            {
                List<double> obs = new List<double>();

                foreach(TraceData t in Traces.Values)
                {
                    obs.AddRange(t.OBSERVATIONS);
                }

                return obs;
            }
        }


        #region Accessory Structs/Classes
        
        public struct TraceData
        {
            //List of observations
            private List<double> obsevations;
            public ResultantVector resultant;

            public List<double> OBSERVATIONS
            {
                get
                {
                    return obsevations; 
                }
            }

            public TraceData(List<double> Observations,ResultantVector Resultant)
            {
                obsevations = Observations;
                resultant = Resultant;                                                    

            }


        }
        public struct ResultantVector
        {
            double angle;
            double magnitude;
            double dispersion;


            public double Angle
            {
                get
                {
                    return angle;
                }

                set
                {
                    angle = value;
                }
            }

            public double Magnitude
            {
                get
                {
                    return magnitude;
                }

                set
                {
                    magnitude = value;
                }
            }

            public double Dispersion
            {
                get
                {
                    return dispersion;
                }

                set
                {
                    dispersion = value;
                }
            }

            public ResultantVector(double Angle, double Magnitude, double Dispersion)
            {
                angle = Angle;
                magnitude = Magnitude;
                dispersion = Dispersion;
            }

            public override string ToString()
            {
                string s = "";
                s += "Resultant Angle:" + "\n";
                s += Angle + "\n";

                s += "Resultant Magnitude:" + "\n";
                s += Magnitude + "\n";

                s += "Resultant Dispersion:" + "\n";
                s += Dispersion + "\n";

                return s;
            }

        }

        public class DirectionalEvents
        {
            Dictionary<Directions, List<double>> observations;
            Dictionary<Directions, ResultantVector> resultants;
            Dictionary<Directions, double> percentages;
            

            public List<double> GetObservations(Directions dir)
            {
                if (observations.ContainsKey(dir))
                    return observations[dir];

                return new List<double>();
            }
            public ResultantVector GetResultant(Directions dir)
            {
                if (resultants.ContainsKey(dir))
                    return resultants[dir];

                return new ResultantVector(0,1,0);
            }

    #region Properties
            public List<double> UP
            {
                get
                {
                    return observations[Directions.U];
                }
            }

            public List<double> DOWN
            {
                get
                {
                    return observations[Directions.D];
                }
            }

            public List<double> LEFT
            {
                get
                {
                    return observations[Directions.L];
                }
            }

            public List<double> RIGHT
            {
                get
                {
                    return observations[Directions.R];
                }
            }

            public List<double> UP_RIGHT
            {
                get
                {
                    return observations[Directions.UR];
                }
            }

            public List<double> UP_LEFT
            {
                get
                {
                    return observations[Directions.UL];
                }
            }

            public List<double> DOWN_RIGHT
            {
                get
                {
                    return observations[Directions.DR];
                }
            }

            public List<double> DOWN_LEFT
            {
                get
                {
                    return observations[Directions.DL];
                }
            }

            public ResultantVector IN_RESULTANT
            {
                get
                {
                    if (resultants.ContainsKey(Directions.I))
                        return resultants[Directions.I];
                    else
                        return new ResultantVector(-1, -1, -1);
                }
            }
            
            //public ResultantVector C_RESULTANT
            //{
            //    get
            //    {
            //        if (resultants.ContainsKey(Directions.C))
            //            return resultants[Directions.C];
            //        else
            //            return new ResultantVector(-1, -1, -1);
            //    }
            //}


            public double IN_PERCENT
            {
                get
                {
                    if (percentages.ContainsKey(Directions.I))
                        return percentages[Directions.I];
                    else
                        return 0;
                }
            }

            public double OUT_PERCENT
            {
                get
                {
                    if (percentages.ContainsKey(Directions.O))
                        return percentages[Directions.O];
                    else
                        return 0;
                }
            }

            public double R_PERCENT
            {
                get
                {
                    if (percentages.ContainsKey(Directions.R))
                        return percentages[Directions.R];
                    else
                        return 0;
                }
            }
            public double L_PERCENT
            {
                get
                {
                    if (percentages.ContainsKey(Directions.L))
                        return percentages[Directions.L];
                    else
                        return 0;
                }
            }
            public double U_PERCENT
            {
                get
                {
                    if (percentages.ContainsKey(Directions.U))
                        return percentages[Directions.U];
                    else
                        return 0;
                }
            }
            public double D_PERCENT
            {
                get
                {
                    if (percentages.ContainsKey(Directions.D))
                        return percentages[Directions.D];
                    else
                        return 0;
                }
            }

            public double UR_PERCENT
            {
                get
                {
                    if (percentages.ContainsKey(Directions.UR))
                        return percentages[Directions.UR];
                    else
                        return 0;
                }
            }
            public double UL_PERCENT
            {
                get
                {
                    if (percentages.ContainsKey(Directions.UL))
                        return percentages[Directions.UL];
                    else
                        return 0;
                }
            }
            public double DR_PERCENT
            {
                get
                {
                    if (percentages.ContainsKey(Directions.DR))
                        return percentages[Directions.DR];
                    else
                        return 0;
                }
            }
            public double DL_PERCENT
            {
                get
                {
                    if (percentages.ContainsKey(Directions.DL))
                        return percentages[Directions.DL];
                    else
                        return 0;
                }
            }

            public double IN_ANGLE
            {
                get
                {
                    if (resultants.ContainsKey(Directions.I)){
                        return resultants[Directions.I].Angle;
                    }
                    else
                    {
                        return -1;
                     }
                }
            }

            #endregion

            private DirectionalEvents(Dictionary<Directions, List<double>> o, Dictionary<Directions,ResultantVector> r, Dictionary<Directions,double> p)
            {
                observations = o;
                resultants = r;
                percentages = p;
            }
            
            public DirectionalEvents(Dictionary<Directions, List<double>> o, Dictionary<Directions, ResultantVector> r)
            {
                observations = o;
                resultants = r;
                percentages = new Dictionary<Directions, double>();
            }

            

 
            public static DirectionalEvents  CreateDirectionalEvents(List<double> traceObservations)
            {
                Dictionary<Directions, List<double>> observations = new Dictionary<Directions, List<double>>();

                foreach (double o in traceObservations){
                    Directions d = GetDirection(o);

                    if (observations.ContainsKey(d))
                    {
                        observations[d].Add(o);
                    }
                    else
                    {
                        observations.Add(d, new List<double>());
                        observations[d].Add(o);
                    }                   

                }


                Dictionary < Directions, ResultantVector> resultants = new Dictionary<Directions, ResultantVector>();
                Dictionary < Directions, double> percentages = new Dictionary<Directions, double>();
                               

                foreach (Directions dir in observations.Keys)
                {
                    resultants.Add(dir, CalculateResultant(observations[dir]));
                    percentages.Add(dir, observations[dir].Count / traceObservations.Count);
                }

                return new DirectionalEvents(observations, resultants, percentages);
            }

            public DirectionalEvents()
            {
                observations = new Dictionary<Directions, List<double>>();
                //resultants = new Dictionary<Directions, ResultantVector>();
                //percentages = new Dictionary<Directions, double>();
            }

            public void LogDirection(Directions direction , double observation)
            {
                if (observations.ContainsKey(direction))
                    observations[direction].Add(observation);
                else
                {
                    observations.Add(direction, new List<double>());
                    observations[direction].Add(observation);
                }
            }

            public void CalculateResultants()
            {
                resultants = new Dictionary<Directions, ResultantVector>();
                foreach (Directions dir in observations.Keys)
                {
                    resultants.Add(dir, CalculateResultant(observations[dir]));
                    
                }
            }

            internal void CalculatePercentages(int numTraces)
            {
                percentages = new Dictionary<Directions, double>();
                foreach (Directions dir in observations.Keys)
                {
                    percentages.Add(dir, (double) observations[dir].Count / (numTraces * (SAMPLING_RESOLUTION - 1)));
                }
            }




            public override string ToString()
            {
                string s = "";

                s += "Directional Observations:\n";
                foreach (Directions dir in observations.Keys)
                {
                    s += "Direction: " + dir + "\n";
                    List<double> obs = observations[dir];

                    foreach (double o in obs)
                        s += o + "\n";
                }

                s += "Directional Resultants:\n";
                foreach (Directions dir in resultants.Keys)
                {
                    s += "Direction: " + dir + "\n";
                    ResultantVector resultant = resultants[dir];

                    s += "Resultant Angle:" + "\n";
                    s += resultant.Angle + "\n";

                    s += "Resultant Magnitude:" + "\n";
                    s += resultant.Magnitude + "\n";

                    s += "Resultant Dispersion:" + "\n";
                    s += resultant.Dispersion + "\n";

                }

                s += "Directional Percentages:\n";
                foreach (Directions dir in percentages.Keys)
                {
                    s += "Direction: " + dir + " Percentage: " + percentages[dir] + "\n";
                                     
                        
                }
                return s;
            }

            public double getPercentage(Directions direction)
            {
                if (percentages.ContainsKey(direction))
                    return percentages[direction];
                else
                    return 0;
            }

            public string percentToString()
            {
                string s = "Directional Percentages:\n";
                foreach (Directions dir in percentages.Keys)
                {
                    s += "Direction: " + dir + " Percentage: " + percentages[dir] + "\n";


                }
                return s;
            }


        }
        

        public struct TemporalEvents
        {
            Dictionary<Directions, List<double>> events;
            long startTime, endTime, duration;
            Dictionary<Directions, ResultantVector> resultants;

            public long TOTAL_DURATION
            {
                get
                {
                    return duration;
                }
            }


            public TemporalEvents(long startTime, long endTime)
            {
                events = new Dictionary<Directions, List<double>>();
                resultants = new Dictionary<Directions, ResultantVector>();
                this.startTime = startTime;
                this.endTime = endTime;
                duration = endTime - startTime; 

            }

            public TemporalEvents(long startTime, long endTime, Dictionary<Directions, List<double>> temporalEvents)
            {
                events = temporalEvents;
                this.startTime = startTime;
                this.endTime = endTime;
                resultants = new Dictionary<Directions, ResultantVector>();
                duration = endTime - startTime;
            }

            public List<double> GetEvents(Directions dir)
            {
                if (events.ContainsKey(dir))
                    return events[dir];

                return new List<double>();
            }
            public ResultantVector GetEventResultant(Directions dir)
            {
                if (resultants.ContainsKey(dir))
                    return resultants[dir];

                return new ResultantVector(0, 1, 0);
            }


            public void LogEvent(Directions direction, double time)
            {
                if (time > endTime || time < startTime)
                    throw new ArgumentException("Invalid Time");

                //generate a nomalized 0 - 360 value for a time range, 360 = 0 on the circle but it can help separate begging observations (0) to ending (360)
                double observation = ((time - startTime) / duration) * 360;

                observation *= (Math.PI / 180);

                if (events.ContainsKey(direction))
                {                    
                    events[direction].Add(observation);
                }
                else
                {
                    events.Add(direction, new List<double>());
                    events[direction].Add(observation);
                }

            }

            public void CalculateResultants()
            {
                resultants = new Dictionary<Directions, ResultantVector>();
                foreach (Directions dir in events.Keys)
                {
                    resultants.Add(dir, CalculateResultant(events[dir]));

                }
            }

            public override string ToString()
            {
                string s = "";

                s += "Temporal Events:\n";
                foreach (Directions dir in events.Keys)
                {
                    s += "Direction: " + dir + "\n";
                    List<double> obs = events[dir];

                    foreach (double o in obs)
                        s += o + "\n";
                }


                return s;
            }
            
            public string resultantsToString()
            {
                string s = "Resultants\n ";

                foreach (Directions dir in resultants.Keys)
                {
                    s += dir + "-\n";
                    s += resultants[dir];
                    //s += resultants[dir].Dispersion + " ";
                }

                return s;
            } 

        }

        #endregion

        //Defines how large the margin, in degrees, around U,D,R,L should be.
        public const int DIRECTIONAL_MARGIN= 10;
        public List<Point> anchorPoints;
        public Dictionary<int, TraceData> Traces;
        public ResultantVector GestureResultant;
        public TemporalEvents temporalEvents;
        public DirectionalEvents directionalEvents;
        public bool strongResultant = false;

        
        //Main Construct O(n)
        public CircGesture(PointMap points, string gestureName = "", eGestureType gestureType = eGestureType.Template) : base(points,gestureName, gestureType)
        {
            Traces = new Dictionary<int, TraceData>();
            anchorPoints = new List<Point>();
            List<int> anchorIDs = new List<int>();
            List<int> activeTraceIDs = new List<int>();

            List<int> traceIDs = points.TraceIDs;

            Dictionary<Directions, List<double>> directionalEvents = new Dictionary<Directions, List<double>>();
            Dictionary<Directions, List<double>> temporalEvents = new Dictionary<Directions, List<double>>();
            Dictionary<Directions, ResultantVector> dirResultants = new Dictionary<Directions, ResultantVector>();
            Dictionary<Directions, double> dirS = new Dictionary<Directions, double>();
            Dictionary<Directions, double> dirC = new Dictionary<Directions, double>();
            Dictionary<Directions, int> dirCounts = new Dictionary<Directions, int>();
            Dictionary<int, List<double>> observations = new Dictionary<int, List<double>>();



            var pathInfo = PathInfo(points);
            Dictionary<int, double> pathLengths = pathInfo.Item1;
            Dictionary<int, Point> centroids = pathInfo.Item2;
            Dictionary<int, Point> firstPoints = pathInfo.Item3;
            Dictionary<int, int> idCounts = pathInfo.Item4;
            long startTime = pathInfo.Item5;
            long endTime = pathInfo.Item6;

            Dictionary<int, double> intervalLengths = new Dictionary<int, double>();


            foreach (int id in traceIDs)
            {
                double I = pathLengths[id] / (SAMPLING_RESOLUTION - 1);

                if (I < 0.5)
                {
                    anchorPoints.Add(centroids[id]);
                    anchorIDs.Add(id);
                }
                else
                {
                    intervalLengths.Add(id, I);                    
                    activeTraceIDs.Add(id);
                    observations.Add(id, new List<double>());
                }

            }

            //serves as the centroid of the entire gesture
            Point gestureCentroid;

            if (anchorPoints.Count == 0)
            {
                gestureCentroid = CalculateCentroid(firstPoints.Values.ToList());
            }
            else
            {
                gestureCentroid = CalculateCentroid(anchorPoints);
            }


            foreach (int id in activeTraceIDs)
            {
                List<Point> pts = points[id];

                Traces.Add(id,ProcessTrace(pts, intervalLengths[id], gestureCentroid, directionalEvents, temporalEvents, dirCounts, dirC, dirS,startTime,endTime));
            }


            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                if (!dirC.ContainsKey(dir))
                    continue;
                double C = dirC[dir];
                double S = dirS[dir];

                double angle = Math.Atan2(S, C);

                if (angle < 0)
                    angle += 2 * Math.PI;

                double magnitude = Math.Sqrt(Math.Pow(C, 2) + Math.Pow(S, 2));
                ResultantVector r = new ResultantVector(angle, magnitude, dirCounts[dir] - magnitude);
                dirResultants.Add(dir, r);
            }

            //calculate gesture resultant
            double gC = 0;
            double gS = 0;            

            foreach (int id in activeTraceIDs)
            {
                double angle = Traces[id].resultant.Angle;
                gC += Math.Cos(angle);
                gS += Math.Sin(angle);
            }


            double gAngle = Math.Atan2(gS, gC);
            if (gAngle < 0)
                gAngle += 2 * Math.PI;

            double gMagnitude = Math.Sqrt(Math.Pow(gC, 2) + Math.Pow(gS, 2));

            GestureResultant = new ResultantVector(gAngle, gMagnitude, traceIDs.Count - gMagnitude);

            this.temporalEvents = new TemporalEvents(startTime, endTime, temporalEvents);
            this.directionalEvents = new DirectionalEvents(directionalEvents, dirResultants);

        }

        //Consstruct a CircGesture from gesture object
        public CircGesture(Gesture gesture) : this(gesture.rawTraces, gesture.Name, gesture.GestureType) {
            this.ExpectedAs = gesture.ExpectedAs;
        }

        private TraceData ProcessTrace(List<Point> points, double I, Point gestureCentroid, Dictionary<Directions, List<double>> directionalEvents, Dictionary<Directions, List<double>> temporalEvents, Dictionary<Directions, int> dirCounts, Dictionary<Directions,double> dirC, Dictionary<Directions, double> dirS,long startTime, long endTime)
        {

            double D = 0,C = 0 ,S = 0;
            
            double prevObservation = -1;
            Directions prevDir = Directions.CCW; //for Complement Direction
            List<double> observations = new List<double>();
            
            for (int i = 1; i < points.Count; i++)
            {
                //Console.WriteLine("Processing Point {0} ID: {1} X:{2} Y:{3} T:{4}", i, points[i].StrokeID, points[i].X, points[i].Y, points[i].timestamp);
                    
                double d = Geometry.EuclideanDistance(points[i - 1], points[i]);
                //Console.WriteLine("\t d = {0}", d);
                if (D + d >= I)
                {
                    Point prevPt = points[i - 1];
                    while (D + d >= I)
                    {
                        // add interpolated point
                        double t = Math.Min(Math.Max((I - D) / d, 0.0f), 1.0f);
                        Point nPt = new Point(
                                  (1.0f - t) * prevPt.X + t * points[i].X,
                                  (1.0f - t) * prevPt.Y + t * points[i].Y,
                                  points[i].StrokeID,
                                  points[i].timestamp
                              );

                        double observation = CalculateObservation(prevPt, nPt);
                        observations.Add(observation);
                        double c = Math.Cos(observation);
                        double s = Math.Sin(observation);
                        C += c;
                        S += s;
                        Directions direction = GetDirection(observation);
                        Directions centripetalDirection = GetCentripetalDirection(prevPt, nPt, gestureCentroid);
                        UpdateCounts(dirCounts, direction);
                        UpdateCounts(dirCounts, centripetalDirection);
                        UpdateResultants(dirC, direction, c);
                        UpdateResultants(dirS, direction, s);
                        UpdateResultants(dirC, centripetalDirection, c);
                        UpdateResultants(dirS, centripetalDirection, s);

                        UpdateEventsMaps(directionalEvents, direction, observation);
                        UpdateEventsMaps(directionalEvents, centripetalDirection, observation);

                        double temporalObservation = CalculateTemporalObservation(nPt.timestamp, startTime, endTime);

                        UpdateEventsMaps(temporalEvents, direction, temporalObservation);
                        UpdateEventsMaps(temporalEvents, centripetalDirection, temporalObservation);



                        if (!(prevObservation < 0))
                        {
                            Directions clockDirection = GetRotationalDirection(prevObservation, observation);
                            UpdateResultants(dirC, clockDirection, c);
                            UpdateResultants(dirS, clockDirection, s);

                            UpdateEventsMaps(directionalEvents, clockDirection, observation);
                            UpdateEventsMaps(temporalEvents, clockDirection, temporalObservation);
                            UpdateCounts(dirCounts, clockDirection);

                            //Complement direction is disabled for now, was concived after experiment
                            //if (IsComplement(direction, prevDir))
                            //{
                            //    UpdateResultants(dirC, Directions.COMPLEMENT, c);
                            //    UpdateResultants(dirS, Directions.COMPLEMENT, s);
                            //    UpdateEventsMaps(directionalEvents, Directions.COMPLEMENT, observation);
                            //    UpdateEventsMaps(temporalEvents, Directions.COMPLEMENT, temporalObservation);
                            //    UpdateCounts(dirCounts, Directions.COMPLEMENT);
                            //}

                        }

                        prevObservation = observation;
                        prevDir = direction;

                        // update partial length
                        d = D + d - I;
                        //Console.WriteLine("\t\t Updated d={0} stepDistance(D)={1} ", d, D);
                        D = 0;
                        prevPt = nPt;
                    }
                    D = d;
                    //Console.WriteLine("\t\tD going out = {0}", D);
                }
                else
                {
                    D += d;
                    //Console.WriteLine("\tWhile-Loop skipped D={0}", D);
                }
              }

            double angle = Math.Atan2(S, C);

            if (angle < 0)
                angle += 2 * Math.PI;

            double magnitude = Math.Sqrt(Math.Pow(C, 2) + Math.Pow(S, 2));
            return new TraceData(observations, new ResultantVector(angle, magnitude, observations.Count - magnitude));
        }
                
        public Tuple<Dictionary<int, double>, Dictionary<int, Point>, Dictionary<int, Point>, Dictionary<int, int>, long, long> PathInfo(PointMap points)
        {
            Dictionary<int, double> pathLengths = new Dictionary<int, double>();
            Dictionary<int, Point> centroids = new Dictionary<int, Point>();
            Dictionary<int, Point> firstPoints = new Dictionary<int, Point>();
            Dictionary<int, int> idCounts = new Dictionary<int, int>();

            long startTime = long.MaxValue;
            long endTime = long.MinValue;

            foreach(int traceId in points.TraceIDs)
            {
                List<Point> pts = points[traceId];
                Point prev = pts[0];
                firstPoints.Add(traceId, prev);
                centroids.Add(traceId, new Point(prev.X, prev.Y, traceId, -1));
                idCounts.Add(traceId, 1);
                pathLengths.Add(traceId, 0);
               
                for (int i = 1; i < pts.Count; i++)
                {
                    Point p = pts[i];

                    //update the amount of points with this id
                    idCounts[traceId] += 1;

                    //update the centroid for a particular trace id
                    centroids[traceId].X += p.X;
                    centroids[traceId].Y += p.Y;

                    //update start and end time
                    if (p.timestamp > endTime)
                        endTime = p.timestamp;
                    if (p.timestamp < startTime)
                        startTime = p.timestamp;


                    //update pathLength
                    pathLengths[traceId] += Geometry.EuclideanDistance(prev, p);

                    prev = p;

                }
                Point c = centroids[traceId];
                centroids[traceId] = new Point(c.X / idCounts[traceId], c.Y / idCounts[traceId], c.StrokeID, c.timestamp);

            }

            return new Tuple<Dictionary<int, double>, Dictionary<int, Point>, Dictionary<int, Point>, Dictionary<int, int>, long, long>(pathLengths, centroids, firstPoints, idCounts, startTime, endTime);
        }


        private void UpdateCounts(Dictionary<Directions, int> dirCounts, Directions direction)
        {
            if (dirCounts.ContainsKey(direction))
                dirCounts[direction] += 1;
            else
                dirCounts.Add(direction, 1);
        }

        /// <summary>
        /// Small utility function to update the various maps/dictionary
        /// </summary>
        /// <param name="eventMap"></param>
        /// <param name="direction"></param>
        /// <param name="observation"></param>
        private void UpdateEventsMaps(Dictionary<Directions,List<double>> eventMap, Directions direction, double observation)
        {
            if (eventMap.ContainsKey(direction))
            {
                eventMap[direction].Add(observation);
            }
            else
            {
                eventMap.Add(direction, new List<double>());
                eventMap[direction].Add(observation);
            }
        }


        //annonymous function used to update the resultant vectors, its either Cos(x) or Sin(x)
        private delegate double UpdateFunction(double observation);

        private void UpdateResultants(Dictionary<Directions,double> dirResultant, Directions direction ,double observation, UpdateFunction function)
        {
            if (dirResultant.ContainsKey(direction))
            {
                dirResultant[direction] += function(observation);
            }
            else
            {
                dirResultant.Add(direction, function(observation));

            }
        }

        private void UpdateResultants(Dictionary<Directions, double> dirResultant, Directions direction, double value)
        {
            if (dirResultant.ContainsKey(direction))
            {
                dirResultant[direction] += value;
            }
            else
            {
                dirResultant.Add(direction, value);

            }
        }





        /// <summary>
        /// Calculate an observation based on a time interval
        /// </summary>
        /// <param name="time"></param>
        /// <param name="startTime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        private double CalculateTemporalObservation(long time, long startTime, long endtime)
        {
            if (time > endtime || time < startTime)
                throw new Exception("Time not in range");

            return ((time - startTime) / (endtime - startTime)) * 2 *Math.PI;
        }

           

        private Directions GetRotationalDirection(double prev, double curr)
        {
            //work in degress for the mean while
            double p_angle = prev * (180 / Math.PI);
            double c_angle = curr * (180 / Math.PI);

            double max_ccw = p_angle + 180;

            //test if P_angle + 180 crossed 0
            if(max_ccw <= 359)
            {
                if (p_angle <= c_angle && c_angle <= max_ccw)
                    return Directions.CCW;
                else
                    return Directions.CW;
            }
            else
            {
                //angle between p_angle and 0
                double split = 360 - p_angle;
                //angle between 0 and max_ccw
                double adjusted = max_ccw - 360;

                if ((p_angle <= c_angle && c_angle <= 360) || (0 <= c_angle && c_angle <= adjusted))
                    return Directions.CCW;
                else
                    return Directions.CW;
                

            }
        }

        private ResultantVector CalculateGestureResultant(Dictionary<int, TraceData> traces)
        {
            if (traces.Count < 2)
                return traces[traces.Keys.ElementAt(0)].resultant;

            double maxMagnitude = double.MinValue;

            //find max Magnitude for normalization
            foreach (TraceData trace in traces.Values)
            {
                if (trace.resultant.Magnitude > maxMagnitude)
                    maxMagnitude = trace.resultant.Magnitude;
            }

            double C = 0;
            double S = 0;


            foreach (TraceData trace in traces.Values)
            {
                //contributions to the gesture resultant trace are weighted by the magnitude, the higher the dispersion, the less it contributes
                //magnitude is a measure of dispersion in this case because all traces have the same N (number of observations) in Dispersion = N - Magnitude
                C += (trace.resultant.Magnitude/ maxMagnitude) * Math.Cos(trace.resultant.Angle);
                S += (trace.resultant.Magnitude / maxMagnitude) * Math.Sin(trace.resultant.Angle);
            }

            double angle = Math.Atan2(S, C);

            if (angle < 0)
                angle += 2 * Math.PI;

            double magnitude = Math.Sqrt(Math.Pow(C, 2) + Math.Pow(S, 2));
                        
            return new ResultantVector(angle, magnitude, traces.Count - magnitude);
            
        }

        public static Directions GetCentripetalDirection(Point prev, Point cur, Point center)
        {
            if (Geometry.EuclideanDistance(prev, center) > Geometry.EuclideanDistance(cur, center))
                return Directions.I;
            return Directions.O;
        }


        public static Directions GetDirection(double observation)
        {
            //This method deals with degrees because its easier to deal with
            double o = observation * (180 / Math.PI);

            if (o > 180) //UP region
            {

                if (o >= 360 - DIRECTIONAL_MARGIN)
                    return Directions.R;

                if (o >= 270 + DIRECTIONAL_MARGIN)
                    return Directions.UR;

                if (o >= 270 - DIRECTIONAL_MARGIN)
                    return Directions.U;

                if (o >= 180 + DIRECTIONAL_MARGIN)
                    return Directions.UL;

                return Directions.L;             


            } 
            else //DOWN Region
            {
                if (o >= 180 - DIRECTIONAL_MARGIN)
                    return Directions.L;

                if (o >= 90 + DIRECTIONAL_MARGIN)
                    return Directions.DL;

                if (o >= 90 - DIRECTIONAL_MARGIN)
                    return Directions.D;

                if (o >= 0 + DIRECTIONAL_MARGIN)
                    return Directions.DR;

                return Directions.R;
            }



        }

        /// <summary>
        /// Calculates the angle of an observation generated from a point and its subsequent point.
        /// </summary>
        /// <param name="prev">The reference, or "before" point.</param>
        /// <param name="cur">The point on which the observation will be made.</param>
        /// <returns>Observation in radians</returns>
        public static double CalculateObservation(Point prev, Point cur)
        {
            Point o = new Point(cur.X - prev.X, cur.Y - prev.Y);

            double angle = Math.Atan2(o.Y, o.X);

            if (angle < 0)
                angle += (2 * Math.PI);

            return angle;
        }

        /// <summary>
        /// jkhkvasdf
        /// </summary>
        /// <param name="observations"></param>
        /// <returns></returns>
        public static ResultantVector CalculateResultant(List<double> observations)
        {
            double C = 0;
            double S = 0;

            foreach(double o in observations)
            {
                C += Math.Cos(o);
                S += Math.Sin(o);
            }

            double angle = Math.Atan2(S, C);

            if (angle < 0)
                angle += 2 * Math.PI;

            double magnitude = Math.Sqrt(Math.Pow(C, 2) + Math.Pow(S, 2));

            return new ResultantVector(angle, magnitude, observations.Count - magnitude);
        }


        private Point CalculateCentroid(List<Point> anchorPoints)
        {
            double cx = 0, cy = 0;                     

            foreach (Point p in anchorPoints)
            {
                cx += p.X;
                cy += p.Y;
            }

            return new Point(cx / anchorPoints.Count, cy / anchorPoints.Count);
        }

        private Point CalculateCentroid(Dictionary<int, Point[]> resampledPoints)
        {
            double cx = 0, cy = 0;

            int numTraces = resampledPoints.Count;

            foreach(int key in resampledPoints.Keys)
            {
                cx += resampledPoints[key][0].X;
                cy += resampledPoints[key][0].Y;
            }

            return new Point(cx / numTraces, cy / numTraces);
        }


        private bool IsComplement(Directions A, Directions B)
        {
            if (A == B)
                return false;


            if (A == Directions.U)
                return B == Directions.D;
            if (A == Directions.D)
                return B == Directions.U;
            if (A == Directions.UL)
                return B == Directions.DR;
            if (A == Directions.UR)
                return B == Directions.DL;

            if (A == Directions.DL)
                return B == Directions.UR;
            if (A == Directions.DR)
                return B == Directions.UL;
            if (A == Directions.L)
                return B == Directions.R;
            if (A == Directions.R)
                return B == Directions.L;

            return false;
        }


        public override string ToString()
        {
            string s = "";

            s += "# Anchor Points: " + anchorPoints.Count + "\n";

            foreach (Point pt in anchorPoints)
            {
                s += " X:" + pt.X + " Y:" + pt.Y + "\n";
            }


            s += "# of Traces: " + Traces.Count + "\n";

            foreach (int tId in Traces.Keys)
            {
                s += "T.ID: " + tId + "\n";

                TraceData t = Traces[tId];

                s += "Observations:" + "\n";

                foreach (double o in t.OBSERVATIONS)
                {
                    s += o + "\n";
                }


                s += t.resultant;

            }

            //s += "Gesture Resultant\n";
            s += GestureResultant;

            s += directionalEvents.percentToString();
            s += temporalEvents.resultantsToString();

            s += directionalEvents;



            return s; 
        }

        public double getPercentage(Directions direction)
        {
            return this.directionalEvents.getPercentage(direction);
        }


    }



}
