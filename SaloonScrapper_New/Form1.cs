using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaloonScrapper_New
{
    public partial class Form1 : Form
    {
        private delegate void SetDGVValueDelegate(DataTable items);

        private void SetDGVValue(DataTable dt)
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new SetDGVValueDelegate(SetDGVValue), dt);
            }
            else
            {
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[2].Width = 260;
            }
        }

        public Form1()
        {
            SetBrowserFeatureControl();
            InitializeComponent();
            webBrowser1.ObjectForScripting = new ScriptManager(this);
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        [ComVisible(true)]
        public class ScriptManager
        {
            Form1 _form;
            public ScriptManager(Form1 form)
            {
                _form = form;
            }
            public void signature(object obj)
            {
                signatureVal = obj.ToString();
            }
            public void gettimestamp(object obj)
            {
                TimeStamp = obj.ToString();
            }
            public void getnonce(object obj)
            {
                Nonce = obj.ToString();
            }
            public void hdfcPwd(object obj, object obj1)
            {
                
            }
            public void PwdValue(object obj)
            {
               
            }
            public void iciciPINEncrypt(object obj)
            {
               
            }
        }

        public static DataTable dtData = new DataTable();
        public static string signatureVal;
        public static string TimeStamp;
        public static string Nonce;
        string ReturnValue = "true";

        private void Form1_Load(object sender, EventArgs e)
        {
            DataColumn dcSno = new DataColumn("Sno");
            DataColumn dcstart_time = new DataColumn("start_time");
            DataColumn dcsell_price = new DataColumn("sell_price");
            DataColumn dcduration = new DataColumn("duration");
            DataColumn dcdisplay_name = new DataColumn("employee_name");
            DataColumn dcservice_name = new DataColumn("service_name");
            DataColumn dcscheduled_date = new DataColumn("scheduled_date");
            dtData.Columns.Add(dcstart_time);
            dtData.Columns.Add(dcsell_price);
            dtData.Columns.Add(dcduration);
            dtData.Columns.Add(dcdisplay_name);
            dtData.Columns.Add(dcservice_name);
            dtData.Columns.Add(dcscheduled_date);
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string Section = string.Empty;
            webBrowser1 = new WebBrowser();
            webBrowser1.ObjectForScripting = new ScriptManager(this);
            this.webBrowser1.Navigate(Path + "HTMLPage1.html");
            int sno = 1;
            while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            object result = this.webBrowser1.Document.InvokeScript("timestamp");

            object result1 = this.webBrowser1.Document.InvokeScript("getnonce");

            object resul2 = this.webBrowser1.Document.InvokeScript("signature");

            List<Newtonsoft.Json.Linq.JToken> lstValServices = new List<Newtonsoft.Json.Linq.JToken>();
            List<Newtonsoft.Json.Linq.JToken> lstHrefServices = new List<Newtonsoft.Json.Linq.JToken>();
            Newtonsoft.Json.Linq.JArray lstServices = new Newtonsoft.Json.Linq.JArray();
            PostDataIRCTC.PostData.FetchAllServices(@"https://pos.shortcutssoftware.com/webapi/site/7966d291-1b12-dd11-a234-0050563fff01/services?limit=5000&is_active=true&fields=display%2Cdescription%2Cprice%2Clinks%2Cdefault_duration_minutes%2Cbreak_duration_minutes&is_customer_bookable=true", TimeStamp, Nonce, signatureVal, ref lstValServices, ref lstHrefServices, ref lstServices);
            foreach (Newtonsoft.Json.Linq.JObject objService in lstServices)
            {
                foreach (System.Collections.Generic.KeyValuePair<string, Newtonsoft.Json.Linq.JToken> keyVal in objService)
                {
                    if (keyVal.Key == "href")
                    {
                        webBrowser1.ObjectForScripting = new ScriptManager(this);
                        this.webBrowser1.Navigate(Path + "HTMLPage1.html");
                        while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                        {
                            Application.DoEvents();
                        }

                        this.webBrowser1.Document.InvokeScript("timestamp");

                        this.webBrowser1.Document.InvokeScript("getnonce");

                        this.webBrowser1.Document.InvokeScript("signatureEmployeePricing", new object[] { keyVal.Value + "/employee_pricing"});
                        lstServices = new Newtonsoft.Json.Linq.JArray();
                        PostDataIRCTC.PostData.FetchAllEmployeePricing(keyVal.Value + "/employee_pricing", TimeStamp, Nonce, signatureVal, ref lstValServices, ref lstHrefServices, ref lstServices);
                        foreach (Newtonsoft.Json.Linq.JObject objEmployee in lstServices)
                        {
                            foreach (System.Collections.Generic.KeyValuePair<string, Newtonsoft.Json.Linq.JToken> keyValEmployee in objEmployee)
                            {
                                if (keyValEmployee.Key == "href")
                                {
                                    PostDataIRCTC.PostData.FetchAvailableSlots("https://www.ulta.com/bookonline/availableTimes.html");
                                    webBrowser1.ObjectForScripting = new ScriptManager(this);
                                    this.webBrowser1.Navigate(Path + "HTMLPage1.html");
                                    while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                                    {
                                        Application.DoEvents();
                                    }

                                    this.webBrowser1.Document.InvokeScript("timestamp");
                                    this.webBrowser1.Document.InvokeScript("getnonce");
                                    this.webBrowser1.Document.InvokeScript("signatureEmployeePricingCalculate", new object[] { "https://pos.shortcutssoftware.com/webapi/site/7966d291-1b12-dd11-a234-0050563fff01/calculate_available_appointments" });
                                    lstServices = new Newtonsoft.Json.Linq.JArray();
                                    ReturnValue = PostDataIRCTC.PostData.FetchAllEmployeeAppointments("https://pos.shortcutssoftware.com/webapi/site/7966d291-1b12-dd11-a234-0050563fff01/calculate_available_appointments", TimeStamp, Nonce, signatureVal, ref lstValServices, ref lstHrefServices, ref lstServices, keyVal.Value + "/employee_pricing");
                                    if (ReturnValue != "true")
                                    {
                                        label1.Text = ReturnValue;
                                        break;
                                    }
                                    foreach (Newtonsoft.Json.Linq.JObject objEmployeeEmployment in lstServices)
                                    {
                                        Newtonsoft.Json.Linq.JToken keyValEmployeeApp = objEmployeeEmployment as Newtonsoft.Json.Linq.JToken;
                                        //foreach (System.Collections.Generic.KeyValuePair<string, Newtonsoft.Json.Linq.JToken> keyValEmployeeApp in objEmployeeEmployment)
                                        //{
                                            string objDate = ((Newtonsoft.Json.Linq.JValue)(objEmployeeEmployment["scheduled_date"])).Value.ToString();
                                            if (objDate == "2016-10-29")
                                            {
                                                //if (keyValEmployeeApp.Key == "services")
                                                //{
                                                //    Newtonsoft.Json.Linq.JArray objArray = Newtonsoft.Json.Linq.JArray.Parse(keyValEmployeeApp.Value.ToString());
                                                //    foreach (Newtonsoft.Json.Linq.JToken EmployeeToken in objArray)
                                                //    {
                                                //        Newtonsoft.Json.Linq.JObject detailskeyValEmployeeAppObj = EmployeeToken as Newtonsoft.Json.Linq.JObject;

                                            
                                                Newtonsoft.Json.Linq.JArray objArray = objEmployeeEmployment["services"] as Newtonsoft.Json.Linq.JArray;
                                                foreach (Newtonsoft.Json.Linq.JToken EmployeeToken in objArray)
                                                {
                                                    DataRow dr = dtData.NewRow();
                                                    Newtonsoft.Json.Linq.JObject detailskeyValEmployeeAppObj = EmployeeToken as Newtonsoft.Json.Linq.JObject;
                                                    if (((Newtonsoft.Json.Linq.JValue)(EmployeeToken["start_time"])).Value.ToString() == "10:00:00")
                                                    {
                                                       
                                                    }
                                                    dr["start_time"] = ((Newtonsoft.Json.Linq.JValue)(EmployeeToken["start_time"])).Value;
                                                    if(EmployeeToken["sell_price"] != null)
                                                        dr["sell_price"] = ((Newtonsoft.Json.Linq.JValue)(EmployeeToken["sell_price"])).Value;
                                                    dr["duration"] = ((Newtonsoft.Json.Linq.JValue)(EmployeeToken["duration"])).Value;
                                                    Newtonsoft.Json.Linq.JArray detailskeyValEmployeeLinkObj = ((Newtonsoft.Json.Linq.JArray)(EmployeeToken["links"]));
                                                    int count = 1;
                                                    foreach (Newtonsoft.Json.Linq.JObject objEmployeeEmploymentLink in detailskeyValEmployeeLinkObj)
                                                    {
                                                        if (count == 1)
                                                        {
                                                            foreach (System.Collections.Generic.KeyValuePair<string, Newtonsoft.Json.Linq.JToken> keyValEmployeeLink in objEmployeeEmploymentLink)
                                                            {
                                                                if (keyValEmployeeLink.Key == "display_name")
                                                                {
                                                                    dr["service_name"] = ((Newtonsoft.Json.Linq.JValue)(keyValEmployeeLink.Value)).Value;
                                                                }
                                                            }
                                                        }
                                                        if (count == 2)
                                                        {
                                                            foreach (System.Collections.Generic.KeyValuePair<string, Newtonsoft.Json.Linq.JToken> keyValEmployeeLink in objEmployeeEmploymentLink)
                                                            {
                                                                if (keyValEmployeeLink.Key == "display_name")
                                                                {
                                                                    dr["employee_name"] = ((Newtonsoft.Json.Linq.JValue)(keyValEmployeeLink.Value)).Value;
                                                                }
                                                            }
                                                        }
                                                        count++;
                                                    }
                                                    dr["scheduled_date"] = objDate;
                                                    sno++;
                                                    dtData.Rows.Add(dr);
                                                }
                                            }
                                                    
                                                //}
                                            //}
                                    }
                                }
                            }
                            if (ReturnValue != "true")
                            {
                                break;
                            }
                        }
                        if (ReturnValue != "true")
                        {
                            break;
                        }
                    }
                    if (ReturnValue != "true")
                    {
                        break;
                    }
                }
                dtData = dtData.DefaultView.ToTable(true,"start_time", "service_name", "employee_name", "duration", "sell_price","scheduled_date");
                
                backgroundWorker1.RunWorkerAsync(0);
            }
        }

        private UInt32 GetBrowserEmulationMode()
        {
            int browserVersion = 7;
            using (var ieKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer",
                RegistryKeyPermissionCheck.ReadSubTree,
                System.Security.AccessControl.RegistryRights.QueryValues))
            {
                var version = ieKey.GetValue("svcVersion");
                if (null == version)
                {
                    version = ieKey.GetValue("Version");
                    if (null == version)
                        throw new ApplicationException("Microsoft Internet Explorer is required!");
                }
                int.TryParse(version.ToString().Split('.')[0], out browserVersion);
            }

            UInt32 mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode. Default value for Internet Explorer 11.
            switch (browserVersion)
            {
                case 7:
                    mode = 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode. Default value for applications hosting the WebBrowser Control.
                    break;
                case 8:
                    mode = 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode. Default value for Internet Explorer 8
                    break;
                case 9:
                    mode = 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
                    break;
                case 10:
                    mode = 10000; // Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE10 mode. Default value for Internet Explorer 10.
                    break;
                default:
                    // use IE11 mode by default
                    break;
            }

            return mode;
        }

        private void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(
                String.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature),
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(appName, (UInt32)value, RegistryValueKind.DWord);
            }
        }

        private void SetBrowserFeatureControl()
        {
            // http://msdn.microsoft.com/en-us/library/ee330720(v=vs.85).aspx

            // FeatureControl settings are per-process
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            // make the control is not running inside Visual Studio Designer
            if (String.Compare(fileName, "devenv.exe", true) == 0 || String.Compare(fileName, "XDesProc.exe", true) == 0)
                return;

            SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, GetBrowserEmulationMode()); // Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode.
            SetBrowserFeatureControlKey("FEATURE_AJAX_CONNECTIONEVENTS", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_MANAGE_SCRIPT_CIRCULAR_REFS", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_DOMSTORAGE ", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_GPU_RENDERING ", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_IVIEWOBJECTDRAW_DMLT9_WITH_GDI  ", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_DISABLE_LEGACY_COMPRESSION", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_LOCALMACHINE_LOCKDOWN", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_OBJECT", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_SCRIPT", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_DISABLE_NAVIGATION_SOUNDS", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_SCRIPTURL_MITIGATION", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_SPELLCHECKING", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_STATUS_BAR_THROTTLING", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_TABBED_BROWSING", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_VALIDATE_NAVIGATE_URL", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_WEBOC_DOCUMENT_ZOOM", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_WEBOC_POPUPMANAGEMENT", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_WEBOC_MOVESIZECHILD", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_ADDON_MANAGEMENT", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_WEBSOCKET", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_WINDOW_RESTRICTIONS ", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_XMLHTTP", fileName, 1);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            string[] columnNames = dtData.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName).
                                              ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dtData.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                ToArray();
                sb.AppendLine(string.Join(",", fields));
            }


            File.WriteAllText("CSV/test.csv", sb.ToString());
        }

    }
}
