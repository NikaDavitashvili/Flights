using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Infrastructure.Common;
using System.Data;

namespace FinalProject.Infrastructure.Repositories;
public class PassengerRepository : IPassengerRepository
{
    public async Task<Dictionary<int, string>> AddPassenger(PassengerDTO passenger)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Email", passenger.Email },
            { "PasswordHash", passenger.PasswordHash },
            { "UserName", passenger.UserName },
            { "FirstName", passenger.FirstName },
            { "LastName", passenger.LastName },
            { "Gender", passenger.Gender },
            { "PacketID", passenger.PacketID },
            { "PurchasePercent", passenger.PurchasePercent },
            { "CancelPercent", passenger.CancelPercent },
        };

        var query = @"
        INSERT INTO Passengers (Email, PasswordHash, UserName, FirstName, LastName, Gender, PacketID, PurchasePercent, CancelPercent)
        VALUES (@Email, @PasswordHash, @UserName, @FirstName, @LastName, @Gender, @PacketID, @PurchasePercent, @CancelPercent)";

        Dictionary<int, string> result = DB.Run(query, parameters);

        return result;
    }

    public async Task<PassengerDTO> GetPassengerByEmailAndPassword(string email, string passwordHash)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Email", email },
            { "PasswordHash", passwordHash }
        };

        var query = @"
            SELECT Email, PasswordHash, UserName, FirstName, LastName, Gender, PacketID, PurchasePercent, CancelPercent
            FROM Passengers psg
            INNER JOIN Packets pck on psg.PacketID = pck.Id
            WHERE Email = @Email AND PasswordHash = @PasswordHash";

        string errorMessage;
        DataTable dt = DB.Select(query, parameters, out errorMessage);

        if (errorMessage != null)
        {
            throw new Exception(errorMessage);
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            return null;
        }

        var row = dt.Rows[0];

        return new PassengerDTO(
            row["Email"].ToString(),
            row["PasswordHash"].ToString(),
            row["UserName"].ToString(),
            row["FirstName"].ToString(),
            row["LastName"].ToString(),
            row["Gender"].ToString(),
            Convert.ToInt32(row["PacketID"]),
            Convert.ToInt32(row["PurchasePercent"]),
            Convert.ToInt32(row["CancelPercent"])
        );
    }
}
