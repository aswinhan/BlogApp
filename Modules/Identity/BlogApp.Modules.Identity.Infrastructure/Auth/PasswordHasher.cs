namespace BlogApp.Modules.Identity.Infrastructure.Auth;

public sealed class PasswordHasher : IPasswordHasher
{
    // Security Configuration
    // These settings make it computationally expensive for attackers to guess passwords
    private const int TimeCost = 2;          // Number of iterations (Higher = Slower/More Secure)
    private const int MemoryCost = 65536;    // 64MB of Memory usage (Stops GPU cracking)
    private const int Parallelism = 1;       // Number of threads
    private const int SaltSize = 16;         // 128-bit Salt
    private const int HashLength = 32;       // 256-bit Hash

    public string Hash(string password)
    {
        // 1. Generate a random cryptographic salt
        byte[] salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);

        // 2. Configure Argon2id
        var config = new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing, // Argon2i (Resists Side-Channel Attacks)
            Version = Argon2Version.Nineteen,
            TimeCost = TimeCost,
            MemoryCost = MemoryCost,
            Lanes = Parallelism,
            Threads = Parallelism,
            Password = System.Text.Encoding.UTF8.GetBytes(password),
            Salt = salt,
            HashLength = HashLength
        };

        // 3. Create the hash
        var argon2 = new Argon2(config);
        string hashString = config.EncodeString(argon2.Hash().Buffer);

        return hashString;
    }

    public bool Verify(string password, string passwordHash)
    {
        // Argon2 verifies the hash by decoding the string which contains the salt and config
        return Argon2.Verify(passwordHash, password);
    }
}