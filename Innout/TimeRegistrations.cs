using System;
using System.Collections.Generic;
using NodaTime;

namespace Innout
{
	public class TimeRegistrations : Dictionary<LocalDate, List<Registration>>
	{
		new public List<Registration> this[LocalDate key]
		{
			get
			{
				if (base.TryGetValue(key, out var value))
				{
					return value;
				}
				var empty = new List<Registration>();
				base[key] = empty;
				return empty;
			}
			set => base[key] = value;
		}
	}

	public class Registration
	{
		public LocalTime In { get; set; }
		public LocalTime Out { get; set; }
		public string Comment { get; set; }
	}
}