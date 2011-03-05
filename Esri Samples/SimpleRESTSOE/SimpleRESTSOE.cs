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
using System.Runtime.InteropServices;
using System.EnterpriseServices;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.SOESupport;
using System.Collections.Specialized;

namespace SimpleRESTSOE
{
    [ComVisible(true)]
    [Guid("4446A422-DDA1-4b17-B8E5-73E5D49D7444")]
    [ClassInterface(ClassInterfaceType.None)]
    public class SimpleRESTSOE : ServicedComponent, IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {
        private IRESTRequestHandler _reqHandler;

        public SimpleRESTSOE()
        {
            RestResource rootResource = CreateRestSchema();
            SoeRestImpl restImpl = new SoeRestImpl("SimpleRESTSOE", rootResource);
            _reqHandler = (IRESTRequestHandler)restImpl;
        }

        public void Init(IServerObjectHelper pSOH){}

        public void Shutdown(){}

        public void Construct(IPropertySet props){}

        private RestResource CreateRestSchema()
        {
            RestResource soeResource = new RestResource("SimpleRESTSOE", false, RootSOE);
     
            RestOperation findNearFeatsOp = new RestOperation("echo",
                                                      new string[] { "text" },
                                                      new string[] { "json", "html" },
                                                      EchoInput);
            soeResource.operations.Add(findNearFeatsOp);
            return soeResource;
        }

        public string GetSchema()
        {
            return _reqHandler.GetSchema();
        }

        byte[] IRESTRequestHandler.HandleRESTRequest(string Capabilities, 
            string resourceName, 
            string operationName, 
            string operationInput, 
            string outputFormat, 
            string requestProperties, 
            out string responseProperties)
        {
            return _reqHandler.HandleRESTRequest(Capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out responseProperties);
        }

        private byte[] RootSOE(System.Collections.Specialized.NameValueCollection boundVariables, 
            string outputFormat,
            string requestProperties, 
            out string responseProperties)
        {
            responseProperties = null;
            JsonObject jObject = new JsonObject();
            return Encoding.UTF8.GetBytes(jObject.ToJson());
        }

        private byte[] EchoInput(System.Collections.Specialized.NameValueCollection boundVariables, 
            ESRI.ArcGIS.SOESupport.JsonObject operationInput, 
            string outputFormat, 
            string requestProperties, 
            out string responseProperties)
        {
            responseProperties = null;

            string inputText;
            if (!operationInput.TryGetString("text", out inputText))
                throw new ArgumentNullException("text");
            JsonObject jObject = new JsonObject();
            jObject.AddString("text", inputText);
            return Encoding.UTF8.GetBytes(jObject.ToJson());
        }
    }
}
