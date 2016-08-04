using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Utility;
using Autodesk.Revit.UI.Selection;

[Transaction(TransactionMode.ReadOnly)]
class GetMaterials : Command
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
            TaskDialog.Show("Info", "Server not launched");
            return Result.Succeeded;
        }

        //  Get the access to the top most objects. 
        UIApplication rvtUIApp = commandData.Application;
        UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
        Application m_rvtApp = rvtUIApp.Application;
        Document m_rvtDoc = rvtUIDoc.Document;

        // (1) pick an object on a screen.
        Reference refPick = rvtUIDoc.Selection.PickObject(ObjectType.Element, "Pick an element");

        // we have picked something. 
        Element elem = m_rvtDoc.GetElement(refPick);

        List<BIM_Service.Material> elementsList = ModelService.Service.GetMaterialsFromElement(elem.Id.IntegerValue.ToString());

        foreach (BIM_Service.Material el in elementsList)
        {
            string result = "";
            result += "Name: " + el.name + " - ID: " + el.id + "\n";
            result += "### Color:\n" + el.color + " \n\n";
            result += "### Shininess:\n" + el.shininess + " \n\n";
            result += "### Smothness:\n" + el.smothness;

            TaskDialog.Show("Element List", "List of Materials:\n" + result);
        }


        return Result.Succeeded;

    }

    public List<BIM_Service.Material> Get(string elementID)
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

            List<BIM_Service.Material> materialsList = new List<BIM_Service.Material>();

            ICollection<ElementId> mats = element.GetMaterialIds(false);

            foreach (ElementId el in mats)
            {
                Material mat = (Material)doc.GetElement(el);

                AppearanceAssetElement appearance = (AppearanceAssetElement)doc.GetElement(mat.AppearanceAssetId);

                Asset theAsset = appearance.GetRenderingAsset();
                BIM_Service.Material elementMat = new BIM_Service.Material();

                //TODO: Use Appearance/Graphics properties

                //if (!mat.UseRenderAppearanceForShading)
                //{
                    elementMat.name = mat.Name;
                    elementMat.id = mat.Id.IntegerValue;
                    elementMat.color = "[" +
                        mat.Color.Red.ToString() + "," +
                        mat.Color.Green.ToString() + "," +
                        mat.Color.Blue.ToString() + "," +
                        mat.Transparency.ToString() + "]";

                    elementMat.shininess = mat.Shininess.ToString();
                    elementMat.smothness = mat.Smoothness.ToString();
                //}
                //else
                //{
                //    elementMat.name = theAsset.Name;

                //    elementMat.id = theAsset.Id.IntegerValue;
                //    elementMat.color = "[" +
                //        theAsset.Color.Red.ToString() + "," +
                //        theAsset.Color.Green.ToString() + "," +
                //        theAsset.Color.Blue.ToString() + "," +
                //        theAsset.Transparency.ToString() + "]";

                //    elementMat.shininess = theAsset.Shininess.ToString();
                //    elementMat.smothness = theAsset.Smoothness.ToString();
                //}
               
                

                materialsList.Add(elementMat);

            }

            return materialsList;
        }
        catch (Exception e)
        {
            TaskDialog.Show("Error", e.Message);
        }
        return null;
    }

    private string GetAppearanceParameters(AppearanceAssetElement appearance)
    {
        String res = "##########################\n";
        foreach (Parameter p in appearance.Parameters)
        {
            string defName = "Definition: " + p.Definition.Name;
            Definition def = p.Definition;
            
            switch (p.StorageType)
            {
                case StorageType.Double:
                    //covert the number into Metric
                    defName += " : " + p.AsValueString();
                    break;
                case StorageType.ElementId:
                    //find out the name of the element
                    ElementId id = p.AsElementId();
                    if (id.IntegerValue >= 0)
                    {
                        defName += " : " + ModelService.Service.RvtDoc.GetElement(id).Name;
                    }
                    else
                    {
                        defName += " : " + id.IntegerValue.ToString();
                    }
                    break;
                case StorageType.Integer:
                    if (ParameterType.YesNo == p.Definition.ParameterType)
                    {
                        if (p.AsInteger() == 0)
                        {
                            defName += " : " + "False";
                        }
                        else
                        {
                            defName += " : " + "True";
                        }
                    }
                    else
                    {
                        defName += " : " + p.AsInteger().ToString();
                    }
                    break;
                case StorageType.String:
                    defName += " : " + p.AsString();
                    break;
                default:
                    defName = "Unexposed parameter.";
                    break;
            }
            res += "\n" + defName;
        }

        return res;
    }

    private string GetMatParameters(Material mat)
    {
        String res = "##########################\n";
        foreach (Parameter p in mat.Parameters)
        {
            string defName = "Definition: " + p.Definition.Name;
            Definition def = p.Definition;

            switch (p.StorageType)
            {
                case StorageType.Double:
                    //covert the number into Metric
                    defName += " : " + p.AsValueString();
                    break;
                case StorageType.ElementId:
                    //find out the name of the element
                    ElementId id = p.AsElementId();
                    if (id.IntegerValue >= 0)
                    {
                        defName += " : " + ModelService.Service.RvtDoc.GetElement(id).Name;
                    }
                    else
                    {
                        defName += " : " + id.IntegerValue.ToString();
                    }
                    break;
                case StorageType.Integer:
                    if (ParameterType.YesNo == p.Definition.ParameterType)
                    {
                        if (p.AsInteger() == 0)
                        {
                            defName += " : " + "False";
                        }
                        else
                        {
                            defName += " : " + "True";
                        }
                    }
                    else
                    {
                        defName += " : " + p.AsInteger().ToString();
                    }
                    break;
                case StorageType.String:
                    defName += " : " + p.AsString();
                    break;
                default:
                    defName = "Unexposed parameter.";
                    break;
            }
            res += "\n" + defName;
        }

        return res;
    }

    // Show a dialog with the asset proprieties
    private string GetAssetProperties(Asset theAsset)
    {
        List<AssetProperty> assets = new List<AssetProperty>();
        for (int idx = 0; idx < theAsset.Size; idx++)
        {
            AssetProperty ap = theAsset[idx];
            assets.Add(ap);
        }

        String res = "";
        // order the properties!
        assets = assets.OrderBy(ap => ap.Name).ToList();
        for (int idx = 0; idx < assets.Count; idx++)
        {
            AssetProperty ap = assets[idx];
            Type type = ap.GetType();
            object apVal = null;
            try
            {
                // using .net reflection to get the value
                var prop = type.GetProperty("Value");
                if (prop != null &&
                    prop.GetIndexParameters().Length == 0)
                {
                    apVal = prop.GetValue(ap);
                }
                else
                {
                    apVal = "<No Value Property>";
                }
            }
            catch (Exception ex)
            {
                apVal = ex.GetType().Name + "-" + ex.Message;
            }

            if (apVal is DoubleArray)
            {
                var doubles = apVal as DoubleArray;
                apVal = doubles.Cast<double>().Aggregate("", (s, d) => s + Math.Round(d, 5) + ",");
            }
            res += idx + " : [" + ap.Type + "] " + ap.Name + " = " + apVal + "\n";
        }

        return res;
    }
}
