using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using DynamicData.Kernel;

using Keyden.Views;

using Microsoft.Win32;

using ReactiveUI;

namespace Keyden.ViewModels;

public class SettingsViewModel : ViewModelBase
{
	private ISystemServices SystemServices { get; }
	public KeydenSettings Settings { get; }

	public SettingsViewModel(
		ISystemServices systemServices,
		KeydenSettings settings)
	{
		SystemServices = systemServices;
		Settings = settings;
	}

	private int _TabIndexSelected = 0;
	public int TabIndexSelected
	{
		get => _TabIndexSelected;
		set
		{
			if (value == _TabIndexSelected)
				return;

			this.RaisePropertyChanging(nameof(TabIndexSelected));
			this.RaisePropertyChanging(nameof(IsGeneralTabSelected));
			this.RaisePropertyChanging(nameof(IsSecurityTabSelectedAndLocked));
			this.RaisePropertyChanging(nameof(IsSecurityTabSelectedAndUnlocked));
			this.RaisePropertyChanging(nameof(IsAdvancedTabSelected));
			_TabIndexSelected = value;
			this.RaisePropertyChanged(nameof(TabIndexSelected));
			this.RaisePropertyChanged(nameof(IsGeneralTabSelected));
			this.RaisePropertyChanged(nameof(IsSecurityTabSelectedAndLocked));
			this.RaisePropertyChanged(nameof(IsSecurityTabSelectedAndUnlocked));
			this.RaisePropertyChanged(nameof(IsAdvancedTabSelected));
		}
	}
	public bool IsGeneralTabSelected => TabIndexSelected == 0;
	public bool IsSecurityTabSelectedAndLocked => TabIndexSelected == 2 && !SecurityUnlocked;
	public bool IsSecurityTabSelectedAndUnlocked => TabIndexSelected == 2 && SecurityUnlocked;
	public bool IsAdvancedTabSelected => TabIndexSelected == 1;

	public IReadOnlyList<KeystoreBackend> KeystoreBackends =>
		Settings.DeveloperMode
			? [KeystoreBackend.None, KeystoreBackend.OnePassCLI, KeystoreBackend.DeveloperTest]
			: [KeystoreBackend.None, KeystoreBackend.OnePassCLI];

	public IReadOnlyList<AuthenticationMode> AuthenticationModes { get; } = [AuthenticationMode.System, AuthenticationMode.InternalPIN];

	public bool AutomaticallyStartup
	{

		get => SystemServices.AutomaticallyStartApp;
		set
		{
			this.RaisePropertyChanging(nameof(AutomaticallyStartup));
			SystemServices.AutomaticallyStartApp = value;
			this.RaisePropertyChanged(nameof(AutomaticallyStartup));
		}
	}

	private bool _SecurityUnlocked = false;
	public bool SecurityUnlocked
	{
		get => _SecurityUnlocked;
		set
		{
			this.RaisePropertyChanging(nameof(SecurityUnlocked));
			this.RaisePropertyChanging(nameof(IsSecurityTabSelectedAndLocked));
			this.RaisePropertyChanging(nameof(IsSecurityTabSelectedAndUnlocked));
			_SecurityUnlocked = value;
			this.RaisePropertyChanged(nameof(SecurityUnlocked));
			this.RaisePropertyChanged(nameof(IsSecurityTabSelectedAndLocked));
			this.RaisePropertyChanged(nameof(IsSecurityTabSelectedAndUnlocked));
		}
	}

	private bool _NotUnlocking = true;
	public bool NotUnlocking
	{
		get => _NotUnlocking;
		set
		{
			this.RaisePropertyChanging(nameof(NotUnlocking));
			_NotUnlocking = value;
			this.RaisePropertyChanged(nameof(NotUnlocking));
		}
	}

	public string DefaultPipePath => AgentK.DefaultPipePath;

	private CancellationTokenSource? Cts { get; set; } = null;
	public async void Unlock()
	{
		try
		{
			Cts ??= new CancellationTokenSource();
			NotUnlocking = false;

			var success = await SystemServices.TryUnlockSettings(this, Cts.Token);
			if (success)
				SecurityUnlocked = true;
		}
		catch (BackendException ex)
		{
			if (await ExceptionWindow.Prompt(ex.Message, default) == ExceptionWindowResult.Abort)
				throw;
		}
		catch (Exception ex)
		{
			if (await ExceptionWindow.Prompt(ex.ToString(), default) == ExceptionWindowResult.Abort)
				throw;
		}
		finally
		{
			NotUnlocking = true;
		}
	}

	public void Lock()
	{
		Cts?.Cancel();
		Cts = null;

		SecurityUnlocked = false;
	}
}

public class DesignSettingsViewModel : SettingsViewModel
{
	public DesignSettingsViewModel() : base(new GenericSystemServices(), new())
	{
	}
}
