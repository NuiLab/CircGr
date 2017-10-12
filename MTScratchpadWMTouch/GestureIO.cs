using System.IO;
using System.Collections.Generic;
using System.Xml;
using M = MTGRLibrary;
using System.Linq;

namespace MTScratchpadWMTouch
{
    public class GestureIO
    {


        //public static M.Gesture ReadGesture(string fileName, out M.MapPoints originalTraces)
        //{
        //    string expectedAs = ""; //What type of gesture this gesture is supposed to be is not important in this call
        //    return ReadGesture(fileName, out originalTraces, out expectedAs);

        //}

        //public static M.Gesture ReadGesture(string fileName, out  M.MapPoints originalTraces, out string expectedAs, M.Gesture.eGestureType gestureType = M.Gesture.eGestureType.Template)
        //{

        //    XmlTextReader xmlReader = null;
        //    int currentStrokeIndex = -1;
        //    string gestureName = "";
        //    expectedAs = "";

        //    M.MapPoints traces = new M.MapPoints();

        //    try
        //    {
        //        xmlReader = new XmlTextReader(File.OpenText(fileName));
        //        while (xmlReader.Read())
        //        {
        //            if (xmlReader.NodeType != XmlNodeType.Element) continue;
        //            switch (xmlReader.Name)
        //            {
        //                case "Gesture":
        //                    gestureName = xmlReader["Name"];
        //                    if (gestureName.Contains("~")) // '~' character is specific to the naming convention of the MMG set
        //                        gestureName = gestureName.Substring(0, gestureName.LastIndexOf('~'));
        //                    if (gestureName.Contains("_")) // '_' character is specific to the naming convention of the MMG set
        //                        gestureName = gestureName.Replace('_', ' ');
        //                    break;
        //                case "Label": //from Experiment output
        //                    expectedAs = xmlReader["Expected"];
        //                    if (expectedAs.Contains("~")) // '~' character is specific to the naming convention of the MMG set
        //                        expectedAs = expectedAs.Substring(0, expectedAs.LastIndexOf('~'));
        //                    //if (expectedAs.Contains("_")) // '_' character is specific to the naming convention of the MMG set
        //                    //    expectedAs = expectedAs.Replace('_', ' ');
        //                    break;
        //                case "Expected": //from normal output
        //                    expectedAs = xmlReader["As"];
        //                    if (expectedAs.Contains("~")) // '~' character is specific to the naming convention of the MMG set
        //                        expectedAs = expectedAs.Substring(0, expectedAs.LastIndexOf('~'));
        //                    //if (expectedAs.Contains("_")) // '_' character is specific to the naming convention of the MMG set
        //                    //    expectedAs = expectedAs.Replace('_', ' ');
        //                    break;
        //                case "Stroke":
        //                    currentStrokeIndex++;
        //                    break;
        //                case "Point":
        //                    traces.Add(int.Parse(xmlReader["T"]), new M.Point(
        //                        float.Parse(xmlReader["X"]),
        //                        float.Parse(xmlReader["Y"]),
        //                        int.Parse(xmlReader["T"]),
        //                        long.Parse(xmlReader["Timestamp"])
        //                    ));
        //                    break;
        //            }
        //        }
        //    }
        //    catch (System.Xml.XmlException e)
        //    {
        //        // System.W
        //    }
        //    finally
        //    {
        //        if (xmlReader != null)
        //            xmlReader.Close();
        //    }


        //    originalTraces = traces;

        //    //M.Points pts = traces.concatPoint();
        //    //originalPts = pts.getRangeList(0, pts.Count);

        //    return new M.Gesture(traces, gestureName,gestureType);
        //}

        //public static M.PointMap ReadGesture(string fileName, out string name, out string expectedAs, M.Gesture.eGestureType gestureType = M.Gesture.eGestureType.Template)
        //{
        //    XmlTextReader xmlReader = null;
        //    int currentStrokeIndex = -1;
        //    string gestureName = "";
        //    expectedAs = "";
        //    M.PointMap ptMap = new M.PointMap();


