@echo off
setlocal EnableExtensions

set "ORACLE_SID=orcl21"
set "BACKUP_DIR=C:\Backup_Oracle"

echo =========================================
echo RMAN FULL BACKUP - ATHTTT_QLBV
echo =========================================
echo ORACLE_SID=%ORACLE_SID%
echo Backup directory: %BACKUP_DIR%
echo.

if not exist "%BACKUP_DIR%" (
    mkdir "%BACKUP_DIR%"
    if errorlevel 1 (
        echo ERROR: Cannot create %BACKUP_DIR%.
        pause
        exit /b 1
    )
)

where rman.exe >nul 2>nul
if errorlevel 1 (
    echo ERROR: rman.exe not found in PATH.
    echo Please run this file from an Oracle-enabled command prompt or add Oracle bin to PATH.
    pause
    exit /b 1
)

for /f %%I in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd_HHmmss"') do set "TS=%%I"
set "RMAN_CMD=%TEMP%\athttt_rman_backup_%TS%.rman"
set "RMAN_LOG=%BACKUP_DIR%\rman_backup_%TS%.log"

(
    echo CONFIGURE CONTROLFILE AUTOBACKUP ON;
    echo RUN {
    echo   SQL 'ALTER SYSTEM ARCHIVE LOG CURRENT';
    echo   BACKUP AS COMPRESSED BACKUPSET DATABASE FORMAT '%BACKUP_DIR%\DB_FULL_%%d_%%T_%%U.bkp' TAG 'ATHTTT_FULL_BACKUP';
    echo   BACKUP AS COMPRESSED BACKUPSET ARCHIVELOG ALL FORMAT '%BACKUP_DIR%\ARCH_%%d_%%T_%%U.bkp' TAG 'ATHTTT_ARCHIVE_BACKUP';
    echo   BACKUP CURRENT CONTROLFILE FORMAT '%BACKUP_DIR%\CTL_%%d_%%T_%%U.bkp' TAG 'ATHTTT_CONTROLFILE_BACKUP';
    echo   CROSSCHECK BACKUP;
    echo   DELETE NOPROMPT EXPIRED BACKUP;
    echo   DELETE NOPROMPT OBSOLETE RECOVERY WINDOW OF 7 DAYS;
    echo }
    echo LIST BACKUP SUMMARY;
) > "%RMAN_CMD%"

echo Running RMAN backup...
echo RMAN command file: %RMAN_CMD%
echo RMAN log file: %RMAN_LOG%
echo.

rman target / cmdfile="%RMAN_CMD%" log="%RMAN_LOG%"
set "RMAN_EXIT=%ERRORLEVEL%"

del "%RMAN_CMD%" >nul 2>nul

if not "%RMAN_EXIT%"=="0" (
    echo.
    echo ERROR: RMAN backup failed. Exit code: %RMAN_EXIT%
    echo See log: %RMAN_LOG%
    pause
    exit /b %RMAN_EXIT%
)

echo.
echo =========================================
echo RMAN BACKUP COMPLETED.
echo Files/log are in %BACKUP_DIR%
echo Log: %RMAN_LOG%
echo =========================================
pause
exit /b 0
