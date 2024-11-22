# Jiwa Customer Portal

A simple web portal which uses the Jiwa 8 REST API to provide basic account functions.

The portal is built using ASP.NET 9, ServiceStack 8.4.4 and Bootstrap 5.3.3

## Functions

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

## Deployment

The Jiwa customer portal can be deployed on Linux or Windows machines.  The following instructions are for a Linux machine.

### Linux

#### Creating an Azure linux VM 

| Poperty | Value |
| ---  | --- |
| Virtual machine name | CustomerPortal  |
| Region | Content Cell  |

#### Configuring the linux environment

##### Set the timezone 
we need an accurate timezone set to ensure overdue invoices are displayed appropriately
timedatectl set-timezone Australia/Sydney

##### Update the system
sudo  apt-get update

##### SSL Certificate
###### Install certbot
This generates an SSL certificate
sudo apt-get install certbot

###### Run certbot
Issue the command to get a stand-alone SSL certificate - when asked, enter the DNS name of the machine - eg: portal.jiwa.com.au
sudo certbot certonly --standalone 

###### 
Edit appsettings.json again, and add the certificate section, and change the http endpoint to be https and port 443 instead of http and port 80

Publish the project by opening a Visual Studio 2022 command prompt and running the following command from the same folder as the project:
dotnet publish -c release -r linux-x64 --self-contained
When finished, the published folder will be located in the \bin\Release\net9.0\linux-x64\publish relative to the project.

SFTP the publish folder to the linux machine. We use Filezilla, but any FTP client capable of SFTP will do.

When the transfers complete, rename the remote site folder "publish" to "customerportal"


Set the folder contents to be executable
chmod -R 777 customerportal

Use your editor of choice and edit the customerportal/appsettings.json 
pico customerportal/appsettings.json
