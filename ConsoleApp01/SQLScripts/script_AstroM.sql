
/****** Object:  Table [dbo].[Appointment]    Script Date: 3/20/2016 5:24:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Appointment](
	[AppointmentId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ScheduleId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[Comment] [nvarchar](1000) NULL,
	[IsActive] [bit] NOT NULL,
	[ModifiedBy] [nvarchar](256) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PKAppointment] PRIMARY KEY CLUSTERED 
(
	[AppointmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Availability]    Script Date: 3/20/2016 5:24:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Availability](
	[AvailabilityId] [int] IDENTITY(1,1) NOT NULL,
	[LocationId] [int] NOT NULL,
	[FromDate] [datetime] NOT NULL,
	[ToDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[ModifiedBy] [nvarchar](256) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PKAvailability] PRIMARY KEY CLUSTERED 
(
	[AvailabilityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LocationMaster]    Script Date: 3/20/2016 5:24:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationMaster](
	[LocationId] [int] IDENTITY(1,1) NOT NULL,
	[City] [nvarchar](256) NOT NULL,
	[Country] [nvarchar](256) NOT NULL,
	[Address] [nvarchar](1000) NULL,
	[IsActive] [bit] NOT NULL,
	[ModifiedBy] [nvarchar](256) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PKLocationMaster] PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ScheduleMaster]    Script Date: 3/20/2016 5:24:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleMaster](
	[ScheduleId] [int] IDENTITY(1,1) NOT NULL,
	[AppointmentDate] [datetime] NOT NULL,
	[AppointmentTime] [time](7) NOT NULL,
	[SlotNumber] [int] NOT NULL,
	[IsAvailable] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[ModifiedBy] [nvarchar](256) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PKScheduleMaster] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StatusMaster]    Script Date: 3/20/2016 5:24:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatusMaster](
	[StatusId] [int] IDENTITY(1,1) NOT NULL,
	[StatusName] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[IsActive] [bit] NOT NULL,
	[ModifiedBy] [nvarchar](256) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PKStatusMaster] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 3/20/2016 5:24:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](256) NOT NULL,
	[Password] [nvarchar](256) NOT NULL,
	[FirstName] [nvarchar](256) NOT NULL,
	[LastName] [nvarchar](256) NULL,
	[EmailId] [nvarchar](256) NULL,
	[UserRoleId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[ModifiedBy] [nvarchar](256) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PKUser] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 3/20/2016 5:24:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[UserRoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[IsActive] [bit] NOT NULL,
	[ModifiedBy] [nvarchar](256) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PKUserRole] PRIMARY KEY CLUSTERED 
(
	[UserRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Appointment] ADD  CONSTRAINT [DF_Appointment_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Availability] ADD  CONSTRAINT [DF_Availability_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[LocationMaster] ADD  CONSTRAINT [DF_LocationMaster_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ScheduleMaster] ADD  CONSTRAINT [DF_ScheduleMaster_IsAvailable]  DEFAULT ((1)) FOR [IsAvailable]
GO
ALTER TABLE [dbo].[ScheduleMaster] ADD  CONSTRAINT [DF_ScheduleMaster_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[StatusMaster] ADD  CONSTRAINT [DF_Status_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_UserRoleId]  DEFAULT ((1)) FOR [UserRoleId]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[UserRole] ADD  CONSTRAINT [DF_UserRole_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Appointment]  WITH CHECK ADD  CONSTRAINT [FKAppointmentScheduleMaster] FOREIGN KEY([ScheduleId])
REFERENCES [dbo].[ScheduleMaster] ([ScheduleId])
GO
ALTER TABLE [dbo].[Appointment] CHECK CONSTRAINT [FKAppointmentScheduleMaster]
GO
ALTER TABLE [dbo].[Appointment]  WITH CHECK ADD  CONSTRAINT [FKAppointmentStatusMaster] FOREIGN KEY([StatusId])
REFERENCES [dbo].[StatusMaster] ([StatusId])
GO
ALTER TABLE [dbo].[Appointment] CHECK CONSTRAINT [FKAppointmentStatusMaster]
GO
ALTER TABLE [dbo].[Appointment]  WITH CHECK ADD  CONSTRAINT [FKAppointmentUser] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Appointment] CHECK CONSTRAINT [FKAppointmentUser]
GO
ALTER TABLE [dbo].[Availability]  WITH CHECK ADD  CONSTRAINT [FKAvailabilityLocationMaster] FOREIGN KEY([LocationId])
REFERENCES [dbo].[LocationMaster] ([LocationId])
GO
ALTER TABLE [dbo].[Availability] CHECK CONSTRAINT [FKAvailabilityLocationMaster]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FKUserUserRole] FOREIGN KEY([UserRoleId])
REFERENCES [dbo].[UserRole] ([UserRoleId])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FKUserUserRole]
GO
/****** Object:  StoredProcedure [dbo].[USPGetUserRoleByUserId]    Script Date: 3/20/2016 5:24:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USPGetUserRoleByUserId]	
(
	@userId INT
)
AS
BEGIN
	BEGIN TRY
		SELECT 
			[RoleName]	AS RoleName
		FROM
			[dbo].[UserRole]
			INNER JOIN [dbo].[User] ON [dbo].[UserRole].[UserRoleId] = [dbo].[User].[UserRoleId]
									AND [dbo].[User].[UserId] = @userId	
		
	END TRY
	BEGIN CATCH
		THROW
	END CATCH
   
END
GO

-- end of file