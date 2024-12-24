INSERT INTO Clients (Id, Name, Email) VALUES
(1, 'Alice Johnson', 'alice.johnson@example.com'),
(2, 'Bob Smith', 'bob.smith@example.com'),
(3, 'Charlie Brown', 'charlie.brown@example.com'),
(4, 'Diana Prince', 'diana.prince@example.com'),
(5, 'Evan Williams', 'evan.williams@example.com');

GO

INSERT INTO Flights (Id, FlightNumber, DepartureAirport, ArrivalAirport, DepartureDateTime, ArrivalDateTime, Price) VALUES
(1, 'FL123', 'JFK', 'LAX', '2024-01-10 08:00:00', '2024-01-10 11:00:00', 350.00),
(2, 'FL456', 'LAX', 'ORD', '2024-01-15 12:00:00', '2024-01-15 18:00:00', 450.00),
(3, 'FL789', 'ORD', 'ATL', '2024-02-05 10:30:00', '2024-02-05 14:30:00', 300.00),
(4, 'FL012', 'ATL', 'DFW', '2024-02-20 09:00:00', '2024-02-20 11:30:00', 280.00),
(5, 'FL345', 'DFW', 'SEA', '2024-03-03 14:00:00', '2024-03-03 17:30:00', 400.00);

GO

INSERT INTO Destinations (Id, Name) VALUES
(1, 'New York'),
(2, 'Los Angeles'),
(3, 'Chicago'),
(4, 'Atlanta'),
(5, 'Dallas'),
(6, 'Seattle');

GO

INSERT INTO Hotels (Id, Name, Rating, DestinationId, Price) VALUES
(1, 'Grand Central Hotel', 5, 1, 250.00),
(2, 'Sunset Boulevard Inn', 4, 2, 180.00),
(3, 'Magnificent Mile Suites', 3, 3, 150.00),
(4, 'Peachtree Plaza', 4, 4, 200.00),
(5, 'Lone Star Lodge', 5, 5, 220.00),
(6, 'Emerald City Hostel', 2, 6, 100.00);

GO

INSERT INTO Trips (Id, StartDate, EndDate, ClientId, InboundFlightId, OutboundFlightId, HotelId) VALUES
(1, '2024-04-01', '2024-04-10', 1, 1, 2, 1),
(2, '2024-05-15', '2024-05-20', 2, 3, 4, 2),
(3, '2024-06-05', NULL, 3, 5, 6, 3),
(4, '2024-07-20', '2024-07-25', 4, 2, 1, 4),
(5, '2024-08-10', '2024-08-15', 5, 4, 3, 5);

GO