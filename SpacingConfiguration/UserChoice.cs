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
    public partial class UserChoice : System.Windows.Forms.Form
    {
        public List<familyData> f_data = new List<familyData>();

        string fileLocation = null;


        //method to write all configuration data to a text file
        private void WriteConfig()
        {
            //open configuration file
            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

            //convert the active file path into directory name
            if (File.Exists(assemblyLocation))
            {
                assemblyLocation = new FileInfo(assemblyLocation).Directory.ToString();
            }

            fileLocation = assemblyLocation + @"\Specifications.txt";
            //variables for the configuration file
            string treeSelection = null;
            string discipline = null;
            string offset = null;
            string minSpacing = null;
            string supportType = null;

            if (File.Exists(fileLocation) && comboBox1.Text != "" && textBox1.Text != "" && textBox2.Text != "" && comboBox2.Text != "")
            {
                treeSelection = "SelectedFamily:" + ' ' + label7.Text;
                discipline = "Discipline:" + ' ' + comboBox1.Text;
                offset = "Offest:" + ' ' + textBox1.Text;
                minSpacing = "Spacing:" + ' ' + textBox2.Text;
                supportType = "Support Type:" + ' ' + comboBox2.Text;
                fileLocation = "File Location:" + ' ' + fileLocation;

                //create the file in the directory if not existing
                assemblyLocation = assemblyLocation + @"\Configuration.txt";

                if (!System.IO.File.Exists(assemblyLocation))
                {
                    System.IO.File.CreateText(assemblyLocation);
                }

                //start a new file reader
                StreamWriter file = new StreamWriter(assemblyLocation);
                file.WriteLine(treeSelection);
                file.WriteLine(discipline);
                file.WriteLine(offset);
                file.WriteLine(minSpacing);
                file.WriteLine(supportType);
                file.WriteLine(fileLocation);
                file.Close();
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Don't leave fields blank and check spacing details!");                
            }
        }


        //method to populate the tree view with the data from project browser
        private void PopulateTreeView()
        {
            //first sort the list based on the category of the families
            f_data = f_data.OrderBy(r => r.f_category).ToList();

            //set the current group as the first family instances category
            string currentGroup = f_data.First().f_category;

            var treeNodes = new List<TreeNode>();
            var childNodes = new List<TreeNode>();

            foreach (familyData tempData in f_data)
            {
                //if the current family data is of same category then add it to child nodes list
                if (currentGroup == tempData.f_category)
                    childNodes.Add(new TreeNode(tempData.f_name));
                else
                {
                    //if family category is different and childnodes has some data
                    //then add this data to the treenodes list and
                    //empty the childnodes list
                    if (childNodes.Count > 0)
                    {
                        treeNodes.Add(new TreeNode(currentGroup, childNodes.ToArray()));
                        childNodes = new List<TreeNode>();
                    }
                    //add the current familyinstance into the childnode
                    //set the category as the current category
                    childNodes.Add(new TreeNode(tempData.f_name));
                    currentGroup = tempData.f_category;
                }
            }
            //if childnodes has elements add it to tree nodes
            if (childNodes.Count > 0)
            {
                treeNodes.Add(new TreeNode(currentGroup, childNodes.ToArray()));
            }
            //add all data in the tree nodes into the treeview1
            treeView1.Nodes.AddRange(treeNodes.ToArray());
        }


        public UserChoice()
        {
            InitializeComponent();
        }


        public void GetData(List<familyData> data)
        {
            f_data = data;
            //call populate tree view method to populate the tree view with the obtained list
            PopulateTreeView();
        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = !char.IsNumber(e.KeyChar);
            if (e.KeyChar == 13)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            if (e.KeyChar == 27)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }
        

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = !char.IsNumber(e.KeyChar);
            if (e.KeyChar == 13)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            if (e.KeyChar == 27)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }
        

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }


        private void comboBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }
        

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }


        private void button1_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }


        private void button2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }


        private void button3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogueResult = new DialogResult();
            
            //create instance of the form
            using (SpecsList SpecsListInstance = new SpecsList())
            {
                this.Hide();

                //set the selected discipline in the specsListInstance
                SpecsListInstance.GetDiscipline(comboBox1.Text);

                dialogueResult = SpecsListInstance.ShowDialog();
                
                if (dialogueResult == DialogResult.OK)
                {
                    this.Show();
                    SpecsListInstance.Close();
                }
                if (dialogueResult == DialogResult.Abort)
                {
                    this.Show();
                    SpecsListInstance.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WriteConfig();
        }
        
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            label7.Text = treeView1.SelectedNode.Text;
        }

    }
}
