using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.ServiceModel.Description;

[Transaction(TransactionMode.ReadOnly)]
public class TCPServer
{
    private static TCPServer instance = null;

    Int32 port = 13000;
    IPAddress localAddr = IPAddress.Parse("127.0.0.1");

    // Incoming data from the client.
    //public static string request = null;

    TcpClient clientSocket;
    TcpListener serverSocket = null;

    NetworkStream networkStream;


    private static Application m_rvtApp;
    private static Document m_rvtDoc;

    private bool isActive = false;
    private bool keepListening = true;

    private TCPServer() { }

    public static TCPServer Server
    {
        get
        {
            if (instance == null)
            {
                instance = new TCPServer();
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


    public void LaunchServer(ExternalCommandData commandData)
    {
        try
        {
            serverSocket = new TcpListener(localAddr, port);

            clientSocket = default(TcpClient);

            serverSocket.Start();
            

            isActive = true;

            m_rvtApp = commandData.Application.Application;
            m_rvtDoc = commandData.Application.ActiveUIDocument.Document;

            TaskDialog.Show("Server Satus", "Server Launched...");

        }
        catch (Exception e)
        {
            TaskDialog.Show("Ups...", e.Message);
            if (IsActive)
            {
                IsActive = false;
            }
            return;
        }
    }

    public void ListenRequest()
    {

        try
        {
            clientSocket = serverSocket.AcceptTcpClient();

            TaskDialog.Show("Connection", "Accept connection from client");
            
            Byte[] bytesFrom = new Byte[256];
            string request;
            keepListening = true;
            networkStream = clientSocket.GetStream();

            while (keepListening)
            {
                

                networkStream.Read(bytesFrom, 0, bytesFrom.Length);

                request = String.Empty;

                request = System.Text.Encoding.ASCII.GetString(bytesFrom);
                request = request.Substring(0, request.IndexOf("$"));

                //TaskDialog.Show("Request", request);

                string response = ProcessRequest(request);

                Byte[] resp = System.Text.Encoding.ASCII.GetBytes(response);

                networkStream.Write(resp, 0, resp.Length);
            }

            clientSocket.Close();

        }
        catch (Exception ex)
        {
            TaskDialog.Show("Ups", ex.Message);
            CloseServer();
            
        }

        

    }

    private string ProcessRequest(string request)
    {
        string[] req = request.Split('?');
        string response = "";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        switch (req[0])
        {
            case "GetMaterials":
                GetMaterialRequest materialRequest = new GetMaterialRequest();
                //foreach(string p in req[1].Split('&'))
                //{
                    string id = (req[1].Split('&'))[0].Split('=')[1];
                    parameters.Add("elementID", id);
                //}
                materialRequest.Pamareters = parameters;
                response = materialRequest.ExecuteRequest();
                break;
            case "GetElementInfo":
                GetElementInfoRequest elementInfoRequest = new GetElementInfoRequest();
                string idElement = (req[1].Split('&'))[0].Split('=')[1];
                parameters.Add("elementID", idElement);
                elementInfoRequest.Pamareters = parameters;
                response = elementInfoRequest.ExecuteRequest();
                break;
            case "StopListening":
                keepListening = false;
                response = "Stop Listening";
                break;
            default:
                response = "Invalid Request";
                break;

        }

        return response + "$";
    }


    public void CloseServer()
    {
        

        serverSocket.Stop();
        IsActive = false;

        TaskDialog.Show("Server Satus", "Server Stoped!");
        
    }

}

