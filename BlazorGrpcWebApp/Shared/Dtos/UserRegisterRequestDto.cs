using BlazorGrpcWebApp.Shared.Models.UI_Models;

namespace BlazorGrpcWebApp.Shared.Dtos
{
    public class UserRegisterRequestDto
    {
        public UserRegister UserRegister { get; set; }
        public int StartUnitId { get; set; }
    }
}
