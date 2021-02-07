/* NOT SUPPORTED IN SQL AZURE: Both the CREATE DATABASE and DROP DATABASE statements must be in a seperate file.
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'TransientFaultHandlingTest')
	DROP DATABASE [TransientFaultHandlingTest]
GO
*/

CREATE DATABASE [TransientFaultHandlingTest]
/* NOT SUPPORTED IN SQL AZURE
 COLLATE SQL_Latin1_General_CP1_CI_AS
*/
GO