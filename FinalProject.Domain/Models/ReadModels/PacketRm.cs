namespace FinalProject.Domain.Models.ReadModels;
public record PacketRm(
    int Id,
    string Name,
    double Price1,
    double Price3,
    double Price6,
    double Price12,
    int PurchasePercent,
    int CancelPercent
);
