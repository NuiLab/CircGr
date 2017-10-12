using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using M = MTGRLibrary;

namespace MTScratchpadWMTouch
{
    class Experiment
    {
        private static Random rng = new Random();

        /// <summary>
        /// Defines the protocol of the experiment to run.
        /// 
        /// 
        /// </summary>
        public enum ExperimentalMode { DeveloperTemplates, UserTemplates , Random};
        private ExperimentalMode expMode;
        
        /// <summary>
        /// The flow of phases the experiment could be in:
        /// Started - the very intial phase of the experiment, a special "transition" than only happens once at the beginning
        /// Training - allows user to familizarize themselves with a gesture
        /// Testing - captures user gesture for recording and performance
        /// Transitioning - moving from one phase through another for the same gesture (e.g. training A to capturing A)
        /// Break - finished a tranining and capturing session for a gesture in the gesture set, encountered once every gesture
        /// Finished - Experiment complete
        /// </summary>
        public enum Phase { Started, Capturing, Training, Testing, Feedback, Transitioning, Break, Finished};
        private Phase currentPhase;
        private Phase previousPhase;
        private bool phaseSwitch = false;

        /// <summary>
        /// Confusion Matrix for classification at the end of the gesture.
        /// </summary>
        ConfusionMatrix mainMatrix;
        /// <summary>
        /// Confusion Matrix for classifications at each window captured during the execution of a gesture.
        /// </summary>
        Dictionary<int, ConfusionMatrix> windowMatrix;
        
               
        public bool PHASE_SWITCH
        {
            get
            {
                return phaseSwitch;
            }
        }
        
        public Phase CurrentPhase
        {
            get
            {
                return currentPhase;
            }
        }     



        /* Options */
        /// <summary>
        /// Number of Training Gestures to capture for each gestures before user inputs realtime gestures. 
        /// </summary>
        private int NUM_TRAINING_GESTURES = 3;
        /// <summary>
        /// Number of gestures to capture for a gesture class during a trial.
        /// </summary>
        private int NUM_TEST_GESTURES = 7;



        private long userID;
        public long USER_ID
        {
            get
            {
                return userID;
            }
        }

        public string BaseDir
        {
            get
            {
                return baseDir;
            }

        }

        public string TrainDir
        {
            get
            {
                return trainDir;
            }
        }

        public string TestDir
        {
            get
            {
                return testDir;
            }

        }

        private List<M.Gesture> Templates;
        private int activeTemplate = 0;
        private int acquiredTrainingGestures = 0;
        private int acquiredTestGestures = 0;
        private int score = 0;

        private string instruction = "Welcome to the CircGR Experiment.\n Tap Anywhere to start";

        private Rectangle targetRectangle;
        private bool targetRectangleDrawn = false;
        private Pen targetRectanglePen = new Pen(Color.CadetBlue,7);
        private Font instructionFont = new Font(FontFamily.GenericSerif, 18, FontStyle.Regular);

        private bool useUserTemplates = false;

        //Utilized for feedback phase
        private List<string> questions = new List<string> { "This gesture is physically comfortable to perform.",
                                                            "This would work well in mobile devices (e.g. iPads, smartphones, tablets).",
                                                            "This gesture is too complicated to use daily.",
                                                            "This gesture would work well in larger touch-screen PCs(e.g. laptops, desktops, iMacs)."};
        private readonly List<string> answers = new List<string> { "Strongly Disagree", "Disagree", "Somewhat Disagree", "Neutral", "Somewhat Agree", "Agree", "Strongly Agree" };

        private Dictionary<int, GroupBox> groupBoxes;
        private Dictionary<int, List<RadioButton>> radioButtons;
        private Dictionary<int, RadioButton> selectedButtons;
        private GroupBox QuestionContainer;
        private Button getSelection;
        private bool FeedbackGiven = false;

        //a reference to the form that is using this experiment, used to add buttons and other UI elements
        private TouchPad activeTouchPad;



        //writes errors in windows classficiations
        StreamWriter window_writer;
        StreamWriter script_writer;
        /// <summary>
        /// Writes feedback information
        /// </summary>
        StreamWriter feedback_writer;

        //window results should not be accepted for gestures that are user error, held in buffer until verification
        Dictionary<int, Tuple<double, string>> windowLogBuffer = new Dictionary<int, Tuple<double, string>>();

