rmdir c:\packages\EntityFramework /S /Q
mkdir c:\packages\EntityFramework
cd src
cd EFCore
del bin\Debug\*.nupkg
dotnet pack --version-suffix jc-%1
copy bin\Debug\*.symbols.nupkg c:\packages\EntityFramework
cd ..
cd EFCore.Relational
del bin\Debug\*.nupkg
dotnet pack --version-suffix jc-%1
copy bin\Debug\*.symbols.nupkg c:\packages\EntityFramework
cd ..
cd EFCore.SqlServer
del bin\Debug\*.nupkg
dotnet pack --version-suffix jc-%1
copy bin\Debug\*.symbols.nupkg c:\packages\EntityFramework
cd ..
cd EFCore.Design
del bin\Debug\*.nupkg
dotnet pack --version-suffix jc-%1
copy bin\Debug\*.symbols.nupkg c:\packages\EntityFramework
cd ..
cd ..