using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        public int PointNum
        {
            get { return (int)GetValue(PointNumProperty); }
            set
            {
                SetValue(PointNumProperty, value);
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
            }
        }
        public float _MaxValue
        {
            get { return (float)GetValue(MaxValueProperty); }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }
        public float _MinValue
        {
            get { return (float)GetValue(MinValueProperty); }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }
        internal int _Width
        {
            get => width;
            set
            {
                width = value;
            }
        }
        internal int _Height
        {
            get => height;
            set
            {
                height = value;
            }
        }

        public Sequence _Sequence
        {
            get { return (Sequence)GetValue(SequenceProperty); }
            set
            {
                SetValue(SequenceProperty, value);
            }
        }

        public bool _UpdatePlot
        {
            get { return (bool)GetValue(UpdatePlotProperty); }
            set
            {
                SetValue(UpdatePlotProperty, value);
                UpdatePlot();
            }
        }

        public bool _ClearData
        {
            get { return (bool)GetValue(ClearDataProperty); }
            set
            {
                SetValue(ClearDataProperty, value);
              
            }
        }
        public int _ClearDataByIndex
        {
            get { return (int)GetValue(ClearDataByIndexProperty); }
            set
            {
                SetValue(ClearDataByIndexProperty, value);
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
            DependencyPropertyDescriptor UpdatePlotdp = DependencyPropertyDescriptor.FromProperty(UpdatePlotProperty, typeof(PlotChart));
            UpdatePlotdp.AddValueChanged(this, delegate
            {
                if (_UpdatePlot)
                {
                    UpdatePlot();
                }
                _UpdatePlot = false;
            });

            DependencyPropertyDescriptor ClearDatadp = DependencyPropertyDescriptor.FromProperty(ClearDataProperty, typeof(PlotChart));
            ClearDatadp.AddValueChanged(this, delegate
            {
                if (_ClearData)
                {
                    ClearData();
                }
                _ClearData = false;
            });

            DependencyPropertyDescriptor ClearDataByIndexdp = DependencyPropertyDescriptor.FromProperty(ClearDataByIndexProperty, typeof(PlotChart));
            ClearDataByIndexdp.AddValueChanged(this, (o,args) =>
            {
                if (_ClearDataByIndex > -1)
                {
                    ClearDataByIndex(_ClearDataByIndex);
                }
                _ClearDataByIndex = -1;
            });



        }
        // define depandency property
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("_MinValue", typeof(float), typeof(PlotChart),new PropertyMetadata(float.MinValue));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("_MaxValue", typeof(float), typeof(PlotChart),new PropertyMetadata(float.MaxValue));
        public static readonly DependencyProperty SequenceProperty = DependencyProperty.Register("_Sequence", typeof(Sequence), typeof(PlotChart));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PlotChart));
        public static readonly DependencyProperty PointNumProperty = DependencyProperty.Register("PointNum", typeof(int), typeof(PlotChart),new PropertyMetadata(50));
        public static readonly DependencyProperty UpdatePlotProperty = DependencyProperty.Register("_UpdatePlot", typeof(bool), typeof(PlotChart), new PropertyMetadata(false));
        public static readonly DependencyProperty ClearDataProperty = DependencyProperty.Register("_ClearData", typeof(bool), typeof(PlotChart), new PropertyMetadata(false));
        public static readonly DependencyProperty ClearDataByIndexProperty = DependencyProperty.Register("_ClearDataByIndex", typeof(int), typeof(PlotChart), new PropertyMetadata(-1));


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
                G.DrawString((i * 10).ToString(), new Font("宋体", 8), Brushes.Black, new PointF(cpt.X - 30, cpt.Y - i * HeightPerPot - 6));
                G.DrawLine(Pens.Black, cpt.X - 3, cpt.Y - i * HeightPerPot, cpt.X, cpt.Y - i * HeightPerPot);

                // draw x-axis scale
                G.DrawString((i * 10).ToString(), new Font("宋体", 8), Brushes.Black, new PointF(cpt.X + i * WidthPerPot - 6, cpt.Y));
                G.DrawLine(Pens.Black, cpt.X + i * WidthPerPot, cpt.Y, cpt.X + i * WidthPerPot, cpt.Y - 3);

            }

            PlotBox.Source = ChangeBitmapToImageSource(Image);
        }

        private bool IsEmpty = true;

        public void UpdatePlot()
        {
            if (_Sequence == null || _Sequence.IsNull())
            {
                if(_Sequence.Mode == PaintMode.REPAINT_PER_DATA)
                {
                    CreateEmptyPlot();
                }
                else
                {
                    UpdatePreNewPlot();
                }
                
                return;
            }
            // repaint every data point
            if (_Sequence.Mode == PaintMode.REPAINT_PER_DATA)
            {
                UpdateWholePlot();
            }
            else // draw new point only
            {
                if (IsEmpty)
                {
                    UpdatePreNewPlot();
                    IsEmpty = false;
                }
                else
                {
                    UpdateNewPlot();
                }
            }
           
        }

        public void ClearData()
        {
            if (_Sequence == null)
            {
                return;
            }
            _Sequence.ClearData();
            IsEmpty = true;
        }

        public void ClearDataByIndex(int index)
        {
            if (_Sequence == null)
            {
                return;
            }
            if (_Sequence.ClearDataByIndex(index))
            {
                IsEmpty = true;
            }
        }

        private void UpdatePreNewPlot()
        {
            // init picture
            Image = new Bitmap(_Width, _Height);
            G = Graphics.FromImage(Image);
            G.Clear(Color.White);

            float diff = (float)Math.Round((_MaxValue - _MinValue) / 6, 2);
            int pointnum = _Sequence.GetMaxLength();
            int HeightPerPot = _Height / 8;
            int WidthPerPot = (_Width - 35) / PointNum;


            PointF cpt = new PointF(diff.ToString().Length + 35, _Height - 50);// centre point

            G.DrawString(Title, new Font("宋体", 14), Brushes.Black, new PointF(cpt.X + _Width / 2 - 80, cpt.X));// title
            // x axis
            G.DrawLine(Pens.Black, cpt.X, cpt.Y, _Width - cpt.X + 35, cpt.Y);
            // y axis
            G.DrawLine(Pens.Black, cpt.X, cpt.Y, cpt.X, _Height - cpt.Y);

            for (int i = 0; i < 7; i++)
            {
                G.DrawString(((diff * i) + _MinValue).ToString(), new Font("宋体", 8), Brushes.Black, new PointF(cpt.X - WidthPerPot.ToString().Length * 10, cpt.Y - i * HeightPerPot - 6));
                G.DrawLine(Pens.Black, cpt.X - 3, cpt.Y - i * HeightPerPot, cpt.X, cpt.Y - i * HeightPerPot);
            }
            for (int i = 0; i < PointNum; i++)
            {
                // draw x-axis project
                G.DrawString(i.ToString(), new Font("宋体", 8), Brushes.Black, new PointF(cpt.X + i * WidthPerPot - 5, cpt.Y + 5));
                // x-axis scale
                G.DrawLine(Pens.Black, cpt.X + i * WidthPerPot, cpt.Y, cpt.X + i * WidthPerPot, cpt.Y - 3);

            }

            int titlenum = 1;
            for (int j = 0; j < _Sequence.GetCount(); j++)
            {
                float[] d = _Sequence.PlotChartPoints[j].Values.ToArray();
                Pen mycolor = _Sequence.PlotChartPoints[j].Color;
                string tempTitle = _Sequence.PlotChartPoints[j].Title;
                if (tempTitle != null)
                {
                    G.DrawString(tempTitle, new Font("宋体", 8), Brushes.Black, new PointF(cpt.X + 30, cpt.X + titlenum * 24));//图表标题
                    G.DrawLine(mycolor, cpt.X + 120, cpt.X + titlenum * 24 + 10, cpt.X + 160, cpt.X + titlenum * 24 + 10);
                    titlenum++;
                }
                for (int i = 0; i < _Sequence.PlotChartPoints[j].Values.Count; i++)
                {
                    G.DrawEllipse(Pens.Black, cpt.X + i * WidthPerPot - 1.5f, cpt.Y - (d[i] - _MinValue) / diff * HeightPerPot - 1.5f, 3, 3);
                    G.FillEllipse(new SolidBrush(Color.Black), cpt.X + i * WidthPerPot - 1.5f, cpt.Y - (d[i] - _MinValue) / diff * HeightPerPot - 1.5f, 3, 3);
                    // draw broken line
                    if (i > 0) G.DrawLine(mycolor, cpt.X + (i - 1) * WidthPerPot, cpt.Y - (d[i - 1] - _MinValue) / diff * HeightPerPot, cpt.X + i * WidthPerPot, cpt.Y - (d[i] - _MinValue) / diff * HeightPerPot);
                }
            }

            PlotBox.Source = ChangeBitmapToImageSource(Image);
        }

        // in this mode you would better set _MaxValue, _MinValue and PointNum you want to show
        private void UpdateNewPlot()
        {
            G = Graphics.FromImage(Image);

            float diff = (float)Math.Round((_MaxValue - _MinValue) / 6, 2);
            int pointnum = _Sequence.GetMaxLength();
            int HeightPerPot = _Height / 8;
            int WidthPerPot = (_Width-35) / PointNum;

            PointF cpt = new PointF(diff.ToString().Length + 35, _Height - 50);// centre point
            for (int j = 0; j < _Sequence.GetCount(); j++)
            {
                float[] d = _Sequence.PlotChartPoints[j].Values.ToArray();
                Pen mycolor = _Sequence.PlotChartPoints[j].Color;

                for (int i = _Sequence.HasDrawed[j]; i < _Sequence.PlotChartPoints[j].Values.Count; i++)
                {
                    _Sequence.HasDrawed[j]++;
                    G.DrawEllipse(Pens.Black, cpt.X + i * WidthPerPot - 1.5f, cpt.Y - (d[i] - _MinValue) / diff * HeightPerPot - 1.5f, 3, 3);
                    G.FillEllipse(new SolidBrush(Color.Black), cpt.X + i * WidthPerPot - 1.5f, cpt.Y - (d[i] - _MinValue) / diff * HeightPerPot - 1.5f, 3, 3);
                    // draw broken line
                    if (i > 0) G.DrawLine(mycolor, cpt.X + (i - 1) * WidthPerPot, cpt.Y - (d[i - 1] - _MinValue) / diff * HeightPerPot, cpt.X + i * WidthPerPot, cpt.Y - (d[i] - _MinValue) / diff * HeightPerPot);
                }
            }

            PlotBox.Source = ChangeBitmapToImageSource(Image);
        }

        private void UpdateWholePlot()
        {
            // init picture
            Image = new Bitmap(_Width, _Height);
            G = Graphics.FromImage(Image);
            G.Clear(Color.White);

            float max = _Sequence.GetMaxValue();
            max = max > _MaxValue ? _MaxValue : max;
            float min = _Sequence.GetMinValue();
            min = min < _MinValue ? _MinValue : min;
            float diff = (float)Math.Round((max - min) / 6, 2);
            int pointnum = _Sequence.GetMaxLength();
            int HeightPerPot = _Height / 8;
            int WidthPerPot = (_Width - 35) / pointnum;

            PointF cpt = new PointF(diff.ToString().Length + 35, _Height - 50);// centre point

            G.DrawString(Title, new Font("宋体", 14), Brushes.Black, new PointF(cpt.X + _Width / 2 - 80, cpt.X));// title
            // x axis
            G.DrawLine(Pens.Black, cpt.X, cpt.Y, _Width - cpt.X + 35, cpt.Y);
            // y axis
            G.DrawLine(Pens.Black, cpt.X, cpt.Y, cpt.X, _Height - cpt.Y);

            for (int i = 0; i < 7; i++)
            {
                G.DrawString(((diff * i) + min).ToString(), new Font("宋体", 8), Brushes.Black, new PointF(cpt.X - WidthPerPot.ToString().Length * 10, cpt.Y - i * HeightPerPot - 6));
                G.DrawLine(Pens.Black, cpt.X - 3, cpt.Y - i * HeightPerPot, cpt.X, cpt.Y - i * HeightPerPot);
            }
            for (int i = 0; i < pointnum; i++)
            {
                // draw x-axis project
                G.DrawString(i.ToString(), new Font("宋体", 8), Brushes.Black, new PointF(cpt.X + i * WidthPerPot - 5, cpt.Y + 5));
                // x-axis scale
                G.DrawLine(Pens.Black, cpt.X + i * WidthPerPot, cpt.Y, cpt.X + i * WidthPerPot, cpt.Y - 3);

            }

            int titlenum = 1;
            for (int j = 0; j < _Sequence.GetCount(); j++)
            {
                float[] d = _Sequence.PlotChartPoints[j].Values.ToArray();
                Pen mycolor = _Sequence.PlotChartPoints[j].Color;
                string tempTitle = _Sequence.PlotChartPoints[j].Title;
                if (tempTitle != null)
                {
                    G.DrawString(tempTitle, new Font("宋体", 8), Brushes.Black, new PointF(cpt.X + 30, cpt.X + titlenum * 24));//图表标题
                    G.DrawLine(mycolor, cpt.X + 120, cpt.X + titlenum * 24 + 10, cpt.X + 160, cpt.X + titlenum * 24 + 10);
                    titlenum++;
                }
                for (int i = 0; i < _Sequence.PlotChartPoints[j].Values.Count; i++)
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
