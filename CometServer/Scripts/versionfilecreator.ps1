param (
    [string]$buildTarget,
    [string]$publishDir
)

# Ensure the build target is provided
if (-not $buildTarget) {
    Write-Host "No build target provided. Exiting."
    exit 1
}

# Set the path based on the build target
$binPath = if ($publishDir) { $publishDir } else { Join-Path -Path "bin" -ChildPath $buildTarget }

# Path to the output file in the target build directory
$outputFile = Join-Path -Path $binPath -ChildPath "VERSION"

# Echo the path of the VERSION file
Write-Host "The VERSION file will be created at: $outputFile"

# Clear the content of the output file or create it if it doesn't exist
if (Test-Path $outputFile) {
    Clear-Content $outputFile
} else {
    New-Item -Path $outputFile -ItemType File | Out-Null
}

# Normalize the bin path by ensuring it ends with a backslash
$binPath = [System.IO.Path]::GetFullPath($binPath) + [System.IO.Path]::DirectorySeparatorChar

# Get the list of DLLs in the output directory and all subdirectories
$dlls = Get-ChildItem -Path $binPath -Filter *.dll -Recurse

# Initialize lists for categorized DLLs
$cdp4CometDlls = @()
$otherDlls = @()

foreach ($dll in $dlls) {
    # Get the version information
    $versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($dll.FullName)

    # Get the relative path of the DLL starting from the output directory
    $relativeDllPath = $dll.FullName.Substring($binPath.Length)

    # Prepare the output line
    $outputLine = "$relativeDllPath $($versionInfo.FileVersion)"

    # Categorize DLLs
    if ($dll.Name -match "(?i)cdp4|comet") {
        $cdp4CometDlls += $outputLine
    } else {
        $otherDlls += $outputLine
    }
}

# Sort the other DLLs alphabetically
$otherDlls = $otherDlls | Sort-Object

# Write the categorized DLLs to the VERSION file
Add-Content -Path $outputFile -Value "CDP4-COMET:"
Add-Content -Path $outputFile -Value "----------------"
$cdp4CometDlls | ForEach-Object { Add-Content -Path $outputFile -Value $_ }

Add-Content -Path $outputFile -Value ""
Add-Content -Path $outputFile -Value "Dependencies:"
Add-Content -Path $outputFile -Value "----------------"
$otherDlls | ForEach-Object { Add-Content -Path $outputFile -Value $_ }

# Confirm the creation of the VERSION file
Write-Host "Version information written to $outputFile in $binPath"
