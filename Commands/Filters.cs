using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

class Filters : ISelectionFilter
{
    public bool AllowElement(Element elem)
    {
        throw new NotImplementedException();
    }

    public bool AllowReference(Reference reference, XYZ position)
    {
        throw new NotImplementedException();
    }
}
