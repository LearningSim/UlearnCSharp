using System;
using System.Globalization;
using System.Linq;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
    // In real aplication it whould be the place where database is used to find driver by its Id.
    // But in this exercise it is just a mock to simulate database
    public class DriversRepository
    {
        public Driver FindDriver(int driverId)
        {
            if (driverId != 15) throw new Exception("Unknown driver id " + driverId);
            
            var car = new Car("Baklazhan", "Lada sedan", "A123BT 66");
            return new Driver(driverId, new PersonName("Drive", "Driverson"), car);
        }
    }

    public class TaxiApi : ITaxiApi<TaxiOrder>
    {
        private readonly DriversRepository driversRepo;
        private readonly Func<DateTime> currentTime;
        private int idCounter;

        public TaxiApi(DriversRepository driversRepo, Func<DateTime> currentTime)
        {
            this.driversRepo = driversRepo;
            this.currentTime = currentTime;
        }

        public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName, string street,
            string building)
        {
            var client = new PersonName(firstName, lastName);
            var start = new Address(street, building);
            return TaxiOrder
                .CreateWithoutDestination(driversRepo, idCounter++, client, start, currentTime());
        }

        public void UpdateDestination(TaxiOrder order, string street, string building) =>
            order.UpdateDestination(new Address(street, building));

        public void AssignDriver(TaxiOrder order, int driverId) => order.AssignDriver(driverId, currentTime());
        public void UnassignDriver(TaxiOrder order) => order.UnassignDriver();
        public string GetDriverFullInfo(TaxiOrder order) => order.GetDriverFullInfo();
        public string GetShortOrderInfo(TaxiOrder order) => order.GetShortOrderInfo();
        public void Cancel(TaxiOrder order) => order.Cancel(currentTime());
        public void StartRide(TaxiOrder order) => order.StartRide(currentTime());
        public void FinishRide(TaxiOrder order) => order.FinishRide(currentTime());
    }

    public class TaxiOrder : Entity<int>
    {
        private readonly int id;
        private readonly DriversRepository driversRepo;
        private TaxiOrderStatus status;
        public PersonName ClientName { get; private set; }
        public Address Start { get; private set; }
        public Address Destination { get; private set; }
        public Driver Driver { get; private set; }
        private DateTime creationTime;
        private DateTime driverAssignmentTime;
        private DateTime cancelTime;
        private DateTime startRideTime;
        private DateTime finishRideTime;

        private TaxiOrder(int id, DriversRepository driversRepo) : base(id)
        {
            this.id = id;
            this.driversRepo = driversRepo;
        }

        public static TaxiOrder CreateWithoutDestination(DriversRepository driversRepo, int id, PersonName client,
            Address start, DateTime creationTime) =>
            new TaxiOrder(id, driversRepo)
            {
                ClientName = client,
                Start = start,
                creationTime = creationTime
            };

        public void UpdateDestination(Address address)
        {
            Destination = address;
        }

        public void AssignDriver(int driverId, DateTime time)
        {
            if (Driver != null)
            {
                throw new InvalidOperationException("driver already assigned");
            }

            Driver = driversRepo.FindDriver(driverId);
            driverAssignmentTime = time;
            status = TaxiOrderStatus.WaitingCarArrival;
        }

        public void UnassignDriver()
        {
            if (Driver == null)
            {
                throw new InvalidOperationException(status.ToString());
            }

            if (status == TaxiOrderStatus.InProgress)
            {
                throw new InvalidOperationException(status.ToString());
            }

            Driver = null;
            status = TaxiOrderStatus.WaitingForDriver;
        }

        public string GetDriverFullInfo() => status == TaxiOrderStatus.WaitingForDriver ? null : Driver?.GetFullInfo();

        public string GetShortOrderInfo()
        {
            return string.Join(" ",
                "OrderId: " + id,
                "Status: " + status,
                "Client: " + ClientName?.Format(),
                "Driver: " + Driver?.Name.Format(),
                "From: " + Start?.Format(),
                "To: " + Destination?.Format(),
                "LastProgressTime: " + GetLastProgressTime()
                    .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }

        private DateTime GetLastProgressTime()
        {
            if (status == TaxiOrderStatus.WaitingForDriver) return creationTime;
            if (status == TaxiOrderStatus.WaitingCarArrival) return driverAssignmentTime;
            if (status == TaxiOrderStatus.InProgress) return startRideTime;
            if (status == TaxiOrderStatus.Finished) return finishRideTime;
            if (status == TaxiOrderStatus.Canceled) return cancelTime;
            throw new NotSupportedException(status.ToString());
        }

        public void Cancel(DateTime time)
        {
            if (status == TaxiOrderStatus.InProgress)
            {
                throw new InvalidOperationException(status.ToString());
            }

            status = TaxiOrderStatus.Canceled;
            cancelTime = time;
        }

        public void StartRide(DateTime time)
        {
            if (Driver == null)
            {
                throw new InvalidOperationException("driver is not assigned");
            }

            status = TaxiOrderStatus.InProgress;
            startRideTime = time;
        }

        public void FinishRide(DateTime time)
        {
            if (Driver == null)
            {
                throw new InvalidOperationException("driver is not assigned");
            }

            if (status != TaxiOrderStatus.InProgress)
            {
                throw new InvalidOperationException(status.ToString());
            }

            status = TaxiOrderStatus.Finished;
            finishRideTime = time;
        }
    }

    public class Driver : Entity<int>
    {
        public int Id { get; }
        public PersonName Name { get; }
        public Car Car { get; }

        public Driver(int id, PersonName name, Car car) : base(id)
        {
            Id = id;
            Name = name;
            Car = car;
        }

        public string GetFullInfo()
        {
            return string.Join(" ",
                "Id: " + Id,
                "DriverName: " + Name.Format(),
                "Color: " + Car?.Color,
                "CarModel: " + Car?.Model,
                "PlateNumber: " + Car?.PlateNumber);
        }
    }

    public class Car : ValueType<Car>
    {
        public string Color;
        public string Model;
        public string PlateNumber;

        public Car(string color, string model, string plateNumber)
        {
            Color = color;
            Model = model;
            PlateNumber = plateNumber;
        }
    }

    public static class Extensions
    {
        public static string Format(this PersonName personName) =>
            string.Join(" ", new[] { personName.FirstName, personName.LastName }.Where(n => n != null));

        public static string Format(this Address address) =>
            string.Join(" ", new[] { address.Street, address.Building }.Where(n => n != null));
    }
}