using System;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;




[Transaction(TransactionMode.Manual)]
class ListenRequest : IExternalCommand
{

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIRibbon.Server.ListenRequest();

        return Result.Succeeded;
    }
}