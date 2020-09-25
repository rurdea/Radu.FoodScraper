PRINT N'Creating [dbo].[Scraper]...';

GO
CREATE TABLE [dbo].[Scraper](
	[ScraperId] [uniqueidentifier] NOT NULL,
	[ScraperName] [nvarchar](50) NOT NULL,
	[ScraperDescription] [nvarchar](200) NULL,
	[ScraperUrls] [nvarchar](max) null,
	[CreatedDateTime] [datetime2] NOT NULL,
	[LastUpdatedDateTime] [datetime2] NULL,
 CONSTRAINT [PK_ScraperId] PRIMARY KEY CLUSTERED 
(
	[ScraperId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON))

GO 
PRINT N'Creating [dbo].[Dish]...';

GO
CREATE TABLE [dbo].[Dish](
	[DishId] [uniqueidentifier] NOT NULL,
	[MenuTitle] [nvarchar](50) NULL,
	[MenuDescription] [nvarchar](500) NULL,
	[MenuSectionTitle] [nvarchar](200) NULL,
	[DishName] [nvarchar](200) NOT NULL,
	[DishDescription] [nvarchar](500) null,
	[CreatedDateTime] [datetime2] NOT NULL,
	[LastUpdatedDateTime] [datetime2] NULL,
	[ScraperId] [uniqueidentifier] not null
 CONSTRAINT [PK_DishId] PRIMARY KEY CLUSTERED 
(
	[DishId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

ALTER TABLE [dbo].[Dish]  WITH CHECK ADD  CONSTRAINT [FK_[Dish_Scraper_ScraperId] FOREIGN KEY([ScraperId])
REFERENCES [dbo].[Scraper] ([ScraperId])
GO