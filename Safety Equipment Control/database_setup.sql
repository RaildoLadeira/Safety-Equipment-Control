-- Database Setup Script for Safety Equipment Control System

-- 1. Create the Database
CREATE DATABASE SafetyDB;
GO

USE SafetyDB;
GO

-- 2. Create the Inventory Table
CREATE TABLE EquipmentInventory (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Material NVARCHAR(50) NOT NULL,
    Quantity INT NOT NULL,
    FirstDate DATETIME NOT NULL,
    LastIssueDate DATETIME NOT NULL,
    Status NVARCHAR(20)
);
GO

-- 3. (Optional) Insert sample data for testing
INSERT INTO EquipmentInventory (Name, Material, Quantity, FirstDate, LastIssueDate, Status)
VALUES ('Demo User', 'Safety Shoes', 1, GETDATE(), GETDATE(), 'Good');
GO