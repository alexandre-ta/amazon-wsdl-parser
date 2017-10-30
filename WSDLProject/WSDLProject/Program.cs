using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WSDLProject
{
    static class Program
    {
        /// <summary>
        /// Start application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainView view = new MainView();
            WSDLModel model = new WSDLModel();
            Controller controller = new Controller(model, view);
            Application.Run(view);
        }
    }
}
