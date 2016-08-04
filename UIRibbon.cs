using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;


using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;



[Transaction(TransactionMode.Manual)]
public class UIRibbon : IExternalApplication
{

    public static PushButton pushButtonStartServer;
    public static PushButton pushButtonStopServer;

    private static ModelService service;

    private static TCPServer server;
    public static IList<RibbonItem> TCPButtons;

    /// <summary>
    /// This is both the assembly name and the namespace 
    /// of the external command provider.
    /// </summary>
    public const string _introLabName = "DISS_Addin";
    public const string _uiLabName = "UiCs";
    public const string _dllExtension = ".dll";

    /// <summary>
    /// Name of subdirectory containing images.
    /// </summary>
    public const string _imageFolderName = "Images";

    /// <summary>
    /// Location of managed dll where we have defined the commands.
    /// </summary>
    string _introLabPath;

    /// <summary>
    /// Location of images for icons.
    /// </summary>
    string _imageFolder;

    public static ModelService Service
    {
        get
        {
            return service;
        }
    }

    public static TCPServer Server
    {
        get
        {
            return server;
        }

    }

    public Result OnStartup(UIControlledApplication application)
    {
        // External application directory:

        string dir = Path.GetDirectoryName(
          System.Reflection.Assembly
          .GetExecutingAssembly().Location);

        // External command path:
        _introLabPath = Path.Combine(dir, _introLabName + _dllExtension);

        if (!File.Exists(_introLabPath))
        {
            TaskDialog.Show("UIRibbon", "External command assembly not found: "
              + _introLabPath);
            return Result.Failed;
        }

        // Image path:
        _imageFolder = FindFolderInParents(dir, _imageFolderName);

        if (null == _imageFolder
          || !Directory.Exists(_imageFolder))
        {
            TaskDialog.Show(
              "UIRibbon",
              string.Format(
                "No image folder named '{0}' found in the parent directories of '{1}.",
                _imageFolderName, dir));

            return Result.Failed;
        }

        service = ModelService.Service;

        server = TCPServer.Server;

        AddRibbonSampler(application);

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }

    /// <summary>
    /// Starting at the given directory, search upwards for 
    /// a subdirectory with the given target name located
    /// in some parent directory. 
    /// </summary>
    /// <param name="path">Starting directory, e.g. GetDirectoryName( GetExecutingAssembly().Location ).</param>
    /// <param name="target">Target subdirectory name, e.g. "Images".</param>
    /// <returns>The full path of the target directory if found, else null.</returns>

    string FindFolderInParents(string path, string target)
    {
        Debug.Assert(Directory.Exists(path),
          "expected an existing directory to start search in");

        string s;

        do
        {
            s = Path.Combine(path, target);
            if (Directory.Exists(s))
            {
                return s;
            }
            path = Path.GetDirectoryName(path);
        } while (null != path);

        return null;
    }


    /// <summary>
    /// Load a new icon bitmap from our image folder.
    /// </summary>
    BitmapImage NewBitmapImage(string imageName)
    {
        return new BitmapImage(new Uri(
          Path.Combine(_imageFolder, imageName)));
    }


    public void AddRibbonSampler(UIControlledApplication app)
    {
        app.CreateRibbonTab("Dissertation Add-in");

        RibbonPanel panel =
          app.CreateRibbonPanel("Dissertation Add-in", "Server Status");

        AddStartServerButton(panel);

        AddStopServerButton(panel);

        panel.AddSeparator();

        AddStackedButtons_Simple(panel);

        panel.AddSeparator();

        AddStackedButtons_TCPServer(panel);
    }



