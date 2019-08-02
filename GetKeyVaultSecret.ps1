param (
    [Parameter(Mandatory=$True)]
    [string]
    $servicePrincipal,
    [Parameter(Mandatory=$True)]
    [string]
    $spPassword,
    [Parameter(Mandatory=$True)]
    $tenantId,
    [Parameter(Mandatory=$True)]
    $subscriptionId,
    [Parameter(Mandatory=$True)]
    [string]
    $keyVaultName,
    [Parameter(Mandatory=$True)]
    [string]
    $secretName
)

$passwd = ConvertTo-SecureString $spPassword -AsPlainText -Force
$pscredential = New-Object System.Management.Automation.PSCredential($servicePrincipal, $passwd)
Connect-AzAccount -ServicePrincipal -Credential $pscredential -TenantId $tenantId

Set-AzContext -Subscription $subscriptionId
$secret = Get-AzKeyVaultSecret -vaultName $keyVaultName -name $secretName
Write-Host "Value of secret " $secretName " is " $secret.SecretValueText