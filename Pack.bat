@echo off

mkdir pack
mkdir pack\lib
mkdir pack\lib\net40

copy source\alite.core\bin\release\ALite.Core.dll pack\lib\net40
copy source\alite.core\bin\release\ALite.Core.xml pack\lib\net40

copy source\alite.objectvalidator\bin\release\ALite.ObjectValidator.dll pack\lib\net40
copy source\alite.objectvalidator\bin\release\ALite.ObjectValidator.xml pack\lib\net40

copy source\alite.sql\bin\release\ALite.Sql.dll pack\lib\net40
copy source\alite.sql\bin\release\ALite.Sql.xml pack\lib\net40

copy ALite.nuspec pack