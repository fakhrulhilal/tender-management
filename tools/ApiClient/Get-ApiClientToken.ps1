[CmdletBinding()]
param(
    [System.Uri]
    [ValidateNotNull()]
    $Url = 'https://localhost:5001',

    [string]
    [ValidateNotNullOrEmpty()]
    $ClientId = 'default-client',

    [string]
    [ValidateNotNullOrEmpty()]
    $ClientSecret = 'secret',

    [string]
    [ValidateNotNullOrEmpty()]
    $Username = 'administrator@localhost',

    [string]
    [ValidateNotNullOrEmpty()]
    $Password = 'Administrator1!',

    [string[]]
    $Scopes = @('TenderManagement.WebApiAPI', 'tender.read', 'tender.write', 'tender.delete')
)

Process {
    $AllScope = [string]::Join(' ', $Scopes)
    Invoke-RestMethod -Method Post -Body "grant_type=password&scope=$($AllScope)&client_id=$($ClientId)&client_secret=$($ClientSecret)&username=$($Username)&password=$Password" -Uri "$($Url)connect/token" -ContentType 'application/x-www-form-urlencoded'
}