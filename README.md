# Advanced Database Systems - Query Optimization

## Project Description
In this project, we implemented a database system to perform search using file system search and dense index search.

The database containing two relations `User` and `Address` where each user may have one to many Addresses. Each relation is stored in a CSV files filled with large amount of data. In addition, we have two dense index `UserIndex` and `AddressIndex` stored in CSV files as well. `UserIndex` file contain the UserId (the primary key in relation `User`) and the byte offset pointing to the record in `User` table.

`AddressIndex` contain UserId (foreign Id in relation `Address`) and the byte offset for each record containing address for that user.

To generate `User` table, the system will read from a txt file `firstName` that contains 550 first names and loop through them and for each first name in the list, the system will loop through another txt file `lastName` that contains also 550 last names. In total, we will have 302,500 users. While generating new user, the system will generate new address (from 0 to four addresses) by getting the country name from a txt file `country`. In total, we will have ~604,287 addresses.


## Using the application
If the initialization is set to true (`AdvancedDatabaseSystems.Program.Main line 15`), the system will generate the needed database files. If set to false, the system will go through the below steps.
1. Select the operation
2. Choose select method **1** for indexing, **2** for file scan
3. put the user id

#### Index example
![Index example](https://github.com/mhdbouk/advanced-database-system/blob/master/assets/6.png)

#### File scan example
![File scan example](https://github.com/mhdbouk/advanced-database-system/blob/master/assets/7.png)

## Statistics
Table User has **302,500**

Table Address has **604,287**

As showing from the screenshot (Using the application), the time needed to perform file scan is **00:00:01.675** and it is less in the index search type **00:00:00:607**.

Index search is faster with **00:00:01:068**.

## Deep into the code
The system designed by applying the object-oriented paradigm. We store the model classes in folder `Models` and the repositories that perform all actions to the database (the file system) in folder `Repository`.

### User Model
![User Model](https://github.com/mhdbouk/advanced-database-system/blob/master/assets/1.png)

### Address Model
![Address Model](https://github.com/mhdbouk/advanced-database-system/blob/master/assets/2.png)

### Dense Index Model
![Dense Index Model](https://github.com/mhdbouk/advanced-database-system/blob/master/assets/3.png)

### User repository
The main method used in the user repository is `GetById`. In this method, we are passing the user Id and search type (file scan / index).
If the type is file scan, the system will go through all the lines in the user csv file until we found the record. If the type is Index, the system will read the byte offset from the index csv file and return the exact record without looping through all items. 
We are using StreamReader to seek the offset and to return the string containing the record. Then we convert it to a User model.

![User repository](https://github.com/mhdbouk/advanced-database-system/blob/master/assets/4.png)

### Address Repository
The main method used in the address repository is `GetByUserId`. In this method, we are passing the user Id and search type (file scan / index).
If the type is file scan, the system will go through all the lines in the address csv file, if we found the record with the user id, we will add the record to a `List<Address>` and then return all the list of addresses. If the type is Index, the system will read the byte offset from the index csv file and return all record without looping through all items. 

![Address Repository](https://github.com/mhdbouk/advanced-database-system/blob/master/assets/5.png)

