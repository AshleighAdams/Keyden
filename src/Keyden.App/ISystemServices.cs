using System;

namespace Keyden;

public interface ISystemServices
{
	bool AutomaticallyStartApp { get; set; }
	TimeSpan UserIdleDuration { get; }
	public event EventHandler<EventArgs> MachineLocked;
}

public sealed class NullSystemServices : ISystemServices
{
	public bool AutomaticallyStartApp
	{
		get => false;
		set { }
	}
	public event EventHandler<EventArgs> MachineLocked { add { } remove { } }
	public TimeSpan UserIdleDuration => TimeSpan.Zero;
}
