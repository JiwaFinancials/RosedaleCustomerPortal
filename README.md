# Jiwa Customer Portal

##### Table of Contents  
[About](#About)

[Functions](#Functions)

[Deployment](#Deployment)

# About
A simple web portal which uses the Jiwa 8 REST API to provide basic account functions.

The portal is built using ASP.NET 9, ServiceStack Client 8.4.4 and Bootstrap 5.3.3

# Functions

* Add, Edit and Remove contacts
* View Mailing and Delivery Addresses
* View Balances, Transactions and Statements
* View Sales Orders
* View Quotes
* View Backorders
* Password change
* Forgot password
* Dark Mode / Light Mode

Users of the portal authenticate using their email address and password from the debtor contact name.

Users are able to request a password reset token to be emailed to them.

Only users with the debtor contact name tag "Customer Web Portal - User" or "Customer Web Portal - Admin" are able to login

Only users with the debtor contact name tag "Customer Web Portal - Admin" are able edit or delete contact records.

![image](https://github.com/user-attachments/assets/e62b6f58-7d71-47a9-bf91-b7063be35b67)

![image](https://github.com/user-attachments/assets/28e9247f-5c1c-4215-a733-e41ee6bcd515)

![image](https://github.com/user-attachments/assets/825e8e74-76cc-4711-a4f5-aac5c32e9e0f)

![image](https://github.com/user-attachments/assets/b9d36636-3e14-48a7-8b6b-0b6b35e5f8a8)

![image](https://github.com/user-attachments/assets/5656bc11-6689-44fe-9303-c1c9d3c5bc43)

![image](https://github.com/user-attachments/assets/e9d437ed-11fe-454c-b7f1-4299a6a5a5e8)

![image](https://github.com/user-attachments/assets/d3b51b13-2431-4922-9146-ab7c6a40663b)

![image](https://github.com/user-attachments/assets/aa1d6822-bea7-49cc-944e-a3081d158102)

# Deployment

The Jiwa customer portal can be deployed on Linux or Windows machines.  The following instructions are for a Linux machine, created as a VM in the Azure platform.

## Linux

### Create an Azure linux VM 
In the Azure Portal, create a new VM with the following properties (mostly only those deviating from defaults are shown)

| Poperty | Value |
| ---  | --- |
| Virtual machine name | CustomerPortal  |
| Region | Australia East (same region as SQL DB) |
| Availability options | No infrastructure redundancy required |
| Security type | Standard |
| Image | Debian 12 x64 Gen2 |
| VM architecture | x64 |
| Size | B1s - 1 vcpu, 1GB RAM |
| Authentication type | Password |
| Username | Choose a username |
| Password | Choose a password |
| Inbound port rules | SSH (22) |
| OS disk size | default (30GB) |
| OS disk type | Standard SSD |
| Public IP | create new |
| Boot Diagnostics | Disable |

#### Network Settings
After creating the VM, edit the Network settings to limit SSH to only your IP address, and add port 80 and 443 to be open to any address.
80 is used by the Lets Encrypt certbot to validate domain ownership for the generation of SSL certificates, not for the portal itself - that requires port 443.

##### SSH (22)
Edit the following two properties, leave the rest as they were.
Source : IP addesses
Source IP addresses / CIDR Ranges: {Your IP}/32
![image](https://github.com/user-attachments/assets/85f850bc-8972-4296-81fb-2373200daf73)

##### HTTP (80) and HTTPS (443)
Create an inbound port rule for port 80 (HTTP) by selecting the Service as HTTP
![image](https://github.com/user-attachments/assets/5e0b0fbf-8f5f-4db9-a08d-959860b49458)

And do the same for HTTPS
![image](https://github.com/user-attachments/assets/c26a02b2-ed8f-4532-9c51-d1b716669985)

The network settings should look like the following after adding the required rules
![image](https://github.com/user-attachments/assets/8e2ab71c-a072-42d3-b5f9-a5f24ce02212)

#### DNS
Assign a DSN Name to the machine in the Overview tab, Essentials group of the VM.
![image](https://github.com/user-attachments/assets/0031003e-aeae-4cb0-8455-19af473667d0)

Given the DNS name label a suitable value and save.
![image](https://github.com/user-attachments/assets/39a2ec89-caf8-4b04-b903-b9a14c479650)

Back on the Overview tab, essentials group you should see the DNS name now as customerportal.australiaeast.cloudapp.azure.com (or whatever you chose) - copy that into your clipboard
![image](https://github.com/user-attachments/assets/b97ad5c6-a6ea-4824-9843-4aa338300b1d)

##### Public DNS
In your public DNS provider, add a CNAME entry to point portal.yourdomain.com to the DNS name created previously - customerportal.australiaeast.cloudapp.azure.com (or whatever you chose)
For example, in the below we are using Cloudflare to create a CNAME record pointing portal.jiwa.com.au to customerportal.australiaeast.cloudapp.azure.com
![image](https://github.com/user-attachments/assets/3eeeea25-0825-4833-b8fd-2a088ea570d6)

### Jiwa REST API Firewall
Unless you have good reason, your Jiwa 8 REST API should only accept requests from the IP address of the provisioned customer portal machine.
Assuming that the Jiwa 8 REST API is also running on an Azure VM, add an inbound security rule to allow access to the Jiwa 8 REST API on the port it is configured to use, to the IP Address of the custome portal machine.

Copy the IP address of the customer portal VM 
![image](https://github.com/user-attachments/assets/da240e22-8eb5-45bf-909d-d6042bc6ced7)

Open the Network Settings for the machine runing the Jiwa 8 REST API and create a new inbound security rule for the port used by the API to be accessible only by the IP address of the Customer Portal VM.
![image](https://github.com/user-attachments/assets/5fd91303-e3e6-4185-a0d8-0009965d5732)

#### Configuring the linux environment

Use a terminal client such as PuTTY to connect via SSH

![image](https://github.com/user-attachments/assets/5d2b577a-d59f-41e3-a139-189c72f13182)

When the shell appears, login as with the credentials used to create the VM - note that the password will not be echoed back to you.

![image](https://github.com/user-attachments/assets/09bb47fb-7230-475a-8902-5503b32efa4b)

![image](https://github.com/user-attachments/assets/d8f3d619-dbbd-479e-be5d-df3b1ef7a204)

##### Set the timezone 
we need an accurate timezone set to ensure overdue invoices are displayed appropriately
```console
timedatectl set-timezone Australia/Sydney
```

##### Update the system
```console
sudo  apt-get update
```

##### Install Cron
To run in a screen automatically on startup, we’ll use a crontab - used to run programs or scripts on a schedule - but can also be used to run scripts on startup - but first we need to install cron:
```console
sudo apt-get install cron
```

##### SSL Certificate
###### Install certbot
This generates an SSL certificate
```console
sudo apt-get install certbot
```

###### Run certbot
Issue the command to get a stand-alone SSL certificate - when asked, enter the DNS name of the machine - eg: portal.jiwa.com.au
```console
sudo certbot certonly --standalone 
```

###### appsettings.json
Edit appsettings.json and add the certificate section, and change the http endpoint to be https and port 443 instead of http and port 80

#### Publish JiwaCustomerPortal project
Publish the project by opening a Visual Studio 2022 command prompt and running the following command from the same folder as the project:
```console
dotnet publish -c release -r linux-x64 --self-contained
```
When finished, the published folder will be located in the \bin\Release\net9.0\linux-x64\publish relative to the project.

SFTP the publish folder to the linux machine. We use Filezilla, but any FTP client capable of SFTP will do.

When the transfers complete, rename the remote site folder "publish" to "customerportal"

Set the folder contents to be executable
```console
chmod -R 777 customerportal
```

Use your editor of choice and edit the customerportal/appsettings.json
```console
pico customerportal/appsettings.json
```

You must set the JiwaAPIURL, JiwaAPIKey, AllowedHosts, Kestrel.Endpoints.MyHttpEndpoint.Url , Kestrel.Certificates.Default.Path and Kestrel.Certificates.Default.KeyPath.

```json
{
  "JiwaAPIURL": "https://yourjiwaapi.domain.com:5492",
  "JiwaAPIKey": "Your Jiwa API Key",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "portal.domain.com",
  "Kestrel": {
      "Endpoints": {
        "MyHttpEndpoint": {
          "Url": "https://portal.domain.com:443"
        }
      },
    "Certificates": {
      "Default": {
          "Path": "/etc/letsencrypt/live/portal.domain.com/fullchain.pem",
          "KeyPath": "/etc/letsencrypt/live/portal.domain.com/privkey.pem"
        }
    }
  }
}
```

###### Create startup script
We will create a script to run the customer portal in a screen, and we’ll get the crontab to execute that on reboot.
change to the home folder of the current user
```console
cd $home
```
Then use pico to create and edit startportal.sh
```console
pico startportal.sh
```
We want to add the following 3 lines:
```console
screen -dmS customerportalscreen
screen -S customerportalscreen -p 0 -X stuff 'cd /home/YourUsername/customerportal\n'
screen -S customerportalscreen -p 0 -X stuff 'sudo ./JiwaCustomerPortal\n'
```
Set execute permissions on that shell script now:
```console
chmod +x startportal.sh
```
Test the script:
Note we need to use sudo to run any apps which attempt to use ports < 1000 - so when running the script to run the app, we must prefix sudo to each command
```console
sudo ./startportal.sh
```
It should appear to have done nothing, but run the command to show all running screens:
```console
sudo screen -ls
```
and it should appear in the list of running screens:
![image](https://github.com/user-attachments/assets/995b7f95-e2b8-44ad-93ac-0b54589061a2)

Connect to the screen with the command:
```console
sudo screen -x customerportalscreen
```
And it should show the output from that screen:
![image](https://github.com/user-attachments/assets/98683857-679e-403b-b9b3-5ccf0e061ac2)
Press CTRL-AD to exit the screen and leave it running.
If you wanted to stop the screen, press CTRL-C when connected and then type exit to end the screen session.

##### Configure Crontab
Now edit the crontab with the command:
```console
sudo crontab -e
```

If asked to choose an editor, do so - I use nano (aka pico):
![image](https://github.com/user-attachments/assets/34eeff43-769a-47ef-aeda-f602853f36a1)

Then the crontab will be shown:
![image](https://github.com/user-attachments/assets/c395cbf1-e080-4239-a9cb-1fca29f27477)

Add the following line (be sure to include the space after the first period (.)):
```console
@reboot . /home/YourUsername/startportal.sh
```

then CTRL-X and Y to save and exit.

Reboot the machine and ensure that https://portal.domain.com is accessible.

#### Secure the machine
Remove the Azure firewall rule to allow access via port 22 for SSH.  Add the rule as needed for subsequent maintenance operations where you need to SSH back in.