        //    try
        //    {
        //        xmlReader = new XmlTextReader(File.OpenText(fileName));
        //        while (xmlReader.Read())
        //        {
        //            if (xmlReader.NodeType != XmlNodeType.Element) continue;
        //            switch (xmlReader.Name)
        //            {
        //                case "Gesture":
        //                    gestureName = xmlReader["Name"];
        //                    if (gestureName.Contains("~")) // '~' character is specific to the naming convention of the MMG set
        //                        gestureName = gestureName.Substring(0, gestureName.LastIndexOf('~'));
        //                    if (gestureName.Contains("_")) // '_' character is specific to the naming convention of the MMG set
        //                        gestureName = gestureName.Replace('_', ' ');
        //                    break;
        //                case "Label": //from Experiment output
        //                    expectedAs = xmlReader["Expected"];
        //                    if (expectedAs.Contains("~")) // '~' character is specific to the naming convention of the MMG set
        //                        expectedAs = expectedAs.Substring(0, expectedAs.LastIndexOf('~'));
        //                    if (expectedAs.Contains("_")) // '_' character is specific to the naming convention of the MMG set
        //                        expectedAs = expectedAs.Replace('_', ' ');
        //                    break;
        //                case "Expected": //from normal output
        //                    expectedAs = xmlReader["As"];
        //                    if (expectedAs.Contains("~")) // '~' character is specific to the naming convention of the MMG set
        //                        expectedAs = expectedAs.Substring(0, expectedAs.LastIndexOf('~'));
        //                    if (expectedAs.Contains("_")) // '_' character is specific to the naming convention of the MMG set
        //                        expectedAs = expectedAs.Replace('_', ' ');
        //                    break;
        //                case "Stroke":
        //                    currentStrokeIndex++;
        //                    break;
        //                case "Point":
        //                    ptMap.Add(new M.Point(
        //                        float.Parse(xmlReader["X"]),
        //                        float.Parse(xmlReader["Y"]),
        //                        int.Parse(xmlReader["T"]),
        //                        long.Parse(xmlReader["Timestamp"])
        //                    ));
        //                    break;
        //            }
        //        }
        //    }
        //    catch (System.Xml.XmlException e)
        //    {
        //        System.W
        //    }
        //    finally
        //    {
        //        if (xmlReader != null)
        //            xmlReader.Close();
        //    }

        //    name = gestureName;
        //    return ptMap;
        //}

        public static M.Gesture ReadGesture(string fileName, M.Gesture.eGestureType gestureType = M.Gesture.eGestureType.Template)
        {
            XmlTextReader xmlReader = null;
            int currentStrokeIndex = -1;
            string gestureName = "";
            string expectedAs = "";
            M.PointMap ptMap = new M.PointMap();
            
            try
            {
                xmlReader = new XmlTextReader(File.OpenText(fileName));
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element) continue;
                    switch (xmlReader.Name)
                    {
                        case "Gesture":
                            gestureName = xmlReader["Name"];
                            if (gestureName.Contains("~")) // '~' character is specific to the naming convention of the MMG set
                                gestureName = gestureName.Substring(0, gestureName.LastIndexOf('~'));
                            if (gestureName.Contains("_")) // '_' character is specific to the naming convention of the MMG set
                                gestureName = gestureName.Replace('_', ' ');
                            break;
                        case "Label": //from Experiment output
                            expectedAs = xmlReader["Expected"];
                            if (expectedAs.Contains("~")) // '~' character is specific to the naming convention of the MMG set
                                expectedAs = expectedAs.Substring(0, expectedAs.LastIndexOf('~'));
                            //if (expectedAs.Contains("_")) // '_' character is specific to the naming convention of the MMG set
                            //    expectedAs = expectedAs.Replace('_', ' ');
                            break;
                        case "Expected": //from normal output
                            expectedAs = xmlReader["As"];
                            if (expectedAs.Contains("~")) // '~' character is specific to the naming convention of the MMG set
                                expectedAs = expectedAs.Substring(0, expectedAs.LastIndexOf('~'));
                            //if (expectedAs.Contains("_")) // '_' character is specific to the naming convention of the MMG set
                            //    expectedAs = expectedAs.Replace('_', ' ');
                            break;
                        case "Stroke":
                            currentStrokeIndex++;
                            break;
                        case "Point":
                            ptMap.Add(new M.Point(
                                float.Parse(xmlReader["X"]),
                                float.Parse(xmlReader["Y"]),
                                int.Parse(xmlReader["T"]),
                                long.Parse(xmlReader["Timestamp"])
                            ));
                            break;
                    }
                }
            }
            catch (System.Xml.XmlException e)
            {
                // System.W
            }
            finally
            {
                if (xmlReader != null)
                    xmlReader.Close();
            }

            return new M.Gesture(ptMap, gestureName, gestureType, expectedAs);
        }


        /// <summary>
        /// Writes a multistroke gesture to an XML file
        /// </summary>
        public static void WriteGesture(M.Point[] points, string gestureName, string fileName, string expectedAs = "")
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
                

                sw.WriteLine("<Gesture Name = \"{0}\">", gestureName);

                // Used for automatic testing using Canned Gesture Feature.
                // Denotes what this gesture is supposed to be classified as.
                if (expectedAs.Equals(""))
                    sw.WriteLine("<Expected As = \"{0}\"/>", "");
                else
                    sw.WriteLine("<Expected As = \"{0}\"/>", expectedAs);

                int currentStroke = -1;
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i].StrokeID != currentStroke)
                    {
                        if (i > 0)
                            sw.WriteLine("\t</Stroke>");
                        sw.WriteLine("\t<Stroke>");
                        currentStroke = points[i].StrokeID;
                    }

                    sw.WriteLine("\t\t<Point X = \"{0}\" Y = \"{1}\" T = \"{2}\" Timestamp = \"{3}\" Pressure = \"0\" />",
                        points[i].X, points[i].Y, points[i].StrokeID ,points[i].timestamp
                    );
                }
                sw.WriteLine("\t</Stroke>");
                sw.WriteLine("</Gesture>");
            }
        }
        
    }
}