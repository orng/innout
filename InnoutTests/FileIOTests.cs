using System;
using System.Collections.Generic;
using System.Security.Permissions;
using FluentAssertions;
using Innout;
using NodaTime;
using Xunit;

namespace InnoutTests
{
	public class Tests
	{
		[Fact]
		public void TestSerializeAndDeserialize()
		{
			TimeRegistrations timeRegistrations = new TimeRegistrations();
			var clock = SystemClock.Instance.GetCurrentInstant();
			var today = clock.InUtc().Date;
			var registration = new Registration
			{
				Comment = "SomeComment",
				In = LocalTime.Midnight,
				Out = LocalTime.Noon
			};
			timeRegistrations[today] = new List<Registration>() {registration};
			string serialized = FileIO.SerializeTimeRegistrations(timeRegistrations);
			TimeRegistrations deserialized = FileIO.DeserializeTimeRegistrations(serialized);
			deserialized[today][0].Comment.Should().Be(timeRegistrations[today][0].Comment);
			deserialized[today][0].In.Should().Be(timeRegistrations[today][0].In);
			deserialized[today][0].Out.Should().Be(timeRegistrations[today][0].Out);
		}
		
		[Fact]
		public void TestSerializeAndDeserialize_WithOnlyInTime()
		{
			TimeRegistrations timeRegistrations = new TimeRegistrations();
			var clock = SystemClock.Instance.GetCurrentInstant();
			var today = clock.InUtc().Date;
			var registration = new Registration
			{
				In = LocalTime.Midnight,
			};
			timeRegistrations[today] = new List<Registration>() {registration};
			string serialized = FileIO.SerializeTimeRegistrations(timeRegistrations);
			TimeRegistrations deserialized = FileIO.DeserializeTimeRegistrations(serialized);
			deserialized[today][0].Comment.Should().Be(timeRegistrations[today][0].Comment);
			deserialized[today][0].In.Should().Be(timeRegistrations[today][0].In);
			deserialized[today][0].In.Should().Be(LocalTime.Midnight);
			deserialized[today][0].Out.Should().Be(timeRegistrations[today][0].Out);
		}
		
		[Fact]
		public void TestDeserialize_EmptyJson()
		{
			TimeRegistrations deserialized = FileIO.DeserializeTimeRegistrations("{}");
			deserialized.Keys.Count.Should().Be(0);
			
		}

	}
}