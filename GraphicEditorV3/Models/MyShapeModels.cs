using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia;

namespace GraphicEditor.Models
{
    public class MyShapeModels
    {
        public string? shapeName { get; set; }
        public MyShapeModels(string name)
        {
            shapeName = name;
        }
        public MyShapeModels()
        { 

        }
        public string plylinetext { get; set; }
        public string pthdata { get; set; }
        public int brsh1 { get; set; }
        public string rctheight { get; set; }
        public string rctwidth { get; set; }
        public string type { get; set; }
        public string stp { get; set; }
        public string enp { get; set; }
        public int brsh { get; set; }
        public int strk { get; set; }
        public double rotAngle { get; set; }
        public double rotX { get; set; }
        public double rotY { get; set; }
        public double scaleX { get; set; }
        public double scaleY { get; set; }
        public double skewX { get; set; }
        public double skewY { get; set; }
    }
}
