using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Xml.Schema;

namespace WSDLProject
{
    /// <summary>
    /// This class is the controller
    /// </summary>
    public class Controller
    {
        /// <summary>
        /// Model
        /// </summary>
        WSDLModel model;

        /// <summary>
        /// View
        /// </summary>
        MainView view;

        /// <summary>
        /// Contains xsd
        /// </summary>
        Dictionary<String, String> xmlsc;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="view"></param>
        public Controller(WSDLModel model, MainView view)
        {
            this.model = model;
            this.view = view;
            this.view.AddListener(this);
            //
            model.MessagesModel = new MessagesModel();
            model.OperationsModel = new OperationsModel();
            model.BindingsModel = new BindingsModel();
            //
            xmlsc = new Dictionary<string, string>();
        }

        /// <summary>
        /// Load WSDL File by URI
        /// </summary>
        /// <param name="uri"></param>
        public void LoadFileWSDL(String uri)
        {
            //Build the URL request string
            UriBuilder uriBuilder = new UriBuilder(uri);
            uriBuilder.Query = "WSDL";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Method = "GET";
            webRequest.Accept = "text/xml";

            //Submit a web request to get the web service's WSDL
            ServiceDescription serviceDescription;
            using (WebResponse response = webRequest.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    serviceDescription = ServiceDescription.Read(stream);
                }
            }
            // xml
            Types types = serviceDescription.Types;
            String targetURI, targetFile;

            foreach (XmlSchema sc in types.Schemas)
            {
                targetURI = sc.TargetNamespace;

                foreach (XmlSchemaInclude include in sc.Includes)
                {
                    targetFile = include.SchemaLocation;
                    //
                    String pathXSD = targetURI + targetFile;
                    LoadXSD(pathXSD);
                }
            }
            ServiceCollection serv = serviceDescription.Services;
            Port port = serv[0].Ports[0];
            foreach (ServiceDescriptionFormatExtension extension in port.Extensions)
            {
                SoapAddressBinding address = (SoapAddressBinding)extension;
                model.WsdlSoap = address.Location;
            }
            view.Information = "Endpoint : " + model.WsdlSoap;
            // Messages
            MessageCollection msg = serviceDescription.Messages;

            foreach (System.Web.Services.Description.Message message in msg)
            {
                MessageModel tmp = new MessageModel()
                {
                    MessageName = message.Name,
                    Element = message.Parts[0].Element.Name
                };
                model.MessagesModel.Add(tmp);
            }

            // Loop all port type
            foreach (PortType portType in serviceDescription.PortTypes)
            {

                foreach (Operation operation in portType.Operations)
                {
                    OperationModel tmp = new OperationModel();
                    tmp.PortTypeName = portType.Name;
                    tmp.Name = operation.Name;

                    foreach (var message in operation.Messages)
                    {
                        if (message is OperationInput)
                            tmp.Input = ((OperationInput)message).Message.Name;
                        if (message is OperationOutput)
                            tmp.Output = ((OperationOutput)message).Message.Name;
                    }
                    model.OperationsModel.Add(tmp);
                }
            }

            // loop all bindings
            foreach (System.Web.Services.Description.Binding binding in serviceDescription.Bindings)
            {
                foreach (System.Web.Services.Description.OperationBinding operation in binding.Operations)
                {
                    BindingModel tmp = new BindingModel();

                    tmp.BindingName = binding.Name;
                    tmp.Type = binding.Type.Name;
                    tmp.OperationName = operation.Name;
                    tmp.InputName = operation.Input.Name;
                    tmp.OutputName = operation.Output.Name;
                    model.BindingsModel.Add(tmp);
                }
            }

            view.UpdateTable(model);
        }

        /// <summary>
        /// Load XSD into dictionnary
        /// </summary>
        /// <param name="uri"></param>
        public void LoadXSD(String uri)
        {
            UriBuilder uriBuilder = new UriBuilder(uri);
            uriBuilder.Query = "XSD";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Method = "GET";
            webRequest.Accept = "text/xml";

            //Submit a web request to get the web service's WSDL
            XmlSchema xmlSchema;
            using (WebResponse response = webRequest.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    xmlSchema = XmlSchema.Read(stream, null);
                }
            }

