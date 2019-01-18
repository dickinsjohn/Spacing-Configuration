using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;

namespace SpacingConfiguration
{
    public partial class SpecsList : System.Windows.Forms.Form
    {
        string location = null;
        string discipline = null;

        //method to get the specs list instance
        public void GetDiscipline(string disci)
        {
            discipline = disci;
        }
                
        public SpecsList()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                this.DialogResult = DialogResult.Abort;
            }
        }

        private void SpecsList_Load(object sender, EventArgs e)
        {
            location=System.Reflection.Assembly.GetExecutingAssembly().Location;

            //convert the active file path into directory name
            if (File.Exists(location))
            {
                location = new FileInfo(location).Directory.ToString();
            }

            //create the file in the directory if not existing
            location = location + @"\Specifications.txt";

            if (!System.IO.File.Exists(location))
            {
                System.IO.File.CreateText(location);
            }

            AddToDataGridView();
        }

        //method to add data from specificaion file into datagridview
        private void AddToDataGridView()
        {
            // New table.
            DataTable table = new DataTable();

            //********
            //ADD THE CODE HERE FOR MORE DISCIPLINES AND THEIR SPECIFICATION FILES
            //********

            //for FEMS the pipes are going to be selected
            //its specifications will be diameter and spacing
            //else return dialogue result as aborted
            if (discipline == "FEMS")
            {
                table.Columns.Add("Diameter", typeof(string));
                table.Columns.Add("Spacing", typeof(string));
            }
            else
            {
                if (discipline != "")
                {
                    TaskDialog.Show("Coding Limitation!", "Sorry. This program is yet to be modified for your discipline.");                    
                }
                this.DialogResult = DialogResult.Abort;
                return;
            }
                        
            //read all the contents of the file into a string array
            string[] fileContents = File.ReadAllLines(location);

            string[] splitLines = null;

            for (int i = 0; i < fileContents.Count(); i++)
            {
                splitLines = fileContents[i].Split(' ');

                if(splitLines[0]!=""&&splitLines[1]!="")
                    table.Rows.Add(splitLines);
            }

            //set the datatable as the datasource of the datagrid view
            dataGridView1.DataSource = table;
        }


        //method to add data from datagridview to textfile
        private void WriteBackToFile()
        {
            //convert all data in the datagridview into datatable
            DataTable datatable = new DataTable();

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                datatable.Columns.Add(dataGridView1.Columns[i].Name);
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataRow datarw = datatable.NewRow();
                for (int iCol = 0; iCol < dataGridView1.Columns.Count; iCol++)
                {
                    datarw[iCol] = row.Cells[iCol].Value;
                }
                datatable.Rows.Add(datarw);
            }

            //open a new stream reader to write the datatable into the file 
            //append data is set to false
            StreamWriter file = new StreamWriter(location,false);

            //put all data from teh datatable into the file
            foreach (DataRow row in datatable.Rows)
            {
                bool firstCol = true;
                foreach (DataColumn col in datatable.Columns)
                {
                    if (!firstCol)
                        file.Write(' ');
                    if (row[col].ToString() != "")
                        file.Write(row[col].ToString());
                    firstCol = false;
                }
                file.WriteLine();
                file.Flush();
            }
            file.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WriteBackToFile();
            this.DialogResult = DialogResult.OK;
        }
    }
}
