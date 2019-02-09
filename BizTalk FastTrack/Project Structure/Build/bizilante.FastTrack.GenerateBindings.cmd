@echo Executing script
@echo
"%systemroot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" bizilante.FastTrack.custom.targets /t:BeforeDeployment /p:GenerateBindings=True
@echo off
pause