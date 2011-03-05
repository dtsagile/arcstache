-------------------------------
Instructions
-------------------------------
Install and uninstall steps below MUST be done with Administrative privileges. Easiest way is to 
"Run As Administrator" a CMD window and change to the c:\program files\dtsagile\txwrap\ directory.
Then enter the appropriate commands below.

-------------------------------
Install on ArcGIS Server
-------------------------------
* Running identity (likely <machine>\ArcGISSOC):
	- must be member of agsusers group
	- must have full write access to this install folder and subfolders

1. As Administrator, run install.bat 
	- 	registers the custom server object extensions with COM

2. As Administrator, run RegisterRiskAssessment.exe
	- 	registers the RiskAssessment SOE with ArcGIS Server
	- 	NOTE: This only needs to be run once. Redeploying the 
		TxWRAP.AGS.SOE.REST.RiskAssessment.dll on a box DOES NOT require rerunning this.

3. As Administrator, run RegisterReports.exe
	- 	registers the RiskReports SOE with ArcGIS Server
	- 	NOTE: This only needs to be run once. Redeploying the 
		TxWRAP.AGS.SOE.REST.Reports.dll on a box DOES NOT require rerunning this.

-------------------------------
Uninstall on ArcGIS Server
-------------------------------
1. As Administrator, run UnregisterRiskAssessment.bat
	- 	unregisters the RiskAssessment SOE with ArcGIS Server
	-	NOTE: Only necessary to run this if you are removing the 
		SOE from the ArcGIS Server for good.

2. As Administrator, run UnregisterReports.bat
	- 	unregisters the RiskReports SOE with ArcGIS Server
	-	NOTE: Only necessary to run this if you are removing the 
		SOE from the ArcGIS Server for good.		

3. As Administrator, run uninstall.bat
	- 	unregisters the custom server object extensions with COM