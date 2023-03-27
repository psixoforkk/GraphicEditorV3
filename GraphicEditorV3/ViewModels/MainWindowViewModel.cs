using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Utilities;
using GraphicEditor.Models;
using GraphicEditor.ViewModels.Pages;
using ReactiveUI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace GraphicEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string path;
        public ObservableCollection<Shape> ShapesIn { get; set; } = new ObservableCollection<Shape>();
        public ObservableCollection<MyShapeModels> ShapesOut { get; set; } = new ObservableCollection<MyShapeModels>();
        private object content;
        private ObservableCollection<ViewModelBase> viewModelCollection;
        public ObservableCollection<MyColor> ListOfBrushes { get; set; } = new ObservableCollection<MyColor>();
        public MyShapeModels selectedLBItem;
        public int getSelectedItemIndex;
        public ReactiveCommand<Unit, Unit> AddButton { get; }
        public ReactiveCommand<Unit, Unit> ResetCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteShape { get; }
        private bool selection_flag;
        private bool color_flag;
        private bool fcolor_flag;
        public string nameText;
        public string startPointText;
        public string endPointText;
        public string polyLineText;
        public string rectWidth;
        public string rectHeight;
        public string pathShapeText;
        public int numericUpDownText;
        public int getIndex;
        public int getFillIndex;
        public double rotateTransformAngleDegs;
        public string rotateTransformCenterXYs;
        public string scaleTransformXY;
        public double scaleTransformX { get; set; }
        public double scaleTransformY { get; set; }
        public string skewTransformXY;
        public double skewTransformX { get; set; }
        public double skewTransformY { get; set; }
        public double rotateTransformCenterY;
        public double rotateTransformCenterX;

        public MainWindowViewModel()
        {
            LoadColors();
            viewModelCollection = new ObservableCollection<ViewModelBase>();
            viewModelCollection.Add(new LineViewModel("Линия"));
            viewModelCollection.Add(new PolyLineViewModel("Ломаная линия"));
            viewModelCollection.Add(new PolygonViewModel("Многоугольник"));
            viewModelCollection.Add(new RectangleViewModel("Прямоугольник"));
            viewModelCollection.Add(new RectangleViewModel("Эллипс"));
            viewModelCollection.Add(new PathShapeViewModel("Составная фигура"));
            Content = viewModelCollection[0];
            AddButton = ReactiveCommand.Create(() =>
            {
                if (CheckName(NameText) == -1)
                {
                    if (Content == viewModelCollection[0]) LineAdd();
                    if (Content == viewModelCollection[1]) PolyLineAdd();
                    if (Content == viewModelCollection[2]) PolygonAdd();
                    if (Content == viewModelCollection[3]) RectangleAdd(true);
                    if (Content == viewModelCollection[4]) RectangleAdd(false);
                    if (Content == viewModelCollection[5]) PathAdd();
                }
                else
                {
                    int g = CheckName(NameText);
                    ShapesOut.RemoveAt(g);
                    ShapesIn.RemoveAt(g);
                    if (Content == viewModelCollection[0]) LineAdd();
                    if (Content == viewModelCollection[1]) PolyLineAdd();
                    if (Content == viewModelCollection[2]) PolygonAdd();
                    if (Content == viewModelCollection[3]) RectangleAdd(true);
                    if (Content == viewModelCollection[4]) RectangleAdd(false);
                    if (Content == viewModelCollection[5]) PathAdd();

                }
            });
            ResetCommand = ReactiveCommand.Create(() =>
            {
                NameText = "";
                StartPointText = "";
                RotateTransformAngleDegs = 0;
                RotateTransformCenterXYs = "0 0";
                SkewTransformXY = "0 0";
                ScaleTransformXY = "1 1";
                EndPointText = "";
                PolyLineText = "";
                RectHeight = "";
                RectWidth = "";
                PathShapeText = "";
                GetFillIndex = 1;
                GetIndex = 1;
                NumericUpDownText = 1;
            });
        }
        private int CheckName(string name)
        {
            for (int i = 0; i < ShapesOut.Count; i++)
            {
                if (ShapesOut[i].shapeName == name)
                {
                    return i;
                }
            }
            return -1;
        }
        private void PathAdd()
        {
            MyShapeModels newestShape = new MyShapeModels(NameText);
            newestShape.strk = NumericUpDownText;
            newestShape.pthdata = PathShapeText;
            newestShape.brsh = GetIndex;
            newestShape.brsh1 = GetFillIndex;
            newestShape.type = "pthshape";
            TransformGroup newGroup = new TransformGroup();
            RotateTransform newRotate = new RotateTransform();
            ScaleTransform newScale = new ScaleTransform();
            SkewTransform newSkew = new SkewTransform();
            newRotate.Angle = RotateTransformAngleDegs;
            string[] words = rotateTransformCenterXYs.Split(' ');
            rotateTransformCenterX = double.Parse(words[0]);
            rotateTransformCenterY = double.Parse(words[1]);
            words = scaleTransformXY.Split(' ');
            scaleTransformX = double.Parse(words[0]);
            scaleTransformY = double.Parse(words[1]);
            words = skewTransformXY.Split(' ');
            skewTransformX = double.Parse(words[0]);
            skewTransformY = double.Parse(words[1]);
            newSkew.AngleX = skewTransformX;
            newSkew.AngleY = skewTransformY;
            newScale.ScaleX = scaleTransformX;
            newScale.ScaleY = scaleTransformY;
            newRotate.CenterX = RotateTransformCenterX;
            newRotate.CenterY = RotateTransformCenterY;
            newGroup.Children.Add(newRotate);
            newGroup.Children.Add(newScale);
            newGroup.Children.Add(newSkew);
            newestShape.rotAngle = rotateTransformAngleDegs;
            newestShape.rotX = rotateTransformCenterX;
            newestShape.rotY = rotateTransformCenterY;
            newestShape.scaleX = scaleTransformX;
            newestShape.scaleY = scaleTransformY;
            newestShape.skewX = skewTransformX;
            newestShape.skewY = skewTransformY;
            Avalonia.Controls.Shapes.Path newShape = new Avalonia.Controls.Shapes.Path
            {

                RenderTransform = newGroup,
                Data = Geometry.Parse(newestShape.pthdata),
                Fill = ListOfBrushes[newestShape.brsh1].Brush,
                Stroke = ListOfBrushes[newestShape.brsh].Brush,
                StrokeThickness = newestShape.strk
            };
            ShapesIn.Add(newShape);
            ShapesOut.Add(newestShape);
        }
        private void RectangleAdd(bool flag)
        {
            MyShapeModels newestShape = new MyShapeModels(NameText);
            newestShape.stp = StartPointText;
            newestShape.rctheight = RectHeight;
            newestShape.rctwidth = RectWidth;
            newestShape.strk = NumericUpDownText;
            newestShape.brsh = GetIndex;
            newestShape.brsh1 = GetFillIndex;
            TransformGroup newGroup = new TransformGroup();
            RotateTransform newRotate = new RotateTransform();
            ScaleTransform newScale = new ScaleTransform();
            SkewTransform newSkew = new SkewTransform();
            newRotate.Angle = RotateTransformAngleDegs;
            string[] words = rotateTransformCenterXYs.Split(' ');
            rotateTransformCenterX = double.Parse(words[0]);
            rotateTransformCenterY = double.Parse(words[1]);
            words = scaleTransformXY.Split(' ');
            scaleTransformX = double.Parse(words[0]);
            scaleTransformY = double.Parse(words[1]);
            words = skewTransformXY.Split(' ');
            skewTransformX = double.Parse(words[0]);
            skewTransformY = double.Parse(words[1]);
            newSkew.AngleX = skewTransformX;
            newSkew.AngleY = skewTransformY;
            newScale.ScaleX = scaleTransformX;
            newScale.ScaleY = scaleTransformY;
            newRotate.CenterX = RotateTransformCenterX;
            newRotate.CenterY= RotateTransformCenterY;
            newGroup.Children.Add(newRotate);
            newGroup.Children.Add(newScale);
            newGroup.Children.Add(newSkew);
            newestShape.rotAngle = rotateTransformAngleDegs;
            newestShape.rotX = rotateTransformCenterX;
            newestShape.rotY = rotateTransformCenterY;
            newestShape.scaleX = scaleTransformX;
            newestShape.scaleY = scaleTransformY;
            newestShape.skewX = skewTransformX;
            newestShape.skewY = skewTransformY;
            if (flag)
            {
                newestShape.type = "rectangle";
                Rectangle newShape = new Rectangle
                {
                    RenderTransform = newGroup,
                    Margin = Avalonia.Thickness.Parse(newestShape.stp),
                    Width = int.Parse(newestShape.rctheight),
                    Height = int.Parse(newestShape.rctwidth),
                    Fill = ListOfBrushes[newestShape.brsh1].Brush,
                    Stroke = ListOfBrushes[newestShape.brsh].Brush,
                    StrokeThickness = newestShape.strk
                };
                ShapesIn.Add(newShape);
            }
            else
            {
                newestShape.type = "ellipse";
                Ellipse newShape = new Ellipse
                {
                    RenderTransform = newGroup,
                    Margin = Avalonia.Thickness.Parse(newestShape.stp),
                    Width = int.Parse(newestShape.rctheight),
                    Height = int.Parse(newestShape.rctwidth),
                    Fill = ListOfBrushes[newestShape.brsh1].Brush,
                    Stroke = ListOfBrushes[newestShape.brsh].Brush,
                    StrokeThickness = newestShape.strk
                };
                ShapesIn.Add(newShape);
            }
            ShapesOut.Add(newestShape);
        }
        private void PolygonAdd()
        {
            List<Avalonia.Point> listOfPolyLinePoints = new List<Avalonia.Point>();
            MyShapeModels newestShape = new MyShapeModels(NameText);
            newestShape.plylinetext = polyLineText;
            newestShape.strk = NumericUpDownText;
            newestShape.brsh = GetIndex;
            newestShape.brsh1 = GetFillIndex;
            newestShape.type = "polygon";
            string[] words1 = newestShape.plylinetext.Split(' ');
            TransformGroup newGroup = new TransformGroup();
            RotateTransform newRotate = new RotateTransform();
            ScaleTransform newScale = new ScaleTransform();
            SkewTransform newSkew = new SkewTransform();
            newRotate.Angle = RotateTransformAngleDegs;
            string[] words = rotateTransformCenterXYs.Split(' ');
            rotateTransformCenterX = double.Parse(words[0]);
            rotateTransformCenterY = double.Parse(words[1]);
            words = scaleTransformXY.Split(' ');
            scaleTransformX = double.Parse(words[0]);
            scaleTransformY = double.Parse(words[1]);
            words = skewTransformXY.Split(' ');
            skewTransformX = double.Parse(words[0]);
            skewTransformY = double.Parse(words[1]);
            newSkew.AngleX = skewTransformX;
            newSkew.AngleY = skewTransformY;
            newScale.ScaleX = scaleTransformX;
            newScale.ScaleY = scaleTransformY;
            newRotate.CenterX = RotateTransformCenterX;
            newRotate.CenterY = RotateTransformCenterY;
            newGroup.Children.Add(newRotate);
            newGroup.Children.Add(newScale);
            newGroup.Children.Add(newSkew);
            newestShape.rotAngle = rotateTransformAngleDegs;
            newestShape.rotX = rotateTransformCenterX;
            newestShape.rotY = rotateTransformCenterY;
            newestShape.scaleX = scaleTransformX;
            newestShape.scaleY = scaleTransformY;
            newestShape.skewX = skewTransformX;
            newestShape.skewY = skewTransformY;
            foreach (string word in words1)
            {
                listOfPolyLinePoints.Add(Avalonia.Point.Parse(word));
            }
            Polygon newShape = new Polygon
            {
                RenderTransform = newGroup,
                Points = listOfPolyLinePoints,
                Fill = ListOfBrushes[newestShape.brsh1].Brush,
                Stroke = ListOfBrushes[newestShape.brsh].Brush,
                StrokeThickness = newestShape.strk
            };
            ShapesIn.Add(newShape);
            ShapesOut.Add(newestShape);
        }
        private void PolyLineAdd()
        {
            List<Avalonia.Point> listOfPolyLinePoints = new List<Avalonia.Point>();
            MyShapeModels newestShape = new MyShapeModels(NameText);
            newestShape.plylinetext = polyLineText;
            newestShape.strk = NumericUpDownText;
            newestShape.brsh = GetIndex;
            newestShape.type = "polyline";
            TransformGroup newGroup = new TransformGroup();
            RotateTransform newRotate = new RotateTransform();
            ScaleTransform newScale = new ScaleTransform();
            SkewTransform newSkew = new SkewTransform();
            newRotate.Angle = RotateTransformAngleDegs;
            string[] words = rotateTransformCenterXYs.Split(' ');
            rotateTransformCenterX = double.Parse(words[0]);
            rotateTransformCenterY = double.Parse(words[1]);
            words = scaleTransformXY.Split(' ');
            scaleTransformX = double.Parse(words[0]);
            scaleTransformY = double.Parse(words[1]);
            words = skewTransformXY.Split(' ');
            skewTransformX = double.Parse(words[0]);
            skewTransformY = double.Parse(words[1]);
            newSkew.AngleX = skewTransformX;
            newSkew.AngleY = skewTransformY;
            newScale.ScaleX = scaleTransformX;
            newScale.ScaleY = scaleTransformY;
            newRotate.CenterX = RotateTransformCenterX;
            newRotate.CenterY = RotateTransformCenterY;
            newGroup.Children.Add(newRotate);
            newGroup.Children.Add(newScale);
            newGroup.Children.Add(newSkew);
            newestShape.rotAngle = rotateTransformAngleDegs;
            newestShape.rotX = rotateTransformCenterX;
            newestShape.rotY = rotateTransformCenterY;
            newestShape.scaleX = scaleTransformX;
            newestShape.scaleY = scaleTransformY;
            newestShape.skewX = skewTransformX;
            newestShape.skewY = skewTransformY;
            string[] words1 = newestShape.plylinetext.Split(' ');
            foreach (string word in words1)
            {
                listOfPolyLinePoints.Add(Avalonia.Point.Parse(word));
            }
            Polyline newShape = new Polyline
            {
                RenderTransform = newGroup,
                Points = listOfPolyLinePoints,
                Stroke = ListOfBrushes[newestShape.brsh].Brush,
                StrokeThickness = newestShape.strk
            };
            ShapesIn.Add(newShape);
            ShapesOut.Add(newestShape);
        }
        private void LineAdd()
        {
            MyShapeModels newestShape = new MyShapeModels(NameText);
            newestShape.stp = StartPointText;
            newestShape.enp = EndPointText;
            newestShape.brsh = GetIndex;
            newestShape.strk = NumericUpDownText;
            newestShape.type = "line";
            TransformGroup newGroup = new TransformGroup();
            RotateTransform newRotate = new RotateTransform();
            ScaleTransform newScale = new ScaleTransform();
            SkewTransform newSkew = new SkewTransform();
            newRotate.Angle = RotateTransformAngleDegs;
            string[] words = rotateTransformCenterXYs.Split(' ');
            rotateTransformCenterX = double.Parse(words[0]);
            rotateTransformCenterY = double.Parse(words[1]);
            words = scaleTransformXY.Split(' ');
            scaleTransformX = double.Parse(words[0]);
            scaleTransformY = double.Parse(words[1]);
            words = skewTransformXY.Split(' ');
            skewTransformX = double.Parse(words[0]);
            skewTransformY = double.Parse(words[1]);
            newSkew.AngleX = skewTransformX;
            newSkew.AngleY = skewTransformY;
            newScale.ScaleX = scaleTransformX;
            newScale.ScaleY = scaleTransformY;
            newRotate.CenterX = RotateTransformCenterX;
            newRotate.CenterY = RotateTransformCenterY;
            newGroup.Children.Add(newRotate);
            newGroup.Children.Add(newScale);
            newGroup.Children.Add(newSkew);
            newestShape.rotAngle = rotateTransformAngleDegs;
            newestShape.rotX = rotateTransformCenterX;
            newestShape.rotY = rotateTransformCenterY;
            newestShape.scaleX = scaleTransformX;
            newestShape.scaleY = scaleTransformY;
            newestShape.skewX = skewTransformX;
            newestShape.skewY = skewTransformY;
            Line newShape = new Line
            {
                RenderTransform = newGroup,
                StartPoint = Avalonia.Point.Parse(newestShape.stp),
                EndPoint = Avalonia.Point.Parse(newestShape.enp),
                Stroke = ListOfBrushes[newestShape.brsh].Brush,
                StrokeThickness = newestShape.strk,

            };
            ShapesIn.Add(newShape);
            ShapesOut.Add(newestShape);
        }
        public void LoadColors()
        {
            PropertyInfo[] colorProps = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (PropertyInfo colorProp in colorProps)
            {
                if (colorProp.PropertyType == typeof(Color))
                {
                    Color color = (Color)colorProp.GetValue(null, null);
                    string colorName = colorProp.Name;
                    SolidColorBrush brush = new SolidColorBrush(color);
                    MyColor item = new MyColor() { Brush = brush, Name = colorName };
                    ListOfBrushes.Add(item);
                }
            }
        }
        public string Path
        {
            get { return path; }
            set { this.RaiseAndSetIfChanged(ref path, value); }
        }
        public int GetSelectedItemIndex
        {
            get { return getSelectedItemIndex; }
            set 
            {
                this.RaiseAndSetIfChanged(ref getSelectedItemIndex, value);
                if (selectedLBItem != null)
                {
                    ShapesOut.RemoveAt(getSelectedItemIndex);
                    ShapesIn.RemoveAt(getSelectedItemIndex);
                }
            }
        }
        public MyShapeModels SelectedLBItem
        {
            get { return selectedLBItem; }
            set 
            {
                this.RaiseAndSetIfChanged(ref selectedLBItem, value);
                if (SelectedLBItem != null)
                {
                    if (SelectedLBItem.type == "line")
                    {
                        Content = viewModelCollection[0];
                        NameText = SelectedLBItem.shapeName;
                        StartPointText = SelectedLBItem.stp;
                        EndPointText = SelectedLBItem.enp;
                        NumericUpDownText = SelectedLBItem.strk;
                        color_flag = false;
                        GetIndex = SelectedLBItem.brsh;
                        RotateTransformAngleDegs = SelectedLBItem.rotAngle;
                        RotateTransformCenterXYs = SelectedLBItem.rotX + " " + SelectedLBItem.rotY;
                        ScaleTransformXY = SelectedLBItem.scaleX + " " + SelectedLBItem.scaleY;
                        SkewTransformXY = SelectedLBItem.skewX + " " + SelectedLBItem.skewY;
                    }
                    if (SelectedLBItem.type == "polyline")
                    {
                        Content = viewModelCollection[1];
                        NameText = SelectedLBItem.shapeName;
                        PolyLineText = SelectedLBItem.plylinetext;
                        NumericUpDownText = SelectedLBItem.strk;
                        GetIndex = SelectedLBItem.brsh;
                        RotateTransformAngleDegs = SelectedLBItem.rotAngle;
                        RotateTransformCenterXYs = SelectedLBItem.rotX + " " + SelectedLBItem.rotY;
                        ScaleTransformXY = SelectedLBItem.scaleX + " " + SelectedLBItem.scaleY;
                        SkewTransformXY = SelectedLBItem.skewX + " " + SelectedLBItem.skewY;

                    }
                    if (SelectedLBItem.type == "polygon")
                    { 
                        Content = viewModelCollection[2];
                        NameText = SelectedLBItem.shapeName;
                        PolyLineText = SelectedLBItem.plylinetext;
                        NumericUpDownText = SelectedLBItem.strk;
                        color_flag = false;
                        fcolor_flag = false;
                        GetIndex = SelectedLBItem.brsh;
                        GetFillIndex = SelectedLBItem.brsh1;
                        RotateTransformAngleDegs = SelectedLBItem.rotAngle;
                        RotateTransformCenterXYs = SelectedLBItem.rotX + " " + SelectedLBItem.rotY;
                        ScaleTransformXY = SelectedLBItem.scaleX + " " + SelectedLBItem.scaleY;
                        SkewTransformXY = SelectedLBItem.skewX + " " + SelectedLBItem.skewY;
                    }
                    if (SelectedLBItem.type == "rectangle")
                    { 
                        Content = viewModelCollection[3];
                        NameText = SelectedLBItem.shapeName;
                        NumericUpDownText = SelectedLBItem.strk;
                        StartPointText = SelectedLBItem.stp;
                        color_flag = false;
                        fcolor_flag = false;
                        GetIndex = SelectedLBItem.brsh;
                        GetFillIndex = SelectedLBItem.brsh1;
                        RectWidth = SelectedLBItem.rctwidth;
                        RectHeight = SelectedLBItem.rctheight;
                        RotateTransformAngleDegs = SelectedLBItem.rotAngle;
                        RotateTransformCenterXYs = SelectedLBItem.rotX + " " + SelectedLBItem.rotY;
                        ScaleTransformXY = SelectedLBItem.scaleX + " " + SelectedLBItem.scaleY;
                        SkewTransformXY = SelectedLBItem.skewX + " " + SelectedLBItem.skewY;
                    }

                    if (SelectedLBItem.type == "ellipse")
                    {
                        Content = viewModelCollection[4];
                        NameText = SelectedLBItem.shapeName;
                        NumericUpDownText = SelectedLBItem.strk;
                        StartPointText = SelectedLBItem.stp;
                        color_flag = false;
                        fcolor_flag = false;
                        GetIndex = SelectedLBItem.brsh;
                        GetFillIndex = SelectedLBItem.brsh1;
                        RectWidth = SelectedLBItem.rctwidth;
                        RectHeight = SelectedLBItem.rctheight;
                        RotateTransformAngleDegs = SelectedLBItem.rotAngle;
                        RotateTransformCenterXYs = SelectedLBItem.rotX + " " + SelectedLBItem.rotY;
                        ScaleTransformXY = SelectedLBItem.scaleX + " " + SelectedLBItem.scaleY;
                        SkewTransformXY = SelectedLBItem.skewX + " " + SelectedLBItem.skewY;
                    }
                    if (SelectedLBItem.type == "pthshape")
                    {
                        Content = viewModelCollection[5];
                        NameText = SelectedLBItem.shapeName;
                        NumericUpDownText = SelectedLBItem.strk;
                        color_flag = false;
                        fcolor_flag = false;
                        GetIndex = SelectedLBItem.brsh;
                        GetFillIndex = SelectedLBItem.brsh1;
                        PathShapeText = SelectedLBItem.pthdata;
                        RotateTransformAngleDegs = SelectedLBItem.rotAngle;
                        RotateTransformCenterXYs = SelectedLBItem.rotX + " " + SelectedLBItem.rotY;
                        ScaleTransformXY = SelectedLBItem.scaleX + " " + SelectedLBItem.scaleY;
                        SkewTransformXY = SelectedLBItem.skewX + " " + SelectedLBItem.skewY;
                    }
                }
            }
        }
        public string ScaleTransformXY
        {
            get { return scaleTransformXY; }
            set { this.RaiseAndSetIfChanged(ref scaleTransformXY, value); }
        }
        public string SkewTransformXY
        {
            get { return skewTransformXY; }
            set { this.RaiseAndSetIfChanged(ref skewTransformXY, value); }
        }
        public double RotateTransformCenterY
        {
            get { return rotateTransformCenterY; }
            set
            {
                this.RaiseAndSetIfChanged(ref rotateTransformCenterY, value);
            }
        }
        public double RotateTransformCenterX
        {
            get { return rotateTransformCenterX; }
            set
            {
                this.RaiseAndSetIfChanged(ref rotateTransformCenterX, value);
            }
        }
        public string RotateTransformCenterXYs
        {
            get { return rotateTransformCenterXYs;  }
            set { this.RaiseAndSetIfChanged(ref rotateTransformCenterXYs, value); }
        }
        public double RotateTransformAngleDegs
        {
            get { return rotateTransformAngleDegs; }
            set { this.RaiseAndSetIfChanged(ref rotateTransformAngleDegs, value); }
        }
        public string PathShapeText
        {
            get { return pathShapeText; }
            set { this.RaiseAndSetIfChanged(ref pathShapeText, value); }
        }
        public string RectWidth
        {
            get { return rectWidth; }
            set { this.RaiseAndSetIfChanged(ref rectWidth, value); }
        }  
        public string RectHeight
        {
            get { return rectHeight; }
            set { this.RaiseAndSetIfChanged(ref rectHeight, value); }
        }
        public string PolyLineText
        {
            get { return polyLineText; }
            set { this.RaiseAndSetIfChanged(ref polyLineText, value); }
        }
        public string NameText
        {
            get { return nameText; }
            set { this.RaiseAndSetIfChanged(ref nameText, value); }
        }
        public string StartPointText
        {
            get { return startPointText; }
            set { this.RaiseAndSetIfChanged(ref startPointText, value); }
        }
        public string EndPointText
        {
            get { return endPointText; }
            set { this.RaiseAndSetIfChanged(ref endPointText, value); }
        }
        public int NumericUpDownText
        {
            get { return numericUpDownText; }
            set { this.RaiseAndSetIfChanged(ref numericUpDownText, value); }
        }
        public int GetFillIndex
        {
            get
            {
                if (fcolor_flag == true)
                {
                    getFillIndex = 1;
                    fcolor_flag = false;
                }
                return getFillIndex;
            }
            set { this.RaiseAndSetIfChanged(ref getFillIndex, value); }
        }
        public int GetIndex
        {
            get
            {
                if (color_flag == true)
                {
                    getIndex = 1;
                    color_flag = false;
                }
                return getIndex;
            }
            set { this.RaiseAndSetIfChanged(ref getIndex, value); }
        }
        public object Content
        {
            get 
            {
                return content; 
            }
            set
            {
                NameText = "";
                StartPointText = "";
                EndPointText = "";
                PolyLineText = "";
                RectHeight = "";
                RectWidth = "";
                PathShapeText = "";
                RotateTransformAngleDegs = 0;
                RotateTransformCenterXYs = "0 0";
                SkewTransformXY = "0 0";
                ScaleTransformXY = "1 1";
                color_flag = true;
                fcolor_flag = true;
                NumericUpDownText = 1;
                this.RaiseAndSetIfChanged(ref content, value); 
            }
        }
        public ObservableCollection<ViewModelBase> ViewModelCollection
        {
            get { return viewModelCollection; }
            set { this.RaiseAndSetIfChanged(ref viewModelCollection, value); }
        }
    }
}
