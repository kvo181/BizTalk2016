@echo Executing script
@echo
"%systemroot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" bizilante.FastTrack.Build.proj /p:ConfigurationName=Release,Platform="Any CPU",DeploymentMode=Deployment,IncludeTests=False,GenerateBindings=False
@echo off
pause