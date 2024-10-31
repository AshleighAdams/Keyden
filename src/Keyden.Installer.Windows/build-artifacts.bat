@echo off

set "config=%1" & shift /1
set "arch=%1" & shift /1

if "%arch%" == "x86" set "rid=windows-x86"
if "%arch%" == "x64" set "rid=windows-x64"

if "%rid%" == "" exit 1

set "target=artifacts-%config%-%arch%"

rmdir /s /q "%target%"

mkdir "%target%"
echo > "%target%/hello.txt"
echo > "%target%/Keyden.exe"

rem dotnet publish -c "$config" -r "$rid" -o "artifacts-$config-$arch"
