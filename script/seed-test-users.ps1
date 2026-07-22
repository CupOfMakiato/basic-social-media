#!/usr/bin/env pwsh

param(
    [string]$ApiBase = "http://localhost:5148/api/Auth",
    [Parameter(Mandatory = $true)]
    [string]$Password
)

$ErrorActionPreference = "Stop"

if ($Password.Length -lt 5 -or
    -not ($Password.ToCharArray() | Where-Object { [char]::IsUpper($_) }) -or
    -not ($Password.ToCharArray() | Where-Object { [char]::IsDigit($_) }) -or
    -not ($Password.ToCharArray() | Where-Object { -not [char]::IsLetterOrDigit($_) })) {
    throw "Password must be at least 5 characters and include uppercase, number, and special character."
}

$registerUrl = "$($ApiBase.TrimEnd('/'))/register"
Write-Host "Register URL: $registerUrl"

$users = @(
    @{
        UserName = "user1"
        Email    = "user1@example.com"
    },
    @{
        UserName = "user2"
        Email    = "user2@example.com"
    }
)

$failed = $false

function Get-ErrorBody {
    param($ErrorRecord)

    if ($ErrorRecord.ErrorDetails.Message) {
        return $ErrorRecord.ErrorDetails.Message
    }

    if ($ErrorRecord.Exception.Response) {
        $stream = $ErrorRecord.Exception.Response.GetResponseStream()
        if ($stream) {
            $reader = [System.IO.StreamReader]::new($stream)
            return $reader.ReadToEnd()
        }
    }

    return ""
}

foreach ($user in $users) {
    $payload = @{
        userName = $user.UserName
        email    = $user.Email
        password = $Password
    }

    Write-Host "`n--- Registering $($user.UserName): $($user.Email) ---"

    try {
        $response = Invoke-RestMethod `
            -Method POST `
            -Uri $registerUrl `
            -ContentType "application/json" `
            -Body ($payload | ConvertTo-Json -Compress) `
            -ErrorAction Stop

        Write-Host "SUCCESS: $($response | ConvertTo-Json -Compress)"
    }
    catch {
        $body = Get-ErrorBody $_

        if ($body -match "already exists") {
            Write-Host "SKIPPED: user already exists"
            continue
        }

        $failed = $true
        $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { $null }
        Write-Host "FAILED$(if ($statusCode) { " (HTTP $statusCode)" }): $($_.Exception.Message)"
        if ($body) {
            Write-Host "Response body: $body"
        }
    }
}

if ($failed) {
    exit 1
}

Write-Host "`nDone."
