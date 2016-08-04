using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;


[Transaction(TransactionMode.ReadOnly)]
class GetModelInfo : Command
{
    public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        // The first argument, commandData, provides access to the top most object model. 
        // You will get the necessary information from commandData. 
        // To see what's in there, print out a few data accessed from commandData 
        // 
        // Exercise: Place a break point at commandData and drill down the data. 

        if (!ModelService.Service.IsActive)
        {
            TaskDialog.Show("Info","Server not launched");
            return Result.Succeeded;
        }


        BIM_Service.Info info = ModelService.Service.GetModelInfo();

        TaskDialog.Show(
          "Geting Model Info",
          "Version Name = " + info.name
          + "\nDocument Title = " + info.docTitle);
    
        return Result.Succeeded;
    }

    public BIM_Service.Info Get()
    {
        BIM_Service.Info info = new BIM_Service.Info();
        info.name = ModelService.Service.RvtApp.VersionName;
        info.docTitle = ModelService.Service.RvtDoc.Title;

        //string path = "C:\\Dev\\DISS\\UnityAddin\\DISS_Addin\\Assets\\Models";



        return info;
    }
}
