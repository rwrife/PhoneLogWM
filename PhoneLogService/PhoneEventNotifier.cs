using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsMobile.Status;
using Microsoft.WindowsMobile.PocketOutlook;
using System.Threading;
using Microsoft.Win32;

namespace PhoneLog
{	
	public class PhoneEventNotifier
	{
		private int missedCalls = 0;
		private string callerName = "";
		private string callerNumber = "";
		private bool inCall = false;
		private DateTime startTime = DateTime.MinValue;
		Thread watchThread = null;

		private string lastCallerNumber = "";
		private Contact lastCaller = null;

		public delegate void MissedCallEventHandler(object sender, MissedCallEventArgs e);
		public delegate void FinishCallEventHandler(object sender, FinishedCallEventArgs e);

		public MissedCallEventHandler MissedCall;
		public FinishCallEventHandler FinishedCall;

		private bool logMissed = true;
		private bool logCompleted = true;

		public PhoneEventNotifier()
		{
			missedCalls = SystemState.PhoneMissedCalls;

			//SystemState missedCall = new SystemState(SystemProperty.PhoneMissedCall);			
			//missedCall.Changed += new ChangeEventHandler(missedCall_Changed);

			logCompleted = Convert.ToBoolean(Convert.ToInt16(PhoneLogRegKey.GetValue("LogCompleted", "1")));
			logMissed = Convert.ToBoolean(Convert.ToInt16(PhoneLogRegKey.GetValue("LogMissed", "1")));

			watchThread = new Thread(new ThreadStart(WatchForChange));
			watchThread.Start();
		}

		private RegistryKey PhoneLogRegKey
		{
			get
			{
				RegistryKey lm = Registry.CurrentUser;
				RegistryKey softRk = lm.OpenSubKey("SOFTWARE\\Infinityball\\PhoneLog", true);
				if (softRk == null)
					softRk = lm.CreateSubKey("SOFTWARE\\Infinityball\\PhoneLog");
				return softRk;
			}
		}

		~PhoneEventNotifier()
		{
			watchThread.Abort();
		}

		private void WatchForChange()
		{
			while (true)
			{
				if (logMissed)
				{
					if (missedCalls != SystemState.PhoneMissedCalls)
						missedCall_Changed(this, null);
				}

				if (logCompleted)
				{
					if (!inCall && SystemState.PhoneCallTalking)
					{
						inCall = true;
						startTime = DateTime.Now;
						lastCallerNumber = SystemState.PhoneTalkingCallerNumber;
						lastCaller = SystemState.PhoneTalkingCallerContact;
					}

					if (inCall && !SystemState.PhoneCallTalking)
					{
						inCall = false;
						if (startTime != DateTime.MinValue)
						{
							if (FinishedCall != null)
							{
								FinishedCallEventArgs e = new FinishedCallEventArgs();
								e.CallerName = GetCallerName(lastCaller);
								e.CallerNumber = lastCallerNumber;
								e.StartTime = startTime;
								e.EndTime = DateTime.Now;
								FinishedCall(this, e);
							}
						}
						startTime = DateTime.MinValue;
					}
				}

				Thread.Sleep(1000);
			}
		}

		public string CallerName
		{
			get
			{ return callerName; }
		}

		public string CallerNumber
		{
			get
			{ return callerNumber; }
		}

		private string GetCallerName(Contact caller)
		{
			string callerName = "(unknown)";
			if (caller == null)
			{
				if (SystemState.PhoneLastIncomingCallerNumber.Length > 0)
					callerName = ""; // SystemState.PhoneLastIncomingCallerNumber;
			}
			else
			{
				if (caller.FirstName.Length == 0 && caller.LastName.Length == 0 && caller.CompanyName.Length > 0)
					callerName = caller.CompanyName;
				else
					callerName = caller.FirstName + " " + caller.LastName;
			}

			return callerName;
		}

		private void missedCall_Changed(object sender, ChangeEventArgs args)
		{
			if (missedCalls < SystemState.PhoneMissedCalls)
			{
				if (MissedCall != null)
				{
					MissedCallEventArgs e = new MissedCallEventArgs();
					e.CallerNumber = SystemState.PhoneLastIncomingCallerNumber;
					e.CallerName = GetCallerName(SystemState.PhoneLastIncomingCallerContact);
					MissedCall(this, e);
				}

			}

			missedCalls = SystemState.PhoneMissedCalls;
		}
	}

	public class FinishedCallEventArgs : EventArgs
	{
		public string CallerName;
		public string CallerNumber;
		public DateTime StartTime;
		public DateTime EndTime;
	}

	public class MissedCallEventArgs : EventArgs
	{
		public string CallerName;
		public string CallerNumber;
	}
}
