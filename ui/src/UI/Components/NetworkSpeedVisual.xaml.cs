// <copyright file="NetworkSpeedVisual.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirefoxPrivateNetwork.UI.Components
{
    /// <summary>
    /// Interaction logic for NetworkSpeedVisual.xaml.
    /// </summary>
    public partial class NetworkSpeedVisual : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Dependency property for the download speeds.
        /// </summary>
        public static readonly DependencyProperty DownloadSpeedsStringProperty = DependencyProperty.Register("DownloadSpeedsString", typeof(string), typeof(NetworkSpeedVisual), new PropertyMetadata(OnDownloadSpeedsStringChangedCallBack));

        /// <summary>
        /// Dependency property for the upload speeds.
        /// </summary>
        public static readonly DependencyProperty UploadSpeedsStringProperty = DependencyProperty.Register("UploadSpeedsString", typeof(string), typeof(NetworkSpeedVisual), new PropertyMetadata(OnUploadSpeedsStringChangedCallBack));

        /// <summary>
        /// Dependency property for whether the graph should be drawn.
        /// </summary>
        public static readonly DependencyProperty GraphShownProperty = DependencyProperty.Register("GraphShown", typeof(bool), typeof(NetworkSpeedVisual), new PropertyMetadata(OnGraphShownChangedCallBack));

        /// <summary>
        /// Dependency property for whether the connection is stable.
        /// </summary>
        public static readonly DependencyProperty IsStableProperty = DependencyProperty.Register("IsStable", typeof(bool), typeof(NetworkSpeedVisual), new PropertyMetadata(IsStableChangedCallBack));

        private static List<double> downloadSpeeds;
        private static List<double> uploadSpeeds;

        /// <summary>
        /// Points used in graph for download speeds.
        /// </summary>
        private static List<Point> downloadPoints;

        /// <summary>
        /// Points used in graph for upload speeds.
        /// </summary>
        private static List<Point> uploadPoints;

        // Constant for tension in Bezier curves.
        private readonly double tension = 0.4;

        // Constant for initial number of speeds to have in speeds lists.
        private readonly int initNumSpeeds = 30;

        // Graph position constants
        private readonly int graphHeight = 80;
        private readonly int graphYMin = 184;
        private readonly int graphGradientMin = 194;
        private readonly int graphXMin = -1;
        private readonly int graphXMax = 329;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSpeedVisual"/> class.
        /// </summary>
        public NetworkSpeedVisual()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the download speeds list.
        /// </summary>
        public List<double> DownloadSpeeds
        {
            get
            {
                return downloadSpeeds;
            }

            set
            {
                downloadSpeeds = value;
            }
        }

        /// <summary>
        /// Gets or sets the upload speeds list.
        /// </summary>
        public List<double> UploadSpeeds
        {
            get
            {
                return uploadSpeeds;
            }

            set
            {
                uploadSpeeds = value;
            }
        }

        /// <summary>
        /// Gets or sets the download speeds list as a string.
        /// </summary>
        public string DownloadSpeedsString
        {
            get
            {
                return (string)GetValue(DownloadSpeedsStringProperty);
            }

            set
            {
                SetValue(DownloadSpeedsStringProperty, value);
                OnPropertyChanged("DownloadSpeedsString");
            }
        }

        /// <summary>
        /// Gets or sets the upload speeds list as a string.
        /// </summary>
        public string UploadSpeedsString
        {
            get
            {
                return (string)GetValue(UploadSpeedsStringProperty);
            }

            set
            {
                SetValue(UploadSpeedsStringProperty, value);
                OnPropertyChanged("UploadSpeedsString");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the graph is in view.
        /// </summary>
        public bool GraphShown
        {
            get
            {
                return (bool)GetValue(GraphShownProperty);
            }

            set
            {
                SetValue(GraphShownProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connection is stable.
        /// </summary>
        public bool IsStable
        {
            get
            {
                return (bool)GetValue(IsStableProperty);
            }

            set
            {
                SetValue(IsStableProperty, value);
            }
        }

        /// <summary>
        /// Make the curves.
        /// </summary>
        public void DrawCurves()
        {
            DownloadSpeeds = ConvertStringToList(DownloadSpeedsString);
            UploadSpeeds = ConvertStringToList(UploadSpeedsString);

            CreatePointsFromSpeeds();

            // Remove any previous curves.
            drawCan.Children.Clear();

            // Make paths from points.
            Path downloadPath = MakeCurve(downloadPoints);
            Path uploadPath = MakeCurve(uploadPoints);

            // Add upload path to canvas.
            if (uploadPath != null)
            {
                SetPathBrush(uploadPath, Color.FromRgb(226, 40, 80), Color.FromRgb(255, 164, 54));
                if (IsStable)
                {
                    SetPathFill(uploadPath, Color.FromArgb(51, 254, 117, 72), Color.FromArgb(0, 254, 116, 72));
                }

                drawCan.Children.Add(uploadPath);
            }

            // Add download path to canvas.
            if (downloadPath != null)
            {
                SetPathBrush(downloadPath, Color.FromRgb(157, 98, 252), Color.FromRgb(253, 50, 150));
                if (IsStable)
                {
                    SetPathFill(downloadPath, Color.FromArgb(51, 219, 67, 186), Color.FromArgb(0, 219, 67, 186));
                }

                drawCan.Children.Add(downloadPath);
            }
        }

        /// <summary>
        /// Reacts when the property of the element has been changed.
        /// </summary>
        /// <param name="propertyName">Name of the property which has been changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }

            if (propertyName == "DownloadSpeedsString")
            {
                DownloadSpeeds = ConvertStringToList(DownloadSpeedsString);
                if (GraphShown)
                {
                    DrawCurves();
                }
            }
            else if (propertyName == "UploadSpeedsString")
            {
                UploadSpeeds = ConvertStringToList(UploadSpeedsString);
                if (GraphShown)
                {
                    DrawCurves();
                }
            }
        }

        private static void OnDownloadSpeedsStringChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NetworkSpeedVisual n = sender as NetworkSpeedVisual;
            if (n != null)
            {
                n.OnPropertyChanged("DownloadSpeedsString");
            }
        }

        private static void OnUploadSpeedsStringChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NetworkSpeedVisual n = sender as NetworkSpeedVisual;
            if (n != null)
            {
                n.OnPropertyChanged("UploadSpeedsString");
            }
        }

        private static void OnGraphShownChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NetworkSpeedVisual n = sender as NetworkSpeedVisual;
            if (n != null)
            {
                n.OnPropertyChanged("GraphShown");
            }
        }

        private static void IsStableChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NetworkSpeedVisual n = sender as NetworkSpeedVisual;
            if (n != null)
            {
                n.OnPropertyChanged("IsStable");
            }
        }

        /// <summary>
        /// Initializes list of 0's for initial speeds.
        /// </summary>
        /// <returns>List of speeds.</returns>
        private List<double> GetInitialSpeedList()
        {
            List<double> speeds = new List<double>();

            for (int i = 0; i < initNumSpeeds; i++)
            {
                speeds.Add(0);
            }

            return speeds;
        }

        private List<double> ConvertStringToList(string stringList)
        {
            if (string.IsNullOrEmpty(stringList))
            {
                return GetInitialSpeedList();
            }

            List<double> newList = stringList.Split(',').Select(num => double.Parse(num)).ToList();
            return newList;
        }

        /// <summary>
        /// Creates points for upload and download graphs from lists of speeds.
        /// </summary>
        private void CreatePointsFromSpeeds()
        {
            List<double> mappedDownloadSpeeds = MapSpeedsToGraph(DownloadSpeeds);
            List<double> mappedUploadSpeeds = MapSpeedsToGraph(UploadSpeeds);

            downloadPoints = MakePoints(mappedDownloadSpeeds);
            uploadPoints = MakePoints(mappedUploadSpeeds);
        }

        /// <summary>
        /// Normalizes speeds to values between 0 and 1.
        /// The max speed will be 1, others are mapped proportionally.
        /// </summary>
        /// <param name="speeds">List of speeds to be normalized.</param>
        /// <returns>List of normalized speeds.</returns>
        private List<double> NormalizeSpeeds(List<double> speeds)
        {
            if (speeds == null)
            {
                return new List<double>();
            }

            double downloadMax = DownloadSpeeds != null && DownloadSpeeds.Count > 0 ? DownloadSpeeds.Max() : 0;
            double uploadMax = UploadSpeeds != null && UploadSpeeds.Count > 0 ? UploadSpeeds.Max() : 0;
            double max = Math.Max(downloadMax, uploadMax);

            if (max == 0)
            {
                return speeds;
            }

            List<double> normalizedSpeeds = new List<double>();

            foreach (double speed in speeds)
            {
                normalizedSpeeds.Add(speed / max);
            }

            return normalizedSpeeds;
        }

        /// <summary>
        /// Maps speeds to appropriate Y values for graph.
        /// </summary>
        /// <param name="speeds">List of speeds to be displayed on the graph.</param>
        /// <returns>List of Y values for graph.</returns>
        private List<double> MapSpeedsToGraph(List<double> speeds)
        {
            List<double> normalizedSpeeds = NormalizeSpeeds(speeds);
            List<double> mappedSpeeds = new List<double>();

            foreach (double speed in normalizedSpeeds)
            {
                mappedSpeeds.Add(graphYMin - (speed * graphHeight));
            }

            return mappedSpeeds;
        }

        /// <summary>
        /// Maps speed Y axis values to x axis values.
        /// </summary>
        /// <return>
        /// List of points for graph.
        /// </return>
        private List<Point> MakePoints(List<double> speeds)
        {
            if (speeds == null || speeds.Count < 2)
            {
                return new List<Point>();
            }

            List<Point> points = new List<Point>();

            double x = graphXMin;
            double inc = (double)(graphXMax - graphXMin) / (speeds.Count - 1);

            for (int i = 0; i < speeds.Count; i++)
            {
                points.Add(new Point(x, speeds[i]));
                x += inc;
            }

            return points;
        }

        /// <summary>
        /// Sets the Brush property of a path.
        /// </summary>
        /// <param name="path">Path to have the brush set.</param>
        /// <param name="startCol">Colour for start of gradient brush.</param>
        /// <param name="endCol">Colour for end of gradient brush.</param>
        private void SetPathBrush(Path path, Color startCol, Color endCol)
        {
            path.Stroke = new RadialGradientBrush(startCol, endCol);
            path.StrokeThickness = 2;
        }

        /// <summary>
        /// Sets the fill under the paths.
        /// </summary>
        /// <param name="path">Path to set fill under.</param>
        /// <param name="startCol">Colour for start of gradient fill.</param>
        /// <param name="endCol">Colour for end of gradient fill.</param>
        private void SetPathFill(Path path, Color startCol, Color endCol)
        {
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
            linearGradientBrush.StartPoint = new Point(0.5, 0);
            linearGradientBrush.EndPoint = new Point(0.5, 1);
            linearGradientBrush.GradientStops.Add(new GradientStop(startCol, 0.0));
            linearGradientBrush.GradientStops.Add(new GradientStop(endCol, 1));

            path.Fill = linearGradientBrush;
        }

        /// <summary>
        /// Make a Bezier curve connecting points.
        /// </summary>
        /// <param name="points">List of points to connect in graph.</param>
        /// <returns>Bezier Path connecting points.</returns>
        private Path MakeCurve(List<Point> points)
        {
            if (points == null || points.Count < 2)
            {
                return null;
            }

            List<Point> result_points = MakeCurvePoints(points);

            // Use the points to create the path.
            return MakeBezierPath(result_points);
        }

        /// <summary>
        ///  Make a list containing Bezier curve points and control points.
        /// </summary>
        /// <param name="points">List of points to be shown in graph.</param>
        /// <returns>List of curve points and control points for Bezier curve.</returns>
        private List<Point> MakeCurvePoints(List<Point> points)
        {
            if (points.Count < 2)
            {
                return null;
            }

            double control_scale = tension / 0.5 * 0.175;

            List<Point> result_points = new List<Point>();
            result_points.Add(points[0]);

            for (int i = 0; i < points.Count - 1; i++)
            {
                // Get the point and its neighbors.
                Point pt_before = points[Math.Max(i - 1, 0)];
                Point pt = points[i];
                Point pt_after = points[i + 1];
                Point pt_after2 = points[Math.Min(i + 2, points.Count - 1)];

                Point p4 = pt_after;

                double dx = pt_after.X - pt_before.X;
                double dy = pt_after.Y - pt_before.Y;
                Point p2 = new Point(
                    pt.X + (control_scale * dx),
                    Math.Min(pt.Y + (control_scale * dy), graphYMin));

                dx = pt_after2.X - pt.X;
                dy = pt_after2.Y - pt.Y;
                Point p3 = new Point(
                    pt_after.X - (control_scale * dx),
                    Math.Min(pt_after.Y - (control_scale * dy), graphYMin));

                // Save points p2, p3, and p4.
                result_points.Add(p2);
                result_points.Add(p3);
                result_points.Add(p4);
            }

            return result_points;
        }

        /// <summary>
        /// Make a Path holding a series of Bezier curves.
        /// <param name="curvePoints">Points to visit in curves and control points.</param>
        /// </summary>
        private Path MakeBezierPath(List<Point> curvePoints)
        {
            // Create a Path to hold the geometry.
            Path path = new Path();

            // Add a PathGeometry.
            PathGeometry path_geometry = new PathGeometry();
            path.Data = path_geometry;

            // Create a PathFigure.
            PathFigure path_figure = new PathFigure();
            path_geometry.Figures.Add(path_figure);

            // Start at the first point.
            path_figure.StartPoint = curvePoints[0];

            // Create a PathSegmentCollection.
            PathSegmentCollection path_segment_collection =
                new PathSegmentCollection();
            path_figure.Segments = path_segment_collection;

            // Add the rest of the points to a PointCollection.
            PointCollection point_collection =
                new PointCollection(curvePoints.Count - 1);
            for (int i = 1; i < curvePoints.Count; i++)
            {
                point_collection.Add(curvePoints[i]);
            }

            // Make a PolyBezierSegment from the points.
            PolyBezierSegment bezier_segment = new PolyBezierSegment();
            bezier_segment.Points = point_collection;

            // Add the PolyBezierSegment to other segment collection.
            path_segment_collection.Add(bezier_segment);

            // Make path figure for connecting graph below.
            PathFigure path_figure_connect = new PathFigure();
            path_geometry.Figures.Add(path_figure_connect);

            path_figure_connect.StartPoint = curvePoints[0];

            // Add points for lower connect path.
            PointCollection connectPoints = new PointCollection();
            connectPoints.Add(new Point(curvePoints[0].X, graphGradientMin));
            connectPoints.Add(new Point(curvePoints[curvePoints.Count - 1].X, graphGradientMin));
            connectPoints.Add(new Point(curvePoints[curvePoints.Count - 1].X, curvePoints[curvePoints.Count - 1].Y));

            PolyLineSegment polyLineSegment = new PolyLineSegment();
            polyLineSegment.Points = connectPoints;
            polyLineSegment.IsStroked = false;

            path_figure_connect.Segments.Add(polyLineSegment);

            return path;
        }
    }
}