using System;

namespace Keyden;

public class BackendException : Exception
{
	public BackendException(string? message) : base(message) { }
}