            foreach (object item in xmlSchema.Items)
            {
                XmlSchemaElement schemaElement = item as XmlSchemaElement;
                XmlSchemaComplexType complexType = item as XmlSchemaComplexType;
                String msg = "";
                String id = "";

                if (schemaElement != null)
                {
                    id =  schemaElement.Name;

                    XmlSchemaType schemaType = schemaElement.SchemaType;
                    XmlSchemaComplexType schemaComplexType = schemaType as XmlSchemaComplexType;

                    if (schemaComplexType != null)
                    {
                        XmlSchemaParticle particle = schemaComplexType.Particle;
                        XmlSchemaSequence sequence =
                            particle as XmlSchemaSequence;
                        if (sequence != null)
                        {
                            foreach (XmlSchemaElement childElement in sequence.Items)
                            {
                                msg += "-"+childElement.Name + " : " + childElement.SchemaTypeName.Name + "\n";
                            }
                        }
                    }
                    xmlsc.Add(id, msg);
                }
                else if (complexType != null)
                {
                    id = complexType.Name;
                    OutputElements(complexType.Particle, msg);
                    xmlsc.Add(id, msg);
                }
            }
        }

        /// <summary>
        /// Parse element
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="msg"></param>
        private static void OutputElements(XmlSchemaParticle particle, String msg)
        {
            XmlSchemaSequence sequence = particle as XmlSchemaSequence;
            XmlSchemaChoice choice = particle as XmlSchemaChoice;
            XmlSchemaAll all = particle as XmlSchemaAll;

            if (sequence != null)
            {
                msg += "Sequence";
                for (int i = 0; i < sequence.Items.Count; i++)
                {
                    XmlSchemaElement childElement = sequence.Items[i] as XmlSchemaElement;
                    XmlSchemaSequence innerSequence = sequence.Items[i] as XmlSchemaSequence;
                    XmlSchemaChoice innerChoice = sequence.Items[i] as XmlSchemaChoice;
                    XmlSchemaAll innerAll = sequence.Items[i] as XmlSchemaAll;

                    if (childElement != null)
                    {
                        msg +=  "-"+childElement.Name+" : "+childElement.SchemaTypeName.Name +"\n";
                    }
                    else
                    {
                        OutputElements(sequence.Items[i] as XmlSchemaParticle, msg);
                    }
                }
            }
            else if (choice != null)
            {
                for (int i = 0; i < choice.Items.Count; i++)
                {
                    XmlSchemaElement childElement = choice.Items[i] as XmlSchemaElement;
                    XmlSchemaSequence innerSequence = choice.Items[i] as XmlSchemaSequence;
                    XmlSchemaChoice innerChoice = choice.Items[i] as XmlSchemaChoice;
                    XmlSchemaAll innerAll = choice.Items[i] as XmlSchemaAll;

                    if (childElement != null)
                    {
                        msg += "-"+childElement.Name + " : " + childElement.SchemaTypeName.Name + "\n";
                    }
                    else { 
                        OutputElements(choice.Items[i] as XmlSchemaParticle, msg);
                    } 
                }
            }
            else if (all != null)
            {
                for (int i = 0; i < all.Items.Count; i++)
                {
                    XmlSchemaElement childElement = all.Items[i] as XmlSchemaElement;
                    XmlSchemaSequence innerSequence = all.Items[i] as XmlSchemaSequence;
                    XmlSchemaChoice innerChoice = all.Items[i] as XmlSchemaChoice;
                    XmlSchemaAll innerAll = all.Items[i] as XmlSchemaAll;

                    if (childElement != null)
                    {
                        msg += "-"+childElement.Name + " : " + childElement.SchemaTypeName.Name + "\n";
                    }
                    else { 
                        OutputElements(all.Items[i] as XmlSchemaParticle, msg);
                    } 
                }
            }
        }

        /// <summary>
        /// Get information (name, type of attributes
        /// </summary>
        /// <param name="index"></param>
        /// <param name="table"></param>
        public void GetInformationByIndex(int index, int table)
        {
            String element = "";
            String operation = "";
            switch(table)
            { 
                case 1 : element = model.MessagesModel.GetMessages().ElementAt(index).Element;
                    break;
                case 2 : element = model.OperationsModel.GetOperations().ElementAt(index).Output;
                    operation = model.OperationsModel.GetOperations().ElementAt(index).Name;
                    break;
                case 3: element = model.BindingsModel.GetBindings().ElementAt(index).OutputName;
                    operation = model.BindingsModel.GetBindings().ElementAt(index).OperationName;
                    break;
            }
            view.Information = "";
            if (xmlsc.ContainsKey(operation))
            {
                String value = xmlsc[operation];
                view.Information = operation +"\n"+value+"\n";
            }
            if (xmlsc.ContainsKey(element))
            {
                String value = xmlsc[element];
                view.Information = view.Information + element + "\n" + value + "\n";
            }
        }
    }
}
