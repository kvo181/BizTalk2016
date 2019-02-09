@echo Executing script
@echo
"%systemroot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" bizilante.FastTrack.Build.proj /p:ConfigurationName=Debug,Platform="Any CPU",DeploymentMode=Development,IncludeTests=False,GenerateBindings=False
@echo off
pause