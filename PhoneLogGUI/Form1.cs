using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;

namespace PhoneLogGUI
{			
	public partial class Form1 : Form
	{
		const string appname = "phonelog";
		const int appmajor = 1;
		const int appminor = 1;
		const int appbuild = 0;

		[DllImport("coredll.dll")]
		static extern int SHGetSpecialFolderPath(IntPtr hwndOwner, StringBuilder lpszPath, int nFolder, int fCreate);
		[DllImport("coredll.dll")]
		static extern int SHCreateShortcut(StringBuilder szShortcut, StringBuilder szTarget);

		const int CSIDL_PROGRAMS = 2;  // \Windows\Start Menu\Programs
		const int CSIDL_STARTUP = 7; // \Windows\Startup

		ToolHelp.Process[] procs = null;

		public Form1()
		{
			InitializeComponent();
			
			procs = ToolHelp.Process.GetProcesses();
			foreach (ToolHelp.Process proc in procs)
			{
				if (proc.ProcessName.ToLower() == "phonelogservice.exe")
				{
					startStopMenu.Text = "Stop";
					break;
				}
			}

			StringBuilder programs = new StringBuilder(255);
			SHGetSpecialFolderPath((IntPtr)0, programs, CSIDL_STARTUP, 0);
			StringBuilder shortcutLocation = new StringBuilder(Path.Combine(programs.ToString(), "LogPhoneService.lnk"));

			if (!System.IO.File.Exists(shortcutLocation.ToString()))
				cbRunOnStart.Checked = false;
			else
				cbRunOnStart.Checked = true;

			cbLogCompleted.Checked = Convert.ToBoolean(Convert.ToInt16(PhoneLogRegKey.GetValue("LogCompleted", "1")));
			cbLogMissed.Checked = Convert.ToBoolean(Convert.ToInt16(PhoneLogRegKey.GetValue("LogMissed", "1")));
		}

		private RegistryKey PhoneLogRegKey
		{
			get
			{
				RegistryKey lm = Registry.CurrentUser;				
				RegistryKey softRk = lm.OpenSubKey("SOFTWARE\\Infinityball\\PhoneLog",true);
				if (softRk == null)
					softRk = lm.CreateSubKey("SOFTWARE\\Infinityball\\PhoneLog");
				return softRk;
			}
		}

		private void startStopMenu_Click(object sender, EventArgs e)
		{
			try
			{
				if (startStopMenu.Text == "Stop")
				{
					procs = ToolHelp.Process.GetProcesses();
					foreach (ToolHelp.Process proc in procs)
					{
						if (proc.ProcessName.ToLower().IndexOf("phonelogservice") >= 0)
						{
							Process cfProc = Process.GetProcessById(proc.Handle.ToInt32());
							if (cfProc != null && cfProc.Responding)
							{
								cfProc.Kill();
								startStopMenu.Text = "Start";
							}
						}
					}
				}
				else
				{					
					Process pl = new Process();
					pl.StartInfo.FileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\PhoneLogService.exe";
					pl.StartInfo.UseShellExecute = true;
					pl.Start();
					startStopMenu.Text = "Stop";
					Activate();
				}
			}
			catch (System.Exception ex)
			{
				Console.Write(ex.Message);
				MessageBox.Show("Unable to start/stop the PhoneLog service, check to see if PhoneLog.exe exist in the installation folder", "Error Starting Service");
			}
		}

		private void cbLogCompleted_CheckStateChanged(object sender, EventArgs e)
		{
			PhoneLogRegKey.SetValue("LogCompleted", Convert.ToInt16(cbLogCompleted.Checked).ToString());
		}

		private void cbLogMissed_CheckStateChanged(object sender, EventArgs e)
		{
			PhoneLogRegKey.SetValue("LogMissed", Convert.ToInt16(cbLogMissed.Checked).ToString());			
		}

		private void cbRunOnStart_CheckStateChanged(object sender, EventArgs e)
		{
			StringBuilder programs = new StringBuilder(255);
			SHGetSpecialFolderPath((IntPtr)0, programs, CSIDL_STARTUP, 0);
			StringBuilder shortcutLocation = new StringBuilder(Path.Combine(programs.ToString(), "LogPhoneService.lnk"));

			if (cbRunOnStart.Checked)
			{
				if (!System.IO.File.Exists(shortcutLocation.ToString()))
				{
					StringBuilder shortcutTarget = new StringBuilder("\"" + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\PhoneLogService.exe" + "\"");
					SHCreateShortcut(shortcutLocation, shortcutTarget);
				}
			}
			else
			{
				if (!System.IO.File.Exists(shortcutLocation.ToString()))
				{
					System.IO.File.Delete(shortcutLocation.ToString());
				}
			}
		}

		private void updateCheckMenu_Click(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			Cursor.Show();
			try
			{
				string downloadURL = "http://www.infinityball.com/support/update.xml";
				downloadURL = PhoneLogRegKey.GetValue("UpdateURL", downloadURL).ToString();				
				if (downloadURL != null && downloadURL.Length > 0)
				{
					string content = "";
					System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(downloadURL);
					System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();
					System.IO.StreamReader rdr = new System.IO.StreamReader(res.GetResponseStream());
					content = rdr.ReadToEnd();
//					content = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><updateinfo><download name=\"PhoneLog\" action=\"cab\"><version maj=\"1\" min=\"1\" bld=\"0\"/>http://www.infinityball.com/software/phonelog.cab</download></updateinfo>";
					rdr.Close();
					res.Close();

					XmlDocument xmlDoc = new XmlDocument();
					xmlDoc.LoadXml(content);
					
					XmlElement ui = xmlDoc["updateinfo"];
					if (ui != null)
					{
						foreach (XmlNode appui in ui.ChildNodes)
						{
							if (appui.Attributes["name"] != null)
							{
								if (appui.Attributes["name"].Value.ToLower() == "phonelog")
								{
									int maj = 0;
									int min = 0;
									int bld = 0;

									bool update = false;

									XmlNode verui = appui.FirstChild;
									if (verui != null)
									{
										if(verui.Attributes["maj"] != null) maj = Convert.ToInt16(verui.Attributes["maj"].Value);
										if (verui.Attributes["min"] != null) min = Convert.ToInt16(verui.Attributes["min"].Value);
										if (verui.Attributes["bld"] != null) bld = Convert.ToInt16(verui.Attributes["bld"].Value);
									}

									if (maj > appmajor)
										update = true;
									else
										if (min > appminor)
											update = true;
										else
											if (bld > appbuild)
												update = true;

									if (update && appui.InnerText.Length > 0)
									{
										if (MessageBox.Show("An update is available, would you like to download it?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
										{
											System.Diagnostics.Process.Start(appui.InnerText, "");
										}
									}
									else
										MessageBox.Show("No updates are available at this time.");
								}
							}
						}
					}
				}
			}
			catch(System.Exception ex)
			{
				MessageBox.Show("Unable to check for updates.");
			}
			Cursor.Current = Cursors.Default;
			Cursor.Hide(); 
			
		}
	}
}