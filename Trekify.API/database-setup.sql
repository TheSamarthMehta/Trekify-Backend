-- SQL Server Database Setup Script for Trekify
-- Run this script to manually create the database if needed

-- Create the TrekifyDB database
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'TrekifyDB')
BEGIN
    CREATE DATABASE TrekifyDB;
    PRINT 'TrekifyDB database created successfully.';
END
ELSE
BEGIN
    PRINT 'TrekifyDB database already exists.';
END

-- Use the database
USE TrekifyDB;

-- Verify database creation
SELECT 'Database TrekifyDB is ready for Entity Framework migrations!' AS Status;