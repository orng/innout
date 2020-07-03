using FluentAssertions;
using Innout;
using NodaTime;
using Xunit;

namespace InnoutTests
{
	public class TimeRegistrationsTests
	{
		
		[Fact]
		public void TestGettingEmptyAndAddingToIt()
		{
			var timeRegistrations = new TimeRegistrations();
			var clock = SystemClock.Instance.GetCurrentInstant();
			var todayDate = clock.InUtc().Date;
			var today = timeRegistrations[todayDate];
			var registration = new Registration() {In = clock.InUtc().TimeOfDay};
			today.Add(registration);

			timeRegistrations.Should().ContainKey(todayDate);
			timeRegistrations.Should().ContainValue(today);
		}
	}
}