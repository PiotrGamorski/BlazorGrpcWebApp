using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;

namespace BlazorGrpcWebApp.Server.EntitySeeder
{
    public interface ISeeder
    {
        void Seed();
    }
    public class EntitySeeder : ISeeder
    {
        private readonly DataContext _dBContext;

        public EntitySeeder(DataContext dBContext)
        {
            _dBContext = dBContext;
        }

        public void Seed()
        {
            if (_dBContext.Database.CanConnect())
            {
                if (!_dBContext.Units.Any())
                {
                    var units = GetUnits();
                    _dBContext.Units.AddRange(units);
                    _dBContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Unit> GetUnits()
        {
            return new List<Unit>()
            {
                new Unit()
                {
                    Title = "Knight",
                    BananaCost = 100,
                    HitPoints = 100,
                    Attack = 10,
                    Defense = 10,
                },
                new Unit()
                {
                    Title = "Archer",
                    BananaCost = 100,
                    HitPoints = 100,
                    Attack = 15,
                    Defense = 15,
                },
                new Unit()
                {
                    Title = "Mage",
                    BananaCost = 100,
                    HitPoints = 100,
                    Attack = 20,
                    Defense = 1,
                }
            };
        }
    }
}
