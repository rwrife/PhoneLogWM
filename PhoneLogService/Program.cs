using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsMobile.PocketOutlook;

namespace PhoneLog
{
	class Program
	{
		static void Main(string[] args)
		{
			PhoneEventNotifier pen = new PhoneEventNotifier();
			pen.MissedCall += new PhoneEventNotifier.MissedCallEventHandler(pen_MissedCall);
			pen.FinishedCall += new PhoneEventNotifier.FinishCallEventHandler(pen_FinishedCall);

			while (true)
			{
				System.Threading.Thread.Sleep(5000);
			}
		}

		private static void pen_FinishedCall(object sender, FinishedCallEventArgs e)
		{
			OutlookSession s = new OutlookSession();
			Appointment app = new Appointment();
			app.ReminderSet = false;
			app.Start = e.StartTime;
			app.End = e.EndTime;
			if (e.CallerName.Length > 0)
				app.Subject = "Talked to " + e.CallerName;
			else
				app.Subject = "Talked to " + e.CallerNumber;
			app.Body = e.CallerNumber;
			s.Appointments.Items.Add(app);
		}

		private static void pen_MissedCall(object sender, MissedCallEventArgs e)
		{
			OutlookSession s = new OutlookSession();
			Appointment app = new Appointment();			
			app.ReminderSet = false;
			app.Start = DateTime.Now;
			app.Duration = TimeSpan.FromMinutes(0);
			if(e.CallerName.Length > 0)
				app.Subject = "Missed call from " + e.CallerName;
			else
				app.Subject = "Missed call from " + e.CallerNumber;
			app.Body = e.CallerNumber;
			s.Appointments.Items.Add(app);
		}
	}
}
