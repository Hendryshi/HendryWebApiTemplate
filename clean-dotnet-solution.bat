@echo off
SETLOCAL EnableDelayedExpansion

echo.
echo ================================
echo Cleaning .NET Solution...
echo ================================
echo.

REM 清理 bin 和 obj 文件夹
for /d /r %%d in (bin,obj) do (
    if exist "%%d" (
        echo Deleting %%d
        rmdir /s /q "%%d"
    )
)

REM 删除 .vs 文件夹（Visual Studio）
if exist ".vs" (
    echo Deleting .vs directory
    rmdir /s /q ".vs"
)

REM 删除 .vscode 文件夹（VS Code）
if exist ".vscode" (
    echo Deleting .vscode directory
    rmdir /s /q ".vscode"
)

REM 删除用户配置文件
for %%f in (*.user *.suo *.sln.docstates) do (
    echo Deleting %%f
    del /f /q "%%f"
)

echo.
echo ✅ Solution cleaned successfully.
echo.
pause