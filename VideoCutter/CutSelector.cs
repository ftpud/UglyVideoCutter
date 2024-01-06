using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoCutter
{

    public partial class CutSelector : UserControl
    {
        public double LeftValue => Math.Min(Max, Math.Max(Min, ((double)left / Width) * Max));
        public double RightValue => Math.Max(Min, Math.Min(Max, ((double)right / Width) * Max));

        public double Position => Math.Max(Min, Math.Min(Max, ((double)pos / Width) * Max));

        public event EventHandler<ChangeEventArgs> OnChanged;
        public event EventHandler<MoveEventArgs> OnPosMove;

        int left = 0;
        int right = 1;
        int pos = 0;

        public double Max = 100;
        public double Min = 0;



        public CutSelector()
        {
            InitializeComponent();
            MouseDown += CutSelector_MouseDown;
            MouseUp += CutSelector_MouseUp;
            MouseMove += CutSelector_MouseMove;
            MouseLeave += CutSelector_MouseLeave;

            panel2.MouseLeave += Panel2_MouseLeave;
            panel2.MouseEnter += Panel2_MouseEnter;

            panel2.MouseMove += Panel2_MouseMove;

            panel2.Width = 1;
        }

        private void CutSelector_MouseLeave(object sender, EventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.Blue))
            {
                panel1.CreateGraphics().FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void Panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panel2.Width = 1;
                panel2.Left = panel2.Left + e.Location.X;
                pos = panel2.Left;


                if (OnPosMove != null)
                {
                    OnPosMove.Invoke(this, new MoveEventArgs() { Position = this.Position });
                }
            }
            else {
                panel2.Width = 5;
            }
            
        }

        private void Panel2_MouseEnter(object sender, EventArgs e)
        {
            panel2.Width = 5;
        }

        private void Panel2_MouseLeave(object sender, EventArgs e)
        {
            panel2.Width = 1;
        }


        bool pressLeft = false;


        private void ProcessChange(int xPosition)
        {
            int calculated_X = xPosition;



            if (pressLeft)
            {
                if (calculated_X >= left + right) calculated_X = left + right - 1;

                left = calculated_X;
                int size = panel1.Width + (panel1.Left - left);
                right = size;
                //right = panel1.Width;
                if (OnChanged != null)
                {
                    OnChanged.Invoke(this, new ChangeEventArgs()
                    {
                        LeftChanged = true,
                        RightChanged = false,
                        Left = LeftValue,
                        Right = RightValue
                    });
                }
            }
            else
            {
                if (calculated_X <= left) calculated_X = left + 1;

                right = calculated_X - panel1.Left;
                if (OnChanged != null)
                {
                    OnChanged.Invoke(this, new ChangeEventArgs()
                    {
                        LeftChanged = false,
                        RightChanged = true,
                        Left = LeftValue,
                        Right = RightValue
                    });
                }
            }

            redraw();

        }

        private void CutSelector_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ProcessChange(e.Location.X);
            }



            if (e.Location.X < (left + (right / 2)))
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(panel1.ClientRectangle,
                                                               Color.DarkBlue,
                                                               Color.Blue,
                                                               0F))
                {
                    panel1.CreateGraphics().FillRectangle(brush, this.ClientRectangle);
                }
            }
            else
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(panel1.ClientRectangle,
                                                               Color.Blue,
                                                               Color.DarkBlue,
                                                               0F))
                {
                    panel1.CreateGraphics().FillRectangle(brush, this.ClientRectangle);
                }
            }
        }

        private void CutSelector_MouseUp(object sender, MouseEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private void CutSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Location.X < (left + (right / 2) ))
            {
                pressLeft = true;
            }
            else
            {
                pressLeft = false;
            }
            
            ProcessChange(e.Location.X);
        }

        private void CutSelector_Load(object sender, EventArgs e)
        {

        }

        void redraw()
        {
            panel1.Left = left;
            panel1.Width = right;
            panel1.Height = Height;
            panel1.Top = 0;
        }


        public void Reset()
        {

            left = Width / 2 - Width / 8;
            right = Width / 4;
            redraw();
        }

        public void SetCursor(double pos) {
            //
            panel2.Top = 0;
            panel2.Height = Height;

            panel2.Left = (int) ( pos / Max * Width );
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    public class ChangeEventArgs : EventArgs
    {

        public bool LeftChanged = false;
        public bool RightChanged = false;
        public double Left = 0;
        public double Right = 0;

    }

    public class MoveEventArgs : EventArgs
    {

        public double Position = 0;

    }

}
