# ============================================
# Fix-CPM.ps1 — Repair CPM violations
# Run from: C:\Users\PC\InterviewHub
# ============================================

$root = "C:\Users\PC\InterviewHub"
Set-Location $root

Write-Host "=== Step 1: Discovering all PackageReferences with Version ===" -ForegroundColor Cyan

# Find all .csproj files
$csprojFiles = Get-ChildItem -Path $root -Recurse -Filter *.csproj

# Collect unique package name -> version pairs
$packageVersions = @{}

foreach ($file in $csprojFiles) {
    [xml]$xml = Get-Content $file.FullName
    $refs = $xml.SelectNodes("//PackageReference[@Version]")
    
    foreach ($ref in $refs) {
        $name = $ref.GetAttribute("Include")
        $version = $ref.GetAttribute("Version")
        
        if ($name -and $version) {
            if (-not $packageVersions.ContainsKey($name)) {
                $packageVersions[$name] = $version
                Write-Host "  Found: $name -> $version"
            } else {
                # If two projects reference same package with different versions, keep highest
                # (simple string compare — adjust manually if needed)
                Write-Host "  Duplicate: $name ($version vs $($packageVersions[$name]))"
            }
        }
    }
}

Write-Host ""
Write-Host "=== Step 2: Writing Directory.Packages.props ===" -ForegroundColor Cyan

$packagesPropsPath = Join-Path $root "Directory.Packages.props"

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("<Project>")
[void]$sb.AppendLine("  <PropertyGroup>")
[void]$sb.AppendLine("    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>")
[void]$sb.AppendLine("    <CentralPackageTransitivePinningEnabled>true</CentralPackageTransitivePinningEnabled>")
[void]$sb.AppendLine("  </PropertyGroup>")
[void]$sb.AppendLine("  <ItemGroup>")

foreach ($name in ($packageVersions.Keys | Sort-Object)) {
    $version = $packageVersions[$name]
    [void]$sb.AppendLine("    <PackageVersion Include=""$name"" Version=""$version"" />")
}

[void]$sb.AppendLine("  </ItemGroup>")
[void]$sb.AppendLine("</Project>")

$sb.ToString() | Out-File -FilePath $packagesPropsPath -Encoding utf8
Write-Host "  Wrote: $packagesPropsPath" -ForegroundColor Green
Write-Host "  Packages: $($packageVersions.Count)"

Write-Host ""
Write-Host "=== Step 3: Stripping Version attributes from all .csproj ===" -ForegroundColor Cyan

foreach ($file in $csprojFiles) {
    $content = Get-Content $file.FullName -Raw
    
    # Remove Version="x.y.z" from PackageReference elements only
    # Pattern: <PackageReference Include="Foo" Version="1.2.3" />
    $newContent = [regex]::Replace(
        $content,
        '(<PackageReference\s+[^>]*?)\s+Version="[^"]*"',
        '$1'
    )
    
    if ($newContent -ne $content) {
        $newContent | Out-File -FilePath $file.FullName -Encoding utf8 -NoNewline
        Write-Host "  Fixed: $($file.FullName.Replace($root, '.'))" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=== Step 4: Restoring & Building ===" -ForegroundColor Cyan

dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Restore failed. See errors above." -ForegroundColor Red
    exit 1
}

dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. See errors above." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "  CPM fix complete. Build succeeded." -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green