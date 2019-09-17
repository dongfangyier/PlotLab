﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;

namespace PlotLab
{
    public class Sequence
    {
        public List<SequenceEntity> PlotChartPoints = new List<SequenceEntity>();
        private int count = 0;
        public PaintMode Mode = PaintMode.REPAINT_PER_DATA;
        public List<int> HasDrawed = new List<int>();

        public Sequence(List<SequenceEntity> entities, PaintMode mode = PaintMode.REPAINT_PER_DATA)
        {
            PlotChartPoints.Clear();
            HasDrawed.Clear();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Color == null)
                {
                    entities[i].Color = MyPens.getEntityByIndex(count);
                }
                PlotChartPoints.Add(entities[i]);
                HasDrawed.Add(entities[i].Values.Count); // because the first time will always repaint
                count++;
                if (count > 9)
                {
                    count = 0;
                }
            }
            Mode = mode;
        }

        public bool IsNull()
        {
            if (PlotChartPoints.Count == 0)
            {
                return true;
            }
            return false;
        }
        public int GetCount()
        {
            return PlotChartPoints.Count;
        }
        public float GetMinValue()
        {
            float min = float.MaxValue;
            for (int i = 0; i < PlotChartPoints.Count; i++)
            {
                float temp = PlotChartPoints[i].GetMin();
                if (min > temp)
                {
                    min = temp;
                }
            }
            return min;
        }
        public float GetMaxValue()
        {
            float max = float.MinValue;
            for (int i = 0; i < PlotChartPoints.Count; i++)
            {
                float temp = PlotChartPoints[i].GetMax();
                if (max < temp)
                {
                    max = temp;
                }
            }
            return max;
        }
        public int GetMaxLength()
        {
            int max = int.MinValue;
            for (int i = 0; i < PlotChartPoints.Count; i++)
            {
                int temp = PlotChartPoints[i].Values.Count;
                if (max < temp)
                {
                    max = temp;
                }
            }
            return max;
        }

        internal void ClearData()
        {
            for(int i = 0; i < PlotChartPoints.Count; i++)
            {
                PlotChartPoints[i].Values.Clear();
                HasDrawed[i] = 0;
            }
        }

        internal bool ClearDataByIndex(int index)
        {
            if(index>= PlotChartPoints.Count)
            {
                return false;
            }
            PlotChartPoints[index].Values.Clear();
            HasDrawed[index] = 0;
            return true;
        }
    }

    public class SequenceEntity
    {
        public List<float> Values = new List<float>();
        public Pen Color = null;
        public string Title = string.Empty;

        public SequenceEntity(List<float> values, Pen color = null, string title = null)
        {
            Values = values;
            Color = color;
            Title = title;
        }

        public float GetMax()
        {
            return Values.Max();
        }

        public float GetMin()
        {
            return Values.Min();
        }
    }
    class MyPens
    {
        public static Pen getEntityByIndex(int index)
        {
            Pen color = null;
            switch (index % 10)
            {
                case 0:
                    color = Pens.Blue;
                    break;
                case 1:
                    color = Pens.Red;
                    break;
                case 2:
                    color = Pens.Green;
                    break;
                case 3:
                    color = Pens.Orange;
                    break;
                case 4:
                    color = Pens.Purple;
                    break;
                case 5:
                    color = Pens.Gray;
                    break;
                case 6:
                    color = Pens.Silver;
                    break;
                case 7:
                    color = Pens.Salmon;
                    break;
                case 8:
                    color = Pens.RosyBrown;
                    break;
                case 9:
                    color = Pens.Olive;
                    break;
                default:
                    color = Pens.Pink;
                    break;

            }
            return color;
        }
    }

    public enum PaintMode
    {
        REPAINT_PER_DATA = 0,
        NO_REPAINT = 1
    }
}
