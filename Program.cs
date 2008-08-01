using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MPlayerFront
{
    static class Program
    {
        private static readonly System.Xml.Serialization.XmlSerializer xs_ =
            new System.Xml.Serialization.XmlSerializer(typeof(MFOptions));

        private static MFOptions options_ = new MFOptions();

        public static MainFrm MainFrm = null;

        public static MFOptions Options
        {
            get { return options_; }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                LoadOptions();

                Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                MainFrm = new MainFrm();
                Application.Run(MainFrm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ":" + ex.StackTrace);
            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            SaveOptions();
        }

        private static void LoadOptions()
        {
            try
            {

                string path = AppDomain.CurrentDomain.BaseDirectory +
                    System.IO.Path.DirectorySeparatorChar +
                    "options.config";

                if (System.IO.File.Exists(path))
                {
                    using (System.IO.Stream s = new System.IO.FileStream(path,
                        System.IO.FileMode.Open,
                        System.IO.FileAccess.Read))
                    {
                        options_ = xs_.Deserialize(s) as MFOptions;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);

                options_ = new MFOptions();
            }
        }

        public static void SaveOptions()
        {
            try
            {

                string path = AppDomain.CurrentDomain.BaseDirectory +
                    System.IO.Path.DirectorySeparatorChar +
                    "options.config";

                using (System.IO.Stream s = new System.IO.FileStream(path,
                    System.IO.FileMode.Create,
                    System.IO.FileAccess.Write))
                {
                    xs_.Serialize(s, options_);
                }
            }
            catch
            {
            }
        }
    }
}