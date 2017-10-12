using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using M =MTGRLibrary;
namespace MTScratchpadWMTouch
{
    public partial class FrmLog : Form
    {
        bool selected = false; 
        
        private Dictionary<String, M.Gesture> map = new Dictionary<String,M.Gesture>(); 
        public FrmLog()
        {
            InitializeComponent();
        }

        //does not add new line char
        public void addText(string rt, bool verbose = false)
        {
            String t = "";
            if (verbose)
                t += "[" + DateTime.Now.ToString("hh.mm.ss.ffffff") + "] ";

                
            
            this.textBox1.AppendText(t+ rt);
        }

        //adds new line char
        public void addLine(string rt, bool verbose = true)
        {
            String t = "";
            if (verbose)
                t += "[" + DateTime.Now.ToString("hh.mm.ss.ffffff") + "] ";


            this.textBox1.AppendText(t + rt + Environment.NewLine);
        }

        
        public void clearText()
        {
            this.textBox1.Clear();
            this.textBox1.Text = "";
        }

        public void draw(TouchPad frm, M.Gesture gesture)
        {
            throw new NotImplementedException("Not completed yet"); 
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }
    
    }
}
