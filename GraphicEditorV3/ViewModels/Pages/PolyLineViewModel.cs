using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicEditor.ViewModels.Pages
{
    public class PolyLineViewModel : ViewModelBase
    {
        public string Header { get; set; }
        public PolyLineViewModel(string name)
        {
            Header = name;
        }
    }
}
