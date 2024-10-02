using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Infrastructure.Common;
using System.Data;

namespace FinalProject.Infrastructure.Repositories;
public class PassengerRepository : IPassengerRepository
{
    public async Task StoreEmailVerificationToken(string token, string email, DateTime expiryDate)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Token", token },
            { "Email", email },
            { "ExpiryDate", expiryDate }
        };

        var query = @"
        INSERT INTO EmailVerificationTokens (Token, Email, ExpiryDate)
        VALUES (@Token, @Email, @ExpiryDate)";

        DB.Run(query, parameters);

        await Task.CompletedTask;
    }

    public async Task<string> GetEmailByVerificationToken(string token)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Token", token }
        };

        var query = @"
        SELECT Email
        FROM EmailVerificationTokens
        WHERE Token = @Token AND ExpiryDate > GETDATE()";  // Ensure token hasn't expired

        string errorMessage;
        DataTable dt = DB.Select(query, parameters, out errorMessage);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            throw new Exception(errorMessage);
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            return null;
        }

        return dt.Rows[0]["Email"].ToString();
    }

    public async Task RemoveEmailVerificationToken(string token)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Token", token }
        };

        var query = @"
        DELETE FROM EmailVerificationTokens
        WHERE Token = @Token";

        DB.Run(query, parameters);

        await Task.CompletedTask;
    }
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
            { "IsVerified", passenger.IsVerified },
        };

        var query = @"
        INSERT INTO Passengers (Email, PasswordHash, UserName, FirstName, LastName, Gender, PacketID, IsVerified)
        VALUES (@Email, @PasswordHash, @UserName, @FirstName, @LastName, @Gender, @PacketID, @IsVerified)";

        Dictionary<int, string> result = DB.Run(query, parameters);

        await Task.CompletedTask;

        return result;
    }

    public async Task<EmailDTO> FindPassengerByEmail(string passengerEmail)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Email", passengerEmail }
        };

        var query = @"
        SELECT TOP (1) [Email]
        FROM [Flights].[dbo].[Passengers]
        WHERE Email = @Email";

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

        await Task.CompletedTask;

        return new EmailDTO(
            row["Email"].ToString()
        );
    }

    public async Task<UsernameDTO> FindPassengerByUsername(string passengerUsername)
    {
        var parameters = new Dictionary<string, object>
        {
            { "UserName", passengerUsername }
        };

        var query = @"
        SELECT TOP (1) [UserName]
        FROM [Flights].[dbo].[Passengers]
        WHERE UserName = @UserName";

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

        await Task.CompletedTask;

        return new UsernameDTO(
            row["UserName"].ToString()
        );
    }

    public async Task<PassengerDTO> GetPassengerByEmailAndPassword(string email, string passwordHash)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Email", email },
            { "PasswordHash", passwordHash }
        };

        var query = @"
            SELECT Email, PasswordHash, UserName, FirstName, LastName, Gender, PacketID, PurchasePercent, CancelPercent, IsVerified
            FROM Passengers psg
            INNER JOIN Packets pck on psg.PacketID = pck.Id
            WHERE Email = @Email AND PasswordHash = @PasswordHash";

        string errorMessage;
        DataTable dt = DB.Select(query, parameters, out errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);

        if (dt == null || dt.Rows.Count == 0)
            return null;

        var row = dt.Rows[0];

        await Task.CompletedTask;

        return new PassengerDTO(
            row["Email"].ToString(),
            row["PasswordHash"].ToString(),
            row["UserName"].ToString(),
            row["FirstName"].ToString(),
            row["LastName"].ToString(),
            row["Gender"].ToString(),
            Convert.ToInt32(row["PacketID"]),
            Convert.ToInt32(row["PurchasePercent"]),
            Convert.ToInt32(row["CancelPercent"]),
            (bool)row["IsVerified"]
        );
    }

    public async Task VerifyPassenger(string passengerEmail)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Email", passengerEmail },
            { "IsVerified", true }
        };

        var query = @"
        UPDATE Passengers
        SET IsVerified = @IsVerified
        WHERE Email = @Email";

        DB.Run(query, parameters);

        await Task.CompletedTask;
    }
}
