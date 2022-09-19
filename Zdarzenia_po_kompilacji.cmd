@echo off
chcp 65001

::Debug
xcopy "!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" "PWRlangConverter\bin\Debug\net6.0" /Y
if exist "PWRlangConverter\bin\Debug\net6.0\!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" (
   move /Y "PWRlangConverter\bin\Debug\net6.0\!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" "PWRlangConverter\bin\Debug\net6.0\cfg.json"
)
if exist "kompatybilność z win7-x64\api-ms-win-core-winrt-l1-1-0.dll" (
  xcopy "kompatybilność z win7-x64\api-ms-win-core-winrt-l1-1-0.dll" "PWRlangConverter\bin\Debug\net6.0\" /Y
)

::Release_publish
xcopy "!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" "PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\" /Y
if exist "PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" (
   move /Y "PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" "PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\cfg.json"
)
if exist "kompatybilność z win7-x64\api-ms-win-core-winrt-l1-1-0.dll" (
  xcopy "kompatybilność z win7-x64\api-ms-win-core-winrt-l1-1-0.dll" "PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\" /Y
)
::if exist "!INSTRUKCJA_IMPLEMENTACJI.txt" (
::  xcopy "!INSTRUKCJA_IMPLEMENTACJI.txt" "PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\" /Y
::)



