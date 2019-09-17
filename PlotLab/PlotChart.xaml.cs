﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PlotLab
{
    /// <summary>
    /// PlotChart.xaml 的交互逻辑
    /// </summary>
    public partial class PlotChart : UserControl
    {
        #region attribute
        private float maxValue = float.MaxValue;
        private float minValue = float.MinValue;
        private int width = 700;
        private int height = 480;
        private Sequence seq = null;
        private string title = string.Empty;

        public string Title
        {
            get => title;
            set
            {
                title = value;
                UpdatePlot();
            }
        }
        public float _MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                UpdatePlot();
            }
        }
        public float _MinValue
        {
            get => minValue;
            set
            {
                minValue = value;
                UpdatePlot();
            }
        }
        public int _Width
        {
            get => width;
            set
            {
                width = value;
                UpdatePlot();
            }
        }
        public int _Height
        {
            get => height;
            set
            {
                height = value;
                UpdatePlot();
            }
        }

        public Sequence Sequence
        {
            get => seq;
            set
            {
                seq = value;
                // maybe we can update in several times later??
                UpdatePlot();
            }
        }

        #endregion

        #region parameter
        Bitmap Image = null;
        Graphics G = null;
        #endregion


        public PlotChart()
        {
            InitializeComponent();
        }

        private void Component_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePlot();
        }
        #region functions

        private void CreateEmptyPlot()
        {
            // init bitmap
            Image = new Bitmap(_Width, _Height);
            G = Graphics.FromImage(Image);
            G.Clear(Color.White);
            PointF cpt = new PointF(40, _Height - 50);//中心点
            // draw x axis
            G.DrawLine(Pens.Black, cpt.X, cpt.Y, _Width - cpt.X, cpt.Y);
            // draw y axis
            G.DrawLine(Pens.Black, cpt.X, cpt.Y, cpt.X, _Height - cpt.Y);
            int HeightPerPot = _Height / 8;
            int WidthPerPot = _Width / 8;
            G.DrawString(Title, new Font("宋体", 14), Brushes.Black, new PointF(cpt.X + 60, cpt.X));// title

            for (int i = 0; i < 7; i++)
            {
                // draw y axis
                G.DrawString((i * 10).ToString(), new Font("宋体", 11), Brushes.Black, new PointF(cpt.X - 30, cpt.Y - i * HeightPerPot - 6));
                G.DrawLine(Pens.Black, cpt.X - 3, cpt.Y - i * HeightPerPot, cpt.X, cpt.Y - i * HeightPerPot);

                // draw x-axis scale
                G.DrawString((i * 10).ToString(), new Font("宋体", 11), Brushes.Black, new PointF(cpt.X + i * WidthPerPot - 6, cpt.Y));
                G.DrawLine(Pens.Black, cpt.X + i * WidthPerPot, cpt.Y, cpt.X + i * WidthPerPot, cpt.Y - 3);

            }

            PlotBox.Source = ChangeBitmapToImageSource(Image);
        }

        private void UpdatePlot()
        {
            if (Sequence == null || Sequence.IsNull())
            {
                MessageBox.Show(Sequence.GetCount().ToString());
                CreateEmptyPlot();
                return;
            }

            // init picture
            Image = new Bitmap(_Width, _Height);
            G = Graphics.FromImage(Image);
            G.Clear(Color.White);

            float max = Sequence.GetMaxValue();
            max = max > _MaxValue ? _MaxValue : max;
            float min = Sequence.GetMinValue();
            min = min < _MinValue ? _MinValue : min;
            float diff = (float)Math.Round((max - min) / 6, 2);
            int pointnum = Sequence.GetMaxLength();
            int HeightPerPot = _Height / 8;
            int WidthPerPot = _Width / pointnum;

            PointF cpt = new PointF(diff.ToString().Length + 35, _Height - 50);// centre point

            G.DrawString(Title, new Font("宋体", 14), Brushes.Black, new PointF(cpt.X + _Width / 2 - 80, cpt.X));// title
            // x axis
            G.DrawLine(Pens.Black, cpt.X, cpt.Y, _Width - cpt.X, cpt.Y);
            // y axis
            G.DrawLine(Pens.Black, cpt.X, cpt.Y, cpt.X, _Height - cpt.Y);

            for (int i = 0; i < 7; i++)
            {
                G.DrawString(((diff * i) + min).ToString(), new Font("宋体", 11), Brushes.Black, new PointF(cpt.X - WidthPerPot.ToString().Length * 10, cpt.Y - i * HeightPerPot - 6));
                G.DrawLine(Pens.Black, cpt.X - 3, cpt.Y - i * HeightPerPot, cpt.X, cpt.Y - i * HeightPerPot);
            }
            for (int i = 0; i < pointnum; i++)
            {
                // draw x-axis project
                G.DrawString(i.ToString(), new Font("宋体", 11), Brushes.Black, new PointF(cpt.X + i * WidthPerPot - 5, cpt.Y + 5));
                // x-axis scale
                G.DrawLine(Pens.Black, cpt.X + i * WidthPerPot, cpt.Y, cpt.X + i * WidthPerPot, cpt.Y - 3);

            }

            int titlenum = 1;
            for (int j = 0; j < Sequence.GetCount(); j++)
            {
                float[] d = Sequence.PlotChartPoints[j].Values.ToArray();
                Pen mycolor = Sequence.PlotChartPoints[j].Color;
                string tempTitle = Sequence.PlotChartPoints[j].Title;
                if (tempTitle != null)
                {
                    G.DrawString(tempTitle, new Font("宋体", 8), Brushes.Black, new PointF(cpt.X + 30, cpt.X + titlenum * 24));//图表标题
                    G.DrawLine(mycolor, cpt.X + 120, cpt.X + titlenum * 24 + 10, cpt.X + 160, cpt.X + titlenum * 24 + 10);
                    titlenum++;
                }
                for (int i = 0; i < Sequence.PlotChartPoints[j].Values.Count; i++)
                {
                    G.DrawEllipse(Pens.Black, cpt.X + i * WidthPerPot - 1.5f, cpt.Y - (d[i] - min) / diff * HeightPerPot - 1.5f, 3, 3);
                    G.FillEllipse(new SolidBrush(Color.Black), cpt.X + i * WidthPerPot - 1.5f, cpt.Y - (d[i] - min) / diff * HeightPerPot - 1.5f, 3, 3);
                    // draw broken line
                    if (i > 0) G.DrawLine(mycolor, cpt.X + (i - 1) * WidthPerPot, cpt.Y - (d[i - 1] - min) / diff * HeightPerPot, cpt.X + i * WidthPerPot, cpt.Y - (d[i] - min) / diff * HeightPerPot);
                }
            }

            PlotBox.Source = ChangeBitmapToImageSource(Image);
        }

        private System.Windows.Media.ImageSource ChangeBitmapToImageSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            System.Windows.Media.ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return wpfBitmap;
        }

        #endregion

    }
}