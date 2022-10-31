using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Graphics_Chart_
{

    public partial class Form1 : Form
    {
        Dictionary<double, double> x_Fx = new Dictionary<double, double>();      
        public Form1()
        {
            InitializeComponent();
            for(double i = 0; i <= 1.5; i += 0.3)
            {
                x_Fx.Add(i, getFx(i));
            }
            chartOfPolynom.ChartAreas.Add(new ChartArea("Math functions"));
        }
        #region "Math"
        double getAn(double an, double qn)
        {
            return an * qn;
        }
        double getQn(double n, double x)
        {
            return -(Math.PI * Math.PI * Math.Pow(x, 4) * (4 * n + 1)) / (4 * (2 * n + 1) * (2 * n + 2) * (4 * n + 5));
        }
        
        double getFx(double x)
        {

            double eps = 0.00001;
            double a = x;
            double sum = x;
            int j = 0;
            while (Math.Abs(a) > eps)
            {
                double qj = getQn(j, x);
                sum += getAn(a, qj);
                a = getAn(a, qj);
                j++;
            }
            return sum;
        }

        List<double> GetInterpollationPoints(int n, double a, double b)
        {
            double h = (b - a) / n;
            List<double> points = new List<double>();
            for (int i = 0; i <= n; i++)
            {
                points.Add(a);
                a += h;
            }
            return points;
            
        }
        double GetLnX(double x)
        {
            double sum = 0;
            List<double> points = null;
            if (checkBox1.Checked)
            {
                points = GetChebyshevInterpolatedPoints(x_Fx.Count);
            }
            else
            {
                 points = GetInterpollationPoints(x_Fx.Count, 0, 1.5);
            }

            for (int i = 0; i < x_Fx.Count; i++)
            {
                double prod = 1;
                for (int j = 0; j < x_Fx.Count; j++)
                {
                    if (i != j)
                    {
                        prod *= (x - points[j]) / (points[i] - points[j]);
                    }
                }
                sum += prod * getFx(points[i]);
            }
            return sum;
        }
        double GetLnX(double x,int n)
        {
            double sum = 0;
            List<double> points;
            if (checkBox1.Checked)
                 points = GetChebyshevInterpolatedPoints(n);
            else
            {
                points = GetInterpollationPoints(n,0,1.5);
            }
            for (int i = 0; i <= n; i++)
            {
                double prod = 1;
                for (int j = 0; j <= n; j++)
                {
                    if (i != j)
                    {
                        prod *= (x - points[j]) / (points[i] - points[j]);
                    }
                }
                sum += prod * getFx(points[i]);
            }
            return sum;
        }
        #endregion
        private void DrawFX(object sender, EventArgs e)
        {
            int n = x_Fx.Count;
            chartOfPolynom.Series.Clear();
            double h = (double)1.5 / n;
            double x = 0;
            Series mySeriesOfPoint = new Series("Функция");
            mySeriesOfPoint.ChartType = SeriesChartType.Line;
            mySeriesOfPoint.ChartArea = "Math functions";
            for (int  i = 0; i <= n; i++)
            {
                double f = Math.Abs(getFx(x));
                mySeriesOfPoint.Points.AddXY(x, f);
                x += h;

            }
            chartOfPolynom.Series.Add(mySeriesOfPoint);
        }

        private void DrawLNX(object sender, EventArgs e)
        {
            chartOfPolynom.Series.Clear();
            int n = (int)numericUpDown1.Value;
            int m = (int)countOfInterpolatedPoints.Value;
            double h = (double)1.5 / n;
            Series mySeriesOfPoint = new Series("Полином лагранжа");
            mySeriesOfPoint.ChartType = SeriesChartType.Line;
            mySeriesOfPoint.ChartArea = "Math functions";
            double x= 0;
            for (int i = 0; i <= n; i++)
            {
                double f = Math.Abs(GetLnX(x,x_Fx.Count));
                mySeriesOfPoint.Points.AddXY(x, f);
                x += h;
            }
            chartOfPolynom.Series.Add(mySeriesOfPoint);
        }
        double GetMaxErr(int n)
        {
            double h = (double)1.5 / n;
            double maxErr = 0;
            double x = 0;
            for (int i= 0; i< n; i++)
            {
                maxErr = Math.Max(maxErr, Math.Abs(GetLnX(x,x_Fx.Count) - getFx(x)));
                x += h;
            }
            return maxErr;
        }
        private void DrawDependeceCountOfInerpolationPointsAndError(object sender, EventArgs e)
        {
            chartOfPolynom.Series.Clear();
            Series mySeriesOfPoint = new Series("err : n");
            mySeriesOfPoint.ChartType = SeriesChartType.Line;
            mySeriesOfPoint.ChartArea = "Math functions";
            int n = (int)numericUpDown1.Value;
            for (int i = 2; i < n; i++)
            {
                InterpolatedPointsChanger(i);
                 if(i%5 !=0)
                     mySeriesOfPoint.Points.AddXY(i, GetMaxErr(i));
                
            }
            chartOfPolynom.Series.Add(mySeriesOfPoint);
        }

        private void DrawError(object sender, EventArgs e)
        {
            chartOfPolynom.Series.Clear();
            int n = (int)numericUpDown1.Value;
            
            double x = 0;
            double h = (double)1.5 / n;
            Series mySeriesOfPoint = new Series("Ошибка");
            mySeriesOfPoint.ChartType = SeriesChartType.Line;
            mySeriesOfPoint.ChartArea = "Math functions";
            for (int i = 0; i <= n; i++)
            {
                double f = Math.Abs(GetLnX(x,x_Fx.Count) - getFx(x));
                mySeriesOfPoint.LegendText = f.ToString();
                mySeriesOfPoint.Points.AddXY(x, f);
                x += h;
            }            
            chartOfPolynom.Series.Add(mySeriesOfPoint);
            
        }

        private void InterpolatedPointsChanger(int n)
        {
            double h = 1.5/n;
            x_Fx.Clear();
            if (checkBox1.Checked)
            {
                List<double> points = GetChebyshevInterpolatedPoints(n);
                for (int i = 0; i<points.Count; i++)
                {
                    x_Fx.Add(points[i], getFx(points[i]));
                }
            }
            else
            {
                for (double i = 0; i <= 1.5; i += h)
                {
                    x_Fx.Add(i, getFx(i));
                }
            }
        }

        private void countOfInterpolatedPoints_ValueChanged(object sender, EventArgs e)
        {
            int n = (int)countOfInterpolatedPoints.Value ;
            double h = 1.5 / n;
            x_Fx.Clear();
            double x = 0;
            if (checkBox1.Checked)
            {
                SetChebyshevPoints(n);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    x_Fx.Add(x, getFx(x));
                    x += h;
                }
            }
        }
        List<double> GetChebyshevInterpolatedPoints(int n)
        {
            double a = 0;
            double b = 1.5;
            List<double> points = new List<double>();
            for (int i = 0; i <= n; i++)
            {
                points.Add((b + a) / 2 + ((b - a) / 2) * Math.Cos((2 * i + 1) * Math.PI / (2 * n + 2)));               
            }
            points.Reverse();
            return points;
        }
        void SetChebyshevPoints(int n)
        {
            
            double a = 0;
            double b = 1.5;
            double x;
            for (int i = 0; i <= n; i++)
            {
                x = (b + a) / 2 + ((b - a) / 2) * Math.Cos((2 * i + 1)*Math.PI / (2 * n + 2));
                x_Fx.Add(x, getFx(x));
            }
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            x_Fx.Clear();
            SetChebyshevPoints((int)countOfInterpolatedPoints.Value);
        }
    }
}
