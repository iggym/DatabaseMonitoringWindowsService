# Database Monitoring Windows Service
Database Monitoring Windows Service

This project is a monitoring service which can monitor a SQL Server table for new records. As soon
as a new record is found the service logs the new record to a file called C:\alerts.txt.

---
Implemented as a Windows Service which checks the database for new records every x number of seconds.(x can be specified in the config file.)

--
## Database
* The database used to test the service was SQL Server database. 
* Table used was named SensorData (used for an IoT solution)

![database table](table.png)
```sql
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SensorData](
[ReadingID] [bigint] IDENTITY(1,1) NOT NULL,
[Temperature] [float] NOT NULL,
[Pressure] [float] NOT NULL,
[Luminosity] [float] NOT NULL,
[Timestamp] [datetime] NOT NULL,
CONSTRAINT [PK_SensorData] PRIMARY KEY CLUSTERED
(
[ReadingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
```
* Test data 

![data](sampledata.png)
```sql
INSERT INTO [dbo].[SensorData] ([Temperature], [Pressure], [Luminosity], [Timestamp])
VALUES (RAND(),RAND(),RAND(),GETDATE())
```

## Installation
The project also includes a Service Installer.
