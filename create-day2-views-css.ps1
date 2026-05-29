# Day 02 - Create Razor Views and append basic CSS
# Run this script at solution root: D:\thanh-hai\app\ToeicLearningApp

$webProjectPath = ".\Toeic.Web"

if (-not (Test-Path $webProjectPath)) {
    Write-Error "Toeic.Web folder was not found. Please run this script at the solution root."
    exit 1
}

$viewsRoot = Join-Path $webProjectPath "Views"
$cssPath = Join-Path $webProjectPath "wwwroot\css\site.css"

# Create View folders
$viewFolders = @(
    "Dashboard",
    "Practice",
    "Vocabulary",
    "Grammar",
    "MockTest",
    "Admin"
)

foreach ($folder in $viewFolders) {
    $path = Join-Path $viewsRoot $folder

    if (-not (Test-Path $path)) {
        New-Item -Path $path -ItemType Directory | Out-Null
        Write-Host "Created folder: $path"
    }
    else {
        Write-Host "Folder already exists: $path"
    }
}

# Create View files
$views = @{
    "Dashboard\Index.cshtml" = @'
@{
    ViewData["Title"] = "Dashboard";
}

<h1>Dashboard</h1>

<p>This page will show TOEIC learning progress in future steps.</p>

<div class="card-grid">
    <div class="feature-card">
        <h3>Total Practice</h3>
        <p>Coming soon.</p>
    </div>
    <div class="feature-card">
        <h3>Accuracy</h3>
        <p>Coming soon.</p>
    </div>
    <div class="feature-card">
        <h3>Recommended Study</h3>
        <p>Coming soon.</p>
    </div>
</div>
'@

    "Practice\Index.cshtml" = @'
@{
    ViewData["Title"] = "Practice";
}

<h1>Practice</h1>

<p>Select TOEIC parts and practice questions. Question data will be added later.</p>

<div class="card-grid">
    <div class="feature-card">Part 1 - Photographs</div>
    <div class="feature-card">Part 2 - Question Response</div>
    <div class="feature-card">Part 3 - Conversations</div>
    <div class="feature-card">Part 4 - Talks</div>
    <div class="feature-card">Part 5 - Incomplete Sentences</div>
    <div class="feature-card">Part 6 - Text Completion</div>
    <div class="feature-card">Part 7 - Reading Comprehension</div>
</div>
'@

    "Vocabulary\Index.cshtml" = @'
@{
    ViewData["Title"] = "Vocabulary";
}

<h1>Vocabulary</h1>

<p>This page will show TOEIC vocabulary by topic.</p>

<div class="card-grid">
    <div class="feature-card">Office</div>
    <div class="feature-card">Meeting</div>
    <div class="feature-card">Travel</div>
    <div class="feature-card">Finance</div>
    <div class="feature-card">Recruitment</div>
    <div class="feature-card">Shopping</div>
</div>
'@

    "Grammar\Index.cshtml" = @'
@{
    ViewData["Title"] = "Grammar";
}

<h1>Grammar</h1>

<p>This page will contain grammar lessons and grammar review for TOEIC learners.</p>

<div class="card-grid">
    <div class="feature-card">Part of Speech</div>
    <div class="feature-card">Verb Tense</div>
    <div class="feature-card">Preposition</div>
    <div class="feature-card">Conjunction</div>
</div>
'@

    "MockTest\Index.cshtml" = @'
@{
    ViewData["Title"] = "Mock Test";
}

<h1>Mock Test</h1>

<p>This page will provide TOEIC mini tests and full mock tests in later steps.</p>

<div class="feature-card">
    <h3>TOEIC Mini Test</h3>
    <p>Coming soon.</p>
</div>
'@

    "Admin\Index.cshtml" = @'
@{
    ViewData["Title"] = "Admin";
}

<h1>Admin</h1>

<p>This page will be used to manage TOEIC questions, vocabulary, and mock tests later.</p>

<div class="card-grid">
    <div class="feature-card">Question Management</div>
    <div class="feature-card">Vocabulary Management</div>
    <div class="feature-card">Mock Test Management</div>
</div>
'@
}

foreach ($view in $views.GetEnumerator()) {
    $filePath = Join-Path $viewsRoot $view.Key
    $view.Value | Set-Content -Path $filePath -Encoding UTF8
    Write-Host "Created/Updated view: $filePath"
}

# Append CSS
if (-not (Test-Path $cssPath)) {
    New-Item -Path $cssPath -ItemType File -Force | Out-Null
    Write-Host "Created CSS file: $cssPath"
}

$cssMarker = "/* Day 02 TOEIC basic layout styles */"

$existingCss = Get-Content -Path $cssPath -Raw -ErrorAction SilentlyContinue

if ($existingCss -notlike "*$cssMarker*") {
    $cssContent = @"

$cssMarker

.hero-section {
    padding: 48px 24px;
    background: #f8fafc;
    border-radius: 16px;
    margin-bottom: 32px;
}

.hero-content {
    max-width: 800px;
}

.hero-content h1 {
    font-size: 40px;
    font-weight: 700;
    margin-bottom: 16px;
}

.hero-content p {
    font-size: 18px;
    color: #475569;
    margin-bottom: 24px;
}

.hero-actions {
    display: flex;
    gap: 12px;
    flex-wrap: wrap;
}

.card-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
    gap: 16px;
    margin-top: 24px;
}

.feature-card {
    padding: 20px;
    border: 1px solid #e5e7eb;
    border-radius: 12px;
    background: #ffffff;
    box-shadow: 0 1px 3px rgba(15, 23, 42, 0.08);
}

.feature-card h3 {
    margin-bottom: 8px;
    font-size: 20px;
}
"@

    Add-Content -Path $cssPath -Value $cssContent -Encoding UTF8
    Write-Host "Appended Day 02 CSS to: $cssPath"
}
else {
    Write-Host "Day 02 CSS already exists. Skipped appending CSS."
}

Write-Host ""
Write-Host "Done. Created views and CSS for Day 02."
Write-Host "Next commands:"
Write-Host "dotnet build"
Write-Host "dotnet run --project .\Toeic.Web\Toeic.Web.csproj"