using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Innout
{
	class FileIO
	{
		/// <summary>
		/// Filename is {monthname}{year}.txt
		/// Filepath is same as the exe, TODO: make configurable
		/// </summary>
		public static string FileName
		{
			get
			{
				string path = Properties.Settings.Default.FileDirectory.Trim();
				string filename = String.Format("{0}.txt", DateTime.Now.ToString("MMMyy", CultureInfo.InvariantCulture)).ToLower();
				if (String.IsNullOrEmpty(path))
				{
					path = AppDomain.CurrentDomain.BaseDirectory;
				}
				return Path.Combine(path, filename);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registations"></param>
		public static void WriteRegistrations(List<TimeRegistration> registations)
		{
			string[] lines = registations.Select(r => RegistrationToString(r)).ToArray();
			File.WriteAllLines(FileName, lines);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static List<TimeRegistration> GetRegistrations()
		{
			List<TimeRegistration> registations = new List<TimeRegistration>();
			try
			{
				if (File.Exists(FileName))
				{
					string[] lines = File.ReadAllLines(FileName);
					registations = lines.Select(line => RegistrationFromString(line))
						.Where(line => line != null)
						.ToList();
				}
				return registations;
			}
			catch (Exception e)
			{
				return registations;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registration"></param>
		/// <returns></returns>
		public static string RegistrationToString(TimeRegistration registration)
		{
			if (registration.Date == null && registration.Comment != null)
			{
				return registration.Comment;
			}
			string dateString = registration.Date?.ToString("dd.MM.yyyy") ?? "";
			string inString = registration?.In?.ToString("HH:mm") ?? "";
			string outString = registration?.Out?.ToString("HH:mm") ?? "";
			string comment = registration.Comment;
			return $"{dateString}\t{inString}\t{outString}\t{comment}";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		public static TimeRegistration RegistrationFromString(string line)
		{
			TimeRegistration registration = new TimeRegistration();
			if (String.IsNullOrEmpty(line))
			{
				return null;
			}

			if (line.StartsWith("#"))
			{
				registration.Comment = line;
			}
			else
			{
				CultureInfo provider = CultureInfo.InvariantCulture;
				string[] items = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
				if (items.Length >= 1 && !String.IsNullOrEmpty(items[0]))
				{
					registration.Date = DateTime.ParseExact(items[0], "dd.MM.yyyy", provider);
				}

				if (items.Length >= 2 && !String.IsNullOrEmpty(items[1]))
				{
					registration.In = DateTime.ParseExact(items[1], "HH:mm", provider);
				}
				if (items.Length >= 3 && !String.IsNullOrEmpty(items[2]))
				{
					registration.Out = DateTime.ParseExact(items[2], "HH:mm", provider);
				}
				if (items.Length >= 4 && !String.IsNullOrEmpty(items[3]))
				{
					registration.Comment = String.Join(" ", items.Skip(3));
				}

			}
			return registration;
		}

	}
}
