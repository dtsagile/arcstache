TITLE DTSAgile:  Installing ArcStache
echo off
REM: -------------------------------------------
REM: Registers COM components, then starts ArcSOM
REM: NOTE: Assumes ArcSOM has been stopped prior to running script!
REM: -------------------------------------------

@echo ..
@echo *************************
@echo Registering COM components with .NET
@echo *************************
@echo ..
@echo -------------------------
@echo Registering ArcStache.dll with .NET ...
@echo -------------------------
@echo ..
regasm ArcStache.dll /tlb:ArcStache.tlb /codebase

@echo ..
@echo *************************
@echo Restarting ArcGIS Server Object Manager which starts SOC Manager
@echo *************************
@echo ..
SC START ArcServerObjectManager

pause