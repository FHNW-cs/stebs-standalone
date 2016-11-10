!include "MUI2.nsh"
!include "x64.nsh"
!include "LogicLib.nsh"
!include "Sections.nsh"

!define MUI_LANGDLL_ALLLANGUAGES
!define MUI_ICON "installer\cpu.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP "installer\FHNW2.bmp"

!define APPNAME "stebs"
!define COMPANYNAME "FHNW"
!define DESCRIPTION "student training eight bit simulator"
!define VERSIONMAJOR 4
!define VERSIONMINOR 1
!define VERSIONBUILD 2
!define ABOUTURL "http://www.fhnw.ch"
!define INSTALLSIZE 7300

!define MUI_TEXT_WELCOME_INFO_TITLE  "Welcome to the stebs installer"
!define MUI_TEXT_COMPONENTS_TITLE  "Please choose the components to install"
!define MUI_TEXT_COMPONENTS_SUBTITLE
!define MUI_INNERTEXT_COMPONENTS_DESCRIPTION_TITLE
!define MUI_INNERTEXT_COMPONENTS_DESCRIPTION_INFO
!define MUI_TEXT_DIRECTORY_TITLE  "Please choose the directory to install"
!define MUI_TEXT_DIRECTORY_SUBTITLE
!define MUI_TEXT_INSTALLING_TITLE 
!define MUI_TEXT_INSTALLING_SUBTITLE
!define MUI_TEXT_FINISH_TITLE  "Installation complete"
!define MUI_TEXT_FINISH_SUBTITLE
!define MUI_TEXT_ABORT_TITLE
!define MUI_TEXT_ABORT_SUBTITLE
!define MUI_BUTTONTEXT_FINISH "Finish"
!define MUI_TEXT_FINISH_INFO_TITLE "Installation complete"
!define MUI_TEXT_FINISH_INFO_REBOOT
!define MUI_TEXT_FINISH_REBOOTNOW
!define MUI_TEXT_FINISH_REBOOTLATER
!define MUI_TEXT_FINISH_INFO_TEXT "Thank you!"

#!define MUI_WELCOMEPAGE_TEXT "New text goes here"
!define MUI_TEXT_WELCOME_INFO_TEXT "stebs - student training eight bit simulator"
!define MUI_PAGE_CUSTOMFUNCTION_SHOW MyWelcomeShowCallback

RequestExecutionLevel admin ;Requires admin rights on NT6+ (When UAC is turned on)

InstallDir "$PROGRAMFILES\${APPNAME}"

!macro check64
${If} ${RunningX64} 
	SetRegView 64
	StrCpy $INSTDIR "$PROGRAMFILES64\${APPNAME}"
${EndIf}
!macroend

Name "${COMPANYNAME} - ${APPNAME}"
Icon "installer\cpu.ico"
outFile "installer\stebs-installer.exe"
BrandingText "FHNW - student training eight bit simulator"

!include LogicLib.nsh

!macro VerifyUserIsAdmin
UserInfo::GetAccountType
pop $0
${If} $0 != "admin" ;Requires admin rights on NT4+
	messageBox mb_iconstop "Administrator rights required!"
	setErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
	quit
${EndIf}
!macroend

#!insertmacro MUI_LANGUAGE "English"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

Function .onInit
	setShellVarContext all
	!insertmacro VerifyUserIsAdmin
	!insertmacro Check64
	Call CheckAndDownloadDotNet45
FunctionEnd


