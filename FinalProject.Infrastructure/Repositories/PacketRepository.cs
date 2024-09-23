using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Infrastructure.Common;
using System.Data;
using System.Net.Sockets;

namespace FinalProject.Infrastructure.Repositories;
public class PacketRepository : IPacketRepository
{
    public async Task<IEnumerable<PacketRm>> GetPackets()
    {
        var dic = new Dictionary<string, object>(){};

        var query = @"SELECT Id, Name, Price1, Price3, Price6, Price12, PurchasePercent, CancelPercent FROM Packets Where Id <> 1";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);

        if (dt == null || dt.Rows.Count == 0)
            throw new Exception("Data Not Found!");

        var packets = new List<PacketRm>();

        foreach (DataRow row in dt.Rows)
        {
            var packet = new PacketRm(
                Convert.ToInt32(row["Id"].ToString()),
                row["Name"].ToString(),
                Convert.ToDouble(row["Price1"].ToString()),
                Convert.ToDouble(row["Price3"].ToString()),
                Convert.ToDouble(row["Price6"].ToString()),
                Convert.ToDouble(row["Price12"].ToString()),
                Convert.ToInt32(row["PurchasePercent"].ToString()),
                Convert.ToInt32(row["CancelPercent"].ToString())
            );

            packets.Add(packet);
        }

        return packets;
    }

    public async Task<int> GetCurrentPacketId(string userEmail)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Email", userEmail }
        };

        var query = @"SELECT PacketId FROM Passengers Where Email = @Email";

        DataTable dt = DB.Select(query, parameters, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);

        if (dt == null || dt.Rows.Count == 0)
            throw new Exception("Data Not Found!");

        int packetId = Convert.ToInt32(dt.Rows[0].ItemArray[0]);

        return packetId; 
    }

    public async Task<PacketRm> UpdatePassengerPacket(PacketDTO Packet)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Email", Packet.PassengerEmail },
            { "Name", Packet.Name },
            { "Months", Packet.ValidityMonths },
        };

        var query = @"UPDATE Passengers
                      SET PacketID = (SELECT Id FROM Packets WHERE Name = @Name), 
                          PacketStartDate = GETDATE(), PacketEndDate = DATEADD(MONTH, convert (int,@Months), GETDATE()) 
                      Where Email = @Email
                      SELECT Id, Name, PurchasePercent, CancelPercent FROM Packets WHERE Name = @Name";

        DataTable dt = DB.Select(query, parameters, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);

        if (dt == null || dt.Rows.Count == 0)
            throw new Exception("Data Not Found!");

        var packet = new PacketRm(
            Convert.ToInt32(dt.Rows[0]["Id"].ToString()),
            dt.Rows[0]["Name"].ToString(),
            0,0,0,0,
            Convert.ToInt32(dt.Rows[0]["PurchasePercent"].ToString()),
            Convert.ToInt32(dt.Rows[0]["CancelPercent"].ToString())
        );

        return packet;
    }
}
