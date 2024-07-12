using System;
using System.Threading.Tasks;

namespace KeyWarden;

public struct SshKeyOptions
{
	public SshKeyOptions() { }
	public bool RequireAuthorization { get; set; } = true;

	public bool RemainAuthorized { get; set; } = false;
	public TimeSpan RemainAuthorizedFor { get; set; } = TimeSpan.FromHours(8);

	public bool RemainAuthorizedUntilKeyInactivity { get; set; }
	public TimeSpan RemainAuthorizedUntilKeyInactivityFor { get; set; } = TimeSpan.FromHours(1);

	public bool RemainAuthorizedUntilUserInactivity { get; set; }
	public TimeSpan RemainAuthorizedUntilUserInactivityFor { get; set; } = TimeSpan.FromMinutes(10);

	public bool RemainAuthorizedUntilLocked { get; set; }
	public TimeSpan RemainAuthorizedUntilLockedFor { get; set; } = TimeSpan.FromMinutes(10);

	public bool RequireAuthentication { get; set; } = true;

	public bool RemainAuthenticated { get; set; } = false;
	public TimeSpan RemainAuthenticatedFor { get; set; } = TimeSpan.FromHours(8);

	public bool RemainAuthenticatedUntilKeyInactivity { get; set; }
	public TimeSpan RemainAuthenticatedUntilKeyInactivityFor { get; set; } = TimeSpan.FromHours(1);

	public bool RemainAuthenticatedUntilUserInactivity { get; set; }
	public TimeSpan RemainAuthenticatedUntilUserInactivityFor { get; set; } = TimeSpan.FromMinutes(10);

	public bool RemainAuthenticatedUntilLocked { get; set; }
	public TimeSpan RemainAuthenticatedUntilLockedFor { get; set; } = TimeSpan.FromMinutes(10);
}

public interface ISshKeyOptionsStore
{
	public Task<SshKeyOptions> GetKeyOptions(string id);
	public Task SetKeyOptions(string id, SshKeyOptions options);
}
