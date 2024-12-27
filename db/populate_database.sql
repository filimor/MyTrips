INSERT INTO Clients (Name, Email) VALUES
('Alice Johnson', 'alice.johnson@example.com'),
('Bob Smith', 'bob.smith@example.com'),
('Charlie Brown', 'charlie.brown@example.com'),
('Diana Prince', 'diana.prince@example.com'),
('Evan Williams', 'evan.williams@example.com');

GO

INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureDateTime, ArrivalDateTime, Price) VALUES
('FL123', 'JFK', 'LAX', CONVERT(DATETIME, '2024-01-10 08:00:00', 120), CONVERT(DATETIME, '2024-01-10 11:00:00', 120), 350.00),
('FL456', 'LAX', 'ORD', CONVERT(DATETIME, '2024-01-15 12:00:00', 120), CONVERT(DATETIME, '2024-01-15 18:00:00', 120), 450.00),
('FL789', 'ORD', 'ATL', CONVERT(DATETIME, '2024-02-05 10:30:00', 120), CONVERT(DATETIME, '2024-02-05 14:30:00', 120), 300.00),
('FL012', 'ATL', 'DFW', CONVERT(DATETIME, '2024-02-20 09:00:00', 120), CONVERT(DATETIME, '2024-02-20 11:30:00', 120), 280.00),
('FL345', 'DFW', 'SEA', CONVERT(DATETIME, '2024-03-03 14:00:00', 120), CONVERT(DATETIME, '2024-03-03 17:30:00', 120), 400.00);


GO

INSERT INTO Destinations (Name) VALUES
('New York'),
('Los Angeles'),
('Chicago'),
('Atlanta'),
('Dallas'),
('Seattle');

GO

INSERT INTO Hotels (Name, Rating, DestinationId, Price) VALUES
('Grand Central Hotel', 5, 1, 250.00),
('Sunset Boulevard Inn', 4, 2, 180.00),
('Magnificent Mile Suites', 3, 3, 150.00),
('Peachtree Plaza', 4, 4, 200.00),
('Lone Star Lodge', 5, 5, 220.00),
('Emerald City Hostel', 2, 6, 100.00);

GO

INSERT INTO Trips (StartDate, EndDate, ClientId, InboundFlightId, OutboundFlightId, HotelId) VALUES
('2024-04-01', '2024-04-10', 1, 1, 2, 1),
('2024-05-15', '2024-05-20', 2, 3, 4, 2),
('2024-06-05', NULL, 3, 5, 5, 3),
('2024-07-20', '2024-07-25', 4, 2, 1, 4),
('2024-08-10', '2024-08-15', 5, 4, 3, 5);

GO