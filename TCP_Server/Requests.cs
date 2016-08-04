using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


abstract class Request
{
    public Dictionary<string, string> pamareters;

    public Dictionary<string, string> Pamareters
    {
        get
        {
            return pamareters;
        }

        set
        {
            pamareters = value;
        }
    }

    abstract public string ExecuteRequest();
}


class GetMaterialRequest : Request
{
    public override string ExecuteRequest()
    {
        GetMaterials materials = new GetMaterials();

        List<BIM_Service.Material> list = materials.Get(Pamareters["elementID"]);

        string response = "";

        for (int i = 0; i < list.Count; i++)
        {
            response += "{";
            response += "id:" + list[i].id + ";";
            response += "name:" + list[i].name + ";";
            response += "color:" + list[i].color + ";";
            response += "shininess:" + list[i].shininess + ";";
            response += "smothness:" + list[i].smothness + ";}#";
        }

        return response;
    }
}

class GetElementInfoRequest : Request
{
    public override string ExecuteRequest()
    {
        GetElementInfo elementInfo = new GetElementInfo();

        BIM_Service.Element e = elementInfo.Get(Pamareters["elementID"]);

        string response = "";

        response += "{";
        response += "id:" + e.id + ";";
        response += "name:" + e.name + ";";
        response += "category:" + e.category + ";";
        response += "type:" + e.type + ";";
        response += "obs:" + e.obs + ";}#";

        return response;
    }
}

class GetModelInfoRequest : Request
{
    public override string ExecuteRequest()
    {
        GetModelInfo elementInfo = new GetModelInfo();

        BIM_Service.Info info = elementInfo.Get();

        string response = "";

        response += "{";
        response += "name:" + info.name + ";";
        response += "title:" + info.docTitle +";}#";

        return response;
    }
}