Section "stebs (required)" SEC_ST
	SectionIn RO

	# Files for the install directory - to build the installer, these should be in
	# the same directory as the install script (this file)
	setOutPath $INSTDIR

	# Files added here should be removed by the uninstaller (see section "uninstall")
	file "stebs\bin\Release\stebs.exe"
	file "stebs\bin\Release\stebs.exe.config"
	file "stebs\bin\Release\NLog.config"
	file "stebs\bin\Release\*.dll"
	file "stebs\plugin\readme.txt"
	file /nonfatal /r "stebs\bin\Release\plugin\*.dll"
	file /r "stebs\bin\Release\res"

	# Copy plugin DLLs
	setOutPath "$INSTDIR\plugin"
	file /x "IOInterfaceLib.dll" "IOInterruptLib\bin\Release\*.dll"
	file /x "IOInterfaceLib.dll" "IOHeaterLib\bin\Release\*.dll"
	file /x "IOInterfaceLib.dll" "IOKeyboardLib\bin\Release\*.dll"
	file /x "IOInterfaceLib.dll" "IOLightLib\bin\Release\*.dll"
	file /x "IOInterfaceLib.dll" "IOSegmentLib\bin\Release\*.dll"
	file /x "IOInterfaceLib.dll" "IOWatchLib\bin\Release\*.dll"

	# Uninstaller - See function un.onInit and section "uninstall" for configuration
	writeUninstaller "$INSTDIR\uninstall.exe"

	# Start Menu
	# createDirectory "$SMPROGRAMS\${COMPANYNAME}"
	createShortCut "$SMPROGRAMS\${APPNAME}.lnk" "$INSTDIR\stebs.exe" "" "$INSTDIR\res\cpu.ico"

	# Registry information for add/remove programs
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayName" "${COMPANYNAME} - ${APPNAME} - ${DESCRIPTION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "InstallLocation" "$\"$INSTDIR$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayIcon" "$\"$INSTDIR\res\cpu.ico$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "Publisher" "$\"${COMPANYNAME}$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "URLInfoAbout" "$\"${ABOUTURL}$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayVersion" "$\"${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}$\""
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "VersionMajor" ${VERSIONMAJOR}
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "VersionMinor" ${VERSIONMINOR}
	
	# There is no option for modifying or repairing the install
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "NoRepair" 1
	
	# Set the INSTALLSIZE constant (!defined at the top of this script) so Add/Remove Programs can accurately report the size
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "EstimatedSize" ${INSTALLSIZE}
SectionEnd

Section "examples" SEC_SA
    SectionIn 1
    SetOutPath $INSTDIR\examples
    file /nonfatal /r "stebs\examples\*"
SectionEnd

Section /o "desktop shortcut" SEC_SC
    SectionIn 1
    SetOutPath $INSTDIR
    CreateShortCut "$DESKTOP\${APPNAME}.lnk" "$INSTDIR\stebs.exe" "" "$INSTDIR\res\cpu.ico"
SectionEnd


LangString DESC_Section_st ${LANG_ENGLISH} "Required files for stebs"
LangString DESC_Section_sa ${LANG_ENGLISH} "Examples located in examples folder"
LangString DESC_Section_sc ${LANG_ENGLISH} "Create shortcut on desktop"

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
!insertmacro MUI_DESCRIPTION_TEXT ${SEC_ST} $(DESC_Section_st)
!insertmacro MUI_DESCRIPTION_TEXT ${SEC_SA} $(DESC_Section_sa)
!insertmacro MUI_DESCRIPTION_TEXT ${SEC_SC} $(DESC_Section_sc)
!insertmacro MUI_FUNCTION_DESCRIPTION_END


Function .onSelChange
	SectionGetFlags ${SEC_ST} $0
	IntOp $0 $0 | ${SEC_ST}
	SectionSetFlags ${SEC_ST} $0
FunctionEnd


# Uninstaller
Function un.onInit
	SetShellVarContext all
	# Verify the uninstaller - last chance to back out
	MessageBox MB_OKCANCEL "Permanently remove ${APPNAME}?" IDOK next
		Abort
	next:
	!insertmacro VerifyUserIsAdmin
FunctionEnd

Section "uninstall"
	# Remove Desktop Shortcut
	delete "$DESKTOP\${APPNAME}.lnk"

	# Remove Start Menu launcher
	delete "$SMPROGRAMS\${COMPANYNAME}\${APPNAME}.lnk"
	# Try to remove the Start Menu folder - this will only happen if it is empty
	rmDir "$SMPROGRAMS\${COMPANYNAME}"

	delete $INSTDIR\examples\*
	delete $INSTDIR\plugin\*
	delete $INSTDIR\res\Layouts\*
	delete $INSTDIR\res\*
	
	rmDir $INSTDIR\examples
	rmDir $INSTDIR\plugin
	rmDir $INSTDIR\res\Layouts
	rmDir $INSTDIR\res
	
	# Remove files
	delete $INSTDIR\*
	# Always delete uninstaller as the last action
	delete $INSTDIR\uninstall.exe

	# Try to remove the install directory - this will only happen if it is empty
	rmDir $INSTDIR

	# Remove uninstaller information from the registry
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}"
SectionEnd

