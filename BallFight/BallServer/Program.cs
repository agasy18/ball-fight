using BallNetModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

namespace BallServer
{
    class Program
    {
        static void Main(string[] args)
        {
         /* if(args!=null)
            System.Diagnostics.Debugger.Launch();*/
            Program obj = new Program();

            if (obj.IsCurrentlyRunningAsAdmin())
                obj.RunServer();
            else
                MessageBox.Show("Did you run it as administrator?", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void RunServer()
        {            
            System.Console.WriteLine("Is Debuging ="+ System.Diagnostics.Debugger.IsAttached);
            using (ServiceHost host = new ServiceHost(typeof(BallService)))
            {
                host.Open();
                Console.WriteLine("Server started");
                Console.WriteLine("\n");
                Console.WriteLine(" Configuration Name: " + host.Description.ConfigurationName);
                Console.WriteLine(" End point address: " + host.Description.Endpoints[0].Address);
                Console.WriteLine(" End point binding: " + host.Description.Endpoints[0].Binding.Name);
                Console.WriteLine(" End point contract: " + host.Description.Endpoints[0].Contract.ConfigurationName);
                Console.WriteLine(" End point name: " + host.Description.Endpoints[0].Name);
                Console.WriteLine(" End point lisent uri: " + host.Description.Endpoints[0].ListenUri);
                Console.WriteLine(" \nName:" + host.Description.Name);
                Console.WriteLine(" Namespace: " + host.Description.Namespace);
                Console.WriteLine(" Service Type: " + host.Description.ServiceType);

                Console.ReadLine();
                host.Close();
            }
        }
        private bool IsCurrentlyRunningAsAdmin()
        {
            bool isAdmin = false;
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            if (currentIdentity != null)
            {
                WindowsPrincipal pricipal = new WindowsPrincipal(currentIdentity);
                isAdmin = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
                pricipal = null;
            }
            return isAdmin;
        }
    }
}
