# ConsoleAppRepo

# Azure Networking

## VNETs
- Follow Classless inter-domain routing (CIDR)
- Ensure non-overlapping address spaces. Make sure your VNet address space (CIDR block) does not overlap with your organization's other network ranges
- Your subnets should not cover the entire address space of the VNet. Plan ahead and reserve some address space for the future
- It is recommended you have fewer large VNets than multiple small VNets. This will prevent management overhead.
- Secure your VNets by assigning Network Security Groups (NSGs) to the subnets beneath them

## NSG
- Azure NSG can help you in filtering inbound and outbound network traffic from azure resources.

## VNET Service End points
- secure your critical Azure service
- Service Endpoints enables private IP addresses in the VNet to reach the endpoint of an Azure service without needing a public IP address on the VNet

## Putting it all together
- Demilitarized zone (DMZ
- network virtual appliance (NVA

## Demo - Network
- IP Address
- vNet
  - Address spaces
  - Subnets
  - Service Endpoints
- NSG
  - Inbound security rules
  - outbound security rules
  - Subnets
- Storage account
  - Networking
- SQL Server
  - Firewall and virtual network


# Dev Test Labs

## Demo - DevTest Lab
- Create DevTest Lab
- My virtual machines
- my environments
- Configuration and policies

# Infra as a code

## Demo - IaaC
- Resource provider
- Resources
- Resource groups
- Create template
  - VS
  - Az portal
  - Text edditor
- deploy template
  - VS
  - powershell
  - Azure portal

# Azure Security

## Top 10
- 10.Web application firewall protects your web applications from common exploits and vulnerabilities, like sql enjection or cross site scripting 

## Demo - Security
- Azure security Center
- Azure Key vault
