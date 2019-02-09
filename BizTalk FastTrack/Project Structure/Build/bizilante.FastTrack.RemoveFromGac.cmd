@echo Executing script
@echo
"%systemroot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" bizilante.FastTrack.targets /t:RemoveFromGac
@echo off
pause