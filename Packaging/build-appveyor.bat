ECHO APPVEYOR_REPO_BRANCH: %APPVEYOR_REPO_BRANCH%
ECHO APPVEYOR_REPO_TAG: %APPVEYOR_REPO_TAG%
ECHO APPVEYOR_BUILD_NUMBER : %APPVEYOR_BUILD_NUMBER%
ECHO APPVEYOR_BUILD_VERSION : %APPVEYOR_BUILD_VERSION%

Call Tools\Nuget.exe restore ..\Source\Our.Umbraco.RedirectsViewer\Our.Umbraco.RedirectsViewer.sln
CALL "%programfiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MsBuild.exe" build.xml