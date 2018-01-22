//-------------------------------------------------------------------------------------------------
//
// TimeRegistration.cs -- The TimeRegistration class.
//
// Copyright (c) 2018 Marel. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;


namespace Innout
{
	//---------------------------------------------------------------------------------------------
	/// <summary>
	/// The TimeRegistration class TODO: Describe class here
	/// </summary>
	public class TimeRegistration
	{
		public DateTime? In { get; set; }
		public DateTime? Out { get; set; }
		public DateTime? Date { get; set; }
		public string Comment { get; set; }
	}
}
