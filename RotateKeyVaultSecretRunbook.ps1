param (
    [Parameter(Mandatory=$True)]
    [string]
    $keyVaultName,
    [Parameter(Mandatory=$True)]
    [string]
    $secretName
)

# Authenticate to Azure if running from Azure Automation
Write-Output "Logging in...";
$servicePrincipalConnection = Get-AutomationConnection -Name "AzureRunAsConnection"
Login-AzureRmAccount `
    -ServicePrincipal `
    -TenantId $servicePrincipalConnection.TenantId `
    -ApplicationId $servicePrincipalConnection.ApplicationId `
    -CertificateThumbprint $servicePrincipalConnection.CertificateThumbprint | Write-Verbose

# Retrieve current secret value
Write-Output "Retrieving secret " $secretName " from vault " $keyVaultName
$secret = Get-AzureKeyVaultSecret -vaultName $keyVaultName -name $secretName
Write-Output "Current secret value is " $secret.SecretValueText

# Generate new secret value and update secret
$guid = New-Guid
Write-Output "New secret value is " $guid

# Add code here to actually update the password on the associated user account in AAD

$secretValue = ConvertTo-SecureString $guid -AsPlainText -Force
Write-Output "Updating secret..."
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name $secretName -SecretValue $secretValue