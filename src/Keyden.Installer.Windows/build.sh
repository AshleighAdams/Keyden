#!/bin/bash
set -euo pipefail

if [[ ! -d artifacts-x64 ]]; then
	dotnet publish ../Keyden.Desktop.Windows -c Release -r win-x64 -o artifacts-x64
fi

if [[ ! -d artifacts-arm64 ]]; then
	dotnet publish ../Keyden.Desktop.Windows -c Release -r win-arm64 -o artifacts-arm64
fi

VERSION="$(verlite .)"
PWD="$(pwd -W)"

./InnoSetup/ISCC.exe Keyden.iss -DVersion="$VERSION" -DArch=x64 -DArtifactsDir=artifacts-x64 -FKeydenSetup-x64 -DEnableSigning -Sst="signtool.exe sign /fd sha256 /tr http://ts.ssl.com /td sha256 /f ${PWD}/dev.pfx /p hello \$f"
./InnoSetup/ISCC.exe Keyden.iss -DVersion="$VERSION" -DArch=arm64 -DArtifactsDir=artifacts-arm64 -FKeydenSetup-arm64 -DEnableSigning -Sst="signtool.exe sign /fd sha256 /tr http://ts.ssl.com /td sha256 /f ${PWD}/dev.pfx /p hello \$f"
