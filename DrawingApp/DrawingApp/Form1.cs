using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawingApp
{
    enum Tool {
        LINE,
        RECTANGLE,
        PEN,
        CIRCLE,
        TEXT,
        ERASE,
        FILL
    }

    public partial class Form1 : Form
    {
        Bitmap bitmap = default(Bitmap);
        Graphics graphics = default(Graphics);
        Pen pen = new Pen(Color.Black);
        Pen eraserPen = new Pen(Color.White, 5);
        Point prevPoint = default(Point);
        Point currentPoint = default(Point);
        Tool currentTool = Tool.PEN;

        bool isMousePressed = false;

        public Form1()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            pictureBox1.Image = bitmap;
            graphics.Clear(Color.White);

            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            button3.Select();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                bitmap.Save(saveFileDialog1.FileName);
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                bitmap = Bitmap.FromFile(openFileDialog1.FileName) as Bitmap;
                pictureBox1.Image = bitmap;
                graphics = Graphics.FromImage(bitmap);
            }
        }

        private void ToolButtonClicked(object sender, EventArgs e)
        {
            
            Button button = sender as Button;
            if (button != null)
                switch (button.Text)
                {
                    case "Line":
                        currentTool = Tool.LINE;
                        break;
                    case "Rectangle":
                        currentTool = Tool.RECTANGLE;
                        break;
                    case "Pen":
                        currentTool = Tool.PEN;
                        break;
                    case "Fill":
                        currentTool = Tool.FILL;
                        break;
                    case "Circle":
                        currentTool = Tool.CIRCLE;
                        break;
                    case "Eraser":
                        currentTool = Tool.ERASE;
                        break;
                    case "Text":
                        currentTool = Tool.TEXT;
                        break;
                    default:
                        break;
                }

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripStatusLabel1.Text = e.Location.ToString();

            if (isMousePressed)
            {
                switch (currentTool)
                {
                    case Tool.TEXT:
                    case Tool.LINE:
                    case Tool.RECTANGLE:
                    case Tool.CIRCLE:
                        currentPoint = e.Location;
                        break;
                    case Tool.ERASE:
                    case Tool.PEN:
                        prevPoint = currentPoint;
                        currentPoint = e.Location;
                        if (currentTool == Tool.PEN)
                            graphics.DrawLine(pen, prevPoint, currentPoint);
                        else
                            graphics.DrawLine(eraserPen, prevPoint, currentPoint);
                        break;
                    default:
                        break;
                }
                
                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            prevPoint = e.Location;
            currentPoint = e.Location;
            isMousePressed = true;
            /*if (currentTool == Tool.FILL)
            {
                Utils.Fill(bitmap, currentPoint, bitmap.GetPixel(e.X, e.Y), pen.Color);
                graphics = Graphics.FromImage(bitmap);
                pictureBox1.Image = bitmap;
                pictureBox1.Refresh();
            }*/
            if (currentTool == Tool.FILL)
            {
                MapFill mf = new MapFill();
                mf.Fill(graphics, currentPoint, pen.Color, ref bitmap);
                graphics = Graphics.FromImage(bitmap);
                pictureBox1.Image = bitmap;
                pictureBox1.Refresh();
            }
                
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMousePressed = false;
            switch (currentTool)
            {
                case Tool.LINE:
                    graphics.DrawLine(pen, prevPoint, currentPoint);
                    break;
                case Tool.RECTANGLE:
                    graphics.DrawRectangle(pen, GetRectangle(prevPoint, currentPoint));
                    break;
                case Tool.CIRCLE:
                    graphics.DrawEllipse(pen, GetRectangle(prevPoint, currentPoint));
                    break;
                case Tool.TEXT:
                    pictureBox1.Controls.Add(new TextBox() { Name = "", Location = prevPoint, BorderStyle = BorderStyle.None, Size = new Size(Math.Abs(prevPoint.X - currentPoint.X), 100) });
                    
                    break;
                default:
                    break;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            switch (currentTool)
            {
                case Tool.LINE:
                    e.Graphics.DrawLine(pen, prevPoint, currentPoint);
                    break;
                case Tool.RECTANGLE:
                    e.Graphics.DrawRectangle(pen, GetRectangle(prevPoint, currentPoint));
                    break;
                case Tool.CIRCLE:
                    e.Graphics.DrawEllipse(pen, GetRectangle(prevPoint, currentPoint));
                    break;
                default:
                    break;
            }
        }

        Rectangle GetRectangle(Point pPoint, Point cPoint)
        {

            return new Rectangle 
            { 
                X = Math.Min(pPoint.X, cPoint.X),
                Y = Math.Min(pPoint.Y, cPoint.Y),
                Width = Math.Abs(pPoint.X - cPoint.X),
                Height = Math.Abs(pPoint.Y - cPoint.Y)
            };
        }

        private void PickColor_Click(object sender, EventArgs e)
        {
            DialogResult colorResult = colorDialog1.ShowDialog();
            if (colorResult == DialogResult.OK)
            {
                pen.Color = colorDialog1.Color;
                panel1.BackColor = colorDialog1.Color;
            }
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            pen.Width = trackBar1.Value;
            eraserPen.Width = trackBar1.Value * 5;
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(bitmap);
            pictureBox1.Image = bitmap;
            graphics.Clear(Color.White);
            pictureBox1.Controls.Clear();
        }
    }
}