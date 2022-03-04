namespace BlazorGrpcWebApp.Shared.Entities
{
    public class Unit
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int HitPoints { get; set; } = 100;
        public int BananaCost { get; set; }
    }
}
