using Microsoft.Extensions.Logging;
using ODL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDataLoader
{
    public partial class UIMainScreen : Form
    {
        internal ODL.Common.LogHandler Logger;
        internal ODL.Common.DBConnectionDetails ConnectionDetails;

        public UIMainScreen()
        {
            //Create the logger
            InitializeComponent();
            Logger = new ODL.Common.LogHandler();
            Logger.DebugMode = true;
            Logger.LogTableUpdated += RefreshLogGrid;
            Logger.LogInformation("Welcome to OpenDataLoader.");
            if (Logger.DebugMode) Logger.LogDebug("Created Logger");

            //Populate dropdown for dbtype
            if (Logger.DebugMode) Logger.LogTrace("Creating Dropdown Selections");
            List<KeyValuePair<String, String>> lstDBTypes = new List<KeyValuePair<String, String>>();
            Array DBtypes = Enum.GetValues(typeof(ODL.Common.SupportedDatabases));
            foreach (ODL.Common.SupportedDatabases _entry in DBtypes)
            {
                lstDBTypes.Add(new KeyValuePair<String, String>(_entry.ToString(), ((int)_entry).ToString()));
            }
            cmbDatabaseType.DisplayMember = "Key";
            cmbDatabaseType.ValueMember = "Value";
            cmbDatabaseType.DataSource = lstDBTypes;

            //Populate the dropdowns for the File Source and File Type
            cmbFileSource.DisplayMember = "Key";
            cmbFileSource.ValueMember = "Value";
            cmbFileSource.DataSource = ConvolutedWayToMakeNestedDropdowns();

            if (Logger.DebugMode) Logger.LogTrace("Loading DBConfig from json (if available)");
            //Load config from json
            ConnectionDetails = ODL.Common.DatabaseUtils.Load(Logger);
            txtDBUsername.Text = ConnectionDetails.DBUsername;
            txtDBPassword.Text = ConnectionDetails.DBPassword;
            txtDBServer.Text = ConnectionDetails.DBServer;
            txtDBCatalog.Text = ConnectionDetails.DBCatalog;
            cmbDatabaseType.Text = ConnectionDetails.DBType.ToString();

        }

        /// <summary>
        /// Honestly I'm just having fun messing with this, but it's going to return a key value pair of key value pairs that 
        /// we can bind to the FileType dropdown when the FileSource dropdown gets populated so we only have to add them in
        /// one place-- watch this :)  [and check out how elegant it makes cmbFileSource_SelectedIndexChanged]
        /// </summary>
        /// <returns>Stuff to make nested dropdowns</returns>
        private List<KeyValuePair<String, List<KeyValuePair<String, String>>>> ConvolutedWayToMakeNestedDropdowns()
        {
            List<KeyValuePair<String, List<KeyValuePair<String, String>>>> MasterDropdown = 
                new List<KeyValuePair<String, List<KeyValuePair<String, String>>>>();

            MasterDropdown.Add(
                new KeyValuePair<String, List<KeyValuePair<String, String>>>("NYS Education Department", 
                    new List<KeyValuePair<String, String>>()
                        {
                            new KeyValuePair<String, String> ("NYS Graduation Rate", "NYSGradeRate"),
                            new KeyValuePair<String, String> ("NYS Teacher Evaluations", "NYSTeacherEval"),
                            new KeyValuePair<String, String> ("NYS Report Cards", "NYSReportCards"),
                            new KeyValuePair<String, String> ("NYS 3-8 Assessments", "NYS3to8Assessments")
                        } 
                    )
                );
            MasterDropdown.Add(
                new KeyValuePair<String, List<KeyValuePair<String, String>>>("US Census",
                    new List<KeyValuePair<String, String>>()
                        {
                            new KeyValuePair<String, String> ("DP05", "DP05"),
                            new KeyValuePair<String, String> ("S0901", "S0901"),
                            new KeyValuePair<String, String> ("S0501", "S0501"),
                            new KeyValuePair<String, String> ("S1601", "S1601"),
                            new KeyValuePair<String, String> ("DP03", "DP03"),
                            new KeyValuePair<String, String> ("B17020", "B17020"),
                            new KeyValuePair<String, String> ("C14006", "C14006"),
                            new KeyValuePair<String, String> ("S1702", "S1702"),
                            new KeyValuePair<String, String> ("S1810", "S1810"),
                            new KeyValuePair<String, String> ("S2703", "S2703"),
                            new KeyValuePair<String, String> ("S2704", "S2704"),
                            new KeyValuePair<String, String> ("S2801", "S2801")
                        }
                    )
                );
            MasterDropdown.Add(
                new KeyValuePair<String, List<KeyValuePair<String, String>>>("NYS Human Services",
                    new List<KeyValuePair<String, String>>()
                        {
                            new KeyValuePair<String, String> ("Child Care Regulated Programs", "NYSChildCare")
                        }
                    )
                );
            MasterDropdown.Add(
                new KeyValuePair<String, List<KeyValuePair<String, String>>>("City of Rochester Data",
                    new List<KeyValuePair<String, String>>()
                        {
                            new KeyValuePair<String, String> ("Police Crime Data", "ROCNYCrime"),
                            new KeyValuePair<String, String> ("Rochester Open Data", "ROCOpenData")
                        }
                    )
                );

            return MasterDropdown;
        }

        public void RefreshLogGrid(object sender, bool refresh)
        {
            dgMessages.DataSource = Logger.LogRecords;
            dgMessages.Columns[0].Width = (int)(dgMessages.Width * .15);
            dgMessages.Columns[1].Width = (int)(dgMessages.Width * .10);
            dgMessages.Columns[2].Width = (int)(dgMessages.Width * .65);
            dgMessages.Update();
            dgMessages.Refresh();
        }

        private void cmbFileSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Logger.DebugMode) Logger.LogTrace("Method: cmbFileSource_SelectedIndexChanged");
            cmbFileType.DisplayMember = "Key";
            cmbFileType.ValueMember = "Value";
            cmbFileType.DataSource = cmbFileSource.SelectedValue;
        }

        private void btnSaveDBInfo_Click(object sender, EventArgs e)
        {
            if (Logger.DebugMode) Logger.LogTrace("Method: btnSaveDBInfo_Click");
            DBConnectionDetails _tempObject = new DBConnectionDetails();
            _tempObject.DBUsername = txtDBUsername.Text;
            _tempObject.DBPassword = txtDBPassword.Text;
            _tempObject.DBServer = txtDBServer.Text;
            _tempObject.DBCatalog = txtDBCatalog.Text;            
            _tempObject.DBType = (SupportedDatabases)Enum.Parse(typeof(SupportedDatabases), cmbDatabaseType.Text);

            ODL.Common.DatabaseUtils.Save(_tempObject, Logger);
        }

        private void btnExitApp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void chkDebugMode_CheckedChanged(object sender, EventArgs e)
        {
            if (Logger.DebugMode) Logger.LogTrace("Method: chkDebugMode_CheckedChanged");
            if (chkDebugMode.Checked) Logger.DebugMode = true;
            if (!chkDebugMode.Checked) Logger.DebugMode = false;
        }

        private void btnLoadSelectedFile_Click(object sender, EventArgs e)
        {

        }
    }
}
