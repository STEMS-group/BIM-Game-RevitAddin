using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using BIM_Service;

[Transaction(TransactionMode.ReadOnly)]
class GetElements : Command
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

        
        foreach (BIM_Service.ElementType type in elementsList.Keys)
        {
            string result = "";
            foreach (BIM_Service.Element el in elementsList[type])
            {
                if (el is BIM_Service.ServiceWall)
                {
                    ServiceWall w = el as ServiceWall;
                    result += "Name: " + w.name + " - ID: " + w.id + "\n";
                    result += "\tGeometry: " + w.geometry + " - ID: " + w.position + "\n";
                }
                else
                {
                    result += "Name: " + el.name + " - ID: " + el.name + "\n";
                }

            }
            TaskDialog.Show("Element List", "List of  Elements:\n" + result);
        }
        
        return Result.Succeeded;

    }


    public List<BIM_Service.Element> Get(BIM_Service.ElementType type)
    {
        List<BIM_Service.Element> list = new List<BIM_Service.Element>();
        switch (type)
        {
            case BIM_Service.ElementType.Walls:
                list = GetWalls();
                break;
            case BIM_Service.ElementType.Floors:
                list = GetFloors();
                break;
            case BIM_Service.ElementType.Roofs:
                list = GetRoofs();
                break;
            case BIM_Service.ElementType.Doors:
                list = GetDoors();
                break;
            case BIM_Service.ElementType.Windows:
                list = GetWindows();
                break;
            default:
                TaskDialog.Show("Work in progress", "Function not implemented yet");
                break;
        }

        return list;
    }

    private List<BIM_Service.Element> GetWalls()
    {
        try
        {
            var elementCollector = new FilteredElementCollector(ModelService.Service.RvtDoc);

            var walls = elementCollector.OfCategory(BuiltInCategory.OST_Walls);

            List<BIM_Service.Element> serviceList = new List<BIM_Service.Element>();

            foreach (Autodesk.Revit.DB.Element el in walls)
            {
                // Set a geometry option 
                Options opt = ModelService.Service.RvtApp.Create.NewGeometryOptions();
                opt.DetailLevel = ViewDetailLevel.Fine;

                // Get the geometry from the element 
                GeometryElement geomElem = el.get_Geometry(opt);

                ServiceWall w = new ServiceWall();
                w.id = el.Id.IntegerValue;
                w.name = el.Name;
                w.category = el.Category.Name;

                w.geometry = "geometry info of the element";
                w.position = "position of the element";

                serviceList.Add(w);
            }
            return serviceList;
        }
        catch (Exception e)
        {
            TaskDialog.Show("Error", e.Message);
        }
        return null;
    }

    private List<BIM_Service.Element> GetFloors()
    {
        try
        {
            var elementCollector = new FilteredElementCollector(ModelService.Service.RvtDoc);

            var floors = elementCollector.OfCategory(BuiltInCategory.OST_Floors);

            List<BIM_Service.Element> serviceList = new List<BIM_Service.Element>();

            foreach (Autodesk.Revit.DB.Element el in floors)
            {
                // Set a geometry option 
                Options opt = ModelService.Service.RvtApp.Create.NewGeometryOptions();
                opt.DetailLevel = ViewDetailLevel.Fine;

                // Get the geometry from the element 
                GeometryElement geomElem = el.get_Geometry(opt);

                ServiceWall w = new ServiceWall();
                w.id = el.Id.IntegerValue;
                w.name = el.Name;
                w.category = el.Category.Name;

                w.geometry = "geometry info of the element";
                w.position = "position of the element";

                serviceList.Add(w);
            }
            return serviceList;
        }
        catch (Exception e)
        {
            TaskDialog.Show("Error", e.Message);
        }
        return null;
    }

    private List<BIM_Service.Element> GetRoofs()
    {
        try
        {
            var elementCollector = new FilteredElementCollector(ModelService.Service.RvtDoc);

            var roofs = elementCollector.OfCategory(BuiltInCategory.OST_Roofs);

            List<BIM_Service.Element> serviceList = new List<BIM_Service.Element>();

            foreach (Autodesk.Revit.DB.Element el in roofs)
            {
                // Set a geometry option 
                Options opt = ModelService.Service.RvtApp.Create.NewGeometryOptions();
                opt.DetailLevel = ViewDetailLevel.Fine;

                // Get the geometry from the element 
                GeometryElement geomElem = el.get_Geometry(opt);

                ServiceWall w = new ServiceWall();
                w.id = el.Id.IntegerValue;
                w.name = el.Name;
                w.category = el.Category.Name;

                w.geometry = "geometry info of the element";
                w.position = "position of the element";

                serviceList.Add(w);
            }
            return serviceList;
        }
        catch (Exception e)
        {
            TaskDialog.Show("Error", e.Message);
        }
        return null;
    }

    private List<BIM_Service.Element> GetDoors()
    {
        try
        {
            var elementCollector = new FilteredElementCollector(ModelService.Service.RvtDoc);

            var doors = elementCollector.OfCategory(BuiltInCategory.OST_Doors);

            List<BIM_Service.Element> serviceList = new List<BIM_Service.Element>();

            foreach (Autodesk.Revit.DB.Element el in doors)
            {
                // Set a geometry option 
                Options opt = ModelService.Service.RvtApp.Create.NewGeometryOptions();
                opt.DetailLevel = ViewDetailLevel.Fine;

                // Get the geometry from the element 
                GeometryElement geomElem = el.get_Geometry(opt);

                ServiceWall w = new ServiceWall();
                w.id = el.Id.IntegerValue;
                w.name = el.Name;
                w.category = el.Category.Name;

                w.geometry = "geometry info of the element";
                w.position = "position of the element";

                serviceList.Add(w);
            }
            return serviceList;
        }
        catch (Exception e)
        {
            TaskDialog.Show("Error", e.Message);
        }
        return null;
    }

    private List<BIM_Service.Element> GetWindows()
    {
        try
        {
            var elementCollector = new FilteredElementCollector(ModelService.Service.RvtDoc);

            var windows = elementCollector.OfCategory(BuiltInCategory.OST_Windows);

            List<BIM_Service.Element> serviceList = new List<BIM_Service.Element>();

            foreach (Autodesk.Revit.DB.Element el in windows)
            {
                // Set a geometry option 
                Options opt = ModelService.Service.RvtApp.Create.NewGeometryOptions();
                opt.DetailLevel = ViewDetailLevel.Fine;

                // Get the geometry from the element 
                GeometryElement geomElem = el.get_Geometry(opt);

                ServiceWall w = new ServiceWall();
                w.id = el.Id.IntegerValue;
                w.name = el.Name;
                w.category = el.Category.Name;

                w.geometry = "geometry info of the element";
                w.position = "position of the element";

                serviceList.Add(w);
            }
            return serviceList;
        }
        catch (Exception e)
        {
            TaskDialog.Show("Error", e.Message);
        }
        return null;
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

}
