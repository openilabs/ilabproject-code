/***
	$Id$

A simple script to add Ticket_Types and ProcessAgent_Types to existing iLab databases.
It should not effect databases where the new types have already been added. (between 4.0 & 4.3)

**********/

if EXISTS (select processAgent_Type_ID from PROCESSAGENT_Type where Description = 'AUTHORIZATION SERVICE')
	BEGIN
		if EXISTS(select processAgent_Type_ID from PROCESSAGENT_Type where Description = 'AUTHORIZATION SERVICE' AND processAgent_Type_ID = 128)
		BEGIN
			SET IDENTITY_INSERT ProcessAgent_Type ON
			INSERT INTO ProcessAgent_Type(ProcessAgent_Type_ID, Short_Name, Description) VALUES (129,'AUTH', 'AUTHORIZATION SERVICE');
			UPDATE ProcessAgent set ProcessAgent_Type_ID = 129 Where ProcessAgent_Type_ID = 128
			DELETE from ProcessAgent_Type where processAgent_Type_ID = 128
			SET IDENTITY_INSERT ProcessAgent_Type OFF
		END
	END
ELSE
	BEGIN
		SET IDENTITY_INSERT ProcessAgent_Type ON
		INSERT INTO ProcessAgent_Type(ProcessAgent_Type_ID, Short_Name, Description) VALUES (129,'AUTH', 'AUTHORIZATION SERVICE');
		SET IDENTITY_INSERT ProcessAgent_Type OFF
	END

if NOT EXISTS (select Name from Ticket_Type where Name  = 'AUTHORIZE ACCESS')
	BEGIN
		SET IDENTITY_INSERT Ticket_Type ON
		INSERT INTO Ticket_Type(Ticket_Type_ID, Name, Short_Description, Abstract) VALUES (28,'AUTHORIZE ACCESS','Authorize Access', 0);
		SET IDENTITY_INSERT Ticket_Type OFF
		DBCC CHECKIDENT (Ticket_Type, RESEED);
END

if NOT EXISTS (select Name from Ticket_Type where Name  = 'AUTHORIZE CLIENT')
	BEGIN
		SET IDENTITY_INSERT Ticket_Type ON
		INSERT INTO Ticket_Type(Ticket_Type_ID, Name, Short_Description, Abstract) VALUES (29,'AUTHORIZE CLIENT','Authorize Access', 0);
		SET IDENTITY_INSERT Ticket_Type OFF
		DBCC CHECKIDENT (Ticket_Type, RESEED);
	END

if NOT EXISTS (select Name from Ticket_Type where Name  = 'LAUNCH CLIENT')
	BEGIN
		SET IDENTITY_INSERT Ticket_Type ON
		INSERT INTO Ticket_Type(Ticket_Type_ID, Name, Short_Description, Abstract) VALUES (30,'LAUNCH CLIENT','Authorize Access', 0);
		SET IDENTITY_INSERT Ticket_Type OFF
		DBCC CHECKIDENT (Ticket_Type, RESEED);
	END