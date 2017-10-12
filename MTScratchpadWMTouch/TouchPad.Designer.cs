// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

namespace MTScratchpadWMTouch
{
    public partial class TouchPad : WMTouchForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TouchPad));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customGesturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCustomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllCustomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runCannedGesturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runShrinkingWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.endTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automatedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kfoldsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trainingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trainTemplatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTrainingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.endTrainingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.experimentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.basicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userAdjustedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.endToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripSplitModeButton = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSplitAddButton = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSplitButtonHold = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSplitButtonClean = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSplitButtonRec = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSplitByPoly = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.rawFeaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.circGRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.glacierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.customGesturesToolStripMenuItem,
            this.displayToolStripMenuItem,
            this.testToolStripMenuItem,
            this.trainingToolStripMenuItem,
            this.experimentToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(944, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // customGesturesToolStripMenuItem
            // 
            this.customGesturesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.deleteAllToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.loadCustomToolStripMenuItem,
            this.removeAllCustomToolStripMenuItem,
            this.runCannedGesturesToolStripMenuItem,
            this.runShrinkingWindowToolStripMenuItem});
            this.customGesturesToolStripMenuItem.Name = "customGesturesToolStripMenuItem";
            this.customGesturesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.customGesturesToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.customGesturesToolStripMenuItem.Text = "Gestures";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.addToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // deleteAllToolStripMenuItem
            // 
            this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
            this.deleteAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.D)));
            this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.deleteAllToolStripMenuItem.Text = "Delete All";
            this.deleteAllToolStripMenuItem.Click += new System.EventHandler(this.deleteAllToolStripMenuItem_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.reloadToolStripMenuItem.Text = "Load Gestures";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // loadCustomToolStripMenuItem
            // 
            this.loadCustomToolStripMenuItem.Name = "loadCustomToolStripMenuItem";
            this.loadCustomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.loadCustomToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.loadCustomToolStripMenuItem.Text = "Load Custom";
            // 
            // removeAllCustomToolStripMenuItem
            // 
            this.removeAllCustomToolStripMenuItem.Name = "removeAllCustomToolStripMenuItem";
            this.removeAllCustomToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.removeAllCustomToolStripMenuItem.Text = "Remove Custom";
            this.removeAllCustomToolStripMenuItem.Click += new System.EventHandler(this.removeAllCustomToolStripMenuItem_Click);
            // 
            // runCannedGesturesToolStripMenuItem
            // 
            this.runCannedGesturesToolStripMenuItem.Name = "runCannedGesturesToolStripMenuItem";
            this.runCannedGesturesToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.runCannedGesturesToolStripMenuItem.Text = "Run Canned";
            this.runCannedGesturesToolStripMenuItem.Click += new System.EventHandler(this.toolStripRunCannedGesture_ButtonClick);
            // 
            // runShrinkingWindowToolStripMenuItem
            // 
            this.runShrinkingWindowToolStripMenuItem.Name = "runShrinkingWindowToolStripMenuItem";
            this.runShrinkingWindowToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.runShrinkingWindowToolStripMenuItem.Text = "Run Shrinking Window";
            this.runShrinkingWindowToolStripMenuItem.Click += new System.EventHandler(this.toolStripRunShrinkingWindow_ButtonClick);
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cleanToolStripMenuItem});
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.displayToolStripMenuItem.Text = "Display";
            // 
            // cleanToolStripMenuItem
            // 
            this.cleanToolStripMenuItem.Name = "cleanToolStripMenuItem";
            this.cleanToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.cleanToolStripMenuItem.Text = "Clean";
            this.cleanToolStripMenuItem.Click += new System.EventHandler(this.cleanToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startTestToolStripMenuItem,
            this.endTestToolStripMenuItem,
            this.automatedTestsToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // startTestToolStripMenuItem
            // 
            this.startTestToolStripMenuItem.Name = "startTestToolStripMenuItem";
            this.startTestToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.startTestToolStripMenuItem.Text = "Start Test";
            this.startTestToolStripMenuItem.Click += new System.EventHandler(this.startTestToolStripMenuItem_Click);
            // 
            // endTestToolStripMenuItem
            // 
            this.endTestToolStripMenuItem.Name = "endTestToolStripMenuItem";
            this.endTestToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.endTestToolStripMenuItem.Text = "End Test";
            this.endTestToolStripMenuItem.Click += new System.EventHandler(this.endTestToolStripMenuItem_Click);
            // 
            // automatedTestsToolStripMenuItem
            // 
            this.automatedTestsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kfoldsToolStripMenuItem});
            this.automatedTestsToolStripMenuItem.Name = "automatedTestsToolStripMenuItem";
            this.automatedTestsToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.automatedTestsToolStripMenuItem.Text = "Automated Tests";
            // 
            // kfoldsToolStripMenuItem
            // 
            this.kfoldsToolStripMenuItem.Name = "kfoldsToolStripMenuItem";
            this.kfoldsToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.kfoldsToolStripMenuItem.Text = "k-folds";
            this.kfoldsToolStripMenuItem.Click += new System.EventHandler(this.kFoldsToolStripMenuItem_Click);
            // 
            // trainingToolStripMenuItem
            // 
            this.trainingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trainTemplatesToolStripMenuItem});
            this.trainingToolStripMenuItem.Name = "trainingToolStripMenuItem";
            this.trainingToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.trainingToolStripMenuItem.Text = "Training";
            // 
            // trainTemplatesToolStripMenuItem
            // 
            this.trainTemplatesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startTrainingToolStripMenuItem,
            this.endTrainingToolStripMenuItem});
            this.trainTemplatesToolStripMenuItem.Name = "trainTemplatesToolStripMenuItem";
            this.trainTemplatesToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.trainTemplatesToolStripMenuItem.Text = "Train Templates";
            // 
            // startTrainingToolStripMenuItem
            // 
            this.startTrainingToolStripMenuItem.Name = "startTrainingToolStripMenuItem";
            this.startTrainingToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.startTrainingToolStripMenuItem.Text = "Start Training";
            this.startTrainingToolStripMenuItem.Click += new System.EventHandler(this.startTrainingToolStripMenuItem_Click);
            // 
            // endTrainingToolStripMenuItem
            // 
            this.endTrainingToolStripMenuItem.Name = "endTrainingToolStripMenuItem";
            this.endTrainingToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.endTrainingToolStripMenuItem.Text = "End Training";
            this.endTrainingToolStripMenuItem.Click += new System.EventHandler(this.endTrainingToolStripMenuItem_Click);
            // 
            // experimentToolStripMenuItem
            // 
            this.experimentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modeToolStripMenuItem,
            this.startToolStripMenuItem,
            this.endToolStripMenuItem});
            this.experimentToolStripMenuItem.Name = "experimentToolStripMenuItem";
            this.experimentToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.experimentToolStripMenuItem.Text = "Experiment";
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.basicToolStripMenuItem,
            this.userAdjustedToolStripMenuItem,
            this.randomToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.modeToolStripMenuItem.Text = "Mode";
            // 
            // basicToolStripMenuItem
            // 
            this.basicToolStripMenuItem.Name = "basicToolStripMenuItem";
            this.basicToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.basicToolStripMenuItem.Text = "Developer Templates";
            this.basicToolStripMenuItem.Click += new System.EventHandler(this.BasicModeClick);
            // 
            // userAdjustedToolStripMenuItem
            // 
            this.userAdjustedToolStripMenuItem.Name = "userAdjustedToolStripMenuItem";
            this.userAdjustedToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.userAdjustedToolStripMenuItem.Text = "User Templates";
            this.userAdjustedToolStripMenuItem.Click += new System.EventHandler(this.UserAdjustedModeClick);
            // 
            // randomToolStripMenuItem
            // 
            this.randomToolStripMenuItem.Name = "randomToolStripMenuItem";
            this.randomToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.randomToolStripMenuItem.Text = "Random";
            this.randomToolStripMenuItem.Click += new System.EventHandler(this.RandomExpModeClick);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.StartExperiment);
            // 
            // endToolStripMenuItem
            // 
            this.endToolStripMenuItem.Name = "endToolStripMenuItem";
            this.endToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.endToolStripMenuItem.Text = "End";
            this.endToolStripMenuItem.Click += new System.EventHandler(this.EndExeriment);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitModeButton,
            this.toolStripSplitAddButton,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripSplitButtonHold,
            this.toolStripSplitButtonClean,
            this.toolStripSplitButtonRec,
            this.toolStripSplitByPoly,
            this.toolStatusLabel,
            this.toolStripSplitButton1,
            this.toolStripDropDownButton1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 482);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(944, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripSplitModeButton
            // 
            this.toolStripSplitModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitModeButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitModeButton.Image")));
            this.toolStripSplitModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitModeButton.Name = "toolStripSplitModeButton";
            this.toolStripSplitModeButton.Size = new System.Drawing.Size(110, 20);
            this.toolStripSplitModeButton.Text = "Mode: Template";
            this.toolStripSplitModeButton.ButtonClick += new System.EventHandler(this.toolStripSplitModeButton_ButtonClick);
            // 
            // toolStripSplitAddButton
            // 
            this.toolStripSplitAddButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitAddButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitAddButton.Image")));
            this.toolStripSplitAddButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitAddButton.Name = "toolStripSplitAddButton";
            this.toolStripSplitAddButton.Size = new System.Drawing.Size(88, 20);
            this.toolStripSplitAddButton.Text = "Add Gesture";
            this.toolStripSplitAddButton.ButtonClick += new System.EventHandler(this.toolStripSplitAddButton_ButtonClick);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripSplitButtonHold
            // 
            this.toolStripSplitButtonHold.Enabled = false;
            this.toolStripSplitButtonHold.Name = "toolStripSplitButtonHold";
            this.toolStripSplitButtonHold.Size = new System.Drawing.Size(92, 20);
            this.toolStripSplitButtonHold.Text = "Hold Gesture";
            this.toolStripSplitButtonHold.ButtonClick += new System.EventHandler(this.toolStripSplitButtonHold_ButtonClick);
            // 
            // toolStripSplitButtonClean
            // 
            this.toolStripSplitButtonClean.Name = "toolStripSplitButtonClean";
            this.toolStripSplitButtonClean.Size = new System.Drawing.Size(79, 20);
            this.toolStripSplitButtonClean.Text = "Clean PAD";
            this.toolStripSplitButtonClean.ButtonClick += new System.EventHandler(this.toolStripSplitButtonClean_ButtonClick);
            // 
            // toolStripSplitButtonRec
            // 
            this.toolStripSplitButtonRec.Name = "toolStripSplitButtonRec";
            this.toolStripSplitButtonRec.Size = new System.Drawing.Size(111, 20);
            this.toolStripSplitButtonRec.Text = "Recognition=On";
            this.toolStripSplitButtonRec.ButtonClick += new System.EventHandler(this.toolStripSplitButtonRec_ButtonClick);
            // 
            // toolStripSplitByPoly
            // 
            this.toolStripSplitByPoly.Name = "toolStripSplitByPoly";
            this.toolStripSplitByPoly.Size = new System.Drawing.Size(59, 20);
            this.toolStripSplitByPoly.Text = "By BB  ";
            this.toolStripSplitByPoly.ButtonClick += new System.EventHandler(this.toolStripSplitByPoly_ButtonClick);
            // 
            // toolStatusLabel
            // 
            this.toolStatusLabel.Name = "toolStatusLabel";
            this.toolStatusLabel.Size = new System.Drawing.Size(30, 17);
            this.toolStatusLabel.Text = "IDLE";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(86, 20);
            this.toolStripSplitButton1.Text = "SetExpected";
            this.toolStripSplitButton1.ButtonClick += new System.EventHandler(this.toolStripSplitButton1_ButtonClick_1);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rawFeaturesToolStripMenuItem,
            this.circGRToolStripMenuItem,
            this.toolStripMenuItem2,
            this.pToolStripMenuItem1});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(72, 20);
            this.toolStripDropDownButton1.Text = "Classifiers";
            // 
            // rawFeaturesToolStripMenuItem
            // 
            this.rawFeaturesToolStripMenuItem.Name = "rawFeaturesToolStripMenuItem";
            this.rawFeaturesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rawFeaturesToolStripMenuItem.Text = "Raw Features";
            this.rawFeaturesToolStripMenuItem.Click += new System.EventHandler(this.setRecognizer);
            // 
            // circGRToolStripMenuItem
            // 
            this.circGRToolStripMenuItem.Name = "circGRToolStripMenuItem";
            this.circGRToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.circGRToolStripMenuItem.Text = "CircGR";
            this.circGRToolStripMenuItem.Click += new System.EventHandler(this.setRecognizer);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem2.Text = "$1";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.setRecognizer);
            // 
            // glacierToolStripMenuItem
            // 
            this.glacierToolStripMenuItem.Name = "glacierToolStripMenuItem";
            this.glacierToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // pToolStripMenuItem
            // 
            this.pToolStripMenuItem.Name = "pToolStripMenuItem";
            this.pToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // pToolStripMenuItem1
            // 
            this.pToolStripMenuItem1.Name = "pToolStripMenuItem1";
            this.pToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.pToolStripMenuItem1.Text = "$P";
            this.pToolStripMenuItem1.Click += new System.EventHandler(this.setRecognizer);
            // 
            // TouchPad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 504);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TouchPad";
            this.Text = "MTScratchpadWMTouch";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TouchPad_FormClosed);
            this.Load += new System.EventHandler(this.TouchPad_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customGesturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAllToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitAddButton;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStatusLabel;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitModeButton;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCustomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllCustomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runCannedGesturesToolStripMenuItem; //recognize premade candidates
        private System.Windows.Forms.ToolStripMenuItem displayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem endTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonHold;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonClean;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonRec;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitByPoly;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem trainingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trainTemplatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startTrainingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem endTrainingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem automatedTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kfoldsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem glacierToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem rawFeaturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem circGRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem experimentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem basicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userAdjustedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem endToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runShrinkingWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem randomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem pToolStripMenuItem1;
    }
}

