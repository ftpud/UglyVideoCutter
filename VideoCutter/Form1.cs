using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoCutter
{
    public partial class Form1 : Form
    {
        double newPos = -1;
        OpenFileDialog openFileDialog = new OpenFileDialog();


        public Form1()
        {
            InitializeComponent();

            openFileDialog.Filter = "Video Files|*.ts;*.mp4;*.mkv;*.flv|All|*.*";

            // Hide the controls
            axWindowsMediaPlayer1.uiMode = "none";
            // axWindowsMediaPlayer1.Size = new System.Drawing.Size(500, 300);
            axWindowsMediaPlayer1.PlayStateChange += AxWindowsMediaPlayer1_PlayStateChange;

            cutSelector1.OnChanged += CutSelector1_OnChanged;
            cutSelector1.OnPosMove += CutSelector1_OnPosMove;
            cutSelector1.Reset();

            posTimer.Interval = 100;
            posTimer.Start();
            posTimer.Tick += PosTimer_Tick;

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);


        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) Console.WriteLine(file);

            try {
                axWindowsMediaPlayer1.URL = files.Last();
                openFileDialog.FileName = files.Last();
            } catch { }
             
        }

        private void CutSelector1_OnPosMove(object sender, MoveEventArgs e)
        {
            newPos = e.Position;
        }

        private void PosTimer_Tick(object sender, EventArgs e)
        {
            if (newPos != -1) { 
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition = newPos;
                newPos = -1;
            }

            lblTime.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;
            cutSelector1.SetCursor(axWindowsMediaPlayer1.Ctlcontrols.currentPosition);
        }

        Timer posTimer = new Timer();

        private void AxWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {


            if (e.newState == 3)
            {

                try
                {
                    double duration = axWindowsMediaPlayer1.Ctlcontrols.currentItem.duration;
                    double pos = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;

                    lblDuration.Text = duration.ToString();
                    cutSelector1.Max = duration;
                    //cutSelector1.Reset();
                }
                catch { }
            }
        }

        private void CutSelector1_OnChanged(object sender, ChangeEventArgs e)
        {
            if (e.LeftChanged)
            {
                lblLeft.Text = e.Left.ToString();
                //axWindowsMediaPlayer1.Ctlcontrols.currentPosition = e.Left;
                newPos = e.Left;
            }
            else if (e.RightChanged)
            {
                lblRight.Text = e.Right.ToString();
                //axWindowsMediaPlayer1.Ctlcontrols.currentPosition = e.Right;
                newPos = e.Left + e.Right;
            }
        }


  
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog(this);

            axWindowsMediaPlayer1.URL = openFileDialog.FileName; // "D:\\_IDOL_CUT\\Botan Stage - Yakousei Amuse.mkv";
            axWindowsMediaPlayer1.uiMode = "none";



        }

        private void button2_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.pause();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //cutSelector1.Reset();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string strCmdLine ="\"D:\\_IDOL_CUT\\ffmpeg.exe\"";

            var parmaters = $" -hwaccel cuda -hwaccel_output_format cuda -i \"{openFileDialog.FileName}\" -ss { Math.Round(cutSelector1.LeftValue, 2).ToString().Replace(",",".") } -t {Math.Round(cutSelector1.RightValue, 2).ToString().Replace(",", ".")} -async 1 -c:a copy -c:v h264_nvenc -preset p7 -b:v 5M \"D:\\_IDOL_CUT\\cut\\{textBox1.Text}{textBox2.Text}{textBox3.Text}.mp4\"";

            Console.WriteLine(parmaters);

            System.Diagnostics.Process.Start(strCmdLine, parmaters);
        }

        private void cutSelector1_Load(object sender, EventArgs e)
        {

        }
    }
}
