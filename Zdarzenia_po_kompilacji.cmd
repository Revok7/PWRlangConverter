@echo off
chcp 65001

::Debug
xcopy "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\PWRlangConverter\bin\Debug\net6.0" /Y
if exist "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\PWRlangConverter\bin\Debug\net6.0\!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" (
   move /Y "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\PWRlangConverter\bin\Debug\net6.0\!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\PWRlangConverter\bin\Debug\net6.0\cfg.json"
)
if exist "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\kompatybilność z win7-x64\api-ms-win-core-winrt-l1-1-0.dll" (
  xcopy "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\kompatybilność z win7-x64\api-ms-win-core-winrt-l1-1-0.dll" "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\PWRlangConverter\bin\Debug\net6.0\" /Y
)

::Release_publish
xcopy "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\" /Y
if exist "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" (
   move /Y "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\!!!plik_konfiguracyjny_autokopiowany_po_kompilacji.json" "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\cfg.json"
)
if exist "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\kompatybilność z win7-x64\api-ms-win-core-winrt-l1-1-0.dll" (
  xcopy "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\kompatybilność z win7-x64\api-ms-win-core-winrt-l1-1-0.dll" "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\" /Y
)
if exist "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\!INSTRUKCJA_IMPLEMENTACJI.txt" (
  xcopy "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\!INSTRUKCJA_IMPLEMENTACJI.txt" "C:\Users\revok\Documents\Visual Studio 2022\Revok-projekty\C#\PWRlangConverter\PWRlangConverter\bin\Release_publish\net6.0\PWRlangConverter_e1ps_winx64\" /Y
)



