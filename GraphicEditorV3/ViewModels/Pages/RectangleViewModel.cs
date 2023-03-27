using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicEditor.ViewModels.Pages
{
    public class RectangleViewModel : ViewModelBase
    {
        public string Header { get; set; }
        public RectangleViewModel(string name)
        {
            Header = name;
        }
    }
}
