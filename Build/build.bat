"C:\Program Files (x86)\MSBuild\14.0\bin\msbuild" build.xml /fl
echo off
if %errorlevel% gtr 0 goto error
echo on
"C:\Program Files (x86)\MSBuild\14.0\bin\msbuild" build.xml /target:CleanUp
:error
pause