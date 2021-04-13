SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (select 1 FROM sysobjects WHERE [ID] = object_id('Tender') and [Type] = 'U')
BEGIN
    CREATE TABLE [dbo].[Tender](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [ReferenceNumber] [nvarchar](50) NOT NULL,
        [ReleaseDate] [datetime2] NOT NULL,
        [ClosingDate] [datetime2] NOT NULL,
        [Details] [nvarchar](max) NOT NULL,
        [CreatedBy] [nvarchar](50) NOT NULL,
        [Created] [datetime2] NOT NULL,
        [LastModifiedBy] [nvarchar](50) NULL,
        [LastModified] [datetime2] NULL,
        [IsDeleted] [bit] NOT NULL,
     CONSTRAINT [PK_Tender] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
    )
    ALTER TABLE [dbo].[Tender] ADD  CONSTRAINT [DF_Tender_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IDX_Tender_DeleteStatus' and object_id = object_id(N'Tender'))
BEGIN
    CREATE NONCLUSTERED INDEX [IDX_Tender_DeleteStatus] ON [dbo].[Tender]
    (
        [IsDeleted] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IDX_Tender_Name' and object_id = object_id(N'Tender'))
BEGIN
CREATE NONCLUSTERED INDEX [IDX_Tender_Name] ON [dbo].[Tender]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END
GO
