using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroOrmDemo.DataLayer.Tests
{


    class ResetScripts : IDisposable
    {
        public const string QueryCheckIfAddressesExist =
            @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES 
           WHERE TABLE_NAME = N'Addresses'";
        public const string QueryIfContactsExists =
            @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES 
           WHERE TABLE_NAME = N'Contacts'";
        public const string QueryIfStatesExists =
            @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES
           WHERE TABLE_NAME = N'States';";
        public const string QueryDropTables =
            "DROP TABLE Addresses;" +
            "DROP TABLE Contacts;" +
            "DROP TABLE States;";
        public const string QueryCreateTables =
            @"CREATE TABLE [dbo].[Contacts] (
    [Id]        INT          IDENTITY (1, 1) NOT NULL,
    [FirstName] VARCHAR (50) NULL,
    [LastName]  VARCHAR (50) NULL,
    [Email]     VARCHAR (50) NULL,
    [Company]   VARCHAR (50) NULL,
    [Title]     VARCHAR (50) NULL,
    CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [dbo].[States] (
    [Id]        INT          NOT NULL,
    [StateName] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_States] PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [dbo].[Addresses] (
    [Id]            INT          IDENTITY (1, 1) NOT NULL,
    [ContactId]     INT          NOT NULL,
    [AddressType]   VARCHAR (10) NOT NULL,
    [StreetAddress] VARCHAR (50) NOT NULL,
    [City]          VARCHAR (50) NOT NULL,
    [StateId]       INT          NOT NULL,
    [PostalCode]    VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Addresses_Contacts] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contacts] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Addresses_States] FOREIGN KEY ([StateId]) REFERENCES [dbo].[States] ([Id])
);
";

        public const string QueryPopulateTables =
            @"INSERT [dbo].[States] ([Id], [StateName]) VALUES (1, N'Alabama')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (2, N'Alaska')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (4, N'Arizona')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (5, N'Arkansas')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (6, N'California')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (8, N'Colorado')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (9, N'Connecticut')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (10, N'Delaware')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (11, N'District of Columbia')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (12, N'Florida')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (13, N'Georgia')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (15, N'Hawaii')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (16, N'Idaho')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (17, N'Illinois')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (18, N'Indiana')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (19, N'Iowa')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (20, N'Kansas')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (21, N'Kentucky')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (22, N'Louisiana')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (23, N'Maine')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (24, N'Maryland')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (25, N'Massachusetts')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (26, N'Michigan')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (27, N'Minnesota')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (28, N'Mississippi')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (29, N'Missouri')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (30, N'Montana')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (31, N'Nebraska')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (32, N'Nevada')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (33, N'New Hampshire')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (34, N'New Jersey')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (35, N'New Mexico')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (36, N'New York')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (37, N'North Carolina')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (38, N'North Dakota')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (39, N'Ohio')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (40, N'Oklahoma')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (41, N'Oregon')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (42, N'Pennsylvania')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (44, N'Rhode Island')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (45, N'South Carolina')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (46, N'South Dakota')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (47, N'Tennessee')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (48, N'Texas')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (49, N'Utah')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (50, N'Vermont')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (51, N'Virginia')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (53, N'Washington')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (54, N'West Virginia')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (55, N'Wisconsin')
INSERT [dbo].[States] ([Id], [StateName]) VALUES (56, N'Wyoming');

INSERT INTO [Contacts] VALUES ('Michael', 'Jordan', 'michael@bulls.com', 'Chicago Bulls', 'MVP');
INSERT INTO [Contacts] VALUES ('LaBron', 'James', 'labron@heat.com', 'Miami Heat', 'King James');
INSERT INTO [Contacts] VALUES ('Kobe', 'Bryant', 'kobe@lakers.com', 'Los Angeles Lakers', 'Guard');
INSERT INTO [Contacts] VALUES ('Kevin', 'Durant', 'kevin@thunder.com', 'OKC Thunder', 'Durantula');
INSERT INTO [Contacts] VALUES ('Kyrie', 'Irving', 'kyrie@cavs.com', 'Cleveland Cavaliers', 'Uncle Drew');
INSERT INTO [Contacts] VALUES ('Chris', 'Paul', 'chris@clippers.com', 'Los Angeles Clippers', 'CP3');

INSERT INTO [Addresses] VALUES(1, 'Home', '123 Main Street', 'Chicago', 17, '60290');
INSERT INTO [Addresses] VALUES(1, 'Work', '1901 W Madison St', 'Chicago', 17, '60612');
INSERT INTO [Addresses] VALUES(2, 'Home', '123 Main Street', 'Miami', 12, '33101');
INSERT INTO [Addresses] VALUES(3, 'Home', '123 Main Street', 'Los Angeles', 6, '90001');
INSERT INTO [Addresses] VALUES(4, 'Home', '123 Main Street', 'Oklahoma City', 40, '73101');
INSERT INTO [Addresses] VALUES(5, 'Home', '123 Main Street', 'Cleveland', 39, '44101');
INSERT INTO [Addresses] VALUES(6, 'Home', '456 Main Street', 'Los Angeles', 6, '90003');
";




        private static IDbConnection connection =
    new SqlConnection(
        ConfigurationManager.
        ConnectionStrings["ContactsDb"]
        .ConnectionString);

        public bool CheckIfAddressesExist()
        {

            var exists = connection.ExecuteScalar<bool>(QueryCheckIfAddressesExist);

            return exists;
        }
        public bool CheckIfStatesExist()
        {
            var exists = connection.ExecuteScalar<bool>(QueryIfStatesExists);

            return exists;

        }

        public bool CheckIfContactsExists()
        {
            var exists = connection.ExecuteScalar<bool>(QueryIfContactsExists);

            return exists;

        }

        public void DropTables()
        {

            connection.Execute(QueryDropTables);

        }
        public int CountRecords(string tableName)
        {
            int result = connection.ExecuteScalar<int>(
                $"SELECT COUNT(*) FROM {tableName}");

            return result;
        }

        public void CreateTables()
        {
            connection.Execute(QueryCreateTables);
        }

        public void PopulateTables()
        {
            connection.Execute(QueryPopulateTables);
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
