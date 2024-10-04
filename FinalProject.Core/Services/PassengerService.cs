using System.Security.Cryptography;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.DTOs;
using Amazon.SimpleEmail;
using Amazon;
using Amazon.SimpleEmail.Model;
using Amazon.Runtime;
using FinalProject.Domain.Common;

public class PassengerService : IPassengerService
{
    private readonly IPassengerRepository _passengerRepository;
    private readonly IHelper _helper;

    // For simplicity, store tokens in memory, but you should store them in the database
    private static Dictionary<string, string> _emailVerificationTokens = new Dictionary<string, string>();

    public PassengerService(IPassengerRepository passengerRepository, IHelper helper)
    {
        _passengerRepository = passengerRepository;
        _helper = helper;
    }

    public async Task Register(NewPassengerDTO dto)
    {
        var existingEmail = await _passengerRepository.FindPassengerByEmail(dto.Email.Trim());
        var existingUsername = await _passengerRepository.FindPassengerByUsername(dto.UserName);

        if (existingEmail != null && existingUsername != null)
            throw new Exception("This email and username is already registered!");
        if (existingEmail != null)
            throw new Exception("This email is already registered!");
        if (existingUsername != null)
            throw new Exception("This username is already registered!");

        var passwordHash = _helper.PasswordHash(dto.Password);
        var gender = dto.Gender ? "Female" : "Male";
        int packetId = 1;
        int purchasePercent = 0;
        int cancelPercent = 0;

        // Create passenger with IsVerified = false
        var passenger = new PassengerDTO(
            dto.Email.Trim(),
            passwordHash,
            dto.UserName,
            dto.FirstName,
            dto.LastName,
            gender,
            packetId,
            purchasePercent,
            cancelPercent,
            false  // Not verified yet
        );

        var result = await _passengerRepository.AddPassenger(passenger);

        if (result.Values.Any(message => message.StartsWith("The input string '")))
        {
            throw new Exception("Registration Failed!");
        }
    }

    public async Task<UserDTO> Login(string email, string password)
    {
        var passwordHash = _helper.PasswordHash(password);
        var passenger = await _passengerRepository.GetPassengerByEmailAndPassword(email, passwordHash);

        if (passenger == null)
        {
            throw new Exception("Passenger not found");
        }

        return new UserDTO(
            passenger.Email,
            passenger.PasswordHash,
            passenger.UserName,
            passenger.PacketID,
            passenger.PurchasePercent,
            passenger.CancelPercent,
            passenger.IsVerified
        );
    }

    public async Task<string> GenerateEmailVerificationToken(string email)
    {
        string token = GenerateRandomToken();

        // Set an expiration time for the token (e.g., 24 hours from now)
        DateTime expiryDate = DateTime.UtcNow.AddHours(24);
        await _passengerRepository.StoreEmailVerificationToken(token, email, expiryDate);

        return token;
    }

    // Send verification email (this is a mock, replace with actual email sending logic)
    public async Task SendVerificationEmail(string email, string userName, string verificationUrl)
    {
        var credentials = new BasicAWSCredentials("AKIAYEKP52OWFM437AOL", "ikgnhixZp8MmcFkrhdn2Kaz9Af+X4sdrRJbzgNO9");
        var client = new AmazonSimpleEmailServiceClient(credentials, RegionEndpoint.EUNorth1); // Set your region

        // Create the email request
        var sendRequest = new SendEmailRequest
        {
            Source = "skyconnect463@gmail.com",
            Destination = new Destination
            {
                ToAddresses = new List<string> { email }
            },
            Message = new Message
            {
                Subject = new Content($"Email Verification for '{userName}'"),
                Body = new Body
                {
                    Html = new Content
                    {
                        Charset = "UTF-8",
                        Data = $"<html><body><p>Hello {userName},</p><p>Please verify your email by clicking the link below:</p><p><a href='{verificationUrl}'>Verify Email</a></p></body></html>"
                    },
                    Text = new Content
                    {
                        Charset = "UTF-8",
                        Data = $"Hello {userName},\n\nPlease verify your email by clicking the link: {verificationUrl}"
                    }
                }
            }
        };

        try
        {
            // Send the email
            var response = await client.SendEmailAsync(sendRequest);
        }
        catch (MessageRejectedException ex)
        {
            // Handle message rejected exception (invalid email, etc.)
            throw new MessageRejectedException("Email was rejected. Error message: " + ex.Message);
        }
        catch (AmazonSimpleEmailServiceException ex)
        {
            // General SES errors (e.g., access denied, incorrect region, etc.)
            throw new AmazonSimpleEmailServiceException($"Error sending email. StatusCode: {ex.StatusCode}, Message: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred when sending the email: " + ex.Message);
        }
    }

    public async Task<bool> VerifyEmailToken(string token)
    {
        // Retrieve the email associated with the token from the database
        var email = await _passengerRepository.GetEmailByVerificationToken(token);

        if (string.IsNullOrEmpty(email))
        {
            return false; // Token invalid or expired
        }

        var passenger = await _passengerRepository.FindPassengerByEmail(email);
        if (passenger == null)
        {
            return false; // Passenger not found
        }

        // Mark the passenger as verified
        await _passengerRepository.VerifyPassenger(email);

        // Remove the token after it's used
        await _passengerRepository.RemoveEmailVerificationToken(token);

        return true;
    }

    // Helper method to generate a random token (this could be replaced with JWT if needed)
    private string GenerateRandomToken()
    {
        using (var cryptoProvider = new RNGCryptoServiceProvider())
        {
            byte[] tokenBytes = new byte[64];
            cryptoProvider.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }
    }
}