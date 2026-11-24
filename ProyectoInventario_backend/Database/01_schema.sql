USE master;
GO

IF DB_ID('G_Inventario') IS NOT NULL
BEGIN
    ALTER DATABASE G_Inventario SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE G_Inventario;
END
GO

CREATE DATABASE G_Inventario;
GO

USE G_Inventario;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

CREATE TABLE [dbo].[Usuario]
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Nombre] VARCHAR(50) NOT NULL,
    [Email] VARCHAR(100) NOT NULL,
    [Contrasena] VARCHAR(200) NOT NULL,
    [Estado] CHAR(1) NOT NULL DEFAULT 'A',
    [Rol] VARCHAR(20) NOT NULL DEFAULT 'User',
    [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_Usuario_Estado CHECK ([Estado] IN ('A','I','N')),
    CONSTRAINT CK_Usuario_Rol CHECK ([Rol] IN ('Admin','User'))
);

CREATE TABLE [dbo].[Producto]
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Codigo] VARCHAR(20) NOT NULL,
    [Nombre] VARCHAR(50) NOT NULL,
    [Descripcion] VARCHAR(200) NULL,
    [Precio] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [Estado] CHAR(1) NOT NULL DEFAULT 'A',
    CONSTRAINT CK_Producto_Estado CHECK ([Estado] IN ('A','I','N'))
);

CREATE TABLE [dbo].[Lote]
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [NumeroLote] VARCHAR(50) NOT NULL,
    [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
    [Estado] CHAR(1) NOT NULL DEFAULT 'A',
    CONSTRAINT CK_Lote_Estado CHECK ([Estado] IN ('A','I','N'))
);

CREATE TABLE [dbo].[ProductoLote]
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [ProductoId] INT NOT NULL,
    [LoteId] INT NOT NULL,
    [FechaIngreso] DATETIME NOT NULL DEFAULT GETDATE(),
    [PrecioUnitario] DECIMAL(18,2) NOT NULL,
    [Cantidad] INT NOT NULL,
    [Estado] CHAR(1) NOT NULL DEFAULT 'A',
    CONSTRAINT FK_ProductoLote_Producto FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto]([Id]),
    CONSTRAINT FK_ProductoLote_Lote FOREIGN KEY ([LoteId]) REFERENCES [dbo].[Lote]([Id]),
    CONSTRAINT CK_ProductoLote_Estado CHECK ([Estado] IN ('A','I','N'))
);
GO

-- INSERTS
 
USE G_Inventario;
GO

SET IDENTITY_INSERT [dbo].[Usuario] ON;
INSERT INTO [dbo].[Usuario] (Id, Nombre, Email, Contrasena, Estado, Rol, FechaCreacion)
VALUES (1, 'Carlos', 'carlos@inventario.com', '$2b$11$411h.3ITfIytaQ/hM9knPeehkoloWjGQfKYHXbJm88xihG85MIjra', 'A', 'Admin', GETDATE());
SET IDENTITY_INSERT [dbo].[Usuario] OFF;
GO

SET IDENTITY_INSERT [dbo].[Lote] ON;
INSERT INTO [dbo].[Lote] (Id, NumeroLote, FechaCreacion, Estado) VALUES
(1, 'LOT-2026-001', '2026-01-15', 'A'),
(2, 'LOT-2026-002', '2026-02-10', 'A'),
(3, 'LOT-2026-003', '2026-03-05', 'A'),
(4, 'LOT-2026-004', '2026-04-12', 'A'),
(5, 'LOT-2026-005', '2026-05-08', 'A');
SET IDENTITY_INSERT [dbo].[Lote] OFF;
GO

SET IDENTITY_INSERT [dbo].[Producto] ON;
INSERT INTO [dbo].[Producto] (Id, Codigo, Nombre, Descripcion, Precio, Estado) VALUES
(1, 'MOU-001', 'Mouse Logitech MX Master 3S', 'Mouse inalámbrico ergonómico, sensor 8000 DPI, Bluetooth y USB', 99.00, 'A'),
(2, 'AUD-001', 'Headset Jabra Evolve2 65', 'Audífonos inalámbricos con micrófono, cancelación de ruido, USB Bluetooth', 210.00, 'A'),
(3, 'IMP-001', 'Impresora HP LaserJet Pro M404dn', 'Impresora láser monocromática, dúplex automático, red Ethernet', 340.00, 'A'),
(4, 'ALM-001', 'SSD Samsung 990 Pro 1TB', 'Unidad de estado sólido NVMe PCIe 4.0, lectura hasta 7450 MB/s', 135.00, 'A'),
(5, 'RAM-001', 'Memoria RAM Kingston Fury 16GB', 'Memoria DDR4 3200MHz, módulo de 16GB para equipos de escritorio', 58.00, 'A'),
(6, 'RED-001', 'Router TP-Link Archer AX55', 'Router Wi-Fi 6 AX3000, doble banda, 4 puertos Gigabit', 145.00, 'A'),
(7, 'UPS-001', 'UPS APC Back-UPS Pro 1500VA', 'Sistema de alimentación ininterrumpida 1500VA, 10 tomas, pantalla LCD', 275.00, 'A');
SET IDENTITY_INSERT [dbo].[Producto] OFF;
GO

SET IDENTITY_INSERT [dbo].[ProductoLote] ON;
INSERT INTO [dbo].[ProductoLote] (Id, ProductoId, LoteId, FechaIngreso, PrecioUnitario, Cantidad, Estado) VALUES
(1, 1, 1, '2026-01-15', 95.00, 10, 'A'),
(2, 1, 2, '2026-02-10', 105.50, 8, 'A'),
(3, 2, 2, '2026-02-10', 220.00, 12, 'A'),
(4, 3, 3, '2026-03-05', 310.00, 5, 'A'),
(5, 4, 4, '2026-04-12', 115.00, 20, 'A');
SET IDENTITY_INSERT [dbo].[ProductoLote] OFF;
GO
