REM Deleting output
FOR %%A in (bin obj\debug obj\release) do IF EXIST "%~dp0%%A" rd /s /q "%~dp0%%A"

@pause
