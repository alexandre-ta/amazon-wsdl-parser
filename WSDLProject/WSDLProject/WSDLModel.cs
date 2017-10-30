using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSDLProject
{
    /// <summary>
    /// This class represents WSDL
    /// </summary>
    public class WSDLModel
    {
        public MessagesModel MessagesModel { get; set; }

        public OperationsModel OperationsModel { get; set; }

        public BindingsModel BindingsModel { get; set; }

        public String WsdlSoap { get; set; }
    }

    /// <summary>
    /// This class represents a message model
    /// </summary>
    public class MessageModel
    {
        public String MessageName { get; set; }

        public String Element { get; set; }

        public String Name { get; set; }
    };

    /// <summary>
    /// This class represents a set of message model
    /// </summary>
    public class MessagesModel
    {
        private List<MessageModel> MessageList;

        public MessagesModel()
        {
            MessageList = new List<MessageModel>();
        }

        public void Add(MessageModel msg)
        {
            MessageList.Add(msg);
        }

        public List<MessageModel> GetMessages()
        {
            return MessageList;
        }
    };

    /// <summary>
    /// This class represents an operation
    /// </summary>
    public class OperationModel
    {
        public String PortTypeName { get; set; }

        public String Name { get; set; }

        public String Input { get; set; }

        public String Output { get; set; }
    };

    /// <summary>
    /// This class represents a set of operation
    /// </summary>
    public class OperationsModel
    {
        private List<OperationModel> OperationList;

        public OperationsModel()
        {
            OperationList = new List<OperationModel>();
        }

        public void Add(OperationModel operation)
        {
            OperationList.Add(operation);
        }

        public List<OperationModel> GetOperations()
        {
            return OperationList;
        }
    };

    /// <summary>
    /// This class represents a binding
    /// </summary>
    public class BindingModel
    {
        public String BindingName { get; set; }

        public String Type { get; set; }

        public String StyleBinding { get; set; }

        public String OperationName { get; set; }

        public String InputName { get; set; }

        public String OutputName { get; set; }
    };

    /// <summary>
    /// This class represents a set of binding
    /// </summary>
    public class BindingsModel
    {

        List<BindingModel> BindingList;

        public BindingsModel()
        {
            BindingList = new List<BindingModel>();
        }

        public void Add(BindingModel binding)
        {
            BindingList.Add(binding);
        }

        public List<BindingModel> GetBindings()
        {
            return BindingList;
        }
    };
}
