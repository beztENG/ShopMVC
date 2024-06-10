-- Check if the 'ShopMVC' database exists. If not, create it.
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ShopMVC')
BEGIN
    CREATE DATABASE ShopMVC;
END;
GO

USE ShopMVC
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- Create OrderDetail table
CREATE TABLE [dbo].[OrderDetail](
    [OrderDetailID] [int] IDENTITY(1,1) NOT NULL, 
    [OrderID] [int] NOT NULL,      
    [ProductID] [int] NOT NULL,    
    [UnitPrice] [float] NOT NULL DEFAULT 0,        
    [Quantity] [int] NOT NULL DEFAULT 1,
    [Discount] [float] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED 
    (
        [OrderDetailID] ASC
    )
) ON [PRIMARY]
GO

-- Create Product table
CREATE TABLE [dbo].[Product](
    [ProductID] [int] IDENTITY(1,1) NOT NULL,    
    [ProductName] [nvarchar](50) NOT NULL,      
    [ProductAlias] [nvarchar](50) NULL,           
    [CategoryID] [int] NOT NULL,            
    [UnitDescription] [nvarchar](50) NULL,       
    [UnitPrice] [float] NULL DEFAULT 0,          
    [ShippingFee] [float] NOT NULL,              
    [Image] [nvarchar](50) NULL,                  
    [ProductionDate] [datetime] NOT NULL DEFAULT GETDATE(),
    [Discount] [float] NOT NULL DEFAULT 0,       
    [Views] [int] NOT NULL DEFAULT 0,            
    [Description] [nvarchar](max) NULL,          
    [SupplierID] [nvarchar](50) NOT NULL,       
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
    (
        [ProductID] ASC
    )
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[Order](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [nvarchar](50) NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[RequiredDate] [datetime] NULL,
	[ShippedDate] [datetime] NULL,
	[FullName] [nvarchar](50) NULL,
	[Address] [nvarchar](60) NOT NULL,
	[PaymentMethod] [nvarchar](50) NOT NULL,
	[ShippingMethod] [nvarchar](50) NOT NULL,
	[ShippingFee] [float] NOT NULL,
	[StatusID] [int] NOT NULL,
	[EmployeeID] [nvarchar](50) NULL,
	[Notes] [nvarchar](50) NULL,
	CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
	(
		[OrderID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Customer](
	[CustomerID] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NULL,
	[FullName] [nvarchar](50) NOT NULL,
	[Gender] [bit] NOT NULL,
	[BirthDate] [datetime] NOT NULL,
	[Address] [nvarchar](60) NULL,
	[Phone] [nvarchar](24) NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Image] [nvarchar](50) NULL,
	[Active] [bit] NOT NULL,
	[Role] [int] NOT NULL,
	[RandomKey] [varchar](50) NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[Category](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](50) NOT NULL,
	[CategoryAlias] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[Image] [nvarchar](50) NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE VIEW [dbo].[vOrderDetails]
AS 
	SELECT od.*, p.ProductName
	FROM OrderDetail od JOIN Product p 
		ON p.ProductID = od.ProductID
GO

-- Create User table with a unique constraint for CustomerID
CREATE TABLE [dbo].[User] (
    [UserID]        INT             IDENTITY(1,1) NOT NULL,  -- Primary key for the User table
    [CustomerID]    NVARCHAR(50)   NOT NULL UNIQUE,         -- Foreign key referencing CustomerID (unique)
    [Password]      NVARCHAR(50)   NULL,
    [Email]         NVARCHAR(50)   NOT NULL UNIQUE,          -- Make Email unique as well (optional, but recommended)
    [Role]          INT            NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [FK_User_Customer] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([CustomerID]) ON DELETE CASCADE ON UPDATE CASCADE 
) ON [PRIMARY];
GO


-- Insert sample data into the OrderDetail table
SET IDENTITY_INSERT [dbo].[OrderDetail] ON 

INSERT INTO [dbo].[OrderDetail] 
([OrderDetailID], [OrderID], [ProductID], [UnitPrice], [Quantity], [Discount])
VALUES 
(100001, 10248, 1001, 14, 12, 0),
(100003, 10250, 1003, 14, 12, 0),
(100004, 10251, 1004, 14, 12, 0),
(100005, 10252, 1005, 14, 12, 0),
(100006, 10253, 1006, 14, 12, 0),
(100007, 10254, 1007, 14, 12, 0),
(100008, 10255, 1008, 14, 12, 0),
(100002, 10249, 1002, 9.8, 10, 0);

SET IDENTITY_INSERT [dbo].[OrderDetail] OFF
GO

-- Insert sample data into the Product table
SET IDENTITY_INSERT [dbo].[Product] ON 

INSERT INTO [dbo].[Product] 
([ProductID], [ProductName], [ProductAlias], [CategoryID], [UnitDescription], [UnitPrice], [ShippingFee], [Image], [ProductionDate], [Discount], [Views], [Description], [SupplierID])
VALUES 
(1001, N'Tissot', N'Tissot', 1000, N'10 items remaining', 190, 10, N'Tissot.jpg', CAST(N'2009-07-31T07:00:00.000' AS DateTime), 0, 1378, N'Product description for Tissot goes here', N'AP'),
(1003, N'Citizen', N'Citizen', 1000, N'10 items remaining', 190, 10, N'Citizen.jpg', CAST(N'2009-07-31T07:00:00.000' AS DateTime), 0, 1378, N'Product description for Citizen goes here', N'AP'),
(1004, N'Omega', N'Omega', 1000, N'10 items remaining', 190, 10, N'Omega.jpg', CAST(N'2009-07-31T07:00:00.000' AS DateTime), 0, 1378, N'Product description for Omega goes here', N'AP'),
(1005, N'pf', N'pf', 1000, N'10 items remaining', 190, 10, N'pf.jpg', CAST(N'2009-07-31T07:00:00.000' AS DateTime), 0, 1378, N'Product description for pf goes here', N'AP'),
(1006, N'Rolex 123', N'Rolex', 1000, N'10 items remaining', 190, 10, N'Rolex.jpg', CAST(N'2009-07-31T07:00:00.000' AS DateTime), 0, 1378, N'Product description for Rolex goes here', N'AP'),
(1007, N'Seiko', N'Seiko', 1000, N'10 items remaining', 190, 10, N'Seiko.jpg', CAST(N'2009-07-31T07:00:00.000' AS DateTime), 0, 1378, N'Product description for Seiko goes here', N'AP'),
(1008, N'TagHeuer', N'TagHeuer', 1000, N'10 items remaining', 190, 10, N'TagHeuer.jpg', CAST(N'2009-07-31T07:00:00.000' AS DateTime), 0, 1378, N'Product description for TagHeuer goes here', N'AP'),
(1002, N'Cartier', N'Cartier', 1001, N'10 items remaining', 19, 10, N'Cartier.jpg', CAST(N'2009-07-31T07:00:00.000' AS DateTime), 0, 1562, N'Product description for Cartier goes here', N'AP');


SET IDENTITY_INSERT [dbo].[Product] OFF
GO

-- Insert sample data into Order table
SET IDENTITY_INSERT [dbo].[Order] ON 

INSERT INTO [dbo].[Order] 
([OrderID], [CustomerID], [OrderDate], [RequiredDate], [ShippedDate], [FullName], [Address], [PaymentMethod], [ShippingMethod], [ShippingFee], [StatusID], [EmployeeID], [Notes]) 
VALUES 
(10248, N'Admin', CAST(N'2022-07-04T00:00:00.000' AS DateTime), CAST(N'2022-08-01T00:00:00.000' AS DateTime), CAST(N'2022-07-16T00:00:00.000' AS DateTime), NULL, N'59 rue de l''Abbaye', N'Cash', N'Airline', 0, 0, NULL, NULL),
(10250, N'Admin', CAST(N'2022-07-04T00:00:00.000' AS DateTime), CAST(N'2022-08-01T00:00:00.000' AS DateTime), CAST(N'2022-07-16T00:00:00.000' AS DateTime), NULL, N'59 rue de l''Abbaye', N'Cash', N'Airline', 0, 0, NULL, NULL),
(10251, N'Admin', CAST(N'2022-07-04T00:00:00.000' AS DateTime), CAST(N'2022-08-01T00:00:00.000' AS DateTime), CAST(N'2022-07-16T00:00:00.000' AS DateTime), NULL, N'59 rue de l''Abbaye', N'Cash', N'Airline', 0, 0, NULL, NULL),
(10252, N'Admin', CAST(N'2022-07-04T00:00:00.000' AS DateTime), CAST(N'2022-08-01T00:00:00.000' AS DateTime), CAST(N'2022-07-16T00:00:00.000' AS DateTime), NULL, N'59 rue de l''Abbaye', N'Cash', N'Airline', 0, 0, NULL, NULL),
(10253, N'Admin', CAST(N'2022-07-04T00:00:00.000' AS DateTime), CAST(N'2022-08-01T00:00:00.000' AS DateTime), CAST(N'2022-07-16T00:00:00.000' AS DateTime), NULL, N'59 rue de l''Abbaye', N'Cash', N'Airline', 0, 0, NULL, NULL),
(10254, N'Admin', CAST(N'2022-07-04T00:00:00.000' AS DateTime), CAST(N'2022-08-01T00:00:00.000' AS DateTime), CAST(N'2022-07-16T00:00:00.000' AS DateTime), NULL, N'59 rue de l''Abbaye', N'Cash', N'Airline', 0, 0, NULL, NULL),
(10255, N'Admin', CAST(N'2022-07-04T00:00:00.000' AS DateTime), CAST(N'2022-08-01T00:00:00.000' AS DateTime), CAST(N'2022-07-16T00:00:00.000' AS DateTime), NULL, N'59 rue de l''Abbaye', N'Cash', N'Airline', 0, 0, NULL, NULL),
(10249, N'ALFKI', CAST(N'2022-07-05T00:00:00.000' AS DateTime), CAST(N'2022-08-16T00:00:00.000' AS DateTime), CAST(N'2022-07-10T00:00:00.000' AS DateTime), NULL, N'Luisenstr. 48', N'Cash', N'Airline', 0, 0, NULL, NULL);

SET IDENTITY_INSERT [dbo].[Order] OFF
GO


-- Insert sample data into Customer table
INSERT INTO [dbo].[Customer] 
([CustomerID], [Password], [FullName], [Gender], [BirthDate], [Address], [Phone], [Email], [Image], [Active], [Role], [RandomKey]) 
VALUES 
(N'Admin', N'Admin', N'Admin', 0, CAST(N'2009-08-01T15:10:40.857' AS DateTime), N'Obere Str. 57', N'030-0074321', N'alfki@abc.com', N'User.jpg', 1, 0, NULL),
(N'ALFKI', N'iloveyou', N'Maria Anders', 0, CAST(N'2009-08-01T15:10:40.857' AS DateTime), N'Obere Str. 57', N'030-0074321', N'alfki@abc.com', N'User.jpg', 0, 0, NULL);

GO

-- Insert sample data into Category table
SET IDENTITY_INSERT [dbo].[Category] ON 
INSERT INTO [dbo].[Category] ([CategoryID], [CategoryName], [CategoryAlias], [Description], [Image]) VALUES (1000, N'Men watch', N'watch', N'Soft drinks, coffees, teas, beers, and ales', N'Best.png')
INSERT INTO [dbo].[Category] ([CategoryID], [CategoryName], [CategoryAlias], [Description], [Image]) VALUES (1001, N'Woman watch', N'watch', N'Soft drinks, coffees, teas, beers, and ales', N'Best.png')
SET IDENTITY_INSERT [dbo].[Category] OFF
GO


-- Alter OrderDetail table to add default constraints
ALTER TABLE [dbo].[OrderDetail] ADD CONSTRAINT [DF_OrderDetail_UnitPrice] DEFAULT ((0)) FOR [UnitPrice]; 
GO
ALTER TABLE [dbo].[OrderDetail] ADD CONSTRAINT [DF_OrderDetail_Quantity] DEFAULT ((1)) FOR [Quantity];
GO
ALTER TABLE [dbo].[OrderDetail] ADD CONSTRAINT [DF_OrderDetail_Discount] DEFAULT ((0)) FOR [Discount];
GO

-- Alter Product table to add default constraints
ALTER TABLE [dbo].[Product] ADD CONSTRAINT [DF_Product_UnitPrice] DEFAULT ((0)) FOR [UnitPrice];
GO
ALTER TABLE [dbo].[Product] ADD CONSTRAINT [DF_Product_ProductionDate] DEFAULT (GETDATE()) FOR [ProductionDate]; 
GO
ALTER TABLE [dbo].[Product] ADD CONSTRAINT [DF_Product_Discount] DEFAULT ((0)) FOR [Discount];
GO
ALTER TABLE [dbo].[Product] ADD CONSTRAINT [DF_Product_Views] DEFAULT ((0)) FOR [Views]; -- "Views" replaces "SoLanXem"
GO

-- Alter Order table to add default constraints
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [DF_Order_OrderDate] DEFAULT (GETDATE()) FOR [OrderDate]; -- "OrderDate" replaces "NgayDat"
GO
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [DF_Order_RequiredDate] DEFAULT (GETDATE()) FOR [RequiredDate]; -- "RequiredDate" replaces "NgayCan"
GO
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [DF_Order_ShippedDate] DEFAULT (((1)/(1))/(1900)) FOR [ShippedDate]; -- "ShippedDate" replaces "NgayGiao"
GO
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [DF_Order_PaymentMethod] DEFAULT (N'Cash') FOR [PaymentMethod]; -- "PaymentMethod" replaces "CachThanhToan"
GO
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [DF_Order_ShippingMethod] DEFAULT (N'Airline') FOR [ShippingMethod]; -- "ShippingMethod" replaces "CachVanChuyen"
GO
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [DF_Order_ShippingFee] DEFAULT ((0)) FOR [ShippingFee];  -- "ShippingFee" replaces "PhiVanChuyen"
GO


-- Alter Customer table to add default constraints
ALTER TABLE [dbo].[Customer] ADD CONSTRAINT [DF_Customer_Gender] DEFAULT ((0)) FOR [Gender];
GO
ALTER TABLE [dbo].[Customer] ADD CONSTRAINT [DF_Customer_BirthDate] DEFAULT (GETDATE()) FOR [BirthDate]; -- "BirthDate" replaces "NgaySinh"
GO
ALTER TABLE [dbo].[Customer] ADD CONSTRAINT [DF_Customer_Image] DEFAULT (N'Photo.gif') FOR [Image];
GO
ALTER TABLE [dbo].[Customer] ADD CONSTRAINT [DF_Customer_Active] DEFAULT ((0)) FOR [Active];  -- "Active" replaces "HieuLuc"
GO
ALTER TABLE [dbo].[Customer] ADD CONSTRAINT [DF_Customer_Role] DEFAULT ((0)) FOR [Role];  -- "Role" replaces "VaiTro"
GO


-- Alter OrderDetail table to add foreign key constraints
ALTER TABLE [dbo].[OrderDetail] WITH CHECK ADD CONSTRAINT [FK_OrderDetail_Order] 
FOREIGN KEY([OrderID])  -- Establish a relationship between OrderDetail and Order based on OrderID
REFERENCES [dbo].[Order] ([OrderID])
ON UPDATE CASCADE       -- When the referenced OrderID is updated, cascade the change
ON DELETE CASCADE       -- When the referenced Order is deleted, delete the related OrderDetail rows as well
GO
ALTER TABLE [dbo].[OrderDetail] CHECK CONSTRAINT [FK_OrderDetail_Order]; 
GO

ALTER TABLE [dbo].[OrderDetail] WITH CHECK ADD CONSTRAINT [FK_OrderDetail_Product] 
FOREIGN KEY([ProductID]) -- Establish a relationship between OrderDetail and Product based on ProductID
REFERENCES [dbo].[Product] ([ProductID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetail] CHECK CONSTRAINT [FK_OrderDetail_Product];
GO

-- Alter Product table to add foreign key constraint
ALTER TABLE [dbo].[Product] WITH CHECK ADD CONSTRAINT [FK_Product_Category] 
FOREIGN KEY([CategoryID])  -- Establish a relationship between Product and Category based on CategoryID
REFERENCES [dbo].[Category] ([CategoryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Category];
GO

-- Select all data from the Customer table
SELECT * FROM [dbo].[Customer];
Select* from [dbo].[User]


update [dbo].[User]
set Active = 'true'
where UserID = 5

Update [dbo].[Customer]
set FullName = 'Dinh Quoc Vinh'
where Email = 'admin@admin.com'

alter table [dbo].[User]
add Active bit 

-- Select all data from the Product table
SELECT * FROM dbo.Product;
Select * from dbo.Category;
Select * from [dbo].[Order]

--Drop DATABASE ShopMVC;
