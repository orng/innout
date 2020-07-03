using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using NodaTime;

namespace Innout
{
	public partial class Innout : ServiceBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Innout()
		{
			InitializeComponent();
			this.CanHandleSessionChangeEvent = true;
			In();
		}

		/// <summary>
		/// Handle session changes, e.g. when the user locks/unlocks the desktop or logs on/off
		/// In these cases we want to update the time registration
		/// </summary>
		/// <param name="changeDescription"></param>
		protected override void OnSessionChange(SessionChangeDescription changeDescription)
		{
			base.OnSessionChange(changeDescription);
			switch(changeDescription.Reason)
			{
				case SessionChangeReason.SessionLock:
				case SessionChangeReason.SessionLogoff:
					Out();
					break;
				case SessionChangeReason.SessionUnlock:
				case SessionChangeReason.SessionLogon:
					In();
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Register event listener for system events when service is started
		/// </summary>
		/// <param name="args"></param>
		protected override void OnStart(string[] args)
		{
			Microsoft.Win32.SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
		}

		/// <summary>
		/// Write logons/logoffs/locks and unlocks to file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			switch (e.Reason)
			{
				case SessionSwitchReason.SessionLock:
				case SessionSwitchReason.SessionLogoff:
					Out();
					break;
				case SessionSwitchReason.SessionUnlock:
				case SessionSwitchReason.SessionLogon:
					In();
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// If this is the first time we are logging in then we want to add a new "in" time
		/// </summary>
		private void In()
		{
			List<TimeRegistration> registrations = FileIO.GetRegistrations();
			if (IsFirstInOfToday(registrations) || registrations.Count == 0)
			{ 
				AddNewIn(registrations);
				FileIO.WriteRegistrations(registrations);
			}
			
			var detailedRegs = FileIO.GetDetailedRegistrations();
			var clock = SystemClock.Instance.GetCurrentInstant();
			var today = detailedRegs[clock.InUtc().Date];
			today.Add(new Registration(){In = clock.InUtc().TimeOfDay});
			FileIO.WriteDetailedRegistrations(detailedRegs);
		}

		/// <summary>
		/// Update the last out time to just now
		/// </summary>
		private void Out()
		{
			List<TimeRegistration> registrations = FileIO.GetRegistrations();
			UpdateLastOut(registrations);
			FileIO.WriteRegistrations(registrations);
			
			var detailedRegs = FileIO.GetDetailedRegistrations();
			var clock = SystemClock.Instance.GetCurrentInstant();
			var today = detailedRegs[clock.InUtc().Date];
			var lastIn = today.LastOrDefault();
			if (lastIn == null)
			{
				// TODO: handle past midnight and past month change
				return; 
			}
			lastIn.Out = clock.InUtc().TimeOfDay;
			FileIO.WriteDetailedRegistrations(detailedRegs);
		}

		/// <summary>
		/// Returns true if we don't have any registration for today in registrations
		/// </summary>
		/// <param name="registrations">Registrations</param>
		/// <returns></returns>
		private static bool IsFirstInOfToday(List<TimeRegistration> registrations)
		{
			if (registrations.Count == 0)
			{
				return false;
			}

			TimeRegistration last = GetLastNonComment(registrations);
			return last.Date.Value.Day < DateTime.Now.Day;
		}

		/// <summary>
		/// Update the last out time in the list of registrations
		/// </summary>
		/// <param name="registrations"></param>
		private static List<TimeRegistration> UpdateLastOut(List<TimeRegistration> registrations)
		{
			TimeRegistration last = GetLastNonComment(registrations);
			if (last.Date?.Day == DateTime.Now.Day)
			{
				last.Out = DateTime.Now;
			}
			else
			{
				TimeRegistration newOut = new TimeRegistration() { Out = DateTime.Now, Date = DateTime.Now };
				registrations.Add(newOut);
			}
			return registrations;
		}

		/// <summary>
		/// Add a new "in" time to the list of registrations
		/// </summary>
		/// <param name="registrations"></param>
		/// <returns></returns>
		private static List<TimeRegistration> AddNewIn(List<TimeRegistration> registrations)
		{
			TimeRegistration newRegistration = new TimeRegistration() { Date = DateTime.Now, In = DateTime.Now };
			registrations.Add(newRegistration);
			return registrations;
		}

		/// <summary>
		/// Get last non-empty non-comment line of the list of registrations
		/// </summary>
		/// <param name="registrations"></param>
		/// <returns></returns>
		private static TimeRegistration GetLastNonComment(List<TimeRegistration> registrations)
		{
			return registrations.Where(r => r.Date != null).LastOrDefault();
		}

		/// <summary>
		/// Unregister the event listener when service is stopped
		/// </summary>
		protected override void OnStop()
		{
			Microsoft.Win32.SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
		}

	}
}
