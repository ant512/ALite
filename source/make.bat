@ECHO OFF

REM Set variables
SET dll=ALite.dll
SET src=%cd%
SET dst=bin
SET doc=/doc:%src%\%dst%\ALite.xml
SET csc=C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc.exe /nologo /optimize
SET out=/out:"%src%\%dst%\%dll%"
SET target=/target:library
SET recurse=/recurse:"%src%\*.cs"

SET ref=

IF EXIST "%src%\%dst%" GOTO DELDESTDIR

:BUILD
REM Create output directory
MKDIR "%src%\%dst%"

REM Compile
%csc% %out% %target% %ref% %recurse% %doc%

EXIT

REM Remove existing destination directory  and files
:DELDESTDIR
RMDIR /S /Q "%src%\%dst%"
GOTO BUILD