        private string baseDir;
        private string trainDir;
        private string testDir;
        private string errorDir;
        private string templateDir;
        private string noClassificationDir;
        private string misClassificationDir;

        private int numNoClassifications = 0;
        
 
        private DateTime UTC = new DateTime(1970, 1, 1);

        private Experiment(List<M.Gesture> Templates, TouchPad touchPad, ExperimentalMode mode)
        {
            expMode = mode;
            activeTouchPad = touchPad;
            this.Templates = new List<M.Gesture>(Templates);

            mainMatrix = new ConfusionMatrix(Templates);
            windowMatrix = new Dictionary<int, ConfusionMatrix>();
            Shuffle(Templates);

            //user is currently UTC time
            userID = (long)getTime();


            switch (mode)
            {
                case ExperimentalMode.UserTemplates:
                    useUserTemplates = true;
                    break;
                case ExperimentalMode.Random:
                    if (rng.NextDouble() >= 0.50)
                        useUserTemplates = true;
                    break;
                default:
                    useUserTemplates = false;
                    break;
            }

            
            //Create folders for experiment
            string pathSeparator = TouchPad.getPathSeparator();

            baseDir = Application.StartupPath + pathSeparator + "GestureSet" + pathSeparator + "Experiment" + pathSeparator + USER_ID + pathSeparator;
            trainDir = baseDir + "Training" + pathSeparator;
            testDir = baseDir + "TestGestures" + pathSeparator;
            errorDir = baseDir + "Errors" + pathSeparator;
            templateDir = baseDir + "Templates" + pathSeparator;
            noClassificationDir = baseDir + "NoClassification" + pathSeparator;
            misClassificationDir = baseDir + "MisClassification" + pathSeparator;

            Directory.CreateDirectory(baseDir);
            Directory.CreateDirectory(trainDir);
            Directory.CreateDirectory(testDir);
            Directory.CreateDirectory(errorDir);
            Directory.CreateDirectory(templateDir);
            Directory.CreateDirectory(noClassificationDir);
            Directory.CreateDirectory(misClassificationDir);

            //initialize Feedback UI, hide until used by Feedback Phase
            InitializeFeedbackForm();
            QuestionContainer.Hide();


            //initiate bookkeeping
            writeConfig();
            window_writer = new StreamWriter(baseDir + "WindowError.txt");
            window_writer.WriteLine("# Target Classification Window_Number");
            feedback_writer = new StreamWriter(baseDir + "Feedback.txt");
            script_writer = new StreamWriter(baseDir + "Script.txt");

            currentPhase = Phase.Started;
            Log();
            
        }

        public void Log(string misc ="", double timeStamp = -1)
        {
            if(timeStamp < 0)
                script_writer.WriteLine(getTime() + " " + currentPhase.ToString() + " " + Templates[activeTemplate].Name + " " + misc);
            else
                script_writer.WriteLine(timeStamp + " " + currentPhase.ToString() + " " + Templates[activeTemplate].Name + " " + misc);
        }


        public void writeConfig()
        {
            StreamWriter writer = new StreamWriter(baseDir + "Config.txt");
            writer.WriteLine("WindowSize= " + TouchPad.CANDIDATE_CLASSIFY_ONCOUNT);
            writer.WriteLine("SamplingResolution= " + M.Gesture.SAMPLING_RESOLUTION);
            writer.WriteLine("Mode= " + expMode.ToString());
            writer.WriteLine("NUM_TRAINING_GESTURES= " + NUM_TRAINING_GESTURES);
            writer.WriteLine("NUM_TEST_GESTURES= " + NUM_TEST_GESTURES);
            writer.WriteLine("USER_ID= " + userID);
            writer.WriteLine("USER_TEMPLATES= " + (useUserTemplates? "1":"0"));
            writer.Write("Templates_used= ");
            List<string> labels = mainMatrix.GetAllLabels();
            foreach(string s in labels)
            {
                writer.Write(s + " ");
            }
            writer.WriteLine();
            writer.Write("Template_order= ");
            foreach(M.Gesture template in Templates)
            {
                writer.Write(template.Name + " ");
            }
            writer.WriteLine();
            writer.WriteLine("FQA");
            foreach (string question in questions)
                writer.WriteLine(question);
            writer.WriteLine("FQA_END");

            writer.Close();

        }

        public static Experiment CreateExperiment(List<M.Gesture> Templates, TouchPad touchPad, ExperimentalMode mode = ExperimentalMode.DeveloperTemplates)
        {
            return new Experiment(Templates, touchPad, mode);
        }


