@REM Creating logo
set iconFile="Link\icon_15341.svg"
inkscape.exe %iconFile% --export-png=logo_16.png -w16 -h16
inkscape.exe %iconFile% --export-png=logo_32.png -w32 -h32
inkscape.exe %iconFile% --export-png=logo_64.png -w64 -h64
inkscape.exe %iconFile% --export-png=logo_128.png -w128 -h128
inkscape.exe %iconFile% --export-png=logo_256.png -w256 -h256 
inkscape.exe %iconFile% --export-png=logo_512.png -w512 -h512
inkscape.exe %iconFile% --export-png=logo_1024.png -w1024 -h1024
pause