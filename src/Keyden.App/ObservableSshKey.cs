using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using DynamicData;

namespace Keyden;

public partial class ObservableSshKey : ObservableObject
{
	public ObservableSshKey(
		string id,
		string name,
		string fingerprint,
		string publicKey)
	{
		Id = id;
		_Name = name;
		_Fingerprint = fingerprint;
		_PublicKey = publicKey;

		PropertyChanged += OnPropertyChanged;
		EnableForMachines.CollectionChanged += EnableForMachines_CollectionChanged;
	}

	private void EnableForMachines_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		Modified = true;
	}

	private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Modified))
			return;
		Modified = true;
	}

	public SshKeyOptions GetOptions()
	{
		var ret = new SshKeyOptions();
		{
			ret.EnableForMachines = EnableForMachines.ToList();

			ret.RequireAuthorization = RequireAuthorization;
			{
				ret.RemainAuthorized = RemainAuthorized;
				ret.RemainAuthorizedFor = RemainAuthorizedFor;
				{
					ret.RemainAuthorizedUntilKeyInactivity = RemainAuthorizedUntilKeyInactivity;
					ret.RemainAuthorizedUntilKeyInactivityFor = RemainAuthorizedUntilKeyInactivityFor;
					ret.RemainAuthorizedUntilUserInactivity = RemainAuthorizedUntilUserInactivity;
					ret.RemainAuthorizedUntilUserInactivityFor = RemainAuthorizedUntilUserInactivityFor;
					ret.RemainAuthorizedUntilLocked = RemainAuthorizedUntilLocked;
				}
			}
			ret.RequireAuthentication = RequireAuthentication;
			{
				ret.RemainAuthenticated = RemainAuthenticated;
				ret.RemainAuthenticatedFor = RemainAuthenticatedFor;
				{
					ret.RemainAuthenticatedUntilKeyInactivity = RemainAuthenticatedUntilKeyInactivity;
					ret.RemainAuthenticatedUntilKeyInactivityFor = RemainAuthenticatedUntilKeyInactivityFor;
					ret.RemainAuthenticatedUntilUserInactivity = RemainAuthenticatedUntilUserInactivity;
					ret.RemainAuthenticatedUntilUserInactivityFor = RemainAuthenticatedUntilUserInactivityFor;
					ret.RemainAuthenticatedUntilLocked = RemainAuthenticatedUntilLocked;
				}
			}
		}
		return ret;
	}
	public void SetOptions(SshKeyOptions options)
	{
		EnableForMachines.Clear();
		EnableForMachines.AddRange(options.EnableForMachines);

		RequireAuthorization = options.RequireAuthorization;
		{
			RemainAuthorized = options.RemainAuthorized;
			RemainAuthorizedFor = options.RemainAuthorizedFor;
			{
				RemainAuthorizedUntilKeyInactivity = options.RemainAuthorizedUntilKeyInactivity;
				RemainAuthorizedUntilKeyInactivityFor = options.RemainAuthorizedUntilKeyInactivityFor;
				RemainAuthorizedUntilUserInactivity = options.RemainAuthorizedUntilUserInactivity;
				RemainAuthorizedUntilUserInactivityFor = options.RemainAuthorizedUntilUserInactivityFor;
				RemainAuthorizedUntilLocked = options.RemainAuthorizedUntilLocked;
			}
		}
		RequireAuthentication = options.RequireAuthentication;
		{
			RemainAuthenticated = options.RemainAuthenticated;
			RemainAuthenticatedFor = options.RemainAuthenticatedFor;
			{
				RemainAuthenticatedUntilKeyInactivity = options.RemainAuthenticatedUntilKeyInactivity;
				RemainAuthenticatedUntilKeyInactivityFor = options.RemainAuthenticatedUntilKeyInactivityFor;
				RemainAuthenticatedUntilUserInactivity = options.RemainAuthenticatedUntilUserInactivity;
				RemainAuthenticatedUntilUserInactivityFor = options.RemainAuthenticatedUntilUserInactivityFor;
				RemainAuthenticatedUntilLocked = options.RemainAuthenticatedUntilLocked;
			}
		}

		Modified = false;
	}

	[ObservableProperty]
	public bool _Modified;

	public string Id { get; }

	[ObservableProperty]
	private string _Name;

	[ObservableProperty]
	private string _Fingerprint;

	[ObservableProperty]
	private string _PublicKey;

	public ObservableCollection<string> EnableForMachines { get; } = new();

	[ObservableProperty]
	private bool _RequireAuthorization;

	[ObservableProperty]
	private bool _RemainAuthorized;
	[ObservableProperty]
	private TimeSpan _RemainAuthorizedFor;

	[ObservableProperty]
	private bool _RemainAuthorizedUntilKeyInactivity;
	[ObservableProperty]
	private TimeSpan _RemainAuthorizedUntilKeyInactivityFor;

	[ObservableProperty]
	private bool _RemainAuthorizedUntilUserInactivity;
	[ObservableProperty]
	private TimeSpan _RemainAuthorizedUntilUserInactivityFor;

	[ObservableProperty]
	private bool _RemainAuthorizedUntilLocked;

	[ObservableProperty]
	private bool _RequireAuthentication;

	[ObservableProperty]
	private bool _RemainAuthenticated;
	[ObservableProperty]
	private TimeSpan _RemainAuthenticatedFor;

	[ObservableProperty]
	private bool _RemainAuthenticatedUntilKeyInactivity;
	[ObservableProperty]
	private TimeSpan _RemainAuthenticatedUntilKeyInactivityFor;

	[ObservableProperty]
	private bool _RemainAuthenticatedUntilUserInactivity;
	[ObservableProperty]
	private TimeSpan _RemainAuthenticatedUntilUserInactivityFor;

	[ObservableProperty]
	private bool _RemainAuthenticatedUntilLocked;
}
