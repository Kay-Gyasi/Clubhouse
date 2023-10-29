namespace Clubhouse.Business.Services.Interfaces;

public interface IInitializationService
{
    Task InitializeDbAsync();
}