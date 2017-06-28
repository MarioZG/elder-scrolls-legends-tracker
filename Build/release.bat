"c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" build.xml /fl /target:ReleaseLatest
echo off
if %errorlevel% gtr 0 goto error
echo on
"c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" build.xml /target:CleanUp
:error
pause
