using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

[Transaction(TransactionMode.ReadOnly)]
class GetElementInfo : Command
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

        List<BIM_Service.ElementType> elementsToGet = new List<BIM_Service.ElementType>();
        elementsToGet.Add(BIM_Service.ElementType.Walls);
        elementsToGet.Add(BIM_Service.ElementType.Doors);

        Dictionary<BIM_Service.ElementType, List<BIM_Service.Element>> elementsList = ModelService.Service.GetElements(elementsToGet);

    
        
        return Result.Succeeded;

    }


    public BIM_Service.Element Get(string elementID)
    {
        try
        {
            Document doc;
            if (ModelService.Service.IsActive)
            {
                doc = ModelService.Service.RvtDoc;
            }
            else if (TCPServer.Server.IsActive)
            {
                doc = TCPServer.Server.RvtDoc;
            }
            else
            {
                throw new Exception("Server not running...");
            }

            Element element = doc.GetElement(new ElementId(Convert.ToInt32(elementID)));

            BIM_Service.Element result = new BIM_Service.Element();

            result.id = element.Id.IntegerValue;
            result.name = element.Name;
            result.type = IdentifyElement(element);
            result.category = element.Category.Name;
            result.obs = "";
            if (result.type == "Door")
            {
                ElementId elemTypeId = element.GetTypeId();
                ElementType elemType = (ElementType)doc.GetElement(elemTypeId);
                Parameter parameter = GetParameter(elemType, "Frame Material");
                result.obs = (parameter != null && ParameterToString(parameter) != "") ? "Contain Frame" : "No Frame";
            }

            return result;
        }
        catch (Exception e)
        {
            TaskDialog.Show("Ups...", e.Message);
        }

        return null;
    }

    public string IdentifyElement(Element elem)
    {

        // An instance of a system family has a designated class. 
        // You can use it identify the type of element. 
        // e.g., walls, floors, roofs. 
        // 
        string s = "";

        if (elem is Wall)
        {
            s = "Wall";
        }
        else if (elem is Floor)
        {
            s = "Floor";
        }
        else if (elem is RoofBase)
        {
            s = "Roof";
        }
        else if (elem is FamilyInstance)
        {
            // An instance of a component family is all FamilyInstance. 
            // We'll need to further check its category. 
            // e.g., Doors, Windows, Furnitures. 
            if (elem.Category.Id.IntegerValue ==
            (int)BuiltInCategory.OST_Doors)
            {
                s = "Door";
            }
            else if (elem.Category.Id.IntegerValue ==
            (int)BuiltInCategory.OST_Windows)
            {
                s = "Window";
            }
            else if (elem.Category.Id.IntegerValue ==
            (int)BuiltInCategory.OST_Furniture)
            {
                s = "Furniture";
            }
            else
            {
                // e.g. Plant 
                s = "Component family instance";
            }
        }
        else if (elem is HostObject)
        {
            // check the base class. e.g., CeilingAndFloor. 
            s = "System family instance";
        }
        else
        {
            s = "Other";
        }

        return s;
    }


    public Parameter GetParameter(Element elem, string parameter)
    {

        Dictionary<String, Parameter> dict = new Dictionary<String, Parameter>();

        IList<Parameter> paramSet = elem.GetOrderedParameters();

        dict = paramSet.ToDictionary(g => g.Definition.Name, g => g);

        return dict.ContainsKey(parameter) ? dict[parameter] : null;
    }

    public string GeometryElementToString(GeometryElement geomElem)
    {
        throw new NotImplementedException();
    }

    public FilteredElementCollector GetWithMultipleFilters()
    {
        List<ElementFilter> filters = new List<ElementFilter>();
        filters.Add(new ElementCategoryFilter(BuiltInCategory.OST_Walls));

        LogicalOrFilter filter = new LogicalOrFilter(filters);

        var elementCollector = new FilteredElementCollector(ModelService.Service.RvtDoc);

        var elements = elementCollector.WherePasses(filter);

        return elements;
    }

    public static string ParameterToString(Parameter param)
    {

        string val = "none";

        if (param == null)
        {
            return val;
        }

        // to get to the parameter value, we need to pause it depending
        // on its strage type 
        switch (param.StorageType)
        {
            case StorageType.Double:
                double dVal = param.AsDouble();
                val = dVal.ToString();
                break;

            case StorageType.Integer:
                int iVal = param.AsInteger();
                val = iVal.ToString();
                break;

            case StorageType.String:
                string sVal = param.AsString();
                val = sVal;
                break;

            case StorageType.ElementId:
                ElementId idVal = param.AsElementId();
                val = idVal.IntegerValue.ToString();
                break;

            case StorageType.None:
                break;

            default:
                break;
        }

        return val;
    }
}
