TITLE DTSAgile:  Uninstalling ArcStache
echo off
REM: -------------------------------------------
REM: Shuts down ArcSOM, Unregisters COM components, then deletes tlb and dll files
REM: NOTE: Does NOT restart ArcSOM. Assumes you will run install.bat
REM: -------------------------------------------

@echo ..
@echo *************************
@echo Unregistering COM components with .NET
@echo *************************
@echo ..

@echo -------------------------
@echo Unregistering ArcStache.dll with .NET ...
@echo -------------------------
regasm /unregister ArcStache.dll

@echo ..
@echo ..
@echo *************************
@echo Stopping ArcGIS Server Object Manager and SOC Manager
@echo *************************
@echo ..
SC STOP ArcServerObjectManager
REM: Delay 5 seconds while SOM shuts down before shutting down SOC Monitor
ping -n 5 localhost > NUL
SC STOP ArcSOCMonitor


REM: Delay 10 seconds while SOM shuts down before deleting DLLs
ping -n 5 localhost > NUL

@echo ..
@echo *************************
@echo Deleting files (tlb, Logs/*.txt)...
@echo *************************
@echo ..
 del *.tlb
 del *.pdb
 del Logs\*.txt

pause