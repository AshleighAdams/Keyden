using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden;

public struct SshKeyOptions
{
	public SshKeyOptions() { }

	public static bool operator ==(in SshKeyOptions left, in SshKeyOptions right) =>
		left.EnableForMachines.SequenceEqual(right.EnableForMachines) &&
		left.RequireAuthorization == right.RequireAuthorization &&
		left.RemainAuthorized == right.RemainAuthorized &&
		left.RemainAuthorizedFor == right.RemainAuthorizedFor &&
		left.RemainAuthorizedUntilKeyInactivity == right.RemainAuthorizedUntilKeyInactivity &&
		left.RemainAuthorizedUntilKeyInactivityFor == right.RemainAuthorizedUntilKeyInactivityFor &&
		left.RemainAuthorizedUntilUserInactivity == right.RemainAuthorizedUntilUserInactivity &&
		left.RemainAuthorizedUntilUserInactivityFor == right.RemainAuthorizedUntilUserInactivityFor &&
		left.RemainAuthorizedUntilLocked == right.RemainAuthorizedUntilLocked &&
		left.RemainAuthorizedUntilLockedFor == right.RemainAuthorizedUntilLockedFor &&
		left.RequireAuthentication == right.RequireAuthentication &&
		left.RemainAuthenticated == right.RemainAuthenticated &&
		left.RemainAuthenticatedFor == right.RemainAuthenticatedFor &&
		left.RemainAuthenticatedUntilKeyInactivity == right.RemainAuthenticatedUntilKeyInactivity &&
		left.RemainAuthenticatedUntilKeyInactivityFor == right.RemainAuthenticatedUntilKeyInactivityFor &&
		left.RemainAuthenticatedUntilUserInactivity == right.RemainAuthenticatedUntilUserInactivity &&
		left.RemainAuthenticatedUntilUserInactivityFor == right.RemainAuthenticatedUntilUserInactivityFor &&
		left.RemainAuthenticatedUntilLocked == right.RemainAuthenticatedUntilLocked &&
		left.RemainAuthenticatedUntilLockedFor == right.RemainAuthenticatedUntilLockedFor;

	public static bool operator !=(in SshKeyOptions left, in SshKeyOptions right) => !(left == right);
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is SshKeyOptions right && this == right;

	public override int GetHashCode() =>
		HashCode.Combine(
			EnableForMachines,
			HashCode.Combine(
				HashCode.Combine(
					RequireAuthorization,
					RemainAuthorized,
					RemainAuthorizedFor,
					RemainAuthorizedUntilKeyInactivity
				),
				HashCode.Combine(
					RemainAuthorizedUntilKeyInactivityFor,
					RemainAuthorizedUntilUserInactivity,
					RemainAuthorizedUntilUserInactivityFor,
					RemainAuthorizedUntilLocked,
					RemainAuthorizedUntilLockedFor
				)
			),
			HashCode.Combine(
				HashCode.Combine(
					RequireAuthentication,
					RemainAuthenticated,
					RemainAuthenticatedFor,
					RemainAuthenticatedUntilKeyInactivity
				),
				HashCode.Combine(
					RemainAuthenticatedUntilKeyInactivityFor,
					RemainAuthenticatedUntilUserInactivity,
					RemainAuthenticatedUntilUserInactivityFor,
					RemainAuthenticatedUntilLocked,
					RemainAuthenticatedUntilLockedFor
				)
			)
		);

	public IReadOnlyList<string> EnableForMachines { get; set; } = Array.Empty<string>();

	

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
	Task SyncKeyOptions(CancellationToken ct = default);
	SshKeyOptions? GetKeyOptions(string id);
	void SetKeyOptions(string id, SshKeyOptions? options);
}
