using Microsoft.Extensions.Logging;
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

        public UIMainScreen()
        {
            //Create the logger
            InitializeComponent();
            Logger = new ODL.Common.LogHandler();
            Logger.DebugMode = true;
            Logger.LogTableUpdated += RefreshLogGrid;
            Logger.LogInformation("Welcome to OpenDataLoader -- please make sure your database connection looks right.");

            //Load config from json


            //Populate dropdown for dbtype
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
            dgMessages.Columns[0].Width = 120;
            dgMessages.Columns[1].Width = 95;
            dgMessages.Columns[2].Width = 600;
            dgMessages.Update();
            dgMessages.Refresh();
        }

        private void cmbFileSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbFileType.DisplayMember = "Key";
            cmbFileType.ValueMember = "Value";
            cmbFileType.DataSource = cmbFileSource.SelectedValue;
        }
    }
}