Function CheckAndDownloadDotNet45

	# Set up our Variables
	Var /GLOBAL dotNET45IsThere
	Var /GLOBAL dotNET_CMD_LINE
	Var /GLOBAL EXIT_CODE

	# We are reading a version release DWORD that Microsoft says is the documented
	# way to determine if .NET Framework 4.5 is installed
	ReadRegDWORD $dotNET45IsThere HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
	IntCmp $dotNET45IsThere 378389 is_equal is_less is_greater

	is_equal:
		Goto done_compare_not_needed
	is_greater:
		Goto done_compare_not_needed
	is_less:
		MessageBox MB_OK|MB_ICONEXCLAMATION ".NET Framework (4.5) not found. Trying to install it."
		Goto done_compare_needed

	done_compare_needed:
		#.NET Framework 4.5 install is *NEEDED*

		# Microsoft Download Center EXE:
		# Web Bootstrapper: http://go.microsoft.com/fwlink/?LinkId=225704
		# Full Download: http://go.microsoft.com/fwlink/?LinkId=225702

		# Let's see if the user is doing a Silent install or not
		IfSilent is_quiet is_not_quiet

		is_quiet:
			StrCpy $dotNET_CMD_LINE "/q /norestart"
			Goto LookForLocalFile
		is_not_quiet:
			StrCpy $dotNET_CMD_LINE "/showrmui /passive /norestart"
			Goto LookForLocalFile

		LookForLocalFile:
			# Let's see if the user stored the Full Installer
			IfFileExists "$EXEPATH\components\dotNET45Full.exe" do_local_install do_network_install

			do_local_install:
				# .NET Framework found on the local disk.  Use this copy
				ExecWait '"$EXEPATH\components\dotNET45Full.exe" $dotNET_CMD_LINE' $EXIT_CODE
				Goto is_reboot_requested

			# Now, let's download the .NET
			do_network_install:
				Var /GLOBAL dotNetDidDownload
				NSISdl::download "http://go.microsoft.com/fwlink/?LinkId=225704" "$TEMP\dotNET45Web.exe"
				Pop $dotNetDidDownload
                StrCmp $dotNetDidDownload "success" success fail
				success:
					ExecWait '"$TEMP\dotNET45Web.exe" $dotNET_CMD_LINE' $EXIT_CODE
					Goto is_reboot_requested
       				fail:
					MessageBox MB_OK|MB_ICONEXCLAMATION "Unable to download .NET Framework.  ${APPNAME} will be installed, but will not function without the Framework!"
					Goto done_dotNET_function

				# $EXIT_CODE contains the return codes.  1641 and 3010 means a Reboot has been requested
				is_reboot_requested:
					${If} $EXIT_CODE = 1641
					${OrIf} $EXIT_CODE = 3010
						SetRebootFlag true
					${EndIf}

	done_compare_not_needed:
		# Done dotNET Install
		Goto done_dotNET_function
	
	#exit the function
	done_dotNET_function:
FunctionEnd
	
	
Function MyWelcomeShowCallback
	SendMessage $mui.WelcomePage.Text ${WM_SETTEXT} 0 "STR:$(MUI_TEXT_WELCOME_INFO_TEXT)$\n$\nVersion: ${VERSIONMAJOR}.${VERSIONMINOR}"
FunctionEnd