        #region Feedback

        private void InitializeFeedbackForm()
        {
            int padding = 25;

            Point startPoint = new Point(20, 80);

            this.QuestionContainer = new GroupBox();
            this.QuestionContainer.Location = startPoint;
            this.QuestionContainer.Size = new Size(900, 700);
            this.QuestionContainer.Text = "Questions";


            groupBoxes = new Dictionary<int, GroupBox>();
            radioButtons = new Dictionary<int, List<RadioButton>>();
            selectedButtons = new Dictionary<int, RadioButton>();

            for (int i = 0; i < questions.Count; i++)
            {
                //create groupbox for this question
                GroupBox g = new GroupBox();
                //create radio button list for this question
                radioButtons.Add(i, new List<RadioButton>());
                //point of the top leftcorner of the container for groupbox
                Point gLocation = new Point(startPoint.X + padding, startPoint.Y + 150 * i);
                g.Location = gLocation;
                g.Size = new Size(800, 80);
                g.Text = questions[i];

                for (int j = 0; j < answers.Count; j++)
                {
                    RadioButton rb = new RadioButton();
                    rb.Location = new Point(100 * j + padding, padding);
                    rb.Size = new System.Drawing.Size(100, 30);
                    rb.Text = answers[j];
                    rb.Name = "" + i;
                    rb.CheckedChanged += new EventHandler(new_radio_CheckedChanged);

                    radioButtons[i].Add(rb);
                    g.Controls.Add(rb);
                }
                groupBoxes.Add(i, g);



                QuestionContainer.Controls.Add(g);
            }

            getSelection = new Button();

            this.getSelection.Size = new System.Drawing.Size(200, 25);
            this.getSelection.Location = new System.Drawing.Point(QuestionContainer.Width - getSelection.Width, QuestionContainer.Height - getSelection.Height);
            this.getSelection.Text = "Continue";
            this.getSelection.Click += new EventHandler(getSelection_Click);
            QuestionContainer.Controls.Add(getSelection);

            activeTouchPad.Controls.Add(QuestionContainer);
        }

        void new_radio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null)
            {
                MessageBox.Show("Sender is not a RadioButton");
                return;
            }

