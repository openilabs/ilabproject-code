
SET IDENTITY_INSERT ProcessAgent_Type ON

INSERT INTO ProcessAgent_Type(ProcessAgent_Type_ID, Short_Name, Description) VALUES (0,' GPA', 'GENERIC PA');
INSERT INTO ProcessAgent_Type(ProcessAgent_Type_ID, Short_Name, Description) VALUES (1,'NOTA', 'NOT A PA');
INSERT INTO ProcessAgent_Type(ProcessAgent_Type_ID, Short_Name, Description) VALUES (128,'AUTH', 'AUTHORIZATION SERVICE');

SET IDENTITY_INSERT ProcessAgent_Type OFF