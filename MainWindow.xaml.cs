
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using MouseKeyboardLibrary;
using Purple.DataHandlers;
using System.Windows.Forms;
using Purple.ViewControllers;
using PurpleLib;
using Control = System.Windows.Forms.Control;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;


namespace Purple
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitMouseEventHandlers();
        }

        //Initialize View Controllers
        private MainScreen_VC mainScreenVc = new MainScreen_VC();
        public List<UIA_ElementInfo> Elements;
        

        #region MouseHandlers Code to handle mouse driven events on the MainScreen
        //For some reason i couldn't pass around MouseEventArgs properly.  Bah! --Had to include it in the main form class.  
        private MouseHook mouseHook = new MouseHook();
        private int _mouseXLoc;
        private int _mouseYLoc;

        private void InitMouseEventHandlers()
        {
            mouseHook.MouseMove += mouseHook_MouseMove;
            mouseHook.MouseDown += mouseHook_MouseDown;
            mouseHook.MouseUp += mouseHook_MouseUp;
        }

        private void mouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            //Stub for mouse up action if needed.
        }

        public void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            mouseHook.Stop();
            GatherElementDetail();
        }

        private void mouseHook_MouseMove(object sender, MouseEventArgs e)
        {
            Xcord.Text = e.X.ToString();
            YCord.Text = e.Y.ToString();
        }

        private void GatherElementDetail()
        {
            //This function is called from the mouseHook_MouseDown() function
            if (!mouseHook.IsStarted)
            {
                mainScreenVc.AddPoint(new Point(double.Parse(Xcord.Text), double.Parse(YCord.Text)));
                mainScreenVc.GetElementInfo(ref purplepathtextbox);
                mainScreenVc.SetElementDetail(ref purplepathtextbox, ref AvailableInfo_textbox, ref IsEnabled_Checkbox, ref IsKeyboardFocusable_checkbox, ref IsOffscreen_checkbox,
                    ref ProcessID_textbox);
            }
        }
        #endregion

        #region FormLoad and Exit events
        private void Purple_MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //This function fires when the window is first loaded
            
            Elements = mainScreenVc.BuildApplicationTree();
            ApplicationTree.ItemsSource = Elements;
            ApplicationTextBox.Text = mainScreenVc.getConfigAppName();
            //ApplicationTree.AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(mainScreenVc.BuildChildTree));
            
        }

        private void Purple_MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            mainScreenVc.SaveSettings_OnExit();
        }
        #endregion
        
        #region Options Code to handle options expander
        
        #endregion
        
        #region MotherFuckingTreeView Event handlers for the goddamn treeview
        private void ApplicationTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UIA_ElementInfo thing = (UIA_ElementInfo) ApplicationTree.SelectedItem;
            var what = ApplicationTree.Items.CurrentPosition;
            ApplicationTree_OnExpanded(sender, e);
        }
       
        private void ApplicationTree_OnExpanded(object sender, RoutedEventArgs e)
        {
            mainScreenVc.BuildChildTree((UIA_ElementInfo)ApplicationTree.SelectedValue, sender, e);
        }

        private void TreeItem_GetInfo(object sender, RoutedEventArgs e)
        {
            UIA_ElementInfo thing = (UIA_ElementInfo)ApplicationTree.SelectedItem;
            if (thing != null)
            {
                mainScreenVc.FoundElement = thing;
                mainScreenVc.SetElementDetail(ref purplepathtextbox, ref AvailableInfo_textbox, ref IsEnabled_Checkbox, ref IsKeyboardFocusable_checkbox, ref IsOffscreen_checkbox,
                    ref ProcessID_textbox);
            }
        }
        #endregion

        #region Buttons on Main Form

        private void Cursor_Button_Click(object sender, RoutedEventArgs e)
        {
            purplepathtextbox.Text = "PurplePath";
            mouseHook.Start();
        }

        private void Add_Element_Selected_Click(object sender, RoutedEventArgs e)
        {
            if (!mouseHook.IsStarted)
            {
                mainScreenVc.SelectedElements_AddRow(ref CachedElementsGrid);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainScreenVc.TestPurplePath(purplepathtextbox.Text);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(purplepathtextbox.Text);
        }

        private void ClearCache_button_Click(object sender, RoutedEventArgs e)
        {
            mainScreenVc.ClearCachedElements(ref CachedElementsGrid);
        }

        private void ShowElement_Click(object sender, RoutedEventArgs e)
        {
            mainScreenVc.drawRectangle();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationTextBox.Text.Contains(ConfigurationManager.AppSettings["DefaultStartScreen"]))
            {
                Elements = mainScreenVc.RefreshTreeView(ConfigurationManager.AppSettings["DefaultStartScreen"]);
                ApplicationTextBox.Text = ConfigurationManager.AppSettings["DefaultStartScreen"];
            }
            else
            {
                Elements = mainScreenVc.RefreshTreeView(ApplicationTextBox.Text);
            }
            ApplicationTree.Items.Refresh();
        }
        
        private void BuildCache_button_Click(object sender, RoutedEventArgs e)
        {
            //stub for build cache button
            //This needs to use a function on the MainScreen_VC to pass the List of UIA_ELementInfo to the gridfilewriter
            bool locatorsOnly = (bool) ckbx_Locators_Only.IsChecked;
            mainScreenVc.BuildCacheFile(locatorsOnly);
        }
        #endregion

    }
}
