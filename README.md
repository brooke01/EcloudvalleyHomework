I use the .net core 6.0 web api framework and Microsoft SQL Server to program.

notice: Because the appsettings.json file will save the db connection string,
that could not be uploaded to github for security reasons,
the content of the appsettings.json file is below.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source={your db instance};Database=aws_db;Trusted_Connection=False;TrustServerCertificate=true;MultipleActiveResultSets=true;User ID=sa;Password={your password}"
  }
}
```

一、DataBase prepare
I use the SQL Server as my database to local develop program.
About the script for create database and table, see below.
```sql
CREATE DATABASE aws_db
GO

USE [aws_db]
GO

CREATE TABLE [dbo].[usage_report](
	[usage_report_id] [int] IDENTITY(1,1) NOT NULL,
	[payer_account_id] [decimal](13, 0) NOT NULL,
	[unblended_cost] [decimal](18, 9) NOT NULL,
	[unblended_rate] [decimal](18, 9) NOT NULL,
	[usage_account_id] [decimal](13, 0) NOT NULL,
	[usage_amount] [decimal](18, 9) NOT NULL,
	[usage_start_date] [datetime] NOT NULL,
	[usage_end_date] [datetime] NOT NULL,
	[product_name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_usage_report] PRIMARY KEY CLUSTERED 
(
	[usage_report_id] ASC
)
) ON [PRIMARY]
GO
```

二、APIs
We have three apis for use.

1. The api for import csv data into database

    | item         | content  |
    | ------------ | -------- |
    | url          | /Data    |
    | http method  | Post     |

    request multipart/form-data:

        file : {$binary}

    response:
    ```json
    {
        "message": "import success, total is 98754"
    }   
    ```

2. The api for giving a usageAccountId to get the sum of product of unblended cost.

    | item         | content                         |
    | ------------ | ------------------------------- |
    | url          | /UnblendedCost/{usageAccountId} |
    | http method  | Get                             |

    request route value:

        usageAccountId : {number}

    response:
    ```json
    {
        "productname_A": 123.12,
        "productname_B": 145.45
    }
    ```

3. The api for giving a usageAccountId to get the daily of product of usage amount.

    | item         | content                                                                   |
    | ------------ | ------------------------------------------------------------------------- |
    | url          | /UsageAmount/{usageAccountId}?StartDate={2020-04-01}&EndDate={2020-05-01} |
    | http method  | Get                                                                       |

    request route value and query string:

        usageAccountId : {number}

        StartDate : {Date} (optional)

        EndDate : {Date} (optional)

    notice: the query range for date at most 30 days.

    response:
    ```json
    {
        "productname_A": {
            "2023/01/01": 123,
            "2023/01/02": 123
        },
        "productname_B": {
            "2023/01/01": 123,
            "2023/01/02": 123
        }
    }
    ```
