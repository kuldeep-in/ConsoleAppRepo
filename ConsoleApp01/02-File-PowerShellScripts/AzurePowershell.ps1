
# Sign In
Connect-AzAccount

# Gets the Azure PowerShell context for the current PowerShell session
Get-AzContext
Get-AzContext -ListAvailable

# Get all of the Azure subscriptions in your current Azure tenant
Get-AzSubscription
Get-AzSubscription -TenantId $TenantId

# Set the Azure PowerShell context to a specific Azure subscription
Set-AzContext -TenantId '000000-000000-0000000-00000'
Set-AzContext -SubscriptionId '000000-000000-0000000-00000'

get-azresourcegroup | format-table -groupby location
new-azresourcegroup -name 'rg-demo01' -location 'eastus'

#ADF
get-azdatafactoryv2
set-azdatafactoryv2 -name 'demo-adf941' -location 'eastus' -resourcegroup 'rg-demo01'
