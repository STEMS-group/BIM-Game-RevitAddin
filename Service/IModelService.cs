using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace BIM_Service
{
    [ServiceContract]
    interface IModelService
    {
        [OperationContract]
        Info GetModelInfo();

        [OperationContract]
        //List<Element> GetElements();
        Dictionary<ElementType, List<Element>> GetElements(List<ElementType> elements);

        [OperationContract]
        List<Material> GetMaterialsFromElement(string uniqueId);

    }

    [DataContract]
    public class Info
    {
        [DataMember]
        public string name;

        [DataMember]
        public string docTitle;

    }

    [DataContract]
    [KnownType(typeof(ServiceWall))]
    public class Element
    {
        [DataMember]
        public int id;

        [DataMember]
        public string name;

        [DataMember]
        public string category;

        [DataMember]
        public string type;

        [DataMember]
        public string obs;

    }

    [DataContract]
    public class Material
    {
        [DataMember]
        public int id;

        [DataMember]
        public string name;

        [DataMember]
        public string color;

        [DataMember]
        public string shininess;

        [DataMember]
        public string smothness;

    }

    [DataContract]
    public class ServiceWall: Element
    {
        [DataMember]
        public string geometry;

        [DataMember]
        public string position;
    }

    [DataContract(Name = "ElementType")]
    public enum ElementType
    {
        [EnumMember]
        Ceiling,
        [EnumMember]
        Columns,
        [EnumMember]
        Doors,
        [EnumMember]
        Floors,
        [EnumMember]
        Furniture,
        [EnumMember]
        Railings,
        [EnumMember]
        Roofs,
        [EnumMember]
        Stairs,
        [EnumMember]
        Walls,
        [EnumMember]
        Windows
    }

}