            // Ensure that the RadioButton.Checked property
            // changed to true.
            if (rb.Checked)
            {
                // Keep track of the selected RadioButton by saving a reference
                // to it.
                int qId = Int32.Parse(rb.Name);

                if (selectedButtons.ContainsKey(qId))
                {
                    selectedButtons[qId] = rb;
                }
                else
                {
                    selectedButtons.Add(qId, rb);
                }

            }
        }

        void getSelection_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < questions.Count; i++)
            {
                if (!selectedButtons.ContainsKey(i))
                {
                    MessageBox.Show("Please Answer all questions.");
                    Log("Error");
                    return;
                }
            }

            FeedbackGiven = true;
            Log("DONE");
            WriteFeedback();
            //MessageBox.Show(selectedButtons[0].Text + " " + selectedButtons[1].Text + " " + selectedButtons[2].Text + " " + selectedButtons[3].Text);
            
            QuestionContainer.Hide();
            ResetRadioButtons();
            instruction = "Thank you! Click to Continue";

            
         
        }

        private void WriteFeedback()
        {
            foreach(int question in selectedButtons.Keys)
            {
                RadioButton r = selectedButtons[question];

                int answer = -1;

                for (int i = 0; i < answers.Count; i++)
                {

                    if (r.Text.Equals(answers[i]))
                    {
                        answer = i;
                    }
                }

                feedback_writer.Write(answer + " ");
                               
            }


            feedback_writer.Write(Templates[activeTemplate].Name);
            feedback_writer.WriteLine();
        }

        void ResetRadioButtons()
        {
            foreach (int key in radioButtons.Keys)
            {
                List<RadioButton> rBs = radioButtons[key];
                foreach (RadioButton r in rBs)
                    r.Checked = false;


            }

            selectedButtons = new Dictionary<int, RadioButton>();
        }
        #endregion

        public double getTime()
        {
            return (DateTime.UtcNow - UTC).TotalMilliseconds;
        }

        #region Experiment Flow
        public bool NextStep(string classification, M.Gesture lastCandidate)
        {
            if (lastCandidate == null)
                return false;
            //string nextStep = "";
            phaseSwitch = false;

            switch (currentPhase)
            {
                case Phase.Started:
                    UpdateStarted(classification);
                    break;
                case Phase.Capturing:
                    UpdateCapturing(classification, lastCandidate);
                    break;
                case Phase.Training:
                    return UpdateTraining(classification, lastCandidate);                   
                case Phase.Testing:
                    return UpdateTesting(classification, lastCandidate);
                case Phase.Feedback:
                    UpdateFeedback();
                    break;                   
                case Phase.Break:
                    UpdateBreak();
                    break;
                case Phase.Transitioning:
                    UpdateTransitioning();
                    break;
                case Phase.Finished:
                default:
                    return false;
            }

            return true;          
        }

        private void UpdateFeedback()
        {
            if (FeedbackGiven)
            {
                previousPhase = Phase.Feedback;
                currentPhase = Phase.Transitioning;
            }

        }

        private bool UpdateCapturing(string classification, M.Gesture lastCandidate)
        {
            M.Gesture currentTemplate = Templates[activeTemplate];

            if (lastCandidate.rawTraces.NumOfTraces != currentTemplate.rawTraces.NumOfTraces)
            {
                instruction = "Try " + Templates[activeTemplate].Name + " Again.";
                WriteGesture(lastCandidate, errorDir, classification);
                Log("ERROR");
                return false;
            } else
            {
                DialogResult r = MessageBox.Show("Accept template?","Calibrations",MessageBoxButtons.YesNo);
                if (r == DialogResult.No)
                {
                    Log("REDO");
                    return false;
                } 
                    
            }



            WriteGesture(lastCandidate, templateDir, classification);
            activeTemplate++;
                  




            if (activeTemplate >= Templates.Count)
            {
                //decide to use users templates
                if (useUserTemplates)
                {
                    activeTouchPad.setTemplates(templateDir);
                }
                //reset active for test
                instruction = "Click to Continue.";
                activeTemplate = 0;
                Log("DONE");
                previousPhase = Phase.Capturing;
                currentPhase = Phase.Transitioning;
            }
            else
            {
                Log("CAPTURED");
                instruction = "Draw " + Templates[activeTemplate].Name;
            }

            return true;
        }

        private void UpdateStarted(string classification)
        {
            instruction = "Tap to Continue.";
            previousPhase = Phase.Started;
            currentPhase = Phase.Transitioning;
            phaseSwitch = true;
        }

        private void UpdateTransitioning()
        {
            
            switch (previousPhase)
            {
                case Phase.Started:
                    currentPhase = Phase.Capturing;
                    break;
                case Phase.Capturing:
                    currentPhase = Phase.Training;
                    break;
                case Phase.Training:
                    currentPhase = Phase.Testing;
                    break;
                case Phase.Testing:
                    currentPhase = Phase.Feedback;
                    break;
                case Phase.Feedback:
                    currentPhase = Phase.Break;
                    break;
                case Phase.Break:
                    currentPhase = Phase.Training;
                    break;
                case Phase.Finished:
                    currentPhase = Phase.Finished;
                    break;
                default:
                    instruction = "Invalid State";
                    break;

            }
            //log transition
            script_writer.WriteLine(getTime() + " " + Phase.Transitioning.ToString() + " " + Templates[activeTemplate].Name + " " + previousPhase.ToString() + " " + currentPhase.ToString());

            UpdateNext(currentPhase);

        }

        private bool UpdateTesting(string classification, M.Gesture lastCandidate)
        {
            
            M.Gesture currentTemplate = Templates[activeTemplate];

            if (currentTemplate.rawTraces.NumOfTraces != lastCandidate.rawTraces.NumOfTraces)
            {
                instruction = "Error in input. #of Fingers dont match";
                windowLogBuffer.Clear();
                Log("ERROR");
                WriteGesture(lastCandidate, errorDir,classification);
                return false;
            }
                       

            if (!classification.Equals(currentTemplate.Name))
            {
                if(classification.Equals("No Classification"))
                {
                    WriteGesture(lastCandidate, noClassificationDir, classification);
                    Log("NO_CLASS");
                    numNoClassifications++;
                    windowLogBuffer.Clear();
                }
                else
                {
                    WriteGesture(lastCandidate, misClassificationDir, classification);
                    mainMatrix.RecordInstance(Templates[activeTemplate].Name, classification);
                    acquiredTestGestures++;
                    ProcessWindowLogBuffer();
                    Log("MISCLASSIFIED");
                    
                }
                instruction = "Good Job! Draw Another " + currentTemplate.Name;
                //instruction = "Classification Did Not Match" + currentTemplate.Name + ".";                
                score += rng.Next(10, 50);
            }
            else
            {
                acquiredTestGestures++;
                instruction = "Good Job! " + acquiredTestGestures  + " " + classification + " Acquired. Draw Another " + currentTemplate.Name;
                score += rng.Next(10, 101);

                WriteGesture(lastCandidate, testDir, classification);
                mainMatrix.RecordInstance(Templates[activeTemplate].Name, classification);
                ProcessWindowLogBuffer();
                Log("ACQUIRED");
                

            }              

            targetRectangleDrawn = false;
            

            if (acquiredTestGestures >= NUM_TEST_GESTURES)
            {
                Log("DONE");
                previousPhase = Phase.Testing;
                currentPhase = Phase.Transitioning;
                phaseSwitch = true;
                instruction = "Testing Complete. Tap to continue.";
            }
            return true;
        }

        private bool UpdateTraining(string classification, M.Gesture lastCandidate)
        {
            M.Gesture currentTemplate = Templates[activeTemplate];
            ProcessWindowLogBuffer();
            if (lastCandidate.rawTraces.NumOfTraces != currentTemplate.rawTraces.NumOfTraces)
            {
                instruction = "Try " + Templates[activeTemplate].Name + " Again.";
                Log("ERROR");
                WriteGesture(lastCandidate, errorDir,classification);               
                return false;
            }
            else
            {
                acquiredTrainingGestures++;
                score += rng.Next(10, 101);
                instruction = "Good Job! Training # " + acquiredTrainingGestures;
                targetRectangleDrawn = false;
                WriteGesture(lastCandidate, trainDir,classification);
                Log("ACQUIRED");
            }

            if (acquiredTrainingGestures >= NUM_TRAINING_GESTURES)
            {
                Log("DONE");
                previousPhase = Phase.Training;
                currentPhase = Phase.Transitioning;
                phaseSwitch = true;
                instruction += "Training Complete. Tap to continue.";
            }

            return true;
        }

        private void UpdateBreak()
        {
            Log();
            activeTemplate++;
            previousPhase = Phase.Break;
            if (activeTemplate < Templates.Count)
            {
               
                currentPhase = Phase.Training;
                phaseSwitch = true;
                acquiredTrainingGestures = 0;
                acquiredTestGestures = 0;
                instruction = "Please Draw: " + Templates[activeTemplate].Name; 
            }
            else
            {
                currentPhase = Phase.Finished;
                instruction = "Experiment Complete.";
                UpdateFinished();
                phaseSwitch = true;                            
            }
            

        }

        private void UpdateFinished()
        {
            script_writer.WriteLine(getTime() + "  Finished");        
            StreamWriter r = new StreamWriter(baseDir + "ConfusionMatrix.txt");
            r.WriteLine(mainMatrix.ToString());
            r.Close();

            window_writer.Close();
            feedback_writer.Close();
            script_writer.Close();
            WriteMatrixResults(mainMatrix,"ConfusionMatrixResults");

            foreach (int windowNum in windowMatrix.Keys)
            {
                r = new StreamWriter(baseDir + "Window" + windowNum + ".txt");
                r.WriteLine(windowNum);
                r.WriteLine(windowMatrix[windowNum].ToString());
                r.Close();

                WriteMatrixResults(windowMatrix[windowNum], "Window" + windowNum + "_Results");
            }

        }

        private void UpdateNext(Phase nextPhase)
        {
            switch (nextPhase)
            {
                case Phase.Started:
                    instruction = "Welcome to the OpenHID GR Experiment";
                    break;
                case Phase.Capturing:
                    instruction = "Beginning System Adjustments. Draw " + Templates[activeTemplate].Name;
                    break;
                case Phase.Training:
                case Phase.Testing:
                    instruction = "Enter " + Templates[activeTemplate].Name + " in the bounded box.";
                    break;
                case Phase.Feedback:
                    QuestionContainer.Text = Templates[activeTemplate].Name;
                    QuestionContainer.Show();
                    FeedbackGiven = false;
                    break;
                case Phase.Break:
                    instruction = "Break";
                    break;
                case Phase.Finished:
                    instruction = "Experiment Finished!";
                    break;
                case Phase.Transitioning:
                default:
                    instruction = "Tap to continue.";
                    break;
            }
        

        }

        public void Cancel()
        {
            window_writer.WriteLine("Experiment Canceled");
            window_writer.Close();
            feedback_writer.WriteLine("Experiment Canceled");
            feedback_writer.Close();
            script_writer.WriteLine("Experiment Canceled");
            script_writer.Close();
        }

        public void LogWindow(int windowNumber, string classification)
        {
            if (classification.Equals("No Classification") || currentPhase != Phase.Testing)
                return;


            windowLogBuffer.Add(windowNumber, new Tuple<double, string>(getTime(), classification));

            
            

        }

        public void ProcessWindowLogBuffer()
        {
            foreach(int windowNum in windowLogBuffer.Keys)
            {
                double timeStamp = windowLogBuffer[windowNum].Item1;
                string classification = windowLogBuffer[windowNum].Item2;
                
                if (!windowMatrix.ContainsKey(windowNum))
                    windowMatrix.Add(windowNum, new ConfusionMatrix(mainMatrix.TESTING_SET));

                windowMatrix[windowNum].RecordInstance(Templates[activeTemplate].Name, classification);

                if (!classification.Equals(Templates[activeTemplate].Name))
                    window_writer.WriteLine(Templates[activeTemplate].Name + " " + classification + " " + windowNum);

                Log("WINDOW " + windowNum + " " + classification,timeStamp);
            }

            windowLogBuffer.Clear();
        }
        #endregion

        #region Experiment UI
        public void UpdateExperimentUI(Graphics g, Size windowRes)
        {
            if(activeTemplate < Templates.Count)
            g.DrawString(currentPhase.ToString() + " " + Templates[activeTemplate].Name, SystemFonts.DefaultFont, Brushes.Black, windowRes.Width * 0.80f, windowRes.Height * 0.05f);

            switch (currentPhase)
            {
                case Phase.Started:
                    UpdateStartedUI(g, windowRes);
                    break;
                case Phase.Capturing:
                    UpdateCapturingUI(g, windowRes);
                    break;
                case Phase.Training:
                    UpdateTrainingUI(g,windowRes);
                    break;
                case Phase.Testing:
                    UpdateTestingUI(g,windowRes);
                    break;
                case Phase.Feedback:
                    UpdateFeedbackUI(g, windowRes);
                    break;
                case Phase.Transitioning:
                    UpdateTransitionUI(g,windowRes);
                    break;
                case Phase.Break:
                    UpdateBreakUI(g, windowRes);
                    break;
                case Phase.Finished:
                    UpdateFinishedUI(g, windowRes);
                    break;
                default:
                    break;
            }

        }

        private void UpdateFeedbackUI(Graphics g, Size windowRes)
        {
            g.DrawString("Please answer the following regarding gesture " + Templates[activeTemplate].Name + ":", instructionFont, Brushes.Black, windowRes.Width * 0.05f, windowRes.Height * 0.05f);
        }

        private void UpdateCapturingUI(Graphics g, Size windowRes)
        {
            g.DrawString(instruction, instructionFont, Brushes.Black, windowRes.Width * 0.05f, windowRes.Height * 0.05f);
        }

        private void UpdateBreakUI(Graphics g, Size windowRes)
        {
            g.DrawString(instruction, instructionFont, Brushes.Black, windowRes.Width * 0.50f, windowRes.Height * 0.50f);
        }

        private void UpdateStartedUI(Graphics g, Size windowRes)
        {
            g.DrawString(instruction, instructionFont, Brushes.Black, windowRes.Width * 0.50f, windowRes.Height * 0.50f);
        }

        private void UpdateFinishedUI(Graphics g, Size windowRes)
        {
            g.DrawString(instruction, instructionFont, Brushes.Black, windowRes.Width * 0.50f, windowRes.Height * 0.50f);
        }

        private void UpdateTransitionUI(Graphics g, Size windowRes)
        {
            g.DrawString(instruction, instructionFont, Brushes.Black, windowRes.Width * 0.50f, windowRes.Height * 0.50f);
        }

        private void UpdateTestingUI(Graphics g, Size windowRes)
        {
            DrawTargetSquare(g, windowRes);
            g.DrawString(instruction, instructionFont, Brushes.Black, windowRes.Width * 0.05f, windowRes.Height * 0.05f);
            g.DrawString("Score: " + score, instructionFont, Brushes.Black, windowRes.Width * 0.80f, windowRes.Height * 0.10f);
        }

        private void UpdateTrainingUI(Graphics g, Size windowRes)
        {
            DrawTargetSquare(g, windowRes);
            g.DrawString(instruction, instructionFont, Brushes.Black, windowRes.Width * 0.05f, windowRes.Height * 0.05f);
            g.DrawString("Score: " + score, instructionFont, Brushes.Black, windowRes.Width * 0.80f, windowRes.Height * 0.10f);
        }
        #endregion


        private void DrawTargetSquare(Graphics g, Size windowRes)
        {
            if (!targetRectangleDrawn)
            {
                int xDim = (int)(windowRes.Width * 0.35f);
                int yDim = (int)(windowRes.Height * 0.50f);

                int xLocation = rng.Next(0, (windowRes.Width - xDim));
                int yLocation = rng.Next(0, (windowRes.Height - yDim));

                targetRectangle = new Rectangle(xLocation, yLocation, xDim, yDim);
                changeColor();
            }        

            g.DrawRectangle(targetRectanglePen, targetRectangle);
            targetRectangleDrawn = true;
            

        }

        private void changeColor()
        {
            Random randomGen = new Random();
            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            KnownColor randomColorName = names[randomGen.Next(names.Length)];
            targetRectanglePen = new Pen(Color.FromKnownColor(randomColorName),7);

        }

        public static void Shuffle(List<M.Gesture> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                M.Gesture value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public string Results()
        {
            string results = mainMatrix.ToString() + "\n";
            
            foreach(int key in windowMatrix.Keys)
            {
                results += windowMatrix[key].ToString() + "\n";
            }

            return results;
        }

        public void WriteMatrixResults(ConfusionMatrix matrix, string fileName)
        {
            StreamWriter r = new StreamWriter(baseDir + fileName +  ".txt");

            int TP, FP, TN, FN;
            double TPR, TNR, PPV, NPV, FPR, FDR, FNR, ACC, F1, MCC;

            r.WriteLine("# TP FP TN FN TPR TNR PPV NPV FPR FDR FNR ACC F1 MCC");

            //used to calculate average MCC, F1, Accuracy
            //ConfusionMatrix has these functions as convinience methods but, if used here, they would recalculate values unecessarily 
            double totalMCC = 0;
            double totalF1 = 0;
            double totalACC = 0;
            int totalGestureTypes = 0;

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

                totalGestureTypes++;
                totalMCC += MCC;
                totalF1 += F1;
                totalACC += ACC;

                r.WriteLine(gesture.Name + " " + TP + " " + FP + " " + TN + " " + FN + " " + TPR + " " + TNR + " " + PPV + " " + NPV + " " + FPR + " " + FDR + " " + FNR + " " + ACC + " " + F1 + " " + MCC);

            }

            double avgMCC = (totalMCC / (float)totalGestureTypes);
            double avgF1 = (totalF1 / (float)totalGestureTypes);
            double avgACC = (totalACC / (float)totalGestureTypes);

            r.WriteLine("# AVG-ACC AVG-F1 AVG-MCC ");
            r.WriteLine("AVG " + avgACC + " " + avgF1 + " " + avgMCC);
            r.Close();

        }

        public void WriteGesture(M.Gesture gesture, string dir, string classification)
        {
            //M.Point[] points, string gestureName, string fileName, string expectedAs = ""
            
            string expectedAs = Templates[activeTemplate].Name;
            string gestureName = gesture.Name; ;
            M.Point[] points = gesture.rawTraces.concatPoints();
            string fileName = String.Format("{0}{1}_Candidate-{2}.xml", dir, expectedAs, DateTime.Now.ToFileTime());

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");

                if(currentPhase == Phase.Capturing)
                    sw.WriteLine("<Gesture Name = \"{0}\">", Templates[activeTemplate].Name);
                else
                    sw.WriteLine("<Gesture Name = \"{0}\">", gestureName);

                // Used for automatic testing using Canned Gesture Feature.
                // Denotes what this gesture is supposed to be classified as.
                sw.WriteLine("<Label Expected = \"{0}\" Actual= \"{1}\"/>", expectedAs,classification);

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
                        points[i].X, points[i].Y, points[i].StrokeID, points[i].timestamp
                    );
                }
                sw.WriteLine("\t</Stroke>");
                sw.WriteLine("</Gesture>");
            }
        }


    }
}
