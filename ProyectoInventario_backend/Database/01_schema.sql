CREATE TABLE [dbo].Usuario
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Nombre varchar(50) not null, 
	Email varchar(100) not null, 
	contraseña varchar(100) not null,
	Activo bit not null,
	FechaCreacion datetime not null default getdate()
);


CREATE TABLE [dbo].Producto
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Codigo varchar(20) UNIQUE not null,
	Nombre varchar(50) not null,
	Descripcion varchar(200) null,
	Activo bit not null,	
);


CREATE TABLE [dbo].ProductoLote
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[ProductoId] INT NOT NULL,
	[NumeroLote] varchar(50) not null,
	[FechaIngreso] datetime null,
	PrecioUnitario decimal(18,2) not null,
	[Cantidad] int not null,
	
	CONSTRAINT FK_Lote_Producto FOREIGN KEY (ProductoId) REFERENCES Producto(Id)

	
);

