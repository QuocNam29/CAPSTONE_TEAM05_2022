USE [master]
GO
/****** Object:  Database [CP25Team05]    Script Date: 23/02/2023 4:21:26 CH ******/
CREATE DATABASE [CP25Team05]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CP25Team05', FILENAME = N'/var/opt/mssql/data/CP25Team05.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CP25Team05_log', FILENAME = N'/var/opt/mssql/data/CP25Team05_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [CP25Team05] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CP25Team05].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CP25Team05] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CP25Team05] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CP25Team05] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CP25Team05] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CP25Team05] SET ARITHABORT OFF 
GO
ALTER DATABASE [CP25Team05] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [CP25Team05] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CP25Team05] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CP25Team05] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CP25Team05] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CP25Team05] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CP25Team05] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CP25Team05] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CP25Team05] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CP25Team05] SET  ENABLE_BROKER 
GO
ALTER DATABASE [CP25Team05] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CP25Team05] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CP25Team05] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CP25Team05] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CP25Team05] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CP25Team05] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CP25Team05] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CP25Team05] SET RECOVERY FULL 
GO
ALTER DATABASE [CP25Team05] SET  MULTI_USER 
GO
ALTER DATABASE [CP25Team05] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CP25Team05] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CP25Team05] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CP25Team05] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CP25Team05] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CP25Team05] SET QUERY_STORE = OFF
GO
USE [CP25Team05]
GO
/****** Object:  User [CP25Team05]    Script Date: 23/02/2023 4:21:26 CH ******/
CREATE USER [CP25Team05] FOR LOGIN [CP25Team05] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [CP25Team05]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[carts]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[carts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[product_id] [int] NOT NULL,
	[customer_id] [int] NOT NULL,
	[unit] [nvarchar](50) NULL,
	[quantity] [int] NOT NULL,
	[price] [int] NOT NULL,
	[note] [varchar](max) NULL,
	[created_at] [datetime2](0) NULL,
	[updated_at] [datetime2](0) NULL,
 CONSTRAINT [PK__carts__3213E83FE15DEE37] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[categories]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[categories](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](50) NOT NULL,
	[created_by] [nvarchar](128) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[slug] [nvarchar](255) NOT NULL,
	[status] [int] NOT NULL,
	[created_at] [datetime2](0) NULL,
	[updated_at] [datetime2](0) NULL,
	[deleted_at] [datetime2](0) NULL,
 CONSTRAINT [PK__categori__3213E83F9FD0E036] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[customers]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[customers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[created_by] [nvarchar](128) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[email] [varchar](255) NULL,
	[phone] [varchar](11) NOT NULL,
	[address] [nvarchar](255) NOT NULL,
	[type] [int] NOT NULL,
	[status] [int] NOT NULL,
	[account_number] [varchar](20) NULL,
	[bank] [varchar](255) NULL,
	[note] [nvarchar](max) NULL,
	[birthday] [datetime2](0) NULL,
	[price_level] [int] NOT NULL,
	[created_at] [datetime2](0) NULL,
	[updated_at] [datetime2](0) NULL,
	[deleted_at] [datetime2](0) NULL,
	[code] [varchar](255) NULL,
 CONSTRAINT [PK__customer__3213E83F1FA658DD] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[debts]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[debts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[sale_id] [int] NOT NULL,
	[paid] [money] NOT NULL,
	[total] [money] NOT NULL,
	[created_by] [nvarchar](128) NOT NULL,
	[created_at] [datetime2](0) NULL,
	[updated_at] [datetime2](0) NULL,
	[deleted_at] [datetime2](0) NULL,
	[note] [nvarchar](max) NULL,
 CONSTRAINT [PK__debts__3213E83FC75DB362] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[groups]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[groups](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](50) NOT NULL,
	[created_by] [nvarchar](128) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[slug] [varchar](255) NOT NULL,
	[status] [int] NOT NULL,
	[created_at] [datetime2](0) NULL,
	[updated_at] [datetime2](0) NULL,
	[deleted_at] [datetime2](0) NULL,
 CONSTRAINT [PK__groups__3213E83F0C8B4D4A] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[import_inventory]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[import_inventory](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[product_id] [int] NOT NULL,
	[quantity] [int] NOT NULL,
	[price_import] [money] NOT NULL,
	[sold] [int] NOT NULL,
	[created_by] [nvarchar](128) NOT NULL,
	[created_at] [datetime2](0) NULL,
	[updated_at] [datetime2](0) NULL,
	[deleted_at] [datetime2](0) NULL,
	[sold_swap] [int] NULL,
	[quantity_remaining] [int] NULL,
	[supplier_id] [int] NULL,
 CONSTRAINT [PK__import_i__3213E83F45EE4099] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[products]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](100) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[group_id] [int] NOT NULL,
	[category_id] [int] NOT NULL,
	[unit] [nvarchar](100) NOT NULL,
	[purchase_price] [money] NOT NULL,
	[sell_price] [money] NOT NULL,
	[quantity] [int] NOT NULL,
	[status] [int] NOT NULL,
	[note] [nvarchar](max) NULL,
	[created_by] [nvarchar](128) NOT NULL,
	[created_at] [datetime2](0) NULL,
	[updated_at] [datetime2](0) NULL,
	[deleted_at] [datetime2](0) NULL,
	[name_group] [nvarchar](255) NULL,
	[name_category] [nvarchar](255) NULL,
	[unit_swap] [nvarchar](100) NULL,
	[quantity_swap] [int] NULL,
	[quantity_remaning] [int] NOT NULL,
	[sell_price_swap] [money] NULL,
 CONSTRAINT [PK__products__3213E83FC3174AD5] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[return_details]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[return_details](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[return_id] [int] NOT NULL,
	[product_current_id] [int] NOT NULL,
	[quantity_current] [int] NOT NULL,
	[unit_current] [nvarchar](50) NOT NULL,
	[total_current] [money] NOT NULL,
	[product_return_id] [int] NULL,
	[quantity_return] [int] NULL,
	[total_return] [money] NULL,
	[unit_return] [nvarchar](50) NULL,
	[difference] [money] NOT NULL,
 CONSTRAINT [PK_return_details] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[return_sale]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[return_sale](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[sale_id] [int] NOT NULL,
	[method] [int] NOT NULL,
	[create_at] [datetime] NOT NULL,
	[difference] [money] NOT NULL,
 CONSTRAINT [PK_return] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[return_supplier]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[return_supplier](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[inventory_id] [int] NOT NULL,
	[quantity] [int] NOT NULL,
	[note] [nvarchar](255) NULL,
	[cost_difference] [money] NOT NULL,
	[created_at] [datetime] NOT NULL,
 CONSTRAINT [PK_return_supplier] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[revenue]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[revenue](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[inventory_id] [int] NOT NULL,
	[sale_details_id] [int] NOT NULL,
	[Price] [money] NOT NULL,
	[quantity] [int] NOT NULL,
	[unit] [nvarchar](50) NULL,
 CONSTRAINT [PK_revenue] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sale_details]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sale_details](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[sale_id] [int] NOT NULL,
	[product_id] [int] NOT NULL,
	[unit] [nvarchar](50) NULL,
	[price] [money] NOT NULL,
	[sold] [int] NOT NULL,
	[note] [varchar](max) NULL,
	[created_at] [date] NULL,
	[updated_at] [date] NULL,
	[deleted_at] [datetime2](0) NULL,
 CONSTRAINT [PK__sale_det__3213E83F2236F0A7] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sales]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sales](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[customer_id] [int] NOT NULL,
	[method] [int] NOT NULL,
	[total] [money] NOT NULL,
	[note] [nvarchar](max) NULL,
	[status] [int] NOT NULL,
	[created_by] [nvarchar](128) NOT NULL,
	[created_at] [datetime2](0) NULL,
	[updated_at] [datetime2](0) NULL,
	[deleted_at] [datetime2](0) NULL,
	[prepayment] [money] NULL,
 CONSTRAINT [PK__sales__3213E83FFEC78C43] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 23/02/2023 4:21:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [nvarchar](128) NOT NULL,
	[name] [nvarchar](250) NULL,
	[email] [varchar](255) NOT NULL,
	[avatar] [varchar](max) NULL,
	[phone] [varchar](11) NULL,
	[address] [varchar](max) NULL,
	[remember_token] [varchar](100) NULL,
	[created_at] [datetime2](0) NULL,
	[updated_at] [datetime2](0) NULL,
	[deleted_at] [datetime2](0) NULL,
	[asp_id] [nvarchar](128) NULL,
 CONSTRAINT [PK__users__3213E83F0A1B90B7] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__MigrationHistory] ([MigrationId], [ContextKey], [Model], [ProductVersion]) VALUES (N'202210251019069_InitialCreate', N'CAP_TEAM05_2022.Models.ApplicationDbContext', 0x1F8B0800000000000400DD5C6D6FE3B811FE5EA0FF41D0A7B6C8598ED35D6C03FB0E3E2769836E5EB0CE1EFA2DA025DA1156A27412954B50DC2FEB87FB49F7173A94A837BEE8C5566CE7B0C0C22287CF0C874372381CE6F7FFFD36FDE1C5F78C671CC56E4066E6E9686C1A98D881E392CDCC4CE8FABB4FE60FDFFFF94FD34BC77F317ECAE9CE181DB424F1CC7CA2343CB7ACD87EC23E8A47BE6B47411CACE9C80E7C0B398135198FFF619D9E5A18204CC0328CE9978450D7C7E9077C2E0262E39026C8BB091CECC5BC1C6A9629AA718B7C1C87C8C6337331BF7F7CB89CDF8C3F3C4EC693C9286B611A73CF4520CD127B6BD3408404145190F5FC6B8C97340AC866194201F21E5E430C746BE4C598F7E1BC24EFDA9DF18475C72A1BE6507612D3C0EF09787AC6F56389CDB7D2B259E80F3478099AA6AFACD7A91667E6B583D3A22F81070A10199E2FBC8811CFCC9B82C53C0E6F311DE50D4719E4550470BF04D1B75115F1C4E8DCEEA4B0A7C968CCFE9D188BC4A349846704273442DE89719FAC3CD7FE377E7D08BE61323B3B5DADCF3E7DF8889CB38F7FC7671FAA3D85BE025DAD008AEEA320C411C886D745FF4DC3AAB7B3C48645B34A9B4C2B604B30354CE306BD7CC664439F60D24C3E99C695FB829DBC841BD757E2C24C8246344AE0F336F13CB4F270516F35F264FF37709D7CF83808D75BF4EC6ED2A117F8C3C489605E7DC15E5A1B3FB96136BD6AE3FDC8C9AEA2C067DF75FBCA6A1F974112D9AC338196E401451B4CEBD24DADD2783B9934831ADEAC73D4E3376D26A96CDE4A52D6A16D6642CE62DFB32197F76DF976B6B87918C2E0A5A6C534D26470EA0D6B24208081D4E94A133AED6A4204BAF6475E112F7DE47A032C891DB88047B276231F17BDFC31000344A4B7CCF7288E614570FE85E2A706D1E1E700A22FB19D4460A84B8AFCF0CDB9DD3F0504DF26FE8AD9FFFE780D36340FBF0457C8A641744958AB9DF13E07F6B720A197C4B940147FA5760EC83E1F5CBF3BC020E2CC6D1BC7F115183376160138DC39E035A16793DE706C913AB44BB2F090EBAB7D1261397DCC494BBF444D21F9261A32957FD224EAE760E3926EA2E6A47A51338A565139595F511958374939A55ED094A055CE8C6A308F2F1DA1E15DBE14F6F87DBEDD366FDD5A5051E3125648FC4F4C7004CB98738F28C5112947A0CBBA710867211D3EC6F4CDF7A694D34FC84B8666B5D56C481781E167430A7BFCB32115138A9F5D8779251D0E423931C077A2579FB1DAE79C20D9BEA743AD9BFB66BE9F3540375DE6711CD86E3A0B1421301EC0A8CB0F3E9CD11ECDC87A234644A06360E82EDBF2A004FA668A4675472EB0872936E67616225CA0D8468EAC46E890D343B07C475508564646EAC2FD4DE209968E23D608B143500C33D525549E162EB1DD1079AD5A125A76DCC258DF0B1E62CD050E31610C5B35D185B93A10C20428F80883D2A6A1A955B1B86643D478ADBA316F7361CB7197E2137BB1C916DF596397DC7F7B13C36CD6D81E8CB359255D04D006F50E61A0FCACD2D500C483CBB119A87062D2182877A9F662A0758D1DC040EB2A7977069A1D51BB8EBF705E3D36F3AC1F94F7BFAD37AAEB00B659D3C7919966E67B421B0A2D70249BE7C58A55E217AA389C819CFC7C16735757341106BEC4B41EB229FD5DA51F6A35838846D404581A5A0B28BF0E9480A409D543B83C96D7281DF7227AC0E671B74658BEF60BB0151B90B1ABD7A21542FDE5A9689C9D4E1F45CF0A6B908CBCD361A182A3300871F1AA77BC835274715959315D7CE13EDE70A5637C301A14D4E2B96A94947766702DE5A6D9AE259543D6C725DB494B82FBA4D152DE99C1B5C46DB45D490AA7A0875BB0938AEA5BF840932D8F7414BB4D5137B5B28C295E30B534A955D31B14862ED95452AD7889B1E47956DF2DFB271FF9198665C78A1CA442DA82130D22B4C1422DB00649AFDC28A61788A21562719E85E34B64CABD55B3FCE72CABDBA73C88F93E9053B3DF3CC6ABBEC4AFEDB7B243C271AEA0973EF36AD250BAC206D4CD0D96FE863C1429A2F78BC04B7CA277B2F4ADB33BBC6AFBAC4446985A82FC921325694C7275EBEAEF3438F2C41870A00A3F66FBC1D243E8549E7BA155A5EB3C533D4A1EA8AAA2E88257071B3C9D43D37BC0447FB1FF78B522BCCDFCE2492A55005ED413A392E7208155EABAA3D65351AA98F59AEE8842BE491552A8EA216535ABA42664B5622B3C8D46D514DD39C879245574B9B63BB222A3A40AADA8DE025B21B358D71D5591745205565477C72E3350C485F4887730ED2966A72D2C3BECEEB6876930DE66551C660BACDCE957812AC53DB1F8ADBD04C6CB8FD2A2B427BE9D2C2A8B73EC66511A0CFD0A54BB11AF2F408DD7F87ACCDA35776D916FBAE6D7E3F5B3DB37B50EE9D0279214DC8BC39F70C89BF20357874736E2092C23318D5C8DB0C1BFC614FB2346305AFEEC2D3C17B3E53C27B841C45DE39866A91DE6647C3A11DEE81CCF7B192B8E1D4F7160D53D9AA98FD91EB2B4C8338AEC2714C939133BBC292941A570F43571F0CBCCFC6FDAEA3C8D6CB05F69F189711D7F25EECF09543C4409367E95734087C9B1EFF0AAA310F4D777F15CA2BBCAAFFFF398353D31EE22984EE7C65850F436C35F7F44D14B9AACE90ED26CFDB4E2FDCEB6DA7B0525AA305BB67F9EB072E9204F137229FFE2A397BFF6154DF9FC602744C51383A1F00651A1EE09C13658DAE7030E7CD2F4F940BFCEAA9F136C239AF629814BFA83890F09BA2F4379CB03EE438A43D33E96A454CFAD89D83B65651E7A6F92F2B5779AE8724E760FB841F3AE777351DE593EF3605BA7225D7930EC43DAFD9BE7281F4B5A72E9B41F361B799F09C80D974B7FA8BCE323C8945364FE1C3EBB78DFB6A68B021F798A66BF1CE2233336BECD1F3E5378DFC6A60B101FB9B1F5CA073E325B3BD4FE79604BEBBC851E3CBB574E54D2DCE6A8A2C86DD9BB59C81D8EFFAB008C20F328B34797EA74B1A654D7168625899EA93E4F4D642C4D1C89AF44D1CCB65F5FF986DFD8594ED3CC5693DDD9C49BAFFF8DBC394D336F4DCEE421F28E95598BAA5CF09675AC2995EA3DE519D77AD292D6DEE6B3365ECDBFA7B4E24194529B3D9ADBE5F793453C884A869C3A3DB286E58B62D83B2B7FB311F6EFD8DD9410EC2F38126CD776CD82E69AAC837CF31624CA498408CD0DA6C8812D751E51778D6C0AD52C009DBE1A4F837AEC1A64859D6B7297D030A1D065ECAFBC5AC08B39014DFCD3D4E8BACCD3BB907DC5437401C47459E0FE8EFC98B89E53C87DA5880969209877C1C3BD6C2C290BFB6E5E0BA4DB807404E2EA2B9CA207EC871E80C57764899EF136B281F97DC61B64BF9611401D48FB40D4D53EBD70D126427ECC31CAF6F00936ECF82FDFFF1FE4AD8AFFBA540000, N'6.2.0-61023')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'3', N'Disable')
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'2', N'Nhân viên')
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'1', N'Quản trị viên')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'01461e83-c100-49c2-8b9a-0a633179b8d8', N'2')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0f929735-5665-4168-87f6-3bd07e305dc9', N'2')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'27f9ff45-11b2-48ba-9abb-a2bebdf70893', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'34e295e4-79e0-4c38-a68c-f21c0484f378', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'3cdf3858-810c-4647-9ea7-570190599ae9', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'4dd6f054-8e27-4944-a35d-2a4e5250b509', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'71a0ff44-801a-48aa-92ae-82cb1e944477', N'2')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'80b8a619-4fd0-4fc7-9ecf-cc9397cebf2b', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'9786b4e7-bebb-472a-95f1-9fab0ec32e76', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'a020a6a7-2ec3-426b-8361-f796bea99b67', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'ab90be66-fae3-4411-9b56-6ea8e632feea', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'aed5f79f-2fcc-4fbe-8ef5-94ded6604e0e', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'ca554a57-c55a-41ff-918a-a318a38ec52d', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'caa4fe42-6c74-4647-8e1a-8a92ea6be31a', N'2')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'caf71c69-792c-425b-9533-ec99fd89a5ca', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e869b4f2-ea88-498a-844b-31a7e1e290eb', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'eecd5b1f-9ab0-4d57-8ec1-38b4502e335f', N'1')
GO
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'01461e83-c100-49c2-8b9a-0a633179b8d8', N'admin@email.com', 0, N'AMhR8Wupframd3nxalVFxycQ+k6l9BqcQMRN2FEhs3SPQH6yBJyYxZgNl9nLUnLBTg==', N'28ae5b06-4927-4477-812a-76c0ba02bf2d', N'0934047631', 1, 0, NULL, 1, 2, N'Hoàng Phng')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'0f929735-5665-4168-87f6-3bd07e305dc9', N'nemoshop00014@gmail.com', 0, N'AOq/z/vbP/P2DSJL07hxYB6ls+ismhd64AdWUp8VGzlis8lxxZTcq7RIrAvPb7ElUg==', N'e3bfa658-91e8-41a3-9ade-02f951141af4', N'123456788', 1, 0, NULL, 1, 2, N'nemoshop00014@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'27f9ff45-11b2-48ba-9abb-a2bebdf70893', N'phong@gmail.com', 0, N'AMU/cQsN03lwdqqvBdoPmbEyANb+aVxK9PPCMJOQx3d5skIJwi7NK5I+Sp4bD81Rag==', N'8fd8d3ff-9c78-4a29-94df-bfee86cf8e31', N'085481525', 1, 0, NULL, 1, 0, N'phong@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'34e295e4-79e0-4c38-a68c-f21c0484f378', N'dat12@gmail.com', 0, N'APDjBmHOfxKSXD/cNUZ6W266bUVNxPFmw6/3TtVGsc1mWWWvKwbnP0uBd4T+uaxaFQ==', N'31cbca38-3fe4-4f5c-aedb-090808b66d7e', N'0889262990', 1, 0, NULL, 1, 0, N'dat12@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'3cdf3858-810c-4647-9ea7-570190599ae9', N'phong28@gmail.com', 0, N'AJ9GeJe2oBwkSJWAzixgJg/UyUNHYse9w8ypvCJMhzRSopZfhkvjrJj/lpGO18eYPQ==', N'd6d21d8e-dd47-4b92-a96a-bacca0632ab7', N'0934047631', 0, 0, NULL, 1, 1, N'phong28@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'4dd6f054-8e27-4944-a35d-2a4e5250b509', N'dat@gmail.com', 0, N'ALwDZjwyrPhePnrk4MIu8Qfel1GE+XOOst8ekQGCumKIke9Zl9yrQuWVPosRkGuPvw==', N'a0bccc3a-801e-49bd-b680-75e2c960a5b7', N'0889262990', 1, 0, NULL, 1, 0, N'dat@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'71a0ff44-801a-48aa-92ae-82cb1e944477', N'nguyeng@mail.com', 0, N'AIBM+BTVwvDCeKK1hl7C/OtqTtUihI1UlCQvyeNXUnOxqVda7RCHdEMYmW17tjl8fA==', N'87f7ee01-293d-4530-ae9b-b38334b1a703', N'0123456789', 1, 0, NULL, 1, 2, N'nguyeng@mail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'80b8a619-4fd0-4fc7-9ecf-cc9397cebf2b', N'arrowonthemic@gmail.com', 0, N'AB2lxcwomTNCT13BGQ8J9EHAYWtW70Zi2kVv8RlIJ1Zf38iaLemgOTTwbVtxC+PCCA==', N'1ade8cb9-b02a-4a9e-b179-3129514b6129', N'0889262990', 1, 0, NULL, 1, 0, N'arrowonthemic@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'9786b4e7-bebb-472a-95f1-9fab0ec32e76', N'tranquocnam201@gmail.com', 0, N'AO0329zdsaTbC6JbOzuAUVKMBBEY40u7JombwyUSxDnSjsb5l9E39Iyt9M5/125WMQ==', N'3b555dcb-780d-471f-bbb9-6bb8d4b7ebc7', N'0854858818', 1, 0, NULL, 1, 0, N'tranquocnam201@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'a020a6a7-2ec3-426b-8361-f796bea99b67', N'ngoc@gmail.com', 0, N'AKusoJI32zHXA7RX9GIgyYo6bGOSls9jJM5qRLNG3hLV+Q7p2V08StraESXeqe/pJA==', N'2bcd926c-32b1-4096-b258-b219a88defd1', N'0854858818', 1, 0, NULL, 1, 0, N'ngoc@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'ab90be66-fae3-4411-9b56-6ea8e632feea', N'dat1@gmail.com', 0, N'AAeuLNaIHgGqDaP/dugKam8YrxqUN9pq/wpQ6ZpfrJ5T9IM5qmcbiWXN8naR9ba0KQ==', N'e84d686b-ef90-4889-a234-f1bbdea208c9', N'0889262990', 1, 0, NULL, 1, 0, N'dat1@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'aed5f79f-2fcc-4fbe-8ef5-94ded6604e0e', N'datkillua3@gmail.com', 0, N'AFtN0jr1fzbIIDnmNPeXVo29A8oClLYBGF2n3jBQZ2ypnEXA9XHi4T6wcQwW2bzcWQ==', N'ff16790e-7e13-4601-b190-7fed04d40e3c', N'12345678', 1, 0, NULL, 1, 0, N'datkillua3@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'ca554a57-c55a-41ff-918a-a318a38ec52d', N'dat123@gmail.com', 0, N'AFYD3dKoobvfxg2FBOa5HdWuQTSXgZAQl+HMV8saKKV49YjtCjlcfx8XsTCIMZme8w==', N'2604de14-44a9-4cc0-994a-1e0eaf6de879', N'0889262990', 1, 0, NULL, 1, 0, N'dat123@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'caa4fe42-6c74-4647-8e1a-8a92ea6be31a', N'nguyenvoanhnguyen.lop9a7@gmail.com', 0, N'ALVmzZ0yyUHjQ8JdfZ2J7Is2ufrjW1uf9Y2ftyjTo+MyBxdnjyFVdgVhrhzWQR3nsQ==', N'f529407b-3c57-47ac-8215-ed5f1729ecf0', N'0123456789', 1, 0, NULL, 1, 0, N'nguyenvoanhnguyen.lop9a7@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'caf71c69-792c-425b-9533-ec99fd89a5ca', N'admin@gmail.com', 0, N'AE6BjMSlVxX7v2MVFLaR4C4HDtnCq6TiYL8VG3SN8CAAmXjYssj4x/rpA1EX4rKlEw==', N'30579288-4f3b-48a4-a631-f8fd6f508c72', NULL, 1, 0, NULL, 1, 0, N'admin@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'e869b4f2-ea88-498a-844b-31a7e1e290eb', N'trongcong@gmail.com', 0, N'AB7afec7r7QCsEUYitslI88POZzWCQzr6zhqYXpw+Do7HGCj9zVQEnZ0UXFp/B+AcQ==', N'3ac6c558-6670-4f8e-802d-af485b83af4d', N'0362059587', 1, 0, NULL, 1, 0, N'trongcong@gmail.com')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'eecd5b1f-9ab0-4d57-8ec1-38b4502e335f', N'minh@gmail.com', 0, N'AEFgdwagtjj3Tbefhnw2b++ZXZhVBjZcq8S3RBNUrXLzNocBqjVjwLcD9lPhLUfO1g==', N'ba6eec6a-5a61-4a0e-9f97-3ee05f4703ac', N'0917278756', 1, 0, NULL, 1, 0, N'minh@gmail.com')
GO
SET IDENTITY_INSERT [dbo].[categories] ON 

