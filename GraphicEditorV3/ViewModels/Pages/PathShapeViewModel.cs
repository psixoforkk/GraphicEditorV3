using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicEditor.ViewModels.Pages
{
    public class PathShapeViewModel : ViewModelBase
    {
        public string Header { get; set; } 
        public PathShapeViewModel(string name)
        {
            Header = name;
        }
    }
}
