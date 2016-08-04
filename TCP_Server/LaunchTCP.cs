using System;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;




[Transaction(TransactionMode.Manual)]
class LaunchTCP : IExternalCommand
{

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIRibbon.Server.LaunchServer(commandData);

        UIRibbon.TCPServerEnable(UIRibbon.Server.IsActive);
        
        return Result.Succeeded;
    }
}