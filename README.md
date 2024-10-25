# Jiwa Customer Portal

A simple web portal which uses the Jiwa 8 REST API to provide basic account functions.

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

Deployment
Linux
dotnet publish -c release -r linux-x64 --self-contained
Windows

