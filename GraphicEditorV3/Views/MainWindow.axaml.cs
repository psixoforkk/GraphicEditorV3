using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using GraphicEditor.Models;
using GraphicEditor.ViewModels;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace GraphicEditor.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public void DeleteShape(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel mainWindowViewModel)
            {
                var but = this.GetVisualDescendants().OfType<ListBox>().Where(but => but.Name.Equals("listBoxShapes")).FirstOrDefault();
                mainWindowViewModel.GetSelectedItemIndex = but.SelectedIndex;
            }
        }
        public async void OpenJsonFileDialogButtonClick(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string[]? result = await openFileDialog.ShowAsync(this);
            if (DataContext is MainWindowViewModel mainWindowViewModel)
            {
                if (result != null)
                {
                    using (FileStream fs = new FileStream(result[0], FileMode.Open))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        string jsonText = Encoding.Default.GetString(buffer);
                        mainWindowViewModel.ShapesIn.Clear();
                        mainWindowViewModel.ShapesOut.Clear();
                        List<MyShapeModels> newList;
                        newList = JsonSerializer.Deserialize<List<MyShapeModels>>(jsonText)!;
                        foreach (MyShapeModels gger in newList)
                        {
                            mainWindowViewModel.ShapesOut.Add(gger);
                            TransformGroup newGroup = new TransformGroup();
                            RotateTransform newRotate = new RotateTransform();
                            ScaleTransform newScale = new ScaleTransform();
                            SkewTransform newSkew = new SkewTransform();
                            newSkew.AngleX = gger.skewX;
                            newSkew.AngleY = gger.skewY;
                            newScale.ScaleX = gger.scaleX;
                            newScale.ScaleY = gger.scaleY;
                            newRotate.CenterX = gger.rotX;
                            newRotate.CenterY = gger.rotY;
                            newRotate.Angle = gger.rotAngle;
                            newGroup.Children.Add(newRotate);
                            newGroup.Children.Add(newScale);
                            newGroup.Children.Add(newSkew);
                            if (gger.type == "line")
                            {
                                Line newShape = new Line
                                {
                                    RenderTransform = newGroup,
                                    StartPoint = Avalonia.Point.Parse(gger.stp),
                                    EndPoint = Avalonia.Point.Parse(gger.enp),
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    StrokeThickness = gger.strk,

                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                            if (gger.type == "rectangle")
                            {
                                Rectangle newShape = new Rectangle
                                {
                                    RenderTransform = newGroup,
                                    Margin = Avalonia.Thickness.Parse(gger.stp),
                                    Width = int.Parse(gger.rctheight),
                                    Height = int.Parse(gger.rctwidth),
                                    Fill = mainWindowViewModel.ListOfBrushes[gger.brsh1].Brush,
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    StrokeThickness = gger.strk
                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                            if (gger.type == "ellipse")
                            {
                                Ellipse newShape = new Ellipse
                                {
                                    RenderTransform = newGroup,
                                    Margin = Avalonia.Thickness.Parse(gger.stp),
                                    Width = int.Parse(gger.rctheight),
                                    Height = int.Parse(gger.rctwidth),
                                    Fill = mainWindowViewModel.ListOfBrushes[gger.brsh1].Brush,
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    StrokeThickness = gger.strk
                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                            if (gger.type == "pthshape")
                            {
                                Avalonia.Controls.Shapes.Path newShape = new Avalonia.Controls.Shapes.Path
                                {
                                    RenderTransform = newGroup,
                                    Data = Geometry.Parse(gger.pthdata),
                                    Fill = mainWindowViewModel.ListOfBrushes[gger.brsh1].Brush,
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    StrokeThickness = gger.strk
                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                            if (gger.type == "polyline")
                            {
                                List<Avalonia.Point> listOfPolyLinePoints = new List<Avalonia.Point>();
                                string[] words = gger.plylinetext.Split(' ');
                                foreach (string word in words)
                                {
                                    listOfPolyLinePoints.Add(Avalonia.Point.Parse(word));
                                }
                                Polyline newShape = new Polyline
                                {
                                    RenderTransform = newGroup,
                                    Points = listOfPolyLinePoints,
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    StrokeThickness = gger.strk
                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                            if (gger.type == "polygon")
                            {
                                List<Avalonia.Point> listOfPolyLinePoints = new List<Avalonia.Point>();
                                string[] words = gger.plylinetext.Split(' ');
                                foreach (string word in words)
                                {
                                    listOfPolyLinePoints.Add(Avalonia.Point.Parse(word));
                                }
                                Polyline newShape = new Polyline
                                {
                                    RenderTransform = newGroup,
                                    Points = listOfPolyLinePoints,
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    Fill = mainWindowViewModel.ListOfBrushes[gger.brsh1].Brush,
                                    StrokeThickness = gger.strk
                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                        }
                    }
                }
            }
        }
        public async void OpenXmlFileDialogButtonClick(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string[]? result = await openFileDialog.ShowAsync(this);
            if (DataContext is MainWindowViewModel mainWindowViewModel)
            {
                if (result != null)
                {
                    mainWindowViewModel.ShapesOut.Clear();
                    mainWindowViewModel.ShapesIn.Clear();
                    List<MyShapeModels> newList;
                    XmlSerializer xs = new XmlSerializer(typeof(List<MyShapeModels>));
                    using (XmlReader reader = XmlReader.Create(result[0]))
                    {
                        newList = (List<MyShapeModels>)xs.Deserialize(reader)!;
                        foreach (MyShapeModels gger in newList)
                        {
                            mainWindowViewModel.ShapesOut.Add(gger);
                            TransformGroup newGroup = new TransformGroup();
                            RotateTransform newRotate = new RotateTransform();
                            ScaleTransform newScale = new ScaleTransform();
                            SkewTransform newSkew = new SkewTransform();
                            newSkew.AngleX = gger.skewX;
                            newSkew.AngleY = gger.skewY;
                            newScale.ScaleX = gger.scaleX;
                            newScale.ScaleY = gger.scaleY;
                            newRotate.CenterX = gger.rotX;
                            newRotate.CenterY = gger.rotY;
                            newRotate.Angle = gger.rotAngle;
                            newGroup.Children.Add(newRotate);
                            newGroup.Children.Add(newScale);
                            newGroup.Children.Add(newSkew);
                            if (gger.type == "line")
                            {
                                Line newShape = new Line
                                {
                                    RenderTransform = newGroup,
                                    StartPoint = Avalonia.Point.Parse(gger.stp),
                                    EndPoint = Avalonia.Point.Parse(gger.enp),
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    StrokeThickness = gger.strk,

                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                            if (gger.type == "rectangle")
                            {
                                Rectangle newShape = new Rectangle
                                {
                                    RenderTransform = newGroup,
                                    Margin = Avalonia.Thickness.Parse(gger.stp),
                                    Width = int.Parse(gger.rctheight),
                                    Height = int.Parse(gger.rctwidth),
                                    Fill = mainWindowViewModel.ListOfBrushes[gger.brsh1].Brush,
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    StrokeThickness = gger.strk
                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                            if (gger.type == "ellipse")
                            {
                                Ellipse newShape = new Ellipse
                                {
                                    RenderTransform = newGroup,
                                    Margin = Avalonia.Thickness.Parse(gger.stp),
                                    Width = int.Parse(gger.rctheight),
                                    Height = int.Parse(gger.rctwidth),
                                    Fill = mainWindowViewModel.ListOfBrushes[gger.brsh1].Brush,
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    StrokeThickness = gger.strk
                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                            if (gger.type == "pthshape")
                            {
                                Avalonia.Controls.Shapes.Path newShape = new Avalonia.Controls.Shapes.Path
                                {
                                    RenderTransform = newGroup,
                                    Data = Geometry.Parse(gger.pthdata),
                                    Fill = mainWindowViewModel.ListOfBrushes[gger.brsh1].Brush,
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    StrokeThickness = gger.strk
                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                            if (gger.type == "polyline")
                            {
                                List<Avalonia.Point> listOfPolyLinePoints = new List<Avalonia.Point>();
                                string[] words = gger.plylinetext.Split(' ');
                                foreach (string word in words)
                                {
                                    listOfPolyLinePoints.Add(Avalonia.Point.Parse(word));
                                }
                                Polyline newShape = new Polyline
                                {
                                    RenderTransform = newGroup,
                                    Points = listOfPolyLinePoints,
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    StrokeThickness = gger.strk
                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                            if (gger.type == "polygon")
                            {
                                List<Avalonia.Point> listOfPolyLinePoints = new List<Avalonia.Point>();
                                string[] words = gger.plylinetext.Split(' ');
                                foreach (string word in words)
                                {
                                    listOfPolyLinePoints.Add(Avalonia.Point.Parse(word));
                                }
                                Polyline newShape = new Polyline
                                {
                                    RenderTransform = newGroup,
                                    Points = listOfPolyLinePoints,
                                    Stroke = mainWindowViewModel.ListOfBrushes[gger.brsh].Brush,
                                    Fill = mainWindowViewModel.ListOfBrushes[gger.brsh1].Brush,
                                    StrokeThickness = gger.strk
                                };
                                mainWindowViewModel.ShapesIn.Add(newShape);
                            }
                        }
                    }
                }
            }
        }
        public async void SaveXmlFileDialogButtonClick(object sender, RoutedEventArgs args)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExtension = ".XML";
            string? result = await saveFileDialog.ShowAsync(this);
            if (DataContext is MainWindowViewModel mainWindowViewModel)
            {
                if (result != null)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<MyShapeModels>));
                    using (XmlWriter writer = XmlWriter.Create(result))
                    {
                        xs.Serialize(writer, mainWindowViewModel.ShapesOut);
                    }
                }
            }
        }
        public async void SaveJsonFileDialogButtonClick(object sender, RoutedEventArgs args)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExtension = ".JSON";
            string? result = await saveFileDialog.ShowAsync(this);
            if (DataContext is MainWindowViewModel mainWindowViewModel)
            {
                if (result != null)
                {
                    using (FileStream fs = new FileStream(result, FileMode.OpenOrCreate))
                    {
                        string jsons = JsonSerializer.Serialize(mainWindowViewModel.ShapesOut);
                        byte[] buffer = Encoding.Default.GetBytes(jsons);
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }
        public async void SavePngFileDialogButtonClick(object sender, RoutedEventArgs args)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExtension = ".PNG";
            string? result = await saveFileDialog.ShowAsync(this);
            var canvas = this.GetVisualDescendants().OfType<Canvas>().Where(canvas => canvas.Name.Equals("canvas")).FirstOrDefault();
            if (DataContext is MainWindowViewModel mainWindowViewModel)
            {
                if (result != null)
                {
                    var pxsize = new PixelSize((int)canvas.Bounds.Width, (int)canvas.Bounds.Height);
                    var size = new Size(canvas.Bounds.Width, canvas.Bounds.Height);
                    using (RenderTargetBitmap bitmap = new RenderTargetBitmap(pxsize, new Avalonia.Vector(96, 96)))
                    {
                        canvas.Measure(size);
                        canvas.Arrange(new Rect(size));
                        bitmap.Render(canvas);
                        bitmap.Save(result);
                    }
                }
            }
        }
    }
}
