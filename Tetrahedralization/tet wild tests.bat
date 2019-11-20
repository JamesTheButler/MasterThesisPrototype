SET /p IFile=Enter Input File name:
::SET /p OFile=Enter Output File name:
F:\Programme\TetWild\build\Debug\TetWild --input "%cd%\%IFile%" --output "%cd%\%IFile%.mesh" -e 0.00075 --max-pass 10
PAUSE