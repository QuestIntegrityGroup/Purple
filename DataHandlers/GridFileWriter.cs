using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PurpleLib;


namespace Purple.DataHandlers
{
    class GridFileWriter
    {
        //This class will be used to build files with data contained in a DataGrid
        private List<UIA_ElementInfo> _cachedElements;
        public List<UIA_ElementInfo> ListofElements {set { _cachedElements = value; }}
        private StreamWriter streamWriter;
        
        //Constants for PurpleElement Types
        private const string PURUNKNOWN = "UnknownType";
        private const string PURBUTTON = "PurpleButton";
        private const string PURTEXTBOX = "PurpleTextBox";
        private const string PURWINDOW = "PurpleWindow";
        private const string VARLABEL = "var";
        private const string CREATENEW = " = new ";
        private int varnum = 0;

        private const string COMMENTLINE =
            "//Elements found using Purple UI - verify Golem.Purple settings match PurpleUI app.config before using\n\n";


        private void BuildFile(string filename, bool locatorsOnly = false)
        {
            streamWriter = new StreamWriter(filename, false);
            if (!locatorsOnly)
            {
                streamWriter.WriteLine(COMMENTLINE);
                streamWriter.WriteLine(" ");
            }
            string outputString = "";

            for (int i = 0; i < _cachedElements.Count; i++)
            {
                if (!locatorsOnly)
                {
                    string ElementType = _cachedElements[i].elementData()[4];
                    outputString = PURUNKNOWN + " " + VARLABEL + i + CREATENEW + PURUNKNOWN + "(\"" +
                                          _cachedElements[i].elementData()[2] + "\", \"" +
                                          _cachedElements[i].elementData()[5] + "\");";

                    if (ElementType == "button")
                    {
                        outputString = outputString.Replace(PURUNKNOWN, PURBUTTON);
                    }
                    if (ElementType == "textbox")
                    {
                        outputString = outputString.Replace(PURUNKNOWN, PURTEXTBOX);
                    }
                    if (ElementType == "window")
                    {
                        outputString = outputString.Replace(PURUNKNOWN, PURWINDOW);
                    }
                }
                else
                {
                    outputString = "\"" + _cachedElements[i].elementData()[5] + "\"";
                }

                streamWriter.WriteLine(outputString);

            }
            
            

            streamWriter.Close();

        }

        public void SaveFile(bool locatorOnly)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.Filter = "Text Files (*.txt)|*.txt";
            svd.FilterIndex = 1;
            svd.RestoreDirectory = true;

            if(svd.ShowDialog() == DialogResult.OK)
            {
                BuildFile(svd.FileName, locatorOnly);
            }


        }
    }
}
