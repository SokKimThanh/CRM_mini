=====================================================================

Script: Thêm PostgreSQL 18 vào PATH và kiểm tra kết nối

Chạy script bằng PowerShell (Run as Administrator)

=====================================================================

$pg18Path = "C:\Program Files\PostgreSQL\18\bin"

1. Kiểm tra thư mục cài đặt có tồn tại hay không

if (-not (Test-Path $pg18Path)) {
Write-Warning "Không tìm thấy thư mục: $pg18Path"
Write-Host "Đang tự động tìm kiếm thư mục bin của PostgreSQL 18 trên ổ C:..." -ForegroundColor Yellow
$found = Get-ChildItem -Path "C:\Program Files\PostgreSQL" -Filter "bin" -Recurse -ErrorAction SilentlyContinue |
Where-Object { $_.FullName -like "18" } |
Select-Object -First 1
if ($found) {
$pg18Path =$found.FullName
Write-Host "Đã tìm thấy tại: $pg18Path" -ForegroundColor Green
} else {
Write-Error "Không tìm thấy thư mục cài đặt PostgreSQL 18. Vui lòng kiểm tra lại ổ đĩa cài đặt."
exit 1
}
}

2. Thêm vĩnh viễn vào System PATH (nếu chưa có)

$currentMachinePath = [Environment]::GetEnvironmentVariable("Path", [EnvironmentVariableTarget]::Machine)
if ($currentMachinePath -notlike "$pg18Path") {
[Environment]::SetEnvironmentVariable(
"Path",
"$currentMachinePath;$pg18Path",
[EnvironmentVariableTarget]::Machine
)
Write-Host "[OK] Đã ghi nhận đường dẫn vào System PATH." -ForegroundColor Green
} else {
Write-Host "[INFO] Đường dẫn đã tồn tại sẵn trong System PATH." -ForegroundColor Cyan
}

3. Nạp lại biến môi trường cho phiên PowerShell hiện tại

$env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")

4. Kiểm tra dịch vụ và các tiện ích CLI

Write-Host "`n--- KIỂM TRA POSTGRESQL 18 CLI ---" -ForegroundColor Yellow

try {
& psql --version
& pg_isready -h localhost -p 5432
} catch {
Write-Error "Không thể gọi psql hoặc pg_isready. Hãy đóng cửa sổ PowerShell và mở lại."
}