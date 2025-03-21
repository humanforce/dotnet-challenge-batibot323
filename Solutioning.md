# Overview and Startup
This is to run you through my thought processes as I'm solving this problem. Look at Appendix - Setup for how to set up this project. I've also added the postman collection, titled `hf-appointment-scheduler.postman_collection.json`.

## Setup

### Application
1. install nuget
2. rebuild solution
3. set iis for debugging
4. run in local

### Database
1. install local sql
2. use connection string for that, in this case create a database with name, `humanforce-scheduling`
3. run the sql migration script

## SQL Migration
```sql
-- Drop existing tables if they exist (in reverse order of creation to handle foreign key constraints)
IF OBJECT_ID('dbo.Appointments', 'U') IS NOT NULL
    DROP TABLE dbo.Appointments;

IF OBJECT_ID('dbo.Doctors', 'U') IS NOT NULL
    DROP TABLE dbo.Doctors;

IF OBJECT_ID('dbo.Patients', 'U') IS NOT NULL
    DROP TABLE dbo.Patients;

-- Create tables with appropriate indices
CREATE TABLE Patients (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    PhoneNumber NVARCHAR(15) NOT NULL,
    EmailAddress NVARCHAR(100) NOT NULL
);

CREATE TABLE Doctors (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Specialty NVARCHAR(100) NOT NULL
);
CREATE INDEX IDX_Doctors_Specialty ON Doctors(Specialty);

CREATE TABLE Appointments (
    ID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    FOREIGN KEY (PatientID) REFERENCES Patients(ID),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(ID)
);
CREATE INDEX IDX_Appointments_PatientID ON Appointments(PatientID);
CREATE INDEX IDX_Appointments_DoctorID ON Appointments(DoctorID);
-- not sure about indexing the time. i'm still thinking there should be a way to lock later a one-hour block if you're scheduling for a doctor so we don't do double booking if multiple nodes try to write to the same timeblock concurrently.

-- adapt to your pk
-- Drop the existing primary key constraint and clustered index
ALTER TABLE Appointments DROP CONSTRAINT PK__Appointm__3214EC27CF9E4449;

-- Create a new clustered index on the StartDate column
-- CREATE CLUSTERED INDEX IDX_Appointments_StartDate ON Appointments(StartDate);

-- Recreate the primary key constraint as a non-clustered index
ALTER TABLE Appointments ADD CONSTRAINT PK_Appointments PRIMARY KEY NONCLUSTERED (ID);
```

## SQL Data
```sql
insert into Doctors ([name], specialty) values('john doe', 'im');
insert into Doctors ([name], specialty) values('martin yamz', 'ob');
insert into Patients ([name], DateOfBirth, PhoneNumber, EmailAddress) values('jane doe', '1998-02-07', '+639201234123', 'jane.doe@gmail.com');
insert into Patients ([name], DateOfBirth, PhoneNumber, EmailAddress) values('marsha smith', '1998-10-17', '+639201234123', 'marsha.smith@gmail.com');
insert into Appointments (DoctorID, PatientID, StartDate, EndDate, [Status]) values('1', '1', '2025-03-16 09:00', '2025-03-16 09:30', 'SCHEDULED');
insert into Appointments (DoctorID, PatientID, StartDate, EndDate, [Status]) values('1', '2', '2025-03-17 09:00', '2025-03-17 09:30', 'SCHEDULED');
insert into Appointments (DoctorID, PatientID, StartDate, EndDate, [Status]) values('1', '1', '2025-03-18 23:30', '2025-03-19 00:30', 'SCHEDULED');
```

# Design
- used clean architecture 
  - but most of my entities are anemic classes because it needs to be incorporated with a service to access the repo layer
- decided to use `/doctor/{id}/appointments` and `/patients/{id}/appointments` instead of `/appointments` because it adheres to rest and more extensible
- used ef core but built the db tables using sql
- i kept the crud operations in the domain repo interfaces and repo layer, but i marked them for cleanup because they're not necessary based on the requirements
- i consider a 23:30 - 00:30 appointment to be included in both days
- clustered index on appointments start date because we usually query with a date range
  - however not sure how this works with our query not only looking at start date but also end date

## Bonus
- for authentication, use api key if main api user is businesses. if persons, use oauth2 where they have to do user + password.
- use authentication to limit access to doctors or patients, need to add merchantid or personid field to these db tables to know if possible to get.

## Initial Thoughts
Scheduling appointments seem to be straightforward but now my initial hard think is how small I want the time resolution be. Can we assume appointments are in 15-minute blocks? Google Calendar actually allows you to specify a time down to the minute. The main problem here is to solve scheduling conflicts and how to find present a doctor's available time slots.

I've also read through the _System Design Considerations_ and this tells me that at least I don't have to implement answers to this in this time boxed exercise. First thing that comes to mind is handling concurrent writes from multiple nodes (as we scale), as we create an appointment. Nodes 1 and 2 may see that the doctor is available at 9:00 AM and both may create an appointment at the same time, initial solution is to do db locking of timeslots for that hour.

## Disclaimer
I'll be using GitHub Copilot as it's a tool meant to be used to aid us in our work. Plus the interview with Lachlan and Mike seem to point out that AI tools are meant to be used.

