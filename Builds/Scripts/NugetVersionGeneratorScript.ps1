$majorNugetNumber = $args[0]
$minorNugetVersion = $args[1]
$revision = $args[2]

$CI_Version = "$majorNugetNumber.$minorNugetVersion.$revision"



Write-Host  ("##vso[task.setvariable variable=CI_Version;]$CI_Version")