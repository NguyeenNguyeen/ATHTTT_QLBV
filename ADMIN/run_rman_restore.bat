@echo off
setlocal EnableExtensions EnableDelayedExpansion

set "ORACLE_SID=orcl21"
set "BACKUP_DIR=C:\Backup_Oracle"

echo =========================================
echo RMAN DATABASE RESTORE - ATHTTT_QLBV
echo =========================================
echo ORACLE_SID=%ORACLE_SID%
echo Backup directory: %BACKUP_DIR%
echo.
echo WARNING: This operation will shut down the database and restore datafiles.
echo.

if /I not "%~1"=="--yes" (
    set /p CONFIRM=Type RESTORE to continue: 
    if /I not "!CONFIRM!"=="RESTORE" (
        echo Restore cancelled.
        pause
        exit /b 1
    )
)

if not exist "%BACKUP_DIR%" (
    echo ERROR: Backup directory does not exist: %BACKUP_DIR%
    pause
    exit /b 1
)

dir "%BACKUP_DIR%\*.bkp" >nul 2>nul
if errorlevel 1 (
    echo ERROR: No RMAN .bkp files found in %BACKUP_DIR%.
    pause
    exit /b 1
)

where sqlplus.exe >nul 2>nul
if errorlevel 1 (
    echo ERROR: sqlplus.exe not found in PATH.
    pause
    exit /b 1
)

where rman.exe >nul 2>nul
if errorlevel 1 (
    echo ERROR: rman.exe not found in PATH.
    pause
    exit /b 1
)

for /f %%I in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd_HHmmss"') do set "TS=%%I"
set "PREP_SQL=%TEMP%\athttt_rman_restore_prep_%TS%.sql"
set "OPEN_SQL=%TEMP%\athttt_rman_restore_open_%TS%.sql"
set "RESETLOGS_SQL=%TEMP%\athttt_rman_restore_resetlogs_%TS%.sql"
set "RMAN_CMD=%TEMP%\athttt_rman_restore_%TS%.rman"
set "RMAN_LOG=%BACKUP_DIR%\rman_restore_%TS%.log"

(
    echo WHENEVER SQLERROR EXIT SQL.SQLCODE
    echo SHUTDOWN IMMEDIATE;
    echo STARTUP MOUNT;
    echo EXIT;
) > "%PREP_SQL%"

echo CROSSCHECK BACKUP; > "%RMAN_CMD%"
for %%F in ("%BACKUP_DIR%\*.bkp") do (
    echo CATALOG BACKUPPIECE '%%~fF'; >> "%RMAN_CMD%"
)
(
    echo RUN {
    echo   RESTORE DATABASE;
    echo   RECOVER DATABASE;
    echo }
) >> "%RMAN_CMD%"

(
    echo WHENEVER SQLERROR EXIT SQL.SQLCODE
    echo ALTER DATABASE OPEN;
    echo ALTER PLUGGABLE DATABASE ALL OPEN;
    echo ALTER PLUGGABLE DATABASE ALL SAVE STATE;
    echo EXIT;
) > "%OPEN_SQL%"

(
    echo WHENEVER SQLERROR EXIT SQL.SQLCODE
    echo ALTER DATABASE OPEN RESETLOGS;
    echo ALTER PLUGGABLE DATABASE ALL OPEN;
    echo ALTER PLUGGABLE DATABASE ALL SAVE STATE;
    echo EXIT;
) > "%RESETLOGS_SQL%"

echo 1/3: Shutting down database and starting in MOUNT mode...
sqlplus / as sysdba @"%PREP_SQL%"
set "PREP_EXIT=%ERRORLEVEL%"
if not "%PREP_EXIT%"=="0" (
    echo ERROR: Cannot put database in MOUNT mode. Exit code: %PREP_EXIT%
    goto cleanup_fail
)

echo.
echo 2/3: Running RMAN restore/recover...
echo RMAN log file: %RMAN_LOG%
rman target / cmdfile="%RMAN_CMD%" log="%RMAN_LOG%"
set "RMAN_EXIT=%ERRORLEVEL%"
if not "%RMAN_EXIT%"=="0" (
    echo ERROR: RMAN restore failed. Exit code: %RMAN_EXIT%
    echo See log: %RMAN_LOG%
    goto cleanup_fail
)

echo.
echo 3/3: Opening database...
sqlplus / as sysdba @"%OPEN_SQL%"
set "OPEN_EXIT=%ERRORLEVEL%"
if not "%OPEN_EXIT%"=="0" (
    echo Normal open failed. Trying OPEN RESETLOGS...
    sqlplus / as sysdba @"%RESETLOGS_SQL%"
    set "OPEN_EXIT=!ERRORLEVEL!"
)

if not "%OPEN_EXIT%"=="0" (
    echo ERROR: Database restore finished but database could not be opened. Exit code: %OPEN_EXIT%
    goto cleanup_fail
)

del "%PREP_SQL%" "%OPEN_SQL%" "%RESETLOGS_SQL%" "%RMAN_CMD%" >nul 2>nul

echo.
echo =========================================
echo RMAN RESTORE COMPLETED.
echo Log: %RMAN_LOG%
echo =========================================
pause
exit /b 0

:cleanup_fail
del "%PREP_SQL%" "%OPEN_SQL%" "%RESETLOGS_SQL%" "%RMAN_CMD%" >nul 2>nul
echo.
echo Restore failed. Check log: %RMAN_LOG%
pause
exit /b 1
