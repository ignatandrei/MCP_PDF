# This function updates the TargetFramework from net8.0 to net10.0 in a given .csproj file
function Update-CsprojToNet10 {
    param(
        [Parameter(Mandatory=$true)]
        [string]$CsprojPath
    )

    if (-Not (Test-Path $CsprojPath)) {
        Write-Error "File not found: $CsprojPath"
        return $false
    }

    [xml]$xml = Get-Content $CsprojPath
    $updated = $false
    $tfNodes = $xml.SelectNodes('//TargetFramework')
    foreach ($tf in $tfNodes) {
        if ($tf.InnerText.Trim() -eq 'net8.0') {
            $tf.InnerText = 'net10.0'
            $updated = $true
        }
    }
    if ($updated) {
        $xml.Save($CsprojPath)
        Write-Host "TargetFramework updated to net10.0 in $CsprojPath"
        return $true
    } else {
        Write-Warning "TargetFramework not found or already set to net10.0."
        return $false
    }
}

# Example usage:
Update-CsprojToNet10 -CsprojPath "src/MCP_PDF/MCP_PDF.csproj"
Update-CsprojToNet10 -CsprojPath "src/MCP_PDF.Tests/MCP_PDF.Tests.csproj"
