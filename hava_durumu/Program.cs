using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Deployment.Application;
using System.Reflection;
namespace Ucus_Yolu
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Form1 f = new Form1()) { 
             if (ApplicationDeployment.IsNetworkDeployed) {
                f.Text += " Versiyon: " + ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            KisaYolOlustur();
            Application.Run(f);
            }
            tampon_bellek.dongu_durdur();
        }

        /// <summary>
        /// This will create a Application Reference file on the users desktop
        /// if they do not already have one when the program is loaded.
        /// Check for them running the deployed version before doing this,
        /// so it doesn't kick it when you're running it from Visual Studio.
        /// </summary
        static void KisaYolOlustur()
        {
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                if (ad.IsFirstRun)  //first time user has run the app since installation or update
                {
                    try
                    {
                        Assembly code = Assembly.GetExecutingAssembly();
                        string company = string.Empty;
                        string description = string.Empty;
                        if (Attribute.IsDefined(code, typeof(AssemblyCompanyAttribute)))
                        {
                            AssemblyCompanyAttribute ascompany =
                                (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(code,
                                typeof(AssemblyCompanyAttribute));
                            company = ascompany.Company;
                        }
                        if (Attribute.IsDefined(code, typeof(AssemblyDescriptionAttribute)))
                        {
                            AssemblyDescriptionAttribute asdescription =
                                (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(code,
                                typeof(AssemblyDescriptionAttribute));
                            description = asdescription.Description;
                        }
                        if (company != string.Empty && description != string.Empty)
                        {
                            string desktopPath = string.Empty;
                            desktopPath = string.Concat(
                                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                "\\", description, ".appref-ms");
                            string shortcutName = string.Empty;
                            shortcutName = string.Concat(
                                Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                                "\\", company, "\\", description, ".appref-ms");
                            System.IO.File.Copy(shortcutName, desktopPath, true);
                        }
                    }
                    catch(Exception ex) {
                        MessageBox.Show("Kısayol Oluştururken Hata Meydan Geldi.Hata: " + ex.Message);
                    }
                  
                }
            }
        }
    }
}
