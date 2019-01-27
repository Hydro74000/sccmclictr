﻿using System;
using System.Windows;
using System.Diagnostics;

using sccmclictr.automation;
using System.Windows.Controls.Ribbon;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AgentActionTool_FEPFullScan : System.Windows.Controls.UserControl
    {
        public SCCMAgent oAgent;
        public AgentActionTool_FEPFullScan()
        {
            InitializeComponent();
            btExplore.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
            if (!btExplore.IsEnabled)
            {
                gExplore.ToolTip = "Please make a donation to get access to this feature !";
            }

            foreach(string sPath in Properties.Settings.Default.Folders)
            {
                try
                {
                    RibbonButton bR = new RibbonButton();
                    bR.Label = sPath;
                    bR.Tag = sPath;
                    bR.SmallImageSource = new BitmapImage(new Uri(@"/Plugin_Explorer;component/Images/shell32.dll_I010b_0409.ico", UriKind.Relative));
                    bR.ToolTip = sPath;
                    bR.Click += btC_Click;
                    btExplore.Items.Add(bR);
                }
                catch { }
            }
            
        }

        private void btC_Click(object sender, RoutedEventArgs e)
        {
            try{

                if (((RibbonButton)sender).Tag != null)
                {
                    Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                    System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                    oAgent = (SCCMAgent)pInfo.GetValue(null, null);
                    string sHost = oAgent.TargetHostname;

                    string sTag = ((RibbonButton)sender).Tag.ToString();
                    string sShare = "";
                    switch(sTag)
                    {
                        case "C":
                            sShare = "C$";
                            break;
                        case "Admin":
                            sShare = "Admin$";
                            break;
                        case "WBEM":
                            sShare = @"Admin$\System32\wbem";
                            break;
                        case "ccmsetup":
                            sShare = @"Admin$\ccmsetup\logs";
                            break;
                        case "CCMLOGS":
                            if (oAgent.isConnected)
                                sShare = oAgent.Client.AgentProperties.LocalSCCMAgentLogPath.Replace(':', '$');
                            else
                                sShare = @"Admin$\ccm\logs";
                            break;
                        default:
                            sShare = sTag;
                            break;

                    }


                    //Connect IPC$ if not already connected (not needed with integrated authentication)
                    if (!oAgent.ConnectIPC)
                        oAgent.ConnectIPC = true;

                    Process Explorer = new Process();
                    Explorer.StartInfo.FileName = "Explorer.exe";
                    Explorer.StartInfo.Arguments = @"\\" + oAgent.TargetHostname + @"\" + sShare;
                    Explorer.Start();
                }
            }
            catch{}
        }


    }
}
