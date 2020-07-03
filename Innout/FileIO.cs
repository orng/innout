using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using NodaTime.TimeZones;

namespace Innout
{
	public class FileIO
	{
		/// <summary>
		/// Filename is {monthname}{year}.txt
		/// </summary>
		public static string FileName  => GetFilename("txt");

		/// <summary>
		/// Json filename is {monthname}{year}.json
		/// </summary>
		public static string JsonFileName => GetFilename("json");

		private static string GetFilename(string fileEnding)
		{
			string path = Properties.Settings.Default.FileDirectory.Trim();
			string filename = String.Format("{0}.{1}", DateTime.Now.ToString("MMMyy", CultureInfo.InvariantCulture), fileEnding).ToLower();
			if (String.IsNullOrEmpty(path))
			{
				path = AppDomain.CurrentDomain.BaseDirectory;
			}
			return Path.Combine(path, filename);
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
			List<TimeRegistration> registrations = new List<TimeRegistration>();
			try
			{
				if (File.Exists(FileName))
				{
					string[] lines = File.ReadAllLines(FileName);
					registrations = lines.Select(line => RegistrationFromString(line))
						.Where(line => line != null)
						.ToList();
				}
				return registrations;
			}
			catch (Exception e)
			{
				return registrations;
			}
		}

		public static TimeRegistrations GetDetailedRegistrations()
		{
			TimeRegistrations registrations = new TimeRegistrations();
			try
			{
				if (File.Exists(JsonFileName))
				{
					registrations = DeserializeTimeRegistrations(File.ReadAllText(JsonFileName));
				}
				return registrations;
			}
			catch (Exception e)
			{
				return registrations;
			}	
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jsonData"></param>
		/// <returns></returns>
		public static TimeRegistrations DeserializeTimeRegistrations(string jsonData)
		{
			try
			{
				var settings = new JsonSerializerSettings();
				settings.DateParseHandling = DateParseHandling.None;
				return JsonConvert.DeserializeObject<TimeRegistrations>(jsonData, settings);
			}
			catch
			{
				return new TimeRegistrations();
			}
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registrations"></param>
		/// <returns></returns>
		public static string SerializeTimeRegistrations(TimeRegistrations registrations)
		{
			var settings = new JsonSerializerSettings();
			settings.DateParseHandling = DateParseHandling.None;
			settings.Formatting = Formatting.Indented;
			settings.NullValueHandling = NullValueHandling.Ignore;
			return JsonConvert.SerializeObject(registrations, settings);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registrations"></param>
		public static void WriteDetailedRegistrations(TimeRegistrations registrations)
		{
			string json = SerializeTimeRegistrations(registrations);
			File.WriteAllText(JsonFileName, json);
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