    public void AddStartServerButton(RibbonPanel panel)
    {
        // Set the information about the command we will be assigning 
        // to the button 
        PushButtonData pushButtonDataStartServer =
          new PushButtonData("PushStartServerButton", "Start Server",
                              _introLabPath, "StartServer");

        // Add a button to the panel 
        pushButtonStartServer =
          panel.AddItem(pushButtonDataStartServer) as PushButton;

        // Add an icon 
        // Make sure you reference WindowsBase and PresentationCore, 
        // and import System.Windows.Media.Imaging namespace. 
        pushButtonStartServer.LargeImage = NewBitmapImage("server.png");

        // Add a tooltip 
        pushButtonStartServer.ToolTip = "Server Start";

        //pushButtonStartServer.Enabled = !server.IsActive;
        pushButtonStartServer.Enabled = !service.IsActive;
    }


    public void AddStopServerButton(RibbonPanel panel)
    {
        // Set the information about the command we will be assigning 
        // to the button 
        PushButtonData pushButtonDataStopServer =
          new PushButtonData("PushStopServerButton", "Stop Server",
                              _introLabPath, "StopServer");

        // Add a button to the panel 
        pushButtonStopServer =
          panel.AddItem(pushButtonDataStopServer) as PushButton;

        // Add an icon 
        // Make sure you reference WindowsBase and PresentationCore, 
        // and import System.Windows.Media.Imaging namespace. 
        pushButtonStopServer.LargeImage = NewBitmapImage("server.png");

        // Add a tooltip 
        pushButtonStopServer.ToolTip = "Server Stop";

        //pushButtonStopServer.Enabled = server.IsActive;
        pushButtonStopServer.Enabled = service.IsActive;
    }


    /// <summary>
    /// Stacked Buttons - combination of: push button, dropdown button, combo box and text box. 
    /// (no radio button group, split buttons). 
    /// Here we stack three push buttons for "Command Data", "DB Element" and "Element Filtering". 
    /// </summary>
    public void AddStackedButtons_Simple(RibbonPanel panel)
    {
        // Create three push buttons to stack up 
        // #1 
        PushButtonData pushButtonData1 = new PushButtonData("StackSimpleCommandData", "Get Model Info", _introLabPath, ".GetModelInfo");
        pushButtonData1.Image = NewBitmapImage("ImgHelloWorldSmall.png");

        // #2 
        PushButtonData pushButtonData2 = new PushButtonData("StackSimpleDbElement", "Get Elements", _introLabPath, ".GetElements");
        pushButtonData2.Image = NewBitmapImage("ImgHelloWorldSmall.png");

        // #3 
        PushButtonData pushButtonData3 = new PushButtonData("StackSimpleElementFiltering", "Get Materials", _introLabPath, ".GetMaterials");
        pushButtonData3.Image = NewBitmapImage("ImgHelloWorldSmall.png");

        // Put them on stack 
        IList<RibbonItem> stackedButtons = panel.AddStackedItems(pushButtonData1, pushButtonData2, pushButtonData3);
    }

    /// <summary>
    /// Stacked Buttons - combination of: push button, dropdown button, combo box and text box. 
    /// (no radio button group, split buttons). 
    /// Here we stack three push buttons for "Command Data", "DB Element" and "Element Filtering". 
    /// </summary>
    public void AddStackedButtons_TCPServer(RibbonPanel panel)
    {
        // Create three push buttons to stack up 
        // #1 
        PushButtonData pushButtonData1 = new PushButtonData("LaunchTCPServer", "Launch TCP Server", _introLabPath, ".LaunchTCP");
        pushButtonData1.Image = NewBitmapImage("ImgHelloWorldSmall.png");

        // #2 
        PushButtonData pushButtonData2 = new PushButtonData("ListenRequest", "Listen", _introLabPath, ".ListenRequest");

        // #3 
        PushButtonData pushButtonData3 = new PushButtonData("StopTCP", "Stop TCP Server", _introLabPath, ".StopTCPServer");


        // Put them on stack 
        TCPButtons = panel.AddStackedItems(pushButtonData1, pushButtonData2, pushButtonData3);
        TCPServerEnable(TCPServer.Server.IsActive);
    }

    public static void TCPServerEnable (bool b)
    {
        TCPButtons[0].Enabled = !b;
        TCPButtons[1].Enabled = b;
        TCPButtons[2].Enabled = b;
    }
}