## Thoughts
- Should the entities be what it should exactly look like in the db? Does this mean that Doctor's Available Time Slots (ATS) would be normalized? No, it shouldn't exist in the DB as ATS would be dependent on the input date and it's not worth saving into the DB. Approach would be that the entity Doctor shall have ATS as property but it's not required but we can populate when necessary.
- designing the API based off the initial requirements, i'd say top two domains are appointments and patients. even for bonus points, there's no requirement to fetch a list of doctors or filter by specialty. i can see a need for the future but for now, let's limit to appointments and patients. on hindsight: just need `appointments` api for now, actually.
  - Create, update, and cancel an appointment - `POST /appointments`, `PUT /appointments/{id}`, `DELETE /appointments/{id}`
  - Get available time slots for a specific doctor in a given specific date. - `GET /doctor/{doctorId}/available?date={date}`
  - Retrieve a patient�s appointment history - `GET /patient/{patientId}/appointments`
  - Get a daily schedule of appointments for a doctor - `GET doctor/{doctorId}/appointments/?date={date}`
  - Generate a summary report of appointments by status (e.g., total scheduled, completed, cancelled) for a given time period - `GET /appointments/summary?startDate={startDate}&endDate={endDate}`
- i�m wrinkling my brain how to approach the api design, do i just center this around appointments for now?? GET /appointments/doctor/{doctorId}/available?date={date} or GET /doctor/{doctorId}/appointments/available?date={date} but if it's too hard to think about, there seems to be not much difference whether you pick one or the other. can even go implement both endpoints that point to the same code behind the scenes.
- better to just use `GET /doctor/{doctorId}/appointments/available?date={date}` https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design

## Implementing
- done with scaffolding for repo, domain layer - entities, service, and repo interfaces.
- for repo layer methods that aren't directly needed by the required endpoints, i can just mark them for now as not necessary. but they'll likely be needed to do data setup.
- for now, i just do two db trips to create an appointment, check if there's a conflict, then only i proceed to actual creation once proved conflict-free. supposedly, we plan to do this in one trip to the db. for horizontal scaling, it will do the locking of db rows and inserting data.
- decided to forego heroku and just demo in my local. used local sql, postman, and ef core for repo, but with me creating the db migration myself as it's the easiest for me.

## TODO
- [X] review the repo layer, could be lots of default implementations that's unnecessary.
- [X] edit the presentation layer (api), `AppointmentScheduler`
- [X] try to start e2e debug flow using api + postman + local debugging
- [X] connect to db layer
- [X] unit tests
- [X] `POST /appointments`
- [X] `PUT /appointments/{id}`
- [X] `DELETE /appointments/{id}`
- [X] `GET /doctor/{doctorId}/available?date={date}`
- [X] `GET /patient/{patientId}/appointments`
- [X] `GET doctor/{doctorId}/appointments/?date={date}`
- [X] `GET /appointments/summary?startDate={startDate}&endDate={endDate}`

# Appendix

## Setup
### Application
1. install nuget
2. rebuild solution
3. set iis for debugging
4. run in local

### Database
1. install local sql
2. use connection string for that, in this case create a database with name, `humanforce-scheduling`
3. run the sql migration script

## SQL Migration
```sql
-- Drop existing tables if they exist (in reverse order of creation to handle foreign key constraints)
IF OBJECT_ID('dbo.Appointments', 'U') IS NOT NULL
    DROP TABLE dbo.Appointments;

IF OBJECT_ID('dbo.Doctors', 'U') IS NOT NULL
    DROP TABLE dbo.Doctors;

IF OBJECT_ID('dbo.Patients', 'U') IS NOT NULL
    DROP TABLE dbo.Patients;

-- Create tables with appropriate indices
CREATE TABLE Patients (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    PhoneNumber NVARCHAR(15) NOT NULL,
    EmailAddress NVARCHAR(100) NOT NULL
);

CREATE TABLE Doctors (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Specialty NVARCHAR(100) NOT NULL
);
CREATE INDEX IDX_Doctors_Specialty ON Doctors(Specialty);

CREATE TABLE Appointments (
    ID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    FOREIGN KEY (PatientID) REFERENCES Patients(ID),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(ID)
);
CREATE INDEX IDX_Appointments_PatientID ON Appointments(PatientID);
CREATE INDEX IDX_Appointments_DoctorID ON Appointments(DoctorID);
-- not sure about indexing the time. i'm still thinking there should be a way to lock later a one-hour block if you're scheduling for a doctor so we don't do double booking if multiple nodes try to write to the same timeblock concurrently.

-- adapt to your pk
-- Drop the existing primary key constraint and clustered index
ALTER TABLE Appointments DROP CONSTRAINT PK__Appointm__3214EC27CF9E4449;

-- Create a new clustered index on the StartDate column
-- CREATE CLUSTERED INDEX IDX_Appointments_StartDate ON Appointments(StartDate);

-- Recreate the primary key constraint as a non-clustered index
ALTER TABLE Appointments ADD CONSTRAINT PK_Appointments PRIMARY KEY NONCLUSTERED (ID);
```

## SQL Data
```sql
insert into Doctors ([name], specialty) values('john doe', 'im');
insert into Doctors ([name], specialty) values('martin yamz', 'ob');
insert into Patients ([name], DateOfBirth, PhoneNumber, EmailAddress) values('jane doe', '1998-02-07', '+639201234123', 'jane.doe@gmail.com');
insert into Patients ([name], DateOfBirth, PhoneNumber, EmailAddress) values('marsha smith', '1998-10-17', '+639201234123', 'marsha.smith@gmail.com');
insert into Appointments (DoctorID, PatientID, StartDate, EndDate, [Status]) values('1', '1', '2025-03-16 09:00', '2025-03-16 09:30', 'SCHEDULED');
insert into Appointments (DoctorID, PatientID, StartDate, EndDate, [Status]) values('1', '2', '2025-03-17 09:00', '2025-03-17 09:30', 'SCHEDULED');
insert into Appointments (DoctorID, PatientID, StartDate, EndDate, [Status]) values('1', '1', '2025-03-18 23:30', '2025-03-19 00:30', 'SCHEDULED');
```