set TargetPath=%1
set ConfigurationName=%2
set TargetDir=%3
set TargetName=%4

if /i %ConfigurationName%==Debug set OutBinFolder=C:\Temp\bin\
if /i %ConfigurationName%==Release set OutBinFolder=C:\Temp\bin\


mkdir %OutBinFolder%
echo "Copying pdb files to %OutBinFolder%"
echo %TargetDir%
echo %TargetName%
echo %OutBinFolder%
exit 0

xcopy /f /q /y /c %TargetDir%%TargetName%.pdb  %OutBinFolder%
if %errorlevel% neq 0 exit /b %errorlevel%

exit

echo "Copying dll files to %OutBinFolder%"
xcopy /f /q /y  %TargetDir%%TargetName%.dll  %OutBinFolder%
if %errorlevel% neq 0 exit /b %errorlevel%


echo "Copying exe files to %OutBinFolder%"
xcopy /f /q /y %TargetDir%%TargetName%.exe  %OutBinFolder%
if %errorlevel% neq 0 exit /b %errorlevel%


echo "Copying json files to %OutBinFolder%"
xcopy /f /q /y %TargetDir%%TargetName%.runtimeconfig.json  %OutBinFolder%
if %errorlevel% neq 0 exit /b %errorlevel%
