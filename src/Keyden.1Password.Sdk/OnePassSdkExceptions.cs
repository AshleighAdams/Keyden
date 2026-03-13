using System;

namespace Keyden.OnePassword.SDK;

public class OnePassSdkException : Exception
{
	public OnePassSdkException() {}
	public OnePassSdkException(string? message) : base(message) { }
	public OnePassSdkException(string? message, Exception? innerException) : base(message, innerException) { }
}

public class OnePassSdkChannelClosedException() : OnePassSdkException("Desktop app connection channel is closed.") { }
public class OnePassSdkChannelConnectionDroppedException() : OnePassSdkException("Connection was unexpectedly dropped by the desktop app.") { }
public class OnePassSdkInternalException(int returnCode) : OnePassSdkException($"An internal error occurred. Please contact 1Password support and mention the return returnCode: {returnCode}") { }
public class OnePassSdkDesktopSessionExpiredException(string message) : OnePassSdkException(message) { }
public class OnePassSdkRateLimitExceededException(string message) : OnePassSdkException(message) { }
