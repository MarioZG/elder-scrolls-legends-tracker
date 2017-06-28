"C:\Program Files (x86)\MSBuild\15.0\bin\msbuild" build.xml /fl /target:ReleaseLatest
echo off
if %errorlevel% gtr 0 goto error
echo on
"C:\Program Files (x86)\MSBuild\15.0\bin\msbuild" build.xml /target:CleanUp
:error
pause
