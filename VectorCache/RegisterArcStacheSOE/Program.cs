// Copyright 2010 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions at &lt;your ArcGIS install location&gt;/DeveloperKit10.0/userestrictions.txt.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegisterArcStacheSOE
{
    class Program
    {
        static void Main(string[] args)
        {
            // Must run as an user in the agsadmin group on the SOM
            ESRI.ArcGIS.ADF.Connection.AGS.AGSServerConnection agsServerConnection =
                new ESRI.ArcGIS.ADF.Connection.AGS.AGSServerConnection();
            agsServerConnection.Host = "localhost";
            agsServerConnection.Connect();
            ESRI.ArcGIS.Server.IServerObjectAdmin2 serverObjectAdmin =
                agsServerConnection.ServerObjectAdmin as ESRI.ArcGIS.Server.IServerObjectAdmin2;

            // This name must match those defined for property pages 
            string extensionName = "arcstache";

            // Check command line arguments to see if SOE is to be unregistered
            if (args.Length == 1 && args[0] == "/unregister")
            {
                // Check whether the SOE is registered
                if (ExtensionRegistered(serverObjectAdmin, extensionName))
                {
                    // Delete the SOE
                    serverObjectAdmin.DeleteExtensionType("MapServer", extensionName);
                    Console.WriteLine(extensionName + " successfully unregistered");
                }
                else
                    Console.WriteLine(extensionName + " is not registered with ArcGIS Server");
            }
            else
            {
                // Check whether the SOE is registered
                if (!ExtensionRegistered(serverObjectAdmin, extensionName))
                {
                    // Use IServerObjectExtensionType3 to get access to info properties
                    ESRI.ArcGIS.Server.IServerObjectExtensionType3 serverObjectExtensionType =
                        serverObjectAdmin.CreateExtensionType() as ESRI.ArcGIS.Server.IServerObjectExtensionType3;

                    // Must match the namespace and class name of the class implementing IServerObjectExtension
                    serverObjectExtensionType.CLSID = "ArcStache.ArcStache";
                    //serverObjectExtensionType.CLSID = "{91EB05FE-7970-45F3-A51B-664310938EFF}";
                    serverObjectExtensionType.Description = "ArcStache creates a tile cache of vector data in ArcGIS Server JSON and GeoJSON formats";
                    serverObjectExtensionType.Name = extensionName;

                    // Name that will be shown in the capabilities list on property pages
                    serverObjectExtensionType.DisplayName = "ArcStache";

                    // Use info properties to define capabilities and msd support
                    serverObjectExtensionType.Info.SetProperty("SupportsMSD", "true");

                    // Required to enable exposure of SOE with ArcGIS Server REST endpoint
                    serverObjectExtensionType.Info.SetProperty("SupportsREST", "true");

                    // Register the SOE with the server
                    serverObjectAdmin.AddExtensionType("MapServer", serverObjectExtensionType);

                    Console.WriteLine(extensionName + " successfully registered with ArcGIS Server");
                }
                else
                    Console.WriteLine(extensionName + " is already registered with ArcGIS Server");
            }

            Console.ReadLine();
        }

        // Checks whether an extension with the passed-in name is already registered with the passed-in server
        static private bool ExtensionRegistered(ESRI.ArcGIS.Server.IServerObjectAdmin2 serverObjectAdmin, string extensionName)
        {
            // Get the extensions that extend MapServer server objects
            ESRI.ArcGIS.Server.IEnumServerObjectExtensionType extensionTypes = serverObjectAdmin.GetExtensionTypes("MapServer");
            extensionTypes.Reset();

            // If an extension with a name matching that passed-in is found, return true
            ESRI.ArcGIS.Server.IServerObjectExtensionType extensionType = extensionTypes.Next();
            while (extensionType != null)
            {
                if (extensionType.Name == extensionName)
                    return true;

                extensionType = extensionTypes.Next();
            }

            // No matching extension was found, so return false
            return false;
        }
    }
}
