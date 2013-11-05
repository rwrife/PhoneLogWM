namespace PhoneLogGUI
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.MainMenu mainMenu1;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.startStopMenu = new System.Windows.Forms.MenuItem();
			this.cbRunOnStart = new System.Windows.Forms.CheckBox();
			this.cbLogMissed = new System.Windows.Forms.CheckBox();
			this.cbLogCompleted = new System.Windows.Forms.CheckBox();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.updateCheckMenu = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.Add(this.startStopMenu);
			this.mainMenu1.MenuItems.Add(this.menuItem1);
			// 
			// startStopMenu
			// 
			this.startStopMenu.Text = "Start";
			this.startStopMenu.Click += new System.EventHandler(this.startStopMenu_Click);
			// 
			// cbRunOnStart
			// 
			this.cbRunOnStart.Location = new System.Drawing.Point(4, 4);
			this.cbRunOnStart.Name = "cbRunOnStart";
			this.cbRunOnStart.Size = new System.Drawing.Size(233, 20);
			this.cbRunOnStart.TabIndex = 0;
			this.cbRunOnStart.Text = "Run on startup";
			this.cbRunOnStart.CheckStateChanged += new System.EventHandler(this.cbRunOnStart_CheckStateChanged);
			// 
			// cbLogMissed
			// 
			this.cbLogMissed.Location = new System.Drawing.Point(4, 31);
			this.cbLogMissed.Name = "cbLogMissed";
			this.cbLogMissed.Size = new System.Drawing.Size(233, 20);
			this.cbLogMissed.TabIndex = 1;
			this.cbLogMissed.Text = "Log missed calls";
			this.cbLogMissed.CheckStateChanged += new System.EventHandler(this.cbLogMissed_CheckStateChanged);
			// 
			// cbLogCompleted
			// 
			this.cbLogCompleted.Location = new System.Drawing.Point(4, 58);
			this.cbLogCompleted.Name = "cbLogCompleted";
			this.cbLogCompleted.Size = new System.Drawing.Size(233, 20);
			this.cbLogCompleted.TabIndex = 2;
			this.cbLogCompleted.Text = "Log completed calls";
			this.cbLogCompleted.CheckStateChanged += new System.EventHandler(this.cbLogCompleted_CheckStateChanged);
			// 
			// menuItem1
			// 
			this.menuItem1.MenuItems.Add(this.updateCheckMenu);
			this.menuItem1.Text = "Menu";
			// 
			// updateCheckMenu
			// 
			this.updateCheckMenu.Text = "Check For Updates";
			this.updateCheckMenu.Click += new System.EventHandler(this.updateCheckMenu_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(240, 268);
			this.Controls.Add(this.cbLogCompleted);
			this.Controls.Add(this.cbLogMissed);
			this.Controls.Add(this.cbRunOnStart);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "PhoneLog";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.MenuItem startStopMenu;
		private System.Windows.Forms.CheckBox cbRunOnStart;
		private System.Windows.Forms.CheckBox cbLogMissed;
		private System.Windows.Forms.CheckBox cbLogCompleted;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem updateCheckMenu;
	}
}

