/*CREATE TABLE Voucher
(
VoucherID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
VoucherName NCHAR(255) NOT NULL,
VoucherValue NCHAR(255) NOT NULL,
VoucherLink NCHAR(255) NOT NULL,
VoucherPrice DECIMAL (18,2) NOT NULL,
VoucherS3Key NCHAR (255) NOT NULL,
)

CREATE TABLE Customer
(
CustomerID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
 CustomerName NCHAR(255) NOT NULL,
 CustomerEmail NCHAR(255) NOT NULL,
 CustomerPassword NCHAR(255) NOT NULL,
CustomerPhone INT NOT NULL
)

CREATE TABLE Transactions
(
TransactionID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
 CustomerID INT(255) NOT NULL,
 VoucherID INT(255) NOT NULL,
 GiftcardCode NCHAR(255) NOT NULL
)
*/



select * from voucher

select * from Transactions
