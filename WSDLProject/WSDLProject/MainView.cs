using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WSDLProject
{
    public partial class MainView : Form
    {
        /// <summary>
        /// Controller
        /// </summary>
        Controller controller;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary>
        /// Add controller to this view
        /// </summary>
        /// <param name="controller"></param>
        public void AddListener(Controller controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Process the wsdl file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGo_Click(object sender, EventArgs e)
        {
            controller.LoadFileWSDL(tbURI.Text);
            btnGo.Enabled = false;
        }

        /// <summary>
        /// Update all datagrids
        /// </summary>
        /// <param name="model"></param>
        public void UpdateTable(WSDLModel model)
        { 
            // Messages
            List<MessageModel> messages = model.MessagesModel.GetMessages();
            foreach (MessageModel msg in messages)
            {
                tbMessages.Rows.Add(msg.MessageName, msg.Element);
            }
            // Operations
            List<OperationModel> operations = model.OperationsModel.GetOperations();
            foreach (OperationModel ope in operations)
            {
                tbOperations.Rows.Add(ope.PortTypeName, ope.Name, ope.Input, ope.Output);
            }
            // Bindings
            List<BindingModel> bindings = model.BindingsModel.GetBindings();
            foreach (BindingModel bind in bindings)
            {
                tbBindings.Rows.Add(bind.BindingName, bind.Type, bind.OperationName, bind.InputName, bind.OutputName);
            }
        }

        /// <summary>
        /// Information area
        /// </summary>
        public String Information
        {
            get
            {
                return tfInformation.Text;
            }
            set
            {
                tfInformation.Text = value;
            }
        }

        /// <summary>
        /// Messages Datagrid listener
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbMessages_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index != -1) { 
                controller.GetInformationByIndex(index, 1);
            }
        }

        /// <summary>
        /// Operations Datagrid listener
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbOperations_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index != -1)
            {
                controller.GetInformationByIndex(index, 2);
            }
        }

        /// <summary>
        /// Bindings Datagrid listener
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbBindings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index != -1)
            {
                controller.GetInformationByIndex(index, 3);
            }
        }
    }
}
