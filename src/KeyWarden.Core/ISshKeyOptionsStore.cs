using System;
using System.Threading.Tasks;

namespace KeyWarden;

public struct SshKeyOptions
{
	public bool RequireAuthorization { get; set; }
	public TimeSpan? RemainAuthorizedFor { get; set; }
	public TimeSpan? RemainAuthorizedUntilKeyInactivity { get; set; }
	public TimeSpan? RemainAuthorizedUntilUserInactivity { get; set; }
	public TimeSpan? RemainAuthorizedUntilLocked { get; set; }

	public bool RequireAuthentication { get; set; }
	public TimeSpan? RemainAuthenticatedFor { get; set; }
	public TimeSpan? RemainAuthenticatedUntilKeyInactivity { get; set; }
	public TimeSpan? RemainAuthenticatedUntilUserInactivity { get; set; }
	public TimeSpan? RemainAuthenticatedUntilLocked { get; set; }
}

public interface ISshKeyOptionsStore
{
	public Task<SshKeyOptions> GetKeyOptions(string id);
	public Task SetKeyOptions(string id, SshKeyOptions options);
}
