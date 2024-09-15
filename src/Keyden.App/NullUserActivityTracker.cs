using System;

namespace Keyden;

public sealed class NullUserActivityTracker : IUserActivityTracker
{
	public TimeSpan IdleDuration { get; } = TimeSpan.Zero;
	public event EventHandler<EventArgs> MachineLocked { add { } remove { } }
}
