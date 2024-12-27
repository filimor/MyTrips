CREATE TABLE Clients (
    Id INT NOT NULL PRIMARY KEY  IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Flights (
    Id INT NOT NULL PRIMARY KEY  IDENTITY,
    FlightNumber NVARCHAR(100) NOT NULL,
    DepartureAirport NVARCHAR(100) NOT NULL,
    ArrivalAirport NVARCHAR(100) NOT NULL,
    DepartureDateTime DATETIME NOT NULL,
    ArrivalDateTime DATETIME NOT NULL,
    Price DECIMAL(18, 2) NOT NULL
);

CREATE TABLE Destinations (
    Id INT NOT NULL PRIMARY KEY  IDENTITY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Hotels (
    Id INT NOT NULL PRIMARY KEY  IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Rating INT NULL,
    DestinationId INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    CONSTRAINT FK_Hotels_Destinations FOREIGN KEY (DestinationId) REFERENCES Destinations(Id)
);

CREATE TABLE Trips (
    Id INT NOT NULL PRIMARY KEY  IDENTITY,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    ClientId INT NOT NULL,
    InboundFlightId INT NOT NULL,
    OutboundFlightId INT NOT NULL,
    HotelId INT NOT NULL,
    CONSTRAINT FK_Trips_Clients FOREIGN KEY (ClientId) REFERENCES Clients(Id),
    CONSTRAINT FK_Trips_InboundFlights FOREIGN KEY (InboundFlightId) REFERENCES Flights(Id),
    CONSTRAINT FK_Trips_OutboundFlights FOREIGN KEY (OutboundFlightId) REFERENCES Flights(Id),
    CONSTRAINT FK_Trips_Hotels FOREIGN KEY (HotelId) REFERENCES Hotels(Id)
);