INSERT [dbo].[categories] ([id], [code], [created_by], [name], [slug], [status], [created_at], [updated_at], [deleted_at]) VALUES (116, N'DMhhLrR2qSP', N'eecd5b1f-9ab0-4d57-8ec1-38b4502e335f', N'Thuốc bảo vệ thực vật', N'Thuốc bảo vệ thực vật', 1, CAST(N'2023-02-17T22:53:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[categories] ([id], [code], [created_by], [name], [slug], [status], [created_at], [updated_at], [deleted_at]) VALUES (117, N'DMQ3hY27VdJ', N'eecd5b1f-9ab0-4d57-8ec1-38b4502e335f', N'Phân bón', N'Phân bón', 1, CAST(N'2023-02-17T22:53:05.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[categories] ([id], [code], [created_by], [name], [slug], [status], [created_at], [updated_at], [deleted_at]) VALUES (118, N'DMxDXfUI16k', N'eecd5b1f-9ab0-4d57-8ec1-38b4502e335f', N'Nhiên liệu', N'Nhiên liệu', 1, CAST(N'2023-02-17T22:53:18.0000000' AS DateTime2), NULL, NULL)
SET IDENTITY_INSERT [dbo].[categories] OFF
GO
SET IDENTITY_INSERT [dbo].[customers] ON 

INSERT [dbo].[customers] ([id], [created_by], [name], [email], [phone], [address], [type], [status], [account_number], [bank], [note], [birthday], [price_level], [created_at], [updated_at], [deleted_at], [code]) VALUES (21, N'22', N'Lại Khắc Đạt', N'dat@gmail.com', N'0123456789', N'Đak Lak', 0, 1, N'75210000086654', N'BIDV', N'thi?u', CAST(N'2023-02-17T00:00:00.0000000' AS DateTime2), 0, CAST(N'2022-12-30T22:15:09.0000000' AS DateTime2), CAST(N'2023-02-22T22:02:43.0000000' AS DateTime2), NULL, N'MKHO9xalj3RW')
INSERT [dbo].[customers] ([id], [created_by], [name], [email], [phone], [address], [type], [status], [account_number], [bank], [note], [birthday], [price_level], [created_at], [updated_at], [deleted_at], [code]) VALUES (22, N'22', N'Hoàng Phong', NULL, N'0934047631', N'Bình Tân, BHH.A', 0, 1, NULL, N'Vietcombank', NULL, CAST(N'2023-01-12T00:00:00.0000000' AS DateTime2), 0, CAST(N'2023-01-04T13:02:47.0000000' AS DateTime2), CAST(N'2023-02-16T21:44:37.0000000' AS DateTime2), NULL, N'MKHEBCnGtlKc')
INSERT [dbo].[customers] ([id], [created_by], [name], [email], [phone], [address], [type], [status], [account_number], [bank], [note], [birthday], [price_level], [created_at], [updated_at], [deleted_at], [code]) VALUES (23, N'22', N'Anh Nguyên', NULL, N'02164845', N'hcm', 2, 1, NULL, N'BIDV', NULL, CAST(N'2023-01-06T00:00:00.0000000' AS DateTime2), 0, CAST(N'2023-01-04T13:59:05.0000000' AS DateTime2), CAST(N'2023-02-17T13:09:13.0000000' AS DateTime2), NULL, N'MKHVBTxxM96N')
INSERT [dbo].[customers] ([id], [created_by], [name], [email], [phone], [address], [type], [status], [account_number], [bank], [note], [birthday], [price_level], [created_at], [updated_at], [deleted_at], [code]) VALUES (24, N'22', N'Võ Nhật Minh', N'vonhatminh1321@gmail.com', N'0917278756', N'sfffdsf', 0, 1, NULL, NULL, NULL, CAST(N'2023-01-21T00:00:00.0000000' AS DateTime2), 0, CAST(N'2023-01-04T22:20:48.0000000' AS DateTime2), CAST(N'2023-01-05T13:53:40.0000000' AS DateTime2), NULL, N'MKHTUinc85Cx')
INSERT [dbo].[customers] ([id], [created_by], [name], [email], [phone], [address], [type], [status], [account_number], [bank], [note], [birthday], [price_level], [created_at], [updated_at], [deleted_at], [code]) VALUES (25, N'22', N'Quốc Nam', N'tranquocnam201@gmail.com', N'0854858818', N'Rạch Giá - Kiên Giang', 2, 1, N'75210000089921', N'BIDV', NULL, CAST(N'2001-03-29T00:00:00.0000000' AS DateTime2), 0, CAST(N'2023-01-05T13:41:05.0000000' AS DateTime2), CAST(N'2023-02-22T13:14:30.0000000' AS DateTime2), NULL, N'MKHQ7Yd1QfjA')
INSERT [dbo].[customers] ([id], [created_by], [name], [email], [phone], [address], [type], [status], [account_number], [bank], [note], [birthday], [price_level], [created_at], [updated_at], [deleted_at], [code]) VALUES (26, N'22', N'Trọng Công', NULL, N'05465455454', N'hn', 0, 1, NULL, NULL, NULL, CAST(N'2023-01-07T00:00:00.0000000' AS DateTime2), 0, CAST(N'2023-01-05T14:22:29.0000000' AS DateTime2), CAST(N'2023-01-09T23:47:58.0000000' AS DateTime2), NULL, N'MKH2qcaCR4nJ')
INSERT [dbo].[customers] ([id], [created_by], [name], [email], [phone], [address], [type], [status], [account_number], [bank], [note], [birthday], [price_level], [created_at], [updated_at], [deleted_at], [code]) VALUES (27, N'caf71c69-792c-425b-9533-ec99fd89a5ca', N'Huỳnh Trọng Công', N'trongcong@gmail.com', N'0362059587', N'57 LTP', 0, 1, N'221453132', N'VCB', NULL, CAST(N'2023-02-23T00:00:00.0000000' AS DateTime2), 0, CAST(N'2023-02-17T09:45:55.0000000' AS DateTime2), CAST(N'2023-02-17T09:46:59.0000000' AS DateTime2), NULL, N'MKHm67DB3nAY')
INSERT [dbo].[customers] ([id], [created_by], [name], [email], [phone], [address], [type], [status], [account_number], [bank], [note], [birthday], [price_level], [created_at], [updated_at], [deleted_at], [code]) VALUES (28, N'9786b4e7-bebb-472a-95f1-9fab0ec32e76', N'Nhà cung cấp phân bón	', NULL, N'0858858818', N'HCM', 2, 1, NULL, NULL, NULL, NULL, 0, CAST(N'2023-02-22T21:33:58.0000000' AS DateTime2), NULL, NULL, N'MKHa7snnVEjN')
INSERT [dbo].[customers] ([id], [created_by], [name], [email], [phone], [address], [type], [status], [account_number], [bank], [note], [birthday], [price_level], [created_at], [updated_at], [deleted_at], [code]) VALUES (29, N'9786b4e7-bebb-472a-95f1-9fab0ec32e76', N'Nhà cung cấp thuốc trừ sâu', NULL, N'0858858858', N'HCM', 2, 1, NULL, NULL, NULL, NULL, 0, CAST(N'2023-02-22T21:34:26.0000000' AS DateTime2), NULL, NULL, N'MKHKf9W958Rj')
INSERT [dbo].[customers] ([id], [created_by], [name], [email], [phone], [address], [type], [status], [account_number], [bank], [note], [birthday], [price_level], [created_at], [updated_at], [deleted_at], [code]) VALUES (30, N'9786b4e7-bebb-472a-95f1-9fab0ec32e76', N'Hữu Hiệp', NULL, N'08547977978', N'Quận 12, HCM', 1, 1, NULL, NULL, NULL, NULL, 0, CAST(N'2023-02-22T21:47:51.0000000' AS DateTime2), NULL, NULL, N'MKH21X124MkN')
SET IDENTITY_INSERT [dbo].[customers] OFF
GO
SET IDENTITY_INSERT [dbo].[groups] ON 

INSERT [dbo].[groups] ([id], [code], [created_by], [name], [slug], [status], [created_at], [updated_at], [deleted_at]) VALUES (160, N'NH2RfO8I8if', N'9786b4e7-bebb-472a-95f1-9fab0ec32e76', N'Nhom hang 1', N'Nhom hang 1', 1, CAST(N'2023-02-19T18:47:02.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[groups] ([id], [code], [created_by], [name], [slug], [status], [created_at], [updated_at], [deleted_at]) VALUES (161, N'NHe6228SwrK', N'eecd5b1f-9ab0-4d57-8ec1-38b4502e335f', N'Parystol', N'Parystol', 1, CAST(N'2023-02-20T13:16:12.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[groups] ([id], [code], [created_by], [name], [slug], [status], [created_at], [updated_at], [deleted_at]) VALUES (162, N'NHc1dU6n85f', N'eecd5b1f-9ab0-4d57-8ec1-38b4502e335f', N'Đầu trâu', N'Ð?u trâu', 1, CAST(N'2023-02-20T13:16:16.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[groups] ([id], [code], [created_by], [name], [slug], [status], [created_at], [updated_at], [deleted_at]) VALUES (163, N'NHjwVrhuTil', N'eecd5b1f-9ab0-4d57-8ec1-38b4502e335f', N'Con cò', N'Con cò', 1, CAST(N'2023-02-20T13:16:20.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[groups] ([id], [code], [created_by], [name], [slug], [status], [created_at], [updated_at], [deleted_at]) VALUES (164, N'NHsH33pf7vJ', N'eecd5b1f-9ab0-4d57-8ec1-38b4502e335f', N'ADAMA', N'ADAMA', 1, CAST(N'2023-02-22T22:25:30.0000000' AS DateTime2), NULL, NULL)
SET IDENTITY_INSERT [dbo].[groups] OFF
GO
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'01461e83-c100-49c2-8b9a-0a633179b8d8', N'Hoàng Phng', N'admin@email.com', NULL, N'0934047631', N'15a HHTT', N'28ae5b06-4927-4477-812a-76c0ba02bf2d', CAST(N'2023-02-07T14:29:05.0000000' AS DateTime2), CAST(N'2023-02-17T09:38:07.0000000' AS DateTime2), NULL, N'01461e83-c100-49c2-8b9a-0a633179b8d8')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'0f929735-5665-4168-87f6-3bd07e305dc9', N'Nguyễn Việt Hoàng', N'nemoshop00014@gmail.com', NULL, N'123456788', N'fhhgh', N'e3bfa658-91e8-41a3-9ade-02f951141af4', CAST(N'2023-02-15T19:22:07.0000000' AS DateTime2), CAST(N'2023-02-22T21:29:50.0000000' AS DateTime2), NULL, N'0f929735-5665-4168-87f6-3bd07e305dc9')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'22', N'Admin', N'admin@gmail.com', NULL, NULL, NULL, N'30579288-4f3b-48a4-a631-f8fd6f508c72', NULL, NULL, NULL, N'caf71c69-792c-425b-9533-ec99fd89a5ca')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'27f9ff45-11b2-48ba-9abb-a2bebdf70893', N'Trần Hoàng Phong', N'phong@gmail.com', NULL, N'085481525', N'', N'8fd8d3ff-9c78-4a29-94df-bfee86cf8e31', CAST(N'2023-02-09T23:16:02.0000000' AS DateTime2), NULL, NULL, N'27f9ff45-11b2-48ba-9abb-a2bebdf70893')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'34e295e4-79e0-4c38-a68c-f21c0484f378', N'Lại Khắc Đạt', N'dat12@gmail.com', NULL, N'0889262990', N'', N'31cbca38-3fe4-4f5c-aedb-090808b66d7e', CAST(N'2023-02-22T17:27:21.0000000' AS DateTime2), NULL, NULL, N'34e295e4-79e0-4c38-a68c-f21c0484f378')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'3cdf3858-810c-4647-9ea7-570190599ae9', N'@%Phong!^', N'phong28@gmail.com', NULL, N'0934047631', N'22', N'd6d21d8e-dd47-4b92-a96a-bacca0632ab7', CAST(N'2023-02-11T16:47:58.0000000' AS DateTime2), CAST(N'2023-02-13T17:02:45.0000000' AS DateTime2), NULL, N'3cdf3858-810c-4647-9ea7-570190599ae9')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'4dd6f054-8e27-4944-a35d-2a4e5250b509', N'Lại Khắc Đạt', N'dat@gmail.com', NULL, N'0889262990', N'', N'a0bccc3a-801e-49bd-b680-75e2c960a5b7', CAST(N'2023-02-15T19:13:44.0000000' AS DateTime2), CAST(N'2023-02-15T22:06:41.0000000' AS DateTime2), NULL, N'4dd6f054-8e27-4944-a35d-2a4e5250b509')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'71a0ff44-801a-48aa-92ae-82cb1e944477', N'Ng738*^', N'nguyeng@mail.com', NULL, N'0123456789', N'', N'87f7ee01-293d-4530-ae9b-b38334b1a703', CAST(N'2023-02-13T16:36:47.0000000' AS DateTime2), CAST(N'2023-02-13T16:39:23.0000000' AS DateTime2), NULL, N'71a0ff44-801a-48aa-92ae-82cb1e944477')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'80b8a619-4fd0-4fc7-9ecf-cc9397cebf2b', N'Lại Khắc Đạt', N'arrowonthemic@gmail.com', NULL, N'0889262990', N'', N'1ade8cb9-b02a-4a9e-b179-3129514b6129', CAST(N'2023-02-22T21:28:59.0000000' AS DateTime2), NULL, NULL, N'80b8a619-4fd0-4fc7-9ecf-cc9397cebf2b')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'9786b4e7-bebb-472a-95f1-9fab0ec32e76', N'Trần Quốc Nam', N'tranquocnam201@gmail.com', NULL, N'0854858818', N'', N'3b555dcb-780d-471f-bbb9-6bb8d4b7ebc7', CAST(N'2023-02-07T14:30:40.0000000' AS DateTime2), NULL, NULL, N'9786b4e7-bebb-472a-95f1-9fab0ec32e76')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'a020a6a7-2ec3-426b-8361-f796bea99b67', N'Nguyễn Tân Duy', N'ngoc@gmail.com', NULL, N'0854858818', N'', N'2bcd926c-32b1-4096-b258-b219a88defd1', CAST(N'2023-02-22T21:30:23.0000000' AS DateTime2), NULL, NULL, N'a020a6a7-2ec3-426b-8361-f796bea99b67')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'ab90be66-fae3-4411-9b56-6ea8e632feea', N'Lại Khắc Đạt', N'dat1@gmail.com', NULL, N'0889262990', N'', N'e84d686b-ef90-4889-a234-f1bbdea208c9', CAST(N'2023-02-22T17:26:51.0000000' AS DateTime2), NULL, NULL, N'ab90be66-fae3-4411-9b56-6ea8e632feea')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'aed5f79f-2fcc-4fbe-8ef5-94ded6604e0e', N'Phạm Văn Vinh', N'datkillua3@gmail.com', NULL, N'12345678', N'', N'ff16790e-7e13-4601-b190-7fed04d40e3c', CAST(N'2023-02-15T19:21:35.0000000' AS DateTime2), NULL, NULL, N'aed5f79f-2fcc-4fbe-8ef5-94ded6604e0e')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'ca554a57-c55a-41ff-918a-a318a38ec52d', N'Lại Khắc Đạt', N'dat123@gmail.com', NULL, N'0889262990', N'', N'2604de14-44a9-4cc0-994a-1e0eaf6de879', CAST(N'2023-02-22T21:31:43.0000000' AS DateTime2), NULL, NULL, N'ca554a57-c55a-41ff-918a-a318a38ec52d')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'caa4fe42-6c74-4647-8e1a-8a92ea6be31a', N'Nguyễn Nguyên', N'nguyenvoanhnguyen.lop9a7@gmail.com', NULL, N'0123456789', N'Xô Vi?t Ngh? Tinh', N'f529407b-3c57-47ac-8215-ed5f1729ecf0', CAST(N'2023-02-13T16:14:58.0000000' AS DateTime2), NULL, NULL, N'caa4fe42-6c74-4647-8e1a-8a92ea6be31a')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'e869b4f2-ea88-498a-844b-31a7e1e290eb', N'Huỳnh Trọng Công', N'trongcong@gmail.com', NULL, N'0362059587', N'57 LTP', N'3ac6c558-6670-4f8e-802d-af485b83af4d', CAST(N'2023-02-17T09:40:12.0000000' AS DateTime2), NULL, NULL, N'e869b4f2-ea88-498a-844b-31a7e1e290eb')
INSERT [dbo].[users] ([id], [name], [email], [avatar], [phone], [address], [remember_token], [created_at], [updated_at], [deleted_at], [asp_id]) VALUES (N'eecd5b1f-9ab0-4d57-8ec1-38b4502e335f', N'Võ Nhật Minh', N'minh@gmail.com', NULL, N'0917278756', N'', N'ba6eec6a-5a61-4a0e-9f97-3ee05f4703ac', CAST(N'2023-02-06T23:26:45.0000000' AS DateTime2), CAST(N'2023-02-09T23:16:26.0000000' AS DateTime2), NULL, N'eecd5b1f-9ab0-4d57-8ec1-38b4502e335f')
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 23/02/2023 4:21:31 CH ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 23/02/2023 4:21:31 CH ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 23/02/2023 4:21:31 CH ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RoleId]    Script Date: 23/02/2023 4:21:31 CH ******/
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 23/02/2023 4:21:31 CH ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[carts] ADD  CONSTRAINT [DF__carts__note__24927208]  DEFAULT (NULL) FOR [note]
GO
ALTER TABLE [dbo].[carts] ADD  CONSTRAINT [DF__carts__created_a__25869641]  DEFAULT (NULL) FOR [created_at]
GO
ALTER TABLE [dbo].[carts] ADD  CONSTRAINT [DF__carts__updated_a__267ABA7A]  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[categories] ADD  CONSTRAINT [DF__categorie__statu__286302EC]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[categories] ADD  CONSTRAINT [DF__categorie__creat__29572725]  DEFAULT (NULL) FOR [created_at]
GO
ALTER TABLE [dbo].[categories] ADD  CONSTRAINT [DF__categorie__updat__2A4B4B5E]  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[categories] ADD  CONSTRAINT [DF__categorie__delet__2B3F6F97]  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__email__2D27B809]  DEFAULT (NULL) FOR [email]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__statu__2E1BDC42]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__accou__2F10007B]  DEFAULT (NULL) FOR [account_number]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__bank__300424B4]  DEFAULT (NULL) FOR [bank]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__note__30F848ED]  DEFAULT (NULL) FOR [note]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__birth__31EC6D26]  DEFAULT (NULL) FOR [birthday]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__creat__32E0915F]  DEFAULT (NULL) FOR [created_at]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__updat__33D4B598]  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__delet__34C8D9D1]  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[debts] ADD  CONSTRAINT [DF__debts__paid__36B12243]  DEFAULT ((0)) FOR [paid]
GO
ALTER TABLE [dbo].[debts] ADD  CONSTRAINT [DF__debts__created_a__37A5467C]  DEFAULT (NULL) FOR [created_at]
GO
ALTER TABLE [dbo].[debts] ADD  CONSTRAINT [DF__debts__updated_a__38996AB5]  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[debts] ADD  CONSTRAINT [DF__debts__deleted_a__398D8EEE]  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[groups] ADD  CONSTRAINT [DF__groups__status__3B75D760]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[groups] ADD  CONSTRAINT [DF__groups__created___3C69FB99]  DEFAULT (NULL) FOR [created_at]
GO
ALTER TABLE [dbo].[groups] ADD  CONSTRAINT [DF__groups__updated___3D5E1FD2]  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[groups] ADD  CONSTRAINT [DF__groups__deleted___3E52440B]  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[import_inventory] ADD  CONSTRAINT [DF__import_in__quant__403A8C7D]  DEFAULT ((1)) FOR [quantity]
GO
ALTER TABLE [dbo].[import_inventory] ADD  CONSTRAINT [DF__import_inv__sold__412EB0B6]  DEFAULT ((0)) FOR [sold]
GO
ALTER TABLE [dbo].[import_inventory] ADD  CONSTRAINT [DF__import_in__creat__4316F928]  DEFAULT (NULL) FOR [created_at]
GO
ALTER TABLE [dbo].[import_inventory] ADD  CONSTRAINT [DF__import_in__updat__440B1D61]  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[import_inventory] ADD  CONSTRAINT [DF__import_in__delet__44FF419A]  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[products] ADD  CONSTRAINT [DF_products_quantity]  DEFAULT ((1)) FOR [quantity]
GO
ALTER TABLE [dbo].[products] ADD  CONSTRAINT [DF__products__status__534D60F1]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[products] ADD  CONSTRAINT [DF__products__note__5441852A]  DEFAULT (NULL) FOR [note]
GO
ALTER TABLE [dbo].[products] ADD  CONSTRAINT [DF__products__create__5535A963]  DEFAULT (NULL) FOR [created_at]
GO
ALTER TABLE [dbo].[products] ADD  CONSTRAINT [DF__products__update__5629CD9C]  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[products] ADD  CONSTRAINT [DF__products__delete__571DF1D5]  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[sale_details] ADD  CONSTRAINT [DF_sale_details_note]  DEFAULT (NULL) FOR [note]
GO
ALTER TABLE [dbo].[sale_details] ADD  CONSTRAINT [DF__sale_deta__creat__619B8048]  DEFAULT (NULL) FOR [created_at]
GO
ALTER TABLE [dbo].[sale_details] ADD  CONSTRAINT [DF__sale_deta__updat__628FA481]  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[sale_details] ADD  CONSTRAINT [DF__sale_deta__delet__6383C8BA]  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[sales] ADD  CONSTRAINT [DF__sales__note__5AEE82B9]  DEFAULT (NULL) FOR [note]
GO
ALTER TABLE [dbo].[sales] ADD  CONSTRAINT [DF__sales__status__5BE2A6F2]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[sales] ADD  CONSTRAINT [DF__sales__created_a__5CD6CB2B]  DEFAULT (NULL) FOR [created_at]
GO
ALTER TABLE [dbo].[sales] ADD  CONSTRAINT [DF__sales__updated_a__5DCAEF64]  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[sales] ADD  CONSTRAINT [DF__sales__deleted_a__5EBF139D]  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF__users__avatar__66603565]  DEFAULT (NULL) FOR [avatar]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF__users__phone__6754599E]  DEFAULT (NULL) FOR [phone]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF__users__address__68487DD7]  DEFAULT (NULL) FOR [address]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF__users__remember___6B24EA82]  DEFAULT (NULL) FOR [remember_token]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF__users__created_a__6C190EBB]  DEFAULT (NULL) FOR [created_at]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF__users__updated_a__6D0D32F4]  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF__users__deleted_a__6E01572D]  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[carts]  WITH CHECK ADD  CONSTRAINT [carts_ibfk_1] FOREIGN KEY([product_id])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[carts] CHECK CONSTRAINT [carts_ibfk_1]
GO
ALTER TABLE [dbo].[carts]  WITH CHECK ADD  CONSTRAINT [carts_ibfk_2] FOREIGN KEY([customer_id])
REFERENCES [dbo].[customers] ([id])
GO
ALTER TABLE [dbo].[carts] CHECK CONSTRAINT [carts_ibfk_2]
GO
ALTER TABLE [dbo].[categories]  WITH CHECK ADD  CONSTRAINT [FK_categories_users] FOREIGN KEY([created_by])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[categories] CHECK CONSTRAINT [FK_categories_users]
GO
ALTER TABLE [dbo].[debts]  WITH CHECK ADD  CONSTRAINT [debts_ibfk_1] FOREIGN KEY([sale_id])
REFERENCES [dbo].[sales] ([id])
GO
ALTER TABLE [dbo].[debts] CHECK CONSTRAINT [debts_ibfk_1]
GO
ALTER TABLE [dbo].[debts]  WITH CHECK ADD  CONSTRAINT [debts_ibfk_2] FOREIGN KEY([created_by])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[debts] CHECK CONSTRAINT [debts_ibfk_2]
GO
ALTER TABLE [dbo].[groups]  WITH CHECK ADD  CONSTRAINT [FK_groups_users] FOREIGN KEY([created_by])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[groups] CHECK CONSTRAINT [FK_groups_users]
GO
ALTER TABLE [dbo].[import_inventory]  WITH CHECK ADD  CONSTRAINT [FK_import_inventory_customers] FOREIGN KEY([supplier_id])
REFERENCES [dbo].[customers] ([id])
GO
ALTER TABLE [dbo].[import_inventory] CHECK CONSTRAINT [FK_import_inventory_customers]
GO
ALTER TABLE [dbo].[import_inventory]  WITH CHECK ADD  CONSTRAINT [FK_import_inventory_users] FOREIGN KEY([created_by])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[import_inventory] CHECK CONSTRAINT [FK_import_inventory_users]
GO
ALTER TABLE [dbo].[import_inventory]  WITH CHECK ADD  CONSTRAINT [import_inventory_ibfk_2] FOREIGN KEY([product_id])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[import_inventory] CHECK CONSTRAINT [import_inventory_ibfk_2]
GO
ALTER TABLE [dbo].[products]  WITH CHECK ADD  CONSTRAINT [FK_products_users] FOREIGN KEY([created_by])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[products] CHECK CONSTRAINT [FK_products_users]
GO
ALTER TABLE [dbo].[products]  WITH CHECK ADD  CONSTRAINT [products_ibfk_4] FOREIGN KEY([category_id])
REFERENCES [dbo].[categories] ([id])
GO
ALTER TABLE [dbo].[products] CHECK CONSTRAINT [products_ibfk_4]
GO
ALTER TABLE [dbo].[products]  WITH CHECK ADD  CONSTRAINT [products_ibfk_5] FOREIGN KEY([group_id])
REFERENCES [dbo].[groups] ([id])
GO
ALTER TABLE [dbo].[products] CHECK CONSTRAINT [products_ibfk_5]
GO
ALTER TABLE [dbo].[return_details]  WITH CHECK ADD  CONSTRAINT [FK_return_details_products] FOREIGN KEY([product_current_id])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[return_details] CHECK CONSTRAINT [FK_return_details_products]
GO
ALTER TABLE [dbo].[return_details]  WITH CHECK ADD  CONSTRAINT [FK_return_details_products1] FOREIGN KEY([product_return_id])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[return_details] CHECK CONSTRAINT [FK_return_details_products1]
GO
ALTER TABLE [dbo].[return_details]  WITH CHECK ADD  CONSTRAINT [FK_return_details_return] FOREIGN KEY([return_id])
REFERENCES [dbo].[return_sale] ([id])
GO
ALTER TABLE [dbo].[return_details] CHECK CONSTRAINT [FK_return_details_return]
GO
ALTER TABLE [dbo].[return_sale]  WITH CHECK ADD  CONSTRAINT [FK_return_sales] FOREIGN KEY([sale_id])
REFERENCES [dbo].[sales] ([id])
GO
ALTER TABLE [dbo].[return_sale] CHECK CONSTRAINT [FK_return_sales]
GO
ALTER TABLE [dbo].[return_supplier]  WITH CHECK ADD  CONSTRAINT [FK_return_supplier_import_inventory] FOREIGN KEY([inventory_id])
REFERENCES [dbo].[import_inventory] ([id])
GO
ALTER TABLE [dbo].[return_supplier] CHECK CONSTRAINT [FK_return_supplier_import_inventory]
GO
ALTER TABLE [dbo].[revenue]  WITH CHECK ADD  CONSTRAINT [FK_revenue_import_inventory] FOREIGN KEY([inventory_id])
REFERENCES [dbo].[import_inventory] ([id])
GO
ALTER TABLE [dbo].[revenue] CHECK CONSTRAINT [FK_revenue_import_inventory]
GO
ALTER TABLE [dbo].[revenue]  WITH CHECK ADD  CONSTRAINT [FK_revenue_sale_details] FOREIGN KEY([sale_details_id])
REFERENCES [dbo].[sale_details] ([id])
GO
ALTER TABLE [dbo].[revenue] CHECK CONSTRAINT [FK_revenue_sale_details]
GO
ALTER TABLE [dbo].[sale_details]  WITH CHECK ADD  CONSTRAINT [sale_details_ibfk_1] FOREIGN KEY([product_id])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[sale_details] CHECK CONSTRAINT [sale_details_ibfk_1]
GO
ALTER TABLE [dbo].[sale_details]  WITH CHECK ADD  CONSTRAINT [sale_details_ibfk_2] FOREIGN KEY([sale_id])
REFERENCES [dbo].[sales] ([id])
GO
ALTER TABLE [dbo].[sale_details] CHECK CONSTRAINT [sale_details_ibfk_2]
GO
ALTER TABLE [dbo].[sales]  WITH CHECK ADD  CONSTRAINT [FK_sales_customers] FOREIGN KEY([customer_id])
REFERENCES [dbo].[customers] ([id])
GO
ALTER TABLE [dbo].[sales] CHECK CONSTRAINT [FK_sales_customers]
GO
ALTER TABLE [dbo].[sales]  WITH CHECK ADD  CONSTRAINT [FK_sales_users] FOREIGN KEY([created_by])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[sales] CHECK CONSTRAINT [FK_sales_users]
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD  CONSTRAINT [FK_users_AspNetUsers] FOREIGN KEY([asp_id])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[users] CHECK CONSTRAINT [FK_users_AspNetUsers]
GO
USE [master]
GO
ALTER DATABASE [CP25Team05] SET  READ_WRITE 
GO
