$violations = @()
Get-ChildItem -Path "src\Crm.Web" -Filter "*.razor" -Recurse | ForEach-Object {
    $file = $_
    Get-Content $file.FullName | ForEach-Object -Begin { $line = 0 } -Process {
        $line++
        if ($_ -match 'style\s*=\s*["''][^"'']*["'']') {
            if ($_ -notmatch '@\(' -and $_ -notmatch '@\$') {
                $violations += "$($file.Name):$line -> $($_.Trim())"
            }
        }
    }
}

if ($violations.Count -gt 0) {
    Write-Host "Vẫn còn inline style tĩnh:" -ForegroundColor Red
    $violations | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
} else {
    Write-Host ">> Zero-Inline Audit PASS: 100% sạch." -ForegroundColor Green
}