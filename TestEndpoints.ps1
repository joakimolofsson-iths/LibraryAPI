
# This script is used to test the endpoints
param(
    [string]$environment = "Development",
    [string]$launchProfile = "https-Development",
    [string]$connectionStringKey = "BooksDb",
    [bool]$dropDatabase = $false,
    [bool]$createDatabase = $false
)

# Get the project name
$projectName = Get-ChildItem -Recurse -Filter "*.csproj" | Select-Object -First 1 | ForEach-Object { $_.Directory.Name } # Projectname can also be set manually

# Get the base URL of the project
$launchSettings = Get-Content -LiteralPath ".\$projectName\Properties\launchSettings.json" | ConvertFrom-Json
$baseUrl = ($launchSettings.profiles.$launchProfile.applicationUrl -split ";")[0] # Can also set manually -> $baseUrl = "https://localhost:7253"

#Install module SqlServer
if (-not (Get-Module -ErrorAction Ignore -ListAvailable SqlServer)) {
    Write-Verbose "Installing SqlServer module for the current user..."
    Install-Module -Scope CurrentUser SqlServer -ErrorAction Stop
}
Import-Module SqlServer

# Set the environment variable
$env:ASPNETCORE_ENVIRONMENT = $environment



# Read the connection string from appsettings.Development.json
$appSettings = Get-Content ".\$projectName\appsettings.$environment.json" | ConvertFrom-Json
$connectionString = $appSettings.ConnectionStrings.$connectionStringKey
Write-Host "Database Connection String: $connectionString" -ForegroundColor Blue


# Get the database name from the connection string
if ($connectionString -match "Database=(?<dbName>[^;]+)")
{
    $databaseName = $matches['dbName']
    Write-Host "Database Name: $databaseName" -ForegroundColor Blue
}else{
    Write-Host "Database Name not found in connection string" -ForegroundColor Red
    exit
}


# Check if the database exists
$conStringDbExcluded = $connectionString -replace "Database=[^;]+;", ""
$queryDbExists = Invoke-Sqlcmd -ConnectionString $conStringDbExcluded -Query "Select name FROM sys.databases WHERE name='$databaseName'"
if($queryDbExists){
    if($dropDatabase -or (Read-Host "Do you want to drop the database? (y/n)").ToLower() -eq "y") { 

        # Drop the database
        Invoke-Sqlcmd -ConnectionString $connectionString -Query  "USE master;ALTER DATABASE $databaseName SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE $databaseName;"
        Write-Host "Database $databaseName dropped." -ForegroundColor Green
    }
}

# Create the database from the model
if(Select-String -LiteralPath ".\$projectName\Program.cs" -Pattern "EnsureCreated()"){
    Write-Host "The project uses EnsureCreated() to create the database from the model." -ForegroundColor Yellow
} else {
    if($createDatabase -or (Read-Host "Should dotnet ef migrate and update the database? (y/n)").ToLower() -eq "y") { 

        dotnet ef migrations add "UpdateModelFromScript_$(Get-Date -Format "yyyyMMdd_HHmmss")" --project ".\$projectName\$projectName.csproj"
        dotnet ef database update --project ".\$projectName\$projectName.csproj"
    }
}

# Run the application
if((Read-Host "Start the server from Visual studio? (y/n)").ToLower() -ne "y") { 
    Start-Process -FilePath "dotnet" -ArgumentList "run --launch-profile $launchProfile --project .\$projectName\$projectName.csproj" -WindowStyle Normal    
    Write-Host "Wait for the server to start..." -ForegroundColor Yellow 
}

# Continue with the rest of the script
Read-Host "Press Enter to continue when the server is started..."



### =============================================================
### =============================================================
### =============================================================

# Send requests to the API endpoint




### Copy below code to test the endpoints




###

function Call-RestApi {
    param (
        [string]$HttpMethod,
        [string]$Endpoint,
        [string]$Body = $null
    )
    try {
        $response = Invoke-RestMethod -Uri $Endpoint -Method $HttpMethod -Body $Body -ContentType "application/json"
        $response | Format-Table
    } catch {
        Write-Host "Error during API call: $($_.Exception.Message)" -ForegroundColor Red
    }
}

### ------------ Post Authors
Write-Host "`nCreate an author"

$httpMethod = "Post"
$endPoint = "$baseUrl/api/Authors"
$json = '{ 
    "firstName": "Stephen", 
    "lastName": "King" 
}'
Call-RestApi -HttpMethod $httpMethod -Endpoint $endPoint -Body $json

### ------------ Post Books
Write-Host "`nCreate a book"

$httpMethod = "Post"
$endPoint = "$baseUrl/api/Books"
$json = '{ 
    "title": "The Shining",
    "isbn": "9780307743657",
    "yearPublished": 1977,
    "rating": 10,
    "copies": 3,
    "authorIds": [
        1
    ]
}'
Call-RestApi -HttpMethod $httpMethod -Endpoint $endPoint -Body $json

### ------------ Post Members
Write-Host "`nCreate a member"

$httpMethod = "Post"
$endPoint = "$baseUrl/api/Members"
$json = '{ 
    "firstName": "Joakim",
    "lastName": "Olofsson"
}'
Call-RestApi -HttpMethod $httpMethod -Endpoint $endPoint -Body $json

### ------------ Get Books
Write-Host "`nList all books"

$httpMethod = "Get"
$endPoint = "$baseUrl/api/Books"
Call-RestApi -HttpMethod $httpMethod -Endpoint $endPoint

### ------------ Get Books ID
Write-Host "`nGet a specific book"

$bookId = 1
$httpMethod = "Get"
$endPoint = "$baseUrl/api/Books/$bookId"
Call-RestApi -HttpMethod $httpMethod -Endpoint $endPoint

### ------------ Get BookCopies
Write-Host "`nList all book copies"

$httpMethod = "Get"
$endPoint = "$baseUrl/api/BookCopies"
Call-RestApi -HttpMethod $httpMethod -Endpoint $endPoint

### ------------ Post Loans
Write-Host "`nBorrow a book"

$httpMethod = "Post"
$endPoint = "$baseUrl/api/Loans"
$json = '{ 
    "bookCopyId": 1,
    "memberId": 1
}'
Call-RestApi -HttpMethod $httpMethod -Endpoint $endPoint -Body $json

### ------------ Put Loans Return
Write-Host "`nReturn a book"

$loanId = 1
$httpMethod = "Put"
$endPoint = "$baseUrl/api/Loans/$loanId/return"
$json = '{}'
Call-RestApi -HttpMethod $httpMethod -Endpoint $endPoint -Body $json

### ------------ Delete Members
Write-Host "`nDelete a member"

$memberId = 1
$httpMethod = "Delete"
$endPoint = "$baseUrl/api/Members/$memberId"
Call-RestApi -HttpMethod $httpMethod -Endpoint $endPoint

### ------------ Delete Books
Write-Host "`nDelete a book"

$bookId = 1
$httpMethod = "Delete"
$endPoint = "$baseUrl/api/Books/$bookId"
Call-RestApi -HttpMethod $httpMethod -Endpoint $endPoint

### ------------ Delete Authors
Write-Host "`nDelete an author"

$authorId = 1
$httpMethod = "Delete"
$endPoint = "$baseUrl/api/Authors/$authorId"
Call-RestApi -HttpMethod $httpMethod -Endpoint $endPoint

### ------------ Query Author from the database
# $sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "Select * FROM Authors"
# $sqlResult | Format-Table