using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SSAppend
{
    public class FormSelection : Form
    {
        #region Properties
        
        private Point start;
        private Point end;
        private bool selecting = false;
        private Cursor originalCursor;

        public Point StartPoint { get; private set; }
        public Point EndPoint { get; private set; }


        #endregion Properties

        public FormSelection()
        {
            this.BackColor = Color.Black;
            this.Opacity = 0.5;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.MouseDown += new MouseEventHandler(FormSelection_MouseDown);
            this.MouseMove += new MouseEventHandler(FormSelection_MouseMove);
            this.MouseUp += new MouseEventHandler(FormSelection_MouseUp);

            originalCursor = Cursor.Current;
            this.Cursor = Cursors.Cross;
        }

        public Rectangle SelectArea()
        {
            this.ShowDialog();
            return new Rectangle(
                Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y),
                Math.Abs(end.X - start.X),
                Math.Abs(end.Y - start.Y)
                );
        }

        private void FormSelection_MouseDown(object sender, MouseEventArgs e)
        {
            selecting = true;
            start = e.Location;
            StartPoint = e.Location;
        }

        private void FormSelection_MouseMove(object sender, MouseEventArgs e)
        {
            if (selecting)
            {
                end = e.Location;
                this.Invalidate();
            }
        }

        private void FormSelection_MouseUp(object sender, MouseEventArgs e)
        {
            selecting = false;
            end = e.Location;
            EndPoint = e.Location;
            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (selecting)
            {
                Rectangle select = new Rectangle(
                    Math.Min(start.X, end.X),
                    Math.Min(start.Y, end.Y),
                    Math.Abs(end.X - start.X),
                    Math.Abs(end.Y - start.Y)
                );


                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddRectangle(this.ClientRectangle);
                    path.AddRectangle(select);
                    using (Brush highlightBrush = new SolidBrush(Color.FromArgb(80, Color.White)))
                    {
                        e.Graphics.FillRectangle(highlightBrush, select);
                    }

                }

                using (Pen pen = new Pen(Color.White, 3))
                {
                    e.Graphics.DrawRectangle(pen, select);
                }
            }      
        }
    }
}
