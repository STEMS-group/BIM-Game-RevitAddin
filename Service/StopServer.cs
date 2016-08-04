using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;




[Transaction(TransactionMode.Manual)]
class StopServer : IExternalCommand
{

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        //UIRibbon.Server.StopServer();
        //UIRibbon.pushButtonStartServer.Enabled = !UIRibbon.Server.IsActive;
        //UIRibbon.pushButtonStopServer.Enabled = UIRibbon.Server.IsActive;

        UIRibbon.Service.StopService();
        UIRibbon.pushButtonStartServer.Enabled = !UIRibbon.Service.IsActive;
        UIRibbon.pushButtonStopServer.Enabled = UIRibbon.Service.IsActive;

        return Result.Succeeded;
    }
}

