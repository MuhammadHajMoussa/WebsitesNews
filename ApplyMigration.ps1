# PowerShell script to apply SQL migration
# Execute AddAvatarPath.sql on the database

$connectionString = "Server=(localdb)\mssqllocaldb;Database=NewsSiteDb;Trusted_Connection=true;"
$sqlScript = Get-Content -Path "AddAvatarPath.sql" -Raw

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connectionString
    $connection.Open()

    $command = $connection.CreateCommand()
    $command.CommandText = $sqlScript
    $command.ExecuteNonQuery()

    Write-Host "✓ SQL Migration applied successfully!" -ForegroundColor Green

    $connection.Close()
}
catch {
    Write-Host "✗ Error applying SQL migration:" -ForegroundColor Red
    Write-Host $_.Exception.Message
}
