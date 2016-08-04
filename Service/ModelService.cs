using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.ServiceModel.Description;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
public class ModelService : BIM_Service.IModelService
{
    private bool debugServiceData = false;

    private static ModelService instance;
    private ServiceHost SHost = null;

    private bool isActive = false;

    private static Application m_rvtApp;
    private static Document m_rvtDoc;

    public Application RvtApp
    {
        get
        {
            return m_rvtApp;
        }

        set
        {
            m_rvtApp = value;

        }
    }

    public Document RvtDoc
    {
        get
        {
            return m_rvtDoc;
        }

        set
        {
            m_rvtDoc = value;
        }
    }

    private ModelService() { }

    public static ModelService Service
    {
        get
        {
            if (instance == null)
            {
                instance = new ModelService();
            }
            return instance;
        }
    }

    public bool IsActive
    {
        get
        {
            return isActive;
        }

        set
        {
            isActive = value;
        }
    }


    public void StartService(ExternalCommandData commandData)
    {
        if (IsActive)
            return;

        try
        {
            Uri baseAddress = new Uri("http://localhost:9000/ModelServiceHost");
            SHost = new ServiceHost(typeof(ModelService), baseAddress);

            WSHttpBinding binding = new WSHttpBinding();
            binding.OpenTimeout = new TimeSpan(0, 10, 0);
            binding.CloseTimeout = new TimeSpan(0, 10, 0);
            binding.SendTimeout = new TimeSpan(0, 10, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 10, 0);

            SHost.AddServiceEndpoint(typeof(BIM_Service.IModelService), binding, "ModelService");

            // Check to see if the service host already has a ServiceMetadataBehavior
            ServiceMetadataBehavior smb = SHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
            // If not, add one
            if (smb == null)
                smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.HttpsGetEnabled = true;
            SHost.Description.Behaviors.Add(smb);

            SHost.AddServiceEndpoint(typeof(IMetadataExchange), binding, "ModelService/mex");

            NetTcpBinding netBinding = new NetTcpBinding();
            netBinding.OpenTimeout = new TimeSpan(0, 10, 0);
            netBinding.CloseTimeout = new TimeSpan(0, 10, 0);
            netBinding.SendTimeout = new TimeSpan(0, 10, 0);
            netBinding.ReceiveTimeout = new TimeSpan(0, 10, 0);

            SHost.AddServiceEndpoint(typeof(BIM_Service.IModelService), netBinding, "net.tcp://localhost:8700/ModelServiceHost/ModelService");

            ServiceDebugBehavior debug = SHost.Description.Behaviors.Find<ServiceDebugBehavior>();

            // if not found - add behavior with setting turned on 
            if (debug == null)
            {
                SHost.Description.Behaviors.Add(
                     new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            }
            else
            {
                // make sure setting is turned ON
                if (!debug.IncludeExceptionDetailInFaults)
                {
                    debug.IncludeExceptionDetailInFaults = true;
                }
            }

            SHost.Open();

            IsActive = true;

            m_rvtApp = commandData.Application.Application;
            m_rvtDoc = commandData.Application.ActiveUIDocument.Document;

            TaskDialog.Show("Server Satus", "Running at \n" + baseAddress.AbsoluteUri);

            if (debugServiceData)
            {
                foreach (ServiceEndpoint s in SHost.Description.Endpoints)
                {
                    string operations = "";

                    foreach (OperationDescription op in s.Contract.Operations)
                    {
                        operations += "\t" + op.Name + "\n";
                    }

                    TaskDialog.Show("Endpoint", "EndPoint " + s.Name + ":\n" +
                        "Adress: " + s.Address.Uri.AbsoluteUri + "\n" +
                        "Binding Name: " + s.Binding.Name + "\n" +
                        "Binding NameSpace: " + s.Binding.Namespace + "\n" +
                        "Binding OpenTimeout: " + s.Binding.OpenTimeout + "\n" +
                        "Contract Name: " + s.Contract.Name + "\n" +
                        "Contract Operations:\n" + operations +
                        "Contract Configuration Name: " + s.Contract.ConfigurationName + "\n" +
                        "IsSystemEndpoint: " + s.IsSystemEndpoint + "\n" +
                        "Listen Uri: " + s.ListenUri.AbsoluteUri + "\n" +
                        "Listen Uri Mode: " + s.ListenUriMode);
                }
            }

        }
        catch (Exception e)
        {
            TaskDialog.Show("Ups...", e.Message);
            if (IsActive)
            {
                IsActive = false;
                SHost.Close();
            }
            return;
        }

    }

    public void StopService()
    {
        if (!IsActive)
            return;

        try
        {
            SHost.Close();
            SHost = null;
            TaskDialog.Show("Server Satus", "Server Stoped!");

            IsActive = false;
        }
        catch (Exception e)
        {
            TaskDialog.Show("Ups...", e.Message);
            return;
        }
    }


    /// <summary>
    /// Service Protocol
    /// </summary>
    /// <returns></returns>
    public BIM_Service.Info GetModelInfo()
    {
        GetModelInfo modelInfo = new GetModelInfo();

        return modelInfo.Get(); ;
    }

    public Dictionary<BIM_Service.ElementType, List<BIM_Service.Element> > GetElements(List<BIM_Service.ElementType> elementTypes)
    {
        Dictionary<BIM_Service.ElementType, List<BIM_Service.Element>> result = new Dictionary<BIM_Service.ElementType, List<BIM_Service.Element>>();

        foreach (BIM_Service.ElementType type in elementTypes)
        {
            GetElements elements = new GetElements();
            result.Add(type, elements.Get(type));
        }

        return result;
    }

    public List<BIM_Service.Material> GetMaterialsFromElement(string id)
    {
        GetMaterials materials = new GetMaterials();

        return materials.Get(id);
    }

}


