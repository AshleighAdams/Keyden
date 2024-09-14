using Avalonia.Controls;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Keyden.Views
{
	public partial class AboutWindow : Window
	{
		public string Version => Verlite.Version.Full;
		public AboutWindow()
		{
			InitializeComponent();

			if (ActualTransparencyLevel == WindowTransparencyLevel.Mica)
			{
				Acrylic.Material.TintOpacity = 0;
				Acrylic.Material.MaterialOpacity = 0;
			}

			AdditionalNotices.PointerPressed += AdditionalNotices_PointerPressed;
		}

		private void AdditionalNotices_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
		{
			Process.Start(new ProcessStartInfo()
			{
				FileName = Path.Join(Path.GetDirectoryName(Environment.ProcessPath), "third-party-notices.md"),
				UseShellExecute = true,
			});
		}

		public string MitLicense { get; } =
			"""
			Permission is hereby granted, free of charge, to any person obtaining a copy
			of this software and associated documentation files (the "Software"), to deal
			in the Software without restriction, including without limitation the rights
			to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
			copies of the Software, and to permit persons to whom the Software is
			furnished to do so, subject to the following conditions:
			
			The above copyright notice and this permission notice shall be included in all
			copies or substantial portions of the Software.
			""";

		public string Apache2License { get; } =
			"""
			Licensed under the Apache License, Version 2.0 (the "License");
			you may not use this file except in compliance with the License.
			You may obtain a copy of the License at

			http://www.apache.org/licenses/LICENSE-2.0
			""";

		public string BsdLicense { get; } =
			"""
			Redistribution and use in source and binary forms, with or without
			modification, are permitted provided that the following conditions are
			met:

			  * Redistributions of source code must retain the above copyright
			    notice, this list of conditions and the following disclaimer.

			  * Redistributions in binary form must reproduce the above copyright
			    notice, this list of conditions and the following disclaimer in
			    the documentation and/or other materials provided with the
			    distribution.

			  * Neither the name of the copyright holder nor the names of its
			    contributors may be used to endorse or promote products derived
			    from this software without specific prior written permission.

			THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
			"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
			LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
			A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
			OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
			SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
			LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
			DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
			THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
			(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
			OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
			""";
	}
}
