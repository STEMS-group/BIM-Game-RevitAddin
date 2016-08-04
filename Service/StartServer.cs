using System;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;




[Transaction(TransactionMode.Manual)]
class StartServer : IExternalCommand
{

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        //UIRibbon.Server.StartServer(commandData);

        //UIRibbon.pushButtonStartServer.Enabled = !UIRibbon.Server.IsActive;
        //UIRibbon.pushButtonStopServer.Enabled = UIRibbon.Server.IsActive;
        UIRibbon.Service.StartService(commandData);

        UIRibbon.pushButtonStartServer.Enabled = !UIRibbon.Service.IsActive;
        UIRibbon.pushButtonStopServer.Enabled = UIRibbon.Service.IsActive;
        return Result.Succeeded;
    }
}

