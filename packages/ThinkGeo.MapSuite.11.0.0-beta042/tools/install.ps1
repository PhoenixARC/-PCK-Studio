param($installPath, $toolsPath, $package, $project)

$args = @($installPath, $toolsPath)

$exename = $toolsPath + "\MapSuiteUnmanagedDependencyInstaller.exe" 

#& $exename $args
Start-Process -FilePath $exename -WindowStyle Hidden