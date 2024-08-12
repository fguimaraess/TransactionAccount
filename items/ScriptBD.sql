/*
	DROP TABLE CUSTOMER
	DROP TABLE ACCOUNT
	DROP TABLE ACCOUNT_TRANSACTION
*/

USE master;
GO
	IF NOT EXISTS 
		(SELECT name  
		 FROM master.sys.server_principals
		 WHERE name = 'apiuser')
	BEGIN
		CREATE LOGIN [apiuser] WITH PASSWORD = 'genial'
	END
GO
CREATE TABLE CUSTOMER (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

INSERT INTO CUSTOMER (Name) VALUES ('Felipe Guimarães'),
('Ana Paula Oliveira'),
('Bruno Carvalho'),
('Carla Menezes'),
('Diego Souza'),
('Eduardo Silva'),
('Fernanda Ribeiro'),
('Gabriel Alves'),
('Heloísa Martins'),
('Isabela Ferreira'),
('João Pedro Santos'),
('Larissa Costa'),
('Marcelo Andrade'),
('Natália Lima'),
('Otávio Pereira'),
('Patrícia Rocha'),
('Ricardo Monteiro'),
('Sabrina Oliveira'),
('Tiago Barbosa'),
('Viviane Araújo');



CREATE TABLE ACCOUNT (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    AccountNumber NVARCHAR(20) NOT NULL,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,

    CONSTRAINT FK_BANKACCOUNTS_CUSTOMERID FOREIGN KEY (CustomerId)
        REFERENCES CUSTOMER(Id)
);
CREATE INDEX IDX_001_AC_CustomerId ON ACCOUNT(CustomerId);

INSERT INTO ACCOUNT (CustomerId, AccountNumber, Balance) VALUES 
(1, '0001-01', 1000.00), 
(1, '0001-02', 8000.00), 
(2, '0002-01', 500.00), 
(3, '0003-01', 1500.00), 
(4, '0004-01', 2000.00), 
(5, '0005-01', 250.00), 
(6, '0006-01', 300.00), 
(7, '0007-01', 750.00), 
(8, '0008-01', 900.00), 
(9, '0009-01', 1200.00), 
(10, '0010-01', 400.00), 
(11, '0011-01', 1600.00), 
(12, '0012-01', 1100.00), 
(13, '0013-01', 1400.00), 
(14, '0014-01', 1300.00), 
(15, '0015-01', 800.00), 
(16, '0016-01', 500.00), 
(17, '0017-01', 100.00), 
(18, '0018-01', 200.00), 
(19, '0019-01', 300.00), 
(20, '0020-01', 600.00);

CREATE TABLE ACCOUNT_TRANSACTION (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SourceAccountId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Description NVARCHAR(200),
	Type INT NOT NULL,
    TransactionDate DATETIME NOT NULL,
	InsertDate DATETIME NOT NULL DEFAULT GETDATE()
    CONSTRAINT FK_ACCOUNT_TRANSACTION_SOURCEACCOUNTID FOREIGN KEY (SourceAccountId)
        REFERENCES ACCOUNT(Id)
);
CREATE INDEX IDX_001_AT_Account_TransactionDate ON ACCOUNT_TRANSACTION(SourceAccountId, TransactionDate);
/* TRANSACOES SERAO CRIADAS VIA API - Collection exportada do POSTMAN para testes */

select * from CUSTOMER with(nolock)
select * from ACCOUNT with(nolock)
select * from ACCOUNT_TRANSACTION with(nolock)
