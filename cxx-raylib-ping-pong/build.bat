@echo off

set PATH=%PATH%;c:\raylib\w64devkit\bin
set CXX="g++.exe"
set RAYLIB_INCLUDE="c:\raylib\raylib\src\**"
set APP=".\bin\app.exe"
if not exist ".\bin\" (
    mkdir .\bin
)
del %APP%
%CXX% .\src\*.*xx        ^
-Wall                    ^
-mwindows                ^
-Wextra                  ^
-Wanalyzer-out-of-bounds ^
-Waddress                ^
-Wanalyzer-infinite-loop ^
-Wanalyzer-fd-leak       ^
-I%RAYLIB_INCLUDE%       ^
-L%RAYLIB_INCLUDE%       ^
-lraylib -lopengl32 -lgdi32 -lwinmm ^
-pie                     ^
-o %APP%                 ^
-O3                      ^
-Og

%APP%