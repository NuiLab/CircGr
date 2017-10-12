// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

// MTScratchpadWMTouch application.
// Description:
//  Inside the application window, user can draw using multiple fingers
//  at the same time. The trace of each finger is drawn using different
//  color. The primary finger trace is always drawn in black, and the
//  remaining traces are drawn by rotating through the following colors:
//  red, blue, green, magenta, cyan and yellow.
//
// Purpose:
//  This sample demonstrates handling of the multi-touch input inside
//  a C# application using WM_TOUCH window message:
//  - Registering a window for multi-touch using RegisterTouchWindow,
//    IsTouchWindow.
//  - Handling WM_TOUCH messages and unpacking their parameters using
//    GetTouchInputInfo and CloseTouchInputHandle; reading touch contact
//    data from the TOUCHINPUT structure.
//  In addition, the sample also shows how to store and draw strokes
//  entered by user, using the helper classes Stroke and
//  CollectionOfStrokes.

//comment if not in testmode
//#define TESTMODE 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO; 
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks; 
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Microsoft.Samples.Touch.MTScratchpadWMTouch;
using M = MTGRLibrary;
using MTScratchpadWMTouch;

namespace MTScratchpadWMTouch
{
    // Main app, WMTouchForm-derived, multi-touch aware form.
    public partial class TouchPad : WMTouchForm
    {
        private FrmLog fLog;
        private bool modeTemplate = true;
        private bool holdGesture = true;
        private bool recognitionOn = true;
        private int gestureCount = 0;
        private int gestureTries = 1; 
        private int countPoints = 0;
        private int localPoints = 0;
        private int captureWindowCount = 0;
        private int testInstanceCount = 0;       
        public const int CANDIDATE_CLASSIFY_ONCOUNT = 64;
        private int candidateCount = 0;
        private M.PointMap pointMap = new M.PointMap();
        private int fingers = 0;
        private bool dollarMatchOn = true;
        /// <summary>
        /// Holds information about the previous point recived on a certain touch id by touch form.
        /// Used for filtering purposes.
        /// </summary>
        private Dictionary<int,WMTouchEventArgs> prevPoint = new Dictionary<int, WMTouchEventArgs>();
        /// <summary>
        /// Used to implement filtering. Point will be filterd if it does not change by more than specified
        /// delta.
        /// </summary>
        private double FILTER_DELTA = 1;
        /// <summary>
        /// Setting this to true makes all touch events pop up on the log window.
        /// </summary>
        private bool DISPLAY_TOUCH_DATA= true;
        /// <summary>
        /// If set to true, input map is cleared after every window of points is classified rather than
        /// adding them to the map.
        /// </summary>
        private bool dropWindows = false;
        /// <summary>
        /// Holds the gestures that define classess. TODO:REname
        /// </summary>
        private List<M.Gesture> Templates = new List<M.Gesture>();
       

        /// <summary>
        /// The amount of training Samples to collect.
        /// </summary>
        private int TRAINING_SAMPLES_TO_COLLECT = 10;
        /// <summary>
        /// The amount of training samples collected thus far.
        /// </summary>
        private int Training_SAMPLES_COLLECTED = 0;
        /// <summary>
        /// used to find the next gesture to train
        /// </summary>
        private int trainingPointer = 0;
       
        //for timestamp
        private DateTime UTC = new DateTime(1970, 1, 1);

        //for test modes, number of times no classification is returned
        private int numNoClass = 0;


        private ConfusionMatrix matrix; 
        private String expectedGesture = "";
        private GestureWindowPerformance windowPerformance;
        private bool modeTest = false;
        private bool modeTraining = false;
        //private M.ExcelWriter xlWriter;


        private bool modeExperiment = false;
        private Experiment experiment;
        private string lastClassification = "";
        private M.Gesture lastCandidate;
         


        private enum Recognizers  {CircGR, OneDollar, PDollar, RawFeatures};

        private Recognizers activeRecognizer;

        private M.Recognizer recognizer = null;
        
        private TouchColor touchColor;                  // Color generator
        private CollectionOfStrokes FinishedStrokes;    // Collection of finished strokes
        private CollectionOfStrokes ActiveStrokes;
        StreamWriter writer;
        private string expectedStr = "";
				private string pathSeparator;
                private string mainDirectory;
				private string gestureSubDirectory;
                private string candidateSubDirectory;
                private string cannedCandidatesSubDirectory;
                private string examplesSubDirectory;
				private string outputSubDirectory;
                private string experimentSubDirectory;
                
        //this list is for testing only (as of now) 
       
        #region "Touch Constructor and Events"
        // Constructor
        public TouchPad()
        {
            InitializeComponent();

            // Create data members, color generator and stroke collections.
            fLog = new FrmLog();
            fLog.Show();
            fLog.clearText();
            fLog.addLine("Log started on :" + DateTime.Now.ToString()); 
            touchColor = new TouchColor ();
            ActiveStrokes = new CollectionOfStrokes ();
            FinishedStrokes = new CollectionOfStrokes ();
          
            // Setup handlers
            Touchdown += OnTouchDownHandler;
            Touchup += OnTouchUpHandler;
            TouchMove += OnTouchMoveHandler;
            Paint += new PaintEventHandler(this.OnPaintHandler);

            // Set window background color
            this.BackColor = SystemColors.Window;
						
						switch(Environment.OSVersion.Platform) {
							case PlatformID.Unix:
								pathSeparator = Convert.ToString(Path.AltDirectorySeparatorChar);
								break;
							default:
								pathSeparator = Convert.ToString(Path.DirectorySeparatorChar);
								break;
						}

                        mainDirectory = pathSeparator + "GestureSet" + pathSeparator;

                        gestureSubDirectory = mainDirectory + "TemplateGestures" + pathSeparator;

                        candidateSubDirectory = mainDirectory + "CandidateOutput" + pathSeparator;

                        cannedCandidatesSubDirectory = mainDirectory + "CannedCandidateGestures" + pathSeparator;

                        examplesSubDirectory = mainDirectory + "TemplateExamples" + pathSeparator;

                        outputSubDirectory = mainDirectory + "Output" + pathSeparator;

                        experimentSubDirectory = mainDirectory + "Experiment" + pathSeparator;



            initWriter(); 


            //Default Recognizer
            activeRecognizer = Recognizers.CircGR;
            recognizer = new M.CircGR();
            loadGestures();
            fLog.addLine("Current Classifer: " + activeRecognizer.ToString());
        }

        public static string getPathSeparator()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    return Convert.ToString(Path.AltDirectorySeparatorChar);                    
                default:
                    return Convert.ToString(Path.DirectorySeparatorChar);
                    
            }
        }


        private void initWriter()
        {
            string d = DateTime.Now.Ticks.ToString();
            writer = new StreamWriter("exp" + d + ".txt");
            writer.WriteLine("# Experiment on " + DateTime.Now.ToString());
            writer.WriteLine("# Classifying every " + CANDIDATE_CLASSIFY_ONCOUNT +
                            " points");
            writer.WriteLine("Gesture#, Classification #, Total Points when tried," +
                             "Expected Gesture, Gesture Recognized");
           
        }
        private void writeDataLine(int tpoints, string grec)
        {
            writer.WriteLine(gestureCount.ToString() + ", " +
                             gestureTries.ToString() + ", " +
                             tpoints.ToString() + ", " +
                             (modeTest? expectedGesture:expectedStr) + ", " +  
                             grec + "");
            ++gestureTries;

        }
        private void closeWriter()
        {
            writer.Close(); 
        }
        // Touch down event handler.
        // Starts a new stroke and assigns a color to it. 
        // in:
        //      sender      object that has sent the event
        //      e           touch event arguments
        private void OnTouchDownHandler(object sender, WMTouchEventArgs e)
        {
            // We have just started a new stroke, which must have an ID value unique
            // among all the strokes currently being drawn. Check if there is a stroke
            // with the same ID in the collection of the strokes in drawing.
            Debug.Assert(ActiveStrokes.Get(e.Id) == null);

            addStroke(e.Id, e.IsPrimaryContact,e.LocationX,e.LocationY); 
            
            OnDown(e); 
            
        }
        // Touch move event handler.
        // Adds a point to the active stroke and draws new stroke segment.
        // in:
        //      sender      object that has sent the event
        //      e           touch event arguments
        private void OnTouchMoveHandler(object sender, WMTouchEventArgs e)
        {

            Stroke stroke = updateStroke(e.Id, e.LocationX, e.LocationY);
            // Partial redraw: only the last line segment
            updateGraphics(stroke);
            onMove(e);
        }

        // Touch up event handler.
        // Finishes the stroke and moves it to the collection of finished strokes.
        // in:
        //      sender      object that has sent the event
        //      e           touch event arguments
        private void OnTouchUpHandler(object sender, WMTouchEventArgs e)
        {
            // Find the stroke in the collection of the strokes in drawing
            // and remove it from this collection.

            removeStroke(e.Id);
            onUp(e);            

            // Request full redraw.
            endGraphics(); 
        }
        
        #endregion 

        #region "Stroke Functions and Draw Functions"
        private void updateGraphics(Stroke stroke)
        {
            Graphics g = this.CreateGraphics();
            
            //g.DrawString("Hello World!", DefaultFont, Brushes.Black, new Point(500,500));
            
            if (modeExperiment)
               experiment.UpdateExperimentUI(g,this.Size);

            stroke.DrawLast(g);
        }
        private void endGraphics()
        {
            Invalidate();
        }



        private void addStroke(int Id, bool IsPrimaryContact, int x, int y)
        {
            // Create new stroke, add point and assign a color to it.
            Stroke newStroke = new Stroke();
            newStroke.Color = touchColor.GetColor(IsPrimaryContact);
            newStroke.Id = Id;
            newStroke.Add(new Point(x,y)); 
            //addStroke stroke new collection
            ActiveStrokes.Add(newStroke);
            updateGraphics(newStroke);
        }

        private Stroke updateStroke(int Id, int x, int y)
        {
            // Find the stroke in the collection of the strokes in drawing.
            Stroke stroke = ActiveStrokes.Get(Id);
            Debug.Assert(stroke != null);

            // Add contact point to the stroke
            stroke.Add(new Point(x, y));
            return stroke;
        }
        private void removeStroke(int Id)
        {
            
            Stroke stroke = ActiveStrokes.Remove(Id);
            Debug.Assert(stroke != null);

            // Add this stroke to the collection of finished strokes.
            FinishedStrokes.Add(stroke);
        }


        #endregion 
        #region "Form Misc"

        // OnPaint event handler.
        // in:
        //      sender      object that has sent the event
        //      e           paint event arguments
        private void OnPaintHandler(object sender, PaintEventArgs e)
        {
            // Full redraw: draw complete collection of finished strokes and
            // also all the strokes that are currently in drawing.
            FinishedStrokes.Draw(e.Graphics);
            ActiveStrokes.Draw(e.Graphics);

            if (modeExperiment)
                experiment.UpdateExperimentUI(e.Graphics,this.Size);
        }

        // Attributes
    

        private void TouchPad_Load(object sender, EventArgs e)
        {
         
        }
        #endregion
        #region Input Aquisition ------ Multi-Touch

        public int getTime()
        {
            return (int)(DateTime.UtcNow - UTC).TotalSeconds;
        }



        private void incPoints()
        {
            countPoints++;
            localPoints++;
        }
        private void OnDown(WMTouchEventArgs e)
        {
            fingers++;
            //LogWM(e, "D");
            incPoints();
            M.Point p = new M.Point(e.LocationX, e.LocationY, e.Id, e.Time); //DateTimeOffset.Now.UtcTicks);
            pointMap.Add(p);

            UpdatePrevious(e);

            #if TESTMODE
                addToSList(p); 
            #endif
           
        }
        private void onMove(WMTouchEventArgs e)
        {
            if (FilterPoint(e))
                return;

            UpdatePrevious(e);
            incPoints();
            //LogWM(e, "M");
            M.Point p = new M.Point(e.LocationX, e.LocationY, e.Id, e.Time);//DateTimeOffset.Now.UtcTicks);
            pointMap.Add(p);

            if ( (localPoints >= (CANDIDATE_CLASSIFY_ONCOUNT)) && !modeTraining) //capture gestures continously when in training, rather than in parts
                runRecognition(); 
            #if TESTMODE
                addToSList(p);
            #endif
        }
        private void onUp(WMTouchEventArgs e)
        {
            incPoints();
            //LogWM(e, "U");
            M.Point p = new M.Point(e.LocationX, e.LocationY, e.Id, e.Time);//DateTimeOffset.Now.UtcTicks);
            pointMap.Add(p);


            fingers--; 
            #if TESTMODE
                    addToSList(p);
                    if (fingers == 0)
                        writeSList(); 
            #endif
            if ((localPoints >= (fingers * CANDIDATE_CLASSIFY_ONCOUNT) && !modeTraining) || fingers == 0)
                runRecognition(); 

            if (fingers == 0) 
            {
                //Fix: recognition subroutine is called every window, this recognition 
                if (modeExperiment)
                {
                    bool proceeding = experiment.NextStep(lastClassification,lastCandidate);
                    if (!proceeding)
                        fLog.addLine("[Warning] Experiment Was unable to proceed to next step.");

                    cleanPad();
                    
                }

                cleanUp();
                fLog.addLine("Clean up for total points " + countPoints);
            }

            //allows transition between multiple finger gestures without lifting up all fingers by removing traces, currently disabled
            //map.Remove(e.Id);
            
        }
        private void LogWM(WMTouchEventArgs e,string type)
        {
            if (!DISPLAY_TOUCH_DATA)
                return;

            string isPrimary = (e.IsPrimaryContact) ? "*" : "^"; 
            fLog.addLine("ID" + isPrimary + " {" + type + "}=" +  +e.Id + "[" + e.LocationX + "," 
                         + e.LocationY + "] Cx,y " + e.ContactX + "," + e.ContactY + "] t=" 
                         + e.Time + " ."); 
                         
        }

        private void UpdatePrevious(WMTouchEventArgs e)
        {
            if (prevPoint.ContainsKey(e.Id))
                prevPoint[e.Id] = e;
            else
                prevPoint.Add(e.Id, e);
        }

        private bool FilterPoint(WMTouchEventArgs e)
        {
            if ( (Math.Abs(e.LocationX - prevPoint[e.Id].LocationX) < FILTER_DELTA) && (Math.Abs(e.LocationY - prevPoint[e.Id].LocationY) < FILTER_DELTA))
            {
                //LogWM(e, "Filtered");
                return true;
            }            

            return false;
        }

        private void cleanUp(bool silent = false)
        {
            if (modeTemplate)
                return;
            if(!silent)
                fLog.addLine("Clean up for total points " + countPoints);
            pointMap.Clear();
            localPoints = 0;
            countPoints = 0;
            captureWindowCount = 0;
            gestureTries = 1;
            ++gestureCount;

            prevPoint = new Dictionary<int, WMTouchEventArgs>();

            if (!holdGesture)
                cleanPad();


            if (modeTest || modeTraining) {
                SetExpectedGesture(silent);
            } 
        }

        private void SetExpectedGesture(bool silent = false)
        {
            
            //expectedGesture = matrix.GetRandomElement().Name;
            
            if (Training_SAMPLES_COLLECTED < TRAINING_SAMPLES_TO_COLLECT)
            {
                Training_SAMPLES_COLLECTED++;
            }
            else
            {

                //expectedGesture = TrainingSet[gestureCount % TrainingSet.Count].Name; //for normal Gestures
                expectedGesture = Templates[trainingPointer % Templates.Count].Name;
                Training_SAMPLES_COLLECTED = 0;
                trainingPointer++;
            }

            if(!silent)
                fLog.addLine("Draw " + expectedGesture + " gesture. Sample # " +  (Training_SAMPLES_COLLECTED) + " Collected " + gestureCount);
        }

        #endregion




        #region Recognition    
        private void runRecognition(bool silent = false,M.Gesture autoCandidate = null)
        {

            if (!dollarMatchOn)
                return; 
            
            localPoints = 0;
            

            if (modeTemplate || !recognitionOn)
                return;

            if (!silent)
            {
                fLog.addLine("**************************************************************");
                fLog.addLine("Start Recognition");
            }
            captureWindowCount++;

     
            //------Write candidate to .XML ----------

            //autoMatic candidates  should not be written to file again
            //skip if experiment is happening
            if (autoCandidate == null && !modeExperiment)
            {
                String gestureName;

                
                if (modeTraining || modeTest)
                    gestureName = expectedGesture + "_candidate" + gestureCount + "_" + captureWindowCount;
                else
                    gestureName = "candidate" + gestureCount + "_" + captureWindowCount;

                String fileName = String.Format("{0}{1}{2}-{3}.xml", Application.StartupPath,
                    modeTraining ? examplesSubDirectory : candidateSubDirectory, gestureName, DateTime.Now.ToFileTime());

                M.Point[] tPts = pointMap.concatPoints();

                if (modeTest || modeTraining)
                    GestureIO.WriteGesture(tPts, gestureName, fileName, expectedGesture);
                else
                    GestureIO.WriteGesture(tPts, gestureName, fileName);

                //dont classify while training, training loops here just to record candiates above.
                if (modeTraining)
                    return; 
            }



            ////-------Begin Recognition-----------------//

            //classification message
            String classification = "";
            M.Gesture inputGesture;

            if (autoCandidate == null)
                inputGesture = new M.Gesture(pointMap, "Candidate", M.Gesture.eGestureType.Candidate);
            else
                inputGesture = autoCandidate;


            if (activeRecognizer == Recognizers.RawFeatures)
            {
                classification += recognizer.gestureToString(inputGesture);
            }
            else
            {
                classification += recognizer.Classify(inputGesture);
            }

            
            ////Confusion Matrix code
           if (modeTest)
           {
                if (classification.Equals("No Classification"))
                    numNoClass++; 
               matrix.RecordInstance(expectedGesture, classification);
               windowPerformance.RecordPerformance(captureWindowCount, expectedGesture, classification);
               testInstanceCount++;
           }
            //    fLog.addLine(matrix.ToString(), false);
            //    fLog.addLine("True Postives of " + expectedGesture + " is " + matrix.GetTruePositives(expectedGesture));
            //    fLog.addLine("False Positives of " + expectedGesture + " is " + matrix.GetFalsePositives(expectedGesture));
            //    fLog.addLine("False Negatives of " + expectedGesture + " is " + matrix.GetFalseNegatives(expectedGesture));
            //    fLog.addLine("True Negatives of " + expectedGesture + " is " + matrix.GetTrueNegatives(expectedGesture));



            //int tpts = pts.Length;
            //writeDataLine(tpts, msg); 

            if(dropWindows)
                pointMap.Clear();

            if (!silent)
            {
                fLog.addLine(activeRecognizer.ToString() +  " Recognition MSG = " + classification);
                fLog.addLine("**************************************************************");
            }

            lastClassification = classification;

            if (modeExperiment)
                experiment.LogWindow(captureWindowCount, classification);

        }

#endregion 
        #region "Menu and Status Bar"
        private void saveGesturesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //delete all 
            deleteGestures(); 
        }   
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void toolStripSplitModeButton_ButtonClick(object sender, EventArgs e)
        {
            if (modeTemplate)
            {
                cleanPad();
                this.toolStripSplitButtonHold.Enabled = true; 
                fLog.addLine("Entering Recognition Mode");
                
                //MessageBox.Show("The system is now entering Recognition Mode [Candidate]"); 
                this.toolStripSplitModeButton.Text = "Mode: Candidate";
                this.toolStripSplitAddButton.Enabled = false; 
                modeTemplate = false;         
               
            }
            else
            {
                cleanPad();
                this.toolStripSplitButtonHold.Enabled = false; 
                fLog.addLine("Entering Template Mode");
                //MessageBox.Show("The system is now  entering Training Mode [Template]"); 
                this.toolStripSplitModeButton.Text = "Mode: Template";
                this.toolStripSplitAddButton.Enabled = true; 
                modeTemplate = true; 
            }

        }

        private void removeAllCustomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeCustom(); 
        }

        //called when "load gesture"
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadGestures();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveGesture();
        }
        private void toolStripSplitAddButton_ButtonClick(object sender, EventArgs e)
        {
            saveGesture();
        }      // Collection of active strokes, currently being drawn by the user
        
        private void cleanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cleanPad(); 
        }

        private void toolStripSplitButtonHold_ButtonClick(object sender, EventArgs e)
        {
            if (holdGesture)
            {
                holdGesture = false; 
                this.toolStripSplitButtonHold.Text = "Don't Hold Gesture";


            }
            else
            {
                holdGesture = true; 
                this.toolStripSplitButtonHold.Text = "Hold Gesture";
            }
        }

        private void toolStripSplitButtonClean_ButtonClick(object sender, EventArgs e)
        {
            cleanPad(); 
        }

        private void toolStripSplitButtonRec_ButtonClick(object sender, EventArgs e)
        {
            if (recognitionOn)
            {
                recognitionOn = false;
                this.toolStripSplitButtonRec.Text = "Recognition = OFF";
            }
            else
            {
                recognitionOn = true;
                this.toolStripSplitButtonRec.Text = "Reconigtion = ON ";
            }

        }
        private void toolStripSplitByPoly_ButtonClick(object sender, EventArgs e)
        {
                       
        }

        private void setRecognizer(Recognizers r)
        {
            Recognizers previous = activeRecognizer;

            switch (r)
            {
                case Recognizers.CircGR:
                    recognizer = new M.CircGR();
                    recognizer.SetTemplates(Templates);
                    activeRecognizer = Recognizers.CircGR;
                    break;
                case Recognizers.RawFeatures:
                    activeRecognizer = Recognizers.RawFeatures;
                    break;
                case Recognizers.OneDollar:
                    recognizer = new M.OneDollar.OneDollarRecognizer();
                    recognizer.SetTemplates(Templates);
                    activeRecognizer = Recognizers.OneDollar;
                    break;
                case Recognizers.PDollar:
                    recognizer = new M.PDollar.PDollarRecognizer();
                    recognizer.SetTemplates(Templates);
                    activeRecognizer = Recognizers.PDollar;
                    break;
                default:
                    fLog.addLine("Unsupported Classifier Selected");
                    return;
            }

            fLog.addLine("Classifer set: " + activeRecognizer.ToString() +
                        (activeRecognizer == Recognizers.RawFeatures ? (" for " + previous.ToString()) : ""));
        }

        private void setRecognizer(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolStripMenuItem elementClicked = (System.Windows.Forms.ToolStripMenuItem) sender;
            string c = elementClicked.Text;

            switch (c)
            {
                case "CircGR":
                    setRecognizer(Recognizers.CircGR);
                    break;
                case "Raw Features":
                    setRecognizer(Recognizers.RawFeatures);
                    break;
                case "$1":
                    setRecognizer(Recognizers.OneDollar);
                    break;
                case "$P":
                    setRecognizer(Recognizers.PDollar);
                    break;
                default:
                    fLog.addLine("Unsupported Classifier Selected");
                    return;
            }            

        }
        
        

        #endregion

        #region Testing
        
        private void toolStripRunCannedGesture_ButtonClick(object sender, EventArgs e) 
        {
            if (modeTemplate)
                return;
                        
            RunCannedGestures();
       
        } //Run canned candidate gesture files 

        private void RunCannedGestures()
        {
            if(activeRecognizer == Recognizers.RawFeatures)
            {
                fLog.addLine(activeRecognizer.ToString() + " does not support Canned Gestures.");
            }
            else
            {
                RunCannedGestureSet("", false);
            }

        }

        private void toolStripRunShrinkingWindow_ButtonClick(object sender, EventArgs e) {
            if (modeTemplate)
                return;
            switch (activeRecognizer)
            {
                case Recognizers.RawFeatures:
                    fLog.addLine(activeRecognizer.ToString() + " does not support Shrinking Window Gestures.");
                    return;
                default:
                    RunShrinkingWindow(64, 16, 2);
                    break;
            }
        }



        private void ClassifyCandidateSet(Dictionary<String, List<M.Gesture>> CannedSet)
        {
            fLog.addLine("====================================================");
            fLog.addLine("Running Canned Candidate Gestures");
            
            writer.WriteLine("===Running Canned Gesture Set===");
           
            foreach (string candidateType in CannedSet.Keys)
            {
                List<M.Gesture> CandidateList = CannedSet[candidateType];
                expectedGesture = candidateType;
                fLog.addLine("Running set for: " + candidateType);

                foreach (M.Gesture cand in CandidateList)
                {
                    
                    runRecognition(true,cand);
                    
                }
                fLog.addLine("Ran " + CandidateList.Count + " canned instances of " + candidateType);
                writer.WriteLine("Ran " + CandidateList.Count + " canned instances of " + candidateType);
            }

            fLog.addLine("Finished Canned Candidate Gestures");
        }

        /// <summary>
        /// A version of ClassifyCandidateSet that does no output
        /// </summary>
        /// <param name="CannedSet"></param>
        private void LeanClassifyCandidateSet(Dictionary<string, List<M.Gesture>> CannedSet) {
            foreach (string candidateType in CannedSet.Keys)
            {
                expectedGesture = candidateType;

                foreach (M.Gesture cand in CannedSet[candidateType])
                {
                    runRecognition(true, cand);
                }

            }
        }


        private void RunCannedGestureSet(string directory = "", bool evaluateWindow = false)
        {
            string path;

            if (directory.Equals(""))
            {
                path = cannedCandidatesSubDirectory;
            }
            else
            {
                path = directory;
            }

           if (!evaluateWindow)
            {
                Dictionary<String, List<M.Gesture>> candidates = LoadExamples(Templates, path, M.Gesture.eGestureType.Candidate);
                ClassifyCandidateSet(candidates);
            }
            else
            {
                RunShrinkingWindow(CANDIDATE_CLASSIFY_ONCOUNT, CANDIDATE_CLASSIFY_ONCOUNT, 1);
            }
            cleanUp();
        }


        private void RunShrinkingWindow(int startingWindowSize, int endingWindowSize, int step, string directory ="")
        {
            int targetWindowSize, currentWindowSize;
            int delta = Math.Abs(step);

            if (startingWindowSize > endingWindowSize)
            {
                targetWindowSize = startingWindowSize;
                currentWindowSize = endingWindowSize;
            }
            else
            {
                targetWindowSize = endingWindowSize;
                currentWindowSize = startingWindowSize;
            }


            string path;

            if (directory.Equals(""))
            {
                path = cannedCandidatesSubDirectory;
            }
            else
            {
                path = directory;
            }

            fLog.addLine("Loading Canned Candidate Set at: " + path);
            writer.WriteLine("Canned Candidate Set loaded from: " + path);

            Dictionary<String, List<M.Gesture>> ExamplePool = LoadExamples(Templates, path, M.Gesture.eGestureType.Example);

            if (modeTest)
                endTestToolStripMenuItem_Click(new Object(), new EventArgs());
            if (modeTraining)
                endTrainingToolStripMenuItem_Click(new object(), new EventArgs());


            while (currentWindowSize <= targetWindowSize)
            {
                startTestToolStripMenuItem_Click(new object(), new EventArgs());

                fLog.addLine("Running Canned Set on WindowSize =" + currentWindowSize);

                foreach (string template in ExamplePool.Keys)
                {
                    List<M.Gesture> examples = ExamplePool[template];
                    
                    foreach (M.Gesture e in examples)
                    {
                       
                        expectedGesture = template;
                        int maxPoints = e.rawTraces.MaxNumberOfPoints;
                                                                       

                        for (int i = 0; i < maxPoints;)
                        {
                            i += currentWindowSize;                           

                            pointMap = e.rawTraces.getSubset(i);

                            M.Gesture g = new M.Gesture(pointMap, "Candidate", M.Gesture.eGestureType.Example,e.ExpectedAs);
                            runRecognition(true, g);
                        }
                        cleanUp(true);
                    }
                }

                endTestToolStripMenuItem_Click(new Object(), new EventArgs());
                fLog.addLine("End of WindowSize =" + currentWindowSize);

                currentWindowSize += delta;
            }            
        }

        private void startTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
            if (modeTest == true)
                return;
            if (modeTraining == true)
            {
                fLog.addLine("Please exit Training Mode before initiating testing.");
                return;
            }

            if (modeTemplate)  
                toolStripSplitModeButton_ButtonClick(sender, e);

            fLog.addLine("Testing Session Started.\nDraw the Expected Gesture.");
            writer.WriteLine("Testing Session Started:");

            modeTest = true;
            testInstanceCount = 0;
            numNoClass = 0;

            windowPerformance = new GestureWindowPerformance(Templates);
            matrix = new ConfusionMatrix(Templates);
            expectedGesture = matrix.GetRandomElement().Name;
            fLog.addLine("Draw a " + expectedGesture + " gesture.");
        }
        
        private void endTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (modeTest == false)
                return;

            endTest();

        }
        private void endTest()
        {
            double[] AvgMetricValues;
            
            endTest(out AvgMetricValues);

        }
        
        /// <summary>
        /// End the test and display results.Out values allows one to save results from test.
        /// 
        /// </summary>
        /// <param name="AvgMetricValues">Current float[] size 3 where [0] = MCC, [1]  = F1 Score, [2] = ACC</param>
        private void endTest(out double[] AvgMetricValues)
        {
            fLog.addLine("Testing Session Ended.");

            fLog.addLine("=================================");
            fLog.addLine("Writing Test Results....");
            fLog.addLine("Outputing Confusion Matrix....");

            writer.WriteLine("Generated Confusion Matrix:\n");
            writer.WriteLine(matrix.ToString());

            fLog.addLine("Outputing Raw Stats....");


            int TP, FP, TN, FN;
            double TPR, TNR, PPV, NPV, FPR, FDR, FNR, ACC, F1, MCC;

            //used to calculate average MCC, F1, Accuracy
            //ConfusionMatrix has these functions as convinience methods but, if used here, they would recalculate values unecessarily 
            double totalMCC = 0;
            double totalF1 = 0;
            double totalACC = 0;


            StreamWriter resultsWriter = new StreamWriter(activeRecognizer.ToString() + "_results.csv");
            resultsWriter.Write("recognizer");
            foreach( M.Gesture gesture in Templates)
            {
                resultsWriter.Write(", " + gesture.Name + "-ACC, " + gesture.Name +"-MCC");
            }

            resultsWriter.Write(", ACC, MCC\n" + activeRecognizer.ToString());

            foreach (M.Gesture gesture in Templates)
            {
                TP = matrix.GetTruePositives(gesture);
                FP = matrix.GetFalsePositives(gesture);
                FN = matrix.GetFalseNegatives(gesture);
                TN = matrix.GetTrueNegatives(gesture);

                TPR = matrix.GetTruePositiveRate(gesture);
                TNR = matrix.GetTrueNegativeRate(gesture);
                PPV = matrix.GetPositivePredictiveValue(gesture);
                NPV = matrix.GetNegativePredictiveValue(gesture);
                FPR = matrix.GetFalsePositiveRate(gesture);
                FDR = matrix.GetFalseDiscoveryRate(gesture);
                FNR = matrix.GetFalseNegativeRate(gesture);
                ACC = matrix.GetAccuracy(gesture);
                F1 = matrix.GetF1Score(gesture);
                MCC = matrix.GetMCC(gesture);

                totalMCC += MCC;
                totalF1 += F1;
                totalACC += ACC;

                //output to CSV
                resultsWriter.Write(", " + ACC + ", " + MCC);

                //output to file
                writer.WriteLine("============================= " + gesture.Name + " =============================");

                writer.WriteLine("True Positives: " + TP);
                writer.WriteLine("False Positives: " + FP);
                writer.WriteLine("False Negatives: " + FN);
                writer.WriteLine("True Negatives: " + TN);

                writer.WriteLine("True Positive Rate (Sensitivity) = " + TPR);
                writer.WriteLine("True Negative Rate (Specificity) = " + TNR);
                writer.WriteLine("Positive Predictive Value (Precision) = " + PPV);
                writer.WriteLine("Negative Predictive Value = " + NPV);
                writer.WriteLine("False Positive Rate (Fall-out) = " + FPR);
                writer.WriteLine("False Discovery Rate = " + FDR);
                writer.WriteLine("False Negative Rate (Miss Rate) = " + FNR);

                writer.WriteLine("Accuracy= " + ACC);
                writer.WriteLine("F1 Score = " + F1);

                writer.WriteLine("Mathew's correlation coefficient = " + MCC);


                //oputput to log
                fLog.addLine("============================= " + gesture.Name + " =============================");

                fLog.addLine("True Positives: " + TP);
                fLog.addLine("False Positives: " + FP);
                fLog.addLine("False Negatives: " + FN);
                fLog.addLine("True Negatives: " + TN);

                fLog.addLine("True Positive Rate (Sensitivity) = " + TPR);
                fLog.addLine("True Negative Rate (Specificity) = " + TNR);
                fLog.addLine("Positive Predictive Value (Precision) = " + PPV);
                fLog.addLine("Negative Predictive Value = " + NPV);
                fLog.addLine("False Positive Rate (Fall-out) = " + FPR);
                fLog.addLine("False Discovery Rate = " + FDR);
                fLog.addLine("False Negative Rate (Miss Rate) = " + FNR);

                fLog.addLine("Accuracy= " + ACC);
                fLog.addLine("F1 Score = " + F1);

                fLog.addLine("Mathew's correlation coefficient = " + MCC);

                for (int i = 1; i < 50; i++)
                {
                    
                    double acc = windowPerformance.getPerformance(gesture.Name, i);
                    if (acc == -1.0)
                        break;
                    
                    fLog.addLine(i + "-Window Performance: " + acc);
                }


            }

            double avgMCC = (totalMCC / (float)Templates.Count);
            double avgF1 = (totalF1 / (float)Templates.Count);
            double avgACC = (totalACC / (float)Templates.Count);

            resultsWriter.Write(", " + avgACC + ", " + avgMCC);
            resultsWriter.Close();


            writer.WriteLine("=================================");
            writer.WriteLine("Average Mathew's Correlation coefficient = " + avgMCC);
            writer.WriteLine("Average F1 Score = " + avgF1);
            writer.WriteLine("Average Accuracy = " + avgACC);

            fLog.addLine("=================================");
            fLog.addLine("Average Mathew's Correlation coefficient = " + avgMCC);
            fLog.addLine("Average F1 Score = " + avgF1);
            fLog.addLine("Average Accuracy = " + avgACC);

            AvgMetricValues = new double[3];
            AvgMetricValues[0] = avgMCC;
            AvgMetricValues[1] = avgF1;
            AvgMetricValues[2] = avgACC;

            fLog.addLine("Writing Test Output....Finished.");
            fLog.addLine("Testing Session Ended with " + testInstanceCount + " test Intances and " + numNoClass + " 'NoClassifications'.");


            writer.WriteLine("Testing Session Ended with " + testInstanceCount + " test Intances and " + numNoClass + " 'NoClassifications'.");
            modeTest = false;
            testInstanceCount = 0;
            this.matrix = null;
            this.windowPerformance = null;
            //xlWriter.CloseWriter(); can hang program if called when excel has manually been closed


        }


        //need to tweak and modify this to make it relevant
        private void kFoldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Recognizers> recognizers = new List<Recognizers>();
            recognizers.Add(Recognizers.CircGR);
            recognizers.Add(Recognizers.PDollar);
            benchmarkTime(recognizers);


            fLog.addLine("k-Fold is down during pruning");
            return;

            string kFolds = "1";
            DialogResult dr = ShowInputDialog("Enter a k", ref kFolds);
            if (dr != DialogResult.OK)
                return;

            int k = Int32.Parse(kFolds);
            fLog.addLine("Your input: " + k);


            if (k < 1)
            {
                fLog.addLine("In valid k. K must be a positive integer.");
                return;
            }



            //Dictionary<string, List<M.Gesture>> examples  = new Dictionary<string, List<M.Gesture>>();//= LoadExamples(TrainingSet, "",M.Gesture.eGestureType.Candidate);

            Dictionary<string, List<M.Gesture>> examples = LoadExamples(Templates);

            int numExamples = 0;
            int minExampleCount = Int32.MaxValue;    
            foreach (string template in examples.Keys)
            {
                List<M.Gesture> exampleList = examples[template];
                int exCount = exampleList.Count;
                numExamples += exCount;

                if (minExampleCount > exCount)
                    minExampleCount = exCount;

            }

            if (k > minExampleCount)
            {
                fLog.addLine("k = " + k + " is to big for the current number of examples : " + minExampleCount);
                return;
            }
               

            int foldSize = minExampleCount / k;

            writer.WriteLine("=========== K-Folds Validation Test Session Started =================");
            writer.WriteLine("k = " + k + "\tMinimum example Count = " + minExampleCount + "\tFold Size = " + foldSize);

            //the fold size means examples have to be partitioned into those

            //map of k versus the partitions themselves.
            Dictionary<String, List<M.Gesture>> kTestPartitions;// = new Dictionary<string, List<M.Gesture>>();
            Dictionary<String, List<M.Gesture>> kTrainPartitions;// = new Dictionary<string, List<M.Gesture>>();



            //start the each k-fold at this index
            int startIndex = 0;

            //holds the average values of MCC,F1, and ACC for each test in k test
            double[] AvgMetricValues;
            //overall average values for all k test
            double AvgMCC = 0, AvgF1 = 0, AvgACC = 0;

            for (int i = 1; i <= k; i++)
            {
                kTestPartitions = new Dictionary<string, List<M.Gesture>>();
                kTrainPartitions = new Dictionary<string, List<M.Gesture>>();
                
                foreach (string template in examples.Keys)
                {
                    List<M.Gesture> exampleList = examples[template];
                    List<M.Gesture> testCases = new List<M.Gesture>();
                    List<M.Gesture> trainCases = new List<M.Gesture>();


                    for (int j = 0; j < exampleList.Count; j++)
                    {
                        //to be used as trainining 
                        if(j >= startIndex  && j < startIndex + foldSize){
                            testCases.Add(exampleList[j]);                            
                        } else{

                            trainCases.Add(exampleList[j]);
                        }

                    }

                    kTestPartitions.Add(template,testCases);
                    kTrainPartitions.Add(template,trainCases);

                }

                fLog.addLine("Running " + i +" fold out of  " + k + "-Fold test............");
                writer.WriteLine("Testing Fold # " + i + " out of " + k);
                AvgMetricValues = runKTest(kTestPartitions, kTrainPartitions);

                //update average values
                AvgMCC += AvgMetricValues[0];
                AvgF1 += AvgMetricValues[1];
                AvgACC += AvgMetricValues[2];

                startIndex += foldSize;
            }

            //final compute of average
            AvgMCC /= k;
            AvgF1 /= k;
            AvgACC /= k;


            fLog.addLine("======== k-Fold Validation Test Session Ended ===========");
            writer.WriteLine("======== k-Fold Validation Test Session Ended ===========");

            fLog.addLine("Average MCC: " + AvgMCC );
            fLog.addLine("Average F1: " + AvgF1);
            fLog.addLine("Average ACC: " + AvgACC);

            writer.WriteLine("Average MCC: " + AvgMCC);
            writer.WriteLine("Average F1: " + AvgF1);
            writer.WriteLine("Average ACC: " + AvgACC);


        }
        


        private double[] runKTest(Dictionary<String, List<M.Gesture>> TestInstances, Dictionary<String, List<M.Gesture>> TrainInstances)
        {
            fLog.addLine("Training on Training Instances.");

            startTestToolStripMenuItem_Click(new Object(), new EventArgs());

            ClassifyCandidateSet(TestInstances);

            //holds the average MCC, F1, ACC for 1 run of a test
            double[] AvgMetricValues;

            endTest(out AvgMetricValues);

            return AvgMetricValues;
        }



        private void startTrainingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (modeTraining)
                return;

            if (modeTest == true)
            {
                fLog.addLine("Please exit Testing Mode before initiating training.");
                return;
            }

            if (modeTemplate)
                toolStripSplitModeButton_ButtonClick(sender, e);

            fLog.addLine("Training Started........");
            modeTraining = true;

            MessageBox.Show("The system is now entering Training Mode.\nDraw the Expected Gesture.");

            //this.matrix = new ConfusionMatrix(TrainingSet); //initialize matrix
            this.matrix = new ConfusionMatrix(Templates);
            //expectedGesture = matrix.GetRandomElement().Name; //start random

            expectedGesture = Templates[0].Name; //start first
            trainingPointer++; //increment to prevent repeating the first twice

            fLog.addLine("Draw a " + expectedGesture + " gesture.");


        }

        private void endTrainingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!modeTraining)
                return;

            modeTraining = false;
            
            fLog.addLine("Training Ended........");
        }


        private void benchmarkTime(List<Recognizers> recognizers) {

            string fileName = recognizers[0].ToString();

            for (int i = 1; i < recognizers.Count; i++) {

                fileName += "_vs_" + recognizers[i].ToString();
            }

            fileName += "_TimeBenchmark";
            StreamWriter resultsWriter = new StreamWriter(fileName + ".csv");
            StreamWriter perGestureWriter = new StreamWriter(fileName + "_perGesture.csv");
            //Write labels
            resultsWriter.WriteLine("recognizer, numOfTemplates, numberOfExamples, millisecondsElapsed");
            perGestureWriter.WriteLine("recognizer,gestureName, numFingers, numPoints, millisecondsElapsed");

            Dictionary<String, List<M.Gesture>> examples = LoadExamples(Templates,cannedCandidatesSubDirectory);
            Stopwatch global_sw = new Stopwatch();
            Stopwatch gesture_sw = new Stopwatch();            

            foreach(Recognizers r in recognizers){
                setRecognizer(r);
                global_sw.Start();

                foreach (string template in examples.Keys)
                {
                    
                    foreach (M.Gesture cand in examples[template])
                    {
                        gesture_sw.Start();
                        recognizer.Classify(cand);
                        gesture_sw.Stop();
                        perGestureWriter.WriteLine("{0}, {1}, {2}, {3}, {4}",r.ToString(), template, cand.NumOfTraces,cand.NumOfPoints,gesture_sw.ElapsedMilliseconds);
                        gesture_sw.Reset();
                    }

                }

                global_sw.Stop();

                resultsWriter.WriteLine("{0}, {1}, {2}, {3}", r.ToString(),Templates.Count,examples.Values.Count, global_sw.ElapsedMilliseconds);
                global_sw.Reset();                
            }            
            resultsWriter.Close();
            perGestureWriter.Close();
        }
        
        
        
        #endregion

        #region Experiment
        private void BasicModeClick(object sender, EventArgs e)
        {
            if (experiment != null)
                fLog.addLine("Experiment Currently Active. Start the Experiment.");
            else
            {
                fLog.addLine("Experiment - Basic - Created. Press \"Start\" to start the Experiment.");
                experiment = Experiment.CreateExperiment(Templates,this);

            }
        }

        private void UserAdjustedModeClick(object sender, EventArgs e)
        {
            if (experiment != null)
                fLog.addLine("Experiment Currently Active. Start the Experiment.");
            else
            {
                fLog.addLine("Experiment - User Adjusted - Created. Press \"Start\" to start the Experiment.");
                experiment = Experiment.CreateExperiment(Templates, this, Experiment.ExperimentalMode.UserTemplates);
            }

        }

        
        private void RandomExpModeClick(object sender, EventArgs e)
        {
            if (experiment != null)
                fLog.addLine("Experiment Currently Active. Start the Experiment.");
            else
            {
                fLog.addLine("Experiment - Randomly Selected - Created. Press \"Start\" to start the Experiment.");
                experiment = Experiment.CreateExperiment(Templates, this, Experiment.ExperimentalMode.Random);
            }

        }

        private void StartExperiment(object sender, EventArgs e)
        {
            if (modeExperiment || experiment == null)
                return;

            setRecognizer(Recognizers.CircGR);
            if(modeTemplate)
                toolStripSplitModeButton_ButtonClick(sender, e);

            modeExperiment = true;
            lastCandidate = new M.Gesture(pointMap, "Candidate", M.Gesture.eGestureType.Candidate);
            Invalidate();
        }


        private void EndExeriment(object sender, EventArgs e)
        {
            if (!modeExperiment && experiment ==null)
                return;

            modeExperiment = false;
            fLog.addLine("Experiment Ended Prematurely");
            experiment = null;
        }




        #endregion


        #region "Gesture IO"
        private void cleanPad()
        {
            fLog.addLine("===========================================================");
            this.touchColor = new TouchColor();
            this.FinishedStrokes = new CollectionOfStrokes();
            this.ActiveStrokes = new CollectionOfStrokes();
            this.pointMap.Clear();
            this.Refresh();
            this.localPoints = 0;
            this.countPoints = 0;
            

        }
        private void saveGesture()
        {
            if (!modeTemplate)
                return;
            
            string gestureName = "Custom"; 
            DialogResult dr = ShowInputDialog("Gesture Name?", ref gestureName);
            if (dr != DialogResult.OK)
                return;
            
            if (!Directory.Exists(Application.StartupPath + gestureSubDirectory))
                Directory.CreateDirectory(Application.StartupPath + gestureSubDirectory);

            if (!Directory.Exists(Application.StartupPath + outputSubDirectory))
                Directory.CreateDirectory(Application.StartupPath + outputSubDirectory);
         
            String fileName = String.Format("{0}{1}{2}-{3}.xml", Application.StartupPath,
                            gestureSubDirectory, gestureName, DateTime.Now.ToFileTime());
            String fileNameBinary = String.Format("{0}{1}{2}-{3}", Application.StartupPath,
                            outputSubDirectory , gestureName, DateTime.Now.ToFileTime());

            M.Point[] tPts = pointMap.concatPoints();
            GestureIO.WriteGesture(tPts, gestureName, fileName);
            M.Gesture g = new M.Gesture(pointMap, gestureName, M.Gesture.eGestureType.Template);
            fLog.addLine("Gesture " + gestureName + " was saved on file" + fileName);
            cleanPad();             
        }

        private void deleteGestures()
        {
            fLog.addLine("Deleting Gestures"); 
            if (Directory.Exists(Application.StartupPath + gestureSubDirectory))
            {
                string[] files = Directory.GetFiles(Application.StartupPath +
									gestureSubDirectory);
                foreach (string file in files)
                    File.Delete(file);
            }

            // reload the training set
            loadGestures(); 
        }
        private void loadGestures()
        {
            cleanPad();
            //this.CircTemplates = LoadCircTemplates();
            Templates = LoadTemplates();
            recognizer.SetTemplates(Templates);
            
        }
        private void loadCustomGestures()
        {
            //currently not utilized (YAGNI)
            //this method is supposed to load templates another directory ontop of the default directory
            //loadGestures();
            //this.TrainingSet.AddRange(LoadTrainingSet("NewGestures")); 
        }
        private void removeCustom()
        {
            cleanPad(); 
            loadGestures();
        }

        private List<M.Gesture> LoadTemplates(String Dir = "") {
            string path;
            if (Dir != "")
                path = Dir;
            else
            {
                path = Application.StartupPath + pathSeparator + "GestureSet" + pathSeparator + "TemplateGestures";
            }

            fLog.addLine("Loading Template Set: " + path);

            List<M.Gesture> gestures = new List<M.Gesture>();

            string[] gestureFiles = Directory.GetFiles(path, "*.xml");

            foreach (string file in gestureFiles)
            {
                fLog.addLine("Loading : " + file);
                gestures.Add(GestureIO.ReadGesture(file, M.Gesture.eGestureType.Template));                 
                writer.WriteLine("# Loading gesture : " + file);                
            }
            return gestures;
        }


         public void setTemplates(string dir)
        {
            if (!modeExperiment)
            {
                fLog.addLine("Warning:Setting the templates outside of Touchpad should only be done during experiments.");
                return;
            }

            //CircTemplates = LoadCircTemplates(dir);
            Templates = LoadTemplates(dir);
        }




        private Dictionary<String, List<M.Gesture>> LoadExamples(List<M.Gesture> templateGestures, string subDir = "", M.Gesture.eGestureType gType = M.Gesture.eGestureType.Example)
        {
            string path = examplesSubDirectory;
            if (subDir != "")
                path = subDir;

            fLog.addLine("Loading Examples for each Template from:" + path);

            Dictionary<String, List<M.Gesture>> Examples = new Dictionary<string, List<M.Gesture>>();

            foreach (M.Gesture template in templateGestures)
            {
                Examples.Add(template.Name, new List<M.Gesture>());
            }

            string[] exampleFiles = Directory.GetFiles(Application.StartupPath + path, "*.xml");

            foreach (string file in exampleFiles)
            {
                M.Gesture example = GestureIO.ReadGesture(file, M.Gesture.eGestureType.Example);
                String expected = example.ExpectedAs;

                if (Examples.ContainsKey(expected))
                    Examples[expected].Add(example);
                else
                    fLog.addLine("The key '" + expected + "' was not found in Examples dictionary.");

            }

            foreach (string template in Examples.Keys)
            {
                fLog.addLine("Loaded " + Examples[template].Count + " examples for " + template);
            }

            return Examples;
        }


        #endregion 
        #region "Input Dialog" 
        private static DialogResult ShowInputDialog(string message, ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = message;

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }
        #endregion 

        private void toolStripSplitButton1_ButtonClick_1(object sender, EventArgs e)
        {
            string input = ""; 
            ShowInputDialog("Enter expected gesture", ref input);
            expectedStr = input; 
        }

        private void TouchPad_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeWriter(); 
        }    


   }

    
   #region "Touch Color Class"
    // Color generator: assigns a color to the new stroke.
    public class TouchColor
    {
        // Constructor
        public TouchColor()
        {
        }

        // Returns color for the newly started stroke.
        // in:
        //      primary         boolean, whether the contact is the primary contact
        // returns:
        //      color of the stroke
        public Color GetColor(bool primary)
        {
            if (primary)
            {
                // The primary contact is drawn in black.
                return Color.Black;
            }
            else
            {
                // Take current secondary color.
                Color color = secondaryColors[idx];

                // Move to the next color in the array.
                idx = (idx + 1) % secondaryColors.Length;

                return color;
            }
        }

        // Attributes
        static private Color[] secondaryColors =    // Secondary colors
        {
            Color.Red,
            Color.LawnGreen,
            Color.Blue,
            Color.Cyan,
            Color.Magenta,
            Color.Yellow,
            Color.DarkGreen,
            Color.DarkSalmon,
            Color.HotPink
        };
        private int idx = 0;                // Rotating secondary color index
    }
#endregion 
}
