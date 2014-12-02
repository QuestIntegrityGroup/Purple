using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using Condition = System.Windows.Automation.Condition;


namespace PurpleLib
{
    public class UIA_ElementInfo 
    {
        private AutomationElement _uiaElement;
        private Point _ElementLocation;
        private String _ElementName;
        private String _ElementParent;
        private String _ElementAutomationID;
        private String _ElementType;
        private String _PurplePath;
        private PurplePath _locator;
        private string _patterns;
        private bool _isenabled = false;
        private bool _isoffscreen = false;
        private bool _iskeyboard = false;
        private string _ProcessID = "";
        
        private List<UIA_ElementInfo> _children = new List<UIA_ElementInfo>();
        #region accessors
        public List<UIA_ElementInfo> Children
        {
            get { return _children; }
            set { _children = value; }
        }
        public String ProcessID
        {
            get { return _ProcessID; }
            set { _ProcessID = value; }
        }
        public String Patterns
        {
            get
            {
                BuildElementPatterns();
                return _patterns;
            }
        }

        public String Name
        {
            get
            {
                if (_ElementName == "")
                {
                    _ElementName = "<Blank>";
                }
                return _ElementName;
            }
        }

        public AutomationElement AElement
        {
            get { return _uiaElement; }
        }

        public String Purplepath
        {
            get
            {
                _PurplePath = _locator.getPurplePath(_uiaElement);
                return _PurplePath;
            }
        }

        public Point ElementLocation{get { return _ElementLocation; }}

        public bool IsEnabled{get { return _isenabled; }}
        public bool IsKeyboard{get { return _iskeyboard; }}
        public bool IsOffscreen{get { return _isoffscreen; }}

        #endregion

        public UIA_ElementInfo(Point loc, AutomationElement element, PurplePath locator)
        {
            _uiaElement = element;
            _ElementLocation = loc;
            _ElementName = _uiaElement.Current.Name;
            _ElementAutomationID = _uiaElement.Current.AutomationId;
            _ElementType = _uiaElement.Current.LocalizedControlType;
            _PurplePath = locator.getPurplePath(element);
            _locator = locator;
            
        }

        public UIA_ElementInfo(AutomationElement element, PurplePath locator)
        {
            _uiaElement = element;
            _ElementName = element.Current.Name;
            _locator = locator;
        }

        public string[] Headers()
        {
            string[] headerRow = new string[6];
            headerRow[0] = "X";
            headerRow[1] = "Y";
            headerRow[2] = "Name";
            headerRow[3] = "Automation ID";
            headerRow[4] = "Type";
            headerRow[5] = "PurplePath";

            return headerRow;
        }

        public string[] elementData()
        {

            string[] data = new string[6];
            data[0] = _ElementLocation.X.ToString();
            data[1] = _ElementLocation.Y.ToString();
            data[2] = _ElementName;
            data[3] = _uiaElement.Current.AutomationId;
            data[4] = _uiaElement.Current.LocalizedControlType;
            data[5] = _PurplePath;
            return data;
        }

        public void BuildNextLevel()
        {
            if (_locator.HasChildren(_uiaElement))
            {
                List<AutomationElement> childelements = _locator.GetChildren(_uiaElement);
                for (int x = 0; x < childelements.Count; x++)
                {
                    _children.Add(new UIA_ElementInfo(childelements[x], _locator));
                }
            }
        }

        public void setfocus()
        {
            _uiaElement.SetFocus();
        }

        private void BuildElementPatterns()
        {
            _patterns = "AvailablePatterns:\n";
            AutomationPattern[] automationPatterns = _uiaElement.GetSupportedPatterns();
            foreach (var automationPattern in automationPatterns)
            {
                _patterns += automationPattern.ProgrammaticName + "\n Pattern Name: ";
                _patterns += Automation.PatternName(automationPattern) + "\n";
            }
            BuildPropertyValues();
        }

        private void BuildPropertyValues()
        {
            _isenabled = _uiaElement.Current.IsEnabled;
            _iskeyboard = _uiaElement.Current.IsKeyboardFocusable;
            _isoffscreen = _uiaElement.Current.IsOffscreen;
            _ProcessID = _uiaElement.Current.ProcessId.ToString();
            _patterns += "\nProperty Values:\n";

            _patterns += "Content:         " + _uiaElement.Current.IsContentElement + "\n";
            _patterns += "Control:         " + _uiaElement.Current.IsControlElement + "\n";
            _patterns += "Password:        " + _uiaElement.Current.IsPassword + "\n";
            _patterns += "Required:        " + _uiaElement.Current.IsRequiredForForm + "\n";
            _patterns += "Type:            " + _uiaElement.Current.LocalizedControlType + "\n";
            _patterns += "WinHWND:         " + _uiaElement.Current.NativeWindowHandle + "\n";
        }


        public void patterns()
        {
            AutomationProperty[] props = _uiaElement.GetSupportedProperties();
            AutomationProperty prop = props[1];

            AutomationPattern[] patterns = _uiaElement.GetSupportedPatterns();
            AutomationPattern pattern = patterns[0];
        }

       
    }
}
