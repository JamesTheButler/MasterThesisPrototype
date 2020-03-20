SET /p IFile=Enter Input File name:
::SET /p OFile=Enter Output File name:
F:\Programme\TetWild\build\Debug\TetWild --input "%cd%\%IFile%" --output "%cd%\%IFile%.mesh" --max-pass 10 --is-laplacian 
PAUSE