﻿using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Dtos;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IArmyService
    {
        Task<List<UserUnitResponse>> RestApiGetUserUnits();
        Task<HttpResponseMessage> RestApiReviveArmy();
        Task<GrpcReviveArmyResponse> DoGrpcReviveArmy();
        Task<GrpcHealUnitResponse> DoGrpcHealUnit(int userUnitId);
    }
}
