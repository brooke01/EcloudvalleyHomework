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

    | field | data type | required | description                                                               |
    | ----- | --------- | -------- | ------------------------------------------------------------------------- |
    | file  | binary    | true     | request with multipart/form-data method, and import data from *.csv file. |

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

    | field          | data type | required | description                            |
    | -------------- | --------- | -------- | -------------------------------------- |
    | usageAccountId | number    | true     |                                        |

    response:
    ```json
    {
        "productname_A": 123.12,
        "productname_B": 145.45
    }
    ```

3. The api for giving a usageAccountId to get the daily of product of usage amount.

    | item         | content                                                               |
    | ------------ | --------------------------------------------------------------------- |
    | url          | /UsageAmount/{usageAccountId}?StartDate={StartDate}&EndDate={EndDate} |
    | http method  | Get                                                                   |

    | field          | data type | required | description                                           |
    | -------------- | --------- | -------- | ----------------------------------------------------- |
    | usageAccountId | number    | true     |                                                       |
    | StartDate      | date      | false    | Determine which started date to query. ex: 2020-04-01 |
    | EndDate        | date      | false    | Determine which ended date to query. ex: 2020-05-01   |

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

4. The api for giving a usageAccountId to get the daily of product of usage amount.

    | item         | content                                                                                                    |
    | ------------ | ---------------------------------------------------------------------------------------------------------- |
    | url          | /UsageAmount_V2/{usageAccountId}?queryStartDate={queryStartDate}&pageIndex={pageIndex}&pageSize={pageSize} |
    | http method  | Get                                                                                                        |

    | field          | data type | required | description                                                     |
    | -------------- | --------- | -------- | --------------------------------------------------------------- |
    | usageAccountId | number    | true     |                                                                 |
    | queryStartDate | date      | false    | Determine which started date to query. ex: 2020-04-01           |
    | pageIndex      | number    | false    | The one-based index of the currently displayed page of product. |
    | pageSize       | number    | false    | The data quantity of per page displayed of product.             |
 
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
