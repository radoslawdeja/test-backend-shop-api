# test-backend-shop-api
API: **RESTful API**  
Database: **MariaDB**

## 1. Install WSL (Windows Subsystem for Linux)

Open PowerShell Window and run command: 
```
wsl --install
```

## 2. Install Rancher Desktop

Download from site: https://rancherdesktop.io/  

Install Rancher Desktop on windows.


## 3. Install HeidiSQL

Download from site: https://www.heidisql.com/download.php?download=installer

Install HeidiSQL on windows or download portable version.

## 4. Create container for MariaDB

Open PowerShell Window and run command:
```
docker run --detach --name shoptest-mariadb --env MARIADB_ROOT_PASSWORD=1234 -p 3306:3306 mariadb:latest
```
`shoptest-mariadb` - sample container name  
`1234` - sample password on root account  
`3306` - default port for MariaDB  

![alt text](.\img\powershell.png)

In Rancher Desktop you can see new container `shoptest-mariadb`:

![alt text](.\img\rancherdesktop.png)

## 5. Create migration in Visual Studio

In Visual Studio you must create your first migration.  
Open `Package Manager Console` and set "Default project" as `src\Test.Shop.Infrastructure`.  
Create new migration with command:
```
Add-Migration InitShopTables -Context ShopDbContext
```

Package Manager Console window:

![alt text](.\img\packagemanager.png)


## 5. Database

Run Visual Studio project API (Swagger).
Open HeidiSQL and add new connection:  

Connection type: `MariaDB or MySQL`  
Host / IP: `127.0.0.1` or `localhost`  
User: `root`  
Password: `1234`
Port: `3306`

![alt text](.\img\heidisql_connection.png)

Connect to database:

![alt text](.\img\heidisql_db.png)