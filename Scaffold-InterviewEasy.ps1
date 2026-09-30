# ============================================
# InterviewEasy — Full Solution Scaffolding
# Target: C:\Users\PC\InterviewHub
# .NET 9, Web API, Class Library, xUnit
# ============================================

$root = "C:\Users\PC\InterviewHub"

# --- 1. Create root directory ---
if (-not (Test-Path $root)) {
    New-Item -ItemType Directory -Path $root -Force
}
Set-Location $root

Write-Host "=== Creating Solution ===" -ForegroundColor Cyan
dotnet new sln -n InterviewEasy

# ============================================
# 2. BUILDING BLOCKS (Class Libraries)
# ============================================
Write-Host "=== Creating BuildingBlocks ===" -ForegroundColor Cyan

$buildingBlocks = @(
    "BuildingBlocks\InterviewEasy.BuildingBlocks.Core",
    "BuildingBlocks\InterviewEasy.BuildingBlocks.EventBus",
    "BuildingBlocks\InterviewEasy.BuildingBlocks.Common",
    "BuildingBlocks\InterviewEasy.BuildingBlocks.Observability"
)

foreach ($proj in $buildingBlocks) {
    dotnet new classlib -n (Split-Path $proj -Leaf) -o "src\$proj" -f net9.0
    dotnet sln add "src\$proj"
}

# ============================================
# 3. SERVICES (4-layer Clean Architecture each)
# ============================================
Write-Host "=== Creating Services ===" -ForegroundColor Cyan

$services = @(
    "Identity",
    "Requirement",
    "Question",
    "Scheduling",
    "Session",
    "Feedback",
    "Sandbox",
    "Proctoring",
    "Notification",
    "Recording",
    "Analytics"
)

foreach ($svc in $services) {
    $base = "Services\$svc"
    
    # API (Web API)
    dotnet new webapi -n "InterviewEasy.$svc.API" -o "src\$base\InterviewEasy.$svc.API" -f net9.0
    dotnet sln add "src\$base\InterviewEasy.$svc.API"
    
    # Application (Class Library)
    dotnet new classlib -n "InterviewEasy.$svc.Application" -o "src\$base\InterviewEasy.$svc.Application" -f net9.0
    dotnet sln add "src\$base\InterviewEasy.$svc.Application"
    
    # Domain (Class Library)
    dotnet new classlib -n "InterviewEasy.$svc.Domain" -o "src\$base\InterviewEasy.$svc.Domain" -f net9.0
    dotnet sln add "src\$base\InterviewEasy.$svc.Domain"
    
    # Infrastructure (Class Library)
    dotnet new classlib -n "InterviewEasy.$svc.Infrastructure" -o "src\$base\InterviewEasy.$svc.Infrastructure" -f net9.0
    dotnet sln add "src\$base\InterviewEasy.$svc.Infrastructure"
}

# ============================================
# 4. BFF (Backend-for-Frontend)
# ============================================
Write-Host "=== Creating BFFs ===" -ForegroundColor Cyan

$bffs = @("Admin", "Customer", "Candidate")

foreach ($bff in $bffs) {
    dotnet new webapi -n "InterviewEasy.Bff.$bff" -o "src\Services\Bff\InterviewEasy.Bff.$bff" -f net9.0
    dotnet sln add "src\Services\Bff\InterviewEasy.Bff.$bff"
}

# ============================================
# 5. GATEWAY
# ============================================
Write-Host "=== Creating Gateway ===" -ForegroundColor Cyan

dotnet new webapi -n "InterviewEasy.Gateway" -o "src\Gateways\InterviewEasy.Gateway" -f net9.0
dotnet sln add "src\Gateways\InterviewEasy.Gateway"

# ============================================
# 6. TEST PROJECTS (xUnit v3)
# ============================================
Write-Host "=== Creating Test Projects ===" -ForegroundColor Cyan

# Install xUnit v3 templates first
dotnet new install xunit.v3.templates

foreach ($svc in $services) {
    # Unit Tests
    dotnet new xunit3 -n "InterviewEasy.$svc.UnitTests" -o "tests\UnitTests\InterviewEasy.$svc.UnitTests" -f net9.0
    dotnet sln add "tests\UnitTests\InterviewEasy.$svc.UnitTests"
    
    # Integration Tests
    dotnet new xunit3 -n "InterviewEasy.$svc.IntegrationTests" -o "tests\IntegrationTests\InterviewEasy.$svc.IntegrationTests" -f net9.0
    dotnet sln add "tests\IntegrationTests\InterviewEasy.$svc.IntegrationTests"
}

# ============================================
# 7. DIRECTORY.PACKAGES.PROPS (Central Package Management)
# ============================================
Write-Host "=== Creating Directory.Packages.props ===" -ForegroundColor Cyan

dotnet new packagesprops

# ============================================
# 8. DIRECTORY.BUILD.PROPS
# ============================================
Write-Host "=== Creating Directory.Build.props ===" -ForegroundColor Cyan

@"
<Project>
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Company>InterviewEasy</Company>
    <Version>1.0.0</Version>
  </PropertyGroup>
</Project>
"@ | Out-File -FilePath "Directory.Build.props" -Encoding utf8

# ============================================
# 9. .gitignore
# ============================================
Write-Host "=== Creating .gitignore ===" -ForegroundColor Cyan

dotnet new gitignore

# ============================================
# 10. Build Solution
# ============================================
Write-Host "=== Building Solution ===" -ForegroundColor Cyan

dotnet build

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "  InterviewEasy scaffolding complete!" -ForegroundColor Green
Write-Host "  Location: $root" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Review the solution in Visual Studio or VS Code"
Write-Host "  2. Add project references between layers"
Write-Host "  3. Configure NuGet packages in Directory.Packages.props"
Write-Host "  4. Run: git init && git add . && git commit -m 'Initial scaffold'"
Write-Host "  5. Run: git remote add origin https://github.com/intervieweasy/InterviewEasy.git"
Write-Host "  6. Run: git push -u origin main"