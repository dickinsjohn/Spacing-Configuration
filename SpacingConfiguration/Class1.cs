using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Security_Check;

namespace SpacingConfiguration
{
    //Transaction assigned as automatic
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Automatic)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]


    //Creating an external command to provide supports
    public class SpacingConfiguration : IExternalCommand
    {
        //instances to store application and the document
        UIDocument m_document = null;

        bool security = false;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                //call to the security check method to check for authentication
                security = SecurityLNT.Security_Check();

                if (security == false)
                {
                    return Result.Succeeded;
                }

                //open  the active document in revit
                m_document = commandData.Application.ActiveUIDocument;

                DialogResult dialogueResult = new DialogResult();

                //create instance of the form
                using (UserChoice formInstance = new UserChoice())
                {
                    formInstance.GetData(RetrieveFamilies());
                    dialogueResult = formInstance.ShowDialog();
                    if (dialogueResult == DialogResult.OK||dialogueResult==DialogResult.Abort)
                        return Result.Succeeded;
                }

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Autodesk.Revit.UI.Result.Failed;
            }
            throw new NotImplementedException();
        }


        //method to retrieve all loaded families in the project
        public List<familyData> RetrieveFamilies()
        {
            FilteredElementCollector families = new FilteredElementCollector(m_document.Document);
            families.OfClass(typeof(Family));

            List<familyData> f_data = new List<familyData>();

            familyData f_instance = new familyData();

            foreach (Family fam in families)
            {
                f_instance.f_name = fam.Name;
                f_instance.f_category = fam.FamilyCategory.Name;
                f_data.Add(f_instance);
            }

            return f_data;
        }
    }

    //structure to store the family type and name loaded into the project
    public struct familyData
    {
        public string f_name;
        public string f_category;
    };

}
