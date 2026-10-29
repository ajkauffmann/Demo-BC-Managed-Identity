# Replace these placeholders as appropriate
$managedIdentityDisplayName = '<managed-identity-display-name>'
$tenantId = '<tenant-id>'
$role = 'API.ReadWrite.All'
 
# Login to Azure
Connect-MgGraph -Scopes Application.Read.All, AppRoleAssignment.ReadWrite.All -TenantId $tenantId
 
# Get the service principal details for your MI and for BC
$managedIdentityServicePrincipal = Get-MgServicePrincipal -Filter "displayName eq '$managedIdentityDisplayName'"
$businessCentralServicePrincipal = (Get-MgServicePrincipal -Filter "displayName eq 'Dynamics 365 Business Central'")
 
# Assign the AppRole to the managed identity
$appRole = $businessCentralServicePrincipal.AppRoles | Where-Object Value -eq $role
Write-Host "Assigning $($appRole.Value)"
New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $managedIdentityServicePrincipal.Id `
                                        -PrincipalId $managedIdentityServicePrincipal.Id `
                                        -ResourceId $businessCentralServicePrincipal.Id `
                                        -AppRoleId $appRole.Id

Get-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $managedIdentityServicePrincipal